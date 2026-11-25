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
        public const int CurrentSchema = 163;

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
        public bool BubbleHadCrash;
        public int RivalIntentLastTick = -1;
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
            s.Bool("bubbleHadCrash", ref BubbleHadCrash);
            s.Int("rivalIntentLastTick", ref RivalIntentLastTick);
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
        /// <summary>The weeks in breach of a loan covenant, the week a waiver lasts to, the waivers used and the times the lender has called the loans.</summary>
        public int CovBreach;
        public int CovWaiverUntil;
        public int CovWaivers;
        public int CovCalls;
        /// <summary>The capital allowance policy (0 expense, 1 capitalise, 2 enhanced), the pool still to write off, the week it was last changed and the total written off.</summary>
        public int CapPolicy;
        public long CapPool;
        public int CapSwitchTick;
        public long CapWritten;
        /// <summary>Whether the lab is listed, the share price (1000 at the listing), the guidance level set for the quarter (0 none), the revenue it promises and the revenue so far.</summary>
        public int Listed;
        public int ListPrice;
        public int ListGuidance;
        public long ListTarget;
        public long ListQuarter;
        /// <summary>The lab's smoothed weekly revenue, the week the quarter ends, the share of the lab still held in permille, the quarters in a row that met guidance, the week of the last follow-on offering and the activist's steps.</summary>
        public long ListBase;
        public int ListNext;
        public int ListKeep;
        public int ListStreak;
        public int ListFollowOn;
        public int ListActivist;
        /// <summary>The revenue advance taken, what is still owed on it, and the weekly revenue when it was taken.</summary>
        public long RbfAdvance;
        public long RbfOwed;
        public long RbfBase;
        /// <summary>The share of each week's revenue swept in permille, the tier, the weeks since it was taken and the weeks of slump.</summary>
        public int RbfShare;
        public int RbfTier;
        public int RbfWeeks;
        public int RbfSlump;
        /// <summary>The foundation's fund and what it has given in all.</summary>
        public long FoundFund;
        public long FoundGiven;
        /// <summary>Its focus (1 safety, 2 education, 3 open science), the giving level (0 to 3), the goodwill, the week the focus last changed and the weeks counted toward a reputation point or a report.</summary>
        public int FoundFocus;
        public int FoundPayout;
        public int FoundGoodwill;
        public int FoundSwitch;
        public int FoundDrip;
        /// <summary>GPUs sold and leased back, their value at the sale, the weekly rent and the price of buying them back.</summary>
        public int LeaseUnits;
        public long LeaseValue;
        public long LeaseRent;
        public long LeaseBuyback;
        /// <summary>Weeks of the lease left, weeks of rent missed in a row and whether it was extended (0 or 1).</summary>
        public int LeaseLeft;
        public int LeaseMissed;
        public int LeaseExtended;
        /// <summary>What is owed on the convertible note (with the interest) and what was borrowed.</summary>
        public long NoteOwed;
        public long NotePrincipal;
        /// <summary>Its weekly interest and discount in permille, the weeks to maturity, whether it was extended (0 or 1) and its tier.</summary>
        public int NoteRate;
        public int NoteDiscount;
        public int NoteLeft;
        public int NoteExtended;
        public int NoteTier;
        /// <summary>The device line (1 speaker, 2 glasses, 3 home robot, 0 none), its stage (1 designing, 2 on sale), the weeks left to design or on sale so far, and its quality.</summary>
        public int DevLine;
        public int DevStage;
        public int DevWeeks;
        public int DevQuality;
        /// <summary>The price tier (1 low, 2 middle, 3 high), the units sold in all and the weekly demand it started at.</summary>
        public int DevTier;
        public long DevSold;
        public long DevBase;
        /// <summary>The platform the lab has a deal with (1 phone, 2 browser, 3 office suite, 0 none), the weeks left of the term and the cut it takes, in permille.</summary>
        public int PlatPlat;
        public int PlatLeft;
        public int PlatCut;
        /// <summary>Renewals so far, how much of the lab's users come through the platform (permille), whether it has raised its price this term (0 or 1), and what the deal has paid the platform and left the lab, in all.</summary>
        public int PlatRenewals;
        public int PlatReliance;
        public int PlatHoldup;
        public long PlatPaid;
        public long PlatKept;
        /// <summary>The two values the lab states (1 safety, 2 speed, 3 openness, 4 people, 5 profit; 0 none), how much of them it lives up to (permille) and the band that puts it in (0 neither, 1 held up, 2 a joke).</summary>
        public int CreedA;
        public int CreedB;
        public int CreedIntegrity;
        public int CreedBand;
        /// <summary>Weeks before the creed can be changed, weeks before another town hall or owning up, and how many acts have kept it and broken it.</summary>
        public int CreedLock;
        public int CreedTown;
        public long CreedKept;
        public long CreedBroken;
        /// <summary>The shock waiting for an answer (1 squeeze, 2 backlash, 3 boom; 0 none), the world event behind it (its place in the data plus one) and the weeks left to answer.</summary>
        public int ShockKind;
        public int ShockRef;
        public int ShockLeft;
        /// <summary>How many shocks the lab braced for, rode out and countered.</summary>
        public int ShockBraced;
        public int ShockRode;
        public int ShockCountered;
        /// <summary>Papers waiting for the lab to decide about their code, the weeks the oldest has waited, the weeks left before another lab finishes trying to reproduce the one that went out (0 when none) and how reproducible it is.</summary>
        public int ReplPending;
        public int ReplWait;
        public int ReplLeft;
        public int ReplRigor;
        /// <summary>How many papers replicated and how many did not.</summary>
        public int ReplWins;
        public int ReplFails;
        /// <summary>The tech the lab has publicly bet on researching (its place in the data plus one, 0 none), the weeks left to do it, and how many bets it kept and lost.</summary>
        public int TbRef;
        public int TbLeft;
        public int TbWins;
        public int TbLosses;
        /// <summary>The model the lab has opened a public waitlist for (0 none), the hype built so far, the weeks left of the promise, and how many lists paid off and lapsed.</summary>
        public int WlModel;
        public int WlHype;
        public int WlLeft;
        public int WlWins;
        public int WlFlops;
        /// <summary>Whether the lab keeps a careers page, its score as an employer (0 to 1000), the weeks left for a bad review to be answered, whether the score is under the low bar, and how many reviews it has had.</summary>
        public int BrandOn;
        public int BrandScore;
        public int BrandReview;
        public int BrandLow;
        public int BrandReviews;
        /// <summary>The level of the free tier (0 none), its reach (0 to 1000), the weeks it has run, and the cash it has cost and brought in.</summary>
        public int FtLevel;
        public int FtReach;
        public int FtWeeks;
        public long FtSpent;
        public long FtEarned;
        /// <summary>Whether the lab keeps a launch watch desk, the rival (its id plus one) whose launch is waiting for an answer (0 none), how far ahead of the lab it is in permille, the weeks it has waited, and how many launches landed and were answered.</summary>
        public int LwOn;
        public int LwRival;
        public int LwLead;
        public int LwWait;
        public int LwHits;
        public int LwAnswers;
        /// <summary>The tech a research programme aims at (its place in the data plus one, 0 none), the stages done, the phase (1 a stage running, 2 a failed stage waiting, 3 the next approach waiting), the weeks left, whether the running stage is bold, the bold stages that worked, the points spent, and the programmes finished and given up.</summary>
        public int PgTech;
        public int PgDone;
        public int PgPhase;
        public int PgLeft;
        public int PgBold;
        public int PgInsight;
        public long PgSpent;
        public int PgWins;
        public int PgLosses;
        /// <summary>The people in the lab's project pod, the weeks they have been together, their chemistry (0 to 1000), the week a member may next be swapped, the research points the pod has added (or taken), and the pods formed.</summary>
        public List<int> PodIds = new List<int>();
        public int PodWeeks;
        public int PodChem;
        public int PodSwapAfter;
        public long PodPoints;
        public int PodsFormed;
        /// <summary>The regime the lab has filed under (1 to 3, 0 none), the phase (1 in review, 2 licensed), the weeks left of the review or the term, the strikes against the licence, the week before which it may not apply again, and how many licences it was granted, refused and had suspended.</summary>
        public int LicRegime;
        public int LicPhase;
        public int LicLeft;
        public int LicStrikes;
        public int LicCooldown;
        public int LicGranted;
        public int LicRefused;
        public int LicSuspended;
        /// <summary>How many support pledges the lab kept to their end, and how many it broke.</summary>
        public int LtsKept;
        public int LtsBroken;
        /// <summary>How many integrations the lab has moved to successor models, and how many it lost on the way or stranded.</summary>
        public int ApiMoved;
        public int ApiLost;
        /// <summary>How many conformity certificates the lab was granted, how many reviews ended in findings, and how many certificates were suspended.</summary>
        public int CfGranted;
        public int CfFindings;
        public int CfSuspended;
        /// <summary>The lab structure (0 none, 1 flat, 2 functional, 3 squads), the week it was set, the weeks of disruption left and how many times it was changed.</summary>
        public int OrgForm;
        public int OrgSince;
        public int OrgDisrupt;
        public int OrgReorgs;
        /// <summary>The fabric tier the lab has (0 none), the tier being built (0 none), the weeks left to build it, the GPUs the cost was worked out on, and how many tiers were delivered and cancelled.</summary>
        public int FabTier;
        public int FabBuild;
        public int FabLeft;
        public int FabGpus;
        public int FabBuilt;
        public int FabCancelled;
        /// <summary>The incident register: joined (1), reports waiting, weeks left on the oldest, weeks left preparing a full report, findings on file, reports filed, weeks to the next inspection, and the model of the latest incident.</summary>
        public int DiscOn;
        public int DiscOpen;
        public int DiscLeft;
        public int DiscPrep;
        public int DiscFindings;
        public int DiscFiled;
        public int DiscInspect;
        public int DiscModel;
        /// <summary>Raw units of data by source (1 scraped, 2 licensed, 3 synthetic, 4 users), clean units by source, the source and units and weeks left of the cleaning job, and counts of claims and of units licensed after the fact.</summary>
        public int DataRaw1;
        public int DataRaw2;
        public int DataRaw3;
        public int DataRaw4;
        public int DataClean1;
        public int DataClean2;
        public int DataClean3;
        public int DataClean4;
        public int DataJobKind;
        public int DataJobUnits;
        public int DataJobLeft;
        public int DataSuits;
        public int DataBackfilled;
        /// <summary>The users-data policy (0 none, 1 opt-in, 2 on by default), one more than the week it was set, units collected since the last complaint, units collected in all and complaints so far.</summary>
        public int UserPolicy;
        public int UserSince;
        public int UserBanked;
        public int UserTotal;
        public int UserComplaints;
        /// <summary>How the company is controlled (0 ordinary, 1 dual-class shares, 2 non-profit parent), one more than the week it was set, seated independent directors, weeks left on the search for the next, and how many have resigned.</summary>
        public int GovForm;
        public int GovSince;
        public int GovDirectors;
        public int GovSearch;
        public int GovResigned;
        /// <summary>Ids of the funding rounds the lab has bought back.</summary>
        public List<string> BoughtRounds = new List<string>();
        /// <summary>One more than the week the board last voted a buyback down (0 never), buybacks it passed, and buybacks it refused.</summary>
        public int BoardVoteWeek;
        public int BoardWon;
        public int BoardLost;
        /// <summary>Share of the rented GPUs on spot capacity (permille), one more than the week it was set, the rent saved so far, runs preempted, and compute they lost.</summary>
        public int SpotPermille;
        public int SpotSince;
        public long SpotSaved;
        public int SpotPreempts;
        public long SpotLost;
        /// <summary>Weeks left in the current reclaim wave, and how many waves the cloud has called.</summary>
        public int SpotOut;
        public int SpotWaves;
        /// <summary>Revenue the lab has earned from agent autonomy.</summary>
        public long AgentEarned;
        /// <summary>Vendor under contract (0 none, 1 volume, 2 licensed, 3 expert), weeks left, hidden exposure (permille), scandals so far, one more than the week of the last audit, and units cleaned with the vendor.</summary>
        public int AnnVendor;
        public int AnnLeft;
        public int AnnExposure;
        public int AnnStrikes;
        public int AnnAuditWeek;
        public int AnnCleaned;
        /// <summary>Board agenda: meetings held (0 or 1), tension (out of 1000), the motion on the table (0 none, 1 calm, 2 runway, 3 safety), its goal and the week it must be answered by; the commitment accepted, its goal and its deadline; the week of the next meeting; and the motions tabled, kept, broken and declined.</summary>
        public int MeetingsOn;
        public int BoardTension;
        public int MotionPend;
        public long MotionPendGoal;
        public int MotionAnswerBy;
        public int MotionKind;
        public long MotionGoal;
        public int MotionDue;
        public int MotionNext;
        public int MotionsTabled;
        public int MotionsKept;
        public int MotionsBroken;
        public int MotionsDeclined;
        /// <summary>Drill preparedness (out of 1000), weeks left of the current drill, the week the next drill is due, and how many drills passed and failed.</summary>
        public int DrillPrep;
        public int DrillLeft;
        public int DrillDue;
        public int DrillPassed;
        public int DrillFailed;
        /// <summary>Human escalation: backlog depth, weeks the desk is busy after a clear, and how many items were cleared.</summary>
        public int HumanQueue;
        public int HumanBusy;
        public int HumanCleared;
        /// <summary>Runaway playbook level, armed flag, weeks left armed, and no arm before this tick.</summary>
        public int PlayLevel;
        public int PlayArmed;
        public int PlayLeft;
        public int PlayArmUntil;
        /// <summary>Orchestration budget level and overrun accrual.</summary>
        public int OrchLevel;
        public int OrchOver;
        /// <summary>Weeks left on named data sources 0–3 (schema 136).</summary>
        public int SrcWeeks0;
        public int SrcWeeks1;
        public int SrcWeeks2;
        public int SrcWeeks3;
        /// <summary>Per-sector price tiers (schema 138).</summary>
        public List<string> PriceSectors = new List<string>();
        public List<int> PriceTiers = new List<int>();
        /// <summary>Week when the campaign clock stops (0 sandbox), schema 139.</summary>
        public int CampaignDeadlineTick;
        /// <summary>Law shape waiting for the next era (schema 140).</summary>
        public int LawShapePending;
        public int LawShapeDelta;
        public string LawShapeEra = "";
        /// <summary>Takeoff crisis cannot repeat before this tick (schema 142).</summary>
        public int TakeoffCooldownUntil;
        /// <summary>Scenario baggage id and picked flag (schema 143).</summary>
        public string BaggageId = "";
        public int BaggagePicked;
        public int MarginCallLeft;
        public int RungSixArcUntil;
        public int RungSixCooldownUntil;
        public string ScenarioId = "";
        public int ScenarioGoalResult;
        public string LawTarget = "";
        public int ReservedSignCooldownUntil;
        public int ScenarioEndlessAck;
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
            s.Int("covBreach", ref CovBreach);
            s.Int("covWaiverUntil", ref CovWaiverUntil);
            s.Int("covWaivers", ref CovWaivers);
            s.Int("covCalls", ref CovCalls);
            s.Int("capPolicy", ref CapPolicy);
            s.Long("capPool", ref CapPool);
            s.Int("capSwitchTick", ref CapSwitchTick);
            s.Long("capWritten", ref CapWritten);
            s.Int("listed", ref Listed);
            s.Int("listPrice", ref ListPrice);
            s.Int("listGuidance", ref ListGuidance);
            s.Long("listTarget", ref ListTarget);
            s.Long("listQuarter", ref ListQuarter);
            s.Long("listBase", ref ListBase);
            s.Int("listNext", ref ListNext);
            s.Int("listKeep", ref ListKeep);
            s.Int("listStreak", ref ListStreak);
            s.Int("listFollowOn", ref ListFollowOn);
            s.Int("listActivist", ref ListActivist);
            s.Long("rbfAdvance", ref RbfAdvance);
            s.Long("rbfOwed", ref RbfOwed);
            s.Long("rbfBase", ref RbfBase);
            s.Int("rbfShare", ref RbfShare);
            s.Int("rbfTier", ref RbfTier);
            s.Int("rbfWeeks", ref RbfWeeks);
            s.Int("rbfSlump", ref RbfSlump);
            s.Long("foundFund", ref FoundFund);
            s.Long("foundGiven", ref FoundGiven);
            s.Int("foundFocus", ref FoundFocus);
            s.Int("foundPayout", ref FoundPayout);
            s.Int("foundGoodwill", ref FoundGoodwill);
            s.Int("foundSwitch", ref FoundSwitch);
            s.Int("foundDrip", ref FoundDrip);
            s.Int("leaseUnits", ref LeaseUnits);
            s.Long("leaseValue", ref LeaseValue);
            s.Long("leaseRent", ref LeaseRent);
            s.Long("leaseBuyback", ref LeaseBuyback);
            s.Int("leaseLeft", ref LeaseLeft);
            s.Int("leaseMissed", ref LeaseMissed);
            s.Int("leaseExtended", ref LeaseExtended);
            s.Long("noteOwed", ref NoteOwed);
            s.Long("notePrincipal", ref NotePrincipal);
            s.Int("noteRate", ref NoteRate);
            s.Int("noteDiscount", ref NoteDiscount);
            s.Int("noteLeft", ref NoteLeft);
            s.Int("noteExtended", ref NoteExtended);
            s.Int("noteTier", ref NoteTier);
            s.Int("devLine", ref DevLine);
            s.Int("devStage", ref DevStage);
            s.Int("devWeeks", ref DevWeeks);
            s.Int("devQuality", ref DevQuality);
            s.Int("devTier", ref DevTier);
            s.Long("devSold", ref DevSold);
            s.Long("devBase", ref DevBase);
            s.Int("platPlat", ref PlatPlat);
            s.Int("platLeft", ref PlatLeft);
            s.Int("platCut", ref PlatCut);
            s.Int("platRenewals", ref PlatRenewals);
            s.Int("platReliance", ref PlatReliance);
            s.Int("platHoldup", ref PlatHoldup);
            s.Long("platPaid", ref PlatPaid);
            s.Long("platKept", ref PlatKept);
            s.Int("creedA", ref CreedA);
            s.Int("creedB", ref CreedB);
            s.Int("creedIntegrity", ref CreedIntegrity);
            s.Int("creedBand", ref CreedBand);
            s.Int("creedLock", ref CreedLock);
            s.Int("creedTown", ref CreedTown);
            s.Long("creedKept", ref CreedKept);
            s.Long("creedBroken", ref CreedBroken);
            s.Int("shockKind", ref ShockKind);
            s.Int("shockRef", ref ShockRef);
            s.Int("shockLeft", ref ShockLeft);
            s.Int("shockBraced", ref ShockBraced);
            s.Int("shockRode", ref ShockRode);
            s.Int("shockCountered", ref ShockCountered);
            s.Int("replPending", ref ReplPending);
            s.Int("replWait", ref ReplWait);
            s.Int("replLeft", ref ReplLeft);
            s.Int("replRigor", ref ReplRigor);
            s.Int("replWins", ref ReplWins);
            s.Int("replFails", ref ReplFails);
            s.Int("tbRef", ref TbRef);
            s.Int("tbLeft", ref TbLeft);
            s.Int("tbWins", ref TbWins);
            s.Int("tbLosses", ref TbLosses);
            s.Int("wlModel", ref WlModel);
            s.Int("wlHype", ref WlHype);
            s.Int("wlLeft", ref WlLeft);
            s.Int("wlWins", ref WlWins);
            s.Int("wlFlops", ref WlFlops);
            s.Int("brandOn", ref BrandOn);
            s.Int("brandScore", ref BrandScore);
            s.Int("brandReview", ref BrandReview);
            s.Int("brandLow", ref BrandLow);
            s.Int("brandReviews", ref BrandReviews);
            s.Int("ftLevel", ref FtLevel);
            s.Int("ftReach", ref FtReach);
            s.Int("ftWeeks", ref FtWeeks);
            s.Long("ftSpent", ref FtSpent);
            s.Long("ftEarned", ref FtEarned);
            s.Int("lwOn", ref LwOn);
            s.Int("lwRival", ref LwRival);
            s.Int("lwLead", ref LwLead);
            s.Int("lwWait", ref LwWait);
            s.Int("lwHits", ref LwHits);
            s.Int("lwAnswers", ref LwAnswers);
            s.Int("pgTech", ref PgTech);
            s.Int("pgDone", ref PgDone);
            s.Int("pgPhase", ref PgPhase);
            s.Int("pgLeft", ref PgLeft);
            s.Int("pgBold", ref PgBold);
            s.Int("pgInsight", ref PgInsight);
            s.Long("pgSpent", ref PgSpent);
            s.Int("pgWins", ref PgWins);
            s.Int("pgLosses", ref PgLosses);
            s.IntList("podIds", ref PodIds);
            s.Int("podWeeks", ref PodWeeks);
            s.Int("podChem", ref PodChem);
            s.Int("podSwapAfter", ref PodSwapAfter);
            s.Long("podPoints", ref PodPoints);
            s.Int("podsFormed", ref PodsFormed);
            s.Int("licRegime", ref LicRegime);
            s.Int("licPhase", ref LicPhase);
            s.Int("licLeft", ref LicLeft);
            s.Int("licStrikes", ref LicStrikes);
            s.Int("licCooldown", ref LicCooldown);
            s.Int("licGranted", ref LicGranted);
            s.Int("licRefused", ref LicRefused);
            s.Int("licSuspended", ref LicSuspended);
            s.Int("ltsKept", ref LtsKept);
            s.Int("ltsBroken", ref LtsBroken);
            s.Int("apiMoved", ref ApiMoved);
            s.Int("apiLost", ref ApiLost);
            s.Int("cfGranted", ref CfGranted);
            s.Int("cfFindings", ref CfFindings);
            s.Int("cfSuspended", ref CfSuspended);
            s.Int("orgForm", ref OrgForm);
            s.Int("orgSince", ref OrgSince);
            s.Int("orgDisrupt", ref OrgDisrupt);
            s.Int("orgReorgs", ref OrgReorgs);
            s.Int("fabTier", ref FabTier);
            s.Int("fabBuild", ref FabBuild);
            s.Int("fabLeft", ref FabLeft);
            s.Int("fabGpus", ref FabGpus);
            s.Int("fabBuilt", ref FabBuilt);
            s.Int("fabCancelled", ref FabCancelled);
            s.Int("discOn", ref DiscOn);
            s.Int("discOpen", ref DiscOpen);
            s.Int("discLeft", ref DiscLeft);
            s.Int("discPrep", ref DiscPrep);
            s.Int("discFindings", ref DiscFindings);
            s.Int("discFiled", ref DiscFiled);
            s.Int("discInspect", ref DiscInspect);
            s.Int("discModel", ref DiscModel);
            s.Int("dataRaw1", ref DataRaw1);
            s.Int("dataRaw2", ref DataRaw2);
            s.Int("dataRaw3", ref DataRaw3);
            s.Int("dataRaw4", ref DataRaw4);
            s.Int("dataClean1", ref DataClean1);
            s.Int("dataClean2", ref DataClean2);
            s.Int("dataClean3", ref DataClean3);
            s.Int("dataClean4", ref DataClean4);
            s.Int("dataJobKind", ref DataJobKind);
            s.Int("dataJobUnits", ref DataJobUnits);
            s.Int("dataJobLeft", ref DataJobLeft);
            s.Int("dataSuits", ref DataSuits);
            s.Int("dataBackfilled", ref DataBackfilled);
            s.Int("userPolicy", ref UserPolicy);
            s.Int("userSince", ref UserSince);
            s.Int("userBanked", ref UserBanked);
            s.Int("userTotal", ref UserTotal);
            s.Int("userComplaints", ref UserComplaints);
            s.Int("govForm", ref GovForm);
            s.Int("govSince", ref GovSince);
            s.Int("govDirectors", ref GovDirectors);
            s.Int("govSearch", ref GovSearch);
            s.Int("govResigned", ref GovResigned);
            s.StrList("boughtRounds", ref BoughtRounds);
            s.Int("boardVoteWeek", ref BoardVoteWeek);
            s.Int("boardWon", ref BoardWon);
            s.Int("boardLost", ref BoardLost);
            s.Int("spotPermille", ref SpotPermille);
            s.Int("spotSince", ref SpotSince);
            s.Long("spotSaved", ref SpotSaved);
            s.Int("spotPreempts", ref SpotPreempts);
            s.Long("spotLost", ref SpotLost);
            s.Int("spotOut", ref SpotOut);
            s.Int("spotWaves", ref SpotWaves);
            s.Long("agentEarned", ref AgentEarned);
            s.Int("annVendor", ref AnnVendor);
            s.Int("annLeft", ref AnnLeft);
            s.Int("annExposure", ref AnnExposure);
            s.Int("annStrikes", ref AnnStrikes);
            s.Int("annAuditWeek", ref AnnAuditWeek);
            s.Int("annCleaned", ref AnnCleaned);
            s.Int("meetingsOn", ref MeetingsOn);
            s.Int("boardTension", ref BoardTension);
            s.Int("motionPend", ref MotionPend);
            s.Long("motionPendGoal", ref MotionPendGoal);
            s.Int("motionAnswerBy", ref MotionAnswerBy);
            s.Int("motionKind", ref MotionKind);
            s.Long("motionGoal", ref MotionGoal);
            s.Int("motionDue", ref MotionDue);
            s.Int("motionNext", ref MotionNext);
            s.Int("motionsTabled", ref MotionsTabled);
            s.Int("motionsKept", ref MotionsKept);
            s.Int("motionsBroken", ref MotionsBroken);
            s.Int("motionsDeclined", ref MotionsDeclined);
            s.Int("drillPrep", ref DrillPrep);
            s.Int("drillLeft", ref DrillLeft);
            s.Int("drillDue", ref DrillDue);
            s.Int("drillPassed", ref DrillPassed);
            s.Int("drillFailed", ref DrillFailed);
            s.Int("humanQueue", ref HumanQueue);
            s.Int("humanBusy", ref HumanBusy);
            s.Int("humanCleared", ref HumanCleared);
            s.Int("playLevel", ref PlayLevel);
            s.Int("playArmed", ref PlayArmed);
            s.Int("playLeft", ref PlayLeft);
            s.Int("playArmUntil", ref PlayArmUntil);
            s.Int("orchLevel", ref OrchLevel);
            s.Int("orchOver", ref OrchOver);
            s.Int("srcWeeks0", ref SrcWeeks0);
            s.Int("srcWeeks1", ref SrcWeeks1);
            s.Int("srcWeeks2", ref SrcWeeks2);
            s.Int("srcWeeks3", ref SrcWeeks3);
            s.StrList("priceSectors", ref PriceSectors);
            s.IntList("priceTiers", ref PriceTiers);
            s.Int("campaignDeadlineTick", ref CampaignDeadlineTick);
            s.Int("lawShapePending", ref LawShapePending);
            s.Int("lawShapeDelta", ref LawShapeDelta);
            s.Str("lawShapeEra", ref LawShapeEra);
            s.Int("takeoffCooldownUntil", ref TakeoffCooldownUntil);
            s.Str("baggageId", ref BaggageId);
            s.Int("baggagePicked", ref BaggagePicked);
            s.Int("marginCallLeft", ref MarginCallLeft);
            s.Int("rungSixArcUntil", ref RungSixArcUntil);
            s.Int("rungSixCooldownUntil", ref RungSixCooldownUntil);
            s.Str("scenarioId", ref ScenarioId);
            s.Int("scenarioGoalResult", ref ScenarioGoalResult);
            s.Str("lawTarget", ref LawTarget);
            s.Int("reservedSignCooldownUntil", ref ReservedSignCooldownUntil);
            s.Int("scenarioEndlessAck", ref ScenarioEndlessAck);
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
        /// <summary>The priority of the model's training run: 0 not set (a normal run), 1 background, 2 normal, 3 rush.</summary>
        public int QueueLevel;
        /// <summary>Weeks left of the model's long-term support pledge, or 0 when it has none.</summary>
        public int LtsLeft;
        /// <summary>Customer integrations built on the model, its API state (0 none, 1 open, 2 deprecating), weeks to the deprecation deadline and the successor model id.</summary>
        public int Integrations;
        public int ApiState;
        public int ApiLeft;
        public int ApiSuccessor;
        /// <summary>Conformity assessment: the stage (0 none, 1 dossier, 2 evidence, 3 review, 4 certified), the weeks left in it, the scope (1 narrow, 2 broad) and how many reviews found problems.</summary>
        public int CertStage;
        public int CertLeft;
        public int CertScope;
        public int CertTries;
        public int AuditedUntil;
        /// <summary>The week the model's end-of-life notice was given, 0 when it has none.</summary>
        public int SunsetStart;
        /// <summary>The week the model leaves the market, 0 when it has no notice.</summary>
        public int SunsetTick;
        /// <summary>Release checklist: done bits (1 red-team, 2 docs, 4 eval, 8 policy) and missed bits frozen at first release.</summary>
        public int ChkDone;
        public int ChkMissed;
        /// <summary>Tool allow-list for agents as a bitmask: 1 web, 2 code, 4 data.</summary>
        public int ToolMask;
        /// <summary>Public eval: suite (0 none, 1 narrow, 2 broad), weeks until the next publish, last score (out of 1000), how many publishes ran, and consecutive beats of the bar.</summary>
        public int EvalSuite;
        public int EvalLeft;
        public int EvalScore;
        public int EvalRuns;
        public int EvalStreak;
        /// <summary>Knowledge: the staff id of the person who owns the model (0 none), the level of its documentation, weeks since the documentation was last written, the knowledge gap (out of 1000) and how many owners have left it.</summary>
        public int OwnerId;
        public int DocLevel;
        public int DocAge;
        public int KnowGap;
        public int Handovers;
        /// <summary>Post-training: the kind of run going on (0 none, 1 preference tuning, 2 safety tuning), weeks left of it, and how many of each kind the model has had.</summary>
        public int PostKind;
        public int PostLeft;
        public int PostPref;
        public int PostSafety;
        /// <summary>Autonomy level of the model agents (0 none, 1 supervised, 2 delegated, 3 autonomous), one more than the week it was set, weeks since the last blow-up, and blow-ups.</summary>
        public int AgentLevel;
        public int AgentSince;
        public int AgentStreak;
        public int AgentBlowups;
        /// <summary>Agent sandbox weeks left and graduation (0 none, 1 passed).</summary>
        public int SandLeft;
        public int SandGrad;
        /// <summary>Traffic route share in ppm across released models.</summary>
        public int RoutePpm;
        /// <summary>Public metrics on (0/1), last score (out of 1000), and incident misses while published.</summary>
        public int PubOn;
        public int PubScore;
        public int PubMiss;
        /// <summary>Specialist kind index + 1 (schema 135), open weights 0/1 (137).</summary>
        public int SpecKind;
        public int WgtOpen;
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
            s.Int("certStage", ref CertStage);
            s.Int("certLeft", ref CertLeft);
            s.Int("certScope", ref CertScope);
            s.Int("certTries", ref CertTries);
            s.Int("integrations", ref Integrations);
            s.Int("apiState", ref ApiState);
            s.Int("apiLeft", ref ApiLeft);
            s.Int("apiSuccessor", ref ApiSuccessor);
            s.Int("ltsLeft", ref LtsLeft);
            s.Int("queueLevel", ref QueueLevel);
            s.Int("sunsetStart", ref SunsetStart);
            s.Int("sunsetTick", ref SunsetTick);
            s.Int("chkDone", ref ChkDone);
            s.Int("chkMissed", ref ChkMissed);
            s.Int("toolMask", ref ToolMask);
            s.Int("evalSuite", ref EvalSuite);
            s.Int("evalLeft", ref EvalLeft);
            s.Int("evalScore", ref EvalScore);
            s.Int("evalRuns", ref EvalRuns);
            s.Int("evalStreak", ref EvalStreak);
            s.Int("ownerId", ref OwnerId);
            s.Int("docLevel", ref DocLevel);
            s.Int("docAge", ref DocAge);
            s.Int("knowGap", ref KnowGap);
            s.Int("handovers", ref Handovers);
            s.Int("postKind", ref PostKind);
            s.Int("postLeft", ref PostLeft);
            s.Int("postPref", ref PostPref);
            s.Int("postSafety", ref PostSafety);
            s.Int("agentLevel", ref AgentLevel);
            s.Int("agentSince", ref AgentSince);
            s.Int("agentStreak", ref AgentStreak);
            s.Int("agentBlowups", ref AgentBlowups);
            s.Int("sandLeft", ref SandLeft);
            s.Int("sandGrad", ref SandGrad);
            s.Int("routePpm", ref RoutePpm);
            s.Int("pubOn", ref PubOn);
            s.Int("pubScore", ref PubScore);
            s.Int("pubMiss", ref PubMiss);
            s.Int("specKind", ref SpecKind);
            s.Int("wgtOpen", ref WgtOpen);
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
