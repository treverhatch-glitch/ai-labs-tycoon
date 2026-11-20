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

            foreach (RoomTypeConfig room in c.RoomTypes)
            {
                bool seats = !string.IsNullOrEmpty(room.SeatsRole);
                if (c.Era(room.Era) == null) p.Add("roomType " + room.Id + " names the unknown era \"" + room.Era + "\"");
                if (seats && c.Role(room.SeatsRole) == null) p.Add("roomType " + room.Id + " seats the unknown role \"" + room.SeatsRole + "\"");
                if (seats && room.TilesPerSeat < 1) p.Add("roomType " + room.Id + " seats staff, so tilesPerSeat must be at least 1");
                if (room.GpuSlotsPerTile < 0) p.Add("roomType " + room.Id + " has negative gpuSlotsPerTile");
                if (room.MinWidth < 1 || room.MinHeight < 1) p.Add("roomType " + room.Id + " needs minWidth and minHeight of at least 1");
                if (room.BuildCostPerTile < 0 || room.UpkeepPerTileWeek < 0) p.Add("roomType " + room.Id + " has a negative cost");
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
                    if (e.Kind != TechEffectKind.UnlockLine && e.Amount <= 0) p.Add("tech " + tech.Id + " has an effect with no amount");
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
                PlanProblems(p, c, "rival " + rival.Id, rival.Plan);
            }

            PlanProblems(p, c, "autopilot", c.Autopilot);

            WorldEventProblems(p, c);

            return p;
        }

        static void ComputeProblems(List<string> p, GameConfig c)
        {
            ComputeConfig compute = c.Compute;
            Permille(p, "compute.resalePermille", compute.ResalePermille);
            Permille(p, "compute.generationDecayPermille", compute.GenerationDecayPermille);
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
                if (era.TrainingCostPermille < 1) p.Add(who + " needs trainingCostPermille of at least 1");
                if (era.StartCashPermille < 0 || era.StartGrantPermille < 0) p.Add(who + " has a negative start money share");
                if (c.Lot(era.StartLotId) == null) p.Add(who + " starts on the unknown lot \"" + era.StartLotId + "\"");
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
            foreach (string bloc in plan.SiteBlocs)
                if (c.World.Bloc(bloc) == null) p.Add(who + " plans to build in the unknown bloc \"" + bloc + "\"");
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
