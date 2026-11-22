using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Long-term support pledges. The lab promises the customers of a released model that it will keep supporting it for a number of weeks. While the pledge stands the model earns a revenue lift from customers who value that, the lab pays a weekly support cost, the model cannot be replaced in a sector or given an end-of-life notice, and the lab can keep the pledge to its end for trust or break it for a fee and a loss of trust. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PledgesConfig : ISync
    {
        public int Enabled;
        /// <summary>Shortest support pledge, in weeks.</summary>
        public int MinWeeks;
        /// <summary>Longest support pledge, in weeks.</summary>
        public int MaxWeeks;
        /// <summary>Models that can be under a pledge at once.</summary>
        public int MaxPledges;
        /// <summary>Revenue lift on the pledged model for each full 26 weeks of the pledge (permille).</summary>
        public int LiftPermilleStep;
        /// <summary>Most revenue lift a pledge can give (permille).</summary>
        public int LiftCapPermille;
        /// <summary>Share of the pledged model weekly revenue spent on keeping it supported (permille).</summary>
        public int SupportPermille;
        /// <summary>Trust gained in each sector of the model when a pledge is kept to its end.</summary>
        public int KeptTrust;
        /// <summary>Weeks of the model revenue paid as a fee for breaking a pledge.</summary>
        public int BreachWeeks;
        /// <summary>Trust lost in each sector of the model when a pledge is broken.</summary>
        public int BreachTrust;
        /// <summary>Reputation lost (damped) when a pledge is broken.</summary>
        public int BreachReputation;
        /// <summary>Share a sharp pod takes off the weekly support cost of pledged models (permille).</summary>
        public int PodCutPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("maxPledges", ref MaxPledges);
            s.Int("liftPermilleStep", ref LiftPermilleStep);
            s.Int("liftCapPermille", ref LiftCapPermille);
            s.Int("supportPermille", ref SupportPermille);
            s.Int("keptTrust", ref KeptTrust);
            s.Int("breachWeeks", ref BreachWeeks);
            s.Int("breachTrust", ref BreachTrust);
            s.Int("breachReputation", ref BreachReputation);
            s.Int("podCutPermille", ref PodCutPermille);
        }
    }
}
