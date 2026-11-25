using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Starting flags on new game (Q466, schema 143).</summary>
    public sealed class ScenarioBaggageConfig : ISync
    {
        public int Enabled;
        public List<BaggageItemConfig> Items = new List<BaggageItemConfig>();

        public BaggageItemConfig Item(string id)
        {
            for (int i = 0; i < Items.Count; i++) if (Items[i].IdValue == id) return Items[i];
            return null;
        }

        public void Sync(Syncer s) { s.Int("enabled", ref Enabled); s.ObjList("items", ref Items); }
    }

    public sealed class BaggageItemConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>lawsuit, debt, or leak</summary>
        public string Kind = "";
        public long LoanPrincipal;
        public long NotePrincipal;
        public long RbfAdvance;
        public long CaseClaim;
        public int CaseStrength;
        public int ReputationLoss;
        public int TrustLoss;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("kind", ref Kind);
            s.Long("loanPrincipal", ref LoanPrincipal);
            s.Long("notePrincipal", ref NotePrincipal);
            s.Long("rbfAdvance", ref RbfAdvance);
            s.Long("caseClaim", ref CaseClaim);
            s.Int("caseStrength", ref CaseStrength);
            s.Int("reputationLoss", ref ReputationLoss);
            s.Int("trustLoss", ref TrustLoss);
        }
    }
}