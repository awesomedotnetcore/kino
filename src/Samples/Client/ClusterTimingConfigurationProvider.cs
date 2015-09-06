﻿using kino.Connectivity;

namespace Client
{
    public class ClusterTimingConfigurationProvider : IClusterTimingConfigurationProvider
    {
        private readonly ClusterTimingConfiguration config;

        public ClusterTimingConfigurationProvider(ApplicationConfiguration appConfig)
        {
            config = new ClusterTimingConfiguration
                     {
                         ExpectedPingInterval = appConfig.ExpectedPingInterval,
                         PongSilenceBeforeRouteDeletion = appConfig.PongSilenceBeforeRouteDeletion,
                         PingSilenceBeforeRendezvousFailover = appConfig.PingSilenceBeforeRendezvousFailover
                     };
        }

        public ClusterTimingConfiguration GetConfiguration()
            => config;
    }
}