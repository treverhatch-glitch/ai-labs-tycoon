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
        public const int CurrentSchema = 87;

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
        /// <summary>Cumulative venture cash that inflated the bubble.</summary>
        public long BubbleCapitalRaised;
        /// <summary>Weak-return months while the meter is hot.</summary>
        public int BubbleGraceMonths;
        /// <summary>Months left in a market-wide crash, or 0.</summary>
        public int BubbleCrashMonthsRemaining;
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
            s.Long("bubbleCapitalRaised", ref BubbleCapitalRaised);
            s.Int("bubbleGraceMonths", ref BubbleGraceMonths);
            s.Int("bubbleCrashMonthsRemaining", ref BubbleCrashMonthsRemaining);
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
        /// <summary>The share of general compute set aside for research experiments before training and customers divide the rest. Zero until the player sets it.</summary>
        public int ResearchSharePermille;
        /// <summary>Weeks the team has been in crunch time, or 0 when it is not. Only the player's lab can start one.</summary>
        public int CrunchWeeks;
        /// <summary>The incident insurance the lab pays for: 0 for none, up to <c>finance.insuranceMaxLevel</c>. Only the player's lab takes it.</summary>
        public int InsuranceLevel;
        /// <summary>Claims the insurer still remembers. They load the premium and, at <c>finance.insuranceDropClaims</c>, end the policy.</summary>
        public int InsuranceClaims;
        /// <summary>The policy cannot be changed before this tick.</summary>
        public int InsuranceLockedUntil;
        /// <summary>The level of the interpretability programme the lab runs, 0 for none. Only the player's lab runs one.</summary>
        public int InterpLevel;
        /// <summary>The level of the compliance office the lab runs, 0 for none. Only the player's lab runs one.</summary>
        public int ComplianceLevel;
        /// <summary>The lab's academic prestige from papers it published. It fades every week. Only the player's lab publishes.</summary>
        public int Prestige;
        /// <summary>The share of general compute the lab lets to outside customers, in permille. Zero until the player sets it.</summary>
        public int SubletPermille;
        /// <summary>The share cannot be changed before this tick.</summary>
        public int SubletLockedUntil;
        /// <summary>The research direction the lab picked, "" for none. Only the player's lab picks one.</summary>
        public string ResearchFocus = "";
        /// <summary>The focus cannot be changed before this tick.</summary>
        public int FocusLockedUntil;
        /// <summary>The sector of the current or last marketing campaign, empty when there is none (running or being forgiven).</summary>
        public string CampaignSector = "";
        /// <summary>The budget level of that campaign.</summary>
        public int CampaignLevel;
        /// <summary>The weeks the campaign still has to run, 0 when none is on.</summary>
        public int CampaignWeeks;
        /// <summary>The weeks customers still feel let down after a campaign ended.</summary>
        public int HangoverWeeks;
        /// <summary>The next campaign cannot start before this tick.</summary>
        public int CampaignLockedUntil;
        /// <summary>The level of inference optimisation, 0 for none. Only the player's lab does it.</summary>
        public int OptimiseLevel;
        /// <summary>The level of the on-call rota, 0 for none. Only the player's lab runs one.</summary>
        public int OnCallLevel;
        /// <summary>The rota cannot be changed before this tick.</summary>
        public int OnCallLockedUntil;
        /// <summary>The service-level pledge (0 is none).</summary>
        public int SlaLevel;
        /// <summary>The week the pledge can next be changed.</summary>
        public int SlaLockedUntil;
        /// <summary>Broken weeks in a row.</summary>
        public int SlaStrikes;
        /// <summary>0 with no grant, 1 while one is under review, 2 while the papers for an awarded one are owed.</summary>
        public int ResearchGrantStage;
        /// <summary>The tier applied for (0 with no grant).</summary>
        public int ResearchGrantTier;
        /// <summary>The week the review ends (stage 1) or the papers are due (stage 2).</summary>
        public int ResearchGrantTick;
        /// <summary>The papers still owed for an awarded grant.</summary>
        public int ResearchGrantOwed;
        /// <summary>The week the lab can next apply.</summary>
        public int ResearchGrantLockedUntil;
        /// <summary>The level of the firm of policy consultants that is in, 0 when none. Only the player's lab hires them.</summary>
        public int ConsultantLevel;
        /// <summary>Technical debt, 0 to 1000. Grows with rushed launches, crunch weeks and patched glitches; only paying it down lowers it.</summary>
        public int TechDebt;
        /// <summary>University chairs the lab has endowed. Only the player's lab endows them.</summary>
        public int Chairs;
        /// <summary>Weeks a model has shown more alignment than it has while the safety staff were unhappy, towards a whistleblower raising it.</summary>
        public int WhistleWeeks;
        /// <summary>The model a raised concern is about, valid while <see cref="WhistleDue"/> is above zero.</summary>
        public int WhistleModel;
        /// <summary>The week an unanswered concern leaks, 0 when none is open.</summary>
        public int WhistleDue;
        /// <summary>Silences bought and not yet leaked. Each makes the next dearer and the leak worse.</summary>
        public int Silenced;
        /// <summary>The level of the lab's bug bounty, 0 when none. Only the player's lab runs one.</summary>
        public int BountyLevel;
        /// <summary>The week the level can next change.</summary>
        public int BountyLockedUntil;
        /// <summary>Cash the lab has set aside in its treasury reserve. Only the player's lab keeps one.</summary>
        public long Reserve;
        /// <summary>The level of the lab's public safety pledge, 0 when none. Only the player's lab makes one.</summary>
        public int PledgeLevel;
        /// <summary>The week the pledge can next change.</summary>
        public int PledgeLockedUntil;
        /// <summary>The level of the roadmap the lab has announced, 0 when none.</summary>
        public int RoadmapLevel;
        /// <summary>The week it was announced.</summary>
        public int RoadmapStart;
        /// <summary>The week it is judged.</summary>
        public int RoadmapDue;
        /// <summary>The capability a model must have to deliver it.</summary>
        public int RoadmapGoal;
        /// <summary>The kind of skunkworks project the lab runs (research, safety or engineering), empty when none.</summary>
        public string SkunkKind = "";
        /// <summary>People on the team.</summary>
        public int SkunkPeople;
        /// <summary>The week it started and the week it pays out.</summary>
        public int SkunkStart;
        public int SkunkDue;
        /// <summary>Person-weeks the team has worked so far.</summary>
        public long SkunkWork;
        /// <summary>The week the next project can start.</summary>
        public int SkunkLockedUntil;
        /// <summary>The labour stage: 0 quiet, 1 organising (waiting for the player), 2 union recognised, 3 resisting, 4 on strike.</summary>
        public int LabourStage;
        /// <summary>Staff pressure to organise, 0 to 1000.</summary>
        public int LabourPressure;
        /// <summary>The week the stage ends (stages 1, 3 and 4), 0 otherwise.</summary>
        public int LabourDue;
        /// <summary>1 when the contract was forced by a strike.</summary>
        public int LabourHard;
        /// <summary>The salespeople assigned to sectors, one team a sector.</summary>
        public List<AccountTeam> AccountTeams = new List<AccountTeam>();
        /// <summary>Papers out for review, oldest first.</summary>
        public List<Submission> Submissions = new List<Submission>();
        /// <summary>People who left the lab and can be asked back, oldest first.</summary>
        public List<Alumnus> Alumni = new List<Alumnus>();
        /// <summary>The role of the cohort of interns in the lab now, empty when there is none.</summary>
        public string InternRole = "";
        /// <summary>How many interns are in the cohort.</summary>
        public int InternCount;
        /// <summary>The week the cohort began and the week it ends.</summary>
        public int InternStart;
        public int InternDue;
        /// <summary>Engineers on the support desk.</summary>
        public int SupportPeople;
        /// <summary>Tickets the desk has not cleared.</summary>
        public int SupportBacklog;
        /// <summary>Open orders for people, soonest first.</summary>
        public List<Requisition> Requisitions = new List<Requisition>();
        /// <summary>Visiting scholars in the lab now, and the week they leave.</summary>
        public int ScholarCount;
        public int ScholarStart;
        public int ScholarDue;
        /// <summary>Claims against the lab, soonest hearing first, and the number the next one takes.</summary>
        public List<LawCase> Cases = new List<LawCase>();
        public int CaseSeq;
        /// <summary>The lab's security tier, the tier being built (0 when none) and the week it is done.</summary>
        public int SecurityTier;
        public int SecurityBuilding;
        public int SecurityDue;
        /// <summary>How exposed the lab is, the week an open breach leaks (0 when none) and the ransoms paid since the last disclosure.</summary>
        public int Exposure;
        public int BreachDue;
        public int Ransoms;
        /// <summary>The government contract in force (empty when none), the week it began and ends, the instalments paid and missed.</summary>
        public string ContractKind = "";
        public int ContractStart;
        public int ContractDue;
        public int ContractPaid;
        public int ContractMissed;
        /// <summary>The contract on offer (empty when none), the week the offer lapses, which offer comes next and the week the next one is due.</summary>
        public string OfferKind = "";
        public int OfferDue;
        public int OfferSeq;
        public int ContractNext;
        /// <summary>The lab's charter (0 for-profit, 1 capped-profit, 2 nonprofit), the charter under review (0 none, otherwise the form plus one), the week the review ends and what investors have taken so far under a cap.</summary>
        public int Charter;
        public int CharterTo;
        public int CharterDue;
        public long CapPaid;
        /// <summary>The scaling policy level, the week until which it cannot be lowered, the level of the tripwire now holding the lab up (0 when none) and the times the policy was broken.</summary>
        public int PolicyLevel;
        public int PolicyLockedUntil;
        public int PolicyHold;
        public int PolicyBreaks;
        /// <summary>The tax structure kept (0 none, 1 a holding company, 2 an offshore one), the one being built (0 when none) and the week it opens.</summary>
        public int TaxStructure;
        public int TaxBuilding;
        public int TaxDue;
        /// <summary>Tax owed so far this year, what the structure has saved and the credit claims have taken off since the last audit, the year's training spend and the credit claimed but not yet used.</summary>
        public long TaxAccrued;
        public long TaxSaved;
        public long TaxResearch;
        public long TaxCredit;
        public long TaxClaimed;
        /// <summary>How exposed the lab is to a tax audit, the week an open audit must be answered by (0 when none) and what it is about.</summary>
        public int TaxExposure;
        public int TaxAuditDue;
        public long TaxAuditOwed;
        /// <summary>The standards ratified so far, the level being drafted (0 when none), the support it has, the lab's part in the draft, the week the next draft starts, the level the lab has adopted and the times it released a capable model without complying.</summary>
        public int StdLevel;
        public int StdDraft;
        public int StdSupport;
        public int StdRole;
        public int StdNext;
        public int StdAdopted;
        public int StdLapses;
        /// <summary>The warranty tier, the spare GPUs held, the part-failures carried over in millionths of a unit, the units waiting to be replaced, the week they come and the failures so far.</summary>
        public int WarrantyTier;
        public int Spares;
        public long FailCarry;
        public int RmaUnits;
        public int RmaDue;
        public int GpuFailures;
        /// <summary>The level of the revenue target the lab has committed to, 0 when none.</summary>
        public int RevenueTargetLevel;
        /// <summary>The week the target is judged.</summary>
        public int RevenueTargetEnd;
        /// <summary>The revenue the lab promised to make by then.</summary>
        public long RevenueTargetGoal;
        /// <summary>The revenue made since the commitment.</summary>
        public long RevenueTargetSoFar;
        /// <summary>The weeks the firm still has to stay, 0 when none is in.</summary>
        public int ConsultantWeeks;
        /// <summary>The next firm cannot start before this tick.</summary>
        public int ConsultantLockedUntil;
        /// <summary>The level of the ethics board, 0 for none. Only the player's lab seats one.</summary>
        public int EthicsLevel;
        /// <summary>No board can be seated before this tick (set when one is stepped down or resigns).</summary>
        public int EthicsLockedUntil;
        /// <summary>The person named chief scientist, or -1 for nobody. Only the player's lab names one.</summary>
        public int ChiefScientistId = -1;
        /// <summary>The person named chief safety officer, or -1 for nobody.</summary>
        public int ChiefSafetyId = -1;
        /// <summary>The levels of staff equity the lab has granted, 0 for none. Only the player's lab grants it.</summary>
        public int EquityLevel;
        /// <summary>The share of the company the people own, in permille of the company. Funding rounds dilute it.</summary>
        public int EquityPermille;
        /// <summary>The weeks of hack week still to run, 0 when none is on. Only the player's lab holds one.</summary>
        public int HackWeeks;
        /// <summary>The next hack week cannot be held before this tick.</summary>
        public int HackLockedUntil;
        /// <summary>The pay policy: 0 is market pay, above is better paid, below is worse. Only the player's lab changes it.</summary>
        public int PayLevel;
        /// <summary>The pay policy cannot be changed before this tick.</summary>
        public int PayLockedUntil;
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
        /// <summary>No new China offer until this tick, or -1.</summary>
        public int ChinaDealDeclinedUntilTick = -1;
        /// <summary>When bankrupt after acquisition, names the buyer lab id, or -1.</summary>
        public int AbsorbedByLabId = -1;
        /// <summary>Who the player chose to be at the start (a founder id), or empty for none. Parody rivals have none.</summary>
        public string FounderId = "";
        /// <summary>Rivals' offers to the player's people that still wait for an answer.</summary>
        public List<PoachOffer> PoachOffers = new List<PoachOffer>();
        /// <summary>How hard this lab tunes its entries for the Gauntlet, in permille. 0 enters them as they are.</summary>
        public int GauntletTuning;
        /// <summary>The lab's safety culture as a shift from the level every lab starts at (<c>culture.start</c>); see SafetyCulture.</summary>
        public int CultureShift;
        /// <summary>The least alignment a model may show and still go on sale, set by the lab for itself. 0 = no gate.</summary>
        public int ReleaseGate;
        /// <summary>The tick the gate last changed; it cannot be loosened until <c>culture.gateLockWeeks</c> later.</summary>
        public int GateSetTick;
        /// <summary>How confusing the lab's model names have become, from 0 (tidy) up. Only the player's labs let it grow.</summary>
        public int NamingMess;
        /// <summary>Lucky runs nobody has explained yet.</summary>
        public List<ResearchLead> Leads = new List<ResearchLead>();
        /// <summary>The training speed that followed leads have added, in permille.</summary>
        public int LeadSpeedPermille;
        /// <summary>The office event waiting for an answer (an id of <c>office.events</c>), or empty.</summary>
        public string OfficeEventId = "";
        public int OfficeEventExpires;
        /// <summary>Office events this lab has had in the current round of the list.</summary>
        public List<string> OfficeSeen = new List<string>();
        /// <summary>Journalists' questions that still wait for an answer.</summary>
        public List<PressInquiry> PressInquiries = new List<PressInquiry>();
        public List<SectorTrust> SectorTrust = new List<SectorTrust>();
        public List<Complaint> Complaints = new List<Complaint>();
        /// <summary>How the player chose to be good at the start (a style id), or empty for none.</summary>
        public string StyleId = "";
        /// <summary>The era (an era id) the lab last changed its style in, or empty. A lab can pivot once per era.</summary>
        public List<EraScore> EraScores = new List<EraScore>();
        public string PivotEraId = "";
        /// <summary>Customers are confused by the last pivot until this tick.</summary>
        public int PivotUntilTick;
        /// <summary>Funding round ids this lab has closed, oldest first.</summary>
        public List<string> FundingRounds = new List<string>();
        /// <summary>Loans against the lab's chips that are still being paid back, oldest first.</summary>
        public List<Loan> Loans = new List<Loan>();

        /// <summary>What a rival thinks of the player, -1000 (feud) to 1000 (fan). Always 0 for the player's own lab.</summary>
        public int FeelingToPlayer;
        /// <summary>The first tick the plan acts again after a board fit, or -1.</summary>
        public int PlanPausedUntilTick;
        /// <summary>Player commands are frozen until this tick after investor board drama, or -1.</summary>
        public int BoardLockedUntilTick;
        /// <summary>Consecutive years the lab was crown runner-up at the Gauntlet.</summary>
        public int CrownRunnerUpStreak;
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
            s.Int("researchSharePermille", ref ResearchSharePermille);
            s.Int("crunchWeeks", ref CrunchWeeks);
            s.Int("insuranceLevel", ref InsuranceLevel);
            s.Int("insuranceClaims", ref InsuranceClaims);
            s.Int("insuranceLockedUntil", ref InsuranceLockedUntil);
            s.Int("interpLevel", ref InterpLevel);
            s.Int("complianceLevel", ref ComplianceLevel);
            s.Int("prestige", ref Prestige);
            s.Int("subletPermille", ref SubletPermille);
            s.Int("subletLockedUntil", ref SubletLockedUntil);
            s.Str("researchFocus", ref ResearchFocus);
            s.Int("focusLockedUntil", ref FocusLockedUntil);
            s.Str("campaignSector", ref CampaignSector);
            s.Int("campaignLevel", ref CampaignLevel);
            s.Int("campaignWeeks", ref CampaignWeeks);
            s.Int("hangoverWeeks", ref HangoverWeeks);
            s.Int("campaignLockedUntil", ref CampaignLockedUntil);
            s.Int("optimiseLevel", ref OptimiseLevel);
            s.Int("onCallLevel", ref OnCallLevel);
            s.Int("onCallLockedUntil", ref OnCallLockedUntil);
            s.Int("slaLevel", ref SlaLevel);
            s.Int("slaLockedUntil", ref SlaLockedUntil);
            s.Int("slaStrikes", ref SlaStrikes);
            s.Int("researchGrantStage", ref ResearchGrantStage);
            s.Int("researchGrantTier", ref ResearchGrantTier);
            s.Int("researchGrantTick", ref ResearchGrantTick);
            s.Int("researchGrantOwed", ref ResearchGrantOwed);
            s.Int("researchGrantLockedUntil", ref ResearchGrantLockedUntil);
            s.Int("consultantLevel", ref ConsultantLevel);
            s.Int("techDebt", ref TechDebt);
            s.Int("chairs", ref Chairs);
            s.Int("whistleWeeks", ref WhistleWeeks);
            s.Int("whistleModel", ref WhistleModel);
            s.Int("whistleDue", ref WhistleDue);
            s.Int("silenced", ref Silenced);
            s.Int("bountyLevel", ref BountyLevel);
            s.Int("bountyLockedUntil", ref BountyLockedUntil);
            s.Long("reserve", ref Reserve);
            s.Int("pledgeLevel", ref PledgeLevel);
            s.Int("pledgeLockedUntil", ref PledgeLockedUntil);
            s.Int("roadmapLevel", ref RoadmapLevel);
            s.Int("roadmapStart", ref RoadmapStart);
            s.Int("roadmapDue", ref RoadmapDue);
            s.Int("roadmapGoal", ref RoadmapGoal);
            s.Str("skunkKind", ref SkunkKind);
            s.Int("skunkPeople", ref SkunkPeople);
            s.Int("skunkStart", ref SkunkStart);
            s.Int("skunkDue", ref SkunkDue);
            s.Long("skunkWork", ref SkunkWork);
            s.Int("skunkLockedUntil", ref SkunkLockedUntil);
            s.Int("labourStage", ref LabourStage);
            s.Int("labourPressure", ref LabourPressure);
            s.Int("labourDue", ref LabourDue);
            s.Int("labourHard", ref LabourHard);
            s.ObjList("accountTeams", ref AccountTeams);
            s.ObjList("submissions", ref Submissions);
            s.ObjList("alumni", ref Alumni);
            s.Str("internRole", ref InternRole);
            s.Int("internCount", ref InternCount);
            s.Int("internStart", ref InternStart);
            s.Int("internDue", ref InternDue);
            s.Int("supportPeople", ref SupportPeople);
            s.Int("supportBacklog", ref SupportBacklog);
            s.ObjList("requisitions", ref Requisitions);
            s.Int("scholarCount", ref ScholarCount);
            s.Int("scholarStart", ref ScholarStart);
            s.Int("scholarDue", ref ScholarDue);
            s.ObjList("cases", ref Cases);
            s.Int("caseSeq", ref CaseSeq);
            s.Int("securityTier", ref SecurityTier);
            s.Int("securityBuilding", ref SecurityBuilding);
            s.Int("securityDue", ref SecurityDue);
            s.Int("exposure", ref Exposure);
            s.Int("breachDue", ref BreachDue);
            s.Int("ransoms", ref Ransoms);
            s.Str("contractKind", ref ContractKind);
            s.Int("contractStart", ref ContractStart);
            s.Int("contractDue", ref ContractDue);
            s.Int("contractPaid", ref ContractPaid);
            s.Int("contractMissed", ref ContractMissed);
            s.Str("offerKind", ref OfferKind);
            s.Int("offerDue", ref OfferDue);
            s.Int("offerSeq", ref OfferSeq);
            s.Int("contractNext", ref ContractNext);
            s.Int("charter", ref Charter);
            s.Int("charterTo", ref CharterTo);
            s.Int("charterDue", ref CharterDue);
            s.Long("capPaid", ref CapPaid);
            s.Int("policyLevel", ref PolicyLevel);
            s.Int("policyLockedUntil", ref PolicyLockedUntil);
            s.Int("policyHold", ref PolicyHold);
            s.Int("policyBreaks", ref PolicyBreaks);
            s.Int("taxStructure", ref TaxStructure);
            s.Int("taxBuilding", ref TaxBuilding);
            s.Int("taxDue", ref TaxDue);
            s.Long("taxAccrued", ref TaxAccrued);
            s.Long("taxSaved", ref TaxSaved);
            s.Long("taxResearch", ref TaxResearch);
            s.Long("taxCredit", ref TaxCredit);
            s.Long("taxClaimed", ref TaxClaimed);
            s.Int("taxExposure", ref TaxExposure);
            s.Int("taxAuditDue", ref TaxAuditDue);
            s.Long("taxAuditOwed", ref TaxAuditOwed);
            s.Int("stdLevel", ref StdLevel);
            s.Int("stdDraft", ref StdDraft);
            s.Int("stdSupport", ref StdSupport);
            s.Int("stdRole", ref StdRole);
            s.Int("stdNext", ref StdNext);
            s.Int("stdAdopted", ref StdAdopted);
            s.Int("stdLapses", ref StdLapses);
            s.Int("warrantyTier", ref WarrantyTier);
            s.Int("spares", ref Spares);
            s.Long("failCarry", ref FailCarry);
            s.Int("rmaUnits", ref RmaUnits);
            s.Int("rmaDue", ref RmaDue);
            s.Int("gpuFailures", ref GpuFailures);
            s.Int("revenueTargetLevel", ref RevenueTargetLevel);
            s.Int("revenueTargetEnd", ref RevenueTargetEnd);
            s.Long("revenueTargetGoal", ref RevenueTargetGoal);
            s.Long("revenueTargetSoFar", ref RevenueTargetSoFar);
            s.Int("consultantWeeks", ref ConsultantWeeks);
            s.Int("consultantLockedUntil", ref ConsultantLockedUntil);
            s.Int("ethicsLevel", ref EthicsLevel);
            s.Int("ethicsLockedUntil", ref EthicsLockedUntil);
            s.Int("chiefScientistId", ref ChiefScientistId);
            s.Int("chiefSafetyId", ref ChiefSafetyId);
            s.Int("equityLevel", ref EquityLevel);
            s.Int("equityPermille", ref EquityPermille);
            s.Int("hackWeeks", ref HackWeeks);
            s.Int("hackLockedUntil", ref HackLockedUntil);
            s.Int("payLevel", ref PayLevel);
            s.Int("payLockedUntil", ref PayLockedUntil);
            s.Int("founderSharePermille", ref FounderSharePermille);
            s.Int("boardInvestorSeats", ref BoardInvestorSeats);
            s.Int("publicFloatPpm", ref PublicFloatPpm);
            s.Bool("chinaDealActive", ref ChinaDealActive);
            s.Bool("chinaDealPending", ref ChinaDealPending);
            s.Int("chinaDealDeclinedUntilTick", ref ChinaDealDeclinedUntilTick);
            s.Int("absorbedByLabId", ref AbsorbedByLabId);
            s.Str("founderId", ref FounderId);
            s.ObjList("poachOffers", ref PoachOffers);
            s.Int("gauntletTuning", ref GauntletTuning);
            s.Int("cultureShift", ref CultureShift);
            s.Int("releaseGate", ref ReleaseGate);
            s.Int("gateSetTick", ref GateSetTick);
            s.Int("namingMess", ref NamingMess);
            s.ObjList("researchLeads", ref Leads);
            s.Int("leadSpeedPermille", ref LeadSpeedPermille);
            s.Str("officeEventId", ref OfficeEventId);
            s.Int("officeEventExpires", ref OfficeEventExpires);
            s.StrList("officeSeen", ref OfficeSeen);
            s.ObjList("pressInquiries", ref PressInquiries);
            s.ObjList("sectorTrust", ref SectorTrust);
            s.ObjList("complaints", ref Complaints);
            s.Str("styleId", ref StyleId);
            s.ObjList("eraScores", ref EraScores);
            s.Str("pivotEraId", ref PivotEraId);
            s.Int("pivotUntilTick", ref PivotUntilTick);
            s.StrList("fundingRounds", ref FundingRounds);
            s.ObjList("loans", ref Loans);
            s.Int("feelingToPlayer", ref FeelingToPlayer);
            s.Int("planPausedUntilTick", ref PlanPausedUntilTick);
            s.Int("boardLockedUntilTick", ref BoardLockedUntilTick);
            s.Int("crownRunnerUpStreak", ref CrownRunnerUpStreak);
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
        /// <summary>How many eras have begun since the room was built or last refurbished. Only the player's rooms age.</summary>
        public int EraAge;

        public int Tiles { get { return Width * Height; } }

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("typeId", ref TypeId);
            s.Int("x", ref X);
            s.Int("y", ref Y);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
            s.Int("eraAge", ref EraAge);
        }
    }

    /// <summary>One named person on the payroll.</summary>
    public sealed class StaffMember : ISync
    {
        public int Id;
        public string RoleId = "";
        public string TraitId = "";
        public int Morale;
        /// <summary>1 when the person has been promoted to senior, 0 otherwise.</summary>
        public int Senior;
        /// <summary>The id of the senior mentoring this person, 0 when nobody is.</summary>
        public int MentorId;
        /// <summary>The weeks the pair has held.</summary>
        public int MentorWeeks;
        /// <summary>The week the person is back from a sabbatical, 0 when they are not away.</summary>
        public int LeaveDue;
        /// <summary>Their morale on the day they went.</summary>
        public int LeaveMorale;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("roleId", ref RoleId);
            s.Str("traitId", ref TraitId);
            s.Int("morale", ref Morale);
            s.Int("senior", ref Senior);
            s.Int("mentorId", ref MentorId);
            s.Int("mentorWeeks", ref MentorWeeks);
            s.Int("leaveDue", ref LeaveDue);
            s.Int("leaveMorale", ref LeaveMorale);
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
        /// <summary>The reputation points the model's last incident cost its lab.</summary>
        public int IncidentRepLoss;
        /// <summary>0 while the last incident has no post-mortem, 1 after a summary and 2 after a full write-up.</summary>
        public int PostMortemDone;
        /// <summary>What the run was for (an id of <c>objectives</c>), or empty for the default, to be correct.</summary>
        public string Objective = "";
        /// <summary>The model that helped design this one while it trained, or -1 when people did it alone.</summary>
        public int DesignerId = -1;
        /// <summary>How the run was built (an id of <c>runs.approaches</c>), or empty for the known architecture.</summary>
        public string Approach = "";
        /// <summary>How often the run was checkpointed (an id of <c>runs.checkpoints</c>), or empty for now and then.</summary>
        public string Checkpoints = "";
        /// <summary>Weeks of testing skipped when this model was first put on sale, or 0 for a tested launch.</summary>
        public int RushWeeks;
        /// <summary>The model is safer from red-team fixes until this tick; 0 when it was never red-teamed or the weeks are over.</summary>
        public int RedTeamUntil;
        /// <summary>The model holds an external audit certificate until this tick; 0 when it never had one, or it ran out or was pulled.</summary>
        public int AuditedUntil;
        /// <summary>The week the model's end-of-life notice was given, 0 when it has none.</summary>
        public int SunsetStart;
        /// <summary>The week the model leaves the market, 0 when it has no notice.</summary>
        public int SunsetTick;
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
            s.Int("incidentRepLoss", ref IncidentRepLoss);
            s.Int("postMortemDone", ref PostMortemDone);
            s.Str("objective", ref Objective);
            s.Int("designerId", ref DesignerId);
            s.Int("rushWeeks", ref RushWeeks);
            s.Int("redTeamUntil", ref RedTeamUntil);
            s.Int("auditedUntil", ref AuditedUntil);
            s.Int("sunsetStart", ref SunsetStart);
            s.Int("sunsetTick", ref SunsetTick);
            s.Str("approach", ref Approach);
            s.Str("checkpoints", ref Checkpoints);
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
        /// <summary>Who won "Most Confident While Wrong", or -1.</summary>
        public int ConfidentLabId = -1;
        /// <summary>Who won "Best Newcomer", or -1.</summary>
        public int NewcomerLabId = -1;
        public List<GauntletEntry> Entries = new List<GauntletEntry>();

        public void Sync(Syncer s)
        {
            s.Int("year", ref Year);
            s.Int("crownLabId", ref CrownLabId);
            s.Int("crownModelId", ref CrownModelId);
            s.Int("safestLabId", ref SafestLabId);
            s.Int("confidentLabId", ref ConfidentLabId);
            s.Int("newcomerLabId", ref NewcomerLabId);
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
        /// <summary>True if this score was lifted by tuning and the lift stood.</summary>
        public bool Tuned;

        public void Sync(Syncer s)
        {
            s.Str("bracket", ref Bracket);
            s.Int("rank", ref Rank);
            s.Int("labId", ref LabId);
            s.Int("modelId", ref ModelId);
            s.Str("modelName", ref ModelName);
            s.Long("score", ref Score);
            s.Bool("tuned", ref Tuned);
        }
    }
}
