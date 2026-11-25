using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Government contracts. While nothing is in force, every so often a government offers the player's lab a contract in turn from a list: it needs a security tier,
    /// engineers kept off other work for the whole term, and it pays in instalments if the engineers are still there on each date. Accepting can cost reputation
    /// and the safety staff's morale. Missing too many instalments ends it with a penalty; walking away costs the same and some reputation. A breach that leaks
    /// ends a cleared contract at once. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class GovContractsConfig : ISync
    {
        public int Enabled;
        /// <summary>Weeks after one contract ends or is turned down before the next is offered.</summary>
        public int OfferEveryWeeks;
        /// <summary>Weeks an offer stays open.</summary>
        public int OfferWeeks;
        /// <summary>How many instalments the pay comes in, spread over the term.</summary>
        public int Installments;
        /// <summary>Missed instalments that end a contract.</summary>
        public int MissLimit;
        /// <summary>The penalty for an ended or abandoned contract, as a share of its pay.</summary>
        public int PenaltyPermille;
        /// <summary>Reputation lost for walking away.</summary>
        public int AbandonReputation;
        /// <summary>Engineers who must stay off every contract and the desk.</summary>
        public int KeepFree;
        /// <summary>The role whose morale a contract's moral cost falls on.</summary>
        public string MoraleRole = "";
        public List<ContractKindConfig> Kinds = new List<ContractKindConfig>();

        public ContractKindConfig Kind(string id)
        {
            for (int i = 0; i < Kinds.Count; i++) if (Kinds[i].Id == id) return Kinds[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("offerEveryWeeks", ref OfferEveryWeeks);
            s.Int("offerWeeks", ref OfferWeeks);
            s.Int("installments", ref Installments);
            s.Int("missLimit", ref MissLimit);
            s.Int("penaltyPermille", ref PenaltyPermille);
            s.Int("abandonReputation", ref AbandonReputation);
            s.Int("keepFree", ref KeepFree);
            s.Str("moraleRole", ref MoraleRole);
            s.ObjList("kinds", ref Kinds);
        }
    }

    /// <summary>One kind of government contract.</summary>
    public sealed class ContractKindConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>The security tier the lab must hold to accept it.</summary>
        public int Clearance;
        public int Weeks;
        /// <summary>The whole pay, in instalments.</summary>
        public long Pay;
        /// <summary>Engineers kept on it for the whole term.</summary>
        public int People;
        /// <summary>Morale the safety staff lose when it is accepted.</summary>
        public int Morale;
        /// <summary>Reputation gained (or, below nothing, lost) when it is accepted.</summary>
        public int AcceptReputation;
        /// <summary>Reputation gained when it is completed.</summary>
        public int DoneReputation;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("clearance", ref Clearance);
            s.Int("weeks", ref Weeks);
            s.Long("pay", ref Pay);
            s.Int("people", ref People);
            s.Int("morale", ref Morale);
            s.Int("acceptReputation", ref AcceptReputation);
            s.Int("doneReputation", ref DoneReputation);
        }
    }
}
