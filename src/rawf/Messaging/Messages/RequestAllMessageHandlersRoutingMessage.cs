﻿using ProtoBuf;
using rawf.Framework;

namespace rawf.Messaging.Messages
{
    [ProtoContract]
    public class RequestAllMessageHandlersRoutingMessage : Payload
    {
        public static readonly byte[] MessageIdentity = "REQALLROUTE".GetBytes();

        [ProtoMember(1)]
        public string RequestorUri { get; set; }

        [ProtoMember(2)]
        public byte[] RequestorSocketIdentity { get; set; }                
    }
}