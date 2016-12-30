using System.Net;
using DofusLab.Communication;
using DofusLab.Communication.Enums;
using DofusLab.Communication.Managers;
using DofusLab.Communication.Sockets;

namespace DofusLab.Bot.Proxy.Realm
{
    public class RealmServer : AbstractServer<RealmClient, RealmServer, BufferPool>
    {
        public RealmServer() : base("Realm", IPAddress.Loopback, 5554, 100, new BufferPool(4096, 25),  ServerType.Realm)
        {
            MessageHandlerManager<RealmClient, RealmServer, BufferPool>.InitializeHandlers();
        }
    }
}
