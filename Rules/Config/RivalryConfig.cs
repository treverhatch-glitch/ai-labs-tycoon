using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// How the rival labs explain themselves and feel about the player: the decision log, the sentences it is read
    /// in, the feelings toward the player and the signature behaviours the plans share.
    /// </summary>
    public sealed class RivalryConfig : ISync
    {
        /// <summary>Entries kept per lab; the oldest are dropped.</summary>
        public int LogMax;
        /// <summary>Hires and GPU buys below this many units are too small to be worth a line.</summary>
        public int MinLoggedUnits;
        /// <summary>A change in rented GPUs is logged only when it moves the count by this share or more.</summary>
        public int RentLogPermille;
        /// <summary>What every other lab saves on a safety tech once a lab that publishes its safety research has it.</summary>
        public int PublishedDiscountPermille;
        /// <summary>How long a plan stops issuing commands after its board throws a fit.</summary>
        public int BoardDramaWeeks;
        /// <summary>A hall that opens within this many weeks of its start is a headline.</summary>
        public int FastBuildWeeks;
        /// <summary>When a model is held back by the release gate, the next run's alignment share goes up by this much.</summary>
        public int GateBoostPermille;
        /// <summary>A plan replaces hardware that is this many generations behind the newest.</summary>
        public int ReplaceTierGap;
        /// <summary>A plan that loses money starts cutting once its cash covers fewer than this many weeks of running costs, before it is in debt.</summary>
        public int RetrenchRunwayWeeks;
        /// <summary>Running costs above this share of weekly revenue (1000 = break-even) count as losing money.</summary>
        public int RetrenchCostPermille;
        public FeelingsConfig Feelings;
        /// <summary>One sentence template per decision action, by the action's name.</summary>
        public List<TemplateConfig> ActionTemplates;
        /// <summary>One phrase template per reason code, by the code's name.</summary>
        public List<TemplateConfig> ReasonTemplates;

        public void Sync(Syncer s)
        {
            s.Int("logMax", ref LogMax);
            s.Int("minLoggedUnits", ref MinLoggedUnits);
            s.Int("rentLogPermille", ref RentLogPermille);
            s.Int("publishedDiscountPermille", ref PublishedDiscountPermille);
            s.Int("boardDramaWeeks", ref BoardDramaWeeks);
            s.Int("fastBuildWeeks", ref FastBuildWeeks);
            s.Int("gateBoostPermille", ref GateBoostPermille);
            s.Int("replaceTierGap", ref ReplaceTierGap);
            s.Int("retrenchRunwayWeeks", ref RetrenchRunwayWeeks);
            s.Int("retrenchCostPermille", ref RetrenchCostPermille);
            s.Obj("feelings", ref Feelings);
            s.ObjList("actionTemplates", ref ActionTemplates);
            s.ObjList("reasonTemplates", ref ReasonTemplates);
        }

        public string ActionTemplate(string action)
        {
            for (int i = 0; i < ActionTemplates.Count; i++) if (ActionTemplates[i].Id == action) return ActionTemplates[i].Text;
            return null;
        }

        public string ReasonTemplate(string code)
        {
            for (int i = 0; i < ReasonTemplates.Count; i++) if (ReasonTemplates[i].Id == code) return ReasonTemplates[i].Text;
            return null;
        }
    }

    /// <summary>A line of text with a key. Used for the sentences the decision logs are read in.</summary>
    public sealed class TemplateConfig : ISync, IHasId
    {
        public string IdValue;
        public string Text;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("text", ref Text);
        }
    }

    /// <summary>What makes a rival like or dislike the player, in points on a scale of -1000 to 1000.</summary>
    public sealed class FeelingsConfig : ISync
    {
        /// <summary>The player takes the Gauntlet crown: every rival, and the rival that came second.</summary>
        public int CrownPermille;
        public int SecondPlacePermille;
        /// <summary>The player wins a bracket the rival also entered.</summary>
        public int BracketPermille;
        /// <summary>Per order, when the player buys the rival's listed chips or the rival buys the player's.</summary>
        public int ChipOrderPermille;
        /// <summary>The player is traced in a shady operation against the rival, and (sabotage) against everybody.</summary>
        public int TracedShadyPermille;
        public int SabotageAllPermille;
        /// <summary>Every month a feeling moves this far back toward zero.</summary>
        public int DriftPerMonth;
        /// <summary>Below this a rival is in a feud with the player: it lobbies against them and will not buy their chips.</summary>
        public int FeudBelow;
        /// <summary>Above this a rival is friendly: it pays a little more for the player's chips.</summary>
        public int FriendlyAbove;
        /// <summary>Attention a feuding rival adds against the player in the player's home bloc each month.</summary>
        public int FeudAttentionPerMonth;
        public long LobbyDefenseCash = 9_000;
        public int LobbyDefenseWeeks = 10;
        /// <summary>Feud lobby bump is multiplied by (1000 − this) while <see cref="LabState.LobbyDefenseUntilTick"/> is active.</summary>
        public int LobbyDefenseCutPermille = 600;
        /// <summary>A rival buys listed chips up to this share of the vendor's price per compute, and this share when friendly.</summary>
        public int ListedBuyLimitPermille;
        public int FriendlyBuyLimitPermille;

        public void Sync(Syncer s)
        {
            s.Int("crownPermille", ref CrownPermille);
            s.Int("secondPlacePermille", ref SecondPlacePermille);
            s.Int("bracketPermille", ref BracketPermille);
            s.Int("chipOrderPermille", ref ChipOrderPermille);
            s.Int("tracedShadyPermille", ref TracedShadyPermille);
            s.Int("sabotageAllPermille", ref SabotageAllPermille);
            s.Int("driftPerMonth", ref DriftPerMonth);
            s.Int("feudBelow", ref FeudBelow);
            s.Int("friendlyAbove", ref FriendlyAbove);
            s.Int("feudAttentionPerMonth", ref FeudAttentionPerMonth);
            s.Long("lobbyDefenseCash", ref LobbyDefenseCash);
            s.Int("lobbyDefenseWeeks", ref LobbyDefenseWeeks);
            s.Int("lobbyDefenseCutPermille", ref LobbyDefenseCutPermille);
            s.Int("listedBuyLimitPermille", ref ListedBuyLimitPermille);
            s.Int("friendlyBuyLimitPermille", ref FriendlyBuyLimitPermille);
        }
    }

    /// <summary>A number that holds from a year on.</summary>
    public sealed class YearAmountConfig : ISync
    {
        public int Year;
        public int Amount;

        public void Sync(Syncer s)
        {
            s.Int("year", ref Year);
            s.Int("amount", ref Amount);
        }
    }

    /// <summary>
    /// The GPU vendor as an actor (decision D11: Envydia is a vendor, not a lab): how many units it makes, how it splits them
    /// between labs by standing, what it charges and how it reacts when a lab starts making its own chips.
    /// </summary>
    public sealed class VendorConfig : ISync
    {
        /// <summary>Units of the current generation the vendor can sell every month, from a year on. Sorted by year.</summary>
        public List<YearAmountConfig> SupplyByYear;
        /// <summary>Every lab's share of the month's supply is proportional to this plus its standing.</summary>
        public int AllocationBase;
        public int StandingStart;
        public int StandingMax;
        /// <summary>Standing a lab earns for every this many GPUs it buys.</summary>
        public int GpusPerStanding;
        /// <summary>Standing lost when a lab tapes out its first training chip, for each chip delivery and for listing chips for sale.</summary>
        public int TrainingTapeOutDrop;
        public int DeliveryDrop;
        public int ListingDrop;
        /// <summary>Every month standing moves this far back toward the start value.</summary>
        public int DriftPerMonth;
        /// <summary>Price mark-ups and discounts, in permille of the list price.</summary>
        public int LowStandingBelow;
        public int LowStandingMarkupPermille;
        public int OwnChipsMarkupPermille;
        public int HighStandingAbove;
        public int HighStandingDiscountPermille;
        /// <summary>The month of the yearly keynote, and how much of that month's supply is left after it.</summary>
        public int KeynoteMonth;
        public int KeynoteSupplyPermille;
        /// <summary>A chip design that became ready this many weeks before the keynote is upstaged for a while.</summary>
        public int AmbushLookbackWeeks;
        public int AmbushWeeks;
        /// <summary>A lab with this share of its general compute on its own chips makes the vendor court its rivals.</summary>
        public int CourtingSharePermille;
        public int CourtingStandingBoost;
        /// <summary>Home-bloc attention the dominant chip maker draws when Envydia courts rivals.</summary>
        public int CourtingMakerAttentionPermille;
        /// <summary>A lab's share of the supply moving by this much (as a share of itself) month to month gets a line in the vendor's log.</summary>
        public int ShareLogPermille;
        public int LogMax;
        /// <summary>Weekly cash grant per standing point when standing is above this (Q169).</summary>
        public int WeeklyGrantStandingAbove = 450;
        public long WeeklyGrantPerStanding = 3;
        /// <summary>Cash to lobby Envydia for standing (Q178).</summary>
        public long AdvocateCashCost = 20_000;
        public int AdvocateStandingGain = 40;
        public int AdvocateCooldownWeeks = 12;

        public void Sync(Syncer s)
        {
            s.ObjList("supplyByYear", ref SupplyByYear);
            s.Int("allocationBase", ref AllocationBase);
            s.Int("standingStart", ref StandingStart);
            s.Int("standingMax", ref StandingMax);
            s.Int("gpusPerStanding", ref GpusPerStanding);
            s.Int("trainingTapeOutDrop", ref TrainingTapeOutDrop);
            s.Int("deliveryDrop", ref DeliveryDrop);
            s.Int("listingDrop", ref ListingDrop);
            s.Int("driftPerMonth", ref DriftPerMonth);
            s.Int("lowStandingBelow", ref LowStandingBelow);
            s.Int("lowStandingMarkupPermille", ref LowStandingMarkupPermille);
            s.Int("ownChipsMarkupPermille", ref OwnChipsMarkupPermille);
            s.Int("highStandingAbove", ref HighStandingAbove);
            s.Int("highStandingDiscountPermille", ref HighStandingDiscountPermille);
            s.Int("keynoteMonth", ref KeynoteMonth);
            s.Int("keynoteSupplyPermille", ref KeynoteSupplyPermille);
            s.Int("ambushLookbackWeeks", ref AmbushLookbackWeeks);
            s.Int("ambushWeeks", ref AmbushWeeks);
            s.Int("courtingSharePermille", ref CourtingSharePermille);
            s.Int("courtingStandingBoost", ref CourtingStandingBoost);
            s.Int("courtingMakerAttentionPermille", ref CourtingMakerAttentionPermille);
            s.Int("shareLogPermille", ref ShareLogPermille);
            s.Int("logMax", ref LogMax);
            s.Int("weeklyGrantStandingAbove", ref WeeklyGrantStandingAbove);
            s.Long("weeklyGrantPerStanding", ref WeeklyGrantPerStanding);
            s.Long("advocateCashCost", ref AdvocateCashCost);
            s.Int("advocateStandingGain", ref AdvocateStandingGain);
            s.Int("advocateCooldownWeeks", ref AdvocateCooldownWeeks);
        }

        /// <summary>The monthly supply in a year: the last row that has begun.</summary>
        public int SupplyInYear(int year)
        {
            int supply = SupplyByYear[0].Amount;
            for (int i = 0; i < SupplyByYear.Count; i++) if (SupplyByYear[i].Year <= year) supply = SupplyByYear[i].Amount;
            return supply;
        }
    }
}
