using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Cloud-hosted chips from another lab's listing pay the design lab a weekly royalty share.</summary>
    public static class CloudChipLandlordSystem
    {
        public static void RunWeek(GameConfig c, GameState state)
        {
            int share = c.Chips.CloudLandlordSharePermille;
            if (share <= 0) return;
            var royalties = new Dictionary<int, long>();
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState renter = state.Labs[i];
                if (renter.Bankrupt) continue;
                for (int k = 0; k < renter.Fleet.Count; k++)
                {
                    HardwareBatch batch = renter.Fleet[k];
                    if (batch.DesignLabId < 0 || batch.DesignLabId == renter.Id) continue;
                    LabState landlord = state.Lab(batch.DesignLabId);
                    if (landlord == null || landlord.Bankrupt) continue;
                    long rent = ChipQueries.CloudBatchWeeklyRent(c, state, batch);
                    long pay = MathI.MulPermille(rent, share);
                    if (pay < 1) continue;
                    if (royalties.ContainsKey(landlord.Id)) royalties[landlord.Id] += pay;
                    else royalties[landlord.Id] = pay;
                }
            }
            foreach (KeyValuePair<int, long> pair in royalties)
            {
                LabState landlord = state.Lab(pair.Key);
                if (landlord == null || landlord.Bankrupt) continue;
                Finance.Earn(landlord, LedgerLine.Revenue, pair.Value);
            }
        }
    }
}