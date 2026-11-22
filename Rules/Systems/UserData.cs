using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The flywheel from the lab's own users. See <see cref="UserDataConfig"/>. Only the player's lab.</summary>
    public static class UserData
    {
        public static bool Enabled(GameConfig c) { return c.UserData.Enabled > 0; }

        public static int Rate(GameConfig c, LabState lab)
        {
            return lab.UserPolicy == 2 ? c.UserData.DefaultPermille : lab.UserPolicy == 1 ? c.UserData.OptInPermille : 0;
        }

        public static int UsersUnits(LabState lab) { return lab.DataRaw4 + lab.DataClean4; }

        public static void Set(GameConfig c, GameState state, LabState lab, int policy)
        {
            if (lab.UserPolicy == 2 && policy < 2)
            {
                int keep = c.UserData.KeepPermille;
                int before = UsersUnits(lab);
                lab.DataRaw4 = (int)MathI.MulPermille(lab.DataRaw4, keep);
                lab.DataClean4 = (int)MathI.MulPermille(lab.DataClean4, keep);
                lab.UserBanked = 0;
                Events.Add(state, EventKind.UserDataDeleted, lab.Id, 0, before - UsersUnits(lab), "");
            }
            lab.UserPolicy = policy;
            lab.UserSince = state.Tick + 1;
            Events.Add(state, EventKind.UserDataChanged, lab.Id, policy, 0, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.UserPolicy == 0) return;
            long base_ = lab.LastWeek.Revenue / c.UserData.RevenuePerUnit;
            long units = MathI.MulPermille(base_, Rate(c, lab));
            long room = c.Datasets.StockCap - Datasets.Stock(lab);
            if (units > room) units = room;
            if (units > 0)
            {
                lab.DataRaw4 += (int)units;
                lab.UserBanked += (int)units;
                lab.UserTotal += (int)units;
            }
            if (lab.UserPolicy != 2 || state.Tick % c.UserData.ComplaintWeeks != 0 || lab.UserBanked < c.UserData.ComplaintMin) return;
            long fine = (long)lab.UserBanked * c.UserData.ComplaintFine;
            int banked = lab.UserBanked;
            lab.UserBanked = 0;
            lab.UserComplaints++;
            if (fine > 0) Finance.Spend(lab, LedgerLine.Fines, fine);
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.UserData.ComplaintTrust, false);
            }
            Events.Add(state, EventKind.UserDataComplaint, lab.Id, banked, fine, "");
        }
    }
}
