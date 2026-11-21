using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Retraining one person for another role. See <see cref="RetrainConfig"/>. Only the player's lab does it.</summary>
    public static class Retraining
    {
        public static bool Enabled(GameConfig c) { return c.Retrain.Enabled; }

        public static StaffMember Person(LabState lab, int personId)
        {
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].Id == personId) return lab.Staff[i];
            return null;
        }

        public static bool SameFunction(RoleConfig from, RoleConfig to) { return from.Function == to.Function; }

        public static long Fee(GameConfig c, LabState lab, RoleConfig from, RoleConfig to, int tick)
        {
            int permille = SameFunction(from, to) ? c.Retrain.SameFunctionFeePermille : c.Retrain.FeePermille;
            return MathI.MulPermille(RolePerks.HireFee(c, lab, to, tick), permille);
        }

        public static int MoraleDrop(GameConfig c, RoleConfig from, RoleConfig to)
        {
            return SameFunction(from, to) ? c.Retrain.SameFunctionMoraleDrop : c.Retrain.MoraleDrop;
        }

        public static void Retrain(GameConfig c, GameState state, LabState lab, StaffMember person, RoleConfig to, long fee)
        {
            RoleConfig from = c.Role(person.RoleId);
            int drop = MoraleDrop(c, from, to);
            bool same = SameFunction(from, to);
            string fromId = person.RoleId;
            if (c.Culture.ListeningRoles.Contains(fromId) && !c.Culture.ListeningRoles.Contains(to.Id)) ControlStale.OnFired(c, lab, fromId, 1);
            person.RoleId = to.Id;
            person.Morale = (int)MathI.Max(0, person.Morale - drop);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            GameEvent e = Events.Add(state, EventKind.StaffRetrained, lab.Id, person.Id, fee, fromId + ">" + to.Id);
            e.Pick = same ? 1 : 0;
        }
    }
}
