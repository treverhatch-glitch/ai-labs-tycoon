using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>An external audit of one model. See <see cref="SafetyAuditConfig"/>. Only the player's lab commissions one.</summary>
    public static class SafetyAudit
    {
        public static bool Enabled(GameConfig c) { return c.Audit.Weeks > 0; }

        public static bool Certified(GameState state, ModelState model) { return model.AuditedUntil > state.Tick; }

        public static int SafetyStaff(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Safety); }

        /// <summary>How many of the lab's models hold a certificate right now.</summary>
        public static int CertifiedModels(GameState state, LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (Certified(state, lab.Models[i])) n++;
            return n;
        }

        public static long Cost(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return MathI.Max(c.Audit.MinCost, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.Audit.RevenueWeeks);
        }

        /// <summary>The share off the monthly attention gain while a certificate is held, in permille.</summary>
        public static int AttentionReliefPermille(GameConfig c, GameState state, LabState lab)
        {
            return CertifiedModels(state, lab) > 0 ? c.Audit.AttentionReliefPermille : 0;
        }

        /// <summary>The reputation points added to a hearing while a certificate is held.</summary>
        public static int HearingPoints(GameConfig c, GameState state, LabState lab)
        {
            return CertifiedModels(state, lab) > 0 ? c.Audit.HearingPoints : 0;
        }

        public static void Run(GameConfig c, GameState state, LabState lab, ModelState model, long cost)
        {
            int gap = SafetySystem.AlignmentGap(c, model);
            model.DisplayedAlignment = model.Alignment;
            int pick = -1;
            if (gap <= c.Audit.CleanGap)
            {
                pick = 1;
                model.AuditedUntil = state.Tick + c.Audit.Weeks;
                lab.Reputation = (int)MathI.Clamp(lab.Reputation + c.Audit.CleanReputation, 0, MathI.Permille);
            }
            else if (gap >= c.Audit.ShameGap)
            {
                pick = 0;
                int loss = PressSystem.Damp(c, lab, c.Audit.ShameReputation);
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
            }
            Finance.Spend(lab, LedgerLine.Training, cost);
            SafetyCulture.Move(c, lab, c.Audit.CulturePoints);
            GameEvent e = Events.Add(state, EventKind.AuditDone, lab.Id, model.Id, cost, model.Name);
            e.Pick = pick;
        }

        public static void OnIncident(GameState state, LabState lab, ModelState model)
        {
            if (lab.Id != state.PlayerLabId || !Certified(state, model)) return;
            model.AuditedUntil = 0;
            Events.Add(state, EventKind.AuditRevoked, lab.Id, model.Id, 0, model.Name);
        }
    }
}
