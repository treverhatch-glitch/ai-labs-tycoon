using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Safety culture and the release gate. See <see cref="CultureConfig"/>. Only the player's labs keep a culture that moves.</summary>
    public static class SafetyCulture
    {
        public struct Report
        {
            public int Level;
            public string BandId;
            public int ListeningStaff;
            public int StaffGain;
            public int Drift;
            public int CatchPermille;
            public int Gate;
            /// <summary>The tick from which the gate can be loosened, or 0 when there is no gate.</summary>
            public int GateLockedUntilTick;
            public int WeeklyChange { get { return StaffGain - Drift; } }
        }

        /// <summary>A lab's culture, 0 to 1000. It is saved as the shift from the starting level.</summary>
        public static int Level(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp((long)c.Culture.Start + lab.CultureShift, 0, MathI.Permille);
        }

        public static CultureBandConfig Band(GameConfig c, int level)
        {
            CultureBandConfig best = null;
            for (int i = 0; i < c.Culture.Bands.Count; i++)
                if (c.Culture.Bands[i].Min <= level && (best == null || c.Culture.Bands[i].Min >= best.Min)) best = c.Culture.Bands[i];
            return best;
        }

        /// <summary>The permille of incidents caught that culture adds (or, when negative, takes away).</summary>
        public static int CatchPermille(GameConfig c, LabState lab)
        {
            return (int)((long)(Level(c, lab) - c.Culture.NeutralLevel) * c.Culture.CatchSlopePermille / MathI.Permille);
        }

        public static int ListeningStaff(GameConfig c, LabState lab)
        {
            int n = 0;
            for (int i = 0; i < c.Culture.ListeningRoles.Count; i++) n += LabQueries.StaffCount(lab, c.Culture.ListeningRoles[i]);
            return n;
        }

        public static int StaffGain(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(c.Culture.MaxStaffGain, (long)ListeningStaff(c, lab) * c.Culture.StaffGainPerPerson);
        }

        public static Report Of(GameConfig c, LabState lab)
        {
            var r = new Report();
            r.Level = Level(c, lab);
            CultureBandConfig band = Band(c, r.Level);
            r.BandId = band == null ? "" : band.Id;
            r.ListeningStaff = ListeningStaff(c, lab);
            r.StaffGain = StaffGain(c, lab);
            r.Drift = c.Culture.DriftPerWeek;
            r.CatchPermille = CatchPermille(c, lab);
            r.Gate = lab.ReleaseGate;
            r.GateLockedUntilTick = lab.ReleaseGate > 0 ? lab.GateSetTick + c.Culture.GateLockWeeks : 0;
            return r;
        }

        static bool Moves(LabState lab) { return lab.Controller == LabController.Player; }

        /// <summary>Moves a player lab's culture by points, staying between 0 and 1000.</summary>
        public static void Move(GameConfig c, LabState lab, int points)
        {
            if (!Moves(lab) || points == 0) return;
            int level = (int)MathI.Clamp((long)Level(c, lab) + points, 0, MathI.Permille);
            lab.CultureShift = level - c.Culture.Start;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            Move(c, lab, StaffGain(c, lab) - c.Culture.DriftPerWeek);
        }

        /// <summary>True when the lab's own gate stops this model going on sale.</summary>
        public static bool GateBlocks(LabState lab, ModelState model)
        {
            return lab.ReleaseGate > 0 && SafetySystem.DisplayedAlignment(model) < lab.ReleaseGate;
        }

        /// <summary>A model goes on sale for the first time: rushed if it shows too little alignment, otherwise credit for keeping the gate.</summary>
        public static void OnFirstRelease(GameConfig c, LabState lab, ModelState model)
        {
            if (SafetySystem.DisplayedAlignment(model) < c.Culture.RushBelowAlignment) Move(c, lab, -c.Culture.RushPoints);
            else if (lab.ReleaseGate > 0) Move(c, lab, c.Culture.GateKeptPoints);
        }

        /// <summary>True when the gate may be set to this value now: raising is always allowed, loosening only once the lock has run out.</summary>
        public static bool GateLocked(GameConfig c, GameState state, LabState lab, int gate)
        {
            return gate < lab.ReleaseGate && state.Tick < lab.GateSetTick + c.Culture.GateLockWeeks;
        }

        public static void SetGate(GameConfig c, GameState state, LabState lab, int gate)
        {
            if (gate == lab.ReleaseGate) return;
            if (gate < lab.ReleaseGate) Move(c, lab, -c.Culture.GateLoweredPoints);
            lab.ReleaseGate = gate;
            lab.GateSetTick = state.Tick;
        }

        public static void OnComplaintAnswered(GameConfig c, LabState lab) { Move(c, lab, c.Culture.ComplaintAnsweredPoints); }

        public static void OnComplaintLapsed(GameConfig c, LabState lab) { Move(c, lab, -c.Culture.ComplaintLapsedPoints); }
    }
}
