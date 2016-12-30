using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using DofusLab.Bot.Injector;
using EasyHook;

namespace DofusLab.Hooks
{
    public class Main : IEntryPoint
    {
        public static string ChannelName { get; private set; }

        private readonly RemoteInterface _interface;
        private LocalHook _createConnectHook;
        private List<IPAddress> _whitelist;

        public Main(RemoteHooking.IContext InContext, string InChannelName)
        {
            try
            {
                _interface = RemoteHooking.IpcConnectClient<RemoteInterface>(InChannelName);
                ChannelName = InChannelName;
                _interface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            }
            catch (Exception ex)
            {
                _interface.ErrorHandler(ex);
            }
        }

        public void Run(RemoteHooking.IContext InContext, string InChannelName)
        {

            try
            {
                _createConnectHook = LocalHook.Create(
                                    LocalHook.GetProcAddress("Ws2_32.dll", "connect"),
                                    new NativeSocketMethods.WinsockConnectDelegate(connect_Hooked),
                                    this);



                _createConnectHook.ThreadACL.SetExclusiveACL(new[] { 0 });

                _whitelist = new List<IPAddress>(4)
                {
                    IPAddress.Parse("213.248.126.37"),
                    IPAddress.Parse("213.248.126.38"),
                    IPAddress.Parse("213.248.126.39"),
                    IPAddress.Parse("213.248.126.40")
                };


            }
            catch (Exception ex)
            {
                _interface.ErrorHandler(ex);
            }

            RemoteHooking.WakeUpProcess();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private int connect_Hooked(IntPtr s, IntPtr addr, int addrsize)
        {
            var structure = Marshal.PtrToStructure<NativeSocketMethods.sockaddr_in>(addr);

            if (structure.sin_addr.S_addr > 0)
                _interface.Message(
                    $"<Hook> Tentative de connexion sur {new IPAddress(structure.sin_addr.S_addr)}:{structure.sin_port}");

            if (!_whitelist.Contains(new IPAddress(structure.sin_addr.S_addr))) return NativeSocketMethods.connect(s, addr, addrsize);
            var buffer = Marshal.AllocHGlobal(addrsize);
            var str = new NativeSocketMethods.sockaddr_in
            {
                sin_addr = { S_addr = NativeSocketMethods.inet_addr("127.0.0.1") },
                sin_port = NativeSocketMethods.htons(5555),
                sin_family = (short)NativeSocketMethods.AddressFamily.InterNetworkv4
            };
            Marshal.StructureToPtr(str, buffer, true);
            return NativeSocketMethods.connect(s, buffer, addrsize);
        }
    }
}
