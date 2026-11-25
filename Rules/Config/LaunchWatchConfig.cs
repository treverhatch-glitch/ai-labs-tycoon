using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A desk that watches rival launches. With it open, a rival model that beats the lab best released model brings a loss unless the lab answers (counter-programme for cash, or own the safety story if its model is safe enough); a rival launch the lab already beats earns a little reputation. It only reads what rivals did: it never changes what they do. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LaunchWatchConfig : ISync
    {
        public int Enabled;
        /// <summary>Cash to set up the watch desk.</summary>
        public int OpenCost;
        /// <summary>Cash each week the desk stays open.</summary>
        public int WeeklyCost;
        /// <summary>Weeks a rival launch waits for an answer before it is taken as held.</summary>
        public int WaitWeeks;
        /// <summary>Reputation lost when a rival launch that beats the lab lands unanswered, at an even lead (damped; 0.5 to 1.5 times with the lead).</summary>
        public int LossReputation;
        /// <summary>Cash per person to counter-programme.</summary>
        public int CounterPerStaff;
        /// <summary>Share of the loss that still lands after a counter-programme.</summary>
        public int CounterKeepPermille;
        /// <summary>Alignment of the lab best released model needed to own the safety story.</summary>
        public int SafetyBar;
        /// <summary>Reputation won by owning the safety story.</summary>
        public int SafetyGain;
        /// <summary>Reputation won when a rival launches something the lab already beats.</summary>
        public int LeadGain;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("openCost", ref OpenCost);
            s.Int("weeklyCost", ref WeeklyCost);
            s.Int("waitWeeks", ref WaitWeeks);
            s.Int("lossReputation", ref LossReputation);
            s.Int("counterPerStaff", ref CounterPerStaff);
            s.Int("counterKeepPermille", ref CounterKeepPermille);
            s.Int("safetyBar", ref SafetyBar);
            s.Int("safetyGain", ref SafetyGain);
            s.Int("leadGain", ref LeadGain);
        }
    }
}
