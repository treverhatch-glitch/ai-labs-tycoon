using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Hardware reliability. The player's GPUs fail by a small chance that grows with the age of the generation and with a heatwave at the site, worked out whole in the week so nobody rolls dice. A failed unit is lost unless a spare is stocked to take its place at once, or a warranty sends it back and replaces it with the current generation after some weeks. A warranty costs a fee for each unit a week, spares cost upkeep and a price to stock, and a waiting replacement can be rushed for a price. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ReliabilityConfig : ISync
    {
        public int Enabled;
        /// <summary>Failures in a million unit-weeks, for a unit of the newest generation.</summary>
        public int BaseFailPpm;
        /// <summary>What each year of the generation's age adds to that.</summary>
        public int AgePpm;
        /// <summary>The failure rate at a site in a heatwave, as a multiple in permille of the usual (1000 is no change).</summary>
        public int HeatwavePermille;
        /// <summary>Warranty fee for each unit a week at tier 1.</summary>
        public int FeeTier1;
        /// <summary>Warranty fee for each unit a week at tier 2.</summary>
        public int FeeTier2;
        /// <summary>Weeks a replacement takes at tier 1.</summary>
        public int RmaWeeksTier1;
        /// <summary>Weeks a replacement takes at tier 2.</summary>
        public int RmaWeeksTier2;
        /// <summary>What raising the warranty tier costs.</summary>
        public int SwitchFee;
        /// <summary>Most spares the lab can hold.</summary>
        public int MaxSpares;
        /// <summary>Upkeep for each spare a week.</summary>
        public int SpareUpkeep;
        /// <summary>Share of a unit's price that rushing a waiting replacement costs.</summary>
        public int ExpeditePermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("baseFailPpm", ref BaseFailPpm);
            s.Int("agePpm", ref AgePpm);
            s.Int("heatwavePermille", ref HeatwavePermille);
            s.Int("feeTier1", ref FeeTier1);
            s.Int("feeTier2", ref FeeTier2);
            s.Int("rmaWeeksTier1", ref RmaWeeksTier1);
            s.Int("rmaWeeksTier2", ref RmaWeeksTier2);
            s.Int("switchFee", ref SwitchFee);
            s.Int("maxSpares", ref MaxSpares);
            s.Int("spareUpkeep", ref SpareUpkeep);
            s.Int("expeditePermille", ref ExpeditePermille);
        }
    }
}
