using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Read-only decompositions of numbers the UI shows. Each part sums to the total.</summary>
    public static class DisplayQueries
    {
        public struct QualityBreakdown
        {
            public long FromCapability;
            public long FromAlignment;
            public long SandbagBoost;
            public long GlitchAdjustment;

            public long Total { get { return FromCapability + FromAlignment + SandbagBoost + GlitchAdjustment; } }
        }

        public struct AppealBreakdown
        {
            public long Core;
            public long ReputationBoost;
            public long SalesBoost;
            public long RoleBoost;
            public long TrustBoost;
            public long ChampionBoost;
            /// <summary>Zero or negative: customers who cannot tell the lab's models apart.</summary>
            public long NamingConfusion;
            /// <summary>Positive for a model trained to be engaging, negative for one trained to be careful; zero for the default.</summary>
            public long ObjectiveEffect;
            /// <summary>Positive while the launch window of a rushed release is open; zero otherwise.</summary>
            public long LaunchEffect;
            /// <summary>Zero or negative: customers still confused by the lab's last change of style.</summary>
            public long PivotConfusion;
            /// <summary>Positive while a marketing campaign runs in the sector, negative in the hangover after it; zero otherwise.</summary>
            public long CampaignEffect;
            /// <summary>Positive while an account team works the sector; zero otherwise.</summary>
            public long AccountEffect;

            public long Total { get { return Core + ReputationBoost + SalesBoost + RoleBoost + TrustBoost + ChampionBoost + NamingConfusion + ObjectiveEffect + LaunchEffect + PivotConfusion + CampaignEffect + AccountEffect; } }
        }

        public struct IncidentBreakdown
        {
            public int FromConfig;
            public int FromAlignment;
            public int FromCorruption;
            /// <summary>Extra chance from skipping testing before launch; zero for a tested launch.</summary>
            public int FromRush;
            /// <summary>Extra chance while the lab cannot serve its customers; zero from the model-only query.</summary>
            public int FromOverload;
            public int FromCrunch;
            /// <summary>What the lab's technical debt adds to the weekly chance; zero for a lab with none.</summary>
            public int FromDebt;
            /// <summary>What inference optimisation adds to the weekly chance (the models get brittle); zero when the lab does none.</summary>
            public int FromOptimisation;
            /// <summary>What a red team took off while its weeks last; zero or negative.</summary>
            public int FromRedTeam;
            /// <summary>What a sitting chief safety officer takes off the weekly chance: zero or negative.</summary>
            public int FromChief;
            /// <summary>What the lab's bug bounty takes off the weekly chance: zero or negative.</summary>
            public int FromBounty;

            public int Total { get { return FromConfig + FromAlignment + FromCorruption + FromRush + FromOverload + FromCrunch + FromDebt + FromOptimisation + FromRedTeam + FromChief + FromBounty; } }
        }

        public struct ExitPriceBreakdown
        {
            public long RevenueYears;
            public long FleetResale;
            public long CashOnHand;
            /// <summary>What the people own of the price (taken off the total). Zero until the team has equity.</summary>
            public long StaffShare;
            public long Loans;
            public long Notes;
            public long Rbf;
            public long CovenantCalls;
            public long Total;
        }

        public struct BubbleMeterBreakdown
        {
            public int Permille;
            public int GraceMonths;
            public int GraceMonthsNeeded;
            public int CrashMonthsRemaining;
            public long CapitalRaised;
            public long MarketRevenueLastMonth;
            /// <summary>0 calm, 1 warm, 2 hot, 3 crash.</summary>
            public int WarningLevel;
        }

        public struct RevenueBreakdown
        {
            public List<RevenuePart> Parts;
            public long Total;
        }

        public struct RevenuePart
        {
            public string SectorId;
            public long Amount;
        }

        public struct GpuRentBreakdown
        {
            public long SpotRentedGpus;
            public long ReservedDiscounted;
            public long CloudChips;

            public long Total { get { return SpotRentedGpus + ReservedDiscounted + CloudChips; } }
        }

        public struct ReservedComputeBreakdown
        {
            public int ReservedGpus;
            public int ActiveContracts;
            public long WeeklySpotEquivalent;
            public long WeeklyDiscounted;
            public int VendorQueuedGpus;

            public long WeeklySavings { get { return WeeklySpotEquivalent - WeeklyDiscounted; } }
        }

        public struct RivalDecisionSlice
        {
            public DecisionAction Action;
            public string Ref;
            public long Amount;
            public ReasonCode PrimaryReason;
            public bool HasReason;
        }

        public struct SignReservedQuote
        {
            public int GpuCount;
            public long Deposit;
            public long WeeklyRentAfterSign;
        }

        public struct VendorQueueBreakdown
        {
            public int Standing;
            public int AllocationThisMonth;
            public int BoughtThisMonth;
            public int AllocationLeft;
            public int QueuedGpus;
            public int QueueHoldingId;
            public int PricePermille;
            public long UnitPrice;
        }

        public struct FundingRoundQuote
        {
            public string RoundId;
            public long Cash;
            public int DilutionPermille;
            public int FounderShareAfterPermille;
            public bool DownRound;
        }

        public struct HardwareFleetBreakdown
        {
            public string CurrentGenId;
            public int OwnedHqGpus;
            public int OwnedOffsiteGpus;
            public int RentedGpus;
            public int ReservedGpus;

            public int TotalUnits { get { return OwnedHqGpus + OwnedOffsiteGpus + RentedGpus + ReservedGpus; } }
        }

        public struct FundingLadderStep
        {
            public string RoundId;
            public bool Completed;
            public bool Unlocked;
        }

        public struct RivalRelationSlice
        {
            public string RivalId;
            public string Name;
            public int FeelingToPlayer;
            public int LogLineCount;
        }

        public struct AlignmentGapBreakdown
        {
            public int EffectiveAlignment;
            public int DisplayedAlignment;
            public int GapPermille;
            public long SandbagBoost;
        }

        public struct StaffRosterBreakdown
        {
            public int Headcount;
            public int AverageMoralePermille;
            public long WeeklySalaries;
        }

        public struct BalanceSoakHostPreview
        {
            public int PresetCount;
            public int DefaultGamesPerPreset;
        }

        public static QualityBreakdown QualityParts(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            int effective = SafetySystem.EffectiveAlignment(c, model);
            int displayed = SafetySystem.DisplayedAlignment(model);
            int sandbagGap = (int)MathI.Max(0, displayed - model.Alignment);
            var b = new QualityBreakdown();
            b.FromCapability = MathI.MulPermille(model.Capability, sector.CapabilityWeightPermille);
            long effectiveAlign = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, effective));
            long displayBoost = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, sandbagGap));
            b.FromAlignment = effectiveAlign;
            b.SandbagBoost = displayBoost;
            if (SectorFlavours.IsGlitching(c, model, sector.Id, tick))
            {
                long before = b.FromCapability + effectiveAlign + displayBoost;
                long after = MathI.MulPermille(before, SectorFlavours.GlitchQualityPermille(c, sector.Id));
                b.GlitchAdjustment = after - before;
            }
            return b;
        }

        public static long Quality(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            return QualityParts(c, sector, model, tick).Total;
        }

        public static AppealBreakdown AppealParts(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            long quality = Quality(c, sector, model, state.Tick);
            var b = new AppealBreakdown();
            b.Core = quality * quality;
            long afterRep = MathI.MulPermille(b.Core, c.Safety.ReputationBaseline + lab.Reputation);
            b.ReputationBoost = afterRep - b.Core;
            long afterSales = MathI.MulPermille(afterRep, MarketSystem.SalesPermille(c, lab));
            b.SalesBoost = afterSales - afterRep;
            long afterRole = MathI.MulPermille(afterSales, (int)MathI.Max(1, MathI.Permille + RolePerks.SectorAppealPermille(c, lab, sector.Id) + Founding.AppealPermille(c, lab, sector.Id)));
            b.RoleBoost = afterRole - afterSales;
            long afterTrust = MathI.MulPermille(afterRole, TrustSystem.AppealPermille(c, lab, sector.Id));
            b.TrustBoost = afterTrust - afterRole;
            long afterNames = MathI.MulPermille(afterTrust, VersionNaming.AppealPermille(c, lab));
            b.NamingConfusion = afterNames - afterTrust;
            if (sectorState.ChampionLabId == lab.Id)
            {
                long afterCrown = MathI.MulPermille(afterNames, MathI.Permille + c.Gauntlet.BracketWinnerAppealPermille);
                b.ChampionBoost = afterCrown - afterNames;
            }
            b.ObjectiveEffect = MathI.MulPermille(b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost, TrainingObjectives.AppealPermille(c, model)) - (b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost);
            long beforeLaunch = b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost + b.ObjectiveEffect;
            b.LaunchEffect = MathI.MulPermille(beforeLaunch, RushedLaunch.LaunchAppealPermille(c, state.Tick, model)) - beforeLaunch;
            long beforePivot = beforeLaunch + b.LaunchEffect;
            b.PivotConfusion = MathI.MulPermille(beforePivot, Pivot.AppealPermille(c, state, lab)) - beforePivot;
            long beforeCampaign = beforePivot + b.PivotConfusion;
            b.CampaignEffect = MathI.MulPermille(beforeCampaign, MarketingCampaign.AppealPermille(c, state, lab, sector.Id)) - beforeCampaign;
            long beforeAccount = beforeCampaign + b.CampaignEffect;
            b.AccountEffect = MathI.MulPermille(beforeAccount, MathI.Permille + AccountTeams.AppealPermille(c, state, lab, sector.Id)) - beforeAccount;
            long beforeSpec = b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost + b.ObjectiveEffect + b.LaunchEffect + b.PivotConfusion + b.CampaignEffect + b.AccountEffect;
            int spec = SpecialistLines.AppealPermille(c, model, sector.Id) + OpenWeights.AppealPermille(c, model, sector.Id)
                + SpecialistEraHooks.AppealPermille(c, state, model, sector.Id);
            if (spec != 0)
            {
                long afterSpec = MathI.MulPermille(beforeSpec, MathI.Permille + spec);
                b.RoleBoost += afterSpec - beforeSpec;
            }
            return b;
        }

        public static long Appeal(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            return AppealParts(c, state, sector, sectorState, lab, model).Total;
        }

        public static IncidentBreakdown IncidentParts(GameConfig c, int tick, ModelState model)
        {
            int eraLoad = EraSystem.AtTick(c, tick).IncidentLoadPermille;
            if (eraLoad < 1) eraLoad = MathI.Permille;
            int scaledConfig = (int)MathI.MulPermille(c.Safety.IncidentChancePermille, eraLoad);
            var b = new IncidentBreakdown();
            b.FromConfig = scaledConfig;
            long baseChance = MathI.MulPermille(scaledConfig, MathI.Permille - model.Alignment);
            b.FromAlignment = (int)(baseChance - scaledConfig);
            int withoutRush = SafetySystem.IncidentChanceWithoutRush(c, tick, model);
            b.FromCorruption = withoutRush - (int)baseChance;
            int withRush = (int)MathI.Min(MathI.Permille, withoutRush + RushedLaunch.IncidentPermille(c, tick, model));
            b.FromRush = withRush - withoutRush;
            b.FromRedTeam = RedTeam.IncidentPermille(c, tick, model, withRush);
            return b;
        }

        /// <summary>The same breakdown for a model in a lab, with what overload adds.</summary>
        public static IncidentBreakdown IncidentParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            IncidentBreakdown b = IncidentParts(c, state.Tick, model);
            b.FromOverload = SafetySystem.OverloadPermille(c, state, lab);
            b.FromCrunch = Crunch.IncidentPermille(c, lab);
            b.FromDebt = TechDebt.IncidentPermille(c, lab);
            b.FromOptimisation = Optimisation.IncidentPermille(c, lab);
            long uncut = b.Total;
            b.FromChief = Chiefs.IncidentChange(c, lab, uncut);
            b.FromBounty = BugBounty.IncidentChange(c, lab, uncut);
            return b;
        }

        public static AcquisitionSystem.AcquisitionQuote AcquisitionQuoteParts(GameConfig c, GameState state, LabState buyer, LabState target)
        {
            return AcquisitionSystem.Quote(c, state, buyer, target);
        }

        public static ModelSalesSystem.ModelSaleQuote ModelSaleParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return ModelSalesSystem.Quote(c, state, lab, model);
        }

        public static BubbleMeterBreakdown BubbleMeterParts(GameConfig c, GameState state)
        {
            BubbleConfig b = c.Bubble;
            var p = new BubbleMeterBreakdown();
            p.Permille = state.BubblePermille;
            p.GraceMonths = state.BubbleGraceMonths;
            p.CrashMonthsRemaining = state.BubbleCrashMonthsRemaining;
            p.CapitalRaised = state.BubbleCapitalRaised;
            p.MarketRevenueLastMonth = BubbleSystem.MarketRevenueLastClosedMonth(state);
            p.GraceMonthsNeeded = b == null ? 0 : b.GraceMonthsBeforeCrash;
            int popWarn = b == null ? 50 : b.PopWarningMarginPermille;
            if (popWarn < 1) popWarn = 50;
            if (BubbleSystem.InCrash(state)) p.WarningLevel = 3;
            else if (b != null && state.BubblePermille >= b.PopAtPermille - popWarn) p.WarningLevel = 2;
            else if (b != null && state.BubblePermille >= b.HotPermille) p.WarningLevel = 1;
            return p;
        }

        public static ExitPriceBreakdown ExitPriceParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new ExitPriceBreakdown();
            b.RevenueYears = EndingSystem.RevenueLast12Months(lab) * c.Endings.ExitRevenueYears;
            b.FleetResale = MathI.MulPermille(LabQueries.FleetResaleValue(c, state, lab), c.Endings.ExitFleetPermille);
            b.CashOnHand = lab.Cash + lab.Reserve;
            long gross = b.RevenueYears + b.FleetResale + b.CashOnHand;
            b.StaffShare = StaffEquity.StaffShare(lab, gross);
            b.Loans = ExitDebt.LoanBalance(lab);
            b.Notes = ExitDebt.NoteBalance(lab);
            b.Rbf = ExitDebt.RbfBalance(lab);
            b.CovenantCalls = ExitDebt.CovenantCalls(c, lab);
            long debt = b.Loans + b.Notes + b.Rbf + b.CovenantCalls;
            debt = ExitDebtEraHaircut.AdjustDebt(c, state, debt);
            b.Total = gross - b.StaffShare - debt;
            return b;
        }

        public struct ComputeEquipmentVisuals
        {
            public string PcTower;
            public string Monitor;
            public string ServerRack;
        }

        public static ComputeEquipmentVisuals ComputeEquipmentVisualsFor(GameConfig c, LabState lab)
        {
            var v = new ComputeEquipmentVisuals();
            ComputeEquipmentTierConfig t = ComputeEquipment.Current(c, lab);
            int era = t == null || t.VisualEra <= 0 ? 1 : t.VisualEra;
            string suffix = "era" + era;
            v.PcTower = "compute-pc-tower-" + suffix;
            v.Monitor = "compute-monitor-" + suffix;
            v.ServerRack = "compute-server-rack-" + suffix;
            return v;
        }

        public static SectorPricingBreakdown.Parts SectorPriceParts(GameConfig c, LabState lab, string sectorId)
        {
            return SectorPricingBreakdown.Query(c, lab, sectorId);
        }

        public static RevenueBreakdown WeeklyRevenueParts(GameState state, LabState lab)
        {
            var b = new RevenueBreakdown();
            b.Parts = new List<RevenuePart>();
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sector = state.Sectors[s];
                long sectorTotal = 0;
                for (int i = 0; i < sector.Entries.Count; i++)
                {
                    SectorEntry entry = sector.Entries[i];
                    if (entry.LabId == lab.Id) sectorTotal += entry.LastRevenue;
                }
                if (sectorTotal != 0)
                {
                    b.Parts.Add(new RevenuePart { SectorId = sector.Id, Amount = sectorTotal });
                    b.Total += sectorTotal;
                }
            }
            return b;
        }

        public static GpuRentBreakdown GpuRentParts(GameConfig c, GameState state, LabState lab)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            var b = new GpuRentBreakdown();
            b.SpotRentedGpus = gen.RentPerGpuWeek * lab.RentedGpus;
            b.ReservedDiscounted = ComputeContractSystem.WeeklyReservedRent(c, state, lab);
            b.CloudChips = ChipQueries.WeeklyCloudChipRent(c, state, lab);
            return b;
        }

        public static ReservedComputeBreakdown ReservedComputeParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new ReservedComputeBreakdown();
            if (lab == null) return b;
            b.ReservedGpus = ComputeContractSystem.ReservedGpus(lab);
            b.ActiveContracts = lab.ReservedContracts.Count;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            b.WeeklySpotEquivalent = gen.RentPerGpuWeek * b.ReservedGpus;
            b.WeeklyDiscounted = ComputeContractSystem.WeeklyReservedRent(c, state, lab);
            VendorStanding standing = state.Vendor.Standing(lab.Id);
            b.VendorQueuedGpus = standing == null ? 0 : standing.QueuedGpus;
            return b;
        }

        public static bool TrySignReservedQuote(GameConfig c, GameState state, LabState lab, int gpuCount, out SignReservedQuote quote)
        {
            quote = new SignReservedQuote();
            if (lab == null || gpuCount < 1) return false;
            quote.GpuCount = gpuCount;
            quote.Deposit = ComputeContractSystem.Deposit(c, state, lab, gpuCount);
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            quote.WeeklyRentAfterSign = MathI.MulPermille(gpuCount * gen.RentPerGpuWeek, c.Compute.ReservedRentPermille);
            return true;
        }

        public static HardwareFleetBreakdown HardwareFleetParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new HardwareFleetBreakdown();
            if (lab == null) return b;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            b.CurrentGenId = gen == null ? "" : gen.Id;
            b.OwnedHqGpus = LabQueries.OwnedGpuUnits(lab, HardwareBatch.Hq);
            int owned = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
                if (lab.Fleet[i].ChipDesignId < 0) owned += lab.Fleet[i].Count;
            b.OwnedOffsiteGpus = owned - b.OwnedHqGpus;
            if (b.OwnedOffsiteGpus < 0) b.OwnedOffsiteGpus = 0;
            b.RentedGpus = lab.RentedGpus;
            b.ReservedGpus = ComputeContractSystem.ReservedGpus(lab);
            return b;
        }

        public static List<FundingLadderStep> FundingLadderParts(GameConfig c, GameState state, LabState lab)
        {
            var steps = new List<FundingLadderStep>();
            if (lab == null) return steps;
            int year = GameDate.Year(c.Calendar, state.Tick);
            bool frozen = WorldQueries.IsFrozen(lab, lab.HomeBlocId);
            for (int i = 0; i < c.Funding.Rounds.Count; i++)
            {
                FundingRoundConfig round = c.Funding.Rounds[i];
                var step = new FundingLadderStep();
                step.RoundId = round.Id;
                step.Completed = FundingSystem.HasRound(lab, round.Id);
                step.Unlocked = !step.Completed && !frozen && year >= round.OpensYear
                    && lab.Reputation >= round.MinReputation && FundingSystem.PriorRoundsDone(c, lab, round);
                steps.Add(step);
            }
            return steps;
        }

        public static AlignmentGapBreakdown AlignmentGapParts(GameConfig c, ModelState model, int tick)
        {
            var b = new AlignmentGapBreakdown();
            if (model == null) return b;
            b.EffectiveAlignment = SafetySystem.EffectiveAlignment(c, model);
            b.DisplayedAlignment = SafetySystem.DisplayedAlignment(model);
            b.GapPermille = (int)MathI.Max(0, b.DisplayedAlignment - model.Alignment);
            SectorConfig sector = c.Market.Sectors.Count > 0 ? c.Market.Sectors[0] : null;
            if (sector != null)
            {
                QualityBreakdown q = QualityParts(c, sector, model, tick);
                b.SandbagBoost = q.SandbagBoost;
            }
            return b;
        }

        public static StaffRosterBreakdown StaffRosterParts(GameConfig c, LabState lab)
        {
            var b = new StaffRosterBreakdown();
            if (lab == null) return b;
            b.Headcount = lab.Staff.Count;
            if (b.Headcount > 0)
            {
                long morale = 0;
                for (int i = 0; i < lab.Staff.Count; i++) morale += lab.Staff[i].Morale;
                b.AverageMoralePermille = (int)(morale / b.Headcount);
            }
            b.WeeklySalaries = LabQueries.WeeklySalaries(c, lab);
            return b;
        }

        public static BalanceSoakHostPreview BalanceSoakPreview(GameConfig c)
        {
            var b = new BalanceSoakHostPreview();
            BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(c);
            b.PresetCount = presets == null ? 0 : presets.Length;
            b.DefaultGamesPerPreset = 24;
            return b;
        }

        public static List<RivalRelationSlice> RivalRelationSlices(GameConfig c, GameState state, int maxRivals)
        {
            var slices = new List<RivalRelationSlice>();
            if (state == null || maxRivals < 1) return slices;
            int added = 0;
            for (int i = 0; i < state.Labs.Count && added < maxRivals; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Id == state.PlayerLabId) continue;
                if (lab.Controller != LabController.Plan) continue;
                var slice = new RivalRelationSlice();
                slice.RivalId = lab.RivalId;
                slice.Name = lab.Name;
                slice.FeelingToPlayer = lab.FeelingToPlayer;
                slice.LogLineCount = lab.Log.Count;
                slices.Add(slice);
                added++;
            }
            return slices;
        }

        public static VendorQueueBreakdown VendorQueueParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new VendorQueueBreakdown();
            if (lab == null) return b;
            VendorStanding standing = VendorSystem.EnsureStanding(c, state, lab.Id);
            b.Standing = standing.Standing;
            b.AllocationThisMonth = standing.AllocationThisMonth;
            b.BoughtThisMonth = standing.BoughtThisMonth;
            b.AllocationLeft = VendorSystem.AllocationLeft(state, lab);
            b.QueuedGpus = standing.QueuedGpus;
            b.QueueHoldingId = standing.QueueHoldingId;
            b.PricePermille = VendorSystem.PricePermille(c, state, lab);
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            b.UnitPrice = VendorSystem.PricePerGpu(c, state, lab, gen);
            return b;
        }

        /// <summary>Recent Envydia decision lines (allocation shifts, courting). Same templates as rivals.</summary>
        public static List<string> VendorLogLines(GameConfig c, GameState state, int maxLines)
        {
            var lines = new List<string>();
            if (state?.Vendor == null || maxLines < 1) return lines;
            List<DecisionEntry> log = state.Vendor.Log;
            int start = log.Count - maxLines;
            if (start < 0) start = 0;
            for (int i = start; i < log.Count; i++)
                lines.Add(DecisionText.Line(c, state, null, log[i]));
            return lines;
        }

        public static bool TryNextFundingQuote(GameConfig c, GameState state, LabState lab, out FundingRoundQuote quote)
        {
            quote = new FundingRoundQuote();
            if (lab == null || lab.Id != state.PlayerLabId) return false;
            for (int i = 0; i < c.Funding.Rounds.Count; i++)
            {
                FundingRoundConfig round = c.Funding.Rounds[i];
                if (FundingSystem.HasRound(lab, round.Id)) continue;
                if (!FundingSystem.PriorRoundsDone(c, lab, round)) continue;
                int year = GameDate.Year(c.Calendar, state.Tick);
                if (year < round.OpensYear) continue;
                if (lab.Reputation < round.MinReputation) continue;
                if (WorldQueries.IsFrozen(lab, lab.HomeBlocId)) continue;
                quote.RoundId = round.Id;
                quote.Cash = FundingSystem.EffectiveCash(c, state, round);
                quote.DilutionPermille = FundingSystem.EffectiveDilution(c, state, round);
                quote.DownRound = FundingSystem.IsDownRound(c, state);
                quote.FounderShareAfterPermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - quote.DilutionPermille);
                return true;
            }
            return false;
        }

        /// <summary>Recent rival decision lines for the host UI. Empty for the player's lab or bots.</summary>
        public static List<string> RivalLogLines(GameConfig c, GameState state, LabState lab, int maxLines)
        {
            var lines = new List<string>();
            if (lab == null || lab.Controller != LabController.Plan || maxLines < 1) return lines;
            int start = lab.Log.Count - maxLines;
            if (start < 0) start = 0;
            for (int i = start; i < lab.Log.Count; i++)
                lines.Add(DecisionText.Line(c, state, lab, lab.Log[i]));
            return lines;
        }

        /// <summary>Structured tail of a plan-run lab's decision log. Empty for the player or bots.</summary>
        public static List<RivalDecisionSlice> RivalDecisionSlices(GameConfig c, GameState state, LabState lab, int maxLines)
        {
            var slices = new List<RivalDecisionSlice>();
            if (lab == null || lab.Controller != LabController.Plan || maxLines < 1) return slices;
            int start = lab.Log.Count - maxLines;
            if (start < 0) start = 0;
            for (int i = start; i < lab.Log.Count; i++)
            {
                DecisionEntry e = lab.Log[i];
                var slice = new RivalDecisionSlice();
                slice.Action = e.Action;
                slice.Ref = e.Ref;
                slice.Amount = e.Amount;
                slice.HasReason = e.Reasons.Count > 0;
                if (slice.HasReason) slice.PrimaryReason = e.Reasons[0].Code;
                slices.Add(slice);
            }
            return slices;
        }
    }
}