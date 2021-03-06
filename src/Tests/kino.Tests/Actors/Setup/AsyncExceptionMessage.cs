﻿using System;
using kino.Core.Framework;
using kino.Messaging;
using ProtoBuf;

namespace kino.Tests.Actors.Setup
{
    [ProtoContract]
    public class AsyncExceptionMessage : Payload
    {
        private static readonly byte[] MessageIdentity = "ASYNCEXCMSG".GetBytes();

        [ProtoMember(1)]
        public TimeSpan Delay { get; set; }

        [ProtoMember(2)]
        public string ErrorMessage { get; set; }

        public override ushort Version => Message.CurrentVersion;
        public override byte[] Identity => MessageIdentity;
    }
}