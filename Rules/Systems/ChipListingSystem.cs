using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q168: chip listings need upkeep, caps, spotlighting, and stale auto-delist.</summary>
    public static class ChipListingSystem
    {
        public static long WeeklyUpkeep(GameConfig c, LabState lab)
        {
            ChipsConfig k = c.Chips;
            if (k.ListingWeeklyUpkeep <= 0 || lab == null) return 0;
            long total = 0;
            for (int i = 0; i < lab.ChipListings.Count; i++)
            {
                ChipListing listing = lab.ChipListings[i];
                long each = k.ListingWeeklyUpkeep;
                if (listing.Spotlight && k.ListingSpotlightUpkeepPermille > 0)
                    each = MathI.MulPermille(each, k.ListingSpotlightUpkeepPermille);
                total += each;
            }
            return total;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt) return;
            PruneInvalid(c, state, lab);
            PruneStale(c, state, lab);
            EnforceCap(c, state, lab);
            long upkeep = WeeklyUpkeep(c, lab);
            if (upkeep > 0 && lab.Cash >= upkeep)
                Finance.Spend(lab, LedgerLine.ChipDevelopment, upkeep);
        }

        public static CommandResult SetSpotlight(GameConfig c, GameState state, LabState lab, int designId, bool on)
        {
            ChipsConfig k = c.Chips;
            ChipDesign design = lab.Design(designId);
            if (design == null || design.Status != ChipDesignStatus.Ready)
                return CommandResult.Fail(CommandError.InvalidArgument, "That design is not ready to list.");
            ChipListing listing = lab.Listing(designId);
            if (listing == null)
                return CommandResult.Fail(CommandError.InvalidArgument, "List the design before spotlighting it.");
            if (on && !listing.Spotlight && k.ListingSpotlightCashCost > 0)
            {
                if (lab.Cash < k.ListingSpotlightCashCost)
                    return CommandResult.Fail(CommandError.NotEnoughCash, "Need $" + k.ListingSpotlightCashCost + " to spotlight a listing.");
                Finance.Spend(lab, LedgerLine.ChipDevelopment, k.ListingSpotlightCashCost);
            }
            listing.Spotlight = on;
            return CommandResult.Done(designId);
        }

        static void PruneInvalid(GameConfig c, GameState state, LabState lab)
        {
            for (int i = lab.ChipListings.Count - 1; i >= 0; i--)
            {
                ChipListing listing = lab.ChipListings[i];
                ChipDesign design = lab.Design(listing.DesignId);
                if (design != null && design.Status == ChipDesignStatus.Ready) continue;
                lab.ChipListings.RemoveAt(i);
                Events.Add(state, EventKind.ChipListingEnded, lab.Id, listing.DesignId, 0, "not-ready");
            }
        }

        static void PruneStale(GameConfig c, GameState state, LabState lab)
        {
            int stale = c.Chips.ListingStaleWeeks;
            if (stale < 1) return;
            int tick = state.Tick;
            for (int i = lab.ChipListings.Count - 1; i >= 0; i--)
            {
                ChipListing listing = lab.ChipListings[i];
                if (listing.ListedSinceTick < 0) continue;
                if (tick - listing.ListedSinceTick < stale) continue;
                if (listing.SoldThisMonth > 0) continue;
                lab.ChipListings.RemoveAt(i);
                Events.Add(state, EventKind.ChipListingEnded, lab.Id, listing.DesignId, stale, "stale");
            }
        }

        static void EnforceCap(GameConfig c, GameState state, LabState lab)
        {
            int cap = c.Chips.ListingMaxActive;
            if (cap < 1) return;
            while (lab.ChipListings.Count > cap)
            {
                int drop = PickDropIndex(lab);
                if (drop < 0) break;
                ChipListing victim = lab.ChipListings[drop];
                lab.ChipListings.RemoveAt(drop);
                Events.Add(state, EventKind.ChipListingEnded, lab.Id, victim.DesignId, cap, "over-cap");
            }
        }

        static int PickDropIndex(LabState lab)
        {
            int pick = -1;
            int oldest = int.MaxValue;
            for (int i = 0; i < lab.ChipListings.Count; i++)
            {
                ChipListing listing = lab.ChipListings[i];
                if (listing.Spotlight) continue;
                int since = listing.ListedSinceTick < 0 ? int.MaxValue - 1 : listing.ListedSinceTick;
                if (since < oldest) { oldest = since; pick = i; }
            }
            if (pick >= 0) return pick;
            for (int i = 0; i < lab.ChipListings.Count; i++)
            {
                ChipListing listing = lab.ChipListings[i];
                int since = listing.ListedSinceTick < 0 ? int.MaxValue - 1 : listing.ListedSinceTick;
                if (since < oldest) { oldest = since; pick = i; }
            }
            return pick;
        }
    }
}