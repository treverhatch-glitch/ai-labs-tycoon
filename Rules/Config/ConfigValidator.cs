using System;
using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }
    }

    /// <summary>
    /// Checks a config that parsed: ids are unique, every reference names something that exists,
    /// and numbers are in the range the rules assume. All problems are reported at once.
    /// </summary>
    public static class ConfigValidator
    {
        public static void Validate(GameConfig c)
        {
            List<string> problems = Problems(c);
            if (problems.Count > 0) throw new ConfigException("The config has " + problems.Count + " problem(s):\n- " + string.Join("\n- ", problems));
        }

        public static List<string> Problems(GameConfig c)
        {
            var p = new List<string>();

            if (string.IsNullOrEmpty(c.Version)) p.Add("version is empty");
            if (c.EventLogMax < 1) p.Add("eventLogMax must be at least 1");
            if (c.Calendar.WeeksPerMonth < 1) p.Add("calendar.weeksPerMonth must be at least 1");
            if (c.Calendar.MonthsPerYear < 1) p.Add("calendar.monthsPerYear must be at least 1");

            UniqueIds(p, "eras", c.Eras);
            UniqueIds(p, "worldEvents", c.WorldEvents);
            UniqueIds(p, "lots", c.Lots);
            UniqueIds(p, "roles", c.Roles);
            UniqueIds(p, "rolePerks", c.RolePerks);
            UniqueIds(p, "roomPerks", c.RoomPerks);
            UniqueIds(p, "founders", c.Founders);
            UniqueIds(p, "styles", c.Styles);
            UniqueIds(p, "roomTypes", c.RoomTypes);
            UniqueIds(p, "research.techs", c.Research.Techs);
            UniqueIds(p, "training.lines", c.Training.Lines);
            UniqueIds(p, "market.sectors", c.Market.Sectors);
            UniqueIds(p, "rivals", c.Rivals);

            EraProblems(p, c);

            if (c.Lots.Count == 0) p.Add("lots is empty");
            for (int i = 0; i < c.Lots.Count; i++)
            {
                LotConfig lot = c.Lots[i];
                if (lot.Width < 1 || lot.Height < 1) p.Add("lot " + lot.Id + " has no area");
                if (lot.Price < 0) p.Add("lot " + lot.Id + " has a negative price");
                if (i > 0 && (lot.Width < c.Lots[i - 1].Width || lot.Height < c.Lots[i - 1].Height))
                    p.Add("lot " + lot.Id + " is smaller than the lot before it: rooms keep their place when a lab moves up, so each lot must contain the one before");
            }

            foreach (RoleConfig role in c.Roles)
            {
                if (role.HireCost < 0 || role.WeeklySalary < 0 || role.Severance < 0) p.Add("role " + role.Id + " has a negative cost");
            }

            RolePerkProblems(p, c);
            RoomPerkProblems(p, c);
            FoundingProblems(p, c);
            TalentProblems(p, c);
            PressProblems(p, c);
            TrustProblems(p, c);
            CultureProblems(p, c);
            NamingProblems(p, c);
            AgeingProblems(p, c);
            ControlProblems(p, c);
            LeadsProblems(p, c);
            RepairProblems(p, c);
            RedTeamProblems(p, c);
            InterpProblems(p, c);
            ComplianceProblems(p, c);
            PrestigeProblems(p, c);
            SubletProblems(p, c);
            AuditProblems(p, c);
            FocusProblems(p, c);
            HackProblems(p, c);
            EquityProblems(p, c);
            ChiefsProblems(p, c);
            BoardProblems(p, c);
            OptimiseProblems(p, c);
            MarketingProblems(p, c);
            OnCallProblems(p, c);
            ConsultantsProblems(p, c);
            PostMortemProblems(p, c);
            SlaProblems(p, c);
            ResearchGrantsProblems(p, c);
            AbandonProblems(p, c);
            LayoffsProblems(p, c);
            TechDebtProblems(p, c);
            SeniorityProblems(p, c);
            RevenueTargetsProblems(p, c);
            ChairsProblems(p, c);
            WhistleProblems(p, c);
            BugBountyProblems(p, c);
            TreasuryProblems(p, c);
            PledgeProblems(p, c);
            RoadmapProblems(p, c);
            MentoringProblems(p, c);
            SkunkworksProblems(p, c);
            LabourProblems(p, c);
            SunsetProblems(p, c);
            AccountsProblems(p, c);
            ConferenceProblems(p, c);
            AlumniProblems(p, c);
            InternshipsProblems(p, c);
            SupportProblems(p, c);
            SabbaticalProblems(p, c);
            RequisitionsProblems(p, c);
            ScholarsProblems(p, c);
            LitigationProblems(p, c);
            SecurityProblems(p, c);
            GovContractsProblems(p, c);
            CharterProblems(p, c);
            ScalingProblems(p, c);
            TaxProblems(p, c);
            StandardsProblems(p, c);
            ReliabilityProblems(p, c);
            CovenantsProblems(p, c);
            AllowancesProblems(p, c);
            ListingProblems(p, c);
            RbfProblems(p, c);
            FoundationProblems(p, c);
            LeasebackProblems(p, c);
            NotesProblems(p, c);
            DevicesProblems(p, c);
            PlatformsProblems(p, c);
            CreedProblems(p, c);
            ShocksProblems(p, c);
            ReplicationProblems(p, c);
            TechBetsProblems(p, c);
            WaitlistProblems(p, c);
            BrandProblems(p, c);
            FreeTierProblems(p, c);
            LaunchWatchProblems(p, c);
            ProgrammesProblems(p, c);
            RunQueueProblems(p, c);
            PodsProblems(p, c);
            LicenceProblems(p, c);
            PledgesProblems(p, c);
            IntegrationsProblems(p, c);
            ConformityProblems(p, c);
            OrgProblems(p, c);
            FabricProblems(p, c);
            DisclosureProblems(p, c);
            DatasetsProblems(p, c);
            UserDataProblems(p, c);
            GovernanceProblems(p, c);
            BuybackProblems(p, c);
            SpotProblems(p, c);
            AgencyProblems(p, c);
            AnnotationProblems(p, c);
            PostTrainingProblems(p, c);
            KnowledgeProblems(p, c);
            BoardAgendaProblems(p, c);
            DrillsProblems(p, c);
            EvalHarnessProblems(p, c);
            ToolPolicyProblems(p, c);
            HumanQueueProblems(p, c);
            ReleaseChecklistProblems(p, c);
            AgentSandboxProblems(p, c);
            RunawayPlaybookProblems(p, c);
            OrchestrationBudgetProblems(p, c);
            TrafficRouterProblems(p, c);
            PublicMetricsProblems(p, c);
            PulloutProblems(p, c);
            RestructureProblems(p, c);
            IncidentFloorProblems(p, c);
            RetrainProblems(p, c);
            OfficeProblems(p, c);
            ObjectiveProblems(p, c);
            SuccessorProblems(p, c);
            RushProblems(p, c);
            LobbyProblems(p, c);
            LoanProblems(p, c);
            ScenarioProblems(p, c);
            PivotProblems(p, c);
            CampaignProblems(p, c);
            RunsProblems(p, c);
            NewsProblems(p, c);
            ChatProblems(p, c);
            GlowProblems(p, c);

            foreach (RoomTypeConfig room in c.RoomTypes)
            {
                bool seats = !string.IsNullOrEmpty(room.SeatsRole);
                if (c.Era(room.Era) == null) p.Add("roomType " + room.Id + " names the unknown era \"" + room.Era + "\"");
                if (seats && c.Role(room.SeatsRole) == null) p.Add("roomType " + room.Id + " seats the unknown role \"" + room.SeatsRole + "\"");
                if (seats && room.TilesPerSeat < 1) p.Add("roomType " + room.Id + " seats staff, so tilesPerSeat must be at least 1");
                if (room.GpuSlotsPerTile < 0) p.Add("roomType " + room.Id + " has negative gpuSlotsPerTile");
                if (room.MinWidth < 1 || room.MinHeight < 1) p.Add("roomType " + room.Id + " needs minWidth and minHeight of at least 1");
                if (room.BuildCostPerTile < 0 || room.UpkeepPerTileWeek < 0) p.Add("roomType " + room.Id + " has a negative cost");
                if (!string.IsNullOrEmpty(room.RequiresTech) && c.Tech(room.RequiresTech) == null) p.Add("roomType " + room.Id + " requires the unknown tech \"" + room.RequiresTech + "\"");
            }
            foreach (RoleConfig role in c.Roles)
            {
                bool seated = false;
                foreach (RoomTypeConfig room in c.RoomTypes) if (room.SeatsRole == role.Id) seated = true;
                if (!seated) p.Add("role " + role.Id + " has no room type that seats it, so nobody could ever be hired");
            }

            ComputeProblems(p, c);
            WorldProblems(p, c);

            if (c.Research.PointsPerResearcherWeek < 0) p.Add("research.pointsPerResearcherWeek is negative");
            Permille(p, "research.maxComputeSharePermille", c.Research.MaxComputeSharePermille);
            if (c.Research.ComputeShareBonusPermille < 0) p.Add("research.computeShareBonusPermille is negative");
            foreach (TechConfig tech in c.Research.Techs)
            {
                if (tech.Cost < 0) p.Add("tech " + tech.Id + " has a negative cost");
                EraConfig techEra = c.Era(tech.Era);
                if (techEra == null) p.Add("tech " + tech.Id + " names the unknown era \"" + tech.Era + "\"");
                foreach (string req in tech.Requires)
                {
                    if (c.Tech(req) == null) p.Add("tech " + tech.Id + " requires the unknown tech \"" + req + "\"");
                    if (req == tech.Id) p.Add("tech " + tech.Id + " requires itself");
                    TechConfig reqTech = c.Tech(req);
                    if (techEra != null && reqTech != null && c.Era(reqTech.Era) != null && c.Era(reqTech.Era).StartYear > techEra.StartYear)
                        p.Add("tech " + tech.Id + " (" + tech.Era + ") requires " + req + ", which only arrives with the later era " + reqTech.Era);
                }
                foreach (TechEffectConfig e in tech.Effects)
                {
                    if (e.Kind == TechEffectKind.UnlockLine && c.Line(e.Id) == null) p.Add("tech " + tech.Id + " unlocks the unknown model line \"" + e.Id + "\"");
                    if (e.Kind == TechEffectKind.UnlockChipTarget && c.Chips.Target(e.Id) == null) p.Add("tech " + tech.Id + " unlocks the unknown chip target \"" + e.Id + "\"");
                    if (e.Kind != TechEffectKind.UnlockLine && e.Kind != TechEffectKind.UnlockChipTarget && e.Amount <= 0) p.Add("tech " + tech.Id + " has an effect with no amount");
                }
            }
            TechCycles(p, c);

            if (c.Training.MaxConcurrentRuns < 1) p.Add("training.maxConcurrentRuns must be at least 1");
            Permille(p, "training.carryoverPermille", c.Training.CarryoverPermille);
            if (c.Training.LuckPermille < 0 || c.Training.LuckPermille >= 1000) p.Add("training.luckPermille must be from 0 to 999");
            if (c.Training.Lines.Count == 0) p.Add("training.lines is empty");
            foreach (ModelLineConfig line in c.Training.Lines)
            {
                if (line.ComputeRequired < 1) p.Add("line " + line.Id + " needs computeRequired of at least 1");
                if (line.BaseCapability < 1) p.Add("line " + line.Id + " needs baseCapability of at least 1");
                if (line.PrepCost < 0) p.Add("line " + line.Id + " has a negative prepCost");
                if (line.ServingCostPermille < 1) p.Add("line " + line.Id + " needs servingCostPermille of at least 1");
                if (line.Locked && !Unlockable(c, line.Id)) p.Add("line " + line.Id + " is locked and no tech unlocks it");
            }

            if (c.Safety.FullAlignmentComputePermille < 1) p.Add("safety.fullAlignmentComputePermille must be at least 1");
            if (c.Safety.IncidentFineRevenueWeeks < 0 || c.Safety.IncidentFineMin < 0) p.Add("safety has a negative fine");
            if (c.Safety.GlitchWeeks < 0) p.Add("safety.glitchWeeks is negative");
            Permille(p, "safety.incidentChancePermille", c.Safety.IncidentChancePermille);
            Permille(p, "safety.glitchQualityPermille", c.Safety.GlitchQualityPermille);
            Permille(p, "safety.reputationBaseline", c.Safety.ReputationBaseline);
            Permille(p, "safety.sandbagGapPermille", c.Safety.SandbagGapPermille);
            Permille(p, "safety.rung2BelowAlignment", c.Safety.Rung2BelowAlignment);
            if (c.Safety.IncidentRungs.Count < 3) p.Add("safety.incidentRungs needs at least three rungs");
            UniqueIds(p, "safety.incidentRungs", c.Safety.IncidentRungs);
            var rungSeen = new bool[7];
            for (int i = 0; i < c.Safety.IncidentRungs.Count; i++)
            {
                IncidentRungConfig r = c.Safety.IncidentRungs[i];
                if (r.Rung < 1 || r.Rung > 6) p.Add("incident rung " + r.Id + " must be from 1 to 6");
                else if (rungSeen[r.Rung]) p.Add("safety.incidentRungs has more than one entry for rung " + r.Rung);
                else rungSeen[r.Rung] = true;
                if (r.FinePermille < 0) p.Add("incident rung " + r.Id + " has negative finePermille");
                if (r.ReputationLoss < 0) p.Add("incident rung " + r.Id + " has negative reputationLoss");
            }
            if (!rungSeen[1] || !rungSeen[2] || !rungSeen[3]) p.Add("safety.incidentRungs must name rungs 1, 2 and 3 exactly once each");
            if (rungSeen[4] || rungSeen[5] || rungSeen[6])
            {
                if (c.Safety.Rung4CorruptionMin < 1 || c.Safety.Rung5CorruptionMin <= c.Safety.Rung4CorruptionMin
                    || c.Safety.Rung6CorruptionMin <= c.Safety.Rung5CorruptionMin)
                    p.Add("safety rung4/5/6 corruption thresholds must rise from 1");
            }
            RiskClassProblems(p, c);
            SectorFlavourProblems(p, c);
            ContainmentConfig cont = c.Safety.Containment;
            if (cont != null)
            {
                if (string.IsNullOrEmpty(cont.RoomId)) p.Add("safety.containment.roomId is empty");
                else if (c.RoomType(cont.RoomId) == null) p.Add("safety.containment.roomId names the unknown room \"" + cont.RoomId + "\"");
                if (cont.AlignmentPermillePerTile < 0) p.Add("safety.containment.alignmentPermillePerTile is negative");
                if (cont.AlignmentCapPermille < 0) p.Add("safety.containment.alignmentCapPermille is negative");
            }

            Permille(p, "market.switchRatePermille", c.Market.SwitchRatePermille);
            if (c.Market.Sectors.Count == 0) p.Add("market.sectors is empty");
            foreach (SectorConfig sector in c.Market.Sectors)
            {
                if (sector.GrowthDecayPermille < 1 || sector.GrowthDecayPermille > MathI.Permille) p.Add("sector " + sector.Id + ": growthDecayPermille must be from 1 to 1000");
                if (sector.PoolPerWeek < 0) p.Add("sector " + sector.Id + " has a negative poolPerWeek");
                if (sector.MinCapability < 1) p.Add("sector " + sector.Id + " needs minCapability of at least 1");
                EraConfig sectorEra = c.Era(sector.Era);
                if (sectorEra == null) p.Add("sector " + sector.Id + " names the unknown era \"" + sector.Era + "\"");
                else
                {
                    int next = c.Eras.IndexOf(sectorEra) + 1;
                    int until = next < c.Eras.Count ? c.Eras[next].StartYear : int.MaxValue;
                    if (sector.OpensYear < sectorEra.StartYear || sector.OpensYear >= until)
                        p.Add("sector " + sector.Id + " opens in " + sector.OpensYear + ", outside its era " + sector.Era + " (" + sectorEra.StartYear + " to " + (until == int.MaxValue ? "the end" : (until - 1).ToString()) + ")");
                }
                if (sector.SaturationCapability < sector.MinCapability) p.Add("sector " + sector.Id + " saturates below its own entry bar");
                if (sector.CapabilityWeightPermille + sector.SafetyWeightPermille != 1000) p.Add("sector " + sector.Id + ": capabilityWeightPermille and safetyWeightPermille must add up to 1000");
                if (sector.CapabilityWeightPermille < 0 || sector.SafetyWeightPermille < 0) p.Add("sector " + sector.Id + " has a negative weight");
                if (sector.RevenuePerComputeUnit < 1) p.Add("sector " + sector.Id + " needs revenuePerComputeUnit of at least 1");
            }

            if (c.Finance.BankruptcyGraceWeeks < 1) p.Add("finance.bankruptcyGraceWeeks must be at least 1");
            if (c.Finance.HistoryMonths < 1) p.Add("finance.historyMonths must be at least 1");
            FinanceConfig fin = c.Finance;
            if (fin.InsuranceMaxLevel < 0 || fin.InsuranceMaxLevel > 10) p.Add("finance.insuranceMaxLevel needs 0 to 10");
            if (fin.InsuranceCoverPermille < 0 || fin.InsuranceCoverPermille > MathI.Permille) p.Add("finance.insuranceCoverPermille needs 0 to 1000");
            if (fin.InsuranceMaxLevel * fin.InsuranceCoverPermille > MathI.Permille) p.Add("finance.insuranceMaxLevel times insuranceCoverPermille is over 1000, so the top level would cover more than the fine");
            if (fin.InsurancePremiumPermille < 0 || fin.InsurancePremiumPermille > MathI.Permille) p.Add("finance.insurancePremiumPermille needs 0 to 1000");
            if (fin.InsuranceMinPremium < 0) p.Add("finance.insuranceMinPremium is negative");
            if (fin.InsuranceClaimLoadingPermille < 0 || fin.InsuranceClaimLoadingPermille > 10000) p.Add("finance.insuranceClaimLoadingPermille needs 0 to 10000");
            if (fin.InsuranceForgetWeeks < 1 || fin.InsuranceForgetWeeks > 520) p.Add("finance.insuranceForgetWeeks needs 1 to 520");
            if (fin.InsuranceDropClaims < 1 || fin.InsuranceDropClaims > 100) p.Add("finance.insuranceDropClaims needs 1 to 100");
            if (fin.InsuranceExcludedRung < 1 || fin.InsuranceExcludedRung > 99) p.Add("finance.insuranceExcludedRung needs 1 to 99");
            if (fin.InsuranceLockWeeks < 0 || fin.InsuranceLockWeeks > 520) p.Add("finance.insuranceLockWeeks needs 0 to 520");
            FundingProblems(p, c);
            StaffPeopleProblems(p, c);
            if (c.Gauntlet.JitterPermille < 0 || c.Gauntlet.JitterPermille >= 1000) p.Add("gauntlet.jitterPermille must be from 0 to 999");

            if (c.Gauntlet.HistoryYears < 1) p.Add("gauntlet.historyYears must be at least 1");
            GauntletProblems(p, c);

            if (c.Lot(c.PlayerStart.LotId) == null) p.Add("playerStart.lotId names the unknown lot \"" + c.PlayerStart.LotId + "\"");
            if (c.PlayerStart.Cash < 0) p.Add("playerStart.cash is negative");
            Permille(p, "playerStart.reputation", c.PlayerStart.Reputation);
            Permille(p, "playerStart.trainingSharePermille", c.PlayerStart.TrainingSharePermille);

            foreach (RivalConfig rival in c.Rivals)
            {
                if (c.Lot(rival.LotId) == null) p.Add("rival " + rival.Id + " starts on the unknown lot \"" + rival.LotId + "\"");
                Permille(p, "rival " + rival.Id + " reputation", rival.Reputation);
                if (rival.JoinsYear < 0) p.Add("rival " + rival.Id + " has a negative joinsYear");
                PlanProblems(p, c, "rival " + rival.Id, rival.Plan);
            }

            PlanProblems(p, c, "autopilot", c.Autopilot);

            RivalryProblems(p, c);
            VendorProblems(p, c);
            ChipsProblems(p, c);
            EndingsProblems(p, c);
            ShadyProblems(p, c);
            BubbleProblems(p, c);
            AcquisitionProblems(p, c);
            ModelSalesProblems(p, c);
            SharesProblems(p, c);
            RegulatorsProblems(p, c);
            WorldEventProblems(p, c);

            return p;
        }

        static void EndingsProblems(List<string> p, GameConfig c)
        {
            if (c.Endings == null) { p.Add("endings is missing"); return; }
            UniqueIds(p, "endings.endings", c.Endings.Endings);
            if (c.Endings.BailoutCash < 0) p.Add("endings.bailoutCash is negative");
            Permille(p, "endings.exitFleetPermille", c.Endings.ExitFleetPermille);
            Permille(p, "endings.handcuffsFinePermille", c.Endings.HandcuffsFinePermille);
            if (c.Endings.Frontier.BaseCost < 1) p.Add("endings.frontier.baseCost must be at least 1");
            if (c.Endings.Frontier.MaxLevel < 1) p.Add("endings.frontier.maxLevel must be at least 1");
            if (c.Endings.Museum.PerLine < 1) p.Add("endings.museum.perLine must be at least 1");
            SuperintelligenceConfig si = c.Endings.Superintelligence;
            if (si != null)
            {
                if (si.ReleasedCapabilityMin < 1) p.Add("endings.superintelligence.releasedCapabilityMin must be at least 1");
                Permille(p, "endings.superintelligence.paperclipMaxAlignment", si.PaperclipMaxAlignment);
                Permille(p, "endings.superintelligence.longSummerMinAlignment", si.LongSummerMinAlignment);
                if (si.LongSummerMinFrontier < 1) p.Add("endings.superintelligence.longSummerMinFrontier must be at least 1");
            }
        }

        static void ShadyProblems(List<string> p, GameConfig c)
        {
            if (c.Shady == null) { p.Add("shady is missing"); return; }
            UniqueIds(p, "shady.cards", c.Shady.Cards);
            foreach (ShadyCardConfig card in c.Shady.Cards)
            {
                Permille(p, "shady card " + card.Id + " successPermille", card.SuccessPermille);
                Permille(p, "shady card " + card.Id + " tracePermille", card.TracePermille);
                if (card.Weeks < 1) p.Add("shady card " + card.Id + " needs at least one week");
            }
            Permille(p, "shady.launderKeepPermille", c.Shady.LaunderKeepPermille);
            if (c.Shady.LaunderHeatPer < 1) p.Add("shady.launderHeatPer must be at least 1");
            Permille(p, "shady.auditInvestigationPermille", c.Shady.AuditInvestigationPermille);
        }

        static void BubbleProblems(List<string> p, GameConfig c)
        {
            BubbleConfig b = c.Bubble;
            if (b == null) { p.Add("bubble is missing"); return; }
            Permille(p, "bubble.decayPermillePerMonth", b.DecayPermillePerMonth);
            Permille(p, "bubble.popAtPermille", b.PopAtPermille);
            Permille(p, "bubble.listedChipBoostPermille", b.ListedChipBoostPermille);
            Permille(p, "bubble.defaultFundingBoostPermille", b.DefaultFundingBoostPermille);
            Permille(p, "bubble.weakCashHaircutPermille", b.WeakCashHaircutPermille);
            if (b.SurvivorRevenuePpm < 0) p.Add("bubble.survivorRevenuePpm is negative");
            if (!string.IsNullOrEmpty(b.PopWorldEventId) && c.WorldEvent(b.PopWorldEventId) == null)
                p.Add("bubble.popWorldEventId names unknown world event \"" + b.PopWorldEventId + "\"");
            Permille(p, "bubble.hotPermille", b.HotPermille);
            if (b.GraceMonthsBeforeCrash < 0) p.Add("bubble.graceMonthsBeforeCrash is negative");
            Permille(p, "bubble.returnsNeededPermille", b.ReturnsNeededPermille);
            Permille(p, "bubble.returnCoolingPermille", b.ReturnCoolingPermille);
            if (b.CrashDurationMonths < 0) p.Add("bubble.crashDurationMonths is negative");
            Permille(p, "bubble.postCrashResidualPermille", b.PostCrashResidualPermille);
            Permille(p, "bubble.crashCustomerSpendPermille", b.CrashCustomerSpendPermille);
            Permille(p, "bubble.crashFundingCashPermille", b.CrashFundingCashPermille);
            Permille(p, "bubble.crashGpuPricePermille", b.CrashGpuPricePermille);
            if (b.CrashFloatCutPpm < 0) p.Add("bubble.crashFloatCutPpm is negative");
        }

        static void AcquisitionProblems(List<string> p, GameConfig c)
        {
            AcquisitionConfig a = c.Acquisitions;
            if (a == null) { p.Add("acquisitions is missing"); return; }
            if (a.CapabilityValuePerPoint < 0) p.Add("acquisitions.capabilityValuePerPoint is negative");
            if (a.StaffValuePerHead < 0) p.Add("acquisitions.staffValuePerHead is negative");
            Permille(p, "acquisitions.fleetResalePermille", a.FleetResalePermille);
            Permille(p, "acquisitions.integrationCostPermille", a.IntegrationCostPermille);
            Permille(p, "acquisitions.distressPricePermille", a.DistressPricePermille);
            Permille(p, "acquisitions.crashDistressPricePermille", a.CrashDistressPricePermille);
            if (a.MinInstalledModels < 0) p.Add("acquisitions.minInstalledModels is negative");
            if (a.RegulatorDealRevenue < 0) p.Add("acquisitions.regulatorDealRevenue is negative");
            if (a.RegulatorAttention < 0) p.Add("acquisitions.regulatorAttention is negative");
        }

        static void ModelSalesProblems(List<string> p, GameConfig c)
        {
            ModelSalesConfig m = c.ModelSales;
            if (m == null) { p.Add("modelSales is missing"); return; }
            Permille(p, "modelSales.unprofitableSalePermille", m.UnprofitableSalePermille);
            if (m.EarningsMultipleWeeks < 0) p.Add("modelSales.earningsMultipleWeeks is negative");
            if (m.SectorReputationHit < 0) p.Add("modelSales.sectorReputationHit is negative");
            if (m.MinOffer < 0) p.Add("modelSales.minOffer is negative");
        }

        static void SharesProblems(List<string> p, GameConfig c)
        {
            SharesConfig s = c.Shares;
            if (s == null) return;
            if (s.RivalStartPublicFloatPpm < 0 || s.RivalMaxPublicFloatPpm < s.RivalStartPublicFloatPpm)
                p.Add("shares rival public float bounds are out of order");
            Permille(p, "shares.ipoWhenBubbleAtPermille", s.IpoWhenBubbleAtPermille);
            if (s.IpoStepPpm < 0) p.Add("shares.ipoStepPpm is negative");
            Permille(p, "shares.ipoBubbleBoostPermille", s.IpoBubbleBoostPermille);
        }

        static void RegulatorsProblems(List<string> p, GameConfig c)
        {
            RegulatorsConfig r = c.Regulators;
            if (r == null) return;
            Permille(p, "regulators.bubbleScrutinyAtPermille", r.BubbleScrutinyAtPermille);
            if (r.ScrutinyRevenuePpm < 0) p.Add("regulators.scrutinyRevenuePpm is negative");
            if (r.ScrutinyAttentionPerMonth < 0) p.Add("regulators.scrutinyAttentionPerMonth is negative");
            if (r.MonopolyRegulatorAttention < 0) p.Add("regulators.monopolyRegulatorAttention is negative");
        }

        static void ComputeProblems(List<string> p, GameConfig c)
        {
            ComputeConfig compute = c.Compute;
            Permille(p, "compute.resalePermille", compute.ResalePermille);
            Permille(p, "compute.generationDecayPermille", compute.GenerationDecayPermille);
            Permille(p, "compute.reservedRentPermille", compute.ReservedRentPermille);
            if (compute.ReservedContractWeeks < 1) p.Add("compute.reservedContractWeeks must be at least 1");
            if (compute.ReservedDepositWeeks < 0) p.Add("compute.reservedDepositWeeks is negative");
            if (compute.MoveCostPerUnit < 0) p.Add("compute.moveCostPerUnit is negative");
            UniqueIds(p, "compute.gpuGenerations", compute.GpuGenerations);
            if (compute.GpuGenerations.Count == 0) { p.Add("compute.gpuGenerations is empty"); return; }
            if (compute.GpuGenerations[0].OpensYear > c.Calendar.StartYear) p.Add("the first GPU generation opens in " + compute.GpuGenerations[0].OpensYear + ", after the game starts in " + c.Calendar.StartYear);

            for (int i = 0; i < compute.GpuGenerations.Count; i++)
            {
                GpuGenerationConfig g = compute.GpuGenerations[i];
                string who = "gpu generation " + g.Id;
                if (g.ComputePerGpuWeek < 1) p.Add(who + " needs computePerGpuWeek of at least 1");
                if (g.RentPerGpuWeek < 0 || g.UpkeepPerGpuWeek < 0 || g.BuyPrice < 0) p.Add(who + " has a negative price");
                if (g.KwPerGpu < 1) p.Add(who + " needs kwPerGpu of at least 1");
                if (g.Tier != i) p.Add(who + " has tier " + g.Tier + " but is number " + i + " in the list: tiers count up from 0 in order");
                if (i == 0) continue;
                GpuGenerationConfig before = compute.GpuGenerations[i - 1];
                if (g.OpensYear <= before.OpensYear) p.Add(who + " does not open after " + before.Id);
                if (g.ComputePerGpuWeek <= before.ComputePerGpuWeek) p.Add(who + " is not faster than " + before.Id);
            }
        }

        static void RiskClassProblems(List<string> p, GameConfig c)
        {
            SafetyConfig safety = c.Safety;
            if (safety.GenericRiskWeight < 1 || safety.GenericRiskWeight > 100000) p.Add("safety.genericRiskWeight must be from 1 to 100000: the era's plain mishap always stays possible");
            UniqueIds(p, "safety.riskClasses", safety.RiskClasses);
            for (int i = 0; i < safety.RiskClasses.Count; i++)
            {
                RiskClassConfig r = safety.RiskClasses[i];
                string who = "risk class " + r.Id;
                if (string.IsNullOrEmpty(r.Name)) p.Add(who + " has no name");
                EraConfig opens = c.Era(r.Era);
                if (opens == null) p.Add(who + " opens in the unknown era \"" + r.Era + "\"");
                else if (c.Eras.IndexOf(opens) == 0) p.Add(who + " opens in the first era: the first era's trouble is the plain mishap");
                if (r.Weight < 1 || r.Weight > 100000) p.Add(who + " needs a weight from 1 to 100000");
                if (r.FinePermille < 250 || r.FinePermille > 10000) p.Add(who + " has finePermille " + r.FinePermille + ": it must be from 250 to 10000");
                if (r.ReputationPermille < 250 || r.ReputationPermille > 10000) p.Add(who + " has reputationPermille " + r.ReputationPermille + ": it must be from 250 to 10000");
                if (r.Headlines.Count == 0) p.Add(who + " has no headlines");
                foreach (string h in r.Headlines)
                    if (h.IndexOf("{model}", StringComparison.Ordinal) < 0) p.Add(who + " has a headline without {model}: \"" + h + "\"");
                foreach (string sectorId in r.Sectors)
                    if (c.Sector(sectorId) == null) p.Add(who + " bites the unknown sector \"" + sectorId + "\"");
                if (i > 0 && opens != null)
                {
                    EraConfig before = c.Era(safety.RiskClasses[i - 1].Era);
                    if (before != null && c.Eras.IndexOf(opens) < c.Eras.IndexOf(before)) p.Add(who + " is listed before a class that opens in an earlier era: list them oldest first");
                }
            }
        }

        static void SectorFlavourProblems(List<string> p, GameConfig c)
        {
            SafetyConfig safety = c.Safety;
            UniqueIds(p, "safety.sectorFlavours", safety.SectorFlavours);
            Permille(p, "safety.overloadIncidentPermille", safety.OverloadIncidentPermille);
            for (int i = 0; i < safety.SectorFlavours.Count; i++)
            {
                SectorFlavourConfig f = safety.SectorFlavours[i];
                string who = "sector flavour " + f.Id;
                if (c.Sector(f.Sector) == null) p.Add(who + " belongs to the unknown sector \"" + f.Sector + "\"");
                for (int k = 0; k < i; k++)
                    if (safety.SectorFlavours[k].Sector == f.Sector) p.Add(who + " is the second flavour of sector " + f.Sector);
                if (string.IsNullOrEmpty(f.Failure)) p.Add(who + " has no failure line");
                if (f.FinePermille < 250 || f.FinePermille > 5000) p.Add(who + " has finePermille " + f.FinePermille + ": it must be from 250 to 5000");
                if (f.ReputationPermille < 250 || f.ReputationPermille > 5000) p.Add(who + " has reputationPermille " + f.ReputationPermille + ": it must be from 250 to 5000");
                if (f.TrustPermille < 250 || f.TrustPermille > 5000) p.Add(who + " has trustPermille " + f.TrustPermille + ": it must be from 250 to 5000");
                if (f.MakeGoodWeeks < 0 || f.MakeGoodWeeks > 26) p.Add(who + " has makeGoodWeeks " + f.MakeGoodWeeks + ": it must be from 0 to 26");
                if (f.GlitchWeeks < 0 || f.GlitchWeeks > 52) p.Add(who + " has glitchWeeks " + f.GlitchWeeks + ": it must be from 0 to 52 (0 = the lab-wide value)");
                if (f.GlitchQualityPermille < 0 || f.GlitchQualityPermille > MathI.Permille) p.Add(who + " has glitchQualityPermille " + f.GlitchQualityPermille + ": it must be from 0 to 1000 (0 = the lab-wide value)");
            }
        }

        static void RolePerkProblems(List<string> p, GameConfig c)
        {
            foreach (RolePerkConfig perk in c.RolePerks)
            {
                string who = "rolePerk " + perk.Id;
                if (c.Role(perk.RoleId) == null) p.Add(who + " names the unknown role \"" + perk.RoleId + "\"");
                if (perk.PerStaffPermille < 1 || perk.PerStaffPermille > 1000) p.Add(who + " needs perStaffPermille from 1 to 1000");
                if (perk.CapPermille < perk.PerStaffPermille || perk.CapPermille > 900) p.Add(who + " needs a cap from perStaffPermille up to 900, so one person always counts and a fee or chance never reaches zero");
                if (string.IsNullOrEmpty(perk.Blurb)) p.Add(who + " has no blurb");
                foreach (string sector in perk.Sectors)
                    if (c.Sector(sector) == null) p.Add(who + " names the unknown sector \"" + sector + "\"");
            }
        }

        static void TalentProblems(List<string> p, GameConfig c)
        {
            TalentConfig t = c.Talent;
            if (t == null) { p.Add("talent is missing"); return; }
            if (t.OfferWeeks < 0 || t.OfferWeeks > 52) p.Add("talent.offerWeeks needs a value from 0 to 52");
            if (t.MaxOpenOffers < 1) p.Add("talent.maxOpenOffers needs at least 1");
            if (t.PackageMultiple < 1) p.Add("talent.packageMultiple needs at least 1");
            if (t.StarPackagePermille < MathI.Permille) p.Add("talent.starPackagePermille must be at least 1000: a star never costs less than anyone else");
            Permille(p, "talent.minSuccessPermille", t.MinSuccessPermille);
            if (t.MinSuccessPermille < 1) p.Add("talent.minSuccessPermille needs at least 1 so no answer is hopeless");
            if (t.PoachBackFeePermille < 0 || t.PoachBackFeePermille > 5000) p.Add("talent.poachBackFeePermille needs a value from 0 to 5000");
            Permille(p, "talent.poachBackBasePermille", t.PoachBackBasePermille);
            if (t.PoachBackMoraleSlopePermille < -2000 || t.PoachBackMoraleSlopePermille > 2000) p.Add("talent.poachBackMoraleSlopePermille needs a value from -2000 to 2000");
            if (t.PoachBackReputationSlopePermille < -2000 || t.PoachBackReputationSlopePermille > 2000) p.Add("talent.poachBackReputationSlopePermille needs a value from -2000 to 2000");
            if (t.PoachBackFeelingHit < 0 || t.PoachBackFailFeelingHit < 0) p.Add("talent.poachBackFeelingHit and poachBackFailFeelingHit cannot be negative");
            if (t.Counters.Count == 0) p.Add("talent.counters is empty: an offer would have no answer");
            UniqueIds(p, "talent.counters", t.Counters);
            foreach (CounterConfig k in t.Counters)
            {
                string who = "counter " + k.Id;
                if (string.IsNullOrEmpty(k.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(k.Blurb)) p.Add(who + " has no blurb");
                if (k.CostPermille < 0 || k.CostPermille > 3000) p.Add(who + " needs costPermille from 0 to 3000");
                Permille(p, who + " baseSuccessPermille", k.BaseSuccessPermille);
                if (k.MoraleSlopePermille < -2000 || k.MoraleSlopePermille > 2000) p.Add(who + " needs moraleSlopePermille from -2000 to 2000");
                if (k.ReputationSlopePermille < -2000 || k.ReputationSlopePermille > 2000) p.Add(who + " needs reputationSlopePermille from -2000 to 2000");
                if (k.MoraleGain < 0 || k.MoraleGain > 500) p.Add(who + " needs moraleGain from 0 to 500");
            }
        }

        static void ChatProblems(List<string> p, GameConfig c)
        {
            ChatConfig h = c.Chat;
            if (h == null) { p.Add("chat is missing"); return; }
            if (h.DefaultLines < 1 || h.DefaultLines > 100) p.Add("chat.defaultLines must be from 1 to 100");
            if (h.Lines.Count == 0) p.Add("chat.lines is empty");
            UniqueIds(p, "chat.lines", h.Lines);
            for (int i = 0; i < h.Lines.Count; i++)
            {
                ChatLineConfig l = h.Lines[i];
                string who = "chat line " + l.IdValue;
                if (!AiLabs.Rules.Systems.CompanyChat.IsEventKind(l.Kind)) p.Add(who + " is about the unknown event kind \"" + l.Kind + "\"");
                if (string.IsNullOrWhiteSpace(l.Text)) p.Add(who + " has no text");
                if (l.Pick < -1 || l.Pick > 1) p.Add(who + " needs pick -1, 0 or 1");
                if (l.FromYear < 0 || l.ToYear < 0 || (l.ToYear > 0 && l.FromYear > l.ToYear)) p.Add(who + " has years that do not make a range (" + l.FromYear + " to " + l.ToYear + ")");
                if (l.About != "lab" && l.About != "subject") p.Add(who + " needs about \"lab\" or \"subject\"");
                foreach (string role in l.Roles)
                    if (c.Role(role) == null) p.Add(who + " names the unknown role \"" + role + "\"");
                if (l.Trait.Length > 0 && c.StaffPeople.Trait(l.Trait) == null) p.Add(who + " names the unknown trait \"" + l.Trait + "\"");
            }
        }

        static void GlowProblems(List<string> p, GameConfig c)
        {
            GlowConfig g = c.Glow;
            if (g == null) { p.Add("glow is missing"); return; }
            if (g.FullAtCapability < 1) p.Add("glow.fullAtCapability must be at least 1");
            if (g.Stages.Count == 0) p.Add("glow.stages is empty");
            UniqueIds(p, "glow.stages", g.Stages);
            for (int i = 0; i < g.Stages.Count; i++)
            {
                GlowStageConfig s = g.Stages[i];
                string who = "glow stage " + s.IdValue;
                if (string.IsNullOrEmpty(s.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(s.Sign)) p.Add(who + " has no sign");
                if (s.MinChancePermille < -1 || s.MinChancePermille > MathI.Permille) p.Add(who + " needs minChancePermille from -1 to 1000");
                if (s.MinRung < 0 || s.MinRung > 6) p.Add(who + " needs minRung from 0 to 6");
                if (s.MinChancePermille < 0 && s.MinRung == 0) p.Add(who + " never applies: it needs a chance or a rung");
                if (i == 0 && s.MinChancePermille != 0) p.Add("the first glow stage must apply at a chance of 0 so every model has one");
                if (i > 0 && s.MinChancePermille >= 0 && g.Stages[i - 1].MinChancePermille >= 0 && s.MinChancePermille <= g.Stages[i - 1].MinChancePermille)
                    p.Add(who + " does not start above the chance of the stage before it");
                if (i > 0 && s.MinRung > 0 && g.Stages[i - 1].MinRung > 0 && s.MinRung <= g.Stages[i - 1].MinRung)
                    p.Add(who + " does not start above the rung of the stage before it");
            }
        }

        static void NewsProblems(List<string> p, GameConfig c)
        {
            NewsConfig n = c.News;
            if (n == null) { p.Add("news is missing"); return; }
            if (n.DefaultHeadlines < 1 || n.DefaultHeadlines > 100) p.Add("news.defaultHeadlines must be from 1 to 100");
            if (n.MinHeadlines < 0 || n.MinHeadlines > n.DefaultHeadlines) p.Add("news.minHeadlines must be from 0 to news.defaultHeadlines");
            if (n.MinHeadlines > 0 && n.Filler.Count == 0) p.Add("news.minHeadlines needs some news.filler to pad with");
            UniqueIds(p, "news.headlines", n.Headlines);
            UniqueIds(p, "news.filler", n.Filler);
            for (int i = 0; i < n.Headlines.Count; i++)
            {
                HeadlineConfig h = n.Headlines[i];
                string who = "headline " + h.IdValue;
                if (!AiLabs.Rules.Systems.CompanyChat.IsEventKind(h.Kind)) p.Add(who + " is about the unknown event kind \"" + h.Kind + "\"");
                if (string.IsNullOrWhiteSpace(h.Text)) p.Add(who + " has no text");
                if (h.Pick < -1 || h.Pick > 1) p.Add(who + " needs pick -1, 0 or 1");
                UnknownPlaceholders(p, who, h.Text);
            }
            for (int i = 0; i < n.Filler.Count; i++)
            {
                FillerConfig f = n.Filler[i];
                string who = "filler " + f.IdValue;
                if (string.IsNullOrWhiteSpace(f.Text)) p.Add(who + " has no text");
                if (f.Text != null && f.Text.IndexOf('{') >= 0) p.Add(who + " has a placeholder, and filler is not about an event");
                if (f.FromYear < 1000 || f.FromYear > 3000) p.Add(who + " needs a fromYear from 1000 to 3000");
                if (f.ToYear != 0 && f.ToYear < f.FromYear) p.Add(who + " ends (" + f.ToYear + ") before it starts (" + f.FromYear + ")");
            }
        }

        static void UnknownPlaceholders(List<string> p, string who, string text)
        {
            if (text == null) return;
            int at = 0;
            while ((at = text.IndexOf('{', at)) >= 0)
            {
                int end = text.IndexOf('}', at);
                string tag = end < 0 ? text.Substring(at) : text.Substring(at, end - at + 1);
                if (Array.IndexOf(AiLabs.Rules.Systems.NewsTicker.Placeholders, tag) < 0) p.Add(who + " has the unknown placeholder " + tag);
                if (end < 0) break;
                at = end + 1;
            }
        }

        static void RunsProblems(List<string> p, GameConfig c)
        {
            RunsConfig r = c.Runs;
            if (r == null) { p.Add("runs is missing"); return; }
            UniqueIds(p, "runs.approaches", r.Approaches);
            UniqueIds(p, "runs.checkpoints", r.Checkpoints);
            if (r.EngineerSavingPermillePerStaff < 0) p.Add("runs.engineerSavingPermillePerStaff must be non-negative");
            if (r.EngineerSavingCapPermille < 0 || r.EngineerSavingCapPermille > MathI.Permille) p.Add("runs.engineerSavingCapPermille must be from 0 to 1000");
            ApproachConfig fallbackApproach = r.Approach(r.DefaultApproachId);
            if (fallbackApproach == null) p.Add("runs.defaultApproachId \"" + r.DefaultApproachId + "\" is not an approach");
            else if (fallbackApproach.CapabilityPermille != 0 || fallbackApproach.FailurePermillePerWeek != 0)
                p.Add("the default approach \"" + fallbackApproach.IdValue + "\" must change nothing and never fail");
            CheckpointConfig fallbackCheckpoint = r.Checkpoint(r.DefaultCheckpointId);
            if (fallbackCheckpoint == null) p.Add("runs.defaultCheckpointId \"" + r.DefaultCheckpointId + "\" is not a checkpoint habit");
            else if (fallbackCheckpoint.WeeklyCostPermille != 0) p.Add("the default checkpoint habit \"" + fallbackCheckpoint.IdValue + "\" must be free");
            for (int i = 0; i < r.Approaches.Count; i++)
            {
                ApproachConfig a = r.Approaches[i];
                string who = "approach " + a.IdValue;
                if (string.IsNullOrEmpty(a.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(a.Text)) p.Add(who + " has no text");
                if (a.CapabilityPermille <= -MathI.Permille || a.CapabilityPermille > 5000) p.Add(who + " capabilityPermille must be above -1000 and up to 5000");
                if (a.FailurePermillePerWeek < 0 || a.FailurePermillePerWeek > MathI.Permille) p.Add(who + " failurePermillePerWeek must be from 0 to 1000");
            }
            for (int i = 0; i < r.Checkpoints.Count; i++)
            {
                CheckpointConfig k = r.Checkpoints[i];
                string who = "checkpoint habit " + k.IdValue;
                if (string.IsNullOrEmpty(k.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(k.Text)) p.Add(who + " has no text");
                if (k.LossPermille < 0 || k.LossPermille > MathI.Permille) p.Add(who + " lossPermille must be from 0 to 1000");
                if (k.WeeklyCostPermille < 0 || k.WeeklyCostPermille > 5000) p.Add(who + " weeklyCostPermille must be from 0 to 5000");
            }
        }

        static void CampaignProblems(List<string> p, GameConfig c)
        {
            CampaignConfig m = c.Campaign;
            if (m == null) { p.Add("campaign is missing"); return; }
            if (m.AllMetBonus < 0) p.Add("campaign.allMetBonus must be non-negative");
            UniqueIds(p, "campaign.goals", m.Goals);
            for (int i = 0; i < m.Goals.Count; i++)
            {
                EraGoalConfig g = m.Goals[i];
                string who = "campaign goal " + g.IdValue;
                if (c.Era(g.EraId) == null) p.Add(who + " is for the unknown era \"" + g.EraId + "\"");
                if (string.IsNullOrEmpty(g.Name)) p.Add(who + " has no name");
                if (g.Target < 1) p.Add(who + " has a target under 1");
                if (g.Points < 1) p.Add(who + " is worth under 1 point");
                if ((g.Metric == GoalMetric.Reputation || g.Metric == GoalMetric.BestAlignment) && g.Target > MathI.Permille) p.Add(who + " asks for more than 1000 of a 0 to 1000 meter");
            }
            for (int e = 0; e < c.Eras.Count; e++)
                if (m.GoalsOf(c.Eras[e].Id).Count == 0) p.Add("era " + c.Eras[e].Id + " has no campaign goals");
        }

        static void PivotProblems(List<string> p, GameConfig c)
        {
            PivotConfig v = c.Pivot;
            if (v == null) { p.Add("pivot is missing"); return; }
            if (v.MinWeekCost < 0) p.Add("pivot.minWeekCost must be non-negative");
            if (v.CostWeeks < 1 || v.CostWeeks > 520) p.Add("pivot.costWeeks must be from 1 to 520");
            if (v.StaffLossPermille < 0 || v.StaffLossPermille > MathI.Permille) p.Add("pivot.staffLossPermille must be from 0 to 1000");
            if (v.ConfusionWeeks < 1 || v.ConfusionWeeks > 520) p.Add("pivot.confusionWeeks must be from 1 to 520");
            if (v.ConfusionAppealPermille < 0 || v.ConfusionAppealPermille >= MathI.Permille) p.Add("pivot.confusionAppealPermille must be from 0 to 999");
        }

        static void ScenarioProblems(List<string> p, GameConfig c)
        {
            ScenariosConfig sc = c.Scenarios;
            if (sc == null) { p.Add("scenarios is missing"); return; }
            UniqueIds(p, "scenarios", sc.Scenarios);
            for (int i = 0; i < sc.Scenarios.Count; i++)
            {
                ScenarioConfig s = sc.Scenarios[i];
                string who = "scenario " + s.IdValue;
                if (string.IsNullOrEmpty(s.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(s.Blurb)) p.Add(who + " has no blurb");
                EraConfig era = c.Era(s.EraId);
                if (era == null) p.Add(who + " starts in the unknown era \"" + s.EraId + "\"");
                else if (!era.Startable) p.Add(who + " starts in " + era.Name + ", which is not a starting era");
                if (!string.IsNullOrEmpty(s.FounderId) && c.Founder(s.FounderId) == null) p.Add(who + " has the unknown founder \"" + s.FounderId + "\"");
                if (!string.IsNullOrEmpty(s.StyleId) && c.Style(s.StyleId) == null) p.Add(who + " has the unknown style \"" + s.StyleId + "\"");
                if (!string.IsNullOrEmpty(s.HomeBlocId) && c.World.Bloc(s.HomeBlocId) == null) p.Add(who + " has the unknown home bloc \"" + s.HomeBlocId + "\"");
                for (int k = 0; k < s.RivalIds.Count; k++)
                    if (c.Rival(s.RivalIds[k]) == null) p.Add(who + " has the unknown rival \"" + s.RivalIds[k] + "\"");
            }
        }

        static void LoanProblems(List<string> p, GameConfig c)
        {
            LoansConfig l = c.Loans;
            if (l == null) { p.Add("loans is missing"); return; }
            if (l.LoanToValuePermille < 1 || l.LoanToValuePermille > MathI.Permille) p.Add("loans.loanToValuePermille must be from 1 to 1000");
            if (l.TermWeeks < 1 || l.TermWeeks > 520) p.Add("loans.termWeeks must be from 1 to 520");
            if (l.InterestPermille < 0 || l.InterestPermille > 2000) p.Add("loans.interestPermille must be from 0 to 2000");
            if (l.MinPrincipal < 1) p.Add("loans.minPrincipal must be at least 1");
            if (l.MaxOpen < 1 || l.MaxOpen > 20) p.Add("loans.maxOpen must be from 1 to 20");
            if (l.MissPenaltyPermille < 0 || l.MissPenaltyPermille > MathI.Permille) p.Add("loans.missPenaltyPermille must be from 0 to 1000");
            if (l.DefaultAfterMisses < 1 || l.DefaultAfterMisses > 52) p.Add("loans.defaultAfterMisses must be from 1 to 52");
        }

        static void LobbyProblems(List<string> p, GameConfig c)
        {
            LobbyConfig l = c.Lobbying;
            if (l == null) { p.Add("lobbying is missing"); return; }
            if (l.CooldownWeeks < 0 || l.CooldownWeeks > 520) p.Add("lobbying.cooldownWeeks must be from 0 to 520");
            if (l.MinWeekCost < 0) p.Add("lobbying.minWeekCost must be non-negative");
            if (l.QuietCostWeeks < 1 || l.QuietCostWeeks > 520) p.Add("lobbying.quietCostWeeks must be from 1 to 520");
            if (l.QuietAttention < 1 || l.QuietAttention > MathI.Permille) p.Add("lobbying.quietAttention must be from 1 to 1000");
            if (l.SubsidyCostWeeks < 1 || l.SubsidyCostWeeks > 520) p.Add("lobbying.subsidyCostWeeks must be from 1 to 520");
            if (l.SubsidyReturnWeeks < 1 || l.SubsidyReturnWeeks > 520) p.Add("lobbying.subsidyReturnWeeks must be from 1 to 520");
            Permille(p, "lobbying.subsidyChancePermille", l.SubsidyChancePermille);
            Permille(p, "lobbying.subsidyChanceCapPermille", l.SubsidyChanceCapPermille);
            if (l.SubsidyChancePermillePerStaff < 0) p.Add("lobbying.subsidyChancePermillePerStaff must be non-negative");
            if (l.SubsidyChanceCapPermille < l.SubsidyChancePermille) p.Add("lobbying.subsidyChanceCapPermille must not be under subsidyChancePermille");
            Permille(p, "lobbying.backfirePermille", l.BackfirePermille);
            Permille(p, "lobbying.backfireFloorPermille", l.BackfireFloorPermille);
            if (l.BackfireSavingPermillePerStaff < 0) p.Add("lobbying.backfireSavingPermillePerStaff must be non-negative");
            if (l.BackfireFloorPermille > l.BackfirePermille) p.Add("lobbying.backfireFloorPermille must not be over backfirePermille");
            if (l.BackfireAttention < 0 || l.BackfireAttention > MathI.Permille) p.Add("lobbying.backfireAttention must be from 0 to 1000");
            if (l.BackfireReputation < 0 || l.BackfireReputation > MathI.Permille) p.Add("lobbying.backfireReputation must be from 0 to 1000");
        }

        static void RushProblems(List<string> p, GameConfig c)
        {
            RushConfig r = c.Rush;
            if (r == null) { p.Add("rush is missing"); return; }
            if (r.MaxWeeks < 1 || r.MaxWeeks > 52) p.Add("rush.maxWeeks must be from 1 to 52");
            if (r.LaunchWeeks < 1 || r.LaunchWeeks > 520) p.Add("rush.launchWeeks must be from 1 to 520");
            if (r.IncidentWeeks < 1 || r.IncidentWeeks > 520) p.Add("rush.incidentWeeks must be from 1 to 520");
            if (r.LaunchAppealPermillePerWeek < 0 || (long)r.LaunchAppealPermillePerWeek * r.MaxWeeks > 5000) p.Add("rush.launchAppealPermillePerWeek times maxWeeks must be from 0 to 5000");
            if (r.IncidentPermillePerWeek < 0 || (long)r.IncidentPermillePerWeek * r.MaxWeeks > MathI.Permille) p.Add("rush.incidentPermillePerWeek times maxWeeks must be from 0 to 1000");
            if (r.CulturePointsPerWeek < 0 || (long)r.CulturePointsPerWeek * r.MaxWeeks > MathI.Permille) p.Add("rush.culturePointsPerWeek times maxWeeks must be from 0 to 1000");
        }

        static void SuccessorProblems(List<string> p, GameConfig c)
        {
            SuccessorConfig s = c.Successor;
            if (s == null) { p.Add("successor is missing"); return; }
            if (s.ComputeSavedPermille < 0 || s.ComputeSavedPermille >= MathI.Permille) p.Add("successor.computeSavedPermille must be from 0 to 999");
            if (s.CapabilityPermille < 0 || s.CapabilityPermille > 5000) p.Add("successor.capabilityPermille must be from 0 to 5000");
            if (s.AlignmentPoints < 0 || s.AlignmentPoints > MathI.Permille) p.Add("successor.alignmentPoints must be from 0 to 1000");
            if (s.ChainPoints < 0 || s.ChainPoints > MathI.Permille) p.Add("successor.chainPoints must be from 0 to 1000");
            if (s.MaxChain < 0 || s.MaxChain > 20) p.Add("successor.maxChain must be from 0 to 20");
            if (s.CulturePoints < 0 || s.CulturePoints > MathI.Permille) p.Add("successor.culturePoints must be from 0 to 1000");
            if (s.HelperMinPermille < 0 || s.HelperMinPermille > 5000) p.Add("successor.helperMinPermille must be from 0 to 5000");
        }

        static void ObjectiveProblems(List<string> p, GameConfig c)
        {
            ObjectivesConfig o = c.Objectives;
            if (o == null) { p.Add("objectives is missing"); return; }
            UniqueIds(p, "objectives", o.Objectives);
            ObjectiveConfig fallback = o.Objective(o.DefaultId);
            if (fallback == null) p.Add("objectives.defaultId \"" + o.DefaultId + "\" is not an objective");
            else if (fallback.AlignmentPoints != 0 || fallback.CapabilityPermille != 0 || fallback.AppealPermille != 0 || fallback.CulturePoints != 0)
                p.Add("the default objective \"" + fallback.IdValue + "\" must change nothing");
            for (int i = 0; i < o.Objectives.Count; i++)
            {
                ObjectiveConfig x = o.Objectives[i];
                string who = "objective " + x.IdValue;
                if (string.IsNullOrEmpty(x.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(x.Text)) p.Add(who + " has no text");
                if (x.AlignmentPoints < -1000 || x.AlignmentPoints > 1000) p.Add(who + " alignmentPoints must be from -1000 to 1000");
                if (x.CapabilityPermille <= -MathI.Permille || x.CapabilityPermille > 5000) p.Add(who + " capabilityPermille must be above -1000 and up to 5000");
                if (x.AppealPermille <= -MathI.Permille || x.AppealPermille > 5000) p.Add(who + " appealPermille must be above -1000 and up to 5000");
                if (x.CulturePoints < -1000 || x.CulturePoints > 1000) p.Add(who + " culturePoints must be from -1000 to 1000");
            }
        }

        static void OfficeProblems(List<string> p, GameConfig c)
        {
            OfficeConfig o = c.Office;
            if (o == null) { p.Add("office is missing"); return; }
            if (o.ChancePermille < 0 || o.ChancePermille > MathI.Permille) p.Add("office.chancePermille must be from 0 to 1000");
            if (o.FirstAfterWeeks < 0 || o.FirstAfterWeeks > 2000) p.Add("office.firstAfterWeeks must be from 0 to 2000");
            if (o.ExpireWeeks < 1 || o.ExpireWeeks > 520) p.Add("office.expireWeeks must be from 1 to 520");
            if (o.MinWeekCost < 0) p.Add("office.minWeekCost is negative");
            UniqueIds(p, "office.events", o.Events);
            for (int i = 0; i < o.Events.Count; i++)
            {
                OfficeEventConfig e = o.Events[i];
                string who = "office event " + e.IdValue;
                if (string.IsNullOrEmpty(e.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(e.Text)) p.Add(who + " has no text");
                if (e.FromYear < 0 || e.ToYear < 0 || (e.ToYear > 0 && e.FromYear > e.ToYear)) p.Add(who + " has years that do not make a range (" + e.FromYear + " to " + e.ToYear + ")");
                if (e.Choices.Count < 2) p.Add(who + " needs at least two choices");
                if (!string.IsNullOrEmpty(e.LapseChoice) && e.Choice(e.LapseChoice) == null) p.Add(who + " lapses into the unknown choice \"" + e.LapseChoice + "\"");
                UniqueIds(p, who + " choices", e.Choices);
                for (int k = 0; k < e.Choices.Count; k++)
                {
                    OfficeChoiceConfig ch = e.Choices[k];
                    string what = who + " choice " + ch.IdValue;
                    if (string.IsNullOrEmpty(ch.Text)) p.Add(what + " has no text");
                    if (string.IsNullOrEmpty(ch.Result)) p.Add(what + " has no result");
                    for (int f = 0; f < ch.Effects.Count; f++)
                        if (ch.Effects[f].Amount == 0 || ch.Effects[f].Amount < -1000 || ch.Effects[f].Amount > 1000) p.Add(what + " has an effect with an amount of " + ch.Effects[f].Amount + " (it must be from -1000 to 1000 and not 0)");
                }
            }
        }

        static void RepairProblems(List<string> p, GameConfig c)
        {
            RepairConfig r = c.Repair;
            if (r == null) { p.Add("repair is missing"); return; }
            if (r.FineTuneRevenueWeeks < 0 || r.FineTuneRevenueWeeks > 520) p.Add("repair.fineTuneRevenueWeeks must be from 0 to 520");
            if (r.FineTuneMinCost < 0) p.Add("repair.fineTuneMinCost is negative");
            if (r.FineTuneAlignment < 0 || r.FineTuneAlignment > MathI.Permille) p.Add("repair.fineTuneAlignment must be from 0 to 1000");
            if (r.RollbackReputationLoss < 0 || r.RollbackReputationLoss > MathI.Permille) p.Add("repair.rollbackReputationLoss must be from 0 to 1000");
        }

        static void RedTeamProblems(List<string> p, GameConfig c)
        {
            RedTeamConfig r = c.RedTeam;
            if (r == null) { p.Add("redTeam is missing"); return; }
            if (r.RoleId != "" && c.Role(r.RoleId) == null) p.Add("redTeam.roleId names the unknown role \"" + r.RoleId + "\"");
            if (r.MinStaff < 0 || r.MinStaff > 1000) p.Add("redTeam.minStaff must be from 0 to 1000");
            if (r.RevenueWeeks < 0 || r.RevenueWeeks > 520) p.Add("redTeam.revenueWeeks must be from 0 to 520");
            if (r.MinCost < 0) p.Add("redTeam.minCost is negative");
            if (r.Weeks < 0 || r.Weeks > 520) p.Add("redTeam.weeks must be from 0 to 520");
            if (r.IncidentCutPermille < 0 || r.IncidentCutPermille > MathI.Permille) p.Add("redTeam.incidentCutPermille must be from 0 to 1000");
            if (r.GapClosePermille < 0 || r.GapClosePermille > MathI.Permille) p.Add("redTeam.gapClosePermille must be from 0 to 1000");
            if (r.AlignmentGain < 0 || r.AlignmentGain > MathI.Permille) p.Add("redTeam.alignmentGain must be from 0 to 1000");
            if (r.CulturePoints < 0 || r.CulturePoints > 100) p.Add("redTeam.culturePoints must be from 0 to 100");
        }

        static void InterpProblems(List<string> p, GameConfig c)
        {
            InterpConfig i = c.Interp;
            if (i == null) { p.Add("interp is missing"); return; }
            if (i.MaxLevel > 10) p.Add("interp.levelTechs has more than 10 levels");
            foreach (string tech in i.LevelTechs) if (c.Tech(tech) == null) p.Add("interp.levelTechs names the unknown tech \"" + tech + "\"");
            if (i.WeeklyCostPerLevel < 0) p.Add("interp.weeklyCostPerLevel is negative");
            if (i.GapClosePermillePerLevel < 0 || i.GapClosePermillePerLevel > MathI.Permille) p.Add("interp.gapClosePermillePerLevel must be from 0 to 1000");
            if (i.ResearchCostPermillePerLevel < 0 || i.ResearchCostPermillePerLevel * (long)i.MaxLevel > 900) p.Add("interp.researchCostPermillePerLevel must be 0 or more, and every level together must leave at least a tenth of the research");
            if (i.StaleReliefPermillePerLevel < 0 || i.StaleReliefPermillePerLevel * (long)i.MaxLevel > MathI.Permille) p.Add("interp.staleReliefPermillePerLevel must be 0 or more, and every level together can take away at most all of the loss");
        }

        static void ComplianceProblems(List<string> p, GameConfig c)
        {
            ComplianceConfig k = c.Compliance;
            if (k == null) { p.Add("compliance is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("compliance.maxLevel must be from 0 to 10");
            if (k.StaffPerLevel < 0 || k.StaffPerLevel > 100) p.Add("compliance.staffPerLevel must be from 0 to 100");
            if (k.WeeklyCostPerLevel < 0) p.Add("compliance.weeklyCostPerLevel is negative");
            if (k.AttentionPermillePerLevel < 0 || k.AttentionPermillePerLevel * (long)k.MaxLevel > 900) p.Add("compliance.attentionPermillePerLevel must be 0 or more, and every level together must leave at least a tenth of the attention");
            if (k.AuditFinePermillePerLevel < 0 || k.AuditFinePermillePerLevel * (long)k.MaxLevel > MathI.Permille) p.Add("compliance.auditFinePermillePerLevel must be 0 or more, and every level together can take away at most the whole fine");
            if (k.HearingPointsPerLevel < 0 || k.HearingPointsPerLevel * (long)k.MaxLevel > 100) p.Add("compliance.hearingPointsPerLevel must be 0 or more, and every level together can add at most 100 points");
        }

        static void PrestigeProblems(List<string> p, GameConfig c)
        {
            PrestigeConfig k = c.Prestige;
            if (k == null) { p.Add("prestige is missing"); return; }
            if (k.Max < 0 || k.Max > 100000) p.Add("prestige.max must be from 0 to 100000");
            if (k.PaperPrestige < 0 || k.PaperPrestige > k.Max) p.Add("prestige.paperPrestige must be from 0 to the most");
            if (k.DecayPerWeek < 0 || k.DecayPerWeek > k.Max) p.Add("prestige.decayPerWeek must be from 0 to the most");
            if (k.PaperResearchWeeks < 0 || k.PaperResearchWeeks > 520) p.Add("prestige.paperResearchWeeks must be from 0 to 520");
            if (k.MinPoints < 0) p.Add("prestige.minPoints is negative");
            if (k.MinResearchers < 0 || k.MinResearchers > 1000) p.Add("prestige.minResearchers must be from 0 to 1000");
            if (k.PaperReputation < 0 || k.PaperReputation > 200) p.Add("prestige.paperReputation must be from 0 to 200");
            if (k.MoraleAtMax < 0 || k.MoraleAtMax > 300) p.Add("prestige.moraleAtMax must be from 0 to 300");
            if (k.HireDiscountAtMax < 0 || k.HireDiscountAtMax > 500) p.Add("prestige.hireDiscountAtMax must be from 0 to 500");
            if (k.PoachAtMax < 0 || k.PoachAtMax > 500) p.Add("prestige.poachAtMax must be from 0 to 500");
            if (k.CounterAtMax < 0 || k.CounterAtMax > 300) p.Add("prestige.counterAtMax must be from 0 to 300");
        }

        static void SubletProblems(List<string> p, GameConfig c)
        {
            SubletConfig k = c.Sublet;
            if (k == null) { p.Add("sublet is missing"); return; }
            if (k.MaxPermille < 0 || k.MaxPermille > 900) p.Add("sublet.maxPermille must be from 0 to 900, so the lab keeps at least a tenth of its compute");
            if (k.RevenuePermille < 0 || k.RevenuePermille > MathI.Permille) p.Add("sublet.revenuePermille must be from 0 to 1000, so subletting never beats renting out the same GPUs at full price");
            if (k.NoticeWeeks < 0 || k.NoticeWeeks > 520) p.Add("sublet.noticeWeeks must be from 0 to 520");
        }

        static void FocusProblems(List<string> p, GameConfig c)
        {
            ResearchFocusConfig k = c.Focus;
            if (k == null) { p.Add("focus is missing"); return; }
            if (k.DiscountPermille < 0 || k.DiscountPermille > 900) p.Add("focus.discountPermille must be from 0 to 900, so a tech never becomes free");
            if (k.PenaltyPermille < 0 || k.PenaltyPermille > 1000) p.Add("focus.penaltyPermille must be from 0 to 1000");
            if (k.SwitchCost < 0) p.Add("focus.switchCost is negative");
            if (k.LockWeeks < 0 || k.LockWeeks > 520) p.Add("focus.lockWeeks must be from 0 to 520");
            var seen = new HashSet<string>();
            foreach (FocusOptionConfig o in k.Options)
            {
                if (string.IsNullOrEmpty(o.Id) || !seen.Add(o.Id)) p.Add("focus option \"" + o.Id + "\" has no id or repeats one");
                if (string.IsNullOrEmpty(o.Name)) p.Add("focus option " + o.Id + " has no name");
                if (o.Kinds.Count == 0) p.Add("focus option " + o.Id + " covers no tech effect");
                foreach (string kind in o.Kinds)
                    if (!System.Enum.TryParse(kind, out TechEffectKind parsed) || !System.Enum.IsDefined(typeof(TechEffectKind), parsed)) p.Add("focus option " + o.Id + " names the unknown tech effect \"" + kind + "\"");
            }
        }

        static void RetrainProblems(List<string> p, GameConfig c)
        {
            RetrainConfig k = c.Retrain;
            if (k == null) { p.Add("retrain is missing"); return; }
            if (k.FeePermille < 0 || k.FeePermille > MathI.Permille) p.Add("retrain.feePermille must be from 0 to 1000, so retraining never costs more than hiring");
            if (k.SameFunctionFeePermille < 0 || k.SameFunctionFeePermille > k.FeePermille) p.Add("retrain.sameFunctionFeePermille must be from 0 to feePermille");
            if (k.MoraleDrop < 0 || k.MoraleDrop > MathI.Permille) p.Add("retrain.moraleDrop must be from 0 to 1000");
            if (k.SameFunctionMoraleDrop < 0 || k.SameFunctionMoraleDrop > k.MoraleDrop) p.Add("retrain.sameFunctionMoraleDrop must be from 0 to moraleDrop");
            if (k.MinMorale < 0 || k.MinMorale > MathI.Permille) p.Add("retrain.minMorale must be from 0 to 1000");
        }

        static void AbandonProblems(List<string> p, GameConfig c)
        {
            AbandonConfig k = c.Abandon;
            if (k == null) { p.Add("abandon is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("abandon.enabled must be 0 or 1");
            if (k.SalvagePermille < 0 || k.SalvagePermille > 500) p.Add("abandon.salvagePermille must be from 0 to 500, so scrapping a run never pays better than finishing it");
            if (k.ReputationAtFull < 0 || k.ReputationAtFull > 200) p.Add("abandon.reputationAtFull must be from 0 to 200");
            if (k.MoraleAtFull < 0 || k.MoraleAtFull > 500) p.Add("abandon.moraleAtFull must be from 0 to 500");
        }

        static void LayoffsProblems(List<string> p, GameConfig c)
        {
            LayoffsConfig k = c.Layoffs;
            if (k == null) { p.Add("layoffs is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("layoffs.enabled must be 0 or 1");
            if (k.MaxPermille < 0 || k.MaxPermille > 500) p.Add("layoffs.maxPermille must be from 0 to 500, so a round never empties the lab");
            if (k.SeverancePermille < 100 || k.SeverancePermille > MathI.Permille) p.Add("layoffs.severancePermille must be from 100 to 1000, so a round is cheaper than letting people go one by one but never free");
            if (k.ReputationAtAll < 0 || k.ReputationAtAll > 200) p.Add("layoffs.reputationAtAll must be from 0 to 200");
            if (k.MoraleAtAll < 0 || k.MoraleAtAll > 500) p.Add("layoffs.moraleAtAll must be from 0 to 500");
            if (k.DeepPermille < 1 || k.DeepPermille > MathI.Permille) p.Add("layoffs.deepPermille must be from 1 to 1000");
        }

        static void TechDebtProblems(List<string> p, GameConfig c)
        {
            TechDebtConfig k = c.TechDebt;
            if (k == null) { p.Add("techDebt is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("techDebt.enabled must be 0 or 1");
            if (k.RushPerWeek < 0 || k.RushPerWeek > 200) p.Add("techDebt.rushPerWeek must be from 0 to 200");
            if (k.CrunchPerWeek < 0 || k.CrunchPerWeek > 50) p.Add("techDebt.crunchPerWeek must be from 0 to 50");
            if (k.FineTunePatch < 0 || k.FineTunePatch > 200) p.Add("techDebt.fineTunePatch must be from 0 to 200");
            if (k.IncidentAtFull < 0 || k.IncidentAtFull > 200) p.Add("techDebt.incidentAtFull must be from 0 to 200");
            if (k.CostPerPoint < 1) p.Add("techDebt.costPerPoint must be at least 1, so debt is never free to clear");
            if (k.MinEngineers < 0 || k.MinEngineers > 50) p.Add("techDebt.minEngineers must be from 0 to 50");
            if (k.MaxPerCommand < 1 || k.MaxPerCommand > MathI.Permille) p.Add("techDebt.maxPerCommand must be from 1 to 1000");
        }

        static void SeniorityProblems(List<string> p, GameConfig c)
        {
            SeniorityConfig k = c.Seniority;
            if (k == null) { p.Add("seniority is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("seniority.enabled must be 0 or 1");
            if (k.WeightPermille < 1000 || k.WeightPermille > 3000) p.Add("seniority.weightPermille must be from 1000 to 3000, so a senior counts for at least one person and at most three");
            if (k.RaisePermille < 0 || k.RaisePermille > 2000) p.Add("seniority.raisePermille must be from 0 to 2000");
            if (k.RaisePermille < k.WeightPermille - 1000 - 400) p.Add("seniority.raisePermille must not fall more than 400 below the extra weight, so promoting is never a free gain");
            if (k.FeePermille < 0 || k.FeePermille > 10000) p.Add("seniority.feePermille must be from 0 to 10000");
            if (k.MaxSharePermille < 1 || k.MaxSharePermille > 500) p.Add("seniority.maxSharePermille must be from 1 to 500, so a function is never all seniors");
            if (k.MinMorale < 0 || k.MinMorale > MathI.Permille) p.Add("seniority.minMorale must be from 0 to 1000");
            if (k.MoraleBoost < 0 || k.MoraleBoost > 500) p.Add("seniority.moraleBoost must be from 0 to 500");
        }

        static void RevenueTargetsProblems(List<string> p, GameConfig c)
        {
            RevenueTargetsConfig k = c.RevenueTargets;
            if (k == null) { p.Add("revenueTargets is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 5) p.Add("revenueTargets.maxLevel must be from 0 to 5 (0 turns targets off)");
            if (k.Weeks < 4 || k.Weeks > 52) p.Add("revenueTargets.weeks must be from 4 to 52");
            if (k.StretchPerLevel < 0 || k.StretchPerLevel > 1000) p.Add("revenueTargets.stretchPerLevel must be from 0 to 1000");
            if (k.MinWeeklyRevenue < 1) p.Add("revenueTargets.minWeeklyRevenue must be at least 1, so there is something to measure against");
            if (k.RewardPerLevel < 0 || k.RewardPerLevel > 100) p.Add("revenueTargets.rewardPerLevel must be from 0 to 100");
            if (k.PenaltyPerLevel < k.RewardPerLevel || k.PenaltyPerLevel > 200) p.Add("revenueTargets.penaltyPerLevel must be from rewardPerLevel to 200, so a missed target costs at least what a met one earns");
        }

        static void RestructureProblems(List<string> p, GameConfig c)
        {
            RestructureConfig k = c.Restructure;
            if (k == null) { p.Add("restructure is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("restructure.enabled must be 0 or 1");
            if (k.StretchPermille < 1000 || k.StretchPermille > 5000) p.Add("restructure.stretchPermille must be from 1000 to 5000, so a restructure never shortens the term");
            if (k.FeePermille < 0 || k.FeePermille > 500) p.Add("restructure.feePermille must be from 0 to 500");
            if (k.ReputationLoss < 0 || k.ReputationLoss > 500) p.Add("restructure.reputationLoss must be from 0 to 500");
        }

        static void PulloutProblems(List<string> p, GameConfig c)
        {
            PulloutConfig k = c.Pullout;
            if (k == null) { p.Add("pullout is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("pullout.enabled must be 0 or 1");
            if (k.FeeWeeks < 0 || k.FeeWeeks > 52) p.Add("pullout.feeWeeks must be from 0 to 52");
            if (k.TrustDrop < 0 || k.TrustDrop > 1000) p.Add("pullout.trustDrop must be from 0 to 1000");
            if (k.ReputationLoss < 0 || k.ReputationLoss > 500) p.Add("pullout.reputationLoss must be from 0 to 500");
        }

        static void TreasuryProblems(List<string> p, GameConfig c)
        {
            TreasuryConfig k = c.Treasury;
            if (k == null) { p.Add("treasury is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("treasury.enabled must be 0 or 1");
            if (k.MinMove < 1) p.Add("treasury.minMove must be at least 1");
            if (k.WeeklyYieldPpm < 0 || k.WeeklyYieldPpm > 5000) p.Add("treasury.weeklyYieldPpm must be from 0 to 5000, so the reserve is never a money machine");
            if (k.DrawFeePermille < 0 || k.DrawFeePermille > 500) p.Add("treasury.drawFeePermille must be from 0 to 500");
            if (k.CoverFeePermille < k.DrawFeePermille || k.CoverFeePermille > 900) p.Add("treasury.coverFeePermille must be from drawFeePermille to 900, so a rescue costs at least what a planned draw does");
        }

        static void PledgeProblems(List<string> p, GameConfig c)
        {
            PledgeConfig k = c.Pledge;
            if (k == null) { p.Add("pledge is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 5) p.Add("pledge.maxLevel must be from 0 to 5 (0 turns pledges off)");
            if (k.BaseFloor < 1 || k.FloorStep < 0 || k.BaseFloor + (long)k.FloorStep * (MathI.Max(1, k.MaxLevel) - 1) > MathI.Permille)
                p.Add("pledge.baseFloor and floorStep must keep the top level's floor from 1 to 1000");
            if (k.TrustPerLevel < 0 || k.TrustPerLevel > 10) p.Add("pledge.trustPerLevel must be from 0 to 10");
            if (k.BreachReputation < 0 || k.BreachReputation > 500) p.Add("pledge.breachReputation must be from 0 to 500");
            if (k.BreachTrust < 0 || k.BreachTrust > 500) p.Add("pledge.breachTrust must be from 0 to 500");
            if (k.WalkBackReputation < 0 || k.WalkBackReputation > k.BreachReputation) p.Add("pledge.walkBackReputation must be from 0 to breachReputation, so breaking a pledge costs more than walking it back");
            if (k.LockWeeks < 0 || k.LockWeeks > 104) p.Add("pledge.lockWeeks must be from 0 to 104");
        }

        static void RoadmapProblems(List<string> p, GameConfig c)
        {
            RoadmapConfig k = c.Roadmap;
            if (k == null) { p.Add("roadmap is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 5) p.Add("roadmap.maxLevel must be from 0 to 5 (0 turns roadmaps off)");
            if (k.MinWeeks < 1 || k.MaxWeeks < k.MinWeeks || k.MaxWeeks > 156) p.Add("roadmap.minWeeks and maxWeeks must be from 1 to 156 with maxWeeks at least minWeeks");
            if (k.StretchPerLevel < 1 || k.StretchPerLevel > 1000) p.Add("roadmap.stretchPerLevel must be from 1 to 1000");
            if (k.HypePerLevel < 0 || k.HypePerLevel > 100) p.Add("roadmap.hypePerLevel must be from 0 to 100");
            if (k.DeliverPerLevel < 0 || k.DeliverPerLevel > 200) p.Add("roadmap.deliverPerLevel must be from 0 to 200");
            if (k.MissPerLevel <= k.HypePerLevel || k.MissPerLevel > 300) p.Add("roadmap.missPerLevel must be above hypePerLevel and at most 300, so a missed roadmap loses more than the hype earned");
        }

        static void MentoringProblems(List<string> p, GameConfig c)
        {
            MentoringConfig k = c.Mentoring;
            if (k == null) { p.Add("mentoring is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("mentoring.enabled must be 0 or 1");
            if (k.MaxPairs < 1 || k.MaxPairs > 10) p.Add("mentoring.maxPairs must be from 1 to 10");
            if (k.Weeks < 1 || k.Weeks > 104) p.Add("mentoring.weeks must be from 1 to 104");
            if (k.MenteeWeightPermille < 100 || k.MenteeWeightPermille > MathI.Permille) p.Add("mentoring.menteeWeightPermille must be from 100 to 1000, so a junior never counts for more than a whole person");
            if (k.MoralePerWeek < 0 || k.MoralePerWeek > 50) p.Add("mentoring.moralePerWeek must be from 0 to 50");
            if (k.MoraleCeiling < 1 || k.MoraleCeiling > MathI.Permille) p.Add("mentoring.moraleCeiling must be from 1 to 1000");
            if (k.GraduateMorale < 0 || k.GraduateMorale > 500) p.Add("mentoring.graduateMorale must be from 0 to 500");
            if (k.MentorMorale < 0 || k.MentorMorale > 500) p.Add("mentoring.mentorMorale must be from 0 to 500");
        }

        static void SkunkworksProblems(List<string> p, GameConfig c)
        {
            SkunkworksConfig k = c.Skunkworks;
            if (k == null) { p.Add("skunkworks is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("skunkworks.enabled must be 0 or 1");
            if (k.MinPeople < 1 || k.MinPeople > 20) p.Add("skunkworks.minPeople must be from 1 to 20");
            if (k.MaxSharePermille < 1 || k.MaxSharePermille > 900) p.Add("skunkworks.maxSharePermille must be from 1 to 900, so a function is never emptied");
            if (k.MinWeeks < 1 || k.MaxWeeks < k.MinWeeks || k.MaxWeeks > 104) p.Add("skunkworks.minWeeks and maxWeeks must be from 1 to 104 with maxWeeks at least minWeeks");
            if (k.LongWeeks < k.MinWeeks || k.LongWeeks > k.MaxWeeks) p.Add("skunkworks.longWeeks must sit between minWeeks and maxWeeks");
            if (k.LongBonusPermille < 0 || k.LongBonusPermille > 2000) p.Add("skunkworks.longBonusPermille must be from 0 to 2000");
            if (k.EarlyPermille < 0 || k.EarlyPermille >= MathI.Permille) p.Add("skunkworks.earlyPermille must be from 0 to 999, so wrapping up early always pays less than finishing");
            if (k.CostPerPersonWeek < 1) p.Add("skunkworks.costPerPersonWeek must be at least 1, so a project is never free");
            if (k.LockWeeks < 0 || k.LockWeeks > 104) p.Add("skunkworks.lockWeeks must be from 0 to 104");
            if (k.ResearchPayoffPermille < 1 || k.ResearchPayoffPermille > 5000) p.Add("skunkworks.researchPayoffPermille must be from 1 to 5000");
            if (k.AlignmentPerHundred < 0 || k.AlignmentPerHundred > 500) p.Add("skunkworks.alignmentPerHundred must be from 0 to 500");
            if (k.MaxAlignment < 0 || k.MaxAlignment > 500) p.Add("skunkworks.maxAlignment must be from 0 to 500");
            if (k.DebtPerPersonWeek < 0 || k.DebtPerPersonWeek > 50) p.Add("skunkworks.debtPerPersonWeek must be from 0 to 50");
        }

        static void LabourProblems(List<string> p, GameConfig c)
        {
            LabourConfig k = c.Labour;
            if (k == null) { p.Add("labour is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("labour.enabled must be 0 or 1");
            if (k.MinStaff < 2 || k.MinStaff > 200) p.Add("labour.minStaff must be from 2 to 200");
            if (k.MoraleBelow < 0 || k.MoraleBelow > MathI.Permille) p.Add("labour.moraleBelow must be from 0 to 1000");
            if (k.LowMoralePerWeek < 0 || k.LowMoralePerWeek > 500 || k.CrunchPerWeek < 0 || k.CrunchPerWeek > 500) p.Add("labour.lowMoralePerWeek and crunchPerWeek must be from 0 to 500");
            if (k.LayoffPerFull < 0 || k.LayoffPerFull > 2000) p.Add("labour.layoffPerFull must be from 0 to 2000");
            if (k.DecayPerWeek < 0 || k.DecayPerWeek > 500) p.Add("labour.decayPerWeek must be from 0 to 500");
            if (k.DrivePressure < 100 || k.DrivePressure >= MathI.Permille) p.Add("labour.drivePressure must be from 100 to 999, so the staff can always organise before the top");
            if (k.WindowWeeks < 1 || k.WindowWeeks > 52) p.Add("labour.windowWeeks must be from 1 to 52");
            if (k.ResistWeeks < 1 || k.ResistWeeks > 104) p.Add("labour.resistWeeks must be from 1 to 104");
            if (k.ResistFactorPermille < MathI.Permille || k.ResistFactorPermille > 5000) p.Add("labour.resistFactorPermille must be from 1000 to 5000, so resisting never calms anyone");
            if (k.ResistCost < 0) p.Add("labour.resistCost must not be negative");
            if (k.HeldPressure < 0 || k.HeldPressure >= k.DrivePressure) p.Add("labour.heldPressure must be from 0 to below drivePressure");
            if (k.StrikeWeeks < 1 || k.StrikeWeeks > 26) p.Add("labour.strikeWeeks must be from 1 to 26");
            if (k.StrikeMorale < 0 || k.StrikeMorale > MathI.Permille) p.Add("labour.strikeMorale must be from 0 to 1000");
            if (k.StrikeCutPermille < 0 || k.StrikeCutPermille >= MathI.Permille) p.Add("labour.strikeCutPermille must be from 0 to 999");
            if (k.RecogniseMorale < 0 || k.RecogniseMorale > 500) p.Add("labour.recogniseMorale must be from 0 to 500");
            if (k.PayPermille < 0 || k.PayPermille > 500 || k.HardPayPermille < k.PayPermille || k.HardPayPermille > 1000) p.Add("labour.payPermille must be from 0 to 500 and hardPayPermille from payPermille to 1000");
            if (k.SeverancePermille < MathI.Permille || k.HardSeverancePermille < k.SeverancePermille || k.HardSeverancePermille > 10000) p.Add("labour.severancePermille must be at least 1000 and hardSeverancePermille from severancePermille to 10000");
            if (k.ResearchPermille < 0 || k.ResearchPermille > 500 || k.HardResearchPermille < 0 || k.HardResearchPermille > k.ResearchPermille) p.Add("labour.researchPermille must be from 0 to 500 and hardResearchPermille from 0 to researchPermille");
        }

        static void SunsetProblems(List<string> p, GameConfig c)
        {
            SunsetConfig k = c.Sunset;
            if (k == null) { p.Add("sunset is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("sunset.enabled must be 0 or 1");
            if (k.MinWeeks < 1 || k.MaxWeeks <= k.MinWeeks || k.MaxWeeks > 104) p.Add("sunset.minWeeks and maxWeeks must be from 1 to 104 with maxWeeks above minWeeks");
            if (k.SupportPermille < 0 || k.SupportPermille > MathI.Permille) p.Add("sunset.supportPermille must be from 0 to 1000");
            if (k.TrustDropLong < 0 || k.TrustDropShort < k.TrustDropLong || k.TrustDropShort > MathI.Permille) p.Add("sunset.trustDropShort must be from trustDropLong to 1000, so a short notice never hurts less than a long one");
            if (k.AssistedCutPermille < 0 || k.AssistedCutPermille > MathI.Permille) p.Add("sunset.assistedCutPermille must be from 0 to 1000");
            if (k.DroppedPermille < MathI.Permille || k.DroppedPermille > 5000) p.Add("sunset.droppedPermille must be from 1000 to 5000, so losing a sector never hurts less than moving it");
            if (k.CancelReputation < 0 || k.CancelReputation > 200 || k.BrokenReputation < k.CancelReputation || k.BrokenReputation > 400) p.Add("sunset.cancelReputation must be from 0 to 200 and brokenReputation from cancelReputation to 400");
        }

        static void AccountsProblems(List<string> p, GameConfig c)
        {
            AccountsConfig k = c.Accounts;
            if (k == null) { p.Add("accounts is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("accounts.enabled must be 0 or 1");
            if (k.MaxPeople < 1 || k.MaxPeople > 10) p.Add("accounts.maxPeople must be from 1 to 10");
            if (k.KeepFree < 0 || k.KeepFree > 10) p.Add("accounts.keepFree must be from 0 to 10");
            if (k.PerPersonPermille < 0 || k.PerPersonPermille > 500) p.Add("accounts.perPersonPermille must be from 0 to 500");
            if (k.StartPermille < 0 || k.StartPermille >= MathI.Permille) p.Add("accounts.startPermille must be from 0 to 999, so a team always has something to learn");
            if (k.MaxWeeks < 1 || k.MaxWeeks > 260) p.Add("accounts.maxWeeks must be from 1 to 260");
            if (k.ComboCapPermille < 0 || k.ComboCapPermille > 1000) p.Add("accounts.comboCapPermille must be from 0 to 1000");
            if (k.MaxPeople * k.PerPersonPermille > 1000) p.Add("accounts.maxPeople times perPersonPermille must not top 1000");
        }

        static void ConferenceProblems(List<string> p, GameConfig c)
        {
            ConferenceConfig k = c.Conference;
            if (k == null) { p.Add("conference is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("conference.enabled must be 0 or 1");
            if (k.MaxPending < 1 || k.MaxPending > 10) p.Add("conference.maxPending must be from 1 to 10");
            if (k.MainCost < 1 || k.FlagshipCost < k.MainCost || k.FlagshipCost > 20) p.Add("conference.mainCost must be at least 1 and flagshipCost from mainCost to 20");
            if (k.WorkshopWeeks < 1 || k.MainWeeks < k.WorkshopWeeks || k.FlagshipWeeks < k.MainWeeks || k.FlagshipWeeks > 104) p.Add("conference.workshopWeeks must be at least 1, mainWeeks at least workshopWeeks and flagshipWeeks from mainWeeks to 104");
            if (k.MainBar < 0 || k.FlagshipBar < k.MainBar || k.FlagshipBar > 2000) p.Add("conference.mainBar must be at least 0 and flagshipBar from mainBar to 2000");
            if (k.EffortPrestige < 0 || k.EffortPrestige > 500) p.Add("conference.effortPrestige must be from 0 to 500");
            if (k.AcceptPrestige < 0 || k.AcceptPrestige > 500 || k.AcceptReputation < 0 || k.AcceptReputation > 200) p.Add("conference.acceptPrestige must be from 0 to 500 and acceptReputation from 0 to 200");
            if (k.RefundPermille < 0 || k.RefundPermille >= MathI.Permille || k.WithdrawPermille < k.RefundPermille || k.WithdrawPermille >= MathI.Permille) p.Add("conference.refundPermille must be from 0 to 999 and withdrawPermille from refundPermille to 999, so a paper always costs something");
        }

        static void AlumniProblems(List<string> p, GameConfig c)
        {
            AlumniConfig k = c.Alumni;
            if (k == null) { p.Add("alumni is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("alumni.enabled must be 0 or 1");
            if (k.MaxSize < 1 || k.MaxSize > 50) p.Add("alumni.maxSize must be from 1 to 50");
            if (k.ExpireWeeks < 1 || k.ExpireWeeks > 520) p.Add("alumni.expireWeeks must be from 1 to 520");
            if (k.FeeGoodPermille < 0 || k.FeeLaidOffPermille < k.FeeGoodPermille || k.FeeQuitPermille < k.FeeLaidOffPermille || k.FeeQuitPermille > MathI.Permille) p.Add("alumni.feeGoodPermille must be at least 0, feeLaidOffPermille at least feeGoodPermille and feeQuitPermille from feeLaidOffPermille to 1000, so a worse parting never makes a return cheaper");
            if (k.MoraleGood < -500 || k.MoraleGood > 500 || k.MoraleLaidOff < -500 || k.MoraleLaidOff > k.MoraleGood || k.MoraleQuit < -500 || k.MoraleQuit > k.MoraleLaidOff) p.Add("alumni.moraleGood must be from -500 to 500, moraleLaidOff from -500 to moraleGood and moraleQuit from -500 to moraleLaidOff");
            if (k.PeerGood < -200 || k.PeerGood > 200 || k.PeerLaidOff < -200 || k.PeerLaidOff > k.PeerGood || k.PeerQuit < -200 || k.PeerQuit > k.PeerLaidOff) p.Add("alumni.peerGood must be from -200 to 200, peerLaidOff from -200 to peerGood and peerQuit from -200 to peerLaidOff");
        }

        static void InternshipsProblems(List<string> p, GameConfig c)
        {
            InternshipsConfig k = c.Internships;
            if (k == null) { p.Add("internships is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("internships.enabled must be 0 or 1");
            if (k.MaxCohort < 1 || k.MaxCohort > 20) p.Add("internships.maxCohort must be from 1 to 20");
            if (k.Weeks < 1 || k.Weeks > 104) p.Add("internships.weeks must be from 1 to 104");
            if (k.StipendPermille < 0 || k.StipendPermille > MathI.Permille) p.Add("internships.stipendPermille must be from 0 to 1000");
            if (k.ConvertPermille < 0 || k.ConvertPermille > MathI.Permille || k.StipendPermille + k.ConvertPermille >= MathI.Permille) p.Add("internships.convertPermille must be from 0 to 1000 and with the stipend under 1000, or interns would cost more than hiring");
            if (k.ConvertMorale < -500 || k.ConvertMorale > 500) p.Add("internships.convertMorale must be from -500 to 500");
        }

        static void SupportProblems(List<string> p, GameConfig c)
        {
            SupportConfig k = c.Support;
            if (k == null) { p.Add("support is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("support.enabled must be 0 or 1");
            if (k.MaxPeople < 1 || k.MaxPeople > 50) p.Add("support.maxPeople must be from 1 to 50");
            if (k.KeepFree < 0 || k.KeepFree > 20) p.Add("support.keepFree must be from 0 to 20");
            if (k.RevenuePerTicket < 1) p.Add("support.revenuePerTicket must be at least 1");
            if (k.BaseClear < 0 || k.BaseClear > 1000) p.Add("support.baseClear must be from 0 to 1000");
            if (k.PerPerson < 1 || k.PerPerson > 1000) p.Add("support.perPerson must be from 1 to 1000");
            if (k.BacklogLimit < 0 || k.BacklogLimit > k.MaxBacklog) p.Add("support.backlogLimit must be from 0 to maxBacklog");
            if (k.MaxBacklog < 1 || k.MaxBacklog > 100000) p.Add("support.maxBacklog must be from 1 to 100000");
            if (k.TrustPerTicket < 0 || k.TrustPerTicket > 100) p.Add("support.trustPerTicket must be from 0 to 100");
            if (k.MaxWeeklyTrust < 0 || k.MaxWeeklyTrust > MathI.Permille) p.Add("support.maxWeeklyTrust must be from 0 to 1000");
        }

        static void SabbaticalProblems(List<string> p, GameConfig c)
        {
            SabbaticalConfig k = c.Sabbatical;
            if (k == null) { p.Add("sabbatical is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("sabbatical.enabled must be 0 or 1");
            if (k.Weeks < 1 || k.Weeks > 52) p.Add("sabbatical.weeks must be from 1 to 52");
            if (k.MaxAway < 1 || k.MaxAway > 20) p.Add("sabbatical.maxAway must be from 1 to 20");
            if (k.BurnoutBelow < 0 || k.BurnoutBelow > k.RestlessAbove || k.RestlessAbove > MathI.Permille) p.Add("sabbatical.burnoutBelow must be from 0 to restlessAbove, and restlessAbove at most 1000");
            if (k.RefreshedMorale < 0 || k.RefreshedMorale > MathI.Permille || k.RestlessMorale < 0 || k.RestlessMorale > MathI.Permille || k.RefreshedMorale <= k.RestlessMorale) p.Add("sabbatical.refreshedMorale and restlessMorale must be from 0 to 1000, with refreshedMorale above restlessMorale");
            if (k.RestedBonus < -500 || k.RestedBonus > 500) p.Add("sabbatical.restedBonus must be from -500 to 500");
        }

        static void RequisitionsProblems(List<string> p, GameConfig c)
        {
            RequisitionsConfig k = c.Requisitions;
            if (k == null) { p.Add("requisitions is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("requisitions.enabled must be 0 or 1");
            if (k.MaxOpen < 1 || k.MaxOpen > 20) p.Add("requisitions.maxOpen must be from 1 to 20");
            if (k.MaxCount < 1 || k.MaxCount > 20) p.Add("requisitions.maxCount must be from 1 to 20");
            if (k.LeadWeeks < 1 || k.LeadWeeks > 104) p.Add("requisitions.leadWeeks must be from 1 to 104");
            if (k.FeePermille < 1 || k.FeePermille >= MathI.Permille) p.Add("requisitions.feePermille must be from 1 to 999, a price lower than hiring on the day");
            if (k.DepositPermille < 0 || k.DepositPermille > MathI.Permille) p.Add("requisitions.depositPermille must be from 0 to 1000");
        }

        static void ScholarsProblems(List<string> p, GameConfig c)
        {
            ScholarsConfig k = c.Scholars;
            if (k == null) { p.Add("scholars is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("scholars.enabled must be 0 or 1");
            if (k.MaxScholars < 1 || k.MaxScholars > 20) p.Add("scholars.maxScholars must be from 1 to 20");
            if (k.Weeks < 1 || k.Weeks > 104) p.Add("scholars.weeks must be from 1 to 104");
            if (k.StipendPermille < 0 || k.StipendPermille > MathI.Permille) p.Add("scholars.stipendPermille must be from 0 to 1000");
            if (k.ResearchPermille < 0 || k.ResearchPermille > MathI.Permille) p.Add("scholars.researchPermille must be from 0 to 1000, no more than a researcher");
            if (k.PrestigeEach < 0 || k.PrestigeEach > 200) p.Add("scholars.prestigeEach must be from 0 to 200");
        }

        static void LitigationProblems(List<string> p, GameConfig c)
        {
            LitigationConfig k = c.Litigation;
            if (k == null) { p.Add("litigation is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("litigation.enabled must be 0 or 1");
            if (k.MaxOpen < 1 || k.MaxOpen > 20) p.Add("litigation.maxOpen must be from 1 to 20");
            if (k.HearingWeeks < 1 || k.HearingWeeks > 104) p.Add("litigation.hearingWeeks must be from 1 to 104");
            if (k.AppealWeeks < 1 || k.AppealWeeks > 104) p.Add("litigation.appealWeeks must be from 1 to 104");
            if (k.MinClaim < 1 || k.PrivacyClaim < k.MinClaim) p.Add("litigation.minClaim must be at least 1 and no more than privacyClaim");
            if (k.IncidentPermille < 0 || k.IncidentPermille > 20000 || k.DismissalPermille < 0 || k.DismissalPermille > 20000) p.Add("litigation.incidentPermille and dismissalPermille must be from 0 to 20000");
            if (k.SettlePermille < 1 || k.SettlePermille >= k.JudgmentPermille) p.Add("litigation.settlePermille must be from 1 and under judgmentPermille, or nobody would defend");
            if (k.JudgmentPermille < 1 || k.JudgmentPermille > 5000) p.Add("litigation.judgmentPermille must be from 1 to 5000");
            if (k.InHouseStrength < 0 || k.FirmStrength < k.InHouseStrength || k.FirmStrength > MathI.Permille) p.Add("litigation.firmStrength must be from inHouseStrength (at least 0) to 1000");
            if (k.FirmFeePermille < 0 || k.FirmFeePermille > MathI.Permille || k.AppealFeePermille < 0 || k.AppealFeePermille > MathI.Permille) p.Add("litigation.firmFeePermille and appealFeePermille must be from 0 to 1000");
            if (k.JudgeAt < 1 || k.JudgeAt > MathI.Permille) p.Add("litigation.judgeAt must be from 1 to 1000");
            if (k.AppealStrength < 0 || k.AppealStrength > MathI.Permille) p.Add("litigation.appealStrength must be from 0 to 1000");
            if (k.LossReputation < 0 || k.LossReputation > 500 || k.WinReputation < 0 || k.WinReputation > 500) p.Add("litigation.lossReputation and winReputation must be from 0 to 500");
        }

        static void SecurityProblems(List<string> p, GameConfig c)
        {
            SecurityConfig k = c.Security;
            if (k == null) { p.Add("security is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("security.enabled must be 0 or 1");
            if (k.MaxTier < 1 || k.MaxTier > 10) p.Add("security.maxTier must be from 1 to 10");
            if (k.TierCost < 1 || k.Ransom < 1) p.Add("security.tierCost and ransom must be at least 1");
            if (k.BuildWeeks < 1 || k.BuildWeeks > 104 || k.AnswerWeeks < 1 || k.AnswerWeeks > 104) p.Add("security.buildWeeks and answerWeeks must be from 1 to 104");
            if (k.UpkeepPerTier < 0) p.Add("security.upkeepPerTier must not be negative");
            if (k.ExposurePerModel < 1 || k.ExposurePerModel > 1000 || k.ReliefPerTier < 0 || k.ReliefPerTier > 1000) p.Add("security.exposurePerModel must be from 1 to 1000 and reliefPerTier from 0 to 1000");
            if (k.Threshold < 1 || k.Threshold > 100000) p.Add("security.threshold must be from 1 to 100000");
            if (k.DiscloseReputation < 0 || k.DiscloseReputation > 500 || k.LeakReputation < k.DiscloseReputation || k.LeakReputation > 500) p.Add("security.leakReputation must be at least discloseReputation and no more than 500");
        }

        static void GovContractsProblems(List<string> p, GameConfig c)
        {
            GovContractsConfig k = c.GovContracts;
            if (k == null) { p.Add("govContracts is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("govContracts.enabled must be 0 or 1");
            if (k.OfferEveryWeeks < 1 || k.OfferEveryWeeks > 520 || k.OfferWeeks < 1 || k.OfferWeeks > 104) p.Add("govContracts.offerEveryWeeks must be from 1 to 520 and offerWeeks from 1 to 104");
            if (k.Installments < 1 || k.Installments > 20) p.Add("govContracts.installments must be from 1 to 20");
            if (k.MissLimit < 1 || k.MissLimit > k.Installments) p.Add("govContracts.missLimit must be from 1 to installments");
            if (k.PenaltyPermille < 0 || k.PenaltyPermille > 5000) p.Add("govContracts.penaltyPermille must be from 0 to 5000");
            if (k.AbandonReputation < 0 || k.AbandonReputation > 500) p.Add("govContracts.abandonReputation must be from 0 to 500");
            if (k.KeepFree < 0 || k.KeepFree > 20) p.Add("govContracts.keepFree must be from 0 to 20");
            if (c.Role(k.MoraleRole) == null) p.Add("govContracts.moraleRole names the unknown role \"" + k.MoraleRole + "\"");
            if (k.Kinds.Count == 0) p.Add("govContracts.kinds is empty");
            UniqueIds(p, "govContracts.kinds", k.Kinds);
            for (int i = 0; i < k.Kinds.Count; i++)
            {
                ContractKindConfig kind = k.Kinds[i];
                string who = "govContracts.kinds[" + i + "]";
                if (kind.Clearance < 0 || kind.Clearance > 10) p.Add(who + " needs a clearance from 0 to 10");
                if (kind.Weeks < k.Installments || kind.Weeks > 520) p.Add(who + " must last from installments to 520 weeks");
                if (kind.Pay < k.Installments) p.Add(who + " must pay at least one a instalment");
                if (kind.People < 1 || kind.People > 20) p.Add(who + " must need from 1 to 20 engineers");
                if (kind.Morale < 0 || kind.Morale > MathI.Permille) p.Add(who + " has a morale cost from 0 to 1000");
                if (kind.AcceptReputation < -500 || kind.AcceptReputation > 500 || kind.DoneReputation < 0 || kind.DoneReputation > 500) p.Add(who + " has a reputation outside the allowed range");
            }
        }

        static void CharterProblems(List<string> p, GameConfig c)
        {
            CharterConfig k = c.Charter;
            if (k == null) { p.Add("charter is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("charter.enabled must be 0 or 1");
            if (k.ReviewWeeks < 1 || k.ReviewWeeks > 104) p.Add("charter.reviewWeeks must be from 1 to 104");
            if (k.ConvertFee < 0 || k.CapAmount < 1 || k.ChallengeClaim < 1) p.Add("charter.convertFee must not be negative and capAmount and challengeClaim must be at least 1");
            if (k.CappedFundingPermille < 1 || k.CappedFundingPermille > MathI.Permille) p.Add("charter.cappedFundingPermille must be from 1 to 1000");
            if (k.ForProfitGrantPermille < 0 || k.CappedGrantPermille < 0 || k.NonprofitGrantPermille < 0 || k.ForProfitGrantPermille > 5000 || k.CappedGrantPermille > 5000 || k.NonprofitGrantPermille > 5000) p.Add("charter grant shares must be from 0 to 5000");
            if (k.DonationPerPrestige < 0 || k.DonationPerPrestige > 100000) p.Add("charter.donationPerPrestige must be from 0 to 100000");
            if (k.DistributionPermille < 0 || k.DistributionPermille > MathI.Permille) p.Add("charter.distributionPermille must be from 0 to 1000");
            if (k.ChallengeStrength < 1 || k.ChallengeStrength > MathI.Permille) p.Add("charter.challengeStrength must be from 1 to 1000");
            if (k.ProfitMorale < 0 || k.ProfitMorale > MathI.Permille || k.MissionMorale < 0 || k.MissionMorale > MathI.Permille) p.Add("charter morale changes must be from 0 to 1000");
        }

        static void ScalingProblems(List<string> p, GameConfig c)
        {
            ScalingConfig k = c.Scaling;
            if (k == null) { p.Add("scaling is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("scaling.enabled must be 0 or 1");
            if (k.MaxLevel < 1 || k.MaxLevel > 10) p.Add("scaling.maxLevel must be from 1 to 10");
            if (k.FirstTripwire < 1 || k.TripwireStep < 1) p.Add("scaling.firstTripwire and tripwireStep must be at least 1");
            if (k.SafetyPerTier < 0 || k.SafetyPerTier > 20 || k.AdoptFee < 0) p.Add("scaling.safetyPerTier must be from 0 to 20 and adoptFee must not be negative");
            if (k.DampPermille < 0 || k.DampPermille * k.MaxLevel > MathI.Permille) p.Add("scaling.dampPermille times maxLevel must be from 0 to 1000");
            if (k.ClaimStrength < 0 || k.ClaimStrength > MathI.Permille) p.Add("scaling.claimStrength must be from 0 to 1000");
            if (k.WalkBackReputation < 0 || k.WalkBackReputation > 500 || k.OverrideReputation < k.WalkBackReputation || k.OverrideReputation > 500) p.Add("scaling.overrideReputation must be at least walkBackReputation and no more than 500");
            if (k.OverrideMorale < 0 || k.OverrideMorale > MathI.Permille || k.LockWeeks < 0 || k.LockWeeks > 520) p.Add("scaling.overrideMorale must be from 0 to 1000 and lockWeeks from 0 to 520");
        }

        static void TaxProblems(List<string> p, GameConfig c)
        {
            TaxConfig k = c.Tax;
            if (k == null) { p.Add("tax is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("tax.enabled must be 0 or 1");
            if (k.RatePermille < 0 || k.RatePermille > 900 || k.AllowancePerWeek < 0) p.Add("tax.ratePermille must be from 0 to 900 and allowancePerWeek must not be negative");
            if (k.HoldingCost < 0 || k.OffshoreCost < k.HoldingCost || k.HoldingWeeks < 1 || k.OffshoreWeeks < k.HoldingWeeks || k.OffshoreWeeks > 520) p.Add("tax.offshoreCost and offshoreWeeks must be at least the holding company's, weeks from 1 to 520");
            if (k.HoldingRelief < 0 || k.OffshoreRelief < k.HoldingRelief || k.OffshoreRelief > MathI.Permille) p.Add("tax.offshoreRelief must be at least holdingRelief and no more than 1000");
            if (k.HoldingExposure < 0 || k.OffshoreExposure < k.HoldingExposure || k.OffshoreExposure > 1000 || k.FeePerTier < 0) p.Add("tax.offshoreExposure must be at least holdingExposure, no more than 1000, and feePerTier must not be negative");
            if (k.CreditPermille < 0 || k.CreditPermille > MathI.Permille || k.ClaimExposure < 0 || k.ClaimExposure > 10000) p.Add("tax.creditPermille must be from 0 to 1000 and claimExposure from 0 to 10000");
            if (k.AuditThreshold < 1 || k.AuditWeeks < 1 || k.AuditWeeks > 520) p.Add("tax.auditThreshold must be at least 1 and auditWeeks from 1 to 520");
            if (k.CooperateInterest < 0 || k.CooperateInterest > 5000) p.Add("tax.cooperateInterest must be from 0 to 5000");
            if (k.SettlePermille < 0 || k.SettlePermille > MathI.Permille) p.Add("tax.settlePermille must be from 0 to 1000");
            if (k.ContestClaimPermille < 0 || k.ContestClaimPermille > 10000 || k.ContestStrength < 1 || k.StructureStrength < 0 || k.ContestStrength + 2 * k.StructureStrength > MathI.Permille) p.Add("tax.contestStrength plus two tiers of structureStrength must be from 1 to 1000 and contestClaimPermille from 0 to 10000");
            if (k.LapsePermille < 0 || k.LapsePermille > 5000) p.Add("tax.lapsePermille must be from 0 to 5000");
            if (k.ExposedReputation < 0 || k.ExposedReputation > 500) p.Add("tax.exposedReputation must be from 0 to 500");
        }

        static void StandardsProblems(List<string> p, GameConfig c)
        {
            StandardsConfig k = c.Standards;
            if (k == null) { p.Add("standards is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("standards.enabled must be 0 or 1");
            if (k.MaxLevel < 1 || k.MaxLevel > 10 || k.FirstWeeks < 1 || k.EveryWeeks < 1 || k.FirstWeeks > 2000 || k.EveryWeeks > 2000) p.Add("standards.maxLevel must be from 1 to 10 and firstWeeks and everyWeeks from 1 to 2000");
            if (k.SupportNeeded < 1 || k.IndustryRate < 1 || k.MemberRate < 0 || k.SponsorRate < k.MemberRate || k.ObstructRate < 0) p.Add("standards.supportNeeded and industryRate must be at least 1, sponsorRate at least memberRate, and rates must not be negative");
            if (k.MemberDues < 0 || k.SponsorDues < k.MemberDues || k.ObstructDues < 0 || k.SponsorFee < 0) p.Add("standards.sponsorDues must be at least memberDues and dues and fees must not be negative");
            if (k.ObstructEvery < 1 || k.ObstructEvery > 520 || k.ObstructReputation < 0 || k.ObstructReputation > 500) p.Add("standards.obstructEvery must be from 1 to 520 and obstructReputation from 0 to 500");
            if (k.AdoptCost < 0 || k.UpkeepPerLevel < 0 || k.BindingCapability < 0) p.Add("standards.adoptCost, upkeepPerLevel and bindingCapability must not be negative");
            if (k.LapseFine < 0) p.Add("standards.lapseFine must not be negative");
            if (k.LapseReputation < 0 || k.LapseReputation > 500) p.Add("standards.lapseReputation must be from 0 to 500");
            if (k.ClaimStrength < 0 || k.ClaimStrength * k.MaxLevel > MathI.Permille) p.Add("standards.claimStrength times maxLevel must be from 0 to 1000");
            if (k.SafetyRelief < 0 || k.SafetyRelief > 20) p.Add("standards.safetyRelief must be from 0 to 20");
        }

        static void ReliabilityProblems(List<string> p, GameConfig c)
        {
            ReliabilityConfig k = c.Reliability;
            if (k == null) { p.Add("reliability is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("reliability.enabled must be 0 or 1");
            if (k.BaseFailPpm < 0 || k.BaseFailPpm > 1000000 || k.AgePpm < 0 || k.AgePpm > 1000000) p.Add("reliability.baseFailPpm and agePpm must be from 0 to 1000000");
            if (k.HeatwavePermille < MathI.Permille || k.HeatwavePermille > 20000) p.Add("reliability.heatwavePermille must be from 1000 to 20000");
            if (k.FeeTier1 < 0 || k.FeeTier2 < k.FeeTier1) p.Add("reliability.feeTier2 must be at least feeTier1 and fees must not be negative");
            if (k.RmaWeeksTier1 < 1 || k.RmaWeeksTier2 < 1 || k.RmaWeeksTier2 > k.RmaWeeksTier1 || k.RmaWeeksTier1 > 520) p.Add("reliability.rmaWeeksTier2 must be from 1 to rmaWeeksTier1, and rmaWeeksTier1 no more than 520");
            if (k.SwitchFee < 0 || k.SpareUpkeep < 0) p.Add("reliability.switchFee and spareUpkeep must not be negative");
            if (k.MaxSpares < 0 || k.MaxSpares > 10000) p.Add("reliability.maxSpares must be from 0 to 10000");
            if (k.ExpeditePermille < 0 || k.ExpeditePermille > MathI.Permille) p.Add("reliability.expeditePermille must be from 0 to 1000");
        }

        static void CovenantsProblems(List<string> p, GameConfig c)
        {
            CovenantsConfig k = c.Covenants;
            if (k == null) { p.Add("covenants is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("covenants.enabled must be 0 or 1");
            if (k.MinCoverPermille < MathI.Permille || k.MinCoverPermille > 10000) p.Add("covenants.minCoverPermille must be from 1000 to 10000");
            if (k.CureWeeks < 1 || k.CureWeeks > 520) p.Add("covenants.cureWeeks must be from 1 to 520");
            if (k.SurchargePermille < 0 || k.SurchargePermille > 100) p.Add("covenants.surchargePermille must be from 0 to 100");
            if (k.WaiverFeePermille < 0 || k.WaiverFeePermille > MathI.Permille) p.Add("covenants.waiverFeePermille must be from 0 to 1000");
            if (k.WaiverWeeks < 1 || k.WaiverWeeks > 520) p.Add("covenants.waiverWeeks must be from 1 to 520");
            if (k.MaxWaivers < 0 || k.MaxWaivers > 100) p.Add("covenants.maxWaivers must be from 0 to 100");
        }

        static void AllowancesProblems(List<string> p, GameConfig c)
        {
            AllowancesConfig k = c.Allowances;
            if (k == null) { p.Add("allowances is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("allowances.enabled must be 0 or 1");
            if (k.WriteOffPermille < 1 || k.WriteOffPermille > MathI.Permille) p.Add("allowances.writeOffPermille must be from 1 to 1000");
            if (k.EnhancedPermille < 0 || k.EnhancedPermille > 2000) p.Add("allowances.enhancedPermille must be from 0 to 2000");
            if (k.EnhancedExposure < 0 || k.EnhancedExposure > 1000) p.Add("allowances.enhancedExposure must be from 0 to 1000");
            if (k.LockWeeks < 0 || k.LockWeeks > 520) p.Add("allowances.lockWeeks must be from 0 to 520");
        }

        static void ListingProblems(List<string> p, GameConfig c)
        {
            ListingConfig k = c.Listing;
            if (k == null) { p.Add("listing is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("listing.enabled must be 0 or 1");
            if (k.MinReputation < 0 || k.MinWeeklyRevenue < 0 || k.ValuationWeeks < 1 || k.ValuationWeeks > 1000) p.Add("listing.minReputation and minWeeklyRevenue must not be negative and valuationWeeks from 1 to 1000");
            if (k.FloatPermille < 1 || k.FloatPermille > 600 || k.FollowOnPermille < 1 || k.FollowOnPermille > 300 || k.ListingFeePermille < 0 || k.ListingFeePermille > 500) p.Add("listing.floatPermille must be from 1 to 600, followOnPermille from 1 to 300 and listingFeePermille from 0 to 500");
            if (k.ReportingCost < 0 || k.QuarterWeeks < 4 || k.QuarterWeeks > 52 || k.GuidanceWindow < 1 || k.GuidanceWindow >= k.QuarterWeeks) p.Add("listing.quarterWeeks must be from 4 to 52, guidanceWindow below it and reportingCost not negative");
            if (k.Conservative < 100 || k.Base < k.Conservative || k.Stretch < k.Base || k.Stretch > 5000) p.Add("listing.stretch must be at least base, base at least conservative, and all from 100 to 5000");
            if (k.BeatMargin < 0 || k.BeatMove < 0 || k.MeetMove < 0 || k.MissMove < 0 || k.NoGuidanceMove < 0 || k.BeatMove > 500 || k.MissMove > 300) p.Add("listing.beatMargin, beatMove, meetMove, missMove and noGuidanceMove must not be negative, beatMove at most 500 and missMove at most 300");
            if (k.PriceFloor < 1 || k.PriceCap < 1000 || k.PriceFloor > 1000) p.Add("listing.priceFloor must be from 1 to 1000 and priceCap at least 1000");
            if (k.FollowOnWeeks < 0 || k.BuybackImpact < 0 || k.BuybackImpact > 5000) p.Add("listing.followOnWeeks must not be negative and buybackImpact from 0 to 5000");
            if (k.ActivistBelow < 0 || k.ActivistReputation < 0 || k.ActivistReputation > 500 || k.MaxActivist < 1 || k.MaxActivist > 10) p.Add("listing.activistBelow and activistReputation must not be negative (reputation up to 500) and maxActivist from 1 to 10");
            if (k.TakePrivatePermille < MathI.Permille || k.TakePrivatePermille > 5000) p.Add("listing.takePrivatePermille must be from 1000 to 5000");
        }

        static void RbfProblems(List<string> p, GameConfig c)
        {
            RbfConfig k = c.Rbf;
            if (k == null) { p.Add("rbf is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("rbf.enabled must be 0 or 1");
            if (k.MinWeeklyRevenue < 0 || k.AdvanceWeeks < 1 || k.AdvanceWeeks > 100) p.Add("rbf.minWeeklyRevenue must not be negative and advanceWeeks from 1 to 100");
            if (k.ShareA < 1 || k.ShareB < k.ShareA || k.ShareC < k.ShareB || k.ShareC > 1000) p.Add("rbf.shareC must be at least shareB, shareB at least shareA, and all from 1 to 1000");
            if (k.CapA < MathI.Permille || k.CapB < MathI.Permille || k.CapC < MathI.Permille || k.CapA > 5000 || k.CapB > 5000 || k.CapC > 5000) p.Add("rbf.capA, capB and capC must be from 1000 to 5000");
            if (k.SlumpPermille < 0 || k.SlumpPermille > 1000 || k.SlumpCap < 0 || k.SlumpCap > 500) p.Add("rbf.slumpPermille must be from 0 to 1000 and slumpCap from 0 to 500");
            if (k.BuyoutAfter < 0 || k.BuyoutPermille < 100 || k.BuyoutPermille > 1000) p.Add("rbf.buyoutAfter must not be negative and buyoutPermille from 100 to 1000");
        }

        static void FoundationProblems(List<string> p, GameConfig c)
        {
            FoundationConfig k = c.Foundation;
            if (k == null) { p.Add("foundation is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("foundation.enabled must be 0 or 1");
            if (k.MinGift < 1 || k.ReturnPermille < 0 || k.ReturnPermille > 100) p.Add("foundation.minGift must be at least 1 and returnPermille from 0 to 100");
            if (k.PayoutA < 0 || k.PayoutB < k.PayoutA || k.PayoutC < k.PayoutB || k.PayoutC > 200) p.Add("foundation.payoutC must be at least payoutB, payoutB at least payoutA, and all from 0 to 200");
            if (k.GoodwillUnit < 1 || k.GoodwillDecay < 0 || k.GoodwillDecay > 1000 || k.MaxGoodwill < 1 || k.MaxGoodwill > 1000) p.Add("foundation.goodwillUnit must be at least 1, goodwillDecay from 0 to 1000 and maxGoodwill from 1 to 1000");
            if (k.SafetyDamp < 0 || k.MaxDamp < 0 || k.MaxDamp > 800 || k.ScienceStrength < 0 || k.EducationEvery < 1 || k.EducationAt < 0) p.Add("foundation.safetyDamp, educationAt and scienceStrength must not be negative, maxDamp up to 800 and educationEvery at least 1");
            if (k.SwitchLoss < 0 || k.SwitchLoss > 1000 || k.LockWeeks < 0 || k.WindUpPermille < 0 || k.WindUpPermille > 1000 || k.WindUpReputation < 0 || k.ReportWeeks < 1) p.Add("foundation.switchLoss and windUpPermille must be from 0 to 1000, lockWeeks and windUpReputation not negative and reportWeeks at least 1");
        }

        static void LeasebackProblems(List<string> p, GameConfig c)
        {
            LeasebackConfig k = c.Leaseback;
            if (k == null) { p.Add("leaseback is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("leaseback.enabled must be 0 or 1");
            if (k.AdvancePermille < 1 || k.AdvancePermille > MathI.Permille || k.RentPermille < 0 || k.RentPermille > 200) p.Add("leaseback.advancePermille must be from 1 to 1000 and rentPermille from 0 to 200");
            if (k.TermWeeks < 1 || k.ExtendWeeks < 0 || k.ExtendRentPermille < MathI.Permille || k.ExtendRentPermille > 5000) p.Add("leaseback.termWeeks must be at least 1, extendWeeks not negative and extendRentPermille from 1000 to 5000");
            if (k.BuybackPermille < 100 || k.BuybackPermille > 5000 || k.MissLimit < 1 || k.RepoReputation < 0 || k.RepoReputation > 500) p.Add("leaseback.buybackPermille must be from 100 to 5000, missLimit at least 1 and repoReputation from 0 to 500");
        }

        static void NotesProblems(List<string> p, GameConfig c)
        {
            NotesConfig k = c.ConvertibleNotes;
            if (k == null) { p.Add("convertibleNotes is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("convertibleNotes.enabled must be 0 or 1");
            if (k.MaxThousands < 1 || k.MaxThousands > 1000000) p.Add("convertibleNotes.maxThousands must be from 1 to 1000000");
            if (k.RateA < 0 || k.RateB < k.RateA || k.RateC < k.RateB || k.RateC > 100) p.Add("convertibleNotes.rateC must be at least rateB, rateB at least rateA, and all from 0 to 100");
            if (k.DiscountA < 0 || k.DiscountB < k.DiscountA || k.DiscountC < k.DiscountB || k.DiscountC > 800 || k.MaturityDiscount < 0 || k.MaturityDiscount > 800) p.Add("convertibleNotes.discountC must be at least discountB, discountB at least discountA, and all up to 800");
            if (k.WeeksA < 1 || k.WeeksB < 1 || k.WeeksC < 1 || k.ExtendWeeks < 0) p.Add("convertibleNotes.weeksA, weeksB and weeksC must be at least 1 and extendWeeks not negative");
            if (k.MaxConvertPermille < 1 || k.MaxConvertPermille > 900 || k.ExtendRate < MathI.Permille || k.ExtendRate > 5000 || k.ExtendDiscount < 0 || k.ExtendDiscount > 500) p.Add("convertibleNotes.maxConvertPermille must be from 1 to 900, extendRate from 1000 to 5000 and extendDiscount from 0 to 500");
        }

        static void DevicesProblems(List<string> p, GameConfig c)
        {
            DevicesConfig k = c.Devices;
            if (k == null) { p.Add("devices is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("devices.enabled must be 0 or 1");
            if (k.MinCapability < 0 || k.EraA < 0 || k.EraB < k.EraA || k.EraC < k.EraB) p.Add("devices.minCapability and eraA must not be negative, eraB at least eraA and eraC at least eraB");
            if (k.CostA < 0 || k.CostB < 0 || k.CostC < 0 || k.WeeksA < 1 || k.WeeksB < 1 || k.WeeksC < 1) p.Add("devices.costA, costB and costC must not be negative and weeksA, weeksB and weeksC at least 1");
            if (k.DemandA < 0 || k.DemandB < 0 || k.DemandC < 0 || k.MarginA < 0 || k.MarginB < 0 || k.MarginC < 0) p.Add("devices.demandA, demandB, demandC, marginA, marginB and marginC must not be negative");
            if (k.BaseQuality < 0 || k.ModelQuality < 0 || k.CapForMax < 1 || k.TestQuality < 0 || k.MaxTestWeeks < 0 || k.MaxTestWeeks > 12) p.Add("devices.baseQuality, modelQuality and testQuality must not be negative, capForMax at least 1 and maxTestWeeks from 0 to 12");
            if (k.LaunchBar < 0 || k.LaunchBar > MathI.Permille || k.RecallBar < 0 || k.RecallBar > MathI.Permille || k.FlopReputation < 0 || k.HitReputation < 0 || k.FlopReputation > 500 || k.HitReputation > 500) p.Add("devices.launchBar and recallBar must be from 0 to 1000, flopReputation and hitReputation from 0 to 500");
            if (k.FlopDemand < 0 || k.FlopDemand > MathI.Permille || k.FadePermille < 0 || k.FadePermille > 500 || k.RecallWeeks < 1 || k.RecallCost < 0) p.Add("devices.flopDemand must be from 0 to 1000, fadePermille from 0 to 500, recallWeeks at least 1 and recallCost not negative");
            if (k.LowPrice < 100 || k.LowPrice > MathI.Permille || k.HighPrice < MathI.Permille || k.HighPrice > 5000 || k.LowDemand < MathI.Permille || k.LowDemand > 5000 || k.HighDemand < 0 || k.HighDemand > MathI.Permille) p.Add("devices.lowPrice must be from 100 to 1000, highPrice from 1000 to 5000, lowDemand from 1000 to 5000 and highDemand from 0 to 1000");
        }

        static void PlatformsProblems(List<string> p, GameConfig c)
        {
            PlatformsConfig k = c.Platforms;
            if (k == null) { p.Add("platforms is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("platforms.enabled must be 0 or 1");
            if (k.MinCapability < 0 || k.MinReputation < 0 || k.MinReputation > MathI.Permille || k.EraA < 0 || k.EraB < 0 || k.EraC < 0) p.Add("platforms.minCapability, eraA, eraB and eraC must not be negative and minReputation from 0 to 1000");
            if (k.LiftA < 0 || k.LiftB < 0 || k.LiftC < 0 || k.LiftA > 2000 || k.LiftB > 2000 || k.LiftC > 2000) p.Add("platforms.liftA, liftB and liftC must be from 0 to 2000");
            if (k.CutA < 0 || k.CutB < 0 || k.CutC < 0 || k.MaxCut < k.CutA || k.MaxCut < k.CutB || k.MaxCut < k.CutC || k.MaxCut > 950) p.Add("platforms.cutA, cutB and cutC must not be negative and no more than maxCut, which is at most 950");
            if (k.TermWeeks < 2 || k.WindowWeeks < 1 || k.WindowWeeks >= k.TermWeeks) p.Add("platforms.termWeeks must be at least 2 and windowWeeks at least 1 and under termWeeks");
            if (k.RelianceStep < 0 || k.RelianceStep > MathI.Permille || k.HoldupBar < 0 || k.HoldupBar > MathI.Permille || k.HoldupCut < 0 || k.RenewCutStep < 0 || k.WalkReputation < 0 || k.WalkReputation > 500) p.Add("platforms.relianceStep and holdupBar must be from 0 to 1000, holdupCut and renewCutStep not negative and walkReputation from 0 to 500");
        }

        static void CreedProblems(List<string> p, GameConfig c)
        {
            CreedConfig k = c.Creed;
            if (k == null) { p.Add("creed is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("creed.enabled must be 0 or 1");
            if (k.KeptPoints < 0 || k.BrokenPoints < 0 || k.KeptPoints > MathI.Permille || k.BrokenPoints > MathI.Permille) p.Add("creed.keptPoints and brokenPoints must be from 0 to 1000");
            if (k.StartIntegrity < 0 || k.StartIntegrity > MathI.Permille || k.FlipIntegrity < 0 || k.FlipIntegrity > MathI.Permille || k.OwnUpRestore < 0 || k.OwnUpRestore > MathI.Permille || k.FlipLockWeeks < 0) p.Add("creed.startIntegrity, flipIntegrity and ownUpRestore must be from 0 to 1000 and flipLockWeeks not negative");
            if (k.LowBar < 0 || k.HighBar > MathI.Permille || k.LowBar + k.Gap >= k.HighBar - k.Gap || k.Gap < 0) p.Add("creed.lowBar must be under highBar by more than twice gap, and highBar at most 1000");
            if (k.HighReputation < 0 || k.LowReputation < 0 || k.OwnUpReputation < 0 || k.HighReputation > 500 || k.LowReputation > 500 || k.OwnUpReputation > 500 || k.HighMorale < 0 || k.LowMorale < 0 || k.HighMorale > MathI.Permille || k.LowMorale > MathI.Permille) p.Add("creed.highReputation, lowReputation and ownUpReputation must be from 0 to 500 and highMorale and lowMorale from 0 to 1000");
            if (k.ShieldPermille < 0 || k.ShieldPermille > MathI.Permille || k.ExposePermille < MathI.Permille || k.ExposePermille > 5000) p.Add("creed.shieldPermille must be from 0 to 1000 and exposePermille from 1000 to 5000");
            if (k.TownCost < 0 || k.TownGain < 0 || k.TownGain > MathI.Permille || k.TownCooldown < 0) p.Add("creed.townCost, townGain and townCooldown must not be negative and townGain at most 1000");
        }

        static void ShocksProblems(List<string> p, GameConfig c)
        {
            ShocksConfig k = c.Shocks;
            if (k == null) { p.Add("shocks is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("shocks.enabled must be 0 or 1");
            if (k.WindowWeeks < 1 || k.BraceCost < 0 || k.HitCash < 0 || k.BracePermille < 0 || k.BracePermille > MathI.Permille) p.Add("shocks.windowWeeks must be at least 1, braceCost and hitCash not negative and bracePermille from 0 to 1000");
            if (k.HitReputation < 0 || k.HitReputation > 500 || k.BoomReputation < 0 || k.BoomReputation > 500 || k.CounterGain < 0 || k.CounterGain > 500 || k.HitMorale < 0 || k.HitMorale > MathI.Permille) p.Add("shocks.hitReputation, boomReputation and counterGain must be from 0 to 500 and hitMorale from 0 to 1000");
            if (k.BoomBrace < MathI.Permille || k.BoomBrace > 10000 || k.CounterBoom < MathI.Permille || k.CounterBoom > 10000 || k.CounterMiss < MathI.Permille || k.CounterMiss > 10000 || k.CounterBar < 0) p.Add("shocks.boomBrace, counterBoom and counterMiss must be from 1000 to 10000 and counterBar not negative");
        }

        static void ReplicationProblems(List<string> p, GameConfig c)
        {
            ReplicationConfig k = c.Replication;
            if (k == null) { p.Add("replication is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("replication.enabled must be 0 or 1");
            if (k.PendingMax < 1 || k.PendingWeeks < 1 || k.Weeks < 1 || k.MaxEffort < 0 || k.MaxEffort > 12 || k.EffortPoints < 0) p.Add("replication.pendingMax, pendingWeeks and weeks must be at least 1, maxEffort from 0 to 12 and effortPoints not negative");
            if (k.RigorBase < 0 || k.RigorStep < 0 || k.RigorPerWriter < 0 || k.BarBase < 0 || k.BarBase > MathI.Permille || k.BarPerPrestige < 0 || k.BarPerPrestige > MathI.Permille) p.Add("replication.rigorBase, rigorStep and rigorPerWriter must not be negative, barBase and barPerPrestige from 0 to 1000");
            if (k.WinPrestige < 0 || k.LossPrestige < 0 || k.WithholdPrestige < 0 || k.WinReputation < 0 || k.LossReputation < 0 || k.WinReputation > 500 || k.LossReputation > 500) p.Add("replication.winPrestige, lossPrestige and withholdPrestige must not be negative and winReputation and lossReputation from 0 to 500");
        }

        static void TechBetsProblems(List<string> p, GameConfig c)
        {
            TechBetsConfig k = c.TechBets;
            if (k == null) { p.Add("techBets is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("techBets.enabled must be 0 or 1");
            if (k.Weeks < 1 || k.MinCost < 0) p.Add("techBets.weeks must be at least 1 and minCost not negative");
            if (k.WinReputation < 0 || k.WinReputation > 500 || k.LossReputation < 0 || k.LossReputation > 500 || k.WithdrawReputation < 0 || k.WithdrawReputation > 500 || k.WinPrestige < 0) p.Add("techBets.winReputation, lossReputation and withdrawReputation must be from 0 to 500 and winPrestige not negative");
        }

        static void WaitlistProblems(List<string> p, GameConfig c)
        {
            WaitlistConfig k = c.Waitlist;
            if (k == null) { p.Add("waitlist is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("waitlist.enabled must be 0 or 1");
            if (k.MinWeeks < 1 || k.MaxWeeks < k.MinWeeks) p.Add("waitlist.minWeeks must be at least 1 and maxWeeks at least minWeeks");
            if (k.HypeStep < 0 || k.HypeStep > 1000 || k.WinPerThousand < 0 || k.WinPerThousand > 1000 || k.LossPerThousand < 0 || k.LossPerThousand > 1000 || k.ClosePerThousand < 0 || k.ClosePerThousand > 1000) p.Add("waitlist.hypeStep and the per-thousand values must be from 0 to 1000");
        }

        static void BrandProblems(List<string> p, GameConfig c)
        {
            BrandConfig k = c.Brand;
            if (k == null) { p.Add("brand is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("brand.enabled must be 0 or 1");
            if (k.OpenCost < 0 || k.RepairCostPerStaff < 0) p.Add("brand.openCost and repairCostPerStaff must not be negative");
            if (k.Start < 0 || k.Start > 1000 || k.LowBar < 0 || k.LowBar > 1000 || k.MaxFeePermille < 0 || k.MaxFeePermille > 900) p.Add("brand.start and lowBar must be from 0 to 1000 and maxFeePermille from 0 to 900");
            if (k.KeptPoints < 0 || k.BrokenPoints < 0 || k.RepairPoints < 0 || k.IgnorePoints < 0 || k.ReviewWeeks < 1 || k.RebutReputation < 0 || k.RebutReputation > 500) p.Add("brand points must not be negative, reviewWeeks at least 1 and rebutReputation from 0 to 500");
        }

        static void FreeTierProblems(List<string> p, GameConfig c)
        {
            FreeTierConfig k = c.FreeTier;
            if (k == null) { p.Add("freeTier is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("freeTier.enabled must be 0 or 1");
            if (k.LiftA < 0 || k.LiftB < k.LiftA || k.LiftC < k.LiftB || k.LiftC > 900) p.Add("freeTier.liftA, liftB and liftC must not fall and liftC be at most 900");
            if (k.StepA < 0 || k.StepB < 0 || k.StepC < 0 || k.CostA < 0 || k.CostB < 0 || k.CostC < 0 || k.CloseLoss < 0 || k.CloseLoss > 500) p.Add("freeTier steps and costs must not be negative and closeLoss from 0 to 500");
        }

        static void LaunchWatchProblems(List<string> p, GameConfig c)
        {
            LaunchWatchConfig k = c.LaunchWatch;
            if (k == null) { p.Add("launchWatch is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("launchWatch.enabled must be 0 or 1");
            if (k.OpenCost < 0 || k.WeeklyCost < 0 || k.CounterPerStaff < 0 || k.WaitWeeks < 1) p.Add("launchWatch costs must not be negative and waitWeeks at least 1");
            if (k.CounterKeepPermille < 0 || k.CounterKeepPermille > 1000 || k.SafetyBar < 0 || k.SafetyBar > 1000) p.Add("launchWatch.counterKeepPermille and safetyBar must be from 0 to 1000");
            if (k.LossReputation < 0 || k.LossReputation > 500 || k.SafetyGain < 0 || k.SafetyGain > 500 || k.LeadGain < 0 || k.LeadGain > 500) p.Add("launchWatch reputation values must be from 0 to 500");
        }

        static void ProgrammesProblems(List<string> p, GameConfig c)
        {
            ProgrammesConfig k = c.Programmes;
            if (k == null) { p.Add("programmes is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("programmes.enabled must be 0 or 1");
            if (k.Stages < 1 || k.Stages > 6 || k.MinCost < 0 || k.SafeWeeks < 1 || k.BoldWeeks < 1) p.Add("programmes.stages must be from 1 to 6, minCost not negative and both weeks at least 1");
            if (k.SafeCostPermille < 0 || k.BoldCostPermille < 0 || k.BoldChancePermille < 1 || k.BoldChancePermille > 999 || k.InsightPermille < 0 || k.InsightPrestige < 0 || k.RefundPermille < 0 || k.RefundPermille > 1000) p.Add("programmes costs must not be negative, boldChancePermille from 1 to 999 and refundPermille from 0 to 1000");
        }

        static void RunQueueProblems(List<string> p, GameConfig c)
        {
            RunQueueConfig k = c.RunQueue;
            if (k == null) { p.Add("runQueue is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("runQueue.enabled must be 0 or 1");
            if (k.WeightBackground < 1 || k.WeightNormal < k.WeightBackground || k.WeightRush < k.WeightNormal || k.WeightRush > 100) p.Add("runQueue weights must be at least 1, rising from background to rush, and at most 100");
            if (k.SwitchCostPermille < 0 || k.SwitchCostPermille > 500 || k.MaxRush < 1) p.Add("runQueue.switchCostPermille must be from 0 to 500 and maxRush at least 1");
        }

        static void PodsProblems(List<string> p, GameConfig c)
        {
            PodsConfig k = c.Pods;
            if (k == null) { p.Add("pods is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("pods.enabled must be 0 or 1");
            if (k.SharpChem < 0 || k.SharpChem > 1000 || k.ProgrammeWeeksSaved < 0) p.Add("pods.sharpChem must be from 0 to 1000 and programmeWeeksSaved not negative");
            if (k.MinSize < 2 || k.MaxSize < k.MinSize || k.MaxSize > 10) p.Add("pods.minSize must be at least 2, maxSize at least minSize and at most 10");
            if (k.KickoffCost < 0 || k.PointsPerMember < 0 || k.SwapCooldown < 0 || k.TenureStep < 0 || k.TenureCap < 0) p.Add("pods costs, points, steps and cooldown must not be negative");
            if (k.BaseChem < 0 || k.BaseChem > 1000 || k.RolePoints < 0 || k.MixPoints < 0 || k.ClashPoints < 0 || k.MoraleShare < 0 || k.MoraleShare > 1000) p.Add("pods chemistry values must not be negative, baseChem and moraleShare at most 1000");
        }

        static void LicenceProblems(List<string> p, GameConfig c)
        {
            LicenceConfig k = c.Licence;
            if (k == null) { p.Add("licence is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("licence.enabled must be 0 or 1");
            if (k.RushPenalty < 0 || k.RushPenalty > 1000 || k.BreachStrike < 0 || k.BreachStrike > 1) p.Add("licence.rushPenalty must be from 0 to 1000 and breachStrike 0 or 1");
            if (k.EraA < 0 || k.EraB < k.EraA || k.EraC < k.EraB || k.EraC >= c.Eras.Count) p.Add("licence eras must not fall and the last must be a real era");
            if (k.FeeA < 0 || k.FeeB < k.FeeA || k.FeeC < k.FeeB || k.TermA < 1 || k.TermB < 1 || k.TermC < 1 || k.ReviewWeeks < 1 || k.CooldownWeeks < 0 || k.RenewWindow < 0) p.Add("licence fees must not fall, terms and reviewWeeks at least 1, cooldownWeeks and renewWindow not negative");
            if (k.BarA < 0 || k.BarB < k.BarA || k.BarC < k.BarB || k.BarC > 1000 || k.CutA < 0 || k.CutB < k.CutA || k.CutC < k.CutB || k.CutC > 900) p.Add("licence bars and cuts must not fall, bars at most 1000 and cuts at most 900");
            if (k.StrikeLimit < 1 || k.SuspendReputation < 0 || k.SuspendReputation > 500) p.Add("licence.strikeLimit must be at least 1 and suspendReputation from 0 to 500");
        }

        static void PledgesProblems(List<string> p, GameConfig c)
        {
            PledgesConfig k = c.Pledges;
            if (k == null) { p.Add("pledges is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("pledges.enabled must be 0 or 1");
            if (k.PodCutPermille < 0 || k.PodCutPermille > 900) p.Add("pledges.podCutPermille must be from 0 to 900");
            if (k.MinWeeks < 26 || k.MaxWeeks < k.MinWeeks || k.MaxPledges < 1) p.Add("pledges.minWeeks must be at least 26, maxWeeks not below it and maxPledges at least 1");
            if (k.LiftPermilleStep < 0 || k.LiftCapPermille < 0 || k.LiftCapPermille > 1000 || k.SupportPermille < 0 || k.SupportPermille > 900) p.Add("pledges lift and support must not be negative, the cap at most 1000 and support at most 900");
            if (k.KeptTrust < 0 || k.BreachTrust < 0 || k.BreachWeeks < 0 || k.BreachReputation < 0 || k.BreachReputation > 500) p.Add("pledges.keptTrust, breachTrust and breachWeeks must not be negative and breachReputation from 0 to 500");
        }

        static void IntegrationsProblems(List<string> p, GameConfig c)
        {
            IntegrationsConfig k = c.Integrations;
            if (k == null) { p.Add("integrations is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("integrations.enabled must be 0 or 1");
            if (k.MaxOpen < 1 || k.MaxIntegrations < 1 || k.SeedPerWeek < 1 || k.GrowthPermille < 0 || k.GrowthPermille > 1000) p.Add("integrations.maxOpen, maxIntegrations and seedPerWeek must be at least 1 and growthPermille from 0 to 1000");
            if (k.LiftPermillePer10 < 0 || k.LiftCapPermille < 0 || k.LiftCapPermille > 1000 || k.UpkeepBasePermille < 0 || k.UpkeepPer10Permille < 0 || k.UpkeepBasePermille > 900) p.Add("integrations lift and upkeep must not be negative, the cap at most 1000 and upkeepBasePermille at most 900");
            if (k.MigratePermille < 1 || k.MigratePermille > 1000 || k.ChurnPermille < 0 || k.ChurnPermille > 1000 || k.MinWeeks < 1 || k.MaxWeeks < k.MinWeeks || k.StrandTrustPer100 < 0 || k.StrandTrustCap < 0) p.Add("integrations.migratePermille must be from 1 to 1000, churnPermille from 0 to 1000, minWeeks at least 1, maxWeeks not below it, and the stranding trust not negative");
            if (k.IncidentPermille < 0 || k.IncidentPermille > 1000) p.Add("integrations.incidentPermille must be from 0 to 1000");
        }

        static void ConformityProblems(List<string> p, GameConfig c)
        {
            ConformityConfig k = c.Conformity;
            if (k == null) { p.Add("conformity is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("conformity.enabled must be 0 or 1");
            if (k.MaxActive < 1 || k.DossierWeeks < 1 || k.ReviewWeeks < 1 || k.TermWeeks < 2 || k.SurveillanceAt < 1 || k.SurveillanceAt >= k.TermWeeks || k.ResubmitWeeks < 0) p.Add("conformity.maxActive, dossierWeeks and reviewWeeks must be at least 1, termWeeks at least 2, surveillanceAt inside the term and resubmitWeeks not negative");
            if (k.DossierPoints < 0 || k.BroadExtraPoints < 0 || k.BarNarrow < 0 || k.BarBroad < k.BarNarrow || k.BarBroad > 1000 || k.MaxGap < 0) p.Add("conformity costs must not be negative, barBroad not below barNarrow and at most 1000");
            if (k.CutNarrow < 0 || k.CutBroad < k.CutNarrow || k.CutBroad > 900 || k.TrustGain < 0 || k.SuspendTrust < 0) p.Add("conformity cuts must not fall and cutBroad at most 900; trust values not negative");
        }

        static void OrgProblems(List<string> p, GameConfig c)
        {
            OrgConfig k = c.Org;
            if (k == null) { p.Add("org is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("org.enabled must be 0 or 1");
            if (k.FlatCap < 0 || k.FlatStep < 0 || k.SpanSize < 1 || k.SpanStep < 0 || k.LeadlessLoss < 0 || k.FuncResearchBonus < 0) p.Add("org.spanSize must be at least 1 and the flat, span and function values not negative");
            if (k.SquadSize < 2 || k.SquadMinRoles < 1 || k.SquadLeadLoss < 0 || k.SquadTrainingBonus < 0 || k.CrossTeam < 0) p.Add("org.squadSize must be at least 2, squadMinRoles at least 1 and the squad values not negative");
            if (k.LossCap < 0 || k.LossCap > 900 || k.ReorgWeeks < 0 || k.DisruptLoss < 0 || k.CooldownWeeks < 0) p.Add("org.lossCap must be from 0 to 900 and the reorganisation values not negative");
        }

        static void FabricProblems(List<string> p, GameConfig c)
        {
            FabricConfig k = c.Fabric;
            if (k == null) { p.Add("fabric is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("fabric.enabled must be 0 or 1");
            if (k.MinGpus < 1 || k.EraA < 0 || k.EraB < k.EraA || k.EraC < k.EraB || k.EraC >= c.Eras.Count) p.Add("fabric.minGpus must be at least 1 and the eras must not fall and the last must be a real era");
            if (k.LeadA < 1 || k.LeadB < 1 || k.LeadC < 1 || k.CostA < 0 || k.CostB < k.CostA || k.CostC < k.CostB || k.GainA < 0 || k.GainB < k.GainA || k.GainC < k.GainB) p.Add("fabric leads must be at least 1 and costs and gains must not fall from tier to tier");
            if (k.DepositPermille < 0 || k.DepositPermille > 1000 || k.CutoverWeeks < 0 || k.CutoverWeeks > k.LeadA || k.CutoverLoss < 0 || k.CutoverLoss > 900) p.Add("fabric.depositPermille must be from 0 to 1000, cutoverWeeks no more than leadA and cutoverLoss from 0 to 900");
            if (k.SquadCutPermille < 0 || k.SquadCutPermille > 1000) p.Add("fabric.squadCutPermille must be from 0 to 1000");
        }

        static void DisclosureProblems(List<string> p, GameConfig c)
        {
            DisclosureConfig k = c.Disclosure;
            if (k == null) { p.Add("disclosure is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("disclosure.enabled must be 0 or 1");
            if (k.MaxOpen < 1 || k.ReportWeeks < 1 || k.FullWeeks < 1 || k.FullStaff < 0 || k.InspectWeeks < 1 || k.InspectLimit < 1) p.Add("disclosure.maxOpen, reportWeeks, fullWeeks, inspectWeeks and inspectLimit must be at least 1 and fullStaff not negative");
            if (k.TrustGain < 0 || k.TrustFull < k.TrustGain || k.LateFine < 0 || k.InspectFine < 0) p.Add("disclosure.trustFull must not be below trustGain and the fines must not be negative");
            if (k.CertifiedFindings < 0) p.Add("disclosure.certifiedFindings must not be negative");
            if (k.OrgFasterWeeks < 0) p.Add("disclosure.orgFasterWeeks must not be negative");
        }

        static void DatasetsProblems(List<string> p, GameConfig c)
        {
            DatasetsConfig k = c.Datasets;
            if (k == null) { p.Add("datasets is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("datasets.enabled must be 0 or 1");
            if (k.MaxBuy < 1 || k.StockCap < k.MaxBuy || k.CleanWeeks < 1 || k.CleanMax < 1 || k.PerEngineer < 1 || k.SuitWeeks < 1 || k.SuitEra < 0 || k.SuitEra >= c.Eras.Count) p.Add("datasets.maxBuy, cleanWeeks, cleanMax, perEngineer and suitWeeks must be at least 1, stockCap at least maxBuy and suitEra a real era");
            if (k.CostScrape < 0 || k.CostLicense < 0 || k.CostSynth < 0 || k.CleanCost < 0 || k.SuitFine < 0 || k.BackfillCost < 0 || k.BonusPer100 < 0 || k.CollapseSlope < 0) p.Add("datasets costs, fines and bonuses must not be negative");
            if (k.YieldScrape < 0 || k.YieldScrape > 1000 || k.YieldLicense < 0 || k.YieldLicense > 1000 || k.YieldSynth < 0 || k.YieldSynth > 1000 || k.YieldUsers < 0 || k.YieldUsers > 1000 || k.QScrape < 0 || k.QScrape > 1000 || k.QLicense < 0 || k.QLicense > 1000 || k.QSynth < 0 || k.QSynth > 1000 || k.QUsers < 0 || k.QUsers > 1000) p.Add("datasets yields and values must be from 0 to 1000");
            if (k.BonusCap < 0 || k.BonusCap > 900 || k.CollapseShare < 0 || k.CollapseShare > 1000 || k.CollapseCap < 0 || k.CollapseCap > 1000) p.Add("datasets.bonusCap must be from 0 to 900 and the collapse values from 0 to 1000");
            if (k.DirectorSuitPermille < 0 || k.DirectorSuitPermille > 300) p.Add("datasets.directorSuitPermille must be from 0 to 300");
            if (k.WaveStallsCleaning < 0 || k.WaveStallsCleaning > 1) p.Add("datasets.waveStallsCleaning must be 0 or 1");
        }

        static void UserDataProblems(List<string> p, GameConfig c)
        {
            UserDataConfig k = c.UserData;
            if (k == null) { p.Add("userData is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("userData.enabled must be 0 or 1");
            if (k.RevenuePerUnit < 1 || k.LockWeeks < 0 || k.ComplaintWeeks < 1 || k.ComplaintMin < 1) p.Add("userData.revenuePerUnit, complaintWeeks and complaintMin must be at least 1 and lockWeeks not negative");
            if (k.OptInPermille < 0 || k.OptInPermille > 1000 || k.DefaultPermille < k.OptInPermille || k.DefaultPermille > 1000 || k.KeepPermille < 0 || k.KeepPermille > 1000) p.Add("userData.optInPermille must not be above defaultPermille and shares must be from 0 to 1000");
            if (k.ComplaintResigns < 0) p.Add("userData.complaintResigns must not be negative");
            if (k.ComplaintFine < 0 || k.ComplaintTrust < 0) p.Add("userData fines and trust losses must not be negative");
        }

        static void GovernanceProblems(List<string> p, GameConfig c)
        {
            GovernanceConfig k = c.Governance;
            if (k == null) { p.Add("governance is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("governance.enabled must be 0 or 1");
            if (k.DualMaxRounds < 0 || k.NonprofitMaxRounds < 0 || k.DualVoteMult < 1000 || k.DirectorWeeks < 1 || k.MaxDirectors < 0 || k.MaxDirectors > 9) p.Add("governance rounds must not be negative, dualVoteMult at least 1000, directorWeeks at least 1 and maxDirectors from 0 to 9");
            if (k.DualCashCut < 0 || k.DualCashCut > 900 || k.NonprofitCashCut < 0 || k.NonprofitCashCut > 900 || k.DramaCut < 0 || k.DramaCut > 1000) p.Add("governance cuts must be from 0 to 900 and dramaCut from 0 to 1000");
            if (k.DirectorFee < 0 || k.DirectorWeekly < 0 || k.DirectorMinRep < 0 || k.DirectorMinRep > 1000) p.Add("governance fees must not be negative and directorMinRep must be from 0 to 1000");
        }

        static void BuybackProblems(List<string> p, GameConfig c)
        {
            BuybackConfig k = c.Buyback;
            if (k == null) { p.Add("buyback is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("buyback.enabled must be 0 or 1");
            if (k.VoteCooldown < 0) p.Add("buyback.voteCooldown must not be negative");
            if (k.DirectorVote < 0 || k.DirectorVote > 500) p.Add("buyback.directorVote must be from 0 to 500");
            if (k.SpendCapPermille < 0 || k.SpendCapPermille > 1000) p.Add("buyback.spendCapPermille must be from 0 to 1000");
            if (k.RunwayWeeks < 0) p.Add("buyback.runwayWeeks must not be negative");
            if (k.PassPermille < 1 || k.PassPermille > 999) p.Add("buyback.passPermille must be from 1 to 999");
            if (k.WaveBlocksDirectors < 0 || k.WaveBlocksDirectors > 1) p.Add("buyback.waveBlocksDirectors must be 0 or 1");
            if (k.PriceMult < 1000 || k.PriceMult > 20000 || k.BubblePrice < 0 || k.BubblePrice > 5000 || k.BuybackTrust < 0) p.Add("buyback priceMult must be from 1000 to 20000, bubblePrice from 0 to 5000 and buybackTrust not negative");
        }

        static void SpotProblems(List<string> p, GameConfig c)
        {
            SpotConfig k = c.Spot;
            if (k == null) { p.Add("spot is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("spot.enabled must be 0 or 1");
            if (k.ReclaimWeeks < 1) p.Add("spot.reclaimWeeks must be at least 1");
            if (k.MaxPermille < 0 || k.MaxPermille > 1000 || k.DiscountPermille < 0 || k.DiscountPermille > 900 || k.PreemptPermille < 0 || k.PreemptPermille > 1000 || k.DefaultLossPermille < 0 || k.DefaultLossPermille > 1000 || k.ChangeWeeks < 0) p.Add("spot shares must be from 0 to 1000 (discount to 900) and changeWeeks not negative");
        }

        static void AgencyProblems(List<string> p, GameConfig c)
        {
            AgencyConfig k = c.Agency;
            if (k == null) { p.Add("agency is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("agency.enabled must be 0 or 1");
            if (k.GapBlowup < 0 || k.GapBlowup > 1000) p.Add("agency.gapBlowup must be from 0 to 1000");
            if (k.MinEra < 0 || k.SetupCost < 0 || k.Rooms2 < 0 || k.Rooms3 < k.Rooms2 || k.MinAlign3 < 0 || k.MinAlign3 > 1000 || k.Cooldown < 0) p.Add("agency minEra, setupCost and cooldown must not be negative, rooms3 at least rooms2 and minAlign3 from 0 to 1000");
            if (k.Lift1 < 0 || k.Lift2 < k.Lift1 || k.Lift3 < k.Lift2 || k.Lift3 > 2000 || k.Incident1 < 0 || k.Incident2 < k.Incident1 || k.Incident3 < k.Incident2 || k.Incident3 > 1000) p.Add("agency lifts and incident chances must rise with the level, lift3 at most 2000 and incident3 at most 1000");
            if (k.Blowup2 < 0 || k.Blowup3 < k.Blowup2 || k.Blowup3 > 1000 || k.RoomCut < 0 || k.RoomCut > 1000 || k.RefundWeeks < 0 || k.BlowupTrust < 0 || k.StreakWeeks < 1 || k.StreakTrust < 0) p.Add("agency blow-up chances must rise with the level and not pass 1000, roomCut from 0 to 1000, streakWeeks at least 1 and the rest not negative");
        }

        static void AnnotationProblems(List<string> p, GameConfig c)
        {
            AnnotationConfig k = c.Annotation;
            if (k == null) { p.Add("annotation is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("annotation.enabled must be 0 or 1");
            if (k.ScandalTension < 0 || k.ScandalTension > 1000) p.Add("annotation.scandalTension must be from 0 to 1000");
            if (k.Weekly1 < 0 || k.Weekly2 < 0 || k.Weekly3 < 0 || k.Bonus1 < 0 || k.Bonus2 < 0 || k.Bonus3 < 0 || k.Bonus1 > 500 || k.Bonus2 > 500 || k.Bonus3 > 500 || k.Cap1 < 1 || k.Cap2 < 1 || k.Cap3 < 1) p.Add("annotation fees and bonuses must not be negative, bonuses at most 500, and sizes at least 1");
            if (k.Risk1 < 0 || k.Risk2 < 0 || k.Risk3 < 0 || k.Risk1 > 1000 || k.Term < 1 || k.EarlyPermille < 0 || k.EarlyPermille > 1000 || k.AuditCost < 0 || k.AuditCut < 0 || k.AuditCut > 1000 || k.AuditWeeks < 0) p.Add("annotation risks from 0 to 1000, term at least 1, early and audit cut from 0 to 1000, and costs not negative");
            if (k.ExposeDiv < 1 || k.ExposeFine < 0 || k.ExposeTrust < 0 || k.ExposeReset < 0 || k.ExposeReset > 1000) p.Add("annotation exposeDiv must be at least 1, fine and trust not negative and exposeReset from 0 to 1000");
        }

        static void PostTrainingProblems(List<string> p, GameConfig c)
        {
            PostTrainingConfig k = c.PostTraining;
            if (k == null) { p.Add("postTraining is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("postTraining.enabled must be 0 or 1");
            if (k.DocsCutSyco < 0 || k.DocsCutSyco > 250) p.Add("postTraining.docsCutSyco must be from 0 to 250");
            if (k.MinEra < 0 || k.Weeks < 1 || k.Cost < 0 || k.Units < 1 || k.MaxPasses < 1 || k.MaxPasses > 8) p.Add("postTraining minEra and cost must not be negative, weeks and units at least 1 and maxPasses from 1 to 8");
            if (k.PrefLift < 0 || k.PrefLift > 1000 || k.PrefDecay < 0 || k.PrefDecay > 1000 || k.Syco < 0 || k.Syco > 1000 || k.SafetyGain < 0 || k.SafetyGain > 1000 || k.SafetyDecay < 0 || k.SafetyDecay > 1000 || k.Refuse < 0 || k.Refuse > 1000 || k.MinLift > 0 || k.MinLift < -1000) p.Add("postTraining lifts, decays, syco, safetyGain and refuse from 0 to 1000 and minLift from -1000 to 0");
        }

        static void KnowledgeProblems(List<string> p, GameConfig c)
        {
            KnowledgeConfig k = c.Knowledge;
            if (k == null) { p.Add("knowledge is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("knowledge.enabled must be 0 or 1");
            if (k.MaxOwned < 1 || k.MaxDocs < 0 || k.MaxDocs > 4 || k.DocCost < 0 || k.DocStale < 1) p.Add("knowledge maxOwned and docStale must be at least 1, maxDocs from 0 to 4 and docCost not negative");
            if (k.DocCut < 0 || k.DocCut * k.MaxDocs > 1000 || k.GapBase < 0 || k.GapBase > 1000 || k.Ramp < 0 || k.GapIncident < 0 || k.GapIncident > 1000 || k.GapRiskAt < 0 || k.GapRiskAt > 1000) p.Add("knowledge docCut times maxDocs at most 1000, gapBase, gapIncident and gapRiskAt from 0 to 1000 and ramp not negative");
        }

        static void BoardAgendaProblems(List<string> p, GameConfig c)
        {
            BoardAgendaConfig k = c.BoardAgenda;
            if (k == null) { p.Add("boardAgenda is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("boardAgenda.enabled must be 0 or 1");
            if (k.BlowupTension < 0 || k.BlowupTension > 1000) p.Add("boardAgenda.blowupTension must be from 0 to 1000");
            if (k.MinSeats < 1 || k.MeetingWeeks < 1 || k.AnswerWeeks < 1 || k.CommitWeeks < 1 || k.MeetingWeeks < k.AnswerWeeks) p.Add("boardAgenda minSeats, meetingWeeks, answerWeeks and commitWeeks must be at least 1 and a meeting no sooner than the answer");
            if (k.AcceptRelief < 0 || k.KeptRelief < 0 || k.DeclineTension < 0 || k.IgnoreTension < 0 || k.BrokenTension < 0 || k.DirectorCut < 0 || k.DirectorCut > 1000 || k.OustAt < 1 || k.OustAt > 1000 || k.OustMargin < 0 || k.OustMargin > 1000 || k.CalmBelow < 0 || k.CalmBelow > 1000) p.Add("boardAgenda tensions must not be negative, directorCut, oustMargin and calmBelow from 0 to 1000 and oustAt from 1 to 1000");
            if (k.RaiseBonus < 0 || k.RaiseBonus > 1000 || k.RunwayStretch < 0 || k.RunwayFloor < 0 || k.SafetyAdd < 1) p.Add("boardAgenda raiseBonus from 0 to 1000, runway values not negative and safetyAdd at least 1");
        }

        static void DrillsProblems(List<string> p, GameConfig c)
        {
            DrillsConfig k = c.Drills;
            if (k == null) { p.Add("drills is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("drills.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.Cost < 0 || k.Weeks < 1 || k.Interval < 1 || k.Staff < 1 || k.PassGain < 0 || k.FailLoss < 0 || k.Decay < 0 || k.IdleDecay < 0 || k.IncidentCut < 0 || k.IncidentCut > 1000 || k.RepCut < 0 || k.RepCut > 1000 || k.OnCallRepStack < 0 || k.OnCallRepStack > 1000) p.Add("drills minEra, cost and decays must not be negative, weeks and interval and staff at least 1, cuts from 0 to 1000, and onCallRepStack from 0 to 1000");
        }

        static void EvalHarnessProblems(List<string> p, GameConfig c)
        {
            EvalHarnessConfig k = c.EvalHarness;
            if (k == null) { p.Add("evalHarness is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("evalHarness.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.Cost < 0 || k.Cadence < 1 || k.Points < 0 || k.TrustGain < 0 || k.TrustLoss > 0 || k.BarNarrow < 0 || k.BarNarrow > 1000 || k.BarBroad < k.BarNarrow || k.BarBroad > 1000 || k.ConformityPush < 0 || k.FailTension < 0) p.Add("evalHarness minEra and costs not negative, cadence at least 1, trustLoss not positive, bars from 0 to 1000 with broad at least narrow, conformityPush and failTension not negative");
        }

        static void ToolPolicyProblems(List<string> p, GameConfig c)
        {
            ToolPolicyConfig k = c.ToolPolicy;
            if (k == null) { p.Add("toolPolicy is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("toolPolicy.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.SetupCost < 0 || k.LiftWeb < 0 || k.LiftCode < 0 || k.LiftData < 0 || k.IncidentWeb < 0 || k.IncidentCode < 0 || k.IncidentData < 0 || k.BlowupWeb < 0 || k.BlowupCode < 0 || k.BlowupData < 0 || k.QueueWeb < 0 || k.QueueCode < 0 || k.QueueData < 0) p.Add("toolPolicy costs and shares must not be negative");
        }

        static void HumanQueueProblems(List<string> p, GameConfig c)
        {
            HumanQueueConfig k = c.HumanQueue;
            if (k == null) { p.Add("humanQueue is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("humanQueue.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.StaffNeed < 1 || k.Capacity < 0 || k.LiftCut < 0 || k.LiftCut > 1000 || k.IncidentPer < 0 || k.ClearCost < 0 || k.BusyWeeks < 1) p.Add("humanQueue minEra and costs not negative, staffNeed and busyWeeks at least 1, capacity not negative, liftCut from 0 to 1000 and incidentPer not negative");
        }

        static void ReleaseChecklistProblems(List<string> p, GameConfig c)
        {
            ReleaseChecklistConfig k = c.ReleaseChecklist;
            if (k == null) { p.Add("releaseChecklist is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("releaseChecklist.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.DocBar < 0 || k.DocBar > 10 || k.MissTrust > 0) p.Add("releaseChecklist minEra and docBar from 0 to 10 and missTrust not positive");
        }

        static void AgentSandboxProblems(List<string> p, GameConfig c)
        {
            AgentSandboxConfig k = c.AgentSandbox;
            if (k == null) { p.Add("agentSandbox is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("agentSandbox.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.Cost < 0 || k.Weeks < 1 || k.Staff < 0 || k.PassBar < 0 || k.PassBar > 1000) p.Add("agentSandbox weeks at least 1, costs not negative, passBar from 0 to 1000");
        }

        static void RunawayPlaybookProblems(List<string> p, GameConfig c)
        {
            RunawayPlaybookConfig k = c.RunawayPlaybook;
            if (k == null) { p.Add("runawayPlaybook is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("runawayPlaybook.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.MaxLevel < 0 || k.MaxLevel > 5 || k.SetupPerLevel < 0 || k.Staff < 0 || k.CooldownWeeks < 0 || k.ArmWeeks < 1 || k.BlowupCut < 0 || k.TrustKeep < 0 || k.TrustKeep > 1000) p.Add("runawayPlaybook level and timing fields in range");
        }

        static void OrchestrationBudgetProblems(List<string> p, GameConfig c)
        {
            OrchestrationBudgetConfig k = c.OrchestrationBudget;
            if (k == null) { p.Add("orchestrationBudget is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("orchestrationBudget.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.SetupPerLevel < 0 || k.Slots1 < 0 || k.Slots2 < 0 || k.Slots3 < 0 || k.Keep1 < 0 || k.Keep1 > 1000 || k.Keep2 < 0 || k.Keep2 > 1000 || k.Keep3 < 0 || k.Keep3 > 1000 || k.OverDecay < 0 || k.QueuePerOver < 0) p.Add("orchestrationBudget slots and keep permille in range");
        }

        static void TrafficRouterProblems(List<string> p, GameConfig c)
        {
            TrafficRouterConfig k = c.TrafficRouter;
            if (k == null) { p.Add("trafficRouter is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("trafficRouter.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.SkewBar < 0 || k.SkewBar > MathI.Ppm || k.OverloadAdd < 0 || k.ServedBelow < 0 || k.ServedBelow > 1000) p.Add("trafficRouter skew and serving bars in range");
        }

        static void PublicMetricsProblems(List<string> p, GameConfig c)
        {
            PublicMetricsConfig k = c.PublicMetrics;
            if (k == null) { p.Add("publicMetrics is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("publicMetrics.enabled must be 0 or 1");
            if (k.MinEra < 0 || k.PublishCost < 0 || k.Bar < 0 || k.Bar > 1000 || k.ConformityPush < 0) p.Add("publicMetrics costs and bar in range");
        }

        static void BugBountyProblems(List<string> p, GameConfig c)
        {
            BugBountyConfig k = c.BugBounty;
            if (k == null) { p.Add("bugBounty is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 5) p.Add("bugBounty.maxLevel must be from 0 to 5 (0 turns the bounty off)");
            if (k.SetupPerLevel < 0) p.Add("bugBounty.setupPerLevel must not be negative");
            if (k.WeeklyPerLevel < 1) p.Add("bugBounty.weeklyPerLevel must be at least 1, so the programme is never free to run");
            if (k.CutPerLevel < 0 || k.CutPerLevel * (long)k.MaxLevel > 900) p.Add("bugBounty.cutPerLevel times maxLevel must not pass 900, so a bounty never ends incidents");
            if (k.CooldownWeeks < 0 || k.CooldownWeeks > 104) p.Add("bugBounty.cooldownWeeks must be from 0 to 104");
        }

        static void WhistleProblems(List<string> p, GameConfig c)
        {
            WhistleConfig k = c.Whistle;
            if (k == null) { p.Add("whistle is missing"); return; }
            if (k.Enabled < 0 || k.Enabled > 1) p.Add("whistle.enabled must be 0 or 1");
            if (c.Role(k.RoleId) == null) p.Add("whistle.roleId must be a role of the game");
            if (k.GapPermille < 1 || k.GapPermille > 1000) p.Add("whistle.gapPermille must be from 1 to 1000");
            if (k.MoraleBelow < 0 || k.MoraleBelow > 1000) p.Add("whistle.moraleBelow must be from 0 to 1000");
            if (k.WeeksToConcern < 1 || k.WeeksToConcern > 52) p.Add("whistle.weeksToConcern must be from 1 to 52");
            if (k.AnswerWeeks < 1 || k.AnswerWeeks > 26) p.Add("whistle.answerWeeks must be from 1 to 26");
            if (k.CleanReputation < 0 || k.CleanReputation > 500) p.Add("whistle.cleanReputation must be from 0 to 500");
            if (k.CleanMorale < 0 || k.CleanMorale > 500) p.Add("whistle.cleanMorale must be from 0 to 500");
            if (k.SilenceFee < 1) p.Add("whistle.silenceFee must be at least 1, so silence is never free");
            if (k.SilenceMorale < 0 || k.SilenceMorale > 500) p.Add("whistle.silenceMorale must be from 0 to 500");
            if (k.LeakReputation < k.CleanReputation || k.LeakReputation > 500) p.Add("whistle.leakReputation must be from cleanReputation to 500, so waiting is never cheaper than coming clean");
            if (k.LeakPerSilence < 0 || k.LeakPerSilence > 200) p.Add("whistle.leakPerSilence must be from 0 to 200");
            if (k.LeakMorale < 0 || k.LeakMorale > 500) p.Add("whistle.leakMorale must be from 0 to 500");
        }

        static void ChairsProblems(List<string> p, GameConfig c)
        {
            ChairsConfig k = c.Chairs;
            if (k == null) { p.Add("chairs is missing"); return; }
            if (k.MaxChairs < 0 || k.MaxChairs > 10) p.Add("chairs.maxChairs must be from 0 to 10 (0 turns chairs off)");
            if (k.CostBase < 1) p.Add("chairs.costBase must be at least 1, so a chair is never free");
            if (k.StipendPerWeek < 0) p.Add("chairs.stipendPerWeek must not be negative");
            if (k.ResearchPerWeek < 0 || k.ResearchPerWeek > 200) p.Add("chairs.researchPerWeek must be from 0 to 200");
            if (k.PrestigeFloor < 0 || (c.Prestige.Max > 0 && k.PrestigeFloor * (long)k.MaxChairs >= c.Prestige.Max)) p.Add("chairs.prestigeFloor times maxChairs must stay under the most prestige, so papers still matter");
            if (k.MinResearchers < 0 || k.MinResearchers > 50) p.Add("chairs.minResearchers must be from 0 to 50");
        }

        static void ResearchGrantsProblems(List<string> p, GameConfig c)
        {
            ResearchGrantsConfig k = c.ResearchGrants;
            if (k == null) { p.Add("researchGrants is missing"); return; }
            if (k.MaxTier < 0 || k.MaxTier > 5) p.Add("researchGrants.maxTier must be from 0 to 5");
            if (k.MinResearchers < 0 || k.MinResearchers > 100) p.Add("researchGrants.minResearchers must be from 0 to 100");
            if (k.ApplyCostPerTier < 0) p.Add("researchGrants.applyCostPerTier is negative");
            if (k.AwardPerTier < k.ApplyCostPerTier) p.Add("researchGrants.awardPerTier must be at least applyCostPerTier, or no grant could pay for its own application");
            if (k.BarPerTier < 0 || k.BarPerTier * (long)k.MaxTier > 2000) p.Add("researchGrants.barPerTier must be 0 or more, and the top tier's bar at most 2000");
            if (k.ReputationWeightPermille < 0 || k.ReputationWeightPermille > MathI.Permille) p.Add("researchGrants.reputationWeightPermille must be from 0 to 1000");
            if (k.ReviewWeeks < 1 || k.ReviewWeeks > 52) p.Add("researchGrants.reviewWeeks must be from 1 to 52");
            if (k.DeliverWeeks < 1 || k.DeliverWeeks > 104) p.Add("researchGrants.deliverWeeks must be from 1 to 104");
            if (k.ClawbackPermille < 0 || k.ClawbackPermille > MathI.Permille) p.Add("researchGrants.clawbackPermille must be from 0 to 1000");
            if (k.CompletedReputation < 0 || k.CompletedReputation > 100) p.Add("researchGrants.completedReputation must be from 0 to 100");
            if (k.FailedReputation < 0 || k.FailedReputation > 200) p.Add("researchGrants.failedReputation must be from 0 to 200");
            if (k.CooldownWeeks < 0 || k.CooldownWeeks > 104) p.Add("researchGrants.cooldownWeeks must be from 0 to 104");
        }

        static void SlaProblems(List<string> p, GameConfig c)
        {
            SlaConfig k = c.Sla;
            if (k == null) { p.Add("sla is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("sla.maxLevel must be from 0 to 10");
            if (k.PremiumPermillePerLevel < 0 || k.PremiumPermillePerLevel * (long)k.MaxLevel > 300) p.Add("sla.premiumPermillePerLevel must be 0 or more, and every level together at most 300");
            if (k.BreachBelowPermille < 1 || k.BreachBelowPermille > MathI.Permille) p.Add("sla.breachBelowPermille must be from 1 to 1000");
            if (k.RefundPermillePerLevel < k.PremiumPermillePerLevel || k.RefundPermillePerLevel * (long)k.MaxLevel > MathI.Permille) p.Add("sla.refundPermillePerLevel must be at least the premium, and every level together at most 1000, so a pledge that is always broken never pays");
            if (k.BreachReputationPerLevel < 0 || k.BreachReputationPerLevel * (long)k.MaxLevel > 200) p.Add("sla.breachReputationPerLevel must be 0 or more, and every level together at most 200");
            if (k.MinReputation < 0 || k.MinReputation > MathI.Permille) p.Add("sla.minReputation must be from 0 to 1000");
            if (k.LockWeeks < 0 || k.LockWeeks > 104) p.Add("sla.lockWeeks must be from 0 to 104");
            if (k.StrikeWeeks < 0 || k.StrikeWeeks > 52) p.Add("sla.strikeWeeks must be from 0 to 52");
        }

        static void PostMortemProblems(List<string> p, GameConfig c)
        {
            PostMortemConfig k = c.PostMortem;
            if (k == null) { p.Add("postMortem is missing"); return; }
            if (k.WindowWeeks < 0 || k.WindowWeeks > 52) p.Add("postMortem.windowWeeks must be from 0 to 52");
            if (k.SummaryCost < 0) p.Add("postMortem.summaryCost is negative");
            if (k.FullCost < k.SummaryCost) p.Add("postMortem.fullCost must be at least summaryCost");
            if (k.SummaryRecoverPermille < 0 || k.SummaryRecoverPermille > MathI.Permille) p.Add("postMortem.summaryRecoverPermille must be from 0 to 1000");
            if (k.FullRecoverPermille < k.SummaryRecoverPermille || k.FullRecoverPermille > MathI.Permille) p.Add("postMortem.fullRecoverPermille must be from summaryRecoverPermille to 1000, so no write-up wins back more than the incident cost");
            if (k.MinSafetyStaff < 0 || k.MinSafetyStaff > 100) p.Add("postMortem.minSafetyStaff must be from 0 to 100");
        }

        static void ConsultantsProblems(List<string> p, GameConfig c)
        {
            ConsultantsConfig k = c.Consultants;
            if (k == null) { p.Add("consultants is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("consultants.maxLevel must be from 0 to 10");
            if (k.FeePerLevel < 0) p.Add("consultants.feePerLevel is negative");
            if (k.WeeklyPerLevel < 0) p.Add("consultants.weeklyPerLevel is negative");
            if (k.StaffPerLevel < 0 || k.StaffPerLevel > 50) p.Add("consultants.staffPerLevel must be from 0 to 50");
            if (k.Weeks < 0 || k.Weeks > 52) p.Add("consultants.weeks must be from 0 to 52");
            if (k.CooldownWeeks < k.Weeks || k.CooldownWeeks > 104) p.Add("consultants.cooldownWeeks must be at least weeks and at most 104, so a firm is never in all the time");
        }

        static void IncidentFloorProblems(List<string> p, GameConfig c)
        {
            IncidentFloorConfig k = c.IncidentFloor;
            if (k == null) { p.Add("incidentFloor is missing"); return; }
            if (k.FineFloorPermille < 0 || k.FineFloorPermille > MathI.Permille) p.Add("incidentFloor.fineFloorPermille must be from 0 to 1000");
            if (k.ChanceFloorPermille < 0 || k.ChanceFloorPermille > MathI.Permille) p.Add("incidentFloor.chanceFloorPermille must be from 0 to 1000");
        }

        static void OnCallProblems(List<string> p, GameConfig c)
        {
            OnCallConfig k = c.OnCall;
            if (k == null) { p.Add("onCall is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("onCall.maxLevel must be from 0 to 10");
            if (k.WeeklyCostPerLevel < 0) p.Add("onCall.weeklyCostPerLevel is negative");
            if (k.StaffPerLevel < 0 || k.StaffPerLevel > 100) p.Add("onCall.staffPerLevel must be from 0 to 100");
            if (k.FineCutPermillePerLevel < 0 || k.FineCutPermillePerLevel * (long)k.MaxLevel > 900) p.Add("onCall.fineCutPermillePerLevel must be 0 or more, and every level together at most 900, so an incident always costs something");
            if (k.ReputationCutPermillePerLevel < 0 || k.ReputationCutPermillePerLevel * (long)k.MaxLevel > 900) p.Add("onCall.reputationCutPermillePerLevel must be 0 or more, and every level together at most 900");
            if (k.MoraleDropCutPermillePerLevel < 0 || k.MoraleDropCutPermillePerLevel * (long)k.MaxLevel > MathI.Permille) p.Add("onCall.moraleDropCutPermillePerLevel must be 0 or more, and every level together at most 1000");
            if (k.MoralePenaltyPerLevel < 0 || k.MoralePenaltyPerLevel * (long)k.MaxLevel > 50) p.Add("onCall.moralePenaltyPerLevel must be 0 or more, and every level together at most 50");
            if (k.LockWeeks < 0 || k.LockWeeks > 104) p.Add("onCall.lockWeeks must be from 0 to 104");
        }

        static void MarketingProblems(List<string> p, GameConfig c)
        {
            MarketingConfig k = c.Marketing;
            if (k == null) { p.Add("marketing is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("marketing.maxLevel must be from 0 to 10");
            if (k.CostPerLevel < 0) p.Add("marketing.costPerLevel is negative");
            if (k.Weeks < 0 || k.Weeks > 52) p.Add("marketing.weeks must be from 0 to 52");
            if (k.AppealPermillePerLevel < 0 || k.AppealPermillePerLevel * (long)k.MaxLevel > 2000) p.Add("marketing.appealPermillePerLevel must be 0 or more, and every level together at most 2000");
            if (k.ShareCeilingPermille < 1 || k.ShareCeilingPermille > MathI.Permille) p.Add("marketing.shareCeilingPermille must be from 1 to 1000");
            if (k.HangoverWeeks < 0 || k.HangoverWeeks > 52) p.Add("marketing.hangoverWeeks must be from 0 to 52");
            if (k.HangoverPermillePerLevel < 0 || k.HangoverPermillePerLevel * (long)k.MaxLevel > 900) p.Add("marketing.hangoverPermillePerLevel must be 0 or more, and every level together at most 900, so a hangover never ends sales");
            if (k.BacklashReputation < 0 || k.BacklashReputation > 100) p.Add("marketing.backlashReputation must be from 0 to 100");
            if (k.CooldownWeeks < k.Weeks + k.HangoverWeeks || k.CooldownWeeks > 520) p.Add("marketing.cooldownWeeks must be from weeks plus hangoverWeeks to 520, so one cannot start while another is on");
            if (k.MinSales < 0 || k.MinSales > 1000) p.Add("marketing.minSales must be from 0 to 1000");
        }

        static void OptimiseProblems(List<string> p, GameConfig c)
        {
            OptimisationConfig k = c.Optimise;
            if (k == null) { p.Add("optimise is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("optimise.maxLevel must be from 0 to 10");
            if (k.SetupCostPerLevel < 0) p.Add("optimise.setupCostPerLevel is negative");
            if (k.EngineersPerLevel < 0 || k.EngineersPerLevel > 100) p.Add("optimise.engineersPerLevel must be from 0 to 100");
            if (k.DemandCutPermillePerLevel < 0 || k.DemandCutPermillePerLevel * (long)k.MaxLevel > 900) p.Add("optimise.demandCutPermillePerLevel must be 0 or more, and every level together at most 900, so serving never costs nothing");
            if (k.IncidentPermillePerLevel < 0 || k.IncidentPermillePerLevel * (long)k.MaxLevel > 500) p.Add("optimise.incidentPermillePerLevel must be 0 or more, and every level together at most 500");
        }

        static void BoardProblems(List<string> p, GameConfig c)
        {
            EthicsBoardConfig k = c.Board;
            if (k == null) { p.Add("ethicsBoard is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 5) p.Add("ethicsBoard.maxLevel must be from 0 to 5");
            if (k.SetupCostPerLevel < 0) p.Add("ethicsBoard.setupCostPerLevel is negative");
            if (k.WeeklyCostPerLevel < 0) p.Add("ethicsBoard.weeklyCostPerLevel is negative");
            if (k.SafetyStaffPerLevel < 0 || k.SafetyStaffPerLevel > 100) p.Add("ethicsBoard.safetyStaffPerLevel must be from 0 to 100");
            if (k.ScandalCutPermillePerLevel < 0 || k.ScandalCutPermillePerLevel * (long)k.MaxLevel > 900) p.Add("ethicsBoard.scandalCutPermillePerLevel must be 0 or more, and every level together at most 900");
            if (k.HearingPointsPerLevel < 0 || k.HearingPointsPerLevel > 100) p.Add("ethicsBoard.hearingPointsPerLevel must be from 0 to 100");
            if (k.AttentionPermillePerLevel < 0 || k.AttentionPermillePerLevel * (long)k.MaxLevel > 900) p.Add("ethicsBoard.attentionPermillePerLevel must be 0 or more, and every level together at most 900");
            if (k.VetoLevel < 0 || k.VetoLevel > k.MaxLevel) p.Add("ethicsBoard.vetoLevel must be from 0 (no veto) to maxLevel");
            if (k.VetoAlignment < 0 || k.VetoAlignment > MathI.Permille) p.Add("ethicsBoard.vetoAlignment must be from 0 to 1000");
            if (k.ResignationReputation < 0 || k.ResignationReputation > 100) p.Add("ethicsBoard.resignationReputation must be from 0 to 100");
            if (k.LockWeeks < 0 || k.LockWeeks > 520) p.Add("ethicsBoard.lockWeeks must be from 0 to 520");
        }

        static void ChiefsProblems(List<string> p, GameConfig c)
        {
            ChiefsConfig k = c.Chiefs;
            if (k == null) { p.Add("chiefs is missing"); return; }
            if (k.MinStaff < 0 || k.MinStaff > 1000) p.Add("chiefs.minStaff must be from 0 to 1000");
            if (k.Fee < 0) p.Add("chiefs.fee is negative");
            if (k.WeeklyPremium < 0) p.Add("chiefs.weeklyPremium is negative");
            if (k.ResearchPermille < 0 || k.ResearchPermille > 1000) p.Add("chiefs.researchPermille must be from 0 to 1000");
            if (k.IncidentCutPermille < 0 || k.IncidentCutPermille > 900) p.Add("chiefs.incidentCutPermille must be from 0 to 900, so a safety chief never rules incidents out");
            if (k.PoachPenaltyPermille < 0 || k.PoachPenaltyPermille > 300) p.Add("chiefs.poachPenaltyPermille must be from 0 to 300");
            if (k.ShockMorale < 0 || k.ShockMorale > MathI.Permille) p.Add("chiefs.shockMorale must be from 0 to 1000");
        }

        static void EquityProblems(List<string> p, GameConfig c)
        {
            EquityConfig k = c.Equity;
            if (k == null) { p.Add("equity is missing"); return; }
            if (k.MaxLevel < 0 || k.MaxLevel > 10) p.Add("equity.maxLevel must be from 0 to 10");
            if (k.DilutionPermille < 1 || k.DilutionPermille > 300) p.Add("equity.dilutionPermille must be from 1 to 300");
            if (k.SalaryCutPermille < 0 || k.SalaryCutPermille * (long)k.MaxLevel > 600) p.Add("equity.salaryCutPermille must be 0 or more, and every level together must leave at least 40% of a salary");
            if (k.MoraleStep < 0 || k.MoraleStep * (long)k.MaxLevel > 300) p.Add("equity.moraleStep must be 0 or more, and every level together at most 300");
            if (k.PoachStepPermille < 0 || k.PoachStepPermille * (long)k.MaxLevel > 600) p.Add("equity.poachStepPermille must be 0 or more, and every level together at most 600");
            if (k.MinStaff < 0 || k.MinStaff > 1000) p.Add("equity.minStaff must be from 0 to 1000");
        }

        static void HackProblems(List<string> p, GameConfig c)
        {
            HackWeekConfig k = c.Hack;
            if (k == null) { p.Add("hack is missing"); return; }
            if (k.Weeks < 0 || k.Weeks > 52) p.Add("hack.weeks must be from 0 to 52");
            if (k.MinResearchers < 0 || k.MinResearchers > 1000) p.Add("hack.minResearchers must be from 0 to 1000");
            if (k.MinMorale < 0 || k.MinMorale > MathI.Permille) p.Add("hack.minMorale must be from 0 to 1000");
            if (k.CostPerStaff < 0) p.Add("hack.costPerStaff is negative");
            if (k.MoraleLift < 0 || k.MoraleLift > MathI.Permille) p.Add("hack.moraleLift must be from 0 to 1000");
            if (k.ResearchPermille < 0 || k.ResearchPermille > 2000) p.Add("hack.researchPermille must be from 0 to 2000");
            if (k.TrainingCutPermille < 0 || k.TrainingCutPermille > 900) p.Add("hack.trainingCutPermille must be from 0 to 900, so training never stops");
            if (k.CooldownWeeks < k.Weeks || k.CooldownWeeks > 520) p.Add("hack.cooldownWeeks must be from weeks to 520, so one cannot start while another is on");
        }

        static void AuditProblems(List<string> p, GameConfig c)
        {
            SafetyAuditConfig k = c.Audit;
            if (k == null) { p.Add("audit is missing"); return; }
            if (k.Weeks < 0 || k.Weeks > 520) p.Add("audit.weeks must be from 0 to 520");
            if (k.MinSafetyStaff < 0 || k.MinSafetyStaff > 1000) p.Add("audit.minSafetyStaff must be from 0 to 1000");
            if (k.RevenueWeeks < 0 || k.RevenueWeeks > 520) p.Add("audit.revenueWeeks must be from 0 to 520");
            if (k.MinCost < 0) p.Add("audit.minCost is negative");
            if (k.CleanGap < 0 || k.CleanGap > MathI.Permille) p.Add("audit.cleanGap must be from 0 to 1000");
            if (k.ShameGap < k.CleanGap || k.ShameGap > MathI.Permille) p.Add("audit.shameGap must be from cleanGap to 1000");
            if (k.CleanReputation < 0 || k.CleanReputation > 200) p.Add("audit.cleanReputation must be from 0 to 200");
            if (k.ShameReputation < 0 || k.ShameReputation > 200) p.Add("audit.shameReputation must be from 0 to 200");
            if (k.CulturePoints < 0 || k.CulturePoints > 100) p.Add("audit.culturePoints must be from 0 to 100");
            if (k.AttentionReliefPermille < 0 || k.AttentionReliefPermille > 500) p.Add("audit.attentionReliefPermille must be from 0 to 500");
            if (k.HearingPoints < 0 || k.HearingPoints > 100) p.Add("audit.hearingPoints must be from 0 to 100");
        }

        static void LeadsProblems(List<string> p, GameConfig c)
        {
            LeadsConfig l = c.Leads;
            if (l == null) { p.Add("leads is missing"); return; }
            if (l.MinLuckPermille < 1 || l.MinLuckPermille > MathI.Permille) p.Add("leads.minLuckPermille must be from 1 to 1000");
            if (l.MinLuckPermille > c.Training.LuckPermille) p.Add("leads.minLuckPermille is above training.luckPermille, so no run could ever leave a lead");
            if (l.MaxOpen < 1 || l.MaxOpen > 20) p.Add("leads.maxOpen must be from 1 to 20");
            if (l.ExpireWeeks < 1 || l.ExpireWeeks > 520) p.Add("leads.expireWeeks must be from 1 to 520");
            if (l.FollowWeeks < 1 || l.FollowWeeks > 520) p.Add("leads.followWeeks must be from 1 to 520");
            if (l.FollowCostWeeks < 0 || l.FollowCostWeeks > 520) p.Add("leads.followCostWeeks must be from 0 to 520");
            if (l.MinCost < 0) p.Add("leads.minCost is negative");
            if (l.MaxSpeedPermille < 0 || l.MaxSpeedPermille > 2000) p.Add("leads.maxSpeedPermille must be from 0 to 2000");
            if (l.Outcomes.Count == 0) p.Add("leads.outcomes is empty");
            UniqueIds(p, "leads.outcomes", l.Outcomes);
            long weights = 0;
            for (int i = 0; i < l.Outcomes.Count; i++)
            {
                LeadOutcomeConfig o = l.Outcomes[i];
                string who = "lead outcome " + o.IdValue;
                if (string.IsNullOrEmpty(o.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(o.Blurb)) p.Add(who + " has no blurb");
                if (o.Weight < 1 || o.Weight > 1000) p.Add(who + " needs a weight from 1 to 1000");
                if (o.Kind != LeadEffectKind.Nothing && o.Amount < 1) p.Add(who + " does something but gives nothing");
                if (o.Kind == LeadEffectKind.Nothing && o.Amount != 0) p.Add(who + " does nothing but has an amount");
                weights += o.Weight;
            }
        }

        static void ControlProblems(List<string> p, GameConfig c)
        {
            ControlConfig k = c.Control;
            if (k == null) { p.Add("control is missing"); return; }
            if (k.GraceEras < 0 || k.GraceEras > 20) p.Add("control.graceEras must be from 0 to 20");
            if (k.LossPerEraPermille < 0 || k.LossPerEraPermille > MathI.Permille) p.Add("control.lossPerEraPermille must be from 0 to 1000");
            if (k.MaxLossPermille < 0 || k.MaxLossPermille > MathI.Permille) p.Add("control.maxLossPermille must be from 0 to 1000");
            if (k.FiredListenerPoints < 0 || k.FiredListenerPoints > MathI.Permille) p.Add("control.firedListenerPoints must be from 0 to 1000");
        }

        static void AgeingProblems(List<string> p, GameConfig c)
        {
            AgeingConfig a = c.Ageing;
            if (a == null) { p.Add("ageing is missing"); return; }
            if (a.RefurbishCostPermille < 0 || a.RefurbishCostPermille > 3000) p.Add("ageing.refurbishCostPermille must be from 0 to 3000");
            if (a.MaxMoralePenalty < 0 || a.MaxMoralePenalty > MathI.Permille) p.Add("ageing.maxMoralePenalty must be from 0 to 1000");
            if (a.Stages.Count == 0) p.Add("ageing.stages is empty");
            UniqueIds(p, "ageing.stages", a.Stages);
            for (int i = 0; i < a.Stages.Count; i++)
            {
                AgeStageConfig s = a.Stages[i];
                string who = "age stage " + s.IdValue;
                if (string.IsNullOrEmpty(s.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(s.Blurb)) p.Add(who + " has no blurb");
                if (s.UpkeepExtraPermille < 0 || s.UpkeepExtraPermille > 3000) p.Add(who + " needs upkeepExtraPermille from 0 to 3000");
                if (s.MoralePenalty < 0 || s.MoralePenalty > MathI.Permille) p.Add(who + " needs moralePenalty from 0 to 1000");
                if (i == 0 && s.MinEras != 0) p.Add("the first age stage must start at 0 eras so every room has one");
                if (i > 0 && s.MinEras <= a.Stages[i - 1].MinEras) p.Add(who + " does not start above the stage before it");
                if (i > 0 && (s.UpkeepExtraPermille < a.Stages[i - 1].UpkeepExtraPermille || s.MoralePenalty < a.Stages[i - 1].MoralePenalty))
                    p.Add(who + " costs less than the stage before it: rooms never get better with age");
            }
        }

        static void NamingProblems(List<string> p, GameConfig c)
        {
            NamingConfig n = c.Naming;
            if (n == null) { p.Add("naming is missing"); return; }
            if (n.MaxMess < 1 || n.MaxMess > MathI.Permille) p.Add("naming.maxMess must be from 1 to 1000");
            if (n.MessPerVersion < 0 || n.MessPerVersion > MathI.Permille) p.Add("naming.messPerVersion must be from 0 to 1000");
            if (n.SuffixEveryPoints < 1 || n.SuffixEveryPoints > MathI.Permille) p.Add("naming.suffixEveryPoints must be from 1 to 1000");
            if (n.ConfusionPermille < 0 || n.ConfusionPermille > 900) p.Add("naming.confusionPermille must be from 0 to 900, or full mess would erase a model's appeal");
            if (n.CleanPoints < 0 || n.CleanPoints > MathI.Permille) p.Add("naming.cleanPoints must be from 0 to 1000");
            if (n.CleanFeeHireFees < 0 || n.CleanFeeHireFees > 1000) p.Add("naming.cleanFeeHireFees must be from 0 to 1000");
            if (n.Suffixes.Count == 0) p.Add("naming.suffixes is empty");
            for (int i = 0; i < n.Suffixes.Count; i++)
                if (string.IsNullOrWhiteSpace(n.Suffixes[i])) p.Add("naming.suffixes has an empty word at position " + i);
        }

        static void CultureProblems(List<string> p, GameConfig c)
        {
            CultureConfig u = c.Culture;
            if (u == null) { p.Add("culture is missing"); return; }
            if (u.Start < 0 || u.Start > MathI.Permille) p.Add("culture.start must be from 0 to 1000");
            if (u.NeutralLevel < 0 || u.NeutralLevel > MathI.Permille) p.Add("culture.neutralLevel must be from 0 to 1000");
            if (u.CatchSlopePermille < 0 || u.CatchSlopePermille > MathI.Permille) p.Add("culture.catchSlopePermille must be from 0 to 1000");
            if (u.StaffGainPerPerson < 0 || u.StaffGainPerPerson > 100) p.Add("culture.staffGainPerPerson must be from 0 to 100");
            if (u.MaxStaffGain < 0 || u.MaxStaffGain > 1000) p.Add("culture.maxStaffGain must be from 0 to 1000");
            if (u.DriftPerWeek < 0 || u.DriftPerWeek > 100) p.Add("culture.driftPerWeek must be from 0 to 100");
            if (u.RushBelowAlignment < 0 || u.RushBelowAlignment > MathI.Permille) p.Add("culture.rushBelowAlignment must be from 0 to 1000");
            if (u.RushPoints < 0 || u.RushPoints > MathI.Permille) p.Add("culture.rushPoints must be from 0 to 1000");
            if (u.GateKeptPoints < 0 || u.GateKeptPoints > MathI.Permille) p.Add("culture.gateKeptPoints must be from 0 to 1000");
            if (u.GateLoweredPoints < 0 || u.GateLoweredPoints > MathI.Permille) p.Add("culture.gateLoweredPoints must be from 0 to 1000");
            if (u.ComplaintAnsweredPoints < 0 || u.ComplaintAnsweredPoints > MathI.Permille) p.Add("culture.complaintAnsweredPoints must be from 0 to 1000");
            if (u.ComplaintLapsedPoints < 0 || u.ComplaintLapsedPoints > MathI.Permille) p.Add("culture.complaintLapsedPoints must be from 0 to 1000");
            if (u.MaxGate < 0 || u.MaxGate > MathI.Permille) p.Add("culture.maxGate must be from 0 to 1000");
            if (u.GateLockWeeks < 0 || u.GateLockWeeks > 520) p.Add("culture.gateLockWeeks must be from 0 to 520");
            foreach (string role in u.ListeningRoles)
                if (c.Role(role) == null) p.Add("culture.listeningRoles names the unknown role \"" + role + "\"");
            if (u.Bands.Count == 0) p.Add("culture.bands is empty");
            UniqueIds(p, "culture.bands", u.Bands);
            for (int i = 0; i < u.Bands.Count; i++)
            {
                CultureBandConfig b = u.Bands[i];
                string who = "culture band " + b.Id;
                if (string.IsNullOrEmpty(b.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(b.Blurb)) p.Add(who + " has no blurb");
                if (b.Min < 0 || b.Min > MathI.Permille) p.Add(who + " has min " + b.Min + ": it must be from 0 to 1000");
                if (i == 0 && b.Min != 0) p.Add("the first culture band must start at 0 so every level has a name");
                if (i > 0 && b.Min <= u.Bands[i - 1].Min) p.Add(who + " does not start above the band before it");
            }
        }

        static void TrustProblems(List<string> p, GameConfig c)
        {
            TrustConfig t = c.Trust;
            if (t == null) { p.Add("trust is missing"); return; }
            if (t.BaselineTrust < 0 || t.BaselineTrust > 1000) p.Add("trust.baselineTrust must be from 0 to 1000");
            if (t.GainPerWeek < 0 || t.GainPerWeek > 1000) p.Add("trust.gainPerWeek must be from 0 to 1000");
            if (t.IdleDecayPerWeek < 0 || t.IdleDecayPerWeek > 1000) p.Add("trust.idleDecayPerWeek must be from 0 to 1000");
            if (t.IncidentDrop < 0 || t.IncidentDrop > 1000) p.Add("trust.incidentDrop must be from 0 to 1000");
            if (t.BracketWinPoints < 0 || t.BracketWinPoints > 1000) p.Add("trust.bracketWinPoints must be from 0 to 1000");
            if (t.ExposedDrop < 0 || t.ExposedDrop > 1000) p.Add("trust.exposedDrop must be from 0 to 1000");
            Permille(p, "trust.carryPermille", t.CarryPermille);
            if (t.AppealSlopePermille < 0 || t.AppealSlopePermille > 1800) p.Add("trust.appealSlopePermille must be from 0 to 1800, or full distrust would erase a model's appeal");
            if (t.ComplaintWeeks < 1 || t.ComplaintWeeks > 52) p.Add("trust.complaintWeeks needs a value from 1 to 52");
            if (t.MaxOpenComplaints < 0) p.Add("trust.maxOpenComplaints cannot be negative");
            Permille(p, "trust.minSuccessPermille", t.MinSuccessPermille);
            if (t.MinSuccessPermille < 1) p.Add("trust.minSuccessPermille needs at least 1 so no answer is hopeless");
            if (t.Responses.Count == 0) p.Add("trust.responses is empty: a customer would have no answer");
            UniqueIds(p, "trust.responses", t.Responses);
            foreach (ComplaintResponseConfig r in t.Responses)
            {
                string who = "complaint response " + r.Id;
                if (string.IsNullOrEmpty(r.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(r.Blurb)) p.Add(who + " has no blurb");
                if (r.CostHireFees < 0 || r.CostHireFees > 1000) p.Add(who + " needs costHireFees from 0 to 1000");
                if (r.CostRevenuePermille < 0 || r.CostRevenuePermille > 5000) p.Add(who + " needs costRevenuePermille from 0 to 5000");
                Permille(p, who + " baseSuccessPermille", r.BaseSuccessPermille);
                if (r.ReputationSlopePermille < -2000 || r.ReputationSlopePermille > 2000) p.Add(who + " needs reputationSlopePermille from -2000 to 2000");
                if (r.RestorePermille < 0 || r.RestorePermille > 2000) p.Add(who + " needs restorePermille from 0 to 2000");
                if (r.BackfirePermille < 0 || r.BackfirePermille > 2000) p.Add(who + " needs backfirePermille from 0 to 2000");
            }
            foreach (SectorConfig s in c.Market.Sectors)
                if (string.IsNullOrEmpty(s.Family)) p.Add("sector " + s.Id + " has no family, so trust could not carry to or from it");
        }

        static void GauntletProblems(List<string> p, GameConfig c)
        {
            GauntletConfig g = c.Gauntlet;
            if (g.MaxTuningPermille < 0 || g.MaxTuningPermille > 1000) p.Add("gauntlet.maxTuningPermille must be from 0 to 1000");
            Permille(p, "gauntlet.tuningScoreSharePermille", g.TuningScoreSharePermille);
            if (g.TuningCostHireFeesPer100 < 0) p.Add("gauntlet.tuningCostHireFeesPer100 is negative");
            if (g.ExposeChancePerTuningPermille < 0 || g.ExposeFeudBonusPermille < 0) p.Add("gauntlet exposure chances cannot be negative");
            if (g.MaxTuningPermille > 0 && g.ExposeChancePerTuningPermille < 1) p.Add("gauntlet.exposeChancePerTuningPermille needs at least 1 while tuning is on, or tuning is free money");
            if (g.ExposedReputationLoss < 0 || g.DemoTriumphReputation < 0 || g.DemoMemeReputation < 0) p.Add("gauntlet reputation amounts cannot be negative");
            if (g.DemoMemeFromPermille < 0) p.Add("gauntlet.demoMemeFromPermille is negative");
            if (g.DemoMemeFromPermille > 0 && (g.DemoTriumphBelowPermille < 0 || g.DemoTriumphBelowPermille >= g.DemoMemeFromPermille))
                p.Add("gauntlet.demoTriumphBelowPermille must be under demoMemeFromPermille, or one demo could be both");
            if (g.ConfidentWrongMinGap < 1) p.Add("gauntlet.confidentWrongMinGap needs at least 1");
            if (g.ConfidentWrongReputation < 0 || g.NewcomerReputation < 0) p.Add("gauntlet award reputation cannot be negative");
            if (g.NewcomerYears < 0) p.Add("gauntlet.newcomerYears is negative");
            if (g.NewcomerMorale < 0 || g.NewcomerMorale > 500) p.Add("gauntlet.newcomerMorale needs a value from 0 to 500");
            if (g.SeasonCrownPoints < 0 || g.SeasonBracketPoints < 0) p.Add("gauntlet season points cannot be negative");
        }

        static void PressProblems(List<string> p, GameConfig c)
        {
            PressConfig t = c.Press;
            if (t == null) { p.Add("press is missing"); return; }
            if (t.InquiryWeeks < 1 || t.InquiryWeeks > 52) p.Add("press.inquiryWeeks needs a value from 1 to 52");
            if (t.MaxOpenInquiries < 1) p.Add("press.maxOpenInquiries needs at least 1");
            if (t.LaunchBuzzPoints < 0 || t.LaunchBuzzPoints > 500) p.Add("press.launchBuzzPoints needs a value from 0 to 500");
            if (t.MaxDampingPermille < 0 || t.MaxDampingPermille > 900) p.Add("press.maxDampingPermille needs a value from 0 to 900: no lab is immune to a scandal");
            if (t.MaxBuzzPermille < 0 || t.MaxBuzzPermille > 2000) p.Add("press.maxBuzzPermille needs a value from 0 to 2000");
            Permille(p, "press.minSuccessPermille", t.MinSuccessPermille);
            if (t.MinSuccessPermille < 1) p.Add("press.minSuccessPermille needs at least 1 so no answer is hopeless");
            if (t.LeakWindowWeeks < 1) p.Add("press.leakWindowWeeks needs at least 1");
            if (t.LeakFeeHireFees < 0) p.Add("press.leakFeeHireFees cannot be negative");
            Permille(p, "press.leakBaseSuccessPermille", t.LeakBaseSuccessPermille);
            if (t.LeakReputationSlopePermille < -2000 || t.LeakReputationSlopePermille > 2000) p.Add("press.leakReputationSlopePermille needs a value from -2000 to 2000");
            if (t.LeakRoomSlopePermille < -2000 || t.LeakRoomSlopePermille > 2000) p.Add("press.leakRoomSlopePermille needs a value from -2000 to 2000");
            if (t.LeakHitPoints < 0 || t.LeakHitPoints > 1000 || t.LeakBackfirePoints < 0 || t.LeakBackfirePoints > 1000) p.Add("press.leakHitPoints and leakBackfirePoints need values from 0 to 1000");
            if (t.LeakFeelingHit < 0 || t.LeakFailFeelingHit < 0) p.Add("press.leakFeelingHit and leakFailFeelingHit cannot be negative");

            if (t.Beats.Count == 0) p.Add("press.beats is empty");
            UniqueIds(p, "press.beats", t.Beats);
            foreach (BeatConfig b in t.Beats)
            {
                string who = "beat " + b.Id;
                if (string.IsNullOrEmpty(b.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(b.Blurb)) p.Add(who + " has no blurb");
                if (b.StakePoints < 0 || b.StakePoints > 1000) p.Add(who + " needs stakePoints from 0 to 1000");
                if (!b.Good && b.StakePoints > 0) p.Add(who + " is bad news, whose stake is what it cost, so stakePoints must be 0");
                bool covered = false;
                foreach (JournalistConfig j in t.Journalists) if (j.Beats.Contains(b.Id)) covered = true;
                if (!covered) p.Add(who + " is covered by no journalist, so nobody would ever ask");
            }
            if (t.Journalists.Count == 0) p.Add("press.journalists is empty");
            UniqueIds(p, "press.journalists", t.Journalists);
            foreach (JournalistConfig j in t.Journalists)
            {
                string who = "journalist " + j.Id;
                if (string.IsNullOrEmpty(j.Name) || string.IsNullOrEmpty(j.Outlet)) p.Add(who + " needs a name and an outlet");
                if (string.IsNullOrEmpty(j.Blurb)) p.Add(who + " has no blurb");
                if (j.Beats.Count == 0) p.Add(who + " covers no beat");
                foreach (string beat in j.Beats) if (t.Beat(beat) == null) p.Add(who + " covers the unknown beat \"" + beat + "\"");
                if (j.ChancePermille < -900 || j.ChancePermille > 900) p.Add(who + " needs chancePermille from -900 to 900");
                if (j.BackfirePermille < 0 || j.BackfirePermille > 5000) p.Add(who + " needs backfirePermille from 0 to 5000");
            }
            if (t.Responses.Count == 0) p.Add("press.responses is empty: a journalist would have no answer");
            UniqueIds(p, "press.responses", t.Responses);
            foreach (PressResponseConfig r in t.Responses)
            {
                string who = "press response " + r.Id;
                if (string.IsNullOrEmpty(r.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(r.Blurb)) p.Add(who + " has no blurb");
                if (r.CostHireFees < 0 || r.CostHireFees > 1000) p.Add(who + " needs costHireFees from 0 to 1000");
                Permille(p, who + " baseSuccessPermille", r.BaseSuccessPermille);
                if (r.RoomSlopePermille < -2000 || r.RoomSlopePermille > 2000) p.Add(who + " needs roomSlopePermille from -2000 to 2000");
                if (r.ReputationSlopePermille < -2000 || r.ReputationSlopePermille > 2000) p.Add(who + " needs reputationSlopePermille from -2000 to 2000");
                if (r.RecoverPermille < 0 || r.RecoverPermille > 2000) p.Add(who + " needs recoverPermille from 0 to 2000");
                if (r.BackfirePermille < 0 || r.BackfirePermille > 2000) p.Add(who + " needs backfirePermille from 0 to 2000");
            }
            if (c.Role("sales") == null) p.Add("press costs are priced in hiring fees of the \"sales\" role, which does not exist");
        }

        static void FoundingProblems(List<string> p, GameConfig c)
        {
            foreach (FounderConfig f in c.Founders)
            {
                string who = "founder " + f.Id;
                if (string.IsNullOrEmpty(f.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(f.Blurb)) p.Add(who + " has no blurb");
                if (f.CashPermille < 100 || f.CashPermille > 10000) p.Add(who + " needs cashPermille from 100 to 10000");
                if (f.GrantPermille < 0 || f.GrantPermille > 10000) p.Add(who + " needs grantPermille from 0 to 10000");
                if (f.ReputationDelta < -500 || f.ReputationDelta > 500) p.Add(who + " needs reputationDelta from -500 to 500");
                if (f.ResearchPermille < -500 || f.ResearchPermille > 500) p.Add(who + " needs researchPermille from -500 to 500");
                if (f.AppealPermille < -400 || f.AppealPermille > 400) p.Add(who + " needs appealPermille from -400 to 400");
                if (f.HireFeePermille < -500 || f.HireFeePermille > 1000) p.Add(who + " needs hireFeePermille from -500 to 1000");
                if (!string.IsNullOrEmpty(f.EnemyRivalId) && c.Rival(f.EnemyRivalId) == null) p.Add(who + " names the unknown enemy rival \"" + f.EnemyRivalId + "\"");
                if (f.EnemyFeeling < -1000 || f.EnemyFeeling > 0) p.Add(who + " needs enemyFeeling from -1000 to 0");
                if (string.IsNullOrEmpty(f.EnemyRivalId) && f.EnemyFeeling != 0) p.Add(who + " has an enemyFeeling but no enemy");
            }
            foreach (StyleConfig st in c.Styles)
            {
                string who = "style " + st.Id;
                if (string.IsNullOrEmpty(st.Name)) p.Add(who + " has no name");
                if (string.IsNullOrEmpty(st.Blurb)) p.Add(who + " has no blurb");
                if (st.FocusSectors.Count == 0) p.Add(who + " has no focus sectors");
                foreach (string sector in st.FocusSectors)
                    if (c.Sector(sector) == null) p.Add(who + " names the unknown sector \"" + sector + "\"");
                if (st.FocusAppealPermille < 0 || st.FocusAppealPermille > 600) p.Add(who + " needs focusAppealPermille from 0 to 600");
                if (st.OtherAppealPermille > 0 || st.OtherAppealPermille < -400) p.Add(who + " needs otherAppealPermille from -400 to 0");
            }
        }

        static void RoomPerkProblems(List<string> p, GameConfig c)
        {
            foreach (RoomPerkConfig perk in c.RoomPerks)
            {
                string who = "roomPerk " + perk.Id;
                if (c.RoomType(perk.RoomId) == null) p.Add(who + " names the unknown room type \"" + perk.RoomId + "\"");
                if (perk.PerTilePermille < 1 || perk.PerTilePermille > 1000) p.Add(who + " needs perTilePermille from 1 to 1000");
                if (perk.CapPermille < perk.PerTilePermille || perk.CapPermille > 900) p.Add(who + " needs a cap from perTilePermille up to 900, so one tile always counts and a chance never reaches zero");
                if (string.IsNullOrEmpty(perk.Blurb)) p.Add(who + " has no blurb");
                if (perk.Kind != RoomPerkKind.SectorAppeal && perk.Sectors.Count > 0) p.Add(who + " lists sectors but only a SectorAppeal perk is about sectors");
                foreach (string sector in perk.Sectors)
                    if (c.Sector(sector) == null) p.Add(who + " names the unknown sector \"" + sector + "\"");
            }
        }

        static IEnumerable<string> StartTechsMissing(GameConfig c, int eraIndex)
        {
            EraConfig era = c.Eras[eraIndex];
            foreach (TechConfig tech in c.Research.Techs)
            {
                EraConfig techEra = c.Era(tech.Era);
                if (techEra != null && c.Eras.IndexOf(techEra) < eraIndex && !era.StartTechs.Contains(tech.Id)) yield return tech.Id;
            }
        }

        static void EraRulePermille(List<string> p, string who, string name, int value)
        {
            if (value < 250 || value > 5000) p.Add(who + " has " + name + " " + value + ": an era rule moves a cost or a chance between x0.25 and x5");
        }

        static void EraProblems(List<string> p, GameConfig c)
        {
            if (c.Eras.Count == 0) { p.Add("eras is empty"); return; }
            if (c.Eras[0].StartYear != c.Calendar.StartYear) p.Add("the first era starts in " + c.Eras[0].StartYear + " but calendar.startYear is " + c.Calendar.StartYear);
            if (!c.Eras[0].Startable) p.Add("the first era must be startable: it is the default way to begin");
            if (c.Eras[0].StartTechs.Count > 0) p.Add("the first era gives no start techs: a game from the very beginning has nothing researched");

            for (int i = 0; i < c.Eras.Count; i++)
            {
                EraConfig era = c.Eras[i];
                string who = "era " + era.Id;
                if (i > 0 && era.StartYear <= c.Eras[i - 1].StartYear) p.Add(who + " does not start after the era before it (" + c.Eras[i - 1].Id + ")");
                if (i > 0 && era.TrainingCostPermille < c.Eras[i - 1].TrainingCostPermille) p.Add(who + " makes training cheaper than the era before it: costs only grow");
                if (i > 0 && era.ResearchPermille < c.Eras[i - 1].ResearchPermille) p.Add(who + " makes research slower than the era before it");
                if (era.ResearchPermille < 1) p.Add(who + " needs researchPermille of at least 1");
                EraRulePermille(p, who, "hireFeePermille", era.HireFeePermille);
                EraRulePermille(p, who, "chipPricePermille", era.ChipPricePermille);
                EraRulePermille(p, who, "adoptionPermille", era.AdoptionPermille);
                EraRulePermille(p, who, "finePermille", era.FinePermille);
                if (era.MinReleaseAlignment < 0 || era.MinReleaseAlignment > MathI.Permille) p.Add(who + " has minReleaseAlignment " + era.MinReleaseAlignment + ": it must be from 0 to 1000");
                if (i > 0 && era.MinReleaseAlignment < c.Eras[i - 1].MinReleaseAlignment) p.Add(who + " lowers the release law below the era before it: a law stays on the books");
                if (i == 0 && (era.HireFeePermille != MathI.Permille || era.ChipPricePermille != MathI.Permille || era.AdoptionPermille != MathI.Permille || era.FinePermille != MathI.Permille || era.MinReleaseAlignment != 0))
                    p.Add(who + " is the first era and must leave every era rule at its baseline (1000, and no release law)");
                if (era.TrainingCostPermille < 1) p.Add(who + " needs trainingCostPermille of at least 1");
                if (era.StartCashPermille < 0 || era.StartGrantPermille < 0) p.Add(who + " has a negative start money share");
                if (c.Lot(era.StartLotId) == null) p.Add(who + " starts on the unknown lot \"" + era.StartLotId + "\"");
                if (era.Startable && i > 0)
                {
                    if (era.StartCashPermille < 1) p.Add(who + " is a starting era but gives no start cash");
                    foreach (string missing in StartTechsMissing(c, i)) p.Add(who + " is a starting era but does not start with \"" + missing + "\", a tech of the eras before it");
                }
                if (era.IncidentHeadlines.Count == 0) p.Add(who + " has no incident headlines");
                foreach (string h in era.IncidentHeadlines)
                    if (h.IndexOf("{model}", StringComparison.Ordinal) < 0) p.Add(who + " has an incident headline without {model}: \"" + h + "\"");
                if (string.IsNullOrEmpty(era.Tagline) || string.IsNullOrEmpty(era.ModelsCanDo)) p.Add(who + " needs a tagline and a modelsCanDo line");
                if (era.Unlocks.Count == 0) p.Add(who + " lists nothing under unlocks");

                bool hasTech = false, hasSector = false, hasRoom = false;
                foreach (TechConfig t in c.Research.Techs) if (t.Era == era.Id) hasTech = true;
                foreach (SectorConfig s in c.Market.Sectors) if (s.Era == era.Id) hasSector = true;
                foreach (RoomTypeConfig r in c.RoomTypes) if (r.Era == era.Id) hasRoom = true;
                if (!hasTech) p.Add(who + " has no techs: a whole era with nothing to research");
                if (!hasSector) p.Add(who + " has no sectors: a whole era with nothing new to sell");
                if (!hasRoom) p.Add(who + " has no room types: the building would not change");

                foreach (string id in era.StartTechs)
                {
                    TechConfig tech = c.Tech(id);
                    if (tech == null) { p.Add(who + " starts the labs with the unknown tech \"" + id + "\""); continue; }
                    EraConfig techEra = c.Era(tech.Era);
                    if (techEra != null && techEra.StartYear > era.StartYear) p.Add(who + " starts the labs with " + id + ", which arrives in the later era " + tech.Era);
                    foreach (string req in tech.Requires)
                        if (!era.StartTechs.Contains(req)) p.Add(who + " starts the labs with " + id + " but not its prerequisite " + req);
                }
            }
        }

        static void WorldProblems(List<string> p, GameConfig c)
        {
            WorldConfig w = c.World;
            UniqueIds(p, "world.blocs", w.Blocs);
            UniqueIds(p, "world.sites", w.Sites);
            UniqueIds(p, "world.powerDeals", w.PowerDeals);
            UniqueIds(p, "world.exportRules", w.ExportRules);

            foreach (BlocConfig bloc in w.Blocs)
                if (bloc.StrictnessPermille < 0) p.Add("bloc " + bloc.Id + " has a negative strictness");

            foreach (SiteConfig site in w.Sites)
            {
                string who = "site " + site.Id;
                if (w.Bloc(site.BlocId) == null) p.Add(who + " is in the unknown bloc \"" + site.BlocId + "\"");
                if (site.Plots < 1) p.Add(who + " needs at least one plot");
                if (site.LandPrice < 0 || site.PowerPricePerKwWeek < 0) p.Add(who + " has a negative price");
                if (site.GridKw < 1) p.Add(who + " needs a grid of at least 1 kW");
                if (site.CoolingPermille < 1) p.Add(who + " needs coolingPermille of at least 1");
                Permille(p, who + " summerHeatwavePermille", site.SummerHeatwavePermille);
                Permille(p, who + " townVotePermille", site.TownVotePermille);
                if (site.LatencyPermille < 0 || site.LatencyPermille > 900) p.Add(who + " latencyPermille must be from 0 to 900");
            }

            foreach (PowerDealConfig deal in w.PowerDeals)
            {
                string who = "powerDeal " + deal.Id;
                if (deal.AddsKw < 1) p.Add(who + " adds no power");
                if (deal.Weeks < 1) p.Add(who + " needs at least one week to build");
                if (deal.Cost < 0 || deal.PricePerKwWeek < 0) p.Add(who + " has a negative price");
                if (deal.AttentionPermille < 0) p.Add(who + " has negative attentionPermille");
            }

            HallConfig h = w.Halls;
            if (h.CostPerKw < 0 || h.UpkeepPerKwWeek < 0) p.Add("world.halls has a negative cost");
            if (h.BuildWeeksBase < 0 || h.MinBuildWeeks < 1) p.Add("world.halls needs buildWeeksBase of 0 or more and minBuildWeeks of at least 1");
            if (h.KwPerExtraWeek < 1) p.Add("world.halls.kwPerExtraWeek must be at least 1");
            if (h.MinKw < 1) p.Add("world.halls.minKw must be at least 1");
            Permille(p, "world.halls.infraSpeedCapPermille", h.InfraSpeedCapPermille);
            if (h.InfraSpeedPermillePerStaff < 0) p.Add("world.halls.infraSpeedPermillePerStaff is negative");
            if (h.TownVoteDelayWeeks < 0) p.Add("world.halls.townVoteDelayWeeks is negative");
            if (h.TownVoteAttentionDivisor < 1) p.Add("world.halls.townVoteAttentionDivisor must be at least 1");
            Permille(p, "world.halls.townVoteCapPermille", h.TownVoteCapPermille);
            Permille(p, "world.halls.landRefundPermille", h.LandRefundPermille);

            if (w.Colo.MarkupPerKwWeek < 0) p.Add("world.colo.markupPerKwWeek is negative");
            if (w.Colo.MaxKwPerLabPerSite < 1) p.Add("world.colo.maxKwPerLabPerSite must be at least 1");

            HeatwaveConfig hw = w.Heatwave;
            if (hw.Weeks < 1) p.Add("world.heatwave.weeks must be at least 1");
            Permille(p, "world.heatwave.throttlePermille", hw.ThrottlePermille);
            if (hw.FromMonth < 1 || hw.ToMonth > c.Calendar.MonthsPerYear || hw.FromMonth > hw.ToMonth)
                p.Add("world.heatwave months must run from 1 to " + c.Calendar.MonthsPerYear + " with fromMonth not after toMonth");
            if (hw.RollDivisor < 1) p.Add("world.heatwave.rollDivisor must be at least 1");

            AttentionConfig a = w.Attention;
            Permille(p, "world.attention.decayPermillePerMonth", a.DecayPermillePerMonth);
            Permille(p, "world.attention.policySavingCapPermille", a.PolicySavingCapPermille);
            if (a.DominanceDivisor < 1) p.Add("world.attention.dominanceDivisor must be at least 1");
            if (a.NoticeKwByYear.Count == 0) p.Add("world.attention.noticeKwByYear is empty");
            for (int i = 0; i < a.NoticeKwByYear.Count; i++)
            {
                if (a.NoticeKwByYear[i].Kw < 1) p.Add("world.attention.noticeKwByYear has a footprint below 1 kW");
                if (i > 0 && a.NoticeKwByYear[i].Year <= a.NoticeKwByYear[i - 1].Year) p.Add("world.attention.noticeKwByYear must go up in years");
            }
            if (a.NoticeKwByYear.Count > 0 && a.NoticeKwByYear[0].Year > c.Calendar.StartYear) p.Add("world.attention.noticeKwByYear starts after the game does");
            if (!(0 < a.LetterAt && a.LetterAt < a.HearingAt && a.HearingAt < a.AuditAt && a.AuditAt < a.FreezeAt && a.FreezeAt <= MathI.Permille))
                p.Add("world.attention levels must climb: 0 < letter < hearing < audit < freeze <= 1000");
            if (a.FreezeExitBelow >= a.FreezeAt || a.FreezeExitBelow < 1) p.Add("world.attention.freezeExitBelow must be under the freeze level");
            if (a.DropMargin < 0) p.Add("world.attention.dropMargin is negative");
            if (a.AuditFineRevenueWeeks < 0) p.Add("world.attention.auditFineRevenueWeeks is negative");

            ChinaDealsConfig china = w.ChinaDeals;
            if (china != null)
            {
                if (w.Bloc(china.BlocId) == null) p.Add("world.chinaDeals.blocId names unknown bloc \"" + china.BlocId + "\"");
                if (china.MinInstalledKw < 0) p.Add("world.chinaDeals.minInstalledKw is negative");
                Permille(p, "world.chinaDeals.activePowerPermille", china.ActivePowerPermille);
                if (china.AcceptAttention < 0 || china.DeclineAttention < 0) p.Add("world.chinaDeals attention must be non-negative");
                if (china.DeclineCooldownMonths < 0) p.Add("world.chinaDeals.declineCooldownMonths is negative");
            }

            foreach (ExportRuleConfig rule in w.ExportRules)
            {
                string who = "exportRule " + rule.Id;
                if (rule.MaxTier < 0) p.Add(who + " has a negative maxTier");
                foreach (string b in rule.HomeBlocs) if (w.Bloc(b) == null) p.Add(who + " names the unknown home bloc \"" + b + "\"");
                foreach (string b in rule.DestBlocs) if (w.Bloc(b) == null) p.Add(who + " names the unknown destination bloc \"" + b + "\"");
            }

            if (w.Bloc(c.PlayerStart.HomeBlocId) == null) p.Add("playerStart.homeBlocId names the unknown bloc \"" + c.PlayerStart.HomeBlocId + "\"");
            foreach (RivalConfig rival in c.Rivals)
                if (w.Bloc(rival.HomeBlocId) == null) p.Add("rival " + rival.Id + " has the unknown home bloc \"" + rival.HomeBlocId + "\"");
        }

        static void ChipsProblems(List<string> p, GameConfig c)
        {
            ChipsConfig k = c.Chips;
            if (k.PointsPerArchitectWeek < 1) p.Add("chips.pointsPerArchitectWeek must be at least 1");
            Permille(p, "chips.softwareStart", k.SoftwareStart);
            if (k.SoftwareStart > k.SoftwareBaseCap || k.SoftwareBaseCap > k.SoftwareCap || k.SoftwareCap > 2 * MathI.Permille) p.Add("chips: softwareStart, softwareBaseCap and softwareCap must be in that order and no more than 2000");
            if (k.SoftwarePerArchitectWeek < 0) p.Add("chips.softwarePerArchitectWeek is negative");
            if (k.InHouseTrainingBonusPermille < 0) p.Add("chips.inHouseTrainingBonusPermille is negative");
            if (k.TapeoutWeeks < 1 || k.RespinWeeks < 1) p.Add("chips.tapeoutWeeks and chips.respinWeeks must be at least 1");
            Permille(p, "chips.respinCostPermille", k.RespinCostPermille);
            if (k.RespinPerEngineerPermille < 0 || k.RespinEngineerCap < 0) p.Add("chips.respinPerEngineerPermille and respinEngineerCap must not be negative");
            if (k.YieldRampPerWeek < 0 || k.YieldRampPerEngineer < 0 || k.YieldRampEngineerCap < 0) p.Add("chips: the yield ramp numbers must not be negative");
            if (k.TestLabRampPermille < MathI.Permille) p.Add("chips.testLabRampPermille must be at least 1000: a test lab never slows the ramp");
            Permille(p, "chips.yieldCrashPermille", k.YieldCrashPermille);
            Permille(p, "chips.yieldCrashKeepPermille", k.YieldCrashKeepPermille);
            if (k.SnubStandingBelow < 0 || k.SnubStandingBelow > 1000) p.Add("chips.snubStandingBelow must be from 0 to 1000");
            Permille(p, "chips.snubChancePermille", k.SnubChancePermille);
            if (k.SnubWeeks < 0) p.Add("chips.snubWeeks is negative");
            if (k.FoundryVendorSharePermille < 0 || k.FoundryVendorSharePermille >= MathI.Permille) p.Add("chips.foundryVendorSharePermille must be from 0 to 999");
            if (k.BookAheadMonths < 1) p.Add("chips.bookAheadMonths must be at least 1");
            if (k.ListingMaxPerMonth < 1) p.Add("chips.listingMaxPerMonth must be at least 1");
            if (k.CloudRentPermille < 0 || k.CloudRentPermille > 3000) p.Add("chips.cloudRentPermille must be from 0 to 3000");
            Permille(p, "chips.plannerMinYieldPermille", k.PlannerMinYieldPermille);
            RoleConfig architect = c.Role(k.ArchitectRoleId);
            if (architect == null) p.Add("chips.architectRoleId names the unknown role \"" + k.ArchitectRoleId + "\"");
            else if (architect.Function != RoleFunction.ChipDesign) p.Add("chips.architectRoleId must name a role with the ChipDesign function");
            RoomTypeConfig testLab = c.RoomType(k.TestLabRoomId);
            if (testLab == null) p.Add("chips.testLabRoomId names the unknown room type \"" + k.TestLabRoomId + "\"");

            UniqueIds(p, "chips.targets", k.Targets);
            UniqueIds(p, "chips.nodes", k.Nodes);
            if (k.Targets.Count == 0) p.Add("chips.targets is empty");
            if (k.Nodes.Count == 0) p.Add("chips.nodes is empty");
            foreach (ChipTargetConfig t in k.Targets)
            {
                if (t.Points < 1 || t.ComputePermille < 1) p.Add("chip target " + t.Id + " needs points and computePermille of at least 1");
            }
            for (int i = 0; i < k.Nodes.Count; i++)
            {
                ChipNodeConfig n = k.Nodes[i];
                string who = "chip node " + n.Id;
                if (n.PointsPermille < 1 || n.ComputePerChipWeek < 1 || n.KwPerChip < 1) p.Add(who + " needs pointsPermille, computePerChipWeek and kwPerChip of at least 1");
                if (n.WaferCostPerChip < 1 || n.TapeoutCost < 0) p.Add(who + " has a bad cost");
                Permille(p, who + " respinPermille", n.RespinPermille);
                if (n.InitialYieldPermille < 1 || n.MaxYieldPermille > MathI.Permille || n.InitialYieldPermille > n.MaxYieldPermille) p.Add(who + " needs initialYieldPermille from 1 up to maxYieldPermille, which is at most 1000");
                if (n.LeadWeeks < 1 || n.CapacityPerMonth < 1) p.Add(who + " needs leadWeeks and capacityPerMonth of at least 1");
                if (n.OpensYear < c.Calendar.StartYear) p.Add(who + " opens before the game starts");
                if (i > 0 && (n.OpensYear <= k.Nodes[i - 1].OpensYear || n.Tier < k.Nodes[i - 1].Tier)) p.Add(who + " must open after " + k.Nodes[i - 1].Id + " and not on a lower tier");
            }
            foreach (ChipOptionGroup group in Enum.GetValues(typeof(ChipOptionGroup)))
            {
                bool free = false;
                var seen = new List<string>();
                foreach (ChipOptionConfig o in k.Options)
                {
                    if (o.Group != group) continue;
                    if (seen.Contains(o.Id)) p.Add("chips.options has two " + group + " options called " + o.Id);
                    seen.Add(o.Id);
                    if (o.ComputePermille < 1 || o.CostPermille < 1 || o.KwPermille < 1 || o.PointsPermille < 1) p.Add("chip option " + o.Id + " needs positive multipliers");
                    if (!string.IsNullOrEmpty(o.RequiresTech) && c.Tech(o.RequiresTech) == null) p.Add("chip option " + o.Id + " requires the unknown tech \"" + o.RequiresTech + "\"");
                    if (string.IsNullOrEmpty(o.RequiresTech) || o.RequiresTech == "chip-team") free = true;
                }
                if (!free) p.Add("chips.options has no " + group + " option a lab could use from the start of the chip programme");
            }
        }

        static void RivalryProblems(List<string> p, GameConfig c)
        {
            RivalryConfig r = c.Rivalry;
            if (r.LogMax < 1) p.Add("rivalry.logMax must be at least 1");
            if (r.MinLoggedUnits < 1) p.Add("rivalry.minLoggedUnits must be at least 1");
            Permille(p, "rivalry.rentLogPermille", r.RentLogPermille);
            Permille(p, "rivalry.publishedDiscountPermille", r.PublishedDiscountPermille);
            Permille(p, "rivalry.gateBoostPermille", r.GateBoostPermille);
            if (r.BoardDramaWeeks < 1) p.Add("rivalry.boardDramaWeeks must be at least 1");
            if (r.FastBuildWeeks < 1) p.Add("rivalry.fastBuildWeeks must be at least 1");
            if (r.ReplaceTierGap < 1) p.Add("rivalry.replaceTierGap must be at least 1");
            if (r.RetrenchRunwayWeeks < 1) p.Add("rivalry.retrenchRunwayWeeks must be at least 1");
            if (r.RetrenchCostPermille < MathI.Permille) p.Add("rivalry.retrenchCostPermille must be at least 1000: a plan that breaks even is not losing money");

            FeelingsConfig f = r.Feelings;
            if (f.DriftPerMonth < 0) p.Add("rivalry.feelings.driftPerMonth is negative");
            if (f.FeudBelow >= 0 || f.FeudBelow < -MathI.Permille) p.Add("rivalry.feelings.feudBelow must be from -1000 to -1");
            if (f.FriendlyAbove <= 0 || f.FriendlyAbove > MathI.Permille) p.Add("rivalry.feelings.friendlyAbove must be from 1 to 1000");
            if (f.FeudAttentionPerMonth < 0) p.Add("rivalry.feelings.feudAttentionPerMonth is negative");
            Permille(p, "rivalry.feelings.listedBuyLimitPermille", f.ListedBuyLimitPermille);
            Permille(p, "rivalry.feelings.friendlyBuyLimitPermille", f.FriendlyBuyLimitPermille);
            if (f.FriendlyBuyLimitPermille < f.ListedBuyLimitPermille) p.Add("rivalry.feelings: a friendly rival must pay at least what a neutral one does");

            foreach (string name in Enum.GetNames(typeof(AiLabs.Rules.State.DecisionAction)))
                if (r.ActionTemplate(name) == null) p.Add("rivalry.actionTemplates has no sentence for the action " + name);
            foreach (string name in Enum.GetNames(typeof(AiLabs.Rules.State.ReasonCode)))
                if (r.ReasonTemplate(name) == null) p.Add("rivalry.reasonTemplates has no phrase for the reason " + name);
            UniqueIds(p, "rivalry.actionTemplates", r.ActionTemplates);
            UniqueIds(p, "rivalry.reasonTemplates", r.ReasonTemplates);
        }

        static void VendorProblems(List<string> p, GameConfig c)
        {
            VendorConfig v = c.Vendor;
            if (v.SupplyByYear.Count == 0) { p.Add("vendor.supplyByYear is empty"); return; }
            if (v.SupplyByYear[0].Year > c.Calendar.StartYear) p.Add("vendor.supplyByYear starts after the game does");
            for (int i = 0; i < v.SupplyByYear.Count; i++)
            {
                if (v.SupplyByYear[i].Amount < 1) p.Add("vendor.supplyByYear has a month's supply below 1");
                if (i > 0 && v.SupplyByYear[i].Year <= v.SupplyByYear[i - 1].Year) p.Add("vendor.supplyByYear must go up in years");
            }
            if (v.AllocationBase < 1) p.Add("vendor.allocationBase must be at least 1");
            if (v.StandingMax < 1 || v.StandingStart < 0 || v.StandingStart > v.StandingMax) p.Add("vendor.standingStart must be from 0 to standingMax");
            if (v.GpusPerStanding < 1) p.Add("vendor.gpusPerStanding must be at least 1");
            if (v.TrainingTapeOutDrop < 0 || v.DeliveryDrop < 0 || v.ListingDrop < 0 || v.DriftPerMonth < 0) p.Add("vendor has a negative standing change");
            if (v.KeynoteMonth < 1 || v.KeynoteMonth > c.Calendar.MonthsPerYear) p.Add("vendor.keynoteMonth must be from 1 to " + c.Calendar.MonthsPerYear);
            Permille(p, "vendor.keynoteSupplyPermille", v.KeynoteSupplyPermille);
            if (v.AmbushLookbackWeeks < 0 || v.AmbushWeeks < 0) p.Add("vendor has a negative ambush time");
            Permille(p, "vendor.courtingSharePermille", v.CourtingSharePermille);
            Permille(p, "vendor.shareLogPermille", v.ShareLogPermille);
            if (v.LogMax < 1) p.Add("vendor.logMax must be at least 1");
            if (v.LowStandingMarkupPermille < 0 || v.OwnChipsMarkupPermille < 0 || v.HighStandingDiscountPermille < 0 || v.HighStandingDiscountPermille >= MathI.Permille)
                p.Add("vendor has an impossible price change");
        }

        static void WorldEventProblems(List<string> p, GameConfig c)
        {
            for (int i = 0; i < c.WorldEvents.Count; i++)
            {
                WorldEventConfig e = c.WorldEvents[i];
                string who = "worldEvent " + e.Id;
                if (e.Year < c.Calendar.StartYear) p.Add(who + " is dated before the game begins");
                if (e.Month < 1 || e.Month > c.Calendar.MonthsPerYear) p.Add(who + " has the month " + e.Month + ", which is not from 1 to " + c.Calendar.MonthsPerYear);
                if (string.IsNullOrEmpty(e.Headline)) p.Add(who + " has no headline");
                if (string.IsNullOrEmpty(e.SectorId))
                {
                    if (e.PoolPermille != MathI.Permille) p.Add(who + " names no sector, so poolPermille must be 1000");
                }
                else
                {
                    SectorConfig sector = c.Sector(e.SectorId);
                    if (sector == null) p.Add(who + " moves the unknown sector \"" + e.SectorId + "\"");
                    else if (e.Year < sector.OpensYear) p.Add(who + " moves " + e.SectorId + " before it opens in " + sector.OpensYear);
                    if (e.PoolPermille < 100 || e.PoolPermille > 3000) p.Add(who + " has poolPermille " + e.PoolPermille + ": a single event moves a market from 100 to 3000");
                }
                if (i > 0)
                {
                    WorldEventConfig before = c.WorldEvents[i - 1];
                    if (e.Year < before.Year || (e.Year == before.Year && e.Month < before.Month)) p.Add(who + " is dated before " + before.Id + ": keep worldEvents in date order");
                }
            }
        }

        /// <summary>The problems of one plan; also used for a bot's plan that is not part of the config file.</summary>
        public static void PlanProblems(List<string> p, GameConfig c, string who, PlanConfig plan)
        {
            foreach (PlanStaffConfig st in plan.Staff)
            {
                if (c.Role(st.RoleId) == null) p.Add(who + " plans to hire the unknown role \"" + st.RoleId + "\"");
                if (st.Count < 0) p.Add(who + " plans a negative head count");
                if (st.FromYear < 0) p.Add(who + " plans to hire " + st.RoleId + " from a negative year");
            }
            foreach (string sector in plan.Sectors)
                if (c.Sector(sector) == null) p.Add(who + " plans to sell into the unknown sector \"" + sector + "\"");
            Permille(p, who + " trainingSharePermille", plan.TrainingSharePermille);
            Permille(p, who + " alignmentSharePermille", plan.AlignmentSharePermille);
            if (plan.StartDelayWeeks < 0 || plan.StartJitterWeeks < 0) p.Add(who + " has a negative start delay");
            if (plan.WealthGrowthPermillePerMillion < 0) p.Add(who + " has a negative wealthGrowthPermillePerMillion");
            if (plan.Gpus < 0) p.Add(who + " plans a negative number of GPUs");
            if (plan.ReserveWeeks < 0) p.Add(who + " has a negative reserveWeeks");
            if (plan.MaxRunWeeks < 1) p.Add(who + " needs maxRunWeeks of at least 1");
            Permille(p, who + " siteAppetitePermille", plan.SiteAppetitePermille);
            Permille(p, who + " minReleaseAlignment", plan.MinReleaseAlignment);
            Permille(p, who + " dramaPermillePerYear", plan.DramaPermillePerYear);
            if (plan.ChipProgramYear < 0) p.Add(who + " has a negative chipProgramYear");
            if (plan.ListsChipsMarkupPermille < 0) p.Add(who + " has a negative listsChipsMarkupPermille");
            foreach (string bloc in plan.SiteBlocs)
                if (c.World.Bloc(bloc) == null) p.Add(who + " plans to build in the unknown bloc \"" + bloc + "\"");
            if (who.StartsWith("rival ") && plan.ShadyAppetite != 0)
                p.Add(who + " must have shadyAppetite 0 (parody labs never run shady jobs)");
        }

        static void StaffPeopleProblems(List<string> p, GameConfig c)
        {
            StaffPeopleConfig s = c.StaffPeople;
            if (s == null) { p.Add("staffPeople is missing"); return; }
            Permille(p, "staffPeople.moraleBaseline", s.MoraleBaseline);
            if (s.MoraleDriftPerWeek < 0) p.Add("staffPeople.moraleDriftPerWeek is negative");
            if (s.MoraleDebtDriftPerWeek < 0) p.Add("staffPeople.moraleDebtDriftPerWeek is negative");
            if (s.MoraleIncidentDrop < 0) p.Add("staffPeople.moraleIncidentDrop is negative");
            if (s.MoralePoachWitnessDrop < 0) p.Add("staffPeople.moralePoachWitnessDrop is negative");
            if (s.MaxMoraleLift < 0 || s.MoraleBaseline + s.MaxMoraleLift > MathI.Permille) p.Add("staffPeople.maxMoraleLift needs to keep the resting morale within 0 to 1000");
            if (s.MaxPoachDefencePermille < 0 || s.MaxPoachDefencePermille > 900) p.Add("staffPeople.maxPoachDefencePermille needs 0 to 900, so a rival's offer is never ruled out");
            if (s.MoraleResearchSlopePermille < 0 || s.MoraleResearchSlopePermille > 2000) p.Add("staffPeople.moraleResearchSlopePermille needs 0 to 2000");
            if (s.MoraleResearchLossSlopePermille < 0 || s.MoraleResearchLossSlopePermille > 2000) p.Add("staffPeople.moraleResearchLossSlopePermille needs 0 to 2000");
            Permille(p, "staffPeople.poachChancePermille", s.PoachChancePermille);
            Permille(p, "staffPeople.starHirePermille", s.StarHirePermille);
            if (s.Traits.Count < 1) p.Add("staffPeople.traits is empty");
            UniqueIds(p, "staffPeople.traits", s.Traits);
            bool hasStar = false;
            for (int i = 0; i < s.Traits.Count; i++)
            {
                if (s.Traits[i].Star) hasStar = true;
                TraitProblems(p, s, s.Traits[i]);
            }
            if (!hasStar) p.Add("staffPeople.traits needs one star trait");
            if (s.MaxTraitSwingPermille < 0 || s.MaxTraitSwingPermille > 900) p.Add("staffPeople.maxTraitSwingPermille needs 0 to 900");
            if (s.MaxMentorPermille < 0 || s.MaxMentorPermille > 1000) p.Add("staffPeople.maxMentorPermille needs 0 to 1000");
            if (s.MaxOfferBoostPermille < 0 || s.MaxOfferBoostPermille > 2000) p.Add("staffPeople.maxOfferBoostPermille needs 0 to 2000");
            if (s.FrictionPerPairPermille < 0 || s.FrictionPerPairPermille > 500) p.Add("staffPeople.frictionPerPairPermille needs 0 to 500");
            if (s.MaxFrictionPermille < 0 || s.MaxFrictionPermille > 900) p.Add("staffPeople.maxFrictionPermille needs 0 to 900, so a lab always does some research");
            if (s.TraitRollSalt < 0) p.Add("staffPeople.traitRollSalt is negative");
            if (s.MaxPostsPermille < 0 || s.MaxPostsPermille > 1000) p.Add("staffPeople.maxPostsPermille needs 0 to 1000");
            if (s.QuitReputation < 0 || s.QuitReputation > MathI.Permille) p.Add("staffPeople.quitReputation needs 0 to 1000");
            if (s.CrunchResearchPermille < 0 || s.CrunchResearchPermille > 5000) p.Add("staffPeople.crunchResearchPermille needs 0 to 5000");
            if (s.CrunchTrainingPermille < 0 || s.CrunchTrainingPermille > 5000) p.Add("staffPeople.crunchTrainingPermille needs 0 to 5000");
            if (s.CrunchMoraleDrop < 0 || s.CrunchMoraleDrop > MathI.Permille) p.Add("staffPeople.crunchMoraleDrop needs 0 to 1000");
            if (s.CrunchIncidentPerWeek < 0 || s.CrunchIncidentPerWeek > MathI.Permille) p.Add("staffPeople.crunchIncidentPerWeek needs 0 to 1000");
            if (s.CrunchIncidentCap < 0 || s.CrunchIncidentCap > MathI.Permille) p.Add("staffPeople.crunchIncidentCap needs 0 to 1000");
            if (s.CrunchMinMorale < 0 || s.CrunchMinMorale > MathI.Permille) p.Add("staffPeople.crunchMinMorale needs 0 to 1000");
            if (s.CrunchBreakMorale < 0 || s.CrunchBreakMorale > MathI.Permille) p.Add("staffPeople.crunchBreakMorale needs 0 to 1000");
            if (s.CrunchBreakMorale > s.CrunchMinMorale) p.Add("staffPeople.crunchBreakMorale is above crunchMinMorale, so crunch would end the week it starts");
            if (s.CrunchMaxWeeks < 0 || s.CrunchMaxWeeks > 520) p.Add("staffPeople.crunchMaxWeeks needs 0 to 520");
            if (s.OffsiteSalaryPermille < 0 || s.OffsiteSalaryPermille > 52000) p.Add("staffPeople.offsiteSalaryPermille needs 0 to 52000");
            if (s.OffsiteMoraleLift < 0 || s.OffsiteMoraleLift > MathI.Permille) p.Add("staffPeople.offsiteMoraleLift needs 0 to 1000");
            if (s.OffsiteMaxAboveResting < 0 || s.OffsiteMaxAboveResting > MathI.Permille) p.Add("staffPeople.offsiteMaxAboveResting needs 0 to 1000");
            if (s.OffsiteCultureLift < 0 || s.OffsiteCultureLift > MathI.Permille) p.Add("staffPeople.offsiteCultureLift needs 0 to 1000");
            if (s.PayMinLevel > 0 || s.PayMinLevel < -5) p.Add("staffPeople.payMinLevel needs -5 to 0");
            if (s.PayMaxLevel < 0 || s.PayMaxLevel > 5) p.Add("staffPeople.payMaxLevel needs 0 to 5");
            if (s.PaySalaryStepPermille < 0 || s.PaySalaryStepPermille * (long)-s.PayMinLevel > 900) p.Add("staffPeople.paySalaryStepPermille needs 0 or more, and the lowest level must leave at least a tenth of every salary");
            if (s.PayMoraleStep < 0 || s.PayMoraleStep > 300) p.Add("staffPeople.payMoraleStep needs 0 to 300");
            if (s.PayPoachStepPermille < 0 || s.PayPoachStepPermille > 300) p.Add("staffPeople.payPoachStepPermille needs 0 to 300");
            if (s.PayLockWeeks < 0 || s.PayLockWeeks > 520) p.Add("staffPeople.payLockWeeks needs 0 to 520");
            if (s.PayCutShock < 0 || s.PayCutShock > MathI.Permille) p.Add("staffPeople.payCutShock needs 0 to 1000");
            int hireShare = 0;
            for (int i = 0; i < s.Traits.Count; i++) hireShare += s.Traits[i].HirePermille;
            if (hireShare > MathI.Permille) p.Add("staffPeople.traits hirePermille adds up to " + hireShare + ": it must not pass 1000");
        }

        static void TraitProblems(List<string> p, StaffPeopleConfig s, StaffTraitConfig t)
        {
            string who = "trait " + t.Id;
            if (string.IsNullOrEmpty(t.Name)) p.Add(who + " has no name");
            if (string.IsNullOrEmpty(t.Blurb)) p.Add(who + " has no blurb");
            if (t.ResearchPermille < -900 || t.ResearchPermille > 1000) p.Add(who + " needs researchPermille from -900 to 1000");
            if (t.HirePermille < 0 || t.HirePermille > MathI.Permille) p.Add(who + " needs hirePermille from 0 to 1000");
            if (t.Star && t.HirePermille != 0) p.Add(who + " is the star trait and is rolled by starHirePermille, so its hirePermille must be 0");
            if (t.TrainingSpeedPermille < -500 || t.TrainingSpeedPermille > 500) p.Add(who + " needs trainingSpeedPermille from -500 to 500");
            if (t.AlignmentWorkPermille < -500 || t.AlignmentWorkPermille > 500) p.Add(who + " needs alignmentWorkPermille from -500 to 500");
            if (t.IncidentCatchPermille < -500 || t.IncidentCatchPermille > 500) p.Add(who + " needs incidentCatchPermille from -500 to 500");
            if (t.MentorPermille < 0 || t.MentorPermille > 500) p.Add(who + " needs mentorPermille from 0 to 500");
            if (t.OfferPermille < -500 || t.OfferPermille > 500) p.Add(who + " needs offerPermille from -500 to 500");
            if (t.RestMoraleOffset < -500 || t.RestMoraleOffset > 500) p.Add(who + " needs restMoraleOffset from -500 to 500");
            if (t.BuzzPermille < 0 || t.BuzzPermille > 500) p.Add(who + " needs buzzPermille from 0 to 500");
            if (t.ScandalPermille < 0 || t.ScandalPermille > 500) p.Add(who + " needs scandalPermille from 0 to 500");
            if (t.QuitBelowMorale < 0 || t.QuitBelowMorale > s.MoraleBaseline) p.Add(who + " needs quitBelowMorale from 0 to the baseline morale, or its people quit the day they arrive");
            if (t.InterestingPermille < 0 || t.InterestingPermille > 900) p.Add(who + " needs interestingPermille from 0 to 900");
            if (t.Camp != "" && t.Camp != "doom" && t.Camp != "accel") p.Add(who + " has the camp \"" + t.Camp + "\": it is \"doom\", \"accel\" or empty");
        }

        static void FundingProblems(List<string> p, GameConfig c)
        {
            FundingConfig f = c.Funding;
            if (f == null) { p.Add("funding is missing"); return; }
            Permille(p, "funding.founderStartPermille", f.FounderStartPermille);
            Permille(p, "funding.controlPermille", f.ControlPermille);
            if (string.IsNullOrEmpty(f.OustedSuccessor)) p.Add("funding.oustedSuccessor is empty");
            if (f.Rounds.Count == 0) p.Add("funding.rounds is empty");
            UniqueIds(p, "funding.rounds", f.Rounds);
            for (int i = 0; i < f.Rounds.Count; i++)
            {
                FundingRoundConfig r = f.Rounds[i];
                string who = "funding.rounds[" + r.Id + "]";
                if (r.Cash < 1) p.Add(who + " cash must be positive");
                Permille(p, who + " dilutionPermille", r.DilutionPermille);
                if (r.DilutionPermille < 1) p.Add(who + " dilutionPermille must be at least 1");
                Permille(p, who + " minReputation", r.MinReputation);
                if (r.BoardSeats < 0) p.Add(who + " boardSeats is negative");
            }
            FundingFollowOnConfig fo = f.FollowOn;
            if (fo != null)
            {
                Permille(p, "funding.followOn.downRoundBubblePermille", fo.DownRoundBubblePermille);
                Permille(p, "funding.followOn.downRoundCashPermille", fo.DownRoundCashPermille);
                Permille(p, "funding.followOn.downRoundExtraDilutionPermille", fo.DownRoundExtraDilutionPermille);
                if (fo.BoardDramaWeeks < 0) p.Add("funding.followOn.boardDramaWeeks is negative");
                if (fo.InvestorEventMinRounds < 0) p.Add("funding.followOn.investorEventMinRounds is negative");
            }
        }

        static void Permille(List<string> p, string what, int value)
        {
            if (value < 0 || value > MathI.Permille) p.Add(what + " must be from 0 to 1000");
        }

        static void UniqueIds<T>(List<string> p, string what, List<T> list) where T : IHasId
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrEmpty(list[i].Id)) { p.Add(what + "[" + i + "] has no id"); continue; }
                for (int k = 0; k < i; k++)
                    if (list[k].Id == list[i].Id) p.Add(what + " has the id \"" + list[i].Id + "\" twice");
            }
        }

        static bool Unlockable(GameConfig c, string lineId)
        {
            foreach (TechConfig tech in c.Research.Techs)
                foreach (TechEffectConfig e in tech.Effects)
                    if (e.Kind == TechEffectKind.UnlockLine && e.Id == lineId) return true;
            return false;
        }

        /// <summary>A tech that (through others) requires itself could never be researched.</summary>
        static void TechCycles(List<string> p, GameConfig c)
        {
            foreach (TechConfig start in c.Research.Techs)
            {
                var seen = new List<string>();
                var todo = new List<string>(start.Requires);
                while (todo.Count > 0)
                {
                    string id = todo[todo.Count - 1];
                    todo.RemoveAt(todo.Count - 1);
                    if (id == start.Id) { p.Add("tech " + start.Id + " requires itself through other techs"); break; }
                    if (seen.Contains(id)) continue;
                    seen.Add(id);
                    TechConfig t = c.Tech(id);
                    if (t != null) todo.AddRange(t.Requires);
                }
            }
        }
    }
}
