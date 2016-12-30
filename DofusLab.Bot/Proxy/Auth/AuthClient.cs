using System.Net;
using DofusLab.Communication;
using DofusLab.Communication.Sockets;

namespace DofusLab.Bot.Proxy.Auth
{
    public class AuthClient : AbstractClient<AuthClient, AuthServer, BufferPool>
    {
        public override void OnCreate()
        {
            base.OnCreate();
            var remoteAuthStr = "213.248.126.39:443".Split(':');

            RemoteSocket.Connect(new IPEndPoint(IPAddress.Parse(remoteAuthStr[0]), int.Parse(remoteAuthStr[1])));
            WriteServer("Forward activated ...");
        }

        public override void OnReceiveServer(byte[] data)
        {
            base.OnReceiveServer(data);
        }

        public override void OnReceiveClient(byte[] data)
        {
            base.OnReceiveClient(data);
        }
    }
}
