using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A kind of trouble that only exists from a given era on: bias with the first image models, a driving crash, a privacy leak, an agent
    /// that went off-script. A model on sale where the class bites can suffer it instead of the era's generic mishap; the class says how much
    /// worse the fine and the blow to reputation are, and what the headline says. Once open a class stays on the books.
    /// </summary>
    public sealed class RiskClassConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>The era the class opens in; it can happen from the first week of that era.</summary>
        public string Era = "";
        /// <summary>How likely this class is, against the other classes that bite and the generic mishap (<c>safety.genericRiskWeight</c>).</summary>
        public int Weight;
        /// <summary>The sectors where it bites: a model must be on sale in at least one of them. Empty means any sector.</summary>
        public List<string> Sectors = new List<string>();
        /// <summary>Multiplies the incident fine (1000 = unchanged).</summary>
        public int FinePermille;
        /// <summary>Multiplies the reputation lost (1000 = unchanged).</summary>
        public int ReputationPermille;
        /// <summary>Headlines for this class; <c>{model}</c> is the model's name.</summary>
        public List<string> Headlines = new List<string>();

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("era", ref Era);
            s.Int("weight", ref Weight);
            s.StrList("sectors", ref Sectors);
            s.Int("finePermille", ref FinePermille);
            s.Int("reputationPermille", ref ReputationPermille);
            s.StrList("headlines", ref Headlines);
        }
    }
}
