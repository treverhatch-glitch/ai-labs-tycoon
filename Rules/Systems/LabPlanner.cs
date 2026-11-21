using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Runs a lab by a standing plan. It has no special powers: it looks at the lab the way a player
    /// would and gives the same commands a player gives, so rivals live by the same rules and prices.
    /// The plan says what the lab wants; the lab's cash decides how much of it happens.
    /// </summary>
    public static partial class LabPlanner
    {
        /// <summary>Rooms are built at least this many seats big, so a growing lab is not a maze of closets.</summary>
        const int MinSeatsPerRoom = 4;

        /// <summary>What a plan needs to look at while it works, and the way it gives orders: each order that matters leaves a line in the lab's decision log.</summary>
        sealed class Ctx
        {
            public GameConfig C;
            public GameState State;
            public LabState Lab;
            public PlanConfig Plan;
            public Func<Command, CommandResult> Apply;
        }

        public static void Run(GameConfig c, GameState state, LabState lab, PlanConfig plan, Func<Command, CommandResult> apply)
        {
            if (lab.Bankrupt || state.Tick < lab.PlanStartTick || state.Tick < lab.PlanPausedUntilTick) return;
            var x = new Ctx { C = c, State = state, Lab = lab, Plan = plan, Apply = apply };

            if (lab.TrainingSharePermille != plan.TrainingSharePermille)
                apply(Command.SetTrainingShare(lab.Id, plan.TrainingSharePermille));

            Release(x);

            if (lab.WeeksInDebt > 0)
            {
                Tighten(x);
                return;
            }

            int weeks = state.Tick - lab.PlanStartTick;
            Staff(x, weeks);
            Gpus(x, weeks);
            Refresh(x);
            Sites(x, weeks);
            Chips(x, weeks);
            Research(x);
            Train(x);
        }

        /// <summary>
        /// Gives an order and, when it went through, writes it in the lab's log with the numbers behind it. Small hires and GPU
        /// buys, and renting that moves by less than the smallest logged unit count or by under a quarter, are too small to be worth a line.
        /// </summary>
        static CommandResult Decide(Ctx x, Command cmd, DecisionAction action, string reference, long amount, params Reason[] reasons)
        {
            int rentBefore = x.Lab.RentedGpus;
            CommandResult result = x.Apply(cmd);
            if (!result.Ok) return result;

            RivalryConfig r = x.C.Rivalry;
            if ((action == DecisionAction.Hired || action == DecisionAction.BoughtGpus || action == DecisionAction.Replaced) && cmd.Count < r.MinLoggedUnits) return result;
            if (action == DecisionAction.RentedGpus)
            {
                long moved = Math.Abs((long)x.Lab.RentedGpus - rentBefore);
                if (moved < r.MinLoggedUnits || (rentBefore > 0 && moved * MathI.Permille < (long)rentBefore * r.RentLogPermille)) return result;
                amount = x.Lab.RentedGpus;
            }
            DecisionLog.Add(x.C, x.Lab, x.State.Tick, action, reference, amount, reasons);
            return result;
        }

        /// <summary>A line for something the lab decided not to do, at most once per thing per quarter.</summary>
        static void Note(Ctx x, DecisionAction action, string reference, long amount, params Reason[] reasons)
        {
            if (DecisionLog.LoggedThisQuarter(x.C, x.Lab, action, reference, x.State.Tick)) return;
            DecisionLog.Add(x.C, x.Lab, x.State.Tick, action, reference, amount, reasons);
        }

        /// <summary>Weeks the lab could keep paying its bills with the cash it has, at most 999.</summary>
        static Reason Runway(Ctx x)
        {
            long cost = LabQueries.WeeklyRunningCost(x.C, x.State, x.Lab);
            long weeks = cost <= 0 ? 999 : MathI.Clamp(x.Lab.Cash / cost, 0, 999);
            return Reason.Of(ReasonCode.RunwayWeeks, weeks);
        }

        /// <summary>
        /// A target that grows in a straight line from the plan's starting number, and faster for a lab
        /// with money in the bank: a rich lab does not sit on its cash. Money is counted in the first era's
        /// dollars, so a lab does not mistake the same cash for more as every cost in the world grows.
        /// </summary>
        static int Grown(Ctx x, int start, int growthPermillePerYear, int weeks)
        {
            GameConfig c = x.C;
            long growth = (long)growthPermillePerYear * weeks / c.Calendar.WeeksPerYear;
            if (x.Lab.Cash > 0)
            {
                long firstEraCash = x.Lab.Cash * c.Eras[0].TrainingCostPermille / EraSystem.AtTick(c, x.State.Tick).TrainingCostPermille;
                growth += firstEraCash * x.Plan.WealthGrowthPermillePerMillion / 1000000;
            }
            return (int)MathI.MulPermille(start, MathI.Permille + growth);
        }

        static long Reserve(Ctx x)
        {
            return LabQueries.WeeklyRunningCost(x.C, x.State, x.Lab) * x.Plan.ReserveWeeks;
        }

        /// <summary>A lab that spends half as much again as it earns stops growing until its money catches up. Its starting team and GPUs are exempt.</summary>
        static bool Overstretched(Ctx x)
        {
            long revenue = x.Lab.LastWeek.Revenue;
            return revenue > 0 && LabQueries.WeeklyRunningCost(x.C, x.State, x.Lab) * 2 > revenue * 3;
        }

        static bool CanSpend(Ctx x, long cost)
        {
            return x.Lab.Cash - cost >= Reserve(x);
        }

        /// <summary>In debt: hand back a quarter of the rented GPUs and let one person go from the biggest team.</summary>
        static void Tighten(Ctx x)
        {
            LabState lab = x.Lab;
            if (lab.RentedGpus > 0)
            {
                int give = (lab.RentedGpus + 3) / 4;
                Decide(x, Command.SetRentedGpus(lab.Id, lab.RentedGpus - give), DecisionAction.Tightened, "", give, Reason.Of(ReasonCode.InDebt, lab.WeeksInDebt));
            }

            string biggestRole = "";
            int biggestCount = 0;
            GameConfig cfg = x.C;
            for (int i = 0; i < cfg.Roles.Count; i++)
            {
                int n = LabQueries.StaffCount(lab, cfg.Roles[i].Id);
                if (n > biggestCount) { biggestCount = n; biggestRole = cfg.Roles[i].Id; }
            }
            if (biggestCount > 1)
                Decide(x, Command.Fire(lab.Id, biggestRole, 1), DecisionAction.Tightened, biggestRole, 1, Reason.Of(ReasonCode.InDebt, lab.WeeksInDebt));
        }

        static void Staff(Ctx x, int weeks)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            for (int i = 0; i < plan.Staff.Count; i++)
            {
                RoleConfig role = c.Role(plan.Staff[i].RoleId);
                if (GameDate.Year(c.Calendar, x.State.Tick) < plan.Staff[i].FromYear) continue;
                int target = Grown(x, plan.Staff[i].Count, plan.StaffGrowthPermillePerYear, weeks);
                int have = LabQueries.StaffCount(lab, role.Id);
                if (have >= target) continue;
                if (have >= plan.Staff[i].Count && Overstretched(x)) continue;

                int seats = LabQueries.Seats(c, lab, role.Id);
                if (seats < target)
                {
                    RoomTypeConfig type = RoomFor(c, lab, role.Id, x.State.Tick);
                    if (type == null) continue;
                    int want = Math.Max(target - seats, MinSeatsPerRoom);
                    BuildRoom(x, type, want * type.TilesPerSeat);
                    seats = LabQueries.Seats(c, lab, role.Id);
                }

                // Each hire costs the fee now and raises the reserve the lab wants to keep, so count both.
                long perHire = role.HireCost + role.WeeklySalary * plan.ReserveWeeks;
                long spare = lab.Cash - Reserve(x);
                int affordable = perHire <= 0 ? target : (int)MathI.Clamp(spare / perHire, 0, target);
                int hire = Math.Min(Math.Min(target - have, seats - have), affordable);
                if (hire > 0) Decide(x, Command.Hire(lab.Id, role.Id, hire), DecisionAction.Hired, role.Id, hire, Runway(x));
            }
        }

        static void Gpus(Ctx x, int weeks)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            int target = Grown(x, plan.Gpus, plan.GpuGrowthPermillePerYear, weeks);
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            int owned = LabQueries.OwnedUnits(lab);
            int ordered = LabQueries.ChipsOnOrder(lab);

            if (plan.OwnGpus && owned + ordered < target)
            {
                int hq = LabQueries.HqGpus(lab);
                int slots = LabQueries.GpuSlots(c, lab);
                int rackTarget = HqTarget(plan, target);
                if (slots < rackTarget)
                {
                    RoomTypeConfig type = RackRoom(c, state.Tick);
                    if (type != null)
                    {
                        int tiles = (rackTarget - slots + type.GpuSlotsPerTile - 1) / type.GpuSlotsPerTile;
                        BuildRoom(x, type, tiles);
                        slots = LabQueries.GpuSlots(c, lab);
                    }
                }
                long perGpu = VendorSystem.PricePerGpu(c, state, lab, gen) + gen.UpkeepPerGpuWeek * plan.ReserveWeeks;
                long spare = lab.Cash - Reserve(x);
                int affordable = (int)MathI.Clamp(spare / perGpu, 0, target);
                int buy = Math.Min(Math.Min(target - owned - ordered, Math.Min(slots, rackTarget) - hq), affordable);
                buy = Math.Min(buy, VendorSystem.AllocationLeft(state, lab));
                if (buy > 0) BuyGpus(x, buy, HardwareBatch.Hq, target - owned - ordered, DecisionAction.BoughtGpus);
                owned = LabQueries.OwnedUnits(lab);
            }

            // Rent what the lab does not own, as far as the reserve allows. Rent is weekly, so "afford" means the reserve covers it.
            int rent = Math.Max(0, target - owned);
            if (lab.RentedGpus >= plan.Gpus && Overstretched(x)) rent = Math.Min(rent, lab.RentedGpus);
            if (rent > lab.RentedGpus && plan.ReserveWeeks > 0)
            {
                long withoutRent = LabQueries.WeeklyRunningCost(c, state, lab) - LabQueries.WeeklyGpuRent(c, state, lab);
                long weekly = lab.Cash / plan.ReserveWeeks - withoutRent;
                int affordable = gen.RentPerGpuWeek <= 0 ? rent : (int)MathI.Clamp(weekly / gen.RentPerGpuWeek, 0, rent);
                rent = Math.Max(lab.RentedGpus, affordable);
            }
            if (rent != lab.RentedGpus)
                Decide(x, Command.SetRentedGpus(lab.Id, rent), DecisionAction.RentedGpus, "", rent, Reason.Of(ReasonCode.GpuTargetGap, Math.Max(0, target - owned)), Runway(x));
        }

        /// <summary>
        /// A lab that owns its hardware does not let it age for ever: each month the oldest batch that is two or more generations
        /// behind is sold at resale and bought again as current-generation GPUs in the same place, as many as the vendor will
        /// sell, the hall or racks will hold and the cash allows. Only the buying is written in the log.
        /// </summary>
        static void Refresh(Ctx x)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            if (!x.Plan.OwnGpus || Overstretched(x)) return;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            HardwareBatch stale = null;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.GpuGenId == null || b.ChipDesignId >= 0 || gen.Tier - Fleet.TierOf(c, b) < c.Rivalry.ReplaceTierGap) continue;
                if (stale == null || Fleet.TierOf(c, b) < Fleet.TierOf(c, stale) || (Fleet.TierOf(c, b) == Fleet.TierOf(c, stale) && b.Id < stale.Id)) stale = b;
            }
            if (stale == null) return;

            string blocId = WorldQueries.BlocOfPlace(c, lab, stale.HoldingId);
            if (WorldQueries.IsFrozen(lab, blocId) || !LabQueries.ExportAllowed(c, state, lab, gen.Tier, blocId)) return;
            GpuGenerationConfig old = c.Compute.Generation(stale.GpuGenId);
            long resale = LabQueries.ResalePerUnit(c, old, state.Tick);
            long price = VendorSystem.PricePerGpu(c, state, lab, gen);
            int count = Math.Min(stale.Count, VendorSystem.AllocationLeft(state, lab));
            if (stale.HoldingId == HardwareBatch.Hq)
            {
                count = Math.Min(count, LabQueries.GpuSlots(c, lab) - LabQueries.HqGpus(lab) + stale.Count);
            }
            else
            {
                Holding holding = lab.Holding(stale.HoldingId);
                int room = WorldQueries.FreeKw(c, state, lab, holding) + stale.Count * old.KwPerGpu;
                count = Math.Min(count, gen.KwPerGpu <= 0 ? count : room / gen.KwPerGpu);
            }
            long net = price - resale;
            if (net > 0) count = (int)Math.Min(count, Math.Max(0, (lab.Cash - Reserve(x)) / net));
            if (count < 1) return;

            int behind = gen.Tier - Fleet.TierOf(c, stale);
            int holdingId = stale.HoldingId;
            x.Apply(Command.SellGpus(lab.Id, count, holdingId));
            Decide(x, Command.BuyGpus(lab.Id, count, holdingId), DecisionAction.Replaced, gen.Id, count,
                Reason.Of(ReasonCode.GenerationsBehind, behind),
                Reason.Of(ReasonCode.AllocationLeft, VendorSystem.AllocationLeft(state, lab)),
                Runway(x));
        }

        /// <summary>Buys current-generation GPUs and writes down why: the gap to the target, what the vendor will still sell the lab, and its price.</summary>
        static CommandResult BuyGpus(Ctx x, int count, int holdingId, int gap, DecisionAction action)
        {
            GameConfig c = x.C;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, x.State.Tick);
            return Decide(x, Command.BuyGpus(x.Lab.Id, count, holdingId), action, gen.Id, count,
                Reason.Of(ReasonCode.GpuTargetGap, gap),
                Reason.Of(ReasonCode.AllocationLeft, VendorSystem.AllocationLeft(x.State, x.Lab)),
                Reason.Of(ReasonCode.PriceVsEnvydiaPermille, VendorSystem.PricePermille(c, x.State, x.Lab)),
                Runway(x));
        }

        /// <summary>
        /// A lab that builds data centres keeps only its starting fleet at the HQ: past that the hardware goes to sites,
        /// and until a hall is ready the lab rents.
        /// </summary>
        static int HqTarget(PlanConfig plan, int target)
        {
            return plan.SiteAppetitePermille > 0 ? Math.Min(target, plan.Gpus) : target;
        }

        /// <summary>
        /// Once the HQ is full and the lab still wants more hardware: put it in the hall with the most free power; failing that,
        /// buy land at the cheapest open site in the plan's blocs (power times cooling) and start a hall sized by the plan's
        /// appetite. Everything goes through the player's commands, so a frozen bloc or an empty bank stops it like anyone.
        /// </summary>
        static void Sites(Ctx x, int weeks)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            if (!plan.OwnGpus || plan.SiteAppetitePermille <= 0) return;
            int target = Grown(x, plan.Gpus, plan.GpuGrowthPermillePerYear, weeks);
            int want = target - LabQueries.OwnedUnits(lab) - LabQueries.ChipsOnOrder(lab);
            if (want <= 0) return;
            if (Math.Min(LabQueries.GpuSlots(c, lab), plan.Gpus) - LabQueries.HqGpus(lab) > 0) return;

            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            Holding roomiest = null;
            int mostFree = 0;
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                int free = WorldQueries.FreeKw(c, state, lab, lab.Holdings[i]);
                if (roomiest == null || free > mostFree) { roomiest = lab.Holdings[i]; mostFree = free; }
            }
            if (roomiest != null && mostFree >= gen.KwPerGpu)
            {
                long spare = lab.Cash - Reserve(x);
                long price = VendorSystem.PricePerGpu(c, state, lab, gen);
                int buy = (int)MathI.Clamp(Math.Min(want, mostFree / gen.KwPerGpu), 0, price <= 0 ? want : spare / price);
                buy = Math.Min(buy, VendorSystem.AllocationLeft(state, lab));
                if (buy > 0 && BuyGpus(x, buy, roomiest.Id, want, DecisionAction.BoughtGpus).Ok) want -= buy;
            }
            if (want <= 0) return;

            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                if (lab.Holdings[i].HallDoneTick >= 0) return;
                if (WorldQueries.FreeKw(c, state, lab, lab.Holdings[i]) >= gen.KwPerGpu) return;
            }

            HallConfig hall = c.World.Halls;
            long wantKw = MathI.Max(hall.MinKw, (long)want * gen.KwPerGpu * plan.SiteAppetitePermille / MathI.Permille);

            Holding host = null;
            for (int i = 0; i < lab.Holdings.Count && host == null; i++)
            {
                Holding h = lab.Holdings[i];
                if (h.Kind != HoldingKind.Owned || WorldQueries.IsFrozen(lab, WorldQueries.BlocOfHolding(c, h))) continue;
                if (GridRoom(c, state, c.World.Site(h.SiteId)) >= hall.MinKw) host = h;
            }
            if (host == null)
            {
                SiteConfig pick = CheapestSite(x);
                if (pick == null || !CanSpend(x, pick.LandPrice + hall.MinKw * hall.CostPerKw)) return;
                CommandResult bought = Decide(x, Command.BuyLand(lab.Id, pick.Id), DecisionAction.BoughtLand, pick.Id, pick.LandPrice,
                    Reason.Of(ReasonCode.CheapestSite, MathI.MulPermille(pick.PowerPricePerKwWeek, pick.CoolingPermille)),
                    Reason.Of(ReasonCode.PowerPrice, pick.PowerPricePerKwWeek),
                    Reason.Of(ReasonCode.GpuTargetGap, want));
                if (!bought.Ok) return;
                host = lab.Holding(bought.CreatedId);
            }

            SiteConfig site = c.World.Site(host.SiteId);
            long kw = MathI.Min(wantKw, GridRoom(c, state, site) + host.PrivateKw);
            long affordable = hall.CostPerKw <= 0 ? kw : (lab.Cash - Reserve(x)) / hall.CostPerKw;
            kw = MathI.Min(kw, affordable);
            if (kw >= hall.MinKw)
                Decide(x, Command.BuildHall(lab.Id, host.Id, (int)kw), DecisionAction.BuiltHall, site.Id, kw,
                    Reason.Of(ReasonCode.GpuTargetGap, want), Reason.Of(ReasonCode.PowerPrice, site.PowerPricePerKwWeek));
        }

        static long GridRoom(GameConfig c, GameState state, SiteConfig site)
        {
            return site.GridKw - WorldQueries.SiteGridUsed(state, site.Id);
        }

        /// <summary>The open site in the plan's blocs with a free plot and grid room whose power and cooling cost the least, or null.</summary>
        static SiteConfig CheapestSite(Ctx x)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            SiteConfig best = null;
            long bestPrice = 0;
            for (int i = 0; i < c.World.Sites.Count; i++)
            {
                SiteConfig site = c.World.Sites[i];
                if (!x.Plan.SiteBlocs.Contains(site.BlocId) || !WorldQueries.SiteOpen(c, site, state.Tick)) continue;
                if (WorldQueries.PlotsTaken(state, site.Id) >= site.Plots) continue;
                if (WorldQueries.HoldingAt(lab, site.Id, HoldingKind.Owned) != null || WorldQueries.IsFrozen(lab, site.BlocId)) continue;
                if (GridRoom(c, state, site) < c.World.Halls.MinKw) continue;
                long price = (long)site.PowerPricePerKwWeek * site.CoolingPermille;
                if (best == null || price < bestPrice) { best = site; bestPrice = price; }
            }
            return best;
        }

        /// <summary>Buys every tech it can, in the order the config lists them. A lab that publishes its safety research says so when it learns one.</summary>
        static void Research(Ctx x)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            bool bought = true;
            while (bought)
            {
                bought = false;
                for (int i = 0; i < c.Research.Techs.Count; i++)
                {
                    TechConfig tech = c.Research.Techs[i];
                    long cost = LabQueries.TechCost(c, x.State, lab, tech);
                    if (!LabQueries.CanResearch(c, lab, tech, x.State.Tick) || lab.ResearchPoints < cost) continue;
                    CommandResult result = x.Apply(Command.ResearchTech(lab.Id, tech.Id));
                    if (!result.Ok) continue;
                    bought = true;
                    if (x.Plan.PublishesSafetyResearch && LabQueries.IsSafetyTech(tech))
                        DecisionLog.Add(c, lab, x.State.Tick, DecisionAction.Published, tech.Id, cost,
                            Reason.Of(ReasonCode.CultureAlignmentPermille, x.Plan.AlignmentSharePermille));
                }
            }
            if (EndingSystem.AllOpenTechsDone(c, lab, x.State.Tick))
            {
                long cost = EndingSystem.FrontierCost(c, lab);
                if (cost > 0 && lab.ResearchPoints >= cost) x.Apply(Command.ResearchFrontier(lab.Id));
            }
        }

        /// <summary>
        /// The alignment share a run starts with: the plan's own, and 100 more for every model in a row that the release gate held back,
        /// to a limit. A model that passes the gate ends the streak.
        /// </summary>
        public static int RunAlignmentShare(GameConfig c, LabState lab, PlanConfig plan)
        {
            int boost = 0;
            if (plan.MinReleaseAlignment > 0)
            {
                for (int i = lab.Models.Count - 1; i >= 0; i--)
                {
                    ModelState m = lab.Models[i];
                    if (m.Status == ModelStatus.Training) continue;
                    if (m.Status == ModelStatus.Trained && m.Alignment < plan.MinReleaseAlignment) boost += c.Rivalry.GateBoostPermille;
                    else break;
                }
            }
            return (int)MathI.Min(CommandProcessor.MaxAlignmentSharePermille, plan.AlignmentSharePermille + boost);
        }

        /// <summary>
        /// Starts one run at a time: the biggest line the lab can afford and finish in reasonable time,
        /// and only when the new model would clearly beat the lab's best.
        /// </summary>
        static void Train(Ctx x)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            if (LabQueries.ActiveRuns(lab) > 0) return;

            int best = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status != ModelStatus.Training && lab.Models[i].Status != ModelStatus.Retired)
                    best = Math.Max(best, lab.Models[i].Capability);
            long bar = MathI.MulPermille(best, MathI.Permille + plan.MinGainPermille);
            int share = RunAlignmentShare(c, lab, plan);
            bool first = true;

            for (int i = c.Training.Lines.Count - 1; i >= 0; i--)
            {
                ModelLineConfig line = c.Training.Lines[i];
                if (!LabQueries.LineUnlocked(c, lab, line.Id)) continue;
                if (lab.Controller != LabController.Player && line.SpecialistQualityBonusPermille > 0) continue;
                bool report = first;
                first = false;
                // A run is how a lab earns its next dollar, so it may dip into half of the reserve.
                if (lab.Cash - EraSystem.RunPrepCost(c, line, state.Tick) < Reserve(x) / 2)
                {
                    if (report) Note(x, DecisionAction.SkippedRun, line.Id, 0, Runway(x));
                    continue;
                }
                int weeks = TrainingSystem.EstimateWeeks(c, state, lab, line, share);
                if (weeks < 1 || weeks > plan.MaxRunWeeks)
                {
                    if (report) Note(x, DecisionAction.SkippedRun, line.Id, weeks, Reason.Of(ReasonCode.MaxRunWeeks, plan.MaxRunWeeks), Runway(x));
                    continue;
                }
                CapabilityForecast forecast = TrainingSystem.Forecast(c, lab, line);
                if (best > 0 && forecast.Total < bar)
                {
                    if (report) Note(x, DecisionAction.SkippedRun, line.Id, weeks, Reason.Of(ReasonCode.ForecastGainPermille, GainPermille(forecast.Total, best)), Reason.Of(ReasonCode.RivalBestCapability, RivalSystem.BestOtherCapability(state, lab)));
                    continue;
                }
                if (Decide(x, Command.StartTraining(lab.Id, line.Id, share), DecisionAction.StartedRun, line.Id, weeks,
                    Reason.Of(ReasonCode.ForecastGainPermille, GainPermille(forecast.Total, best)),
                    Reason.Of(ReasonCode.MaxRunWeeks, plan.MaxRunWeeks),
                    Reason.Of(ReasonCode.RivalBestCapability, RivalSystem.BestOtherCapability(state, lab)),
                    Runway(x)).Ok) return;
            }
        }

        static long GainPermille(int forecast, int best)
        {
            return best <= 0 ? 0 : (long)(forecast - best) * MathI.Permille / best;
        }

        /// <summary>
        /// Puts each finished model on sale wherever the plan sells and the model is the lab's best offer. A model with less alignment than the
        /// plan's gate is held back instead: the lab would rather not sell it.
        /// </summary>
        static void Release(Ctx x)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) continue;
                if (model.Capability < 1) continue;
                if (model.Alignment < plan.MinReleaseAlignment)
                {
                    Note(x, DecisionAction.HeldBack, model.Name, model.Alignment,
                        Reason.Of(ReasonCode.AlignmentBelowGate, model.Alignment),
                        Reason.Of(ReasonCode.ReleaseGate, plan.MinReleaseAlignment),
                        Reason.Of(ReasonCode.CultureAlignmentPermille, plan.AlignmentSharePermille));
                    continue;
                }

                var sectors = new List<string>();
                int rivalWon = 0;
                for (int k = 0; k < plan.Sectors.Count; k++)
                {
                    SectorConfig sector = c.Sector(plan.Sectors[k]);
                    if (!LabQueries.SectorOpen(c, sector, state.Tick) || model.Capability < sector.MinCapability) continue;
                    SectorState sectorState = state.Sector(sector.Id);
                    SectorEntry entry = sectorState.Entry(lab.Id);
                    if (entry != null)
                    {
                        if (entry.ModelId == model.Id) continue;
                        ModelState current = lab.Model(entry.ModelId);
                        if (LabQueries.Quality(c, sector, model, state.Tick) <= LabQueries.Quality(c, sector, current, state.Tick)) continue;
                    }
                    sectors.Add(sector.Id);
                    if (sectorState.ChampionLabId >= 0 && sectorState.ChampionLabId != lab.Id) rivalWon++;
                }
                if (sectors.Count > 0)
                    Decide(x, Command.ReleaseModel(lab.Id, model.Id, sectors.ToArray()), DecisionAction.Released, model.Name, sectors.Count,
                        Reason.Of(ReasonCode.RivalBestCapability, RivalSystem.BestOtherCapability(state, lab)),
                        Reason.Of(ReasonCode.RivalWonBracket, rivalWon));
            }
        }

        /// <summary>The densest room type that seats the role and is open: later eras seat more people to the tile.</summary>
        static RoomTypeConfig RoomFor(GameConfig c, LabState lab, string roleId, int tick)
        {
            RoomTypeConfig best = null;
            for (int i = 0; i < c.RoomTypes.Count; i++)
            {
                RoomTypeConfig type = c.RoomTypes[i];
                if (type.SeatsRole != roleId || !EraSystem.RoomOpen(c, type, tick) || !LabQueries.RoomUnlocked(lab, type)) continue;
                if (best == null || type.TilesPerSeat <= best.TilesPerSeat) best = type;
            }
            return best;
        }

        /// <summary>The open room type with the most rack space to the tile.</summary>
        static RoomTypeConfig RackRoom(GameConfig c, int tick)
        {
            RoomTypeConfig best = null;
            for (int i = 0; i < c.RoomTypes.Count; i++)
            {
                RoomTypeConfig type = c.RoomTypes[i];
                if (type.GpuSlotsPerTile <= 0 || !EraSystem.RoomOpen(c, type, tick)) continue;
                if (best == null || type.GpuSlotsPerTile > best.GpuSlotsPerTile) best = type;
            }
            return best;
        }

        /// <summary>
        /// Builds a roughly square room of at least this many tiles in the first free spot, buying the
        /// next lot when the current one is full. Does nothing when the lab cannot afford it.
        /// </summary>
        static bool BuildRoom(Ctx x, RoomTypeConfig type, int tiles)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            int w = type.MinWidth;
            while (w * w < tiles) w++;
            int h = Math.Max(type.MinHeight, (tiles + w - 1) / w);
            if (!CanSpend(x, type.BuildCostPerTile * w * h)) return false;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                LotConfig lot = c.Lot(lab.LotId);
                int px, py;
                if (LotGrid.FindFreeRect(lot, lab.Rooms, w, h, out px, out py))
                    return Decide(x, Command.BuildRoom(lab.Id, type.Id, px, py, w, h), DecisionAction.Built, type.Id, type.BuildCostPerTile * w * h, Runway(x)).Ok;
                if (h >= type.MinWidth && w >= type.MinHeight && LotGrid.FindFreeRect(lot, lab.Rooms, h, w, out px, out py))
                    return Decide(x, Command.BuildRoom(lab.Id, type.Id, px, py, h, w), DecisionAction.Built, type.Id, type.BuildCostPerTile * w * h, Runway(x)).Ok;

                int at = c.Lots.IndexOf(lot);
                if (at + 1 >= c.Lots.Count) return false;
                if (!CanSpend(x, c.Lots[at + 1].Price + type.BuildCostPerTile * w * h)) return false;
                if (!Decide(x, Command.BuyLot(lab.Id), DecisionAction.Built, c.Lots[at + 1].Id, c.Lots[at + 1].Price, Runway(x)).Ok) return false;
            }
            return false;
        }
    }
}
