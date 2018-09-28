﻿using kino.Cluster;
using kino.Core;
using kino.Tests.Actors.Setup;
using NUnit.Framework;

namespace kino.Tests.Cluster
{
    public class MessageRouteTests
    {
        [Test]
        public void TwoMessageRoutesAreEqual_IfTheirMessageAndReceiverPropertiesAreEqual()
        {
            var receiver = ReceiverIdentities.CreateForActor();
            var first = new MessageRoute
                        {
                            Message = MessageIdentifier.Create<SimpleMessage>(),
                            Receiver = receiver
                        };
            var second = new MessageRoute
                         {
                             Message = MessageIdentifier.Create<SimpleMessage>(),
                             Receiver = receiver
                         };
            //
            Assert.AreEqual(first, second);
            Assert.True(first.Equals(second));
            Assert.True(first.Equals((object) second));
            Assert.True(first == second);
            Assert.False(first != second);
        }

        [Test]
        public void TwoMessageRoutesAreNotEqual_IfTheirMessagePropertiesAreNotEqual()
        {
            var receiver = ReceiverIdentities.CreateForActor();
            var first = new MessageRoute
                        {
                            Message = MessageIdentifier.Create<NullMessage>(),
                            Receiver = receiver
                        };
            var second = new MessageRoute
                         {
                             Message = MessageIdentifier.Create<SimpleMessage>(),
                             Receiver = receiver
                         };
            //
            Assert.AreNotEqual(first, second);
            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object) second));
            Assert.True(first != second);
            Assert.False(first == second);
        }

        [Test]
        public void TwoMessageRoutesAreNotEqual_IfTheirReceiverPropertiesAreNotEqual()
        {
            var first = new MessageRoute
                        {
                            Message = MessageIdentifier.Create<SimpleMessage>(),
                            Receiver = ReceiverIdentities.CreateForActor()
                        };
            var second = new MessageRoute
                         {
                             Message = MessageIdentifier.Create<SimpleMessage>(),
                             Receiver = ReceiverIdentities.CreateForActor()
                         };
            //
            Assert.AreNotEqual(first, second);
            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object) second));
            Assert.True(first != second);
            Assert.False(first == second);
        }
    }
}