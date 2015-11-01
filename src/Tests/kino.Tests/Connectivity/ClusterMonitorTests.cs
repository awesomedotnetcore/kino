﻿using System;
using System.Linq;
using kino.Connectivity;
using kino.Diagnostics;
using kino.Framework;
using kino.Messaging;
using kino.Messaging.Messages;
using kino.Sockets;
using kino.Tests.Actors.Setup;
using kino.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace kino.Tests.Connectivity
{
    [TestFixture]
    public class ClusterMonitorTests
    {
        private static readonly TimeSpan AsyncOp = TimeSpan.FromMilliseconds(100);
        private ClusterMonitorSocketFactory clusterMonitorSocketFactory;
        private Mock<ILogger> logger;
        private Mock<ISocketFactory> socketFactory;
        private RouterConfiguration routerConfiguration;
        private Mock<IClusterMembership> clusterMembership;
        private Mock<IRendezvousCluster> rendezvousCluster;
        private ClusterMembershipConfiguration clusterMembershipConfiguration;
        private IClusterMessageSender clusterMessageSender;
        private IClusterMessageListener clusterMessageListener;

        [SetUp]
        public void Setup()
        {
            clusterMonitorSocketFactory = new ClusterMonitorSocketFactory();
            logger = new Mock<ILogger>();
            socketFactory = new Mock<ISocketFactory>();
            socketFactory.Setup(m => m.CreateSubscriberSocket()).Returns(clusterMonitorSocketFactory.CreateSocket);
            socketFactory.Setup(m => m.CreateDealerSocket()).Returns(clusterMonitorSocketFactory.CreateSocket);
            rendezvousCluster = new Mock<IRendezvousCluster>();
            routerConfiguration = new RouterConfiguration
                                  {
                                      ScaleOutAddress = new SocketEndpoint(new Uri("tcp://127.0.0.1:5000"), SocketIdentifier.CreateIdentity()),
                                      RouterAddress = new SocketEndpoint(new Uri("inproc://router"), SocketIdentifier.CreateIdentity())
                                  };
            var rendezvousEndpoint = new RendezvousEndpoint(new Uri("tcp://127.0.0.1:5000"),
                                                            new Uri("tcp://127.0.0.1:5000"));
            rendezvousCluster.Setup(m => m.GetCurrentRendezvousServer()).Returns(rendezvousEndpoint);
            clusterMembership = new Mock<IClusterMembership>();
            clusterMembershipConfiguration = new ClusterMembershipConfiguration
                                             {
                                                 RunAsStandalone = false,
                                                 PingSilenceBeforeRendezvousFailover = TimeSpan.FromSeconds(2),
                                                 PongSilenceBeforeRouteDeletion = TimeSpan.FromMilliseconds(4)
                                             };
            clusterMessageSender = new ClusterMessageSender(rendezvousCluster.Object,
                                                            routerConfiguration,
                                                            socketFactory.Object,
                                                            logger.Object);
            clusterMessageListener = new ClusterMessageListener(rendezvousCluster.Object,
                                                                socketFactory.Object,
                                                                routerConfiguration,
                                                                clusterMessageSender,
                                                                clusterMembership.Object,
                                                                clusterMembershipConfiguration,
                                                                logger.Object);
        }

        [Test]
        public void TestRegisterSelf_SendRegistrationMessageToRendezvous()
        {
            var clusterMonitor = new ClusterMonitor(routerConfiguration,
                                                    clusterMembership.Object,
                                                    clusterMessageSender,
                                                    clusterMessageListener);
            try
            {
                clusterMonitor.Start();

                var messageIdentifier = MessageIdentifier.Create<SimpleMessage>();
                clusterMonitor.RegisterSelf(new[] {messageIdentifier});

                var socket = clusterMonitorSocketFactory.GetClusterMonitorSendingSocket();
                var message = socket.GetSentMessages().BlockingLast(AsyncOp);

                Assert.IsNotNull(message);
                Assert.IsTrue(Unsafe.Equals(message.Identity, KinoMessages.RegisterExternalMessageRoute.Identity));
                var payload = message.GetPayload<RegisterExternalMessageRouteMessage>();
                Assert.IsTrue(Unsafe.Equals(payload.SocketIdentity, routerConfiguration.ScaleOutAddress.Identity));
                Assert.AreEqual(payload.Uri, routerConfiguration.ScaleOutAddress.Uri.ToSocketAddress());
                Assert.IsTrue(payload.MessageContracts.Any(mc => Unsafe.Equals(mc.Identity, messageIdentifier.Identity)
                                                                 && Unsafe.Equals(mc.Version, messageIdentifier.Version)));
            }
            finally
            {
                clusterMonitor.Stop();
            }
        }

        [Test]
        public void TestUnregisterSelf_SendUnregisterMessageRouteMessageToRendezvous()
        {
            var clusterMonitor = new ClusterMonitor(routerConfiguration,
                                                    clusterMembership.Object,
                                                    clusterMessageSender,
                                                    clusterMessageListener);
            try
            {
                clusterMonitor.Start();

                var messageIdentifier = new MessageIdentifier(Message.CurrentVersion);
                clusterMonitor.UnregisterSelf(new[] {messageIdentifier});

                var socket = clusterMonitorSocketFactory.GetClusterMonitorSendingSocket();
                var message = socket.GetSentMessages().BlockingLast(AsyncOp);

                Assert.IsNotNull(message);
                Assert.IsTrue(Unsafe.Equals(message.Identity, KinoMessages.UnregisterMessageRoute.Identity));
                var payload = message.GetPayload<UnregisterMessageRouteMessage>();
                Assert.IsTrue(Unsafe.Equals(payload.SocketIdentity, routerConfiguration.ScaleOutAddress.Identity));
                Assert.AreEqual(payload.Uri, routerConfiguration.ScaleOutAddress.Uri.ToSocketAddress());
                Assert.IsTrue(payload.MessageContracts.Any(mc => Unsafe.Equals(mc.Identity, messageIdentifier.Identity)
                                                                 && Unsafe.Equals(mc.Version, messageIdentifier.Version)));
            }
            finally
            {
                clusterMonitor.Stop();
            }
        }

        [Test]
        public void TestRequestClusterRoutes_SendRequestClusterMessageRoutesMessageToRendezvous()
        {
            var clusterMonitor = new ClusterMonitor(routerConfiguration,
                                                    clusterMembership.Object,
                                                    clusterMessageSender,
                                                    clusterMessageListener);
            try
            {
                clusterMonitor.Start();

                clusterMonitor.RequestClusterRoutes();

                var socket = clusterMonitorSocketFactory.GetClusterMonitorSendingSocket();
                var message = socket.GetSentMessages().BlockingLast(AsyncOp);

                Assert.IsNotNull(message);
                Assert.IsTrue(Unsafe.Equals(message.Identity, KinoMessages.RequestClusterMessageRoutes.Identity));
            }
            finally
            {
                clusterMonitor.Stop();
            }
        }

        [Test]
        public void TestDiscoverMessageRoute_SendDiscoverMessageRouteMessageToRendezvous()
        {
            var clusterMonitor = new ClusterMonitor(routerConfiguration,
                                                    clusterMembership.Object,
                                                    clusterMessageSender,
                                                    clusterMessageListener);
            try
            {
                clusterMonitor.Start();

                var messageIdentifier = new MessageIdentifier(Message.CurrentVersion, Guid.NewGuid().ToByteArray());
                clusterMonitor.DiscoverMessageRoute(messageIdentifier);

                var socket = clusterMonitorSocketFactory.GetClusterMonitorSendingSocket();
                var message = socket.GetSentMessages().BlockingLast(AsyncOp);

                Assert.IsNotNull(message);
                Assert.IsTrue(Unsafe.Equals(message.Identity, KinoMessages.DiscoverMessageRoute.Identity));
                var payload = message.GetPayload<DiscoverMessageRouteMessage>();
                Assert.IsTrue(Unsafe.Equals(payload.RequestorSocketIdentity, routerConfiguration.ScaleOutAddress.Identity));
                Assert.AreEqual(payload.RequestorUri, routerConfiguration.ScaleOutAddress.Uri.ToSocketAddress());
                Assert.IsTrue(Unsafe.Equals(payload.MessageContract.Identity, messageIdentifier.Identity));
                Assert.IsTrue(Unsafe.Equals(payload.MessageContract.Version, messageIdentifier.Version));
            }
            finally
            {
                clusterMonitor.Stop();
            }
        }
    }
}