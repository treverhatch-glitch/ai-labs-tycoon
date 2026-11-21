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
            FundingProblems(p, c);
            StaffPeopleProblems(p, c);
            if (c.Gauntlet.JitterPermille < 0 || c.Gauntlet.JitterPermille >= 1000) p.Add("gauntlet.jitterPermille must be from 0 to 999");

            if (c.Gauntlet.HistoryYears < 1) p.Add("gauntlet.historyYears must be at least 1");

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
            Permille(p, "staffPeople.poachChancePermille", s.PoachChancePermille);
            Permille(p, "staffPeople.starHirePermille", s.StarHirePermille);
            if (s.Traits.Count < 1) p.Add("staffPeople.traits is empty");
            UniqueIds(p, "staffPeople.traits", s.Traits);
            bool hasStar = false;
            for (int i = 0; i < s.Traits.Count; i++)
            {
                if (s.Traits[i].Star) hasStar = true;
                Permille(p, "trait " + s.Traits[i].Id + " researchPermille", s.Traits[i].ResearchPermille);
            }
            if (!hasStar) p.Add("staffPeople.traits needs one star trait");
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
