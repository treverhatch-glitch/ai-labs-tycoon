using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Cloud-hosted chips from another lab's listing pay the design lab a weekly royalty share.</summary>
    public static class CloudChipLandlordSystem
    {
        public struct RevenueBreakdown
        {
            /// <summary>Sum of scaled cloud rent on batches designed by this landlord (before royalty share).</summary>
            public long GrossCloudRent;
            /// <summary>What <see cref="RunWeek"/> pays the landlord (gross × share permille).</summary>
            public long Royalty;
        }

        /// <summary>Royalty the design lab earns this week from others' cloud batches (matches <see cref="RunWeek"/>).</summary>
        public static long WeeklyLandlordRevenue(GameConfig c, GameState state, LabState landlord)
        {
            return RevenueParts(c, state, landlord).Royalty;
        }

        public static RevenueBreakdown RevenueParts(GameConfig c, GameState state, LabState landlord)
        {
            var b = new RevenueBreakdown();
            if (landlord.Bankrupt) return b;
            int share = c.Chips.CloudLandlordSharePermille;
            if (share <= 0) return b;
            AccumulateRenters(c, state, share, null, ref b);
            return b;
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            int share = c.Chips.CloudLandlordSharePermille;
            if (share <= 0) return;
            var royalties = new Dictionary<int, long>();
            var scratch = new RevenueBreakdown();
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState landlord = state.Labs[i];
                if (landlord.Bankrupt) continue;
                scratch = new RevenueBreakdown();
                AccumulateRenters(c, state, share, landlord.Id, ref scratch);
                if (scratch.Royalty > 0) royalties[landlord.Id] = scratch.Royalty;
            }
            foreach (KeyValuePair<int, long> pair in royalties)
            {
                LabState landlord = state.Lab(pair.Key);
                if (landlord == null || landlord.Bankrupt) continue;
                if (landlord.CloudRoyaltyPauseUntilTick > state.Tick) continue;
                Finance.Earn(landlord, LedgerLine.Revenue, pair.Value);
            }
        }

        static void AccumulateRenters(GameConfig c, GameState state, int share, int? landlordId, ref RevenueBreakdown into)
        {
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState renter = state.Labs[i];
                if (renter.Bankrupt) continue;
                for (int k = 0; k < renter.Fleet.Count; k++)
                {
                    HardwareBatch batch = renter.Fleet[k];
                    if (batch.DesignLabId < 0 || batch.DesignLabId == renter.Id) continue;
                    if (landlordId.HasValue && batch.DesignLabId != landlordId.Value) continue;
                    LabState landlord = state.Lab(batch.DesignLabId);
                    if (landlord == null || landlord.Bankrupt) continue;
                    long rent = ChipQueries.CloudBatchWeeklyRent(c, state, batch);
                    long pay = MathI.MulPermille(rent, share);
                    if (pay < 1) continue;
                    into.GrossCloudRent += rent;
                    into.Royalty += pay;
                }
            }
        }
    }
}