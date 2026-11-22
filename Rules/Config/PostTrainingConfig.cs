using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Post-training (bible ch. 6, preference tuning and safety tuning). After a model has trained, and before it goes on sale, the lab can tune it. A preference tuning learns from the clean conversation data of users and makes customers like the model more (revenue), but it teaches the model to please: the more of it, the more often the model fails in front of customers. A safety tuning uses up licensed or scraped clean data and raises the true and shown alignment, but the model refuses more and earns less. Each pass is worth less than the one before and a model has a most. A run takes weeks, uses up data that training would also have wanted, and the model cannot go on sale while it runs. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PostTrainingConfig : ISync
    {
        public int Enabled;
        /// <summary>First era (index) in which models can be tuned after training.</summary>
        public int MinEra;
        /// <summary>Weeks a tuning run takes.</summary>
        public int Weeks;
        /// <summary>Cash a tuning run costs.</summary>
        public int Cost;
        /// <summary>Clean data units a tuning run uses up.</summary>
        public int Units;
        /// <summary>Most runs of each kind one model can have.</summary>
        public int MaxPasses;
        /// <summary>Revenue the first preference tuning adds to the model, in permille.</summary>
        public int PrefLift;
        /// <summary>Share each further preference tuning loses of the one before (permille).</summary>
        public int PrefDecay;
        /// <summary>Incident chance every preference tuning adds, in permille.</summary>
        public int Syco;
        /// <summary>Alignment the first safety tuning adds, out of 1000.</summary>
        public int SafetyGain;
        /// <summary>Share each further safety tuning loses of the one before (permille).</summary>
        public int SafetyDecay;
        /// <summary>Revenue every safety tuning takes from the model, in permille: it refuses what customers ask.</summary>
        public int Refuse;
        /// <summary>Lowest the net revenue change of a model can fall (permille, negative).</summary>
        public int MinLift;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("weeks", ref Weeks);
            s.Int("cost", ref Cost);
            s.Int("units", ref Units);
            s.Int("maxPasses", ref MaxPasses);
            s.Int("prefLift", ref PrefLift);
            s.Int("prefDecay", ref PrefDecay);
            s.Int("syco", ref Syco);
            s.Int("safetyGain", ref SafetyGain);
            s.Int("safetyDecay", ref SafetyDecay);
            s.Int("refuse", ref Refuse);
            s.Int("minLift", ref MinLift);
        }
    }
}
