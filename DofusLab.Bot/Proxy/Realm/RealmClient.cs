using DofusLab.Communication;
using DofusLab.Communication.Sockets;

namespace DofusLab.Bot.Proxy.Realm
{
    public class RealmClient : AbstractClient<RealmClient, RealmServer, BufferPool>
    {
        public override void OnCreate()
        {
            base.OnCreate();
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
