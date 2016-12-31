using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DofusLab.Communication.Interfaces;
using DofusLab.Communication.Sockets.Extensions;
using DofusLab.Core.IO.Readers;
using static System.Console;

namespace DofusLab.Communication.Sockets
{
    /*
     * TODO:
     * - Implement IDisposable
     * - Implement buffer pool
     */
    public abstract class AbstractClient<TC, TS, BP> : IDisposable
        where TC : AbstractClient<TC, TS, BP>, new()
        where TS : AbstractServer<TC, TS, BP>
        where BP : IBufferPool
    {
        public Socket Socket { get; internal set; }
        public Socket RemoteSocket { get; internal set; }

        private readonly CancellationTokenSource _receiveSourceClient;
        private readonly CancellationTokenSource _receiveSourceServer;
        public TS Server { get; set; }

        protected AbstractClient()
        {
            _receiveSourceClient = new CancellationTokenSource();
            _receiveSourceServer = new CancellationTokenSource();
        }

        public void WriteClient(string msg) => WriteLine($"<{Server.Name}:{Socket.RemoteEndPoint}> {msg} ...");
        public void WriteServer(string msg) => WriteLine($"<{Server.Name}:{RemoteSocket.RemoteEndPoint}> {msg} ...");

        public virtual void OnReceiveClient(byte[] data)
            => WriteClient($"{data.Length} bytes received from client");

        public virtual void OnReceiveServer(byte[] data)
            => WriteServer($"{data.Length} bytes received from server");

        public virtual void OnCreate() => WriteClient("Client created");

        internal void Init()
        {
            OnCreate();
            ReceiveLoopServer();
            ReceiveLoopClient();
        }

        private void ReceiveLoopClient()
        {
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var mBuilder = new MetadataBuilder();
                        for (;;)
                        {
                            using (var mb = Server.BufferPool.GetManagedBuffer(4096))
                            {
                                var readBytes =
                                    await RemoteSocket.ReceiveAsync(mb.Buffer, 0, mb.RealSize).ConfigureAwait(false);

                                if (readBytes < 1)
                                    DisconnectClient();

                                mb.Resize(readBytes);

                                bool isValid = mBuilder.Build(new DofusBinaryReader(mb.Buffer));

                                if(isValid)
                                    WriteClient($"Received message with ID : {mBuilder.MessageId}");

                                OnReceiveClient(mb.Buffer);
                                mBuilder = new MetadataBuilder();
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    { }

                }, _receiveSourceClient.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ReceiveLoopServer()
        {
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var mBuilder = new MetadataBuilder();
                        for (;;)
                        {
                            using (var mb = Server.BufferPool.GetManagedBuffer(4096))
                            {
                                var readBytes =
                                    await RemoteSocket.ReceiveAsync(mb.Buffer, 0, mb.RealSize).ConfigureAwait(false);
                                
                                if (readBytes < 1)
                                    DisconnectServer();

                                mb.Resize(readBytes);

                                bool isValid = mBuilder.Build(new DofusBinaryReader(mb.Buffer));

                                if (isValid)
                                    WriteClient($"Received message with ID : {mBuilder.MessageId}");

                                OnReceiveServer(mb.Buffer);
                                mBuilder = new MetadataBuilder();
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    { }

                }, _receiveSourceServer.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public virtual void DisconnectClient()
        {
            WriteClient($"Client {Socket.RemoteEndPoint} disconnected");
            _receiveSourceClient.Cancel();
            Socket.Close();
        }

        public virtual void DisconnectServer()
        {
            WriteServer($"Server {RemoteSocket.RemoteEndPoint} disconnected");
            _receiveSourceServer.Cancel();
            RemoteSocket.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Socket != null && RemoteSocket != null)
                {
                    DisconnectClient();
                    DisconnectServer();
                }
                Socket = null;
                RemoteSocket = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static byte ComputeTypeLen(int len)
        {
            if (len > ushort.MaxValue)
                return 3;
            if (len > byte.MaxValue)
                return 2;
            return len > 0 ? (byte)1 : (byte)0;
        }

        private static uint SubComputeStaticHeader(uint id, byte typeLen) => id << 2 | typeLen;
    }
}
