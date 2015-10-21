using System.Collections.Generic;
using kino.Messaging;

namespace kino.Connectivity
{
    public class InternalRoute
    {
        public SocketIdentifier Socket { get; set; }
        public IEnumerable<MessageIdentifier> Messages { get; set; }
    }
}