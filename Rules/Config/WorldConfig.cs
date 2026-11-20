using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The map the labs share: blocs (countries) with a government each, data-centre sites in them, private power
    /// deals, the rules for building halls, heatwaves, how much a government notices and what it may forbid.
    /// Everything here is data, so a new site or a new law is a new entry and not new code.
    /// </summary>
    public sealed class WorldConfig : ISync
    {
        public List<BlocConfig> Blocs;
        public List<SiteConfig> Sites;
        public List<PowerDealConfig> PowerDeals;
        public HallConfig Halls;
        public ColoConfig Colo;
        public HeatwaveConfig Heatwave;
        public AttentionConfig Attention;
        public List<ExportRuleConfig> ExportRules;

        public void Sync(Syncer s)
        {
            s.ObjList("blocs", ref Blocs);
            s.ObjList("sites", ref Sites);
            s.ObjList("powerDeals", ref PowerDeals);
            s.Obj("halls", ref Halls);
            s.Obj("colo", ref Colo);
            s.Obj("heatwave", ref Heatwave);
            s.Obj("attention", ref Attention);
            s.ObjList("exportRules", ref ExportRules);
        }

        public BlocConfig Bloc(string id)
        {
            for (int i = 0; i < Blocs.Count; i++) if (Blocs[i].Id == id) return Blocs[i];
            return null;
        }

        public SiteConfig Site(string id)
        {
            for (int i = 0; i < Sites.Count; i++) if (Sites[i].Id == id) return Sites[i];
            return null;
        }

        public PowerDealConfig PowerDeal(string id)
        {
            for (int i = 0; i < PowerDeals.Count; i++) if (PowerDeals[i].Id == id) return PowerDeals[i];
            return null;
        }
    }

    /// <summary>A country or union. Real names, fictional agencies; no real leaders appear anywhere.</summary>
    public sealed class BlocConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        /// <summary>The fictional agency that watches the labs there.</summary>
        public string Agency;
        /// <summary>How hard the agency leans on a lab with a big footprint (1000 = the baseline).</summary>
        public int StrictnessPermille;
        public string Flavour;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("agency", ref Agency);
            s.Int("strictnessPermille", ref StrictnessPermille);
            s.Str("flavour", ref Flavour);
        }
    }

    /// <summary>A place to put a data centre. Sites are shared: a few plots, a grid that can only give so much power.</summary>
    public sealed class SiteConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public string BlocId;
        public int OpensYear;
        /// <summary>How many labs can own land here.</summary>
        public int Plots;
        public long LandPrice;
        /// <summary>The most power the grid gives every lab here together, in kilowatts.</summary>
        public int GridKw;
        /// <summary>Dollars a week for one kilowatt from the grid.</summary>
        public int PowerPricePerKwWeek;
        /// <summary>What cooling adds to the bill (1000 = nothing, 1450 = hot country).</summary>
        public int CoolingPermille;
        /// <summary>The chance a summer month brings a heatwave is a third of this.</summary>
        public int SummerHeatwavePermille;
        /// <summary>The chance a new hall's town vote goes against the lab.</summary>
        public int TownVotePermille;
        /// <summary>The share of serving compute lost to distance from customers. Training does not care.</summary>
        public int LatencyPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blocId", ref BlocId);
            s.Int("opensYear", ref OpensYear);
            s.Int("plots", ref Plots);
            s.Long("landPrice", ref LandPrice);
            s.Int("gridKw", ref GridKw);
            s.Int("powerPricePerKwWeek", ref PowerPricePerKwWeek);
            s.Int("coolingPermille", ref CoolingPermille);
            s.Int("summerHeatwavePermille", ref SummerHeatwavePermille);
            s.Int("townVotePermille", ref TownVotePermille);
            s.Int("latencyPermille", ref LatencyPermille);
        }
    }

    /// <summary>Private power for one holding, on top of its share of the grid.</summary>
    public sealed class PowerDealConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int OpensYear;
        public int AddsKw;
        public long Cost;
        public int Weeks;
        /// <summary>Dollars a week for one kilowatt of this power. Zero means "the site's grid price".</summary>
        public int PricePerKwWeek;
        /// <summary>Multiplies the attention that holding's hardware draws (1000 = no change).</summary>
        public int AttentionPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("opensYear", ref OpensYear);
            s.Int("addsKw", ref AddsKw);
            s.Long("cost", ref Cost);
            s.Int("weeks", ref Weeks);
            s.Int("pricePerKwWeek", ref PricePerKwWeek);
            s.Int("attentionPermille", ref AttentionPermille);
        }
    }

    /// <summary>What a hall costs and how long it takes to build.</summary>
    public sealed class HallConfig : ISync
    {
        public long CostPerKw;
        public long UpkeepPerKwWeek;
        public int BuildWeeksBase;
        /// <summary>Every this many kilowatts add a week to the build.</summary>
        public int KwPerExtraWeek;
        public int MinKw;
        public int MinBuildWeeks;
        /// <summary>Each infrastructure engineer shaves this much off the build time.</summary>
        public int InfraSpeedPermillePerStaff;
        public int InfraSpeedCapPermille;
        public int TownVoteDelayWeeks;
        /// <summary>A bloc's attention adds this fraction to the chance of a "no" (the attention divided by this).</summary>
        public int TownVoteAttentionDivisor;
        public int TownVoteCapPermille;
        /// <summary>Selling land returns this share of its price. Halls are not refunded.</summary>
        public int LandRefundPermille;

        public void Sync(Syncer s)
        {
            s.Long("costPerKw", ref CostPerKw);
            s.Long("upkeepPerKwWeek", ref UpkeepPerKwWeek);
            s.Int("buildWeeksBase", ref BuildWeeksBase);
            s.Int("kwPerExtraWeek", ref KwPerExtraWeek);
            s.Int("minKw", ref MinKw);
            s.Int("minBuildWeeks", ref MinBuildWeeks);
            s.Int("infraSpeedPermillePerStaff", ref InfraSpeedPermillePerStaff);
            s.Int("infraSpeedCapPermille", ref InfraSpeedCapPermille);
            s.Int("townVoteDelayWeeks", ref TownVoteDelayWeeks);
            s.Int("townVoteAttentionDivisor", ref TownVoteAttentionDivisor);
            s.Int("townVoteCapPermille", ref TownVoteCapPermille);
            s.Int("landRefundPermille", ref LandRefundPermille);
        }
    }

    /// <summary>Renting space in someone else's hall: nothing up front, a markup on every leased kilowatt.</summary>
    public sealed class ColoConfig : ISync
    {
        public int MarkupPerKwWeek;
        public int MaxKwPerLabPerSite;

        public void Sync(Syncer s)
        {
            s.Int("markupPerKwWeek", ref MarkupPerKwWeek);
            s.Int("maxKwPerLabPerSite", ref MaxKwPerLabPerSite);
        }
    }

    public sealed class HeatwaveConfig : ISync
    {
        public int Weeks;
        public int ThrottlePermille;
        /// <summary>The summer months a heatwave can start in (1 to monthsPerYear, both included).</summary>
        public int FromMonth;
        public int ToMonth;
        /// <summary>A site's yearly heatwave chance is divided by this for each summer month.</summary>
        public int RollDivisor;
        /// <summary>Cooling tech at or above this halves the throttle.</summary>
        public int HalvedByCoolingPermille;

        public void Sync(Syncer s)
        {
            s.Int("weeks", ref Weeks);
            s.Int("throttlePermille", ref ThrottlePermille);
            s.Int("fromMonth", ref FromMonth);
            s.Int("toMonth", ref ToMonth);
            s.Int("rollDivisor", ref RollDivisor);
            s.Int("halvedByCoolingPermille", ref HalvedByCoolingPermille);
        }
    }

    /// <summary>How much a government notices a lab's footprint, and what it does about it.</summary>
    public sealed class AttentionConfig : ISync
    {
        public int DecayPermillePerMonth;
        /// <summary>The footprint, in kilowatts, that a government of the year starts to take notice of. Sorted by year.</summary>
        public List<NoticeKwConfig> NoticeKwByYear;
        /// <summary>Attention a lab's incident this month adds in its home bloc.</summary>
        public int IncidentGain;
        /// <summary>Revenue share above this (ppm) draws attention in the home bloc.</summary>
        public int DominanceFromPpm;
        public int DominanceDivisor;
        public int PolicySavingPermillePerStaff;
        public int PolicySavingCapPermille;
        public int LetterAt;
        public int HearingAt;
        public int AuditAt;
        public int FreezeAt;
        public int FreezeExitBelow;
        /// <summary>A level drops one step when attention falls this far under the level's entry point.</summary>
        public int DropMargin;
        public int HearingReputation;
        public int HearingReputationPrepared;
        /// <summary>Policy staff it takes to face a hearing prepared.</summary>
        public int PreparedPolicyStaff;
        public int AuditFineRevenueWeeks;

        public void Sync(Syncer s)
        {
            s.Int("decayPermillePerMonth", ref DecayPermillePerMonth);
            s.ObjList("noticeKwByYear", ref NoticeKwByYear);
            s.Int("incidentGain", ref IncidentGain);
            s.Int("dominanceFromPpm", ref DominanceFromPpm);
            s.Int("dominanceDivisor", ref DominanceDivisor);
            s.Int("policySavingPermillePerStaff", ref PolicySavingPermillePerStaff);
            s.Int("policySavingCapPermille", ref PolicySavingCapPermille);
            s.Int("letterAt", ref LetterAt);
            s.Int("hearingAt", ref HearingAt);
            s.Int("auditAt", ref AuditAt);
            s.Int("freezeAt", ref FreezeAt);
            s.Int("freezeExitBelow", ref FreezeExitBelow);
            s.Int("dropMargin", ref DropMargin);
            s.Int("hearingReputation", ref HearingReputation);
            s.Int("hearingReputationPrepared", ref HearingReputationPrepared);
            s.Int("preparedPolicyStaff", ref PreparedPolicyStaff);
            s.Int("auditFineRevenueWeeks", ref AuditFineRevenueWeeks);
        }

        /// <summary>The attention at which a level (1 to 4) is entered.</summary>
        public int EntryPoint(int level)
        {
            switch (level)
            {
                case 1: return LetterAt;
                case 2: return HearingAt;
                case 3: return AuditAt;
                default: return FreezeAt;
            }
        }
    }

    public sealed class NoticeKwConfig : ISync
    {
        public int Year;
        public long Kw;

        public void Sync(Syncer s)
        {
            s.Int("year", ref Year);
            s.Long("kw", ref Kw);
        }
    }

    /// <summary>
    /// A law about hardware. From its year, a lab whose home bloc is in <see cref="HomeBlocs"/> cannot buy, order or move
    /// hardware above <see cref="MaxTier"/> into a holding in <see cref="DestBlocs"/>. What is already there stays.
    /// </summary>
    public sealed class ExportRuleConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int FromYear;
        public List<string> HomeBlocs;
        public List<string> DestBlocs;
        public int MaxTier;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("fromYear", ref FromYear);
            s.StrList("homeBlocs", ref HomeBlocs);
            s.StrList("destBlocs", ref DestBlocs);
            s.Int("maxTier", ref MaxTier);
        }
    }
}
