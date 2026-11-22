using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Customer integrations bound to a model version. Opening a model's API lets customers build on it; integrations grow over time, lift the model's revenue and cost upkeep. A newer model does not inherit them: they have to be moved by deprecating the old version with a successor and a deadline. Each week of the deadline part of what is left moves over, some is lost on the way, and whatever is still on the old version at the deadline is stranded, which costs trust. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class IntegrationsConfig : ISync
    {
        public int Enabled;
        /// <summary>Models whose API can be open at once.</summary>
        public int MaxOpen;
        /// <summary>Most integrations one model can hold.</summary>
        public int MaxIntegrations;
        /// <summary>New integrations a week on a model with an open API, before growth.</summary>
        public int SeedPerWeek;
        /// <summary>Weekly growth of the integrations a model already has (permille).</summary>
        public int GrowthPermille;
        /// <summary>Revenue lift on the model for each 10 integrations (permille).</summary>
        public int LiftPermillePer10;
        /// <summary>Most revenue lift integrations can give (permille).</summary>
        public int LiftCapPermille;
        /// <summary>Share of the model weekly revenue spent keeping its API running (permille).</summary>
        public int UpkeepBasePermille;
        /// <summary>More upkeep for each 10 integrations (permille of revenue).</summary>
        public int UpkeepPer10Permille;
        /// <summary>Share of the remaining integrations that move to the successor each week of a deprecation (permille).</summary>
        public int MigratePermille;
        /// <summary>Share of the integrations that move that are lost on the way (permille).</summary>
        public int ChurnPermille;
        /// <summary>Shortest deprecation deadline, in weeks.</summary>
        public int MinWeeks;
        /// <summary>Longest deprecation deadline, in weeks.</summary>
        public int MaxWeeks;
        /// <summary>Trust lost in each sector of the model for each 100 integrations stranded at the deadline.</summary>
        public int StrandTrustPer100;
        /// <summary>Most trust a stranding can cost in one sector.</summary>
        public int StrandTrustCap;

        /// <summary>Share of a model's integrations lost when it has an incident (permille).</summary>
        public int IncidentPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("maxIntegrations", ref MaxIntegrations);
            s.Int("seedPerWeek", ref SeedPerWeek);
            s.Int("growthPermille", ref GrowthPermille);
            s.Int("liftPermillePer10", ref LiftPermillePer10);
            s.Int("liftCapPermille", ref LiftCapPermille);
            s.Int("upkeepBasePermille", ref UpkeepBasePermille);
            s.Int("upkeepPer10Permille", ref UpkeepPer10Permille);
            s.Int("migratePermille", ref MigratePermille);
            s.Int("churnPermille", ref ChurnPermille);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("strandTrustPer100", ref StrandTrustPer100);
            s.Int("strandTrustCap", ref StrandTrustCap);
            s.Int("incidentPermille", ref IncidentPermille);
        }
    }
}
