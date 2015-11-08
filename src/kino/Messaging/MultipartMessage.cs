﻿using System;
using System.Collections.Generic;
using System.Linq;
using kino.Framework;
using NetMQ;

namespace kino.Messaging
{
    internal partial class MultipartMessage
    {
        private const int MinFramesCount = 12;
        private readonly IList<byte[]> frames;
        private static readonly byte[] EmptyFrame = new byte[0];

        internal MultipartMessage(Message message)
        {
            frames = BuildMessageParts(message).ToList();
        }

        internal MultipartMessage(NetMQMessage message)
        {
            AssertMessage(message);

            frames = SplitMessageToFrames(message);
        }

        private IList<byte[]> SplitMessageToFrames(IEnumerable<NetMQFrame> message)
            => message.Select(m => m.Buffer).ToList();

        private IEnumerable<byte[]> BuildMessageParts(Message message)
        {
            yield return GetSocketIdentity(message);

            yield return EmptyFrame;

            yield return GetMessageRouteFrame(message); // 13
            yield return GetTraceOptionsFrame(message); // 12
            yield return GetVersionFrame(message); // 11
            yield return GetMessageIdentityFrame(message); // 10
            yield return GetReceiverIdentityFrame(message); // 9
            yield return GetDistributionFrame(message); // 8
            yield return GetCorrelationIdFrame(message); // 7
            yield return GetCallbackVersionFrame(message); // 6
            yield return GetCallbackIdentityFrame(message); // 5
            yield return GetCallbackReceiverIdentityFrame(message); // 4
            yield return GetTTLFrame(message); // 3

            yield return EmptyFrame;

            yield return GetMessageBodyFrame(message);
        }

        private byte[] GetMessageRouteFrame(Message message)
            => message.GetMessageHopsBytes();

        private byte[] GetTraceOptionsFrame(IMessage message)
            => ((long) message.TraceOptions).GetBytes();

        private byte[] GetSocketIdentity(IMessage message)
            => ((Message) message).SocketIdentity ?? EmptyFrame;

        private byte[] GetReceiverIdentityFrame(IMessage message)
            => message.ReceiverIdentity ?? EmptyFrame;

        private byte[] GetCallbackReceiverIdentityFrame(IMessage message)
            => message.CallbackReceiverIdentity ?? EmptyFrame;

        private byte[] GetCallbackIdentityFrame(IMessage message)
            => message.CallbackIdentity ?? EmptyFrame;

        private byte[] GetCallbackVersionFrame(Message message)
            => message.CallbackVersion ?? EmptyFrame;

        private byte[] GetCorrelationIdFrame(IMessage message)
            => message.CorrelationId ?? EmptyFrame;

        private byte[] GetTTLFrame(IMessage message)
            => message.TTL.GetBytes();

        private byte[] GetVersionFrame(IMessage message)
            => message.Version;

        private byte[] GetDistributionFrame(IMessage message)
            => ((int) message.Distribution).GetBytes();

        private byte[] GetMessageBodyFrame(IMessage message)
            => message.Body;

        private byte[] GetMessageIdentityFrame(IMessage message)
            => message.Identity;

        private static void AssertMessage(NetMQMessage message)
        {
            if (message.FrameCount < MinFramesCount)
            {
                throw new Exception($"FrameCount expected (at least): [{MinFramesCount}], received: [{message.FrameCount}]");
            }
        }

        internal byte[] GetMessageIdentity()
            => frames[frames.Count - ReversedFrames.Identity];

        internal byte[] GetMessageVersion()
            => frames[frames.Count - ReversedFrames.Version];

        internal byte[] GetMessageBody()
            => frames[frames.Count - ReversedFrames.Body];

        internal byte[] GetMessageTTL()
            => frames[frames.Count - ReversedFrames.TTL];

        internal byte[] GetMessageDistributionPattern()
            => frames[frames.Count - ReversedFrames.DistributionPattern];

        internal byte[] GetTraceOptions()
            => frames[frames.Count - ReversedFrames.TraceOptions];

        internal byte[] GetCallbackReceiverIdentity()
            => frames[frames.Count - ReversedFrames.CallbackReceiverIdentity];

        internal byte[] GetCallbackIdentity()
            => frames[frames.Count - ReversedFrames.CallbackIdentity];

        internal byte[] GetCallbackVersion()
            => frames[frames.Count - ReversedFrames.CallbackVersion];

        internal byte[] GetCorrelationId()
            => frames[frames.Count - ReversedFrames.CorrelationId];

        internal byte[] GetReceiverIdentity()
            => frames[frames.Count - ReversedFrames.ReceiverIdentity];

        internal byte[] GetMessageRoute()
            => frames[frames.Count - ReversedFrames.MessageRoute];

        internal IEnumerable<byte[]> Frames => frames;
    }
}