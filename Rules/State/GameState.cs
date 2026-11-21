using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>
    /// The whole game in one plain object. Nothing derived is cached outside it, so saving this
    /// and loading it back gives exactly the same game.
    /// </summary>
    public sealed class GameState : ISync
    {
        public const int CurrentSchema = 16;

        public int Schema;
        public string ConfigVersion;
        public long Seed;
        /// <summary>Weeks played. Tick 0 is the first week of the start year.</summary>
        public int Tick;
        public int NextId;
        public RngSet Rng;
        /// <summary>Every lab in the world. The player's lab is always first.</summary>
        public List<LabState> Labs;
        public List<SectorState> Sectors;
        /// <summary>The data-centre sites, one per site of the config.</summary>
        public List<SiteState> Sites;
        public List<GauntletRecord> Gauntlets;
        /// <summary>The newest events, oldest first, capped at the config's eventLogMax.</summary>
        public List<GameEvent> Events;
        /// <summary>The GPU vendor's supply, its standing for every lab and its log.</summary>
        public VendorState Vendor;
        /// <summary>Rivals that have not joined yet, by config id. They arrive when their year does.</summary>
        public List<string> PendingRivals = new List<string>();
        /// <summary>The foundry's booked months, from the current month on.</summary>
        public List<FoundryMonth> Foundry = new List<FoundryMonth>();
        /// <summary>The lab the player controls, or -1 when spectating.</summary>
        public int PlayerLabId;
        /// <summary>Circular-financing bubble pressure shared by the whole economy (0–1000).</summary>
        public int BubblePermille;
        public List<EndingRecord> Endings = new List<EndingRecord>();

        public LabState Player
        {
            get
            {
                if (PlayerLabId < 0) return null;
                return Lab(PlayerLabId);
            }
        }

        public LabState Lab(int id)
        {
            for (int i = 0; i < Labs.Count; i++) if (Labs[i].Id == id) return Labs[i];
            return null;
        }

        public SectorState Sector(string id)
        {
            for (int i = 0; i < Sectors.Count; i++) if (Sectors[i].Id == id) return Sectors[i];
            return null;
        }

        public SiteState Site(string id)
        {
            for (int i = 0; i < Sites.Count; i++) if (Sites[i].SiteId == id) return Sites[i];
            return null;
        }

        public int TakeId() { return NextId++; }

        public void Sync(Syncer s)
        {
            s.Int("schema", ref Schema);
            s.Str("configVersion", ref ConfigVersion);
            s.Long("seed", ref Seed);
            s.Int("tick", ref Tick);
            s.Int("nextId", ref NextId);
            s.Obj("rng", ref Rng);
            s.ObjList("labs", ref Labs);
            s.ObjList("sectors", ref Sectors);
            s.ObjList("sites", ref Sites);
            s.ObjList("gauntlets", ref Gauntlets);
            s.ObjList("events", ref Events);
            s.Obj("vendor", ref Vendor);
            s.StrList("pendingRivals", ref PendingRivals);
            s.ObjList("foundry", ref Foundry);
            s.Int("playerLabId", ref PlayerLabId);
            s.Int("bubblePermille", ref BubblePermille);
            s.ObjList("endings", ref Endings);
        }
    }

    public enum LabController { Player, Plan }

    public sealed class LabState : ISync
    {
        public int Id;
        public string Name;
        public string Leader;
        public string FamilyName;
        public LabController Controller;
        /// <summary>The rival config this lab was made from, or empty for the player's lab.</summary>
        public string RivalId;
        /// <summary>The first tick a lab run by a plan acts. Rolled once when the game starts.</summary>
        public int PlanStartTick;

        public long Cash;
        public int Reputation;
        public bool Bankrupt;
        public int WeeksInDebt;
        public long GrantPerWeek;
        /// <summary>The first tick the grant is no longer paid, or -1 when it never ends.</summary>
        public int GrantEndsTick;

        /// <summary>The country the lab is based in. Export rules and the home government's attention start here.</summary>
        public string HomeBlocId;
        public string LotId;
        public List<RoomState> Rooms = new List<RoomState>();
        public List<StaffMember> Staff = new List<StaffMember>();

        public int RentedGpus;
        public List<ReservedContract> ReservedContracts = new List<ReservedContract>();
        /// <summary>The hardware the lab owns, by place and generation. Rented GPUs are a count: they live in the cloud.</summary>
        public List<HardwareBatch> Fleet = new List<HardwareBatch>();
        /// <summary>The lab's data-centre sites: owned land with a hall, or leased colo space.</summary>
        public List<Holding> Holdings = new List<Holding>();
        /// <summary>One entry per bloc of the config: what each government makes of this lab.</summary>
        public List<BlocStanding> Blocs = new List<BlocStanding>();
        /// <summary>Safety incidents this month, which the home government counts when the month closes.</summary>
        public int IncidentsThisMonth;
        /// <summary>The share of compute that goes to training while a run is active. The rest serves customers.</summary>
        public int TrainingSharePermille;
        /// <summary>The founder's equity, in permille. Drops when funding rounds close.</summary>
        public int FounderSharePermille;
        /// <summary>Investor seats on the board after venture rounds.</summary>
        public int BoardInvestorSeats;
        /// <summary>Plan-run rivals: how much stock the public holds, in ppm.</summary>
        public int PublicFloatPpm;
        /// <summary>Player accepted China's infrastructure partnership (cheaper power, more scrutiny).</summary>
        public bool ChinaDealActive;
        /// <summary>A state request is waiting for accept or decline.</summary>
        public bool ChinaDealPending;
        /// <summary>Funding round ids this lab has closed, oldest first.</summary>
        public List<string> FundingRounds = new List<string>();

        /// <summary>What a rival thinks of the player, -1000 (feud) to 1000 (fan). Always 0 for the player's own lab.</summary>
        public int FeelingToPlayer;
        /// <summary>The first tick the plan acts again after a board fit, or -1.</summary>
        public int PlanPausedUntilTick;
        /// <summary>Player commands are frozen until this tick after investor board drama, or -1.</summary>
        public int BoardLockedUntilTick;
        /// <summary>Until this tick the vendor's keynote has upstaged the lab's new chip and it will not buy lists, or -1.</summary>
        public int ChipAmbushUntilTick;
        /// <summary>Why the lab did what it did, newest last, capped at the config's rivalry.logMax.</summary>
        public List<DecisionEntry> Log = new List<DecisionEntry>();

        public List<ChipDesign> ChipDesigns = new List<ChipDesign>();
        /// <summary>How mature the lab's chip software is, in permille of paper speed: 0 until its first design starts, then it grows.</summary>
        public int SoftwarePermille;
        public List<ChipOrder> ChipOrders = new List<ChipOrder>();
        public List<ChipListing> ChipListings = new List<ChipListing>();

        public long ResearchPoints;
        public List<string> Techs = new List<string>();
        public List<ModelState> Models = new List<ModelState>();
        public int FrontierLevel;
        public int Heat;
        public long Stash;
        public int ShadyLockedUntilTick;
        public int CaseEndsTick = -1;
        public List<ShadyOp> ShadyOps = new List<ShadyOp>();

        public LedgerState Ledger = new LedgerState();
        public WeekReport LastWeek = new WeekReport();

        public HardwareBatch Batch(int id)
        {
            for (int i = 0; i < Fleet.Count; i++) if (Fleet[i].Id == id) return Fleet[i];
            return null;
        }

        public ChipDesign Design(int id)
        {
            for (int i = 0; i < ChipDesigns.Count; i++) if (ChipDesigns[i].Id == id) return ChipDesigns[i];
            return null;
        }

        public ChipListing Listing(int designId)
        {
            for (int i = 0; i < ChipListings.Count; i++) if (ChipListings[i].DesignId == designId) return ChipListings[i];
            return null;
        }

        public Holding Holding(int id)
        {
            for (int i = 0; i < Holdings.Count; i++) if (Holdings[i].Id == id) return Holdings[i];
            return null;
        }

        public BlocStanding Bloc(string blocId)
        {
            for (int i = 0; i < Blocs.Count; i++) if (Blocs[i].BlocId == blocId) return Blocs[i];
            return null;
        }

        public ModelState Model(int id)
        {
            for (int i = 0; i < Models.Count; i++) if (Models[i].Id == id) return Models[i];
            return null;
        }

        public RoomState Room(int id)
        {
            for (int i = 0; i < Rooms.Count; i++) if (Rooms[i].Id == id) return Rooms[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("name", ref Name);
            s.Str("leader", ref Leader);
            s.Str("familyName", ref FamilyName);
            s.Enum("controller", ref Controller);
            s.Str("rivalId", ref RivalId);
            s.Int("planStartTick", ref PlanStartTick);
            s.Long("cash", ref Cash);
            s.Int("reputation", ref Reputation);
            s.Bool("bankrupt", ref Bankrupt);
            s.Int("weeksInDebt", ref WeeksInDebt);
            s.Long("grantPerWeek", ref GrantPerWeek);
            s.Int("grantEndsTick", ref GrantEndsTick);
            s.Str("homeBlocId", ref HomeBlocId);
            s.Str("lotId", ref LotId);
            s.ObjList("rooms", ref Rooms);
            s.ObjList("staff", ref Staff);
            s.Int("rentedGpus", ref RentedGpus);
            s.ObjList("reservedContracts", ref ReservedContracts);
            s.ObjList("fleet", ref Fleet);
            s.ObjList("holdings", ref Holdings);
            s.ObjList("blocs", ref Blocs);
            s.Int("incidentsThisMonth", ref IncidentsThisMonth);
            s.Int("trainingSharePermille", ref TrainingSharePermille);
            s.Int("founderSharePermille", ref FounderSharePermille);
            s.Int("boardInvestorSeats", ref BoardInvestorSeats);
            s.Int("publicFloatPpm", ref PublicFloatPpm);
            s.Bool("chinaDealActive", ref ChinaDealActive);
            s.Bool("chinaDealPending", ref ChinaDealPending);
            s.StrList("fundingRounds", ref FundingRounds);
            s.Int("feelingToPlayer", ref FeelingToPlayer);
            s.Int("planPausedUntilTick", ref PlanPausedUntilTick);
            s.Int("boardLockedUntilTick", ref BoardLockedUntilTick);
            s.Int("chipAmbushUntilTick", ref ChipAmbushUntilTick);
            s.ObjList("log", ref Log);
            s.ObjList("chipDesigns", ref ChipDesigns);
            s.Int("softwarePermille", ref SoftwarePermille);
            s.ObjList("chipOrders", ref ChipOrders);
            s.ObjList("chipListings", ref ChipListings);
            s.Long("researchPoints", ref ResearchPoints);
            s.StrList("techs", ref Techs);
            s.ObjList("models", ref Models);
            s.Int("frontierLevel", ref FrontierLevel);
            s.Int("heat", ref Heat);
            s.Long("stash", ref Stash);
            s.Int("shadyLockedUntilTick", ref ShadyLockedUntilTick);
            s.Int("caseEndsTick", ref CaseEndsTick);
            s.ObjList("shadyOps", ref ShadyOps);
            s.Obj("ledger", ref Ledger);
            s.Obj("lastWeek", ref LastWeek);
        }
    }

    /// <summary>A room on the lot's grid. X and Y are its top-left tile; the lot's top-left tile is 0,0.</summary>
    public sealed class RoomState : ISync
    {
        public int Id;
        public string TypeId;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Tiles { get { return Width * Height; } }

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("typeId", ref TypeId);
            s.Int("x", ref X);
            s.Int("y", ref Y);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
        }
    }

    /// <summary>One named person on the payroll.</summary>
    public sealed class StaffMember : ISync
    {
        public int Id;
        public string RoleId = "";
        public string TraitId = "";
        public int Morale;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("roleId", ref RoleId);
            s.Str("traitId", ref TraitId);
            s.Int("morale", ref Morale);
        }
    }

    public enum ModelStatus { Training, Trained, Released, Retired }

    /// <summary>How a model's creature looks right now. Derived from the state, never stored.</summary>
    public enum ModelMood { Healthy, Overworked, Glitching, Corrupted }

    public sealed class ModelState : ISync
    {
        public int Id;
        /// <summary>Mini, Mid or Flagship (a model line id of the config).</summary>
        public string LineId;
        public int Version;
        public string Name;
        public ModelStatus Status;

        /// <summary>Zero while training. Once trained it is exactly CapBase + CapTech + CapCarryover + CapLuck.</summary>
        public int Capability;
        public int CapBase;
        public int CapTech;
        public int CapCarryover;
        public int CapLuck;

        /// <summary>The safety meter, 0 to 1000. Zero while training.</summary>
        public int Alignment;
        /// <summary>What customers and screens are told. Can sit above <see cref="Alignment"/> when the lab sandbags.</summary>
        public int DisplayedAlignment;
        /// <summary>The ladder rung of the last incident, 1 to 3 in this slice.</summary>
        public int LastIncidentRung;
        public long RequiredCompute;
        public long ProgressCompute;
        public long AlignmentCompute;
        public int AlignmentSharePermille;

        public int StartedTick;
        public int TrainedTick;
        public int ReleasedTick;
        public int LastIncidentTick;
        public int Incidents;
        public int Corruption;
        /// <summary>The sectors this model is on sale in.</summary>
        public List<string> Sectors = new List<string>();

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("lineId", ref LineId);
            s.Int("version", ref Version);
            s.Str("name", ref Name);
            s.Enum("status", ref Status);
            s.Int("capability", ref Capability);
            s.Int("capBase", ref CapBase);
            s.Int("capTech", ref CapTech);
            s.Int("capCarryover", ref CapCarryover);
            s.Int("capLuck", ref CapLuck);
            s.Int("alignment", ref Alignment);
            s.Int("displayedAlignment", ref DisplayedAlignment);
            s.Int("lastIncidentRung", ref LastIncidentRung);
            s.Long("requiredCompute", ref RequiredCompute);
            s.Long("progressCompute", ref ProgressCompute);
            s.Long("alignmentCompute", ref AlignmentCompute);
            s.Int("alignmentSharePermille", ref AlignmentSharePermille);
            s.Int("startedTick", ref StartedTick);
            s.Int("trainedTick", ref TrainedTick);
            s.Int("releasedTick", ref ReleasedTick);
            s.Int("lastIncidentTick", ref LastIncidentTick);
            s.Int("incidents", ref Incidents);
            s.Int("corruption", ref Corruption);
            s.StrList("sectors", ref Sectors);
        }
    }

    public sealed class SectorState : ISync
    {
        public string Id;
        /// <summary>What the whole sector would pay per week today. Grows every year.</summary>
        public long PoolPerWeek;
        /// <summary>How much of the pool has a use for AI at all, given the best model on sale.</summary>
        public int AdoptionPermille;
        /// <summary>The lab that won this sector's bracket at the last Gauntlet, or -1.</summary>
        public int ChampionLabId;
        public List<SectorEntry> Entries = new List<SectorEntry>();

        public SectorEntry Entry(int labId)
        {
            for (int i = 0; i < Entries.Count; i++) if (Entries[i].LabId == labId) return Entries[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Str("id", ref Id);
            s.Long("poolPerWeek", ref PoolPerWeek);
            s.Int("adoptionPermille", ref AdoptionPermille);
            s.Int("championLabId", ref ChampionLabId);
            s.ObjList("entries", ref Entries);
        }
    }

    /// <summary>One lab's model on sale in one sector, with the share of customers it holds.</summary>
    public sealed class SectorEntry : ISync
    {
        public int LabId;
        public int ModelId;
        /// <summary>Share of the sector's customers, in parts per million.</summary>
        public int SharePpm;
        public long LastRevenue;

        public void Sync(Syncer s)
        {
            s.Int("labId", ref LabId);
            s.Int("modelId", ref ModelId);
            s.Int("sharePpm", ref SharePpm);
            s.Long("lastRevenue", ref LastRevenue);
        }
    }

    public sealed class GauntletRecord : ISync
    {
        public int Year;
        public int CrownLabId;
        public int CrownModelId;
        public int SafestLabId;
        public List<GauntletEntry> Entries = new List<GauntletEntry>();

        public void Sync(Syncer s)
        {
            s.Int("year", ref Year);
            s.Int("crownLabId", ref CrownLabId);
            s.Int("crownModelId", ref CrownModelId);
            s.Int("safestLabId", ref SafestLabId);
            s.ObjList("entries", ref Entries);
        }
    }

    public sealed class GauntletEntry : ISync
    {
        /// <summary>"crown" for the overall ranking, else the sector id of the bracket.</summary>
        public string Bracket;
        /// <summary>1 is the winner.</summary>
        public int Rank;
        public int LabId;
        public int ModelId;
        public string ModelName;
        public long Score;

        public void Sync(Syncer s)
        {
            s.Str("bracket", ref Bracket);
            s.Int("rank", ref Rank);
            s.Int("labId", ref LabId);
            s.Int("modelId", ref ModelId);
            s.Str("modelName", ref ModelName);
            s.Long("score", ref Score);
        }
    }
}
