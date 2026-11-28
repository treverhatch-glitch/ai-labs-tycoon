using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    // Every tunable number of the game lives in these classes and is loaded from Rules/Data/default-config.json.
    // Units: money in whole dollars, rates in permille (1000 = 100%), time in weeks (one tick is one week).

    public enum RoleFunction { Research, Engineering, Safety, Sales, Infrastructure, Policy, ChipDesign }

    public enum RoomPurpose { Research, Engineering, Safety, Sales, Compute, Infrastructure, Policy, ChipDesign, HardwareTest, Press, Comfort }

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
        CoolingPermille,
        /// <summary>Lets the lab design chips for this target. <see cref="TechEffectConfig.Id"/> names the chip target.</summary>
        UnlockChipTarget,
        /// <summary>Adds to the maturity of the lab's chip software, in permille, on top of what its architects build.</summary>
        SoftwarePermille
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
        /// <summary>What roles do beyond their function (the recruiter's hiring discount and the rest).</summary>
        public List<RolePerkConfig> RolePerks = new List<RolePerkConfig>();
        public List<RoomTypeConfig> RoomTypes;
        /// <summary>What rooms do beyond seating people or holding GPUs.</summary>
        public List<RoomPerkConfig> RoomPerks = new List<RoomPerkConfig>();
        /// <summary>Who the player can be at the start: a lasting edge and cost each.</summary>
        public List<FounderConfig> Founders = new List<FounderConfig>();
        /// <summary>How the lab can be good at the start: appeal in a few sectors against the rest.</summary>
        public List<StyleConfig> Styles = new List<StyleConfig>();
        public ComputeConfig Compute;
        public WorldConfig World;
        public ResearchConfig Research;
        public TrainingConfig Training;
        public SafetyConfig Safety;
        public MarketConfig Market;
        public FinanceConfig Finance;
        public FundingConfig Funding;
        public BubbleConfig Bubble;
        public SharesConfig Shares;
        public RegulatorsConfig Regulators;
        public StaffPeopleConfig StaffPeople;
        /// <summary>The talent market: offers, counters and poaching back.</summary>
        public TalentConfig Talent = new TalentConfig();
        /// <summary>The press: journalists, beats, answers and leaks.</summary>
        public PressConfig Press = new PressConfig();
        public TrustConfig Trust = new TrustConfig();
        public CultureConfig Culture = new CultureConfig();
        public NamingConfig Naming = new NamingConfig();
        public AgeingConfig Ageing = new AgeingConfig();
        public ControlConfig Control = new ControlConfig();
        public LeadsConfig Leads = new LeadsConfig();
        public RepairConfig Repair = new RepairConfig();
        public RedTeamConfig RedTeam = new RedTeamConfig();
        public InterpConfig Interp = new InterpConfig();
        public ComplianceConfig Compliance = new ComplianceConfig();
        public PrestigeConfig Prestige = new PrestigeConfig();
        public SubletConfig Sublet = new SubletConfig();
        public ResearchFocusConfig Focus = new ResearchFocusConfig();
        public HackWeekConfig Hack = new HackWeekConfig();
        public EquityConfig Equity = new EquityConfig();
        public ChiefsConfig Chiefs = new ChiefsConfig();
        public EthicsBoardConfig Board = new EthicsBoardConfig();
        public OptimisationConfig Optimise = new OptimisationConfig();
        public MarketingConfig Marketing = new MarketingConfig();
        public OnCallConfig OnCall = new OnCallConfig();
        public ConsultantsConfig Consultants = new ConsultantsConfig();
        public PostMortemConfig PostMortem = new PostMortemConfig();
        public SlaConfig Sla = new SlaConfig();
        public ResearchGrantsConfig ResearchGrants = new ResearchGrantsConfig();
        public AbandonConfig Abandon = new AbandonConfig();
        public LayoffsConfig Layoffs = new LayoffsConfig();
        public TechDebtConfig TechDebt = new TechDebtConfig();
        public SeniorityConfig Seniority = new SeniorityConfig();
        public RevenueTargetsConfig RevenueTargets = new RevenueTargetsConfig();
        public ChairsConfig Chairs = new ChairsConfig();
        public WhistleConfig Whistle = new WhistleConfig();
        public BugBountyConfig BugBounty = new BugBountyConfig();
        public TreasuryConfig Treasury = new TreasuryConfig();
        public PledgeConfig Pledge = new PledgeConfig();
        public RoadmapConfig Roadmap = new RoadmapConfig();
        public MentoringConfig Mentoring = new MentoringConfig();
        public SkunkworksConfig Skunkworks = new SkunkworksConfig();
        public LabourConfig Labour = new LabourConfig();
        public SunsetConfig Sunset = new SunsetConfig();
        public AccountsConfig Accounts = new AccountsConfig();
        public ConferenceConfig Conference = new ConferenceConfig();
        public AlumniConfig Alumni = new AlumniConfig();
        public InternshipsConfig Internships = new InternshipsConfig();
        public SupportConfig Support = new SupportConfig();
        public SabbaticalConfig Sabbatical = new SabbaticalConfig();
        public RequisitionsConfig Requisitions = new RequisitionsConfig();
        public ScholarsConfig Scholars = new ScholarsConfig();
        public LitigationConfig Litigation = new LitigationConfig();
        public SecurityConfig Security = new SecurityConfig();
        public GovContractsConfig GovContracts = new GovContractsConfig();
        public CharterConfig Charter = new CharterConfig();
        public ScalingConfig Scaling = new ScalingConfig();
        public TaxConfig Tax = new TaxConfig();
        public StandardsConfig Standards = new StandardsConfig();
        public ReliabilityConfig Reliability = new ReliabilityConfig();
        public CovenantsConfig Covenants = new CovenantsConfig();
        public AllowancesConfig Allowances = new AllowancesConfig();
        public ListingConfig Listing = new ListingConfig();
        public RbfConfig Rbf = new RbfConfig();
        public FoundationConfig Foundation = new FoundationConfig();
        public LeasebackConfig Leaseback = new LeasebackConfig();
        public NotesConfig ConvertibleNotes = new NotesConfig();
        public DevicesConfig Devices = new DevicesConfig();
        public PlatformsConfig Platforms = new PlatformsConfig();
        public CreedConfig Creed = new CreedConfig();
        public ShocksConfig Shocks = new ShocksConfig();
        public ReplicationConfig Replication = new ReplicationConfig();
        public TechBetsConfig TechBets = new TechBetsConfig();
        public WaitlistConfig Waitlist = new WaitlistConfig();
        public BrandConfig Brand = new BrandConfig();
        public FreeTierConfig FreeTier = new FreeTierConfig();
        public LaunchWatchConfig LaunchWatch = new LaunchWatchConfig();
        public ProgrammesConfig Programmes = new ProgrammesConfig();
        public RunQueueConfig RunQueue = new RunQueueConfig();
        public PodsConfig Pods = new PodsConfig();
        public LicenceConfig Licence = new LicenceConfig();
        public PledgesConfig Pledges = new PledgesConfig();
        public IntegrationsConfig Integrations = new IntegrationsConfig();
        public ConformityConfig Conformity = new ConformityConfig();
        public OrgConfig Org = new OrgConfig();
        public FabricConfig Fabric = new FabricConfig();
        public DisclosureConfig Disclosure = new DisclosureConfig();
        public DatasetsConfig Datasets = new DatasetsConfig();
        public UserDataConfig UserData = new UserDataConfig();
        public GovernanceConfig Governance = new GovernanceConfig();
        public BuybackConfig Buyback = new BuybackConfig();
        public SpotConfig Spot = new SpotConfig();
        public AgencyConfig Agency = new AgencyConfig();
        public AnnotationConfig Annotation = new AnnotationConfig();
        public PostTrainingConfig PostTraining = new PostTrainingConfig();
        public KnowledgeConfig Knowledge = new KnowledgeConfig();
        public BoardAgendaConfig BoardAgenda = new BoardAgendaConfig();
        public DrillsConfig Drills = new DrillsConfig();
        public EvalHarnessConfig EvalHarness = new EvalHarnessConfig();
        public ToolPolicyConfig ToolPolicy = new ToolPolicyConfig();
        public HumanQueueConfig HumanQueue = new HumanQueueConfig();
        public ReleaseChecklistConfig ReleaseChecklist = new ReleaseChecklistConfig();
        public AgentSandboxConfig AgentSandbox = new AgentSandboxConfig();
        public RunawayPlaybookConfig RunawayPlaybook = new RunawayPlaybookConfig();
        public OrchestrationBudgetConfig OrchestrationBudget = new OrchestrationBudgetConfig();
        public TrafficRouterConfig TrafficRouter = new TrafficRouterConfig();
        public PublicMetricsConfig PublicMetrics = new PublicMetricsConfig();
        public SpecialistLinesConfig SpecialistLines = new SpecialistLinesConfig();
        public DataSourcesConfig DataSources = new DataSourcesConfig();
        public OpenWeightsConfig OpenWeights = new OpenWeightsConfig();
        public SectorPricingConfig SectorPricing = new SectorPricingConfig();
        public CampaignDeadlineConfig CampaignDeadline = new CampaignDeadlineConfig();
        public LawShapingConfig LawShaping = new LawShapingConfig();
        public TakeoffCrisisConfig TakeoffCrisis = new TakeoffCrisisConfig();
        public ScenarioBaggageConfig ScenarioBaggage = new ScenarioBaggageConfig();
        public MarginCallConfig MarginCall = new MarginCallConfig();
        public HearingSharePriceConfig HearingSharePrice = new HearingSharePriceConfig();
        public RungSixCrisisConfig RungSixCrisis = new RungSixCrisisConfig();
        public ScenarioGoalsConfig ScenarioGoals = new ScenarioGoalsConfig();
        public LawTargetConfig LawTarget = new LawTargetConfig();
        public ReservedComputeGateConfig ReservedComputeGate = new ReservedComputeGateConfig();
        public RivalIntentNewsConfig RivalIntentNews = new RivalIntentNewsConfig();
        public GpuTierUnlockConfig GpuTierUnlock = new GpuTierUnlockConfig();
        public EndlessScenarioConfig EndlessScenario = new EndlessScenarioConfig();
        public ShadyFreezeAuditConfig ShadyFreezeAudit = new ShadyFreezeAuditConfig();
        public SpecialistEraHooksConfig SpecialistEraHooks = new SpecialistEraHooksConfig();
        public SectorPricingBreakdownConfig SectorPricingBreakdown = new SectorPricingBreakdownConfig();
        public DataSourceSpecialistPairingConfig DataSourceSpecialistPairing = new DataSourceSpecialistPairingConfig();
        public PublicMetricsEraLoadConfig PublicMetricsEraLoad = new PublicMetricsEraLoadConfig();
        public OpenWeightsEraThrottleConfig OpenWeightsEraThrottle = new OpenWeightsEraThrottleConfig();
        public EvalHarnessEraBoostConfig EvalHarnessEraBoost = new EvalHarnessEraBoostConfig();
        public CampaignDeadlineEraGraceConfig CampaignDeadlineEraGrace = new CampaignDeadlineEraGraceConfig();
        public LawShapingEraCooldownConfig LawShapingEraCooldown = new LawShapingEraCooldownConfig();
        public DataSourceRenewEraBonusConfig DataSourceRenewEraBonus = new DataSourceRenewEraBonusConfig();
        public SectorPricingEraReliefConfig SectorPricingEraRelief = new SectorPricingEraReliefConfig();
        public LawTargetEraAmplifyConfig LawTargetEraAmplify = new LawTargetEraAmplifyConfig();
        public SpecialistBranchEraDiscountConfig SpecialistBranchEraDiscount = new SpecialistBranchEraDiscountConfig();
        public MarginCallEraGraceConfig MarginCallEraGrace = new MarginCallEraGraceConfig();
        public HearingSharePriceEraBoostConfig HearingSharePriceEraBoost = new HearingSharePriceEraBoostConfig();
        public RungSixEraCooldownConfig RungSixEraCooldown = new RungSixEraCooldownConfig();
        public DataSourceAcquireEraDiscountConfig DataSourceAcquireEraDiscount = new DataSourceAcquireEraDiscountConfig();
        public MarginSatisfyEraReliefConfig MarginSatisfyEraRelief = new MarginSatisfyEraReliefConfig();
        public ScenarioGoalsEraSlackConfig ScenarioGoalsEraSlack = new ScenarioGoalsEraSlackConfig();
        public TakeoffCrisisEraGraceConfig TakeoffCrisisEraGrace = new TakeoffCrisisEraGraceConfig();
        public ComputeEquipmentConfig ComputeEquipment = new ComputeEquipmentConfig();
        public ExitDebtEraHaircutConfig ExitDebtEraHaircut = new ExitDebtEraHaircutConfig();
        public TakeoffAnswerEraCooldownConfig TakeoffAnswerEraCooldown = new TakeoffAnswerEraCooldownConfig();
        public ScenarioGoalEraTrustConfig ScenarioGoalEraTrust = new ScenarioGoalEraTrustConfig();
        public HearingShareEraDampenConfig HearingShareEraDampen = new HearingShareEraDampenConfig();
        public OfficeReleaseEraGraceConfig OfficeReleaseEraGrace = new OfficeReleaseEraGraceConfig();
        public DataSourceAcquireEraWeeksConfig DataSourceAcquireEraWeeks = new DataSourceAcquireEraWeeksConfig();
        public EscapeCrisisEraGraceConfig EscapeCrisisEraGrace = new EscapeCrisisEraGraceConfig();
        public LawShapingEraAmplifyConfig LawShapingEraAmplify = new LawShapingEraAmplifyConfig();
        public DataSourceRenewEraDiscountConfig DataSourceRenewEraDiscount = new DataSourceRenewEraDiscountConfig();
        public RungSixEraGraceConfig RungSixEraGrace = new RungSixEraGraceConfig();
        public EvalHarnessEraReliefConfig EvalHarnessEraRelief = new EvalHarnessEraReliefConfig();
        public PublicMetricsEraDiscountConfig PublicMetricsEraDiscount = new PublicMetricsEraDiscountConfig();
        public OfficeHearingEraGraceConfig OfficeHearingEraGrace = new OfficeHearingEraGraceConfig();
        public OpenWeightsCopyEraReliefConfig OpenWeightsCopyEraRelief = new OpenWeightsCopyEraReliefConfig();
        public EvalFailEraReliefConfig EvalFailEraRelief = new EvalFailEraReliefConfig();
        public LobbyingEraCooldownConfig LobbyingEraCooldown = new LobbyingEraCooldownConfig();
        public PublicMetricsMissEraReliefConfig PublicMetricsMissEraRelief = new PublicMetricsMissEraReliefConfig();
        public OpenWeightsOpenEraBoostConfig OpenWeightsOpenEraBoost = new OpenWeightsOpenEraBoostConfig();
        public LobbyingCostEraReliefConfig LobbyingCostEraRelief = new LobbyingCostEraReliefConfig();
        public PublicMetricsConformityEraSlackConfig PublicMetricsConformityEraSlack = new PublicMetricsConformityEraSlackConfig();
        public SpecialistBranchEraBoostConfig SpecialistBranchEraBoost = new SpecialistBranchEraBoostConfig();
        public PulloutConfig Pullout = new PulloutConfig();
        public RestructureConfig Restructure = new RestructureConfig();
        public IncidentFloorConfig IncidentFloor = new IncidentFloorConfig();
        public RetrainConfig Retrain = new RetrainConfig();
        public SafetyAuditConfig Audit = new SafetyAuditConfig();
        public OfficeConfig Office = new OfficeConfig();
        public ObjectivesConfig Objectives = new ObjectivesConfig();
        public SuccessorConfig Successor = new SuccessorConfig();
        public RushConfig Rush = new RushConfig();
        public LobbyConfig Lobbying = new LobbyConfig();
        public LoansConfig Loans = new LoansConfig();
        public ScenariosConfig Scenarios = new ScenariosConfig();
        public PivotConfig Pivot = new PivotConfig();
        public CampaignConfig Campaign = new CampaignConfig();
        public RunsConfig Runs = new RunsConfig();
        public NewsConfig News = new NewsConfig();
        public ChatConfig Chat = new ChatConfig();
        public GlowConfig Glow = new GlowConfig();
        public GauntletConfig Gauntlet;
        public RivalryConfig Rivalry;
        public VendorConfig Vendor;
        public ChipsConfig Chips;
        public EndingsConfig Endings;
        public ShadyConfig Shady;
        public AcquisitionConfig Acquisitions;
        public ModelSalesConfig ModelSales;
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
            s.ObjList("rolePerks", ref RolePerks);
            s.ObjList("roomTypes", ref RoomTypes);
            s.ObjList("roomPerks", ref RoomPerks);
            s.ObjList("founders", ref Founders);
            s.ObjList("styles", ref Styles);
            s.Obj("compute", ref Compute);
            s.Obj("world", ref World);
            s.Obj("research", ref Research);
            s.Obj("training", ref Training);
            s.Obj("safety", ref Safety);
            s.Obj("market", ref Market);
            s.Obj("finance", ref Finance);
            s.Obj("funding", ref Funding);
            s.Obj("bubble", ref Bubble);
            s.Obj("shares", ref Shares);
            s.Obj("regulators", ref Regulators);
            s.Obj("staffPeople", ref StaffPeople);
            s.Obj("talent", ref Talent);
            s.Obj("press", ref Press);
            s.Obj("trust", ref Trust);
            s.Obj("culture", ref Culture);
            s.Obj("naming", ref Naming);
            s.Obj("ageing", ref Ageing);
            s.Obj("control", ref Control);
            s.Obj("leads", ref Leads);
            s.Obj("repair", ref Repair);
            s.Obj("redTeam", ref RedTeam);
            s.Obj("interp", ref Interp);
            s.Obj("compliance", ref Compliance);
            s.Obj("prestige", ref Prestige);
            s.Obj("sublet", ref Sublet);
            s.Obj("focus", ref Focus);
            s.Obj("hack", ref Hack);
            s.Obj("equity", ref Equity);
            s.Obj("chiefs", ref Chiefs);
            s.Obj("ethicsBoard", ref Board);
            s.Obj("optimise", ref Optimise);
            s.Obj("marketing", ref Marketing);
            s.Obj("onCall", ref OnCall);
            s.Obj("consultants", ref Consultants);
            s.Obj("postMortem", ref PostMortem);
            s.Obj("sla", ref Sla);
            s.Obj("researchGrants", ref ResearchGrants);
            s.Obj("abandon", ref Abandon);
            s.Obj("layoffs", ref Layoffs);
            s.Obj("techDebt", ref TechDebt);
            s.Obj("seniority", ref Seniority);
            s.Obj("revenueTargets", ref RevenueTargets);
            s.Obj("chairs", ref Chairs);
            s.Obj("whistle", ref Whistle);
            s.Obj("bugBounty", ref BugBounty);
            s.Obj("treasury", ref Treasury);
            s.Obj("pledge", ref Pledge);
            s.Obj("roadmap", ref Roadmap);
            s.Obj("mentoring", ref Mentoring);
            s.Obj("skunkworks", ref Skunkworks);
            s.Obj("labour", ref Labour);
            s.Obj("sunset", ref Sunset);
            s.Obj("accounts", ref Accounts);
            s.Obj("conference", ref Conference);
            s.Obj("alumni", ref Alumni);
            s.Obj("internships", ref Internships);
            s.Obj("support", ref Support);
            s.Obj("sabbatical", ref Sabbatical);
            s.Obj("requisitions", ref Requisitions);
            s.Obj("scholars", ref Scholars);
            s.Obj("litigation", ref Litigation);
            s.Obj("security", ref Security);
            s.Obj("govContracts", ref GovContracts);
            s.Obj("charter", ref Charter);
            s.Obj("scaling", ref Scaling);
            s.Obj("tax", ref Tax);
            s.Obj("standards", ref Standards);
            s.Obj("reliability", ref Reliability);
            s.Obj("covenants", ref Covenants);
            s.Obj("allowances", ref Allowances);
            s.Obj("listing", ref Listing);
            s.Obj("rbf", ref Rbf);
            s.Obj("foundation", ref Foundation);
            s.Obj("leaseback", ref Leaseback);
            s.Obj("convertibleNotes", ref ConvertibleNotes);
            s.Obj("devices", ref Devices);
            s.Obj("platforms", ref Platforms);
            s.Obj("creed", ref Creed);
            s.Obj("shocks", ref Shocks);
            s.Obj("replication", ref Replication);
            s.Obj("techBets", ref TechBets);
            s.Obj("waitlist", ref Waitlist);
            s.Obj("brand", ref Brand);
            s.Obj("freeTier", ref FreeTier);
            s.Obj("launchWatch", ref LaunchWatch);
            s.Obj("programmes", ref Programmes);
            s.Obj("runQueue", ref RunQueue);
            s.Obj("pods", ref Pods);
            s.Obj("licence", ref Licence);
            s.Obj("pledges", ref Pledges);
            s.Obj("integrations", ref Integrations);
            s.Obj("conformity", ref Conformity);
            s.Obj("org", ref Org);
            s.Obj("fabric", ref Fabric);
            s.Obj("disclosure", ref Disclosure);
            s.Obj("datasets", ref Datasets);
            s.Obj("userData", ref UserData);
            s.Obj("governance", ref Governance);
            s.Obj("buyback", ref Buyback);
            s.Obj("spot", ref Spot);
            s.Obj("agency", ref Agency);
            s.Obj("annotation", ref Annotation);
            s.Obj("postTraining", ref PostTraining);
            s.Obj("knowledge", ref Knowledge);
            s.Obj("boardAgenda", ref BoardAgenda);
            s.Obj("drills", ref Drills);
            s.Obj("evalHarness", ref EvalHarness);
            s.Obj("toolPolicy", ref ToolPolicy);
            s.Obj("humanQueue", ref HumanQueue);
            s.Obj("releaseChecklist", ref ReleaseChecklist);
            s.Obj("agentSandbox", ref AgentSandbox);
            s.Obj("runawayPlaybook", ref RunawayPlaybook);
            s.Obj("orchestrationBudget", ref OrchestrationBudget);
            s.Obj("trafficRouter", ref TrafficRouter);
            s.Obj("publicMetrics", ref PublicMetrics);
            s.Obj("specialistLines", ref SpecialistLines);
            s.Obj("dataSources", ref DataSources);
            s.Obj("openWeights", ref OpenWeights);
            s.Obj("sectorPricing", ref SectorPricing);
            s.Obj("campaignDeadline", ref CampaignDeadline);
            s.Obj("lawShaping", ref LawShaping);
            s.Obj("takeoffCrisis", ref TakeoffCrisis);
            s.Obj("scenarioBaggage", ref ScenarioBaggage);
            s.Obj("marginCall", ref MarginCall);
            s.Obj("hearingSharePrice", ref HearingSharePrice);
            s.Obj("rungSixCrisis", ref RungSixCrisis);
            s.Obj("scenarioGoals", ref ScenarioGoals);
            s.Obj("lawTarget", ref LawTarget);
            s.Obj("reservedComputeGate", ref ReservedComputeGate);
            s.Obj("rivalIntentNews", ref RivalIntentNews);
            s.Obj("gpuTierUnlock", ref GpuTierUnlock);
            s.Obj("endlessScenario", ref EndlessScenario);
            s.Obj("shadyFreezeAudit", ref ShadyFreezeAudit);
            s.Obj("specialistEraHooks", ref SpecialistEraHooks);
            s.Obj("sectorPricingBreakdown", ref SectorPricingBreakdown);
            s.Obj("dataSourceSpecialistPairing", ref DataSourceSpecialistPairing);
            s.Obj("publicMetricsEraLoad", ref PublicMetricsEraLoad);
            s.Obj("openWeightsEraThrottle", ref OpenWeightsEraThrottle);
            s.Obj("evalHarnessEraBoost", ref EvalHarnessEraBoost);
            s.Obj("campaignDeadlineEraGrace", ref CampaignDeadlineEraGrace);
            s.Obj("lawShapingEraCooldown", ref LawShapingEraCooldown);
            s.Obj("dataSourceRenewEraBonus", ref DataSourceRenewEraBonus);
            s.Obj("sectorPricingEraRelief", ref SectorPricingEraRelief);
            s.Obj("lawTargetEraAmplify", ref LawTargetEraAmplify);
            s.Obj("specialistBranchEraDiscount", ref SpecialistBranchEraDiscount);
            s.Obj("marginCallEraGrace", ref MarginCallEraGrace);
            s.Obj("hearingSharePriceEraBoost", ref HearingSharePriceEraBoost);
            s.Obj("rungSixEraCooldown", ref RungSixEraCooldown);
            s.Obj("dataSourceAcquireEraDiscount", ref DataSourceAcquireEraDiscount);
            s.Obj("marginSatisfyEraRelief", ref MarginSatisfyEraRelief);
            s.Obj("scenarioGoalsEraSlack", ref ScenarioGoalsEraSlack);
            s.Obj("takeoffCrisisEraGrace", ref TakeoffCrisisEraGrace);
            s.Obj("computeEquipment", ref ComputeEquipment);
            s.Obj("exitDebtEraHaircut", ref ExitDebtEraHaircut);
            s.Obj("takeoffAnswerEraCooldown", ref TakeoffAnswerEraCooldown);
            s.Obj("scenarioGoalEraTrust", ref ScenarioGoalEraTrust);
            s.Obj("hearingShareEraDampen", ref HearingShareEraDampen);
            s.Obj("officeReleaseEraGrace", ref OfficeReleaseEraGrace);
            s.Obj("dataSourceAcquireEraWeeks", ref DataSourceAcquireEraWeeks);
            s.Obj("escapeCrisisEraGrace", ref EscapeCrisisEraGrace);
            s.Obj("lawShapingEraAmplify", ref LawShapingEraAmplify);
            s.Obj("dataSourceRenewEraDiscount", ref DataSourceRenewEraDiscount);
            s.Obj("rungSixEraGrace", ref RungSixEraGrace);
            s.Obj("evalHarnessEraRelief", ref EvalHarnessEraRelief);
            s.Obj("publicMetricsEraDiscount", ref PublicMetricsEraDiscount);
            s.Obj("officeHearingEraGrace", ref OfficeHearingEraGrace);
            s.Obj("openWeightsCopyEraRelief", ref OpenWeightsCopyEraRelief);
            s.Obj("evalFailEraRelief", ref EvalFailEraRelief);
            s.Obj("lobbyingEraCooldown", ref LobbyingEraCooldown);
            s.Obj("publicMetricsMissEraRelief", ref PublicMetricsMissEraRelief);
            s.Obj("openWeightsOpenEraBoost", ref OpenWeightsOpenEraBoost);
            s.Obj("lobbyingCostEraRelief", ref LobbyingCostEraRelief);
            s.Obj("publicMetricsConformityEraSlack", ref PublicMetricsConformityEraSlack);
            s.Obj("specialistBranchEraBoost", ref SpecialistBranchEraBoost);
            s.Obj("pullout", ref Pullout);
            s.Obj("restructure", ref Restructure);
            s.Obj("incidentFloor", ref IncidentFloor);
            s.Obj("retrain", ref Retrain);
            s.Obj("audit", ref Audit);
            s.Obj("office", ref Office);
            s.Obj("objectives", ref Objectives);
            s.Obj("successor", ref Successor);
            s.Obj("rush", ref Rush);
            s.Obj("lobbying", ref Lobbying);
            s.Obj("loans", ref Loans);
            s.Obj("scenarios", ref Scenarios);
            s.Obj("pivot", ref Pivot);
            s.Obj("campaign", ref Campaign);
            s.Obj("runs", ref Runs);
            s.Obj("news", ref News);
            s.Obj("chat", ref Chat);
            s.Obj("glow", ref Glow);
            s.Obj("gauntlet", ref Gauntlet);
            s.Obj("rivalry", ref Rivalry);
            s.Obj("vendor", ref Vendor);
            s.Obj("chips", ref Chips);
            s.Obj("endings", ref Endings);
            s.Obj("shady", ref Shady);
            s.Obj("acquisitions", ref Acquisitions);
            s.Obj("modelSales", ref ModelSales);
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
        public FounderConfig Founder(string id) { return Find(Founders, id); }
        public StyleConfig Style(string id) { return Find(Styles, id); }
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
        /// <summary>Multiplies the base weekly incident chance while this era is active (1000 = unchanged).</summary>
        public int IncidentLoadPermille;
        /// <summary>The talent war: multiplies the one-off hiring fee while this era is active (1000 = unchanged). Salaries stay as listed.</summary>
        public int HireFeePermille;
        /// <summary>Chip shortages and gluts: multiplies the vendor's price for a GPU or chip while this era is active (1000 = unchanged).</summary>
        public int ChipPricePermille;
        /// <summary>How eagerly sectors take AI up: multiplies a sector's adoption (still capped at 1000) while this era is active.</summary>
        public int AdoptionPermille;
        /// <summary>The law's bite: multiplies the fine for an incident while this era is active (1000 = unchanged).</summary>
        public int FinePermille;
        /// <summary>The law's floor: a model whose displayed alignment is below this cannot be put on sale in this era. 0 means no rule. Never falls from one era to the next.</summary>
        public int MinReleaseAlignment;

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
            s.Int("incidentLoadPermille", ref IncidentLoadPermille);
            s.Int("hireFeePermille", ref HireFeePermille);
            s.Int("chipPricePermille", ref ChipPricePermille);
            s.Int("adoptionPermille", ref AdoptionPermille);
            s.Int("finePermille", ref FinePermille);
            s.Int("minReleaseAlignment", ref MinReleaseAlignment);
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
        /// <summary>A tech the lab must know before it may build this room, or empty.</summary>
        public string RequiresTech;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("era", ref Era);
            s.Str("requiresTech", ref RequiresTech);
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
        /// <summary>Weekly rent on a reserved contract, as a share of the spot rental price.</summary>
        public int ReservedRentPermille;
        public int ReservedContractWeeks;
        public int ReservedDepositWeeks;
        /// <summary>The vendor's line, oldest first.</summary>
        public List<GpuGenerationConfig> GpuGenerations;

        public void Sync(Syncer s)
        {
            s.Str("vendorName", ref VendorName);
            s.Str("vendorBoss", ref VendorBoss);
            s.Int("resalePermille", ref ResalePermille);
            s.Int("generationDecayPermille", ref GenerationDecayPermille);
            s.Long("moveCostPerUnit", ref MoveCostPerUnit);
            s.Int("reservedRentPermille", ref ReservedRentPermille);
            s.Int("reservedContractWeeks", ref ReservedContractWeeks);
            s.Int("reservedDepositWeeks", ref ReservedDepositWeeks);
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
        /// <summary>The most of its general compute a lab may set aside for research experiments (bible ch. 3, "split compute three ways").</summary>
        public int MaxComputeSharePermille;
        /// <summary>The research bonus, in permille, for each permille of compute set aside: 1200 turns a 10% share into +12% research.</summary>
        public int ComputeShareBonusPermille;
        public List<TechConfig> Techs;

        public void Sync(Syncer s)
        {
            s.Int("pointsPerResearcherWeek", ref PointsPerResearcherWeek);
            s.Int("maxComputeSharePermille", ref MaxComputeSharePermille);
            s.Int("computeShareBonusPermille", ref ComputeShareBonusPermille);
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
        public List<IncidentRungConfig> IncidentRungs = new List<IncidentRungConfig>();
        /// <summary>The weight of the era's plain mishap in the draw against the risk classes that bite a model.</summary>
        public int GenericRiskWeight;
        /// <summary>Kinds of trouble that open era by era.</summary>
        public List<RiskClassConfig> RiskClasses = new List<RiskClassConfig>();
        /// <summary>How each sector fails; a sector without an entry fails the plain way.</summary>
        public List<SectorFlavourConfig> SectorFlavours = new List<SectorFlavourConfig>();
        /// <summary>When false (the shipped value) only the player's labs pay what a flavour costs in fines, reputation, trust and refunds; rivals fail the plain way.</summary>
        public bool SectorFlavoursForRivals;
        /// <summary>The extra weekly incident chance, in permille, for a lab that serves none of its demand. It scales with the share left unserved (bible ch. 6, "overworked: serving load too high; reliability drops").</summary>
        public int OverloadIncidentPermille;
        /// <summary>When false (the shipped value) only the player's labs have the overload risk: rivals' economies are knife-edge (Q182).</summary>
        public bool OverloadForRivals;
        /// <summary>Below this true alignment, jailbreak-class incidents are picked instead of screenshots.</summary>
        public int Rung2BelowAlignment;
        /// <summary>When displayed alignment beats the true meter by at least this much, sandbagging rungs apply.</summary>
        public int SandbagGapPermille;
        public int Rung4CorruptionMin;
        public int Rung5CorruptionMin;
        public int Rung6CorruptionMin;
        public ContainmentConfig Containment;

        public SectorFlavourConfig SectorFlavour(string sectorId)
        {
            for (int i = 0; i < SectorFlavours.Count; i++) if (SectorFlavours[i].Sector == sectorId) return SectorFlavours[i];
            return null;
        }

        public IncidentRungConfig IncidentRung(int rung)
        {
            for (int i = 0; i < IncidentRungs.Count; i++) if (IncidentRungs[i].Rung == rung) return IncidentRungs[i];
            return null;
        }

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
            s.ObjList("incidentRungs", ref IncidentRungs);
            s.Int("genericRiskWeight", ref GenericRiskWeight);
            s.ObjList("riskClasses", ref RiskClasses);
            s.ObjList("sectorFlavours", ref SectorFlavours);
            s.Bool("sectorFlavoursForRivals", ref SectorFlavoursForRivals);
            s.Int("overloadIncidentPermille", ref OverloadIncidentPermille);
            s.Bool("overloadForRivals", ref OverloadForRivals);
            s.Int("rung2BelowAlignment", ref Rung2BelowAlignment);
            s.Int("sandbagGapPermille", ref SandbagGapPermille);
            s.Int("rung4CorruptionMin", ref Rung4CorruptionMin);
            s.Int("rung5CorruptionMin", ref Rung5CorruptionMin);
            s.Int("rung6CorruptionMin", ref Rung6CorruptionMin);
            s.Obj("containment", ref Containment);
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
        /// <summary>The group of sectors whose customers talk to each other: trust carries between them (see TrustConfig).</summary>
        public string Family;
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
            s.Str("family", ref Family);
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
        /// <summary>The highest insurance level; 0 for no insurance in the game.</summary>
        public int InsuranceMaxLevel;
        /// <summary>The share of an incident fine each level covers, in permille.</summary>
        public int InsuranceCoverPermille;
        /// <summary>The weekly premium per level as a share of last week's revenue, in permille.</summary>
        public int InsurancePremiumPermille;
        /// <summary>The weekly premium per level is never less than this many dollars.</summary>
        public long InsuranceMinPremium;
        /// <summary>Added to the premium for each claim the insurer remembers, in permille.</summary>
        public int InsuranceClaimLoadingPermille;
        /// <summary>One remembered claim is forgotten this many weeks apart.</summary>
        public int InsuranceForgetWeeks;
        /// <summary>The insurer ends the policy at this many remembered claims, and will not sell one while the lab is at it.</summary>
        public int InsuranceDropClaims;
        /// <summary>Incidents at this rung or above are not covered.</summary>
        public int InsuranceExcludedRung;
        /// <summary>Weeks a change to the policy locks it for.</summary>
        public int InsuranceLockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("bankruptcyGraceWeeks", ref BankruptcyGraceWeeks);
            s.Int("historyMonths", ref HistoryMonths);
            s.Int("insuranceMaxLevel", ref InsuranceMaxLevel);
            s.Int("insuranceCoverPermille", ref InsuranceCoverPermille);
            s.Int("insurancePremiumPermille", ref InsurancePremiumPermille);
            s.Long("insuranceMinPremium", ref InsuranceMinPremium);
            s.Int("insuranceClaimLoadingPermille", ref InsuranceClaimLoadingPermille);
            s.Int("insuranceForgetWeeks", ref InsuranceForgetWeeks);
            s.Int("insuranceDropClaims", ref InsuranceDropClaims);
            s.Int("insuranceExcludedRung", ref InsuranceExcludedRung);
            s.Int("insuranceLockWeeks", ref InsuranceLockWeeks);
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

        /// <summary>The most a lab can tune for the test. 0 switches tuning off.</summary>
        public int MaxTuningPermille;
        /// <summary>How much of the tuning shows up as score: a tuning of 200 with a share of 500 lifts every entry by 10%.</summary>
        public int TuningScoreSharePermille;
        /// <summary>The prep bill, in sales-role hire fees, for each 100 permille of tuning.</summary>
        public int TuningCostHireFeesPer100;
        /// <summary>The chance a tuned win is exposed, per 1000 permille of tuning (so 2000 makes a tuning of 100 a 20% risk).</summary>
        public int ExposeChancePerTuningPermille;
        /// <summary>Added to the chance for every rival that is in a feud with the tuned lab.</summary>
        public int ExposeFeudBonusPermille;
        public int ExposedReputationLoss;
        /// <summary>A model on stage whose weekly incident chance is at or under this has a triumphant demo.</summary>
        public int DemoTriumphBelowPermille;
        /// <summary>A model whose weekly incident chance is at or over this becomes a meme on stage. 0 switches the demo round off.</summary>
        public int DemoMemeFromPermille;
        public int DemoTriumphReputation;
        public int DemoMemeReputation;
        /// <summary>The smallest gap between claimed and true alignment that earns "Most Confident While Wrong".</summary>
        public int ConfidentWrongMinGap;
        public int ConfidentWrongReputation;
        /// <summary>A lab counts as a newcomer for this many years after its first release.</summary>
        public int NewcomerYears;
        public int NewcomerMorale;
        public int NewcomerReputation;
        public int SeasonCrownPoints;
        public int SeasonBracketPoints;

        public void Sync(Syncer s)
        {
            s.Str("name", ref Name);
            s.Int("jitterPermille", ref JitterPermille);
            s.Int("crownReputation", ref CrownReputation);
            s.Int("safestReputation", ref SafestReputation);
            s.Int("bracketWinnerAppealPermille", ref BracketWinnerAppealPermille);
            s.Int("historyYears", ref HistoryYears);
            s.Int("maxTuningPermille", ref MaxTuningPermille);
            s.Int("tuningScoreSharePermille", ref TuningScoreSharePermille);
            s.Int("tuningCostHireFeesPer100", ref TuningCostHireFeesPer100);
            s.Int("exposeChancePerTuningPermille", ref ExposeChancePerTuningPermille);
            s.Int("exposeFeudBonusPermille", ref ExposeFeudBonusPermille);
            s.Int("exposedReputationLoss", ref ExposedReputationLoss);
            s.Int("demoTriumphBelowPermille", ref DemoTriumphBelowPermille);
            s.Int("demoMemeFromPermille", ref DemoMemeFromPermille);
            s.Int("demoTriumphReputation", ref DemoTriumphReputation);
            s.Int("demoMemeReputation", ref DemoMemeReputation);
            s.Int("confidentWrongMinGap", ref ConfidentWrongMinGap);
            s.Int("confidentWrongReputation", ref ConfidentWrongReputation);
            s.Int("newcomerYears", ref NewcomerYears);
            s.Int("newcomerMorale", ref NewcomerMorale);
            s.Int("newcomerReputation", ref NewcomerReputation);
            s.Int("seasonCrownPoints", ref SeasonCrownPoints);
            s.Int("seasonBracketPoints", ref SeasonBracketPoints);
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
        /// <summary>The year the lab appears in the world. A lab with an earlier year than the game's start is there from the first week. <see cref="NeverJoins"/> is a lab kept in the data that never appears.</summary>
        public int JoinsYear;
        public PlanConfig Plan;

        /// <summary>A joinsYear that is never reached: a rival kept in the config as a spare, such as the successor that takes over when a lab fails.</summary>
        public const int NeverJoins = 9999;

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
            s.Int("joinsYear", ref JoinsYear);
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
        /// <summary>A trained model with less alignment than this is held back instead of released (0 releases everything).</summary>
        public int MinReleaseAlignment;
        /// <summary>The lab publishes its safety research: every other lab pays less for a safety tech the lab already has.</summary>
        public bool PublishesSafetyResearch;
        /// <summary>The chance, per year, that the board throws a fit and the plan stops for a few weeks.</summary>
        public int DramaPermillePerYear;
        /// <summary>The first year the lab designs its own chips (0 never).</summary>
        public int ChipProgramYear;
        /// <summary>The lab buys other labs' listed chips when they are cheaper per compute than the vendor's.</summary>
        public bool BuysListedChips;
        /// <summary>Rent-only labs buy listed chips into cloud deals instead of physical halls.</summary>
        public bool UsesCloudDeals;
        /// <summary>When above 0 the lab puts its own chips on sale at this mark-up over what they cost to make, in permille.</summary>
        public int ListsChipsMarkupPermille;
        /// <summary>How eagerly a plan-run lab takes shady jobs. Rival configs must be 0.</summary>
        public int ShadyAppetite;

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
            s.Int("minReleaseAlignment", ref MinReleaseAlignment);
            s.Bool("publishesSafetyResearch", ref PublishesSafetyResearch);
            s.Int("dramaPermillePerYear", ref DramaPermillePerYear);
            s.Int("chipProgramYear", ref ChipProgramYear);
            s.Bool("buysListedChips", ref BuysListedChips);
            s.Bool("usesCloudDeals", ref UsesCloudDeals);
            s.Int("listsChipsMarkupPermille", ref ListsChipsMarkupPermille);
            s.Int("shadyAppetite", ref ShadyAppetite);
        }
    }

    public sealed class PlanStaffConfig : ISync
    {
        public string RoleId;
        public int Count;
        /// <summary>The first calendar year the plan hires this role (0 from the start), so a late team does not sink a young lab.</summary>
        public int FromYear;

        public void Sync(Syncer s)
        {
            s.Str("roleId", ref RoleId);
            s.Int("count", ref Count);
            s.Int("fromYear", ref FromYear);
        }
    }
}
