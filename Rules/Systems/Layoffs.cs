using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>A layoff round. See <see cref="LayoffsConfig"/>. Only the player's lab does it.</summary>
    public static class Layoffs
    {
        public static bool Enabled(GameConfig c) { return c.Layoffs.Enabled > 0; }

        /// <summary>How many people a round at this share lets go from a function (whole people, rounded down).</summary>
        public static int CutIn(LabState lab, string roleId, int permille)
        {
            return (int)((long)LabQueries.StaffCount(lab, roleId) * permille / MathI.Permille);
        }

        public static int TotalCut(GameConfig c, LabState lab, int permille)
        {
            int total = 0;
            for (int i = 0; i < c.Roles.Count; i++) total += CutIn(lab, c.Roles[i].Id, permille);
            return total;
        }

        /// <summary>What a round at this share costs: the role's severance, reduced, for each person.</summary>
        public static long Package(GameConfig c, LabState lab, int permille)
        {
            long cost = 0;
            for (int i = 0; i < c.Roles.Count; i++)
                cost += Labour.Severance(c, lab, MathI.MulPermille(c.Roles[i].Severance * (long)CutIn(lab, c.Roles[i].Id, permille), c.Layoffs.SeverancePermille));
            return cost;
        }

        /// <summary>The share of the whole staff a round at this share actually lets go, in permille.</summary>
        public static int StaffSharePermille(GameConfig c, LabState lab, int permille)
        {
            int staff = lab.Staff.Count;
            if (staff <= 0) return 0;
            return (int)((long)TotalCut(c, lab, permille) * MathI.Permille / staff);
        }

        public static int ReputationLoss(GameConfig c, int sharePermille) { return (int)((long)c.Layoffs.ReputationAtAll * sharePermille / MathI.Permille); }

        public static int MoraleLoss(GameConfig c, int sharePermille) { return (int)((long)c.Layoffs.MoraleAtAll * sharePermille / MathI.Permille); }

        public static void Run(GameConfig c, GameState state, LabState lab, int permille)
        {
            int share = StaffSharePermille(c, lab, permille);
            long package = Package(c, lab, permille);
            int total = 0;
            var leavers = Alumni.Roster(lab);
            for (int i = 0; i < c.Roles.Count; i++)
            {
                RoleConfig role = c.Roles[i];
                int cut = CutIn(lab, role.Id, permille);
                if (cut <= 0) continue;
                StaffSystem.Fire(lab, role.Id, cut);
                ControlStale.OnFired(c, lab, role.Id, cut);
                total += cut;
            }
            Alumni.RecordGone(c, state, lab, leavers, Alumni.LaidOff);
            TalentSystem.DropGoneOffers(lab);
            Finance.Spend(lab, LedgerLine.Hiring, package);
            int morale = MoraleLoss(c, share);
            if (morale > 0)
                for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - morale);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - ReputationLoss(c, share));
            Labour.OnLayoff(c, lab, share);
            GameEvent e = Events.Add(state, EventKind.LayoffRound, lab.Id, share, total, "");
            e.Pick = share >= c.Layoffs.DeepPermille ? 1 : 0;
        }
    }
}
