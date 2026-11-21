using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The lab's stated values. The player picks two of safety, speed, openness, people and profit. Every week the lab's own acts are held up against them: acts in keeping add integrity, acts against cost more. Integrity high enough makes the lab an example; low enough makes the creed a joke. A town hall lifts it, owning up brings it back at a cost, and changing the creed starts from distrust. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class CreedConfig : ISync
    {
        public int Enabled;
        /// <summary>Integrity each act in keeping with a creed value adds.</summary>
        public int KeptPoints;
        /// <summary>Integrity each act against a creed value costs.</summary>
        public int BrokenPoints;
        /// <summary>Integrity when the lab first states its creed.</summary>
        public int StartIntegrity;
        /// <summary>Integrity when the lab changes its creed: nobody believes the new one yet.</summary>
        public int FlipIntegrity;
        /// <summary>Weeks after a creed is stated before it can be changed.</summary>
        public int FlipLockWeeks;
        /// <summary>Integrity at which the lab is held up as an example.</summary>
        public int HighBar;
        /// <summary>Integrity at which the creed is a joke.</summary>
        public int LowBar;
        /// <summary>How far back inside the band integrity must come before the lab can cross again.</summary>
        public int Gap;
        /// <summary>Reputation earned when the lab is held up as an example.</summary>
        public int HighReputation;
        /// <summary>Morale every person gains then.</summary>
        public int HighMorale;
        /// <summary>Reputation lost when the creed becomes a joke (damped).</summary>
        public int LowReputation;
        /// <summary>Morale every person loses then.</summary>
        public int LowMorale;
        /// <summary>What a town hall costs.</summary>
        public int TownCost;
        /// <summary>Integrity a town hall adds.</summary>
        public int TownGain;
        /// <summary>Weeks before another town hall or owning up.</summary>
        public int TownCooldown;
        /// <summary>Reputation owning up costs.</summary>
        public int OwnUpReputation;
        /// <summary>Integrity owning up brings the lab back to, at least.</summary>
        public int OwnUpRestore;
        /// <summary>What a backlash costs a lab that is held up as an example, in permille of the hit.</summary>
        public int ShieldPermille;
        /// <summary>What a backlash costs a lab whose creed is a joke, in permille of the hit.</summary>
        public int ExposePermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("keptPoints", ref KeptPoints);
            s.Int("brokenPoints", ref BrokenPoints);
            s.Int("startIntegrity", ref StartIntegrity);
            s.Int("flipIntegrity", ref FlipIntegrity);
            s.Int("flipLockWeeks", ref FlipLockWeeks);
            s.Int("highBar", ref HighBar);
            s.Int("lowBar", ref LowBar);
            s.Int("gap", ref Gap);
            s.Int("highReputation", ref HighReputation);
            s.Int("highMorale", ref HighMorale);
            s.Int("lowReputation", ref LowReputation);
            s.Int("lowMorale", ref LowMorale);
            s.Int("townCost", ref TownCost);
            s.Int("townGain", ref TownGain);
            s.Int("townCooldown", ref TownCooldown);
            s.Int("ownUpReputation", ref OwnUpReputation);
            s.Int("ownUpRestore", ref OwnUpRestore);
            s.Int("shieldPermille", ref ShieldPermille);
            s.Int("exposePermille", ref ExposePermille);
        }
    }
}
