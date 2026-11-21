using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A device side line. See <see cref="DevicesConfig"/>. Only the player's lab.</summary>
    public static class Devices
    {
        public static bool Enabled(GameConfig c) { return c.Devices.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.DevLine > 0; }

        public static int Era(GameConfig c, int line) { return line == 1 ? c.Devices.EraA : line == 2 ? c.Devices.EraB : c.Devices.EraC; }

        public static long Cost(GameConfig c, int line) { return line == 1 ? c.Devices.CostA : line == 2 ? c.Devices.CostB : c.Devices.CostC; }

        public static int Weeks(GameConfig c, int line) { return line == 1 ? c.Devices.WeeksA : line == 2 ? c.Devices.WeeksB : c.Devices.WeeksC; }

        public static long Demand(GameConfig c, int line) { return line == 1 ? c.Devices.DemandA : line == 2 ? c.Devices.DemandB : c.Devices.DemandC; }

        public static long Margin(GameConfig c, int line) { return line == 1 ? c.Devices.MarginA : line == 2 ? c.Devices.MarginB : c.Devices.MarginC; }

        public static int EraNow(GameConfig c, GameState state) { return EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick)); }

        public static int Quality(GameConfig c, LabState lab, int testWeeks)
        {
            ModelState best = LabQueries.BestReleased(lab);
            long model = best == null ? 0 : MathI.Min(c.Devices.ModelQuality, best.Capability * (long)c.Devices.ModelQuality / c.Devices.CapForMax);
            return (int)MathI.Min(MathI.Permille, c.Devices.BaseQuality + model + testWeeks * (long)c.Devices.TestQuality);
        }

        public static int PricePermille(GameConfig c, int tier) { return tier == 1 ? c.Devices.LowPrice : tier == 3 ? c.Devices.HighPrice : MathI.Permille; }

        public static int DemandPermille(GameConfig c, int tier) { return tier == 1 ? c.Devices.LowDemand : tier == 3 ? c.Devices.HighDemand : MathI.Permille; }

        /// <summary>The units sold this week and what they clear.</summary>
        public static long UnitsThisWeek(GameConfig c, LabState lab)
        {
            long units = lab.DevBase * lab.DevQuality / MathI.Permille;
            units = MathI.MulPermille(units, DemandPermille(c, lab.DevTier));
            long fade = MathI.Min(900, lab.DevWeeks * (long)c.Devices.FadePermille / 10);
            units = MathI.MulPermille(units, MathI.Permille - fade);
            return MathI.Max(0, units);
        }

        public static long Clears(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(Margin(c, lab.DevLine), PricePermille(c, lab.DevTier));
        }

        public static void Design(GameConfig c, GameState state, LabState lab, int line, int testWeeks)
        {
            long cost = Cost(c, line);
            Finance.Spend(lab, LedgerLine.ChipDevelopment, cost);
            lab.DevLine = line;
            lab.DevStage = 1;
            lab.DevWeeks = Weeks(c, line) + testWeeks;
            lab.DevQuality = Quality(c, lab, testWeeks);
            lab.DevTier = 2;
            lab.DevSold = 0;
            lab.DevBase = 0;
            Events.Add(state, EventKind.DeviceDesigned, lab.Id, line, cost, "");
        }

        static void Clear(LabState lab)
        {
            lab.DevLine = 0;
            lab.DevStage = 0;
            lab.DevWeeks = 0;
            lab.DevQuality = 0;
            lab.DevTier = 0;
            lab.DevSold = 0;
            lab.DevBase = 0;
        }

        public static void Withdraw(GameConfig c, GameState state, LabState lab)
        {
            long sold = lab.DevSold;
            int line = lab.DevLine;
            Clear(lab);
            Events.Add(state, EventKind.DeviceWithdrawn, lab.Id, line, sold, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab)) return;
            if (lab.DevStage == 1)
            {
                lab.DevWeeks--;
                if (lab.DevWeeks > 0) return;
                bool hit = lab.DevQuality >= c.Devices.LaunchBar;
                lab.DevStage = 2;
                lab.DevWeeks = 0;
                lab.DevBase = Demand(c, lab.DevLine);
                if (hit) PressSystem.AddReputation(lab, c.Devices.HitReputation);
                else
                {
                    lab.DevBase = MathI.MulPermille(lab.DevBase, c.Devices.FlopDemand);
                    PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Devices.FlopReputation));
                }
                Events.Add(state, EventKind.DeviceLaunched, lab.Id, lab.DevLine, lab.DevQuality, "").Pick = hit ? 1 : 0;
                return;
            }
            long units = UnitsThisWeek(c, lab);
            lab.DevWeeks++;
            lab.DevSold += units;
            Finance.Earn(lab, LedgerLine.Revenue, units * Clears(c, lab));
            if (lab.DevWeeks >= c.Devices.RecallWeeks && lab.DevQuality < c.Devices.RecallBar)
            {
                long cost = lab.DevSold * c.Devices.RecallCost * lab.DevLine * lab.DevLine / 4;
                Finance.Spend(lab, LedgerLine.Fines, cost);
                int line = lab.DevLine;
                Clear(lab);
                PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Devices.FlopReputation));
                Events.Add(state, EventKind.DeviceRecalled, lab.Id, line, cost, "");
            }
        }
    }
}
