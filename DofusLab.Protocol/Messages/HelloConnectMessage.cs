using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusLab.Protocol.Messages
{
    public class HelloConnectMessage : NetworkMessage
    {
        public override int Id => 3;
    }
}
