using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DofusLab.Communication.Enums;
using DofusLab.Communication.Interfaces;
using DofusLab.Communication.Sockets.Extensions;
using static System.Console;

namespace DofusLab.Communication.Sockets
{
    public abstract class AbstractServer<TC, TS, BP>
        where TC : AbstractClient<TC, TS, BP>, new()
        where TS : AbstractServer<TC, TS, BP>
        where BP : IBufferPool
    {
        private readonly Socket _socket;
        private readonly Socket _remoteSocket;
        private readonly CancellationTokenSource _acceptCts;

        public ConcurrentBag<TC> Clients;

        public BP BufferPool { get; }

        public string Name { get; }
        public ServerType ServerType { get; }

        protected AbstractServer(string name, IPAddress ip, int port, int backlog, BP bufferPool, ServerType serverType)
        {
            Name = name;
            ServerType = serverType;

            Clients = new ConcurrentBag<TC>();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _remoteSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(ip, port));
            _socket.Listen(backlog);

            _acceptCts = new CancellationTokenSource();

            BufferPool = bufferPool;

            WriteHelper($"Server started successfully on {ip}:{port}");
            AcceptLoop();
        }

        private void AcceptLoop()
        {
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        for (;;)
                        {
                            var s = await _socket.AcceptAsync();
                            if (!OnAccept(s)) continue;
                            var tc = new TC { Socket = s, RemoteSocket = _remoteSocket, Server = (TS)this };
                            tc.Init();
                            Clients.Add(tc);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine(ex);
                    }
                }, _acceptCts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            using (_acceptCts)
            {
                _acceptCts.Cancel();
                //disable all clients.
                foreach (var c in Clients)
                    c.Dispose();
            }
            OnStop();
        }

        public virtual void OnStop() => WriteHelper("Server down (stop requested).");
        //handle debug etc..
        public void WriteHelper(string msg) => WriteLine($"<{Name}> {msg} ...");

        public virtual bool OnAccept(Socket client) => true;
    }
}
