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
    public static class LabPlanner
    {
        /// <summary>Rooms are built at least this many seats big, so a growing lab is not a maze of closets.</summary>
        const int MinSeatsPerRoom = 4;

        public static void Run(GameConfig c, GameState state, LabState lab, PlanConfig plan, Func<Command, CommandResult> apply)
        {
            if (lab.Bankrupt || state.Tick < lab.PlanStartTick) return;

            if (lab.TrainingSharePermille != plan.TrainingSharePermille)
                apply(Command.SetTrainingShare(lab.Id, plan.TrainingSharePermille));

            Release(c, state, lab, plan, apply);

            if (lab.WeeksInDebt > 0)
            {
                Tighten(lab, apply);
                return;
            }

            int weeks = state.Tick - lab.PlanStartTick;
            Staff(c, state, lab, plan, weeks, apply);
            Gpus(c, state, lab, plan, weeks, apply);
            Sites(c, state, lab, plan, weeks, apply);
            Research(c, state, lab, apply);
            Train(c, state, lab, plan, apply);
        }

        /// <summary>
        /// A target that grows in a straight line from the plan's starting number, and faster for a lab
        /// with money in the bank: a rich lab does not sit on its cash. Money is counted in the first era's
        /// dollars, so a lab does not mistake the same cash for more as every cost in the world grows.
        /// </summary>
        static int Grown(GameConfig c, GameState state, LabState lab, PlanConfig plan, int start, int growthPermillePerYear, int weeks)
        {
            long growth = (long)growthPermillePerYear * weeks / c.Calendar.WeeksPerYear;
            if (lab.Cash > 0)
            {
                long firstEraCash = lab.Cash * c.Eras[0].TrainingCostPermille / EraSystem.AtTick(c, state.Tick).TrainingCostPermille;
                growth += firstEraCash * plan.WealthGrowthPermillePerMillion / 1000000;
            }
            return (int)MathI.MulPermille(start, MathI.Permille + growth);
        }

        static long Reserve(GameConfig c, GameState state, LabState lab, PlanConfig plan)
        {
            return LabQueries.WeeklyRunningCost(c, state, lab) * plan.ReserveWeeks;
        }

        /// <summary>A lab that spends half as much again as it earns stops growing until its money catches up. Its starting team and GPUs are exempt.</summary>
        static bool Overstretched(GameConfig c, GameState state, LabState lab)
        {
            long revenue = lab.LastWeek.Revenue;
            return revenue > 0 && LabQueries.WeeklyRunningCost(c, state, lab) * 2 > revenue * 3;
        }

        static bool CanSpend(GameConfig c, GameState state, LabState lab, PlanConfig plan, long cost)
        {
            return lab.Cash - cost >= Reserve(c, state, lab, plan);
        }

        /// <summary>In debt: hand back a quarter of the rented GPUs and let one person go from the biggest team.</summary>
        static void Tighten(LabState lab, Func<Command, CommandResult> apply)
        {
            if (lab.RentedGpus > 0) apply(Command.SetRentedGpus(lab.Id, lab.RentedGpus - (lab.RentedGpus + 3) / 4));

            StaffPool biggest = null;
            for (int i = 0; i < lab.Staff.Count; i++)
                if (biggest == null || lab.Staff[i].Count > biggest.Count) biggest = lab.Staff[i];
            if (biggest != null && biggest.Count > 1) apply(Command.Fire(lab.Id, biggest.RoleId, 1));
        }

        static void Staff(GameConfig c, GameState state, LabState lab, PlanConfig plan, int weeks, Func<Command, CommandResult> apply)
        {
            for (int i = 0; i < plan.Staff.Count; i++)
            {
                RoleConfig role = c.Role(plan.Staff[i].RoleId);
                int target = Grown(c, state, lab, plan, plan.Staff[i].Count, plan.StaffGrowthPermillePerYear, weeks);
                int have = LabQueries.StaffCount(lab, role.Id);
                if (have >= target) continue;
                if (have >= plan.Staff[i].Count && Overstretched(c, state, lab)) continue;

                int seats = LabQueries.Seats(c, lab, role.Id);
                if (seats < target)
                {
                    RoomTypeConfig type = RoomFor(c, role.Id, state.Tick);
                    if (type == null) continue;
                    int want = Math.Max(target - seats, MinSeatsPerRoom);
                    BuildRoom(c, state, lab, plan, type, want * type.TilesPerSeat, apply);
                    seats = LabQueries.Seats(c, lab, role.Id);
                }

                // Each hire costs the fee now and raises the reserve the lab wants to keep, so count both.
                long perHire = role.HireCost + role.WeeklySalary * plan.ReserveWeeks;
                long spare = lab.Cash - Reserve(c, state, lab, plan);
                int affordable = perHire <= 0 ? target : (int)MathI.Clamp(spare / perHire, 0, target);
                int hire = Math.Min(Math.Min(target - have, seats - have), affordable);
                if (hire > 0) apply(Command.Hire(lab.Id, role.Id, hire));
            }
        }

        static void Gpus(GameConfig c, GameState state, LabState lab, PlanConfig plan, int weeks, Func<Command, CommandResult> apply)
        {
            int target = Grown(c, state, lab, plan, plan.Gpus, plan.GpuGrowthPermillePerYear, weeks);
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            int owned = LabQueries.OwnedUnits(lab);

            if (plan.OwnGpus && owned < target)
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
                        BuildRoom(c, state, lab, plan, type, tiles, apply);
                        slots = LabQueries.GpuSlots(c, lab);
                    }
                }
                long perGpu = gen.BuyPrice + gen.UpkeepPerGpuWeek * plan.ReserveWeeks;
                long spare = lab.Cash - Reserve(c, state, lab, plan);
                int affordable = (int)MathI.Clamp(spare / perGpu, 0, target);
                int buy = Math.Min(Math.Min(target - owned, Math.Min(slots, rackTarget) - hq), affordable);
                if (buy > 0) apply(Command.BuyGpus(lab.Id, buy));
                owned = LabQueries.OwnedUnits(lab);
            }

            // Rent what the lab does not own, as far as the reserve allows. Rent is weekly, so "afford" means the reserve covers it.
            int rent = Math.Max(0, target - owned);
            if (lab.RentedGpus >= plan.Gpus && Overstretched(c, state, lab)) rent = Math.Min(rent, lab.RentedGpus);
            if (rent > lab.RentedGpus && plan.ReserveWeeks > 0)
            {
                long withoutRent = LabQueries.WeeklyRunningCost(c, state, lab) - LabQueries.WeeklyGpuRent(c, state, lab);
                long weekly = lab.Cash / plan.ReserveWeeks - withoutRent;
                int affordable = gen.RentPerGpuWeek <= 0 ? rent : (int)MathI.Clamp(weekly / gen.RentPerGpuWeek, 0, rent);
                rent = Math.Max(lab.RentedGpus, affordable);
            }
            if (rent != lab.RentedGpus) apply(Command.SetRentedGpus(lab.Id, rent));
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
        static void Sites(GameConfig c, GameState state, LabState lab, PlanConfig plan, int weeks, Func<Command, CommandResult> apply)
        {
            if (!plan.OwnGpus || plan.SiteAppetitePermille <= 0) return;
            int target = Grown(c, state, lab, plan, plan.Gpus, plan.GpuGrowthPermillePerYear, weeks);
            int want = target - LabQueries.OwnedUnits(lab);
            if (want <= 0) return;
            if (Math.Min(LabQueries.GpuSlots(c, lab), plan.Gpus) - LabQueries.HqGpus(lab) > 0) return;

            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            Holding roomiest = null;
            int mostFree = 0;
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                int free = WorldQueries.FreeKw(c, lab, lab.Holdings[i]);
                if (roomiest == null || free > mostFree) { roomiest = lab.Holdings[i]; mostFree = free; }
            }
            if (roomiest != null && mostFree >= gen.KwPerGpu)
            {
                long spare = lab.Cash - Reserve(c, state, lab, plan);
                int buy = (int)MathI.Clamp(Math.Min(want, mostFree / gen.KwPerGpu), 0, gen.BuyPrice <= 0 ? want : spare / gen.BuyPrice);
                if (buy > 0 && apply(Command.BuyGpus(lab.Id, buy, roomiest.Id)).Ok) want -= buy;
            }
            if (want <= 0) return;

            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                if (lab.Holdings[i].HallDoneTick >= 0) return;
                if (WorldQueries.FreeKw(c, lab, lab.Holdings[i]) >= gen.KwPerGpu) return;
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
                SiteConfig pick = CheapestSite(c, state, lab, plan);
                if (pick == null || !CanSpend(c, state, lab, plan, pick.LandPrice + hall.MinKw * hall.CostPerKw)) return;
                CommandResult bought = apply(Command.BuyLand(lab.Id, pick.Id));
                if (!bought.Ok) return;
                host = lab.Holding(bought.CreatedId);
            }

            SiteConfig site = c.World.Site(host.SiteId);
            long kw = MathI.Min(wantKw, GridRoom(c, state, site) + host.PrivateKw);
            long affordable = hall.CostPerKw <= 0 ? kw : (lab.Cash - Reserve(c, state, lab, plan)) / hall.CostPerKw;
            kw = MathI.Min(kw, affordable);
            if (kw >= hall.MinKw) apply(Command.BuildHall(lab.Id, host.Id, (int)kw));
        }

        static long GridRoom(GameConfig c, GameState state, SiteConfig site)
        {
            return site.GridKw - WorldQueries.SiteGridUsed(state, site.Id);
        }

        /// <summary>The open site in the plan's blocs with a free plot and grid room whose power and cooling cost the least, or null.</summary>
        static SiteConfig CheapestSite(GameConfig c, GameState state, LabState lab, PlanConfig plan)
        {
            SiteConfig best = null;
            long bestPrice = 0;
            for (int i = 0; i < c.World.Sites.Count; i++)
            {
                SiteConfig site = c.World.Sites[i];
                if (!plan.SiteBlocs.Contains(site.BlocId) || !WorldQueries.SiteOpen(c, site, state.Tick)) continue;
                if (WorldQueries.PlotsTaken(state, site.Id) >= site.Plots) continue;
                if (WorldQueries.HoldingAt(lab, site.Id, HoldingKind.Owned) != null || WorldQueries.IsFrozen(lab, site.BlocId)) continue;
                if (GridRoom(c, state, site) < c.World.Halls.MinKw) continue;
                long price = (long)site.PowerPricePerKwWeek * site.CoolingPermille;
                if (best == null || price < bestPrice) { best = site; bestPrice = price; }
            }
            return best;
        }

        /// <summary>Buys every tech it can, in the order the config lists them.</summary>
        static void Research(GameConfig c, GameState state, LabState lab, Func<Command, CommandResult> apply)
        {
            bool bought = true;
            while (bought)
            {
                bought = false;
                for (int i = 0; i < c.Research.Techs.Count; i++)
                {
                    TechConfig tech = c.Research.Techs[i];
                    if (!LabQueries.CanResearch(c, lab, tech, state.Tick) || lab.ResearchPoints < tech.Cost) continue;
                    if (apply(Command.ResearchTech(lab.Id, tech.Id)).Ok) bought = true;
                }
            }
        }

        /// <summary>
        /// Starts one run at a time: the biggest line the lab can afford and finish in reasonable time,
        /// and only when the new model would clearly beat the lab's best.
        /// </summary>
        static void Train(GameConfig c, GameState state, LabState lab, PlanConfig plan, Func<Command, CommandResult> apply)
        {
            if (LabQueries.ActiveRuns(lab) > 0) return;

            int best = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status != ModelStatus.Training && lab.Models[i].Status != ModelStatus.Retired)
                    best = Math.Max(best, lab.Models[i].Capability);
            long bar = MathI.MulPermille(best, MathI.Permille + plan.MinGainPermille);

            for (int i = c.Training.Lines.Count - 1; i >= 0; i--)
            {
                ModelLineConfig line = c.Training.Lines[i];
                if (!LabQueries.LineUnlocked(c, lab, line.Id)) continue;
                // A run is how a lab earns its next dollar, so it may dip into half of the reserve.
                if (lab.Cash - EraSystem.RunPrepCost(c, line, state.Tick) < Reserve(c, state, lab, plan) / 2) continue;
                int weeks = TrainingSystem.EstimateWeeks(c, state, lab, line, plan.AlignmentSharePermille);
                if (weeks < 1 || weeks > plan.MaxRunWeeks) continue;
                if (best > 0 && TrainingSystem.Forecast(c, lab, line).Total < bar) continue;
                if (apply(Command.StartTraining(lab.Id, line.Id, plan.AlignmentSharePermille)).Ok) return;
            }
        }

        /// <summary>Puts each finished model on sale wherever the plan sells and the model is the lab's best offer.</summary>
        static void Release(GameConfig c, GameState state, LabState lab, PlanConfig plan, Func<Command, CommandResult> apply)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) continue;

                var sectors = new List<string>();
                for (int k = 0; k < plan.Sectors.Count; k++)
                {
                    SectorConfig sector = c.Sector(plan.Sectors[k]);
                    if (!LabQueries.SectorOpen(c, sector, state.Tick) || model.Capability < sector.MinCapability) continue;
                    SectorEntry entry = state.Sector(sector.Id).Entry(lab.Id);
                    if (entry != null)
                    {
                        if (entry.ModelId == model.Id) continue;
                        ModelState current = lab.Model(entry.ModelId);
                        if (LabQueries.Quality(c, sector, model, state.Tick) <= LabQueries.Quality(c, sector, current, state.Tick)) continue;
                    }
                    sectors.Add(sector.Id);
                }
                if (sectors.Count > 0) apply(Command.ReleaseModel(lab.Id, model.Id, sectors.ToArray()));
            }
        }

        /// <summary>The densest room type that seats the role and is open: later eras seat more people to the tile.</summary>
        static RoomTypeConfig RoomFor(GameConfig c, string roleId, int tick)
        {
            RoomTypeConfig best = null;
            for (int i = 0; i < c.RoomTypes.Count; i++)
            {
                RoomTypeConfig type = c.RoomTypes[i];
                if (type.SeatsRole != roleId || !EraSystem.RoomOpen(c, type, tick)) continue;
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
        static bool BuildRoom(GameConfig c, GameState state, LabState lab, PlanConfig plan, RoomTypeConfig type, int tiles, Func<Command, CommandResult> apply)
        {
            int w = type.MinWidth;
            while (w * w < tiles) w++;
            int h = Math.Max(type.MinHeight, (tiles + w - 1) / w);
            if (!CanSpend(c, state, lab, plan, type.BuildCostPerTile * w * h)) return false;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                LotConfig lot = c.Lot(lab.LotId);
                int x, y;
                if (LotGrid.FindFreeRect(lot, lab.Rooms, w, h, out x, out y))
                    return apply(Command.BuildRoom(lab.Id, type.Id, x, y, w, h)).Ok;
                if (h >= type.MinWidth && w >= type.MinHeight && LotGrid.FindFreeRect(lot, lab.Rooms, h, w, out x, out y))
                    return apply(Command.BuildRoom(lab.Id, type.Id, x, y, h, w)).Ok;

                int at = c.Lots.IndexOf(lot);
                if (at + 1 >= c.Lots.Count) return false;
                if (!CanSpend(c, state, lab, plan, c.Lots[at + 1].Price + type.BuildCostPerTile * w * h)) return false;
                if (!apply(Command.BuyLot(lab.Id)).Ok) return false;
            }
            return false;
        }
    }
}
