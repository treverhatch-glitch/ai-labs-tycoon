using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Customer integrations on model versions. See <see cref="IntegrationsConfig"/>. Only the player's lab.</summary>
    public static class Integrations
    {
        public static bool Enabled(GameConfig c) { return c.Integrations.Enabled > 0; }

        public static int OpenCount(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].ApiState > 0) n++;
            return n;
        }

        public static int LiftPermille(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.ApiState == 0) return 0;
            return (int)MathI.Min(c.Integrations.LiftCapPermille, (long)(model.Integrations / 10) * c.Integrations.LiftPermillePer10);
        }

        public static long Lift(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return 0;
            long total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                int lift = LiftPermille(c, m);
                if (lift > 0) total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), lift);
            }
            return total;
        }

        public static int UpkeepPermille(GameConfig c, ModelState model)
        {
            return (int)MathI.Min(MathI.Permille, c.Integrations.UpkeepBasePermille + (long)(model.Integrations / 10) * c.Integrations.UpkeepPer10Permille);
        }

        public static void Open(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.ApiState = 1;
            Events.Add(state, EventKind.ApiOpened, lab.Id, model.Id, 0, model.Name);
        }

        public static void Deprecate(GameConfig c, GameState state, LabState lab, ModelState model, int successorId, int weeks)
        {
            model.ApiState = 2;
            model.ApiLeft = weeks;
            model.ApiSuccessor = successorId;
            Events.Add(state, EventKind.ApiDeprecated, lab.Id, model.Id, weeks, model.Name);
        }

        public static void Cancel(GameConfig c, LabState lab, ModelState model)
        {
            model.ApiState = 1;
            model.ApiLeft = 0;
            model.ApiSuccessor = 0;
        }

        static void End(GameConfig c, GameState state, LabState lab, ModelState m)
        {
            int stranded = m.Integrations;
            if (stranded > 0)
            {
                lab.ApiLost += stranded;
                int drop = (int)MathI.Min(c.Integrations.StrandTrustCap, (long)(stranded / 100) * c.Integrations.StrandTrustPer100);
                if (drop == 0 && stranded > 0) drop = 1;
                for (int s = 0; s < m.Sectors.Count; s++)
                    TrustSystem.Shift(c, state, lab, m.Sectors[s], -drop, false);
            }
            m.Integrations = 0;
            m.ApiState = 0;
            m.ApiLeft = 0;
            m.ApiSuccessor = 0;
            Events.Add(state, EventKind.ApiEnded, lab.Id, m.Id, stranded, m.Name).Pick = stranded == 0 ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.ApiState == 0) continue;
                long upkeep = MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), UpkeepPermille(c, m));
                if (upkeep > 0) Finance.Spend(lab, LedgerLine.Salaries, upkeep);
                if (m.ApiState == 1)
                {
                    if (m.Status == ModelStatus.Released && m.Sectors.Count > 0)
                        m.Integrations = (int)MathI.Min(c.Integrations.MaxIntegrations, m.Integrations + c.Integrations.SeedPerWeek + MathI.MulPermille(m.Integrations, c.Integrations.GrowthPermille));
                    continue;
                }
                ModelState next = m.ApiSuccessor > 0 ? lab.Model(m.ApiSuccessor) : null;
                if (next != null && next.ApiState == 1 && m.Integrations > 0)
                {
                    int moving = (int)MathI.Max(1, MathI.MulPermille(m.Integrations, c.Integrations.MigratePermille));
                    moving = (int)MathI.Min(moving, m.Integrations);
                    int churn = (int)MathI.MulPermille(moving, c.Integrations.ChurnPermille);
                    m.Integrations -= moving;
                    next.Integrations = (int)MathI.Min(c.Integrations.MaxIntegrations, next.Integrations + moving - churn);
                    lab.ApiMoved += moving - churn;
                    lab.ApiLost += churn;
                }
                m.ApiLeft--;
                if (m.Integrations == 0 || m.ApiLeft <= 0) End(c, state, lab, m);
            }
        }
    }
}
