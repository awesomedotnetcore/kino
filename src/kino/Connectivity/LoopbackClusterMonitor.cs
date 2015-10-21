﻿using System.Collections.Generic;
using System.Linq;
using kino.Messaging;

namespace kino.Connectivity
{
    internal class LoopbackClusterMonitor : IClusterMonitor
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void RegisterSelf(IEnumerable<IMessageIdentifier> messageHandlers)
        {
        }

        public void RequestClusterRoutes()
        {
        }

        public void UnregisterSelf(IEnumerable<IMessageIdentifier> messageIdentifiers)
        {
        }

        public IEnumerable<SocketEndpoint> GetClusterMembers()
            => Enumerable.Empty<SocketEndpoint>();

        public void DiscoverMessageRoute(IMessageIdentifier messageIdentifier)
        {
        }
    }
}