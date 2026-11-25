using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Inference optimisation: the engineers spend time making serving cheaper (batching, quantising, caching). Each level cuts the compute the
    /// lab needs to serve its customers, which matters the moment the lab cannot serve them all, and each level makes the models a little
    /// more brittle, which adds to the weekly chance of an incident. The levels need engineers behind them: with fewer than the data asks for,
    /// the lab only gets the level its engineers cover. Raising a level costs a setup fee; lowering is free but the work is lost. Only the
    /// player's lab optimises. Zero levels turns it off.
    /// </summary>
    public sealed class OptimisationConfig : ISync
    {
        /// <summary>The highest level. Zero turns it off.</summary>
        public int MaxLevel;
        /// <summary>Cash for each level added.</summary>
        public long SetupCostPerLevel;
        /// <summary>The engineers each level needs on the payroll.</summary>
        public int EngineersPerLevel;
        /// <summary>The share each level takes off the compute needed to serve customers, in permille.</summary>
        public int DemandCutPermillePerLevel;
        /// <summary>The weekly incident chance, in permille, each level adds to every released model.</summary>
        public int IncidentPermillePerLevel;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("setupCostPerLevel", ref SetupCostPerLevel);
            s.Int("engineersPerLevel", ref EngineersPerLevel);
            s.Int("demandCutPermillePerLevel", ref DemandCutPermillePerLevel);
            s.Int("incidentPermillePerLevel", ref IncidentPermillePerLevel);
        }
    }
}
