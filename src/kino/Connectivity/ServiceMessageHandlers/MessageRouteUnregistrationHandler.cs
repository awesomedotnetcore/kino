﻿using System.Linq;
using kino.Framework;
using kino.Messaging;
using kino.Messaging.Messages;
using kino.Sockets;

namespace kino.Connectivity.ServiceMessageHandlers
{
    public class MessageRouteUnregistrationHandler : IServiceMessageHandler
    {
        private readonly IExternalRoutingTable externalRoutingTable;

        public MessageRouteUnregistrationHandler(IExternalRoutingTable externalRoutingTable)
        {
            this.externalRoutingTable = externalRoutingTable;
        }

        public bool Handle(IMessage message, ISocket scaleOutBackendSocket)
        {
            var shouldHandle = IsUnregisterMessageRouting(message);
            if (shouldHandle)
            {
                var payload = message.GetPayload<UnregisterMessageRouteMessage>();
                externalRoutingTable.RemoveMessageRoute(payload
                                                            .MessageContracts
                                                            .Select(mh => new MessageIdentifier(mh.Version, mh.Identity)),
                                                        new SocketIdentifier(payload.SocketIdentity));
            }

            return shouldHandle;
        }

        private bool IsUnregisterMessageRouting(IMessage message)
            => Unsafe.Equals(UnregisterMessageRouteMessage.MessageIdentity, message.Identity);
    }
}