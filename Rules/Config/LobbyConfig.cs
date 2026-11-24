using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Lobbying (bible ch. 12: "spend on influence to slow or shape rules, win subsidies; it can backfire as a scandal"; ch. 14 room list: the
    /// policy and legal room is for lobbying, compliance and hearings). The player pays for a push on one government, once in a while. A quiet word
    /// takes some of that government's attention off the lab. A subsidy push might win a grant. Policy staff make both safer. Rivals and the autopilot
    /// never lobby (the rivals' feuds still send the player's government their own attention, in the rivals' plans).
    /// </summary>
    public sealed class LobbyConfig : ISync
    {
        /// <summary>Weeks before the same government can be lobbied again.</summary>
        public int CooldownWeeks;
        /// <summary>The least a week of running the lab is counted at when a push is priced.</summary>
        public int MinWeekCost;
        /// <summary>A quiet word costs this many weeks of running the lab.</summary>
        public int QuietCostWeeks;
        /// <summary>Attention (of 1000) a quiet word takes off, when it lands.</summary>
        public int QuietAttention;
        /// <summary>A subsidy push costs this many weeks of running the lab.</summary>
        public int SubsidyCostWeeks;
        /// <summary>A subsidy that is won pays this many weeks of running the lab.</summary>
        public int SubsidyReturnWeeks;
        /// <summary>The chance a subsidy push wins, in permille, before policy staff.</summary>
        public int SubsidyChancePermille;
        /// <summary>Added to the subsidy chance for each policy person.</summary>
        public int SubsidyChancePermillePerStaff;
        /// <summary>The subsidy chance never goes above this.</summary>
        public int SubsidyChanceCapPermille;
        /// <summary>The chance a push becomes a scandal, in permille, before policy staff.</summary>
        public int BackfirePermille;
        /// <summary>Taken off the backfire chance for each policy person.</summary>
        public int BackfireSavingPermillePerStaff;
        /// <summary>The backfire chance never goes below this.</summary>
        public int BackfireFloorPermille;
        /// <summary>Attention a scandal adds in that government.</summary>
        public int BackfireAttention;
        /// <summary>Reputation a scandal costs (the press room can soften it).</summary>
        public int BackfireReputation;

        public void Sync(Syncer s)
        {
            s.Int("cooldownWeeks", ref CooldownWeeks);
            s.Int("minWeekCost", ref MinWeekCost);
            s.Int("quietCostWeeks", ref QuietCostWeeks);
            s.Int("quietAttention", ref QuietAttention);
            s.Int("subsidyCostWeeks", ref SubsidyCostWeeks);
            s.Int("subsidyReturnWeeks", ref SubsidyReturnWeeks);
            s.Int("subsidyChancePermille", ref SubsidyChancePermille);
            s.Int("subsidyChancePermillePerStaff", ref SubsidyChancePermillePerStaff);
            s.Int("subsidyChanceCapPermille", ref SubsidyChanceCapPermille);
            s.Int("backfirePermille", ref BackfirePermille);
            s.Int("backfireSavingPermillePerStaff", ref BackfireSavingPermillePerStaff);
            s.Int("backfireFloorPermille", ref BackfireFloorPermille);
            s.Int("backfireAttention", ref BackfireAttention);
            s.Int("backfireReputation", ref BackfireReputation);
        }
    }
}
