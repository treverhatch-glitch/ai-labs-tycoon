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
