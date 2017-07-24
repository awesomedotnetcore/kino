﻿using System;
using System.Collections.Generic;
using kino.Messaging;

namespace kino.Consensus
{
    public interface IIntercomMessageHub
    {
        Listener Subscribe();

        void Broadcast(IMessage message);

        void Send(IMessage message, byte[] receiver);

        bool Start(TimeSpan startTimeout);

        void Stop();

        IEnumerable<INodeHealthInfo> GetClusterHealthInfo();
    }
}