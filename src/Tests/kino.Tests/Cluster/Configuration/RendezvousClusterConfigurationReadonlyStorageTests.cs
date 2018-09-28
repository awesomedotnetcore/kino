﻿using System.Collections.Generic;
using kino.Cluster.Configuration;
using kino.Tests.Helpers;
using NUnit.Framework;

namespace kino.Tests.Cluster.Configuration
{
    public class RendezvousClusterConfigurationReadonlyStorageTests
    {
        private RendezvousClusterConfigurationReadonlyStorage configStorage;
        private IEnumerable<RendezvousEndpoint> initialConfiguration;

        [SetUp]
        public void Setup()
        {
            initialConfiguration = Randomizer.Int32(3, 6)
                                             .Produce(i => new RendezvousEndpoint($"tcp://*:808{i}", $"tcp://*:909{i}"));
            configStorage = new RendezvousClusterConfigurationReadonlyStorage(initialConfiguration);
        }

        [Test]
        public void Update_RemovesAllPreviousRendezvousEndpointsAndAddsNewOnes()
        {
            var config = new RendezvousClusterConfiguration
                         {
                             Cluster = Randomizer.Int32(3, 6)
                                                 .Produce(i => new RendezvousEndpoint($"tcp://*:8{i}80", $"tcp://*:9{i}90"))
                         };
            CollectionAssert.AreEquivalent(initialConfiguration, configStorage.Read().Cluster);
            //
            configStorage.Update(config);
            //
            CollectionAssert.AreEquivalent(config.Cluster, configStorage.Read().Cluster);
        }
    }
}