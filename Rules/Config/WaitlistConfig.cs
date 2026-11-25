using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A public waitlist for a model that is not out yet. Hype builds each week the list stays open; shipping before the promised weeks run out cashes the hype in as reputation, and a lapse spends it as a loss. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class WaitlistConfig : ISync
    {
        public int Enabled;
        /// <summary>Fewest weeks a waitlist may promise.</summary>
        public int MinWeeks;
        /// <summary>Most weeks a waitlist may promise.</summary>
        public int MaxWeeks;
        /// <summary>Hype a waitlist gains each week (0 to 1000).</summary>
        public int HypeStep;
        /// <summary>Reputation earned per 1000 hype when the model ships in time.</summary>
        public int WinPerThousand;
        /// <summary>Reputation lost per 1000 hype when the promise lapses (damped).</summary>
        public int LossPerThousand;
        /// <summary>Reputation lost per 1000 hype for closing the list early (damped).</summary>
        public int ClosePerThousand;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minWeeks", ref MinWeeks);
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("hypeStep", ref HypeStep);
            s.Int("winPerThousand", ref WinPerThousand);
            s.Int("lossPerThousand", ref LossPerThousand);
            s.Int("closePerThousand", ref ClosePerThousand);
        }
    }
}
