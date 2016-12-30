using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusLab.Protocol
{
    public abstract class NetworkMessage
    {
        public abstract int Id { get; }
    }
}
