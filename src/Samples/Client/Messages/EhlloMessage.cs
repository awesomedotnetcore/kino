﻿using kino.Core.Framework;
using kino.Core.Messaging;
using ProtoBuf;

namespace Client.Messages
{
    [ProtoContract]
    public class EhlloMessage : Payload
    {
        private static readonly byte[] MessageIdentity = "EHHLO".GetBytes();

        [ProtoMember(1)]
        public string Ehllo { get; set; }

        public override byte[] Version => Message.CurrentVersion;
        public override byte[] Identity => MessageIdentity;
    }
}