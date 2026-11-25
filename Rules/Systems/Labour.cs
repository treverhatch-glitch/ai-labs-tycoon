using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Labour relations. See <see cref="LabourConfig"/>. Only the player's lab is organised.</summary>
    public static class Labour
    {
        public const int Quiet = 0;
        public const int Organising = 1;
        public const int Union = 2;
        public const int Resisting = 3;
        public const int Strike = 4;

        public static bool Enabled(GameConfig c) { return c.Labour.Enabled > 0; }

        public static bool Contract(LabState lab) { return lab.LabourStage == Union; }

        public static bool OnStrike(LabState lab) { return lab.LabourStage == Strike; }

        /// <summary>Crunch time cannot start under a contract or during a strike.</summary>
        public static bool CrunchBlocked(LabState lab) { return lab.LabourStage == Union || lab.LabourStage == Strike; }

        static bool Hard(LabState lab) { return lab.LabourHard == 1; }

        /// <summary>What the contract adds to the pay bill, in permille.</summary>
        public static int PayPermille(GameConfig c, LabState lab)
        {
            return Contract(lab) ? (Hard(lab) ? c.Labour.HardPayPermille : c.Labour.PayPermille) : 0;
        }

        /// <summary>What the contract does to severance, in permille: 1000 when there is none.</summary>
        public static int SeverancePermille(GameConfig c, LabState lab)
        {
            return Contract(lab) ? (Hard(lab) ? c.Labour.HardSeverancePermille : c.Labour.SeverancePermille) : MathI.Permille;
        }

        public static long Severance(GameConfig c, LabState lab, long amount) { return MathI.MulPermille(amount, SeverancePermille(c, lab)); }

        /// <summary>Research gained under a contract and lost during a strike, in permille.</summary>
        public static int ResearchPermille(GameConfig c, LabState lab)
        {
            if (OnStrike(lab)) return -c.Labour.StrikeCutPermille;
            return Contract(lab) ? (Hard(lab) ? c.Labour.HardResearchPermille : c.Labour.ResearchPermille) : 0;
        }

        public static int TrainingPermille(GameConfig c, LabState lab) { return OnStrike(lab) ? -c.Labour.StrikeCutPermille : 0; }

        /// <summary>A layoff round raises pressure by the share it lets go.</summary>
        public static void OnLayoff(GameConfig c, LabState lab, int sharePermille)
        {
            if (!Enabled(c) || !Builds(lab)) return;
            lab.LabourPressure = (int)MathI.Clamp(lab.LabourPressure + (long)c.Labour.LayoffPerFull * sharePermille / MathI.Permille, 0, MathI.Permille);
        }

        /// <summary>Pressure builds in the quiet, organising and resisting stages.</summary>
        static bool Builds(LabState lab) { return lab.LabourStage == Quiet || lab.LabourStage == Organising || lab.LabourStage == Resisting; }

        static int Added(GameConfig c, LabState lab)
        {
            long add = 0;
            if (lab.Staff.Count >= c.Labour.MinStaff)
            {
                if (Crunch.AverageMorale(lab) < c.Labour.MoraleBelow) add += c.Labour.LowMoralePerWeek;
                if (Crunch.Active(lab)) add += c.Labour.CrunchPerWeek;
            }
            if (lab.LabourStage == Resisting) add = MathI.MulPermille(add, c.Labour.ResistFactorPermille);
            return (int)add;
        }

        public static void Recognise(GameConfig c, GameState state, LabState lab, bool chosen)
        {
            lab.LabourStage = Union;
            lab.LabourHard = chosen ? 0 : 1;
            lab.LabourPressure = 0;
            lab.LabourDue = 0;
            if (chosen && c.Labour.RecogniseMorale > 0)
                for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Min(MathI.Permille, lab.Staff[i].Morale + c.Labour.RecogniseMorale);
            GameEvent e = Events.Add(state, EventKind.LabourRecognised, lab.Id, -1, PayPermille(c, lab), "");
            e.Pick = chosen ? 1 : 0;
        }

        public static void Resist(GameConfig c, GameState state, LabState lab, bool chosen)
        {
            lab.LabourStage = Resisting;
            lab.LabourDue = state.Tick + c.Labour.ResistWeeks;
            if (chosen) Finance.Spend(lab, LedgerLine.Fines, c.Labour.ResistCost);
            GameEvent e = Events.Add(state, EventKind.LabourResisted, lab.Id, -1, chosen ? c.Labour.ResistCost : 0, "");
            e.Pick = chosen ? 1 : 0;
        }

        static void GoOut(GameConfig c, GameState state, LabState lab)
        {
            lab.LabourStage = Strike;
            lab.LabourDue = state.Tick + 1 + c.Labour.StrikeWeeks;
            lab.LabourPressure = 0;
            for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - c.Labour.StrikeMorale);
            Events.Add(state, EventKind.LabourStrike, lab.Id, c.Labour.StrikeMorale, c.Labour.StrikeWeeks, "");
        }

        /// <summary>One week: pressure builds and falls, and the stage moves on when its time is up.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int due = lab.LabourDue;
            switch (lab.LabourStage)
            {
                case Strike:
                    if (state.Tick + 1 >= due) Recognise(c, state, lab, false);
                    return;
                case Union:
                    return;
            }
            lab.LabourPressure = (int)MathI.Clamp((long)lab.LabourPressure + Added(c, lab) - c.Labour.DecayPerWeek, 0, MathI.Permille);
            if (lab.LabourStage == Quiet)
            {
                if (lab.LabourPressure < c.Labour.DrivePressure) return;
                lab.LabourStage = Organising;
                lab.LabourDue = state.Tick + 1 + c.Labour.WindowWeeks;
                Events.Add(state, EventKind.LabourDrive, lab.Id, c.Labour.WindowWeeks, lab.LabourPressure, "");
                return;
            }
            if (lab.LabourStage == Organising)
            {
                if (state.Tick + 1 >= due) Resist(c, state, lab, false);
                return;
            }
            if (lab.LabourPressure >= MathI.Permille) { GoOut(c, state, lab); return; }
            if (state.Tick + 1 < due) return;
            if (lab.LabourPressure < c.Labour.DrivePressure)
            {
                lab.LabourStage = Quiet;
                lab.LabourDue = 0;
                lab.LabourPressure = c.Labour.HeldPressure;
                Events.Add(state, EventKind.LabourHeld, lab.Id, -1, lab.LabourPressure, "");
            }
            else GoOut(c, state, lab);
        }
    }
}
