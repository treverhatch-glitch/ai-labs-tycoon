using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Tool policy for agents (bible ch. 6, which tools a model's agents may call). A released model with agents can be given a mask of web, code and data tools. Each bit lifts what agents earn and adds to weekly incidents and blow-up risk, and deep queues build faster when more is allowed. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ToolPolicyConfig : ISync
    {
        public int Enabled;
        /// <summary>First era for per-model agent tool policies.</summary>
        public int MinEra;
        /// <summary>Cash to set the tool mask on a model.</summary>
        public int SetupCost;
        /// <summary>Share of agent revenue lift from allowing web tools (permille).</summary>
        public int LiftWeb;
        /// <summary>Share of agent revenue lift from allowing code tools (permille).</summary>
        public int LiftCode;
        /// <summary>Share of agent revenue lift from allowing data tools (permille).</summary>
        public int LiftData;
        /// <summary>Weekly incident permille from web tools.</summary>
        public int IncidentWeb;
        /// <summary>Weekly incident permille from code tools.</summary>
        public int IncidentCode;
        /// <summary>Weekly incident permille from data tools.</summary>
        public int IncidentData;
        /// <summary>Extra blow-up permille from web tools.</summary>
        public int BlowupWeb;
        /// <summary>Extra blow-up permille from code tools.</summary>
        public int BlowupCode;
        /// <summary>Extra blow-up permille from data tools.</summary>
        public int BlowupData;
        /// <summary>Human-queue items added per week when web is on at delegated or higher.</summary>
        public int QueueWeb;
        /// <summary>Human-queue items added per week when code is on.</summary>
        public int QueueCode;
        /// <summary>Human-queue items added per week when data tools are on.</summary>
        public int QueueData;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("setupCost", ref SetupCost);
            s.Int("liftWeb", ref LiftWeb);
            s.Int("liftCode", ref LiftCode);
            s.Int("liftData", ref LiftData);
            s.Int("incidentWeb", ref IncidentWeb);
            s.Int("incidentCode", ref IncidentCode);
            s.Int("incidentData", ref IncidentData);
            s.Int("blowupWeb", ref BlowupWeb);
            s.Int("blowupCode", ref BlowupCode);
            s.Int("blowupData", ref BlowupData);
            s.Int("queueWeb", ref QueueWeb);
            s.Int("queueCode", ref QueueCode);
            s.Int("queueData", ref QueueData);
        }
    }
}
