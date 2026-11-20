using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    // Every tunable number of the game lives in these classes and is loaded from Rules/Data/default-config.json.
    // Units: money in whole dollars, rates in permille (1000 = 100%), time in weeks (one tick is one week).

    public enum RoleFunction { Research, Engineering, Safety, Sales, Infrastructure, Policy }

    public enum RoomPurpose { Research, Engineering, Safety, Sales, Compute, Infrastructure, Policy }

    public enum TechEffectKind
    {
        /// <summary>Raises the capability a training run aims for, in permille.</summary>
        CapabilityPermille,
        /// <summary>Raises how much training work a unit of compute does, in permille.</summary>
        TrainingSpeedPermille,
        /// <summary>Raises how much alignment work a unit of compute does, in permille.</summary>
        AlignmentPermille,
        /// <summary>Makes a model line available to train. <see cref="TechEffectConfig.Id"/> names the line.</summary>
        UnlockLine,
        /// <summary>Takes this much off the part of a power bill that cooling adds, and halves heatwave throttling at 300 or more.</summary>
        CoolingPermille
    }

    public sealed class GameConfig : ISync
    {
        public string Version;
        public int EventLogMax;
        public CalendarConfig Calendar;
        public List<EraConfig> Eras;
        public PlayerStartConfig PlayerStart;
        public List<LotConfig> Lots;
        public List<RoleConfig> Roles;
        public List<RoomTypeConfig> RoomTypes;
        public ComputeConfig Compute;
        public WorldConfig World;
        public ResearchConfig Research;
        public TrainingConfig Training;
        public SafetyConfig Safety;
        public MarketConfig Market;
        public FinanceConfig Finance;
        public GauntletConfig Gauntlet;
        public List<RivalConfig> Rivals;
        /// <summary>A sensible standing plan for the player's lab: the autopilot the balance bots and the console tool use.</summary>
        public PlanConfig Autopilot;
        /// <summary>Dated headlines of the wider world, some of which move a sector's market. Sorted by date.</summary>
        public List<WorldEventConfig> WorldEvents;

        public void Sync(Syncer s)
        {
            s.Str("version", ref Version);
            s.Int("eventLogMax", ref EventLogMax);
            s.Obj("calendar", ref Calendar);
            s.ObjList("eras", ref Eras);
            s.Obj("playerStart", ref PlayerStart);
            s.ObjList("lots", ref Lots);
            s.ObjList("roles", ref Roles);
            s.ObjList("roomTypes", ref RoomTypes);
            s.Obj("compute", ref Compute);
            s.Obj("world", ref World);
            s.Obj("research", ref Research);
            s.Obj("training", ref Training);
            s.Obj("safety", ref Safety);
            s.Obj("market", ref Market);
            s.Obj("finance", ref Finance);
            s.Obj("gauntlet", ref Gauntlet);
            s.ObjList("rivals", ref Rivals);
            s.Obj("autopilot", ref Autopilot);
            s.ObjList("worldEvents", ref WorldEvents);
        }

        /// <summary>Parses and validates a config. Throws <see cref="ConfigException"/> naming every problem found.</summary>
        public static GameConfig FromJson(string json)
        {
            GameConfig config;
            try { config = Syncer.Read<GameConfig>(Json.Parse(json), "config"); }
            catch (JsonException e) { throw new ConfigException(e.Message); }
            catch (SyncException e) { throw new ConfigException(e.Message); }
            ConfigValidator.Validate(config);
            return config;
        }

        public string ToJson() { return Json.Write(Syncer.Write(this), true); }

        public EraConfig Era(string id) { return Find(Eras, id); }
        public LotConfig Lot(string id) { return Find(Lots, id); }
        public RoleConfig Role(string id) { return Find(Roles, id); }
        public RoomTypeConfig RoomType(string id) { return Find(RoomTypes, id); }
        public TechConfig Tech(string id) { return Find(Research.Techs, id); }
        public ModelLineConfig Line(string id) { return Find(Training.Lines, id); }
        public SectorConfig Sector(string id) { return Find(Market.Sectors, id); }
        public RivalConfig Rival(string id) { return Find(Rivals, id); }
        public WorldEventConfig WorldEvent(string id) { return Find(WorldEvents, id); }

        static T Find<T>(List<T> list, string id) where T : class, IHasId
        {
            for (int i = 0; i < list.Count; i++) if (list[i].Id == id) return list[i];
            return null;
        }
    }

    public interface IHasId
    {
        string Id { get; }
    }

    public sealed class CalendarConfig : ISync
    {
        public int StartYear;
        public int WeeksPerMonth;
        public int MonthsPerYear;

        public int WeeksPerYear { get { return WeeksPerMonth * MonthsPerYear; } }

        public void Sync(Syncer s)
        {
            s.Int("startYear", ref StartYear);
            s.Int("weeksPerMonth", ref WeeksPerMonth);
            s.Int("monthsPerYear", ref MonthsPerYear);
        }
    }

    /// <summary>
    /// One era of the game: when it starts and what it hands the player. Eras are data, so a new era
    /// is a new entry here plus techs, rooms and sectors that name it. Eras are listed oldest first.
    /// </summary>
    public sealed class EraConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        /// <summary>The calendar year the era begins. It lasts until the next era's year.</summary>
        public int StartYear;
        /// <summary>True for eras that ship as expansions in the design. Their content is in the data all the same.</summary>
        public bool Expansion;
        /// <summary>True when the start-era picker offers this era as a way to begin a new game.</summary>
        public bool Startable;
        /// <summary>The era's running joke, for the era banner.</summary>
        public string Tagline;
        /// <summary>What a model can do in this era, in a sentence.</summary>
        public string ModelsCanDo;
        /// <summary>What opens up, as short bullets for the era banner. Descriptive: the techs, rooms and sectors that name the era are what actually open.</summary>
        public List<string> Unlocks;
        /// <summary>The cost of a training run, in permille of the model line's listed cost, for runs started in this era.</summary>
        public int TrainingCostPermille;
        /// <summary>How much research a researcher does, in permille of the listed points per researcher: better tools, more compute for experiments, later models helping.</summary>
        public int ResearchPermille;
        /// <summary>For a game that starts in this era: the share of the configured starting cash every lab gets.</summary>
        public int StartCashPermille;
        /// <summary>For a game that starts in this era: the share of the configured grant and backer money every lab gets.</summary>
        public int StartGrantPermille;
        /// <summary>For a game that starts in this era: the lot every lab starts on.</summary>
        public string StartLotId;
        /// <summary>For a game that starts in this era: techs every lab already has. Must be closed under prerequisites.</summary>
        public List<string> StartTechs;
        /// <summary>Incident headlines for this era; <c>{model}</c> is the model's name.</summary>
        public List<string> IncidentHeadlines;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("startYear", ref StartYear);
            s.Bool("expansion", ref Expansion);
            s.Bool("startable", ref Startable);
            s.Str("tagline", ref Tagline);
            s.Str("modelsCanDo", ref ModelsCanDo);
            s.StrList("unlocks", ref Unlocks);
            s.Int("trainingCostPermille", ref TrainingCostPermille);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("startCashPermille", ref StartCashPermille);
            s.Int("startGrantPermille", ref StartGrantPermille);
            s.Str("startLotId", ref StartLotId);
            s.StrList("startTechs", ref StartTechs);
            s.StrList("incidentHeadlines", ref IncidentHeadlines);
        }
    }

    /// <summary>
    /// A dated headline from the wider world, shown once when its month begins. It may also move one
    /// sector's market (a bubble bursting, a ban lifting); with no sector it is only a headline.
    /// </summary>
    public sealed class WorldEventConfig : ISync, IHasId
    {
        public string IdValue;
        public int Year;
        /// <summary>1 to monthsPerYear. The headline lands in the first week of the month.</summary>
        public int Month;
        public string Headline;
        /// <summary>The sector whose pool is scaled, or empty for a headline with no effect.</summary>
        public string SectorId;
        /// <summary>The sector's pool is multiplied by this, in permille (700 = a 30% crash). 1000 when there is no sector.</summary>
        public int PoolPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("year", ref Year);
            s.Int("month", ref Month);
            s.Str("headline", ref Headline);
            s.Str("sectorId", ref SectorId);
            s.Int("poolPermille", ref PoolPermille);
        }
    }

    public sealed class PlayerStartConfig : ISync
    {
        public string LabName;
        public string Leader;
        public string FamilyName;
        public long Cash;
        public string LotId;
        public int Reputation;
        public int TrainingSharePermille;
        /// <summary>A no-strings research grant paid every week for the first <see cref="GrantWeeks"/> weeks.</summary>
        public long GrantPerWeek;
        public int GrantWeeks;
        /// <summary>The country the lab is founded in. Export rules and government attention start from here.</summary>
        public string HomeBlocId;

        public void Sync(Syncer s)
        {
            s.Str("labName", ref LabName);
            s.Str("leader", ref Leader);
            s.Str("familyName", ref FamilyName);
            s.Long("cash", ref Cash);
            s.Str("lotId", ref LotId);
            s.Int("reputation", ref Reputation);
            s.Int("trainingSharePermille", ref TrainingSharePermille);
            s.Long("grantPerWeek", ref GrantPerWeek);
            s.Int("grantWeeks", ref GrantWeeks);
            s.Str("homeBlocId", ref HomeBlocId);
        }
    }

    /// <summary>A plot of land the HQ stands on. Lots are listed small to large; a lab moves up by buying the next one.</summary>
    public sealed class LotConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int Width;
        public int Height;
        public long Price;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
            s.Long("price", ref Price);
        }
    }

    public sealed class RoleConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public RoleFunction Function;
        public long HireCost;
        public long WeeklySalary;
        public long Severance;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Enum("function", ref Function);
            s.Long("hireCost", ref HireCost);
            s.Long("weeklySalary", ref WeeklySalary);
            s.Long("severance", ref Severance);
        }
    }

    public sealed class RoomTypeConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        /// <summary>The era whose start opens this room type for building.</summary>
        public string Era;
        public RoomPurpose Purpose;
        /// <summary>The role this room seats, or empty for a room that seats nobody (a server closet).</summary>
        public string SeatsRole;
        public int TilesPerSeat;
        public int GpuSlotsPerTile;
        public int MinWidth;
        public int MinHeight;
        public long BuildCostPerTile;
        public long UpkeepPerTileWeek;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("era", ref Era);
            s.Enum("purpose", ref Purpose);
            s.Str("seatsRole", ref SeatsRole);
            s.Int("tilesPerSeat", ref TilesPerSeat);
            s.Int("gpuSlotsPerTile", ref GpuSlotsPerTile);
            s.Int("minWidth", ref MinWidth);
            s.Int("minHeight", ref MinHeight);
            s.Long("buildCostPerTile", ref BuildCostPerTile);
            s.Long("upkeepPerTileWeek", ref UpkeepPerTileWeek);
        }
    }

    /// <summary>
    /// What a lab can do about GPUs: the vendor (Envydia), its generations from 1995 on, and how fast an old
    /// generation loses its value. Renting and buying always get the newest generation open in the current year.
    /// </summary>
    public sealed class ComputeConfig : ISync
    {
        public string VendorName;
        public string VendorBoss;
        /// <summary>A sold GPU of the current generation returns this share of its price.</summary>
        public int ResalePermille;
        /// <summary>Resale keeps this share for every generation the GPU is behind the newest (650 = about 35% lost a generation).</summary>
        public int GenerationDecayPermille;
        /// <summary>What it costs to move one unit of hardware between locations.</summary>
        public long MoveCostPerUnit;
        /// <summary>The vendor's line, oldest first.</summary>
        public List<GpuGenerationConfig> GpuGenerations;

        public void Sync(Syncer s)
        {
            s.Str("vendorName", ref VendorName);
            s.Str("vendorBoss", ref VendorBoss);
            s.Int("resalePermille", ref ResalePermille);
            s.Int("generationDecayPermille", ref GenerationDecayPermille);
            s.Long("moveCostPerUnit", ref MoveCostPerUnit);
            s.ObjList("gpuGenerations", ref GpuGenerations);
        }

        public GpuGenerationConfig Generation(string id)
        {
            for (int i = 0; i < GpuGenerations.Count; i++) if (GpuGenerations[i].Id == id) return GpuGenerations[i];
            return null;
        }

        /// <summary>The newest generation open in a year: the one every purchase and every rental gets.</summary>
        public GpuGenerationConfig CurrentAtYear(int year)
        {
            GpuGenerationConfig best = GpuGenerations[0];
            for (int i = 0; i < GpuGenerations.Count; i++) if (GpuGenerations[i].OpensYear <= year) best = GpuGenerations[i];
            return best;
        }
    }

    /// <summary>One generation of the vendor's GPU. Parody names, no real product names.</summary>
    public sealed class GpuGenerationConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int OpensYear;
        /// <summary>0 for the first generation, up by one each generation. Resale decay and export rules count in tiers.</summary>
        public int Tier;
        public int ComputePerGpuWeek;
        public long RentPerGpuWeek;
        /// <summary>The weekly upkeep of a GPU racked at the HQ. A GPU at a site pays for power instead.</summary>
        public long UpkeepPerGpuWeek;
        public long BuyPrice;
        /// <summary>Power a GPU draws, in whole kilowatts, once it sits in a data-centre hall.</summary>
        public int KwPerGpu;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("opensYear", ref OpensYear);
            s.Int("tier", ref Tier);
            s.Int("computePerGpuWeek", ref ComputePerGpuWeek);
            s.Long("rentPerGpuWeek", ref RentPerGpuWeek);
            s.Long("upkeepPerGpuWeek", ref UpkeepPerGpuWeek);
            s.Long("buyPrice", ref BuyPrice);
            s.Int("kwPerGpu", ref KwPerGpu);
        }
    }

    public sealed class ResearchConfig : ISync
    {
        public int PointsPerResearcherWeek;
        public List<TechConfig> Techs;

        public void Sync(Syncer s)
        {
            s.Int("pointsPerResearcherWeek", ref PointsPerResearcherWeek);
            s.ObjList("techs", ref Techs);
        }
    }

    public sealed class TechConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        /// <summary>The era whose start opens this tech for research.</summary>
        public string Era;
        /// <summary>One funny line for the research screen.</summary>
        public string Blurb;
        /// <summary>Research points it costs.</summary>
        public long Cost;
        public List<string> Requires;
        public List<TechEffectConfig> Effects;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("era", ref Era);
            s.Str("blurb", ref Blurb);
            s.Long("cost", ref Cost);
            s.StrList("requires", ref Requires);
            s.ObjList("effects", ref Effects);
        }
    }

    public sealed class TechEffectConfig : ISync
    {
        public TechEffectKind Kind;
        public int Amount;
        /// <summary>For <see cref="TechEffectKind.UnlockLine"/>: the model line it unlocks. Empty otherwise.</summary>
        public string Id;

        public void Sync(Syncer s)
        {
            s.Enum("kind", ref Kind);
            s.Int("amount", ref Amount);
            s.Str("id", ref Id);
        }
    }

    public sealed class TrainingConfig : ISync
    {
        public int MaxConcurrentRuns;
        /// <summary>The share of the previous version's capability a new version of the same line starts from.</summary>
        public int CarryoverPermille;
        /// <summary>A finished run lands within plus or minus this much of what it aimed for.</summary>
        public int LuckPermille;
        public int EngineerBonusPermille;
        public int EngineerBonusCapPermille;
        public List<ModelLineConfig> Lines;

        public void Sync(Syncer s)
        {
            s.Int("maxConcurrentRuns", ref MaxConcurrentRuns);
            s.Int("carryoverPermille", ref CarryoverPermille);
            s.Int("luckPermille", ref LuckPermille);
            s.Int("engineerBonusPermille", ref EngineerBonusPermille);
            s.Int("engineerBonusCapPermille", ref EngineerBonusCapPermille);
            s.ObjList("lines", ref Lines);
        }
    }

    /// <summary>A model line of the family: Mini, Mid or Flagship. Each version of a line is one training run.</summary>
    public sealed class ModelLineConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public long ComputeRequired;
        public int BaseCapability;
        public long PrepCost;
        /// <summary>How much serving compute a dollar of revenue needs, against the sector's own rate (1000 = the same).</summary>
        public int ServingCostPermille;
        /// <summary>True when the line needs an UnlockLine tech before it can be trained.</summary>
        public bool Locked;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Long("computeRequired", ref ComputeRequired);
            s.Int("baseCapability", ref BaseCapability);
            s.Long("prepCost", ref PrepCost);
            s.Int("servingCostPermille", ref ServingCostPermille);
            s.Bool("locked", ref Locked);
        }
    }

    public sealed class SafetyConfig : ISync
    {
        /// <summary>Alignment work a full safety meter needs, as a share of the run's capability compute.</summary>
        public int FullAlignmentComputePermille;
        public int SafetyStaffBonusPermille;
        public int SafetyStaffBonusCapPermille;
        /// <summary>The weekly chance of an incident for a released model with an empty safety meter.</summary>
        public int IncidentChancePermille;
        public int IncidentReputationLoss;
        /// <summary>An incident's fine is this many weeks of the model's revenue, and never less than the minimum.</summary>
        public int IncidentFineRevenueWeeks;
        public long IncidentFineMin;
        /// <summary>Weeks a model stays in the Glitching state after an incident.</summary>
        public int GlitchWeeks;
        public int GlitchQualityPermille;
        public int ReputationBaseline;
        public int ReputationRecoveryPerWeek;

        public void Sync(Syncer s)
        {
            s.Int("fullAlignmentComputePermille", ref FullAlignmentComputePermille);
            s.Int("safetyStaffBonusPermille", ref SafetyStaffBonusPermille);
            s.Int("safetyStaffBonusCapPermille", ref SafetyStaffBonusCapPermille);
            s.Int("incidentChancePermille", ref IncidentChancePermille);
            s.Int("incidentReputationLoss", ref IncidentReputationLoss);
            s.Int("incidentFineRevenueWeeks", ref IncidentFineRevenueWeeks);
            s.Long("incidentFineMin", ref IncidentFineMin);
            s.Int("glitchWeeks", ref GlitchWeeks);
            s.Int("glitchQualityPermille", ref GlitchQualityPermille);
            s.Int("reputationBaseline", ref ReputationBaseline);
            s.Int("reputationRecoveryPerWeek", ref ReputationRecoveryPerWeek);
        }
    }

    public sealed class MarketConfig : ISync
    {
        /// <summary>How fast customers move toward the lab they would pick today, per week.</summary>
        public int SwitchRatePermille;
        public int SalesBonusPermille;
        public int SalesBonusCapPermille;
        public List<SectorConfig> Sectors;

        public void Sync(Syncer s)
        {
            s.Int("switchRatePermille", ref SwitchRatePermille);
            s.Int("salesBonusPermille", ref SalesBonusPermille);
            s.Int("salesBonusCapPermille", ref SalesBonusCapPermille);
            s.ObjList("sectors", ref Sectors);
        }
    }

    public sealed class SectorConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        /// <summary>The era the sector opens in. <see cref="OpensYear"/> falls inside it.</summary>
        public string Era;
        public string Buyers;
        public int OpensYear;
        /// <summary>What the whole sector would pay per week in the year it opens, once AI is good enough for all of it. It grows every year after that.</summary>
        public long PoolPerWeek;
        public int GrowthPermillePerYear;
        /// <summary>Each year after it opens, the growth rate is this share of the year before's (900 = it slows by a tenth a year), so a sector settles instead of compounding forever.</summary>
        public int GrowthDecayPermille;
        /// <summary>The lowest capability a model needs to be sold here.</summary>
        public int MinCapability;
        /// <summary>The capability at which every customer of the sector has a use for AI.</summary>
        public int SaturationCapability;
        /// <summary>How much of a model's quality here is raw capability; the rest is its safety meter.</summary>
        public int CapabilityWeightPermille;
        public int SafetyWeightPermille;
        public long RevenuePerComputeUnit;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("era", ref Era);
            s.Str("buyers", ref Buyers);
            s.Int("opensYear", ref OpensYear);
            s.Long("poolPerWeek", ref PoolPerWeek);
            s.Int("growthPermillePerYear", ref GrowthPermillePerYear);
            s.Int("growthDecayPermille", ref GrowthDecayPermille);
            s.Int("minCapability", ref MinCapability);
            s.Int("saturationCapability", ref SaturationCapability);
            s.Int("capabilityWeightPermille", ref CapabilityWeightPermille);
            s.Int("safetyWeightPermille", ref SafetyWeightPermille);
            s.Long("revenuePerComputeUnit", ref RevenuePerComputeUnit);
        }
    }

    public sealed class FinanceConfig : ISync
    {
        /// <summary>A lab whose cash stays below zero this many weeks in a row goes bankrupt.</summary>
        public int BankruptcyGraceWeeks;
        public int HistoryMonths;

        public void Sync(Syncer s)
        {
            s.Int("bankruptcyGraceWeeks", ref BankruptcyGraceWeeks);
            s.Int("historyMonths", ref HistoryMonths);
        }
    }

    /// <summary>The yearly model tournament: one bracket per sector plus an overall crown.</summary>
    public sealed class GauntletConfig : ISync
    {
        public string Name;
        /// <summary>Stage nerves: an entry scores within plus or minus this much of its true quality.</summary>
        public int JitterPermille;
        public int CrownReputation;
        public int SafestReputation;
        /// <summary>The pull a bracket winner has on that sector's customers for the next year.</summary>
        public int BracketWinnerAppealPermille;
        /// <summary>How many years of results are kept.</summary>
        public int HistoryYears;

        public void Sync(Syncer s)
        {
            s.Str("name", ref Name);
            s.Int("jitterPermille", ref JitterPermille);
            s.Int("crownReputation", ref CrownReputation);
            s.Int("safestReputation", ref SafestReputation);
            s.Int("bracketWinnerAppealPermille", ref BracketWinnerAppealPermille);
            s.Int("historyYears", ref HistoryYears);
        }
    }

    public sealed class RivalConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public string Leader;
        public string FamilyName;
        public string Archetype;
        public long StartCash;
        public string LotId;
        public int Reputation;
        /// <summary>Money from backers every week, for as long as the lab lives.</summary>
        public long BackerPerWeek;
        public string HomeBlocId;
        public PlanConfig Plan;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("leader", ref Leader);
            s.Str("familyName", ref FamilyName);
            s.Str("archetype", ref Archetype);
            s.Long("startCash", ref StartCash);
            s.Str("lotId", ref LotId);
            s.Int("reputation", ref Reputation);
            s.Long("backerPerWeek", ref BackerPerWeek);
            s.Str("homeBlocId", ref HomeBlocId);
            s.Obj("plan", ref Plan);
        }
    }

    /// <summary>
    /// A simple standing plan a lab follows by itself: who to hire, how much compute to hold, how careful to be
    /// and where to sell. Rivals run on one; a bot can drive the player's lab with one too.
    /// </summary>
    public sealed class PlanConfig : ISync
    {
        /// <summary>Weeks the lab waits before it does anything, plus up to the jitter (rolled once, when the game starts).</summary>
        public int StartDelayWeeks;
        public int StartJitterWeeks;
        public List<PlanStaffConfig> Staff;
        public int StaffGrowthPermillePerYear;
        public int Gpus;
        public int GpuGrowthPermillePerYear;
        /// <summary>Ambition: every million dollars in the bank raises the staff and GPU targets by this much.</summary>
        public int WealthGrowthPermillePerMillion;
        public bool OwnGpus;
        public int TrainingSharePermille;
        public int AlignmentSharePermille;
        /// <summary>The lab keeps this many weeks of running costs in the bank before it spends on growth.</summary>
        public int ReserveWeeks;
        /// <summary>The longest training run, in weeks at today's compute, the lab will start.</summary>
        public int MaxRunWeeks;
        /// <summary>A new run must promise at least this much more capability than the line's last model.</summary>
        public int MinGainPermille;
        public List<string> Sectors;
        /// <summary>How eagerly the lab builds data centres once its own hardware outgrows the HQ (0 never builds a site; 1000 sizes a hall to all of the growth).</summary>
        public int SiteAppetitePermille;
        /// <summary>The blocs the lab will build in.</summary>
        public List<string> SiteBlocs;

        public void Sync(Syncer s)
        {
            s.Int("startDelayWeeks", ref StartDelayWeeks);
            s.Int("startJitterWeeks", ref StartJitterWeeks);
            s.ObjList("staff", ref Staff);
            s.Int("staffGrowthPermillePerYear", ref StaffGrowthPermillePerYear);
            s.Int("gpus", ref Gpus);
            s.Int("gpuGrowthPermillePerYear", ref GpuGrowthPermillePerYear);
            s.Int("wealthGrowthPermillePerMillion", ref WealthGrowthPermillePerMillion);
            s.Bool("ownGpus", ref OwnGpus);
            s.Int("trainingSharePermille", ref TrainingSharePermille);
            s.Int("alignmentSharePermille", ref AlignmentSharePermille);
            s.Int("reserveWeeks", ref ReserveWeeks);
            s.Int("maxRunWeeks", ref MaxRunWeeks);
            s.Int("minGainPermille", ref MinGainPermille);
            s.StrList("sectors", ref Sectors);
            s.Int("siteAppetitePermille", ref SiteAppetitePermille);
            s.StrList("siteBlocs", ref SiteBlocs);
        }
    }

    public sealed class PlanStaffConfig : ISync
    {
        public string RoleId;
        public int Count;

        public void Sync(Syncer s)
        {
            s.Str("roleId", ref RoleId);
            s.Int("count", ref Count);
        }
    }
}
