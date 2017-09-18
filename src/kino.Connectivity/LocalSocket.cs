﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using kino.Core;
using kino.Core.Diagnostics.Performance;

namespace kino.Connectivity
{
    public class LocalSocket<T> : ILocalSocket<T>, IEquatable<ILocalSocket<T>>
    {
        private readonly ManualResetEvent dataAvailable;
        private readonly BlockingCollection<T> messageQueue;
        private readonly BlockingCollection<T> lookAheadQueue;
        private readonly ReceiverIdentifier socketIdentity;
        private readonly int hashCode;

        public LocalSocket()
        {
            dataAvailable = new ManualResetEvent(false);
            socketIdentity = ReceiverIdentifier.Create();
            messageQueue = new BlockingCollection<T>(new ConcurrentQueue<T>());
            lookAheadQueue = new BlockingCollection<T>(new ConcurrentQueue<T>());
            hashCode = socketIdentity.GetHashCode();
        }

        public void Send(T message)
        {
            messageQueue.Add(message);
            dataAvailable.Set();
            SendRate?.Increment();
        }

        public T TryReceive()
        {
            T lookup,
              message;
            if (!lookAheadQueue.TryTake(out message))
            {
                messageQueue.TryTake(out message);
            }
            if (!messageQueue.TryTake(out lookup))
            {
                dataAvailable.Reset();
                if (messageQueue.TryTake(out lookup))
                {
                    lookAheadQueue.Add(lookup);
                    dataAvailable.Set();
                }
            }
            else
            {
                lookAheadQueue.Add(lookup);
            }

            ReceiveRate?.Increment();

            return message;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ILocalSocket<T>) obj);
        }

        public bool Equals(ILocalSocket<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetIdentity() == other.GetIdentity();
        }

        public override int GetHashCode()
            => hashCode;

        public WaitHandle CanReceive()
            => dataAvailable;

        public ReceiverIdentifier GetIdentity()
            => socketIdentity;

        public IPerformanceCounter ReceiveRate { get; set; }

        public IPerformanceCounter SendRate { get; set; }
    }
}