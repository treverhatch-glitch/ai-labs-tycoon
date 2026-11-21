using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Technical debt. The player's lab piles it up by cutting corners: skipping testing before a launch, working the team late, patching a
    /// glitching model instead of rebuilding it. It never decays; it raises the weekly chance of an incident in proportion to how much there
    /// is, and only paying it down lowers it. Paying costs cash and needs engineers. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class TechDebtConfig : ISync
    {
        /// <summary>1 when debt is kept, 0 when not.</summary>
        public int Enabled;
        /// <summary>Points added for each week of testing skipped on a rushed launch.</summary>
        public int RushPerWeek;
        /// <summary>Points added for each week the team works in crunch time.</summary>
        public int CrunchPerWeek;
        /// <summary>Points added when a glitching model is fine-tuned.</summary>
        public int FineTunePatch;
        /// <summary>The extra weekly incident chance, in permille, at the most debt (1000 points); less for less.</summary>
        public int IncidentAtFull;
        /// <summary>Cash for each point paid down.</summary>
        public int CostPerPoint;
        /// <summary>The engineers a lab needs before it can pay debt down.</summary>
        public int MinEngineers;
        /// <summary>The most points one command can pay down.</summary>
        public int MaxPerCommand;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("rushPerWeek", ref RushPerWeek);
            s.Int("crunchPerWeek", ref CrunchPerWeek);
            s.Int("fineTunePatch", ref FineTunePatch);
            s.Int("incidentAtFull", ref IncidentAtFull);
            s.Int("costPerPoint", ref CostPerPoint);
            s.Int("minEngineers", ref MinEngineers);
            s.Int("maxPerCommand", ref MaxPerCommand);
        }
    }
}
