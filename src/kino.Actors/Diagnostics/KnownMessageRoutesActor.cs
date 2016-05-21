﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kino.Core.Connectivity;
using kino.Core.Framework;
using kino.Core.Messaging;
using kino.Core.Messaging.Messages;

namespace kino.Actors.Diagnostics
{
    public class KnownMessageRoutesActor : Actor
    {
        private readonly IExternalRoutingTable externalRoutingTable;
        private readonly IInternalRoutingTable internalRoutingTable;
        private readonly RouterConfiguration routerConfiguration;

        public KnownMessageRoutesActor(IExternalRoutingTable externalRoutingTable,
                                       IInternalRoutingTable internalRoutingTable,
                                       RouterConfiguration routerConfiguration)
        {
            this.externalRoutingTable = externalRoutingTable;
            this.internalRoutingTable = internalRoutingTable;
            this.routerConfiguration = routerConfiguration;
        }

        [MessageHandlerDefinition(typeof (RequestKnownMessageRoutesMessage))]
        private async Task<IActorResult> Handler(IMessage message)
            => new ActorResult(Message.Create(new KnownMessageRoutesMessage
                                              {
                                                  ExternalRoutes = GetExternalRoutes(),
                                                  InternalRoutes = GetInternalRoutes()
                                              }));

        private MessageRoute GetInternalRoutes()
            => new MessageRoute
               {
                   SocketIdentity = routerConfiguration.ScaleOutAddress.Identity,
                   Uri = routerConfiguration.ScaleOutAddress.Uri.AbsoluteUri,
                   MessageContracts = internalRoutingTable
                       .GetAllRoutes()
                       .SelectMany(ir => ir.Messages)
                       .Select(m => new MessageContract
                                    {
                                        Version = m.Version,
                                        Identity = m.Identity,
                                        Partition = m.Partition
                                    })
                       .ToArray()
               };

        private IEnumerable<MessageRoute> GetExternalRoutes()
            => externalRoutingTable
                .GetAllRoutes()
                .Select(mr => new MessageRoute
                              {
                                  SocketIdentity = mr.Connection.Node.SocketIdentity,
                                  Uri = mr.Connection.Node.Uri.ToSocketAddress(),
                                  Connected = mr.Connection.Connected,
                                  MessageContracts = mr.Messages
                                                       .Select(m => new MessageContract
                                                                    {
                                                                        Version = m.Version,
                                                                        Identity = m.Identity,
                                                                        Partition = m.Partition
                                                                    })
                                                       .ToArray()
                              });
    }
}