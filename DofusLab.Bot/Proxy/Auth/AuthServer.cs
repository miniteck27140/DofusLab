using System.Net;
using DofusLab.Communication;
using DofusLab.Communication.Enums;
using DofusLab.Communication.Managers;
using DofusLab.Communication.Sockets;

namespace DofusLab.Bot.Proxy.Auth
{
    public class AuthServer : AbstractServer<AuthClient, AuthServer, BufferPool>
    {
        public AuthServer() : base("Auth", IPAddress.Loopback, 5555, 100, new BufferPool(4096, 25), ServerType.Auth)
        {
            MessageHandlerManager<AuthClient, AuthServer, BufferPool>.InitializeHandlers();
        }
    }
}
