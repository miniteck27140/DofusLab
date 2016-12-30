using System;
using static System.Console;

namespace DofusLab.Bot.Injector
{
    public class RemoteInterface : MarshalByRefObject
    {
        public void IsInstalled(int clientPid)
        {
            WriteLine($"<Hook> Injected to {clientPid}");
        }

        public void Message(object o, params object[] args)
        {
            WriteLine(o.ToString(), args);
        }

        public void ErrorHandler(Exception ex)
        {
            WriteLine(ex.ToString());
        }
    }
}
