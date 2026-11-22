using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Agent autonomy for released models. See <see cref="AgencyConfig"/>. Only the player's lab.</summary>
    public static class Agency
    {
        public static bool Enabled(GameConfig c) { return c.Agency.Enabled > 0; }

        public static int Lift(GameConfig c, int level) { return level == 1 ? c.Agency.Lift1 : level == 2 ? c.Agency.Lift2 : level == 3 ? c.Agency.Lift3 : 0; }

        public static int Incident(GameConfig c, int level) { return level == 1 ? c.Agency.Incident1 : level == 2 ? c.Agency.Incident2 : level == 3 ? c.Agency.Incident3 : 0; }

        public static int BlowupBase(GameConfig c, int level) { return level == 2 ? c.Agency.Blowup2 : level == 3 ? c.Agency.Blowup3 : 0; }

        public static int Wings(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Rooms.Count; i++) if (lab.Rooms[i].TypeId == "containment-wing") n++;
            return n;
        }

        public static int RoomsNeeded(GameConfig c, int level) { return level == 2 ? c.Agency.Rooms2 : level == 3 ? c.Agency.Rooms3 : 0; }

        public static int BlowupChance(GameConfig c, LabState lab, ModelState model)
        {
            int cut = (int)MathI.Min(750, (long)Wings(lab) * c.Agency.RoomCut);
            long baseChance = BlowupBase(c, model.AgentLevel);
            if (Knowledge.Enabled(c)) baseChance += MathI.MulPermille(c.Agency.GapBlowup, model.KnowGap);
            return (int)MathI.MulPermille(baseChance, MathI.Permille - cut);
        }

        public static int IncidentPermille(GameConfig c, ModelState model)
        {
            return Enabled(c) && model.AgentLevel > 0 ? Incident(c, model.AgentLevel) : 0;
        }

        /// <summary>What the agents of the lab's models add to this week's revenue.</summary>
        public static long Lift(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return 0;
            long total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.AgentLevel <= 0 || m.Status != ModelStatus.Released) continue;
                total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), Lift(c, m.AgentLevel));
            }
            lab.AgentEarned += total;
            return total;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, ModelState model, int level)
        {
            int added = (int)MathI.Max(0, level - model.AgentLevel);
            long cost = added * (long)c.Agency.SetupCost;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            model.AgentLevel = level;
            model.AgentSince = level == 0 ? 0 : state.Tick + 1;
            if (level == 0) model.AgentStreak = 0;
            Events.Add(state, EventKind.AutonomySet, lab.Id, model.Id, level, model.Name);
        }

        static long Mix(GameState state, LabState lab, ModelState model)
        {
            long mix = (state.Seed * 41 + state.Tick) * 41 + lab.Id * 23 + model.Id * 13 + 7;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.AgentLevel <= 0) continue;
                if (m.Status != ModelStatus.Released)
                {
                    m.AgentLevel = 0;
                    m.AgentSince = 0;
                    m.AgentStreak = 0;
                    continue;
                }
                int chance = BlowupChance(c, lab, m);
                if (chance > 0 && (int)(Mix(state, lab, m) % MathI.Permille) < chance)
                {
                    long refund = MarketSystem.ModelRevenue(state, lab.Id, m.Id) * c.Agency.RefundWeeks;
                    m.AgentBlowups++;
                    m.AgentStreak = 0;
                    if (refund > 0) Finance.Spend(lab, LedgerLine.Fines, refund);
                    for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.Agency.BlowupTrust, false);
                    BoardMotions.AddTension(c, lab, c.BoardAgenda.BlowupTension);
                    Events.Add(state, EventKind.AgentBlowup, lab.Id, m.Id, refund, m.Name);
                    continue;
                }
                m.AgentStreak++;
                if (m.AgentStreak % c.Agency.StreakWeeks == 0)
                {
                    for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], c.Agency.StreakTrust, false);
                    Events.Add(state, EventKind.AgentStreak, lab.Id, m.Id, 0, m.Name);
                }
            }
        }
    }
}
