using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public enum EndingKind { Best, Win, Neutral, Loss, Worst }

    public sealed class EndingsConfig : ISync
    {
        public List<EndingDefConfig> Endings = new List<EndingDefConfig>();
        public long BailoutCash;
        public int BailoutReputation;
        public int ExitRevenueYears;
        public int ExitFleetPermille;
        public int MonopolyCrownYears;
        public int MonopolyRevenuePpm;
        public int MonopolyComputePpm;
        public int MonopolyCooldownYears;
        public int MonopolyAttention;
        public int HandcuffsFinePermille;
        public int HandcuffsShadyLockWeeks;
        public string HandcuffsSuccessor = "";
        public FrontierConfig Frontier = new FrontierConfig();
        public MuseumConfig Museum = new MuseumConfig();

        public EndingDefConfig Def(string id)
        {
            for (int i = 0; i < Endings.Count; i++) if (Endings[i].Id == id) return Endings[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.ObjList("endings", ref Endings);
            s.Long("bailoutCash", ref BailoutCash);
            s.Int("bailoutReputation", ref BailoutReputation);
            s.Int("exitRevenueYears", ref ExitRevenueYears);
            s.Int("exitFleetPermille", ref ExitFleetPermille);
            s.Int("monopolyCrownYears", ref MonopolyCrownYears);
            s.Int("monopolyRevenuePpm", ref MonopolyRevenuePpm);
            s.Int("monopolyComputePpm", ref MonopolyComputePpm);
            s.Int("monopolyCooldownYears", ref MonopolyCooldownYears);
            s.Int("monopolyAttention", ref MonopolyAttention);
            s.Int("handcuffsFinePermille", ref HandcuffsFinePermille);
            s.Int("handcuffsShadyLockWeeks", ref HandcuffsShadyLockWeeks);
            s.Str("handcuffsSuccessor", ref HandcuffsSuccessor);
            s.Obj("frontier", ref Frontier);
            s.Obj("museum", ref Museum);
        }
    }

    public sealed class EndingDefConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public EndingKind Kind;
        public string BlurbKey = "";
        public List<string> ContinueModes = new List<string>();

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Enum("kind", ref Kind);
            s.Str("blurbKey", ref BlurbKey);
            s.StrList("continueModes", ref ContinueModes);
        }
    }

    public sealed class FrontierConfig : ISync
    {
        public long BaseCost;
        public int GrowthPermille;
        public int MaxLevel;
        public int CapabilityPermillePerLevel;
        /// <summary>Training speed permille gained per frontier level researched.</summary>
        public int TrainingSpeedPermillePerLevel;
        /// <summary>Flat research points granted weekly per frontier level.</summary>
        public long ResearchPointsPerLevel;

        public void Sync(Syncer s)
        {
            s.Long("baseCost", ref BaseCost);
            s.Int("growthPermille", ref GrowthPermille);
            s.Int("maxLevel", ref MaxLevel);
            s.Int("capabilityPermillePerLevel", ref CapabilityPermillePerLevel);
            s.Int("trainingSpeedPermillePerLevel", ref TrainingSpeedPermillePerLevel);
            s.Long("researchPointsPerLevel", ref ResearchPointsPerLevel);
        }
    }

    public sealed class MuseumConfig : ISync
    {
        public int PerLine;
        /// <summary>Research points granted per point of stored capability when a retired model is pruned (permille).</summary>
        public int ResearchPermillePerCapability;
        /// <summary>Reputation gained each week for each model line kept at the retirement cap (museum full).</summary>
        public int ReputationPerFullLine;
        /// <summary>Flat research points each week for each model line at the retirement cap.</summary>
        public long ResearchPointsPerFullLine;
        /// <summary>Cash granted each week for each model line at the retirement cap.</summary>
        public long CashPerFullLine;

        public void Sync(Syncer s)
        {
            s.Int("perLine", ref PerLine);
            s.Int("researchPermillePerCapability", ref ResearchPermillePerCapability);
            s.Int("reputationPerFullLine", ref ReputationPerFullLine);
            s.Long("researchPointsPerFullLine", ref ResearchPointsPerFullLine);
            s.Long("cashPerFullLine", ref CashPerFullLine);
        }
    }
}