﻿using System;
using System.Linq;
using kino.Consensus;
using kino.Core.Framework;
using NUnit.Framework;
using static kino.Tests.Consensus.Setup.RoundBasedRegisterTestsHelper;

namespace kino.Tests.Consensus
{
    [TestFixture(Category = "FLease",
        Description = @"Lemma R2: Write-abort: If WRITE(k; *) aborts, then
some operation READ(k0) or WRITE(k0; *) was invoked with k0 > k.")]
    public class RoundBasedRegisterTests_LemmaR2
    {
        private byte[] ownerPayload;

        
        public void Setup()
        {
            ownerPayload = Guid.NewGuid().ToByteArray();
        }

        [Fact]
        public void WriteIsAborted_AfterReadWithBallotGreaterThanCurrent()
        {
            using (CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().First()))
            {
                using (CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().Second()))
                {
                    using (var testSetup = CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().Third()))
                    {
                        var ballotGenerator = testSetup.BallotGenerator;
                        var localNode = testSetup.LocalNode;
                        var roundBasedRegister = testSetup.RoundBasedRegister;

                        var ballot0 = ballotGenerator.New(localNode.SocketIdentity);
                        var txResult = RepeatUntil(() => roundBasedRegister.Read(ballot0), TxOutcome.Commit);
                        Assert.Equal(TxOutcome.Commit, txResult.TxOutcome);

                        var ballot1 = new Ballot(ballot0.Timestamp - TimeSpan.FromSeconds(10), ballot0.MessageNumber, localNode.SocketIdentity);
                        Assert.True(ballot0 > ballot1);
                        var lease = new Lease(localNode.SocketIdentity, DateTime.UtcNow, ownerPayload);
                        txResult = roundBasedRegister.Write(ballot1, lease);
                        Assert.Equal(TxOutcome.Abort, txResult.TxOutcome);
                    }
                }
            }
        }

        [Fact]
        public void WriteIsAborted_AfterWriteWithBallotGreaterThanCurrent()
        {
            using (CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().First()))
            {
                using (CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().Second()))
                {
                    using (var testSetup = CreateRoundBasedRegister(GetSynodMembers(), GetSynodMembers().Third()))
                    {
                        var ballotGenerator = testSetup.BallotGenerator;
                        var localNode = testSetup.LocalNode;
                        var roundBasedRegister = testSetup.RoundBasedRegister;

                        var ballot0 = ballotGenerator.New(localNode.SocketIdentity);
                        var lease = new Lease(localNode.SocketIdentity, DateTime.UtcNow, ownerPayload);
                        var txResult = RepeatUntil(() => roundBasedRegister.Write(ballot0, lease), TxOutcome.Commit);
                        Assert.Equal(TxOutcome.Commit, txResult.TxOutcome);

                        var ballot1 = new Ballot(ballot0.Timestamp - TimeSpan.FromSeconds(10), ballot0.MessageNumber, localNode.SocketIdentity);
                        Assert.True(ballot0 > ballot1);
                        txResult = roundBasedRegister.Write(ballot1, lease);
                        Assert.Equal(TxOutcome.Abort, txResult.TxOutcome);
                    }
                }
            }
        }
    }
}