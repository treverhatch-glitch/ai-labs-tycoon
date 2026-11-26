using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q163: labs that already researched a tech pay weekly license fees while another lab's patent blocks new research.</summary>
    public static class PatentLicenseSystem
    {
        public static int InfringerCount(GameConfig c, GameState state, LabState holder, string techId, int tick)
        {
            if (c.Research.Patents == null || holder == null || holder.Bankrupt) return 0;
            if (ResearchQueries.ActivePatent(holder, techId, tick) == null) return 0;
            int n = 0;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Id == holder.Id || lab.Bankrupt) continue;
                if (!lab.Techs.Contains(techId)) continue;
                if (ResearchQueries.ActivePatent(lab, techId, tick) != null) continue;
                n++;
            }
            return n;
        }

        public static long WeeklyLicenseCash(GameConfig c, GameState state, LabState holder, int tick)
        {
            if (c.Research.Patents == null) return 0;
            long fee = c.Research.Patents.LicenseCashPerInfringerPerWeek;
            if (fee <= 0 || holder.Patents.Count == 0) return 0;
            long total = 0;
            for (int p = 0; p < holder.Patents.Count; p++)
            {
                PatentState patent = holder.Patents[p];
                if (patent.ExpiresTick <= tick) continue;
                total += fee * InfringerCount(c, state, holder, patent.TechId, tick);
            }
            return total;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState holder)
        {
            if (holder.Bankrupt || holder.Patents.Count == 0) return;
            long fee = c.Research.Patents?.LicenseCashPerInfringerPerWeek ?? 0;
            if (fee <= 0) return;
            for (int p = 0; p < holder.Patents.Count; p++)
            {
                PatentState patent = holder.Patents[p];
                if (patent.ExpiresTick <= state.Tick) continue;
                string techId = patent.TechId;
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState infringer = state.Labs[i];
                    if (infringer.Id == holder.Id || infringer.Bankrupt) continue;
                    if (!infringer.Techs.Contains(techId)) continue;
                    if (ResearchQueries.ActivePatent(infringer, techId, state.Tick) != null) continue;
                    if (infringer.Cash < fee) continue;
                    Finance.Spend(infringer, LedgerLine.PatentLicense, fee);
                    Finance.Earn(holder, LedgerLine.PatentLicense, fee);
                }
            }
        }
    }
}