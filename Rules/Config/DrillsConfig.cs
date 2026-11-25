using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Incident drills (bible ch. 10, rehearsing what to do when a model fails). The lab can run a tabletop drill: it costs money, needs safety people, and takes weeks. When it finishes the team passes or fails; passing raises preparedness, failing lowers it. Preparedness fades slowly each week and faster if the lab goes too long without drilling. High preparedness cuts how often models fail in front of customers and how much reputation an incident costs, beside what on-call already does. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class DrillsConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for incident drills.</summary>
        public int MinEra;
        /// <summary>Cash to start a drill.</summary>
        public int Cost;
        /// <summary>Weeks a drill runs.</summary>
        public int Weeks;
        /// <summary>Weeks after a drill before preparedness fades faster.</summary>
        public int Interval;
        /// <summary>Safety people needed to run a drill.</summary>
        public int Staff;
        /// <summary>Preparedness gained when a drill passes (out of 1000).</summary>
        public int PassGain;
        /// <summary>Preparedness lost when a drill fails (out of 1000).</summary>
        public int FailLoss;
        /// <summary>Preparedness lost each week with no drill scheduled (out of 1000, small).</summary>
        public int Decay;
        /// <summary>Extra preparedness lost each week past the interval without a drill.</summary>
        public int IdleDecay;
        /// <summary>Share of preparedness that cuts model incident chance (permille of prep).</summary>
        public int IncidentCut;
        /// <summary>Share of preparedness that cuts reputation loss from incidents (permille of prep).</summary>
        public int RepCut;
        /// <summary>Extra on-call reputation-cut permille scaled by preparedness (stacks beside repCut).</summary>
        public int OnCallRepStack;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("cost", ref Cost);
            s.Int("weeks", ref Weeks);
            s.Int("interval", ref Interval);
            s.Int("staff", ref Staff);
            s.Int("passGain", ref PassGain);
            s.Int("failLoss", ref FailLoss);
            s.Int("decay", ref Decay);
            s.Int("idleDecay", ref IdleDecay);
            s.Int("incidentCut", ref IncidentCut);
            s.Int("repCut", ref RepCut);
            s.Int("onCallRepStack", ref OnCallRepStack);
        }
    }
}
