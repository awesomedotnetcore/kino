﻿using System;
using System.Linq;
using System.Threading.Tasks;
using kino.Cluster.Configuration;
using kino.Core;
using kino.Core.Framework;
using kino.Tests.Helpers;
using NUnit.Framework;

namespace kino.Tests.Cluster.Configuration
{
    
    public class ScaleOutConfigurationManagerTests
    {
        private ScaleOutConfigurationManager configManager;
        private ScaleOutSocketConfiguration config;

        
        public void Setup()
        {
            config = new ScaleOutSocketConfiguration
                     {
                         AddressRange = EnumerableExtensions.Produce(Randomizer.Int32(3, 6),
                                                                    i => new SocketEndpoint($"tcp://*:808{i}"))
                     };
            configManager = new ScaleOutConfigurationManager(config);
        }

        [Fact]
        public void IfActiveScaleOutAddressIsNotSet_GetScaleOutAddressBlocks()
        {
            var task = Task.Factory.StartNew(() => configManager.GetScaleOutAddress());
            //
            Assert.False(task.Wait(TimeSpan.FromSeconds(3)));
        }

        [Fact]
        public void GetScaleOutAddressUnblocks_WhenActiveScaleOutAddressIsSet()
        {
            var asyncOp = TimeSpan.FromSeconds(4);
            var task = Task.Factory.StartNew(() => configManager.GetScaleOutAddress());
            Task.Factory.StartNew(() =>
                                  {
                                      asyncOp.DivideBy(2).Sleep();
                                      configManager.SetActiveScaleOutAddress(config.AddressRange.First());
                                  });
            //
            Assert.True(task.Wait(asyncOp));
        }

        [Fact]
        public void IfSocketEndpointDoesntBelongToInitialAddressRange_SetActiveScaleOutAddressThrowsException()
        {
            Assert.Throws<Exception>(() => configManager.SetActiveScaleOutAddress(new SocketEndpoint("tcp://*:43")));
        }
    }
}