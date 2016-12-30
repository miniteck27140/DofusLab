using System;
using System.Runtime.Remoting;
using DofusLab.Bot.Injector;
using DofusLab.Bot.Proxy.Auth;
using DofusLab.Bot.Proxy.Realm;
using DofusLab.Protocol.Messages;
using EasyHook;

namespace DofusLab.Bot
{
    public class Program
    {
        static readonly string PathToInject =
            @"C:\Users\NONAME\Documents\Visual Studio 2015\Projects\DofusLab\DofusLab.Hooks\bin\Debug\DofusLab.Hooks.dll";

        static readonly string PathToOpen =
            @"F:\Ankama\Dofus\app\Dofus.exe";

        static string ChannelName;

        static int pId;

        public static void Main(string[] args)
        {
            try
            {
                var auth = new AuthServer();
                //var realm = new RealmServer();  --  Not handled yet

                RemoteHooking.IpcCreateServer<RemoteInterface>(ref ChannelName, WellKnownObjectMode.Singleton);
                RemoteHooking.CreateAndInject(PathToOpen, "", 0x00000004, InjectionOptions.DoNotRequireStrongName, PathToInject, PathToInject, out pId, ChannelName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

    }

    public class Handlers
    {
        public static void ProtocolRequiredHandler(AuthClient client, ProtocolRequired message)
        { }

        public static void HelloConnectMessageHandler(RealmClient client, HelloConnectMessage message)
        { }
    }
}
