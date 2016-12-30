using System;
using System.Linq;
using System.Reflection;
using DofusLab.Communication.Interfaces;
using DofusLab.Communication.Sockets;
using DofusLab.Protocol;

namespace DofusLab.Communication.Managers
{
    public class MessageHandlerManager<TC, TS, BP>
        where TC : AbstractClient<TC, TS, BP>, new()
        where TS : AbstractServer<TC, TS, BP>
        where BP : IBufferPool
    {
        public static void InitializeHandlers()
        {
            var handlersMethodsInfos = from t in Assembly.GetEntryAssembly().GetTypes()
                                        from m in t.GetMethods()
                                        where m.GetParameters().Length >= 2
                                              && m.Name.EndsWith("Handler")
                                              && m.GetParameters()[0].ParameterType == typeof(TC)
                                              && typeof(NetworkMessage).IsAssignableFrom(m.GetParameters()[1].ParameterType)
                                        select m;


            foreach (var handler in handlersMethodsInfos)
            {
                var packetType = handler.GetParameters()[1].ParameterType;
                var t = typeof(Messages<>).MakeGenericType(typeof(TC), typeof(TS), typeof(BP), packetType);
                t.GetMethod("Register").Invoke(null, new object[] { handler });
            }
        }

        public static class Messages<T> where T : NetworkMessage
        {
            public static Action<TC, T> Handler { get; private set; }

            public static void Register(MethodInfo mi)
            {
                Handler = (Action<TC, T>)mi.CreateDelegate(typeof(Action<TC, T>));
            }
        }

        public static void InvokeHandler<T>(TC client, T message) 
            where T : NetworkMessage
        {
            if(message != null)
            {
                var h = Messages<T>.Handler;
                if (h != null)
                    h(client, message);
                else
                    client.WriteServer($"Handler not found for message : {message}");
            }
            else
            {
                client.WriteServer("Received empty message");
            }
        }
    }
}
