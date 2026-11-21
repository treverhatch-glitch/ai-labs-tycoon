using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>One week's compute, split by how it can be used.</summary>
    public struct ComputeSplit
    {
        /// <summary>Rented and owned GPUs and training chips: what a training run draws on.</summary>
        public long General;
        /// <summary>The part of <see cref="General"/> that is the lab's own training chips.</summary>
        public long OwnChips;
        /// <summary>Inference chips, which only serve customers.</summary>
        public long Inference;

        public long Total { get { return General + Inference; } }
    }

    /// <summary>
    /// Read-only answers about a lab, worked out from the state every time they are asked.
    /// Nothing here is cached, so the state stays the single source of truth.
    /// </summary>
    public static class LabQueries
    {
        public static int StaffCount(LabState lab, string roleId)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].RoleId == roleId) n++;
            return n;
        }

        public static int StaffByFunction(GameConfig c, LabState lab, RoleFunction function)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                RoleConfig role = c.Role(lab.Staff[i].RoleId);
                if (role != null && role.Function == function) n++;
            }
            return n;
        }

        public static int TotalStaff(LabState lab) { return lab.Staff.Count; }

        public static int SeatsInRoom(GameConfig c, RoomState room)
        {
            RoomTypeConfig type = c.RoomType(room.TypeId);
            if (type == null || string.IsNullOrEmpty(type.SeatsRole) || type.TilesPerSeat < 1) return 0;
            return room.Tiles / type.TilesPerSeat;
        }

        /// <summary>Desks for a role: every person needs a seat in a room built for their job.</summary>
        public static int Seats(GameConfig c, LabState lab, string roleId)
        {
            int n = 0;
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomTypeConfig type = c.RoomType(lab.Rooms[i].TypeId);
                if (type != null && type.SeatsRole == roleId) n += SeatsInRoom(c, lab.Rooms[i]);
            }
            return n;
        }

        public static int GpuSlotsInRoom(GameConfig c, RoomState room)
        {
            RoomTypeConfig type = c.RoomType(room.TypeId);
            return type == null ? 0 : room.Tiles * type.GpuSlotsPerTile;
        }

        /// <summary>Rack space for owned GPUs. Rented GPUs live in someone else's building and need none.</summary>
        public static int GpuSlots(GameConfig c, LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Rooms.Count; i++) n += GpuSlotsInRoom(c, lab.Rooms[i]);
            return n;
        }

        /// <summary>Hardware units the lab owns at one place: -1 for the HQ's racks, else a holding's id.</summary>
        public static int OwnedUnits(LabState lab, int holdingId)
        {
            int n = 0;
            for (int i = 0; i < lab.Fleet.Count; i++) if (lab.Fleet[i].HoldingId == holdingId) n += lab.Fleet[i].Count;
            return n;
        }

        /// <summary>GPUs the lab owns at one place, not counting its chips.</summary>
        public static int OwnedGpuUnits(LabState lab, int holdingId)
        {
            int n = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
                if (lab.Fleet[i].HoldingId == holdingId && lab.Fleet[i].ChipDesignId < 0) n += lab.Fleet[i].Count;
            return n;
        }

        /// <summary>Chips the lab has on order at the foundry, not yet delivered.</summary>
        public static int ChipsOnOrder(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.ChipOrders.Count; i++) n += lab.ChipOrders[i].Count;
            return n;
        }

        /// <summary>Whether the lab knows the tech a room type needs, if it needs one.</summary>
        public static bool RoomUnlocked(LabState lab, RoomTypeConfig type)
        {
            return string.IsNullOrEmpty(type.RequiresTech) || HasTech(lab, type.RequiresTech);
        }

        /// <summary>Every hardware unit the lab owns, wherever it stands.</summary>
        public static int OwnedUnits(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Fleet.Count; i++) n += lab.Fleet[i].Count;
            return n;
        }

        /// <summary>Hardware racked at the HQ: it needs rack space and pays upkeep, and nothing else.</summary>
        public static int HqGpus(LabState lab) { return OwnedUnits(lab, HardwareBatch.Hq); }

        public static int TotalGpus(LabState lab) { return lab.RentedGpus + OwnedUnits(lab); }

        /// <summary>The newest GPU generation open at a tick. Every purchase and every rental gets it.</summary>
        public static GpuGenerationConfig CurrentGeneration(GameConfig c, int tick)
        {
            return c.Compute.CurrentAtYear(GameDate.Year(c.Calendar, tick));
        }

        /// <summary>
        /// What one GPU of a generation sells for at a tick: the current price scaled by the resale share, then by the decay
        /// once for every generation it is behind the newest. Rounded down at each step, so the number is pinned.
        /// </summary>
        public static long ResalePerUnit(GameConfig c, GpuGenerationConfig gen, int tick)
        {
            int currentTier = CurrentGeneration(c, tick).Tier;
            long value = MathI.MulPermille(gen.BuyPrice, c.Compute.ResalePermille);
            for (int t = gen.Tier; t < currentTier; t++) value = MathI.MulPermille(value, c.Compute.GenerationDecayPermille);
            return value;
        }

        public static long FleetResaleValue(GameConfig c, GameState state, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                GpuGenerationConfig gen = c.Compute.Generation(lab.Fleet[i].GpuGenId);
                if (gen != null) total += lab.Fleet[i].Count * ResalePerUnit(c, gen, state.Tick);
            }
            return total;
        }

        /// <summary>
        /// Compute the lab has this week: rented GPUs of the current generation plus every batch it owns, each at its own
        /// speed. Hardware at a site in a heatwave runs slower.
        /// </summary>
        public static long WeeklyCompute(GameConfig c, GameState state, LabState lab) { return Split(c, state, lab, false).Total; }

        /// <summary>
        /// The same compute as customers see it: hardware at a site loses the site's latency share on the way to them.
        /// Training is not served to anyone, so it never loses it.
        /// </summary>
        public static long WeeklyServingBase(GameConfig c, GameState state, LabState lab) { return Split(c, state, lab, true).Total; }

        /// <summary>
        /// The week's compute in the parts that train differently: general compute (GPUs, rented or owned, and training chips),
        /// the part of it that is the lab's own training chips, and inference chips, which only ever serve.
        /// </summary>
        public static ComputeSplit Split(GameConfig c, GameState state, LabState lab, bool forCustomers)
        {
            var split = new ComputeSplit();
            GpuGenerationConfig gen = CurrentGeneration(c, state.Tick);
            split.General = (long)lab.RentedGpus * gen.ComputePerGpuWeek;
            split.General += (long)ComputeContractSystem.ReservedGpus(lab) * gen.ComputePerGpuWeek;
            int throttle = WorldQueries.HeatwaveThrottle(c, lab);
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch batch = lab.Fleet[i];
                long part;
                bool chip = batch.ChipDesignId >= 0;
                if (chip)
                {
                    part = ChipQueries.BatchCompute(c, state, batch);
                }
                else
                {
                    GpuGenerationConfig batchGen = c.Compute.Generation(batch.GpuGenId);
                    if (batchGen == null) continue;
                    part = batch.Count * (long)batchGen.ComputePerGpuWeek;
                }
                if (batch.HoldingId != HardwareBatch.Hq)
                {
                    Holding holding = lab.Holding(batch.HoldingId);
                    SiteConfig site = holding == null ? null : c.World.Site(holding.SiteId);
                    if (site != null)
                    {
                        if (WorldQueries.InHeatwave(state, site.Id)) part = MathI.MulPermille(part, MathI.Permille - throttle);
                        if (forCustomers) part = MathI.MulPermille(part, MathI.Permille - site.LatencyPermille);
                    }
                }
                if (!chip || ChipQueries.BatchTrains(c, state, batch))
                {
                    split.General += part;
                    if (chip && batch.DesignLabId == lab.Id) split.OwnChips += part;
                }
                else
                {
                    split.Inference += part;
                }
            }
            split.General += ExpansionSystem.PassiveOrbitalGeneralCompute(c, state, lab);
            return split;
        }

        /// <summary>
        /// What goes to training when a run is active: the share of general compute, with the part from the lab's own training
        /// chips counting a little more (the models are tuned to them). Inference chips never train.
        /// </summary>
        public static long TrainingShareOf(GameConfig c, LabState lab, ComputeSplit split)
        {
            long bought = MathI.MulPermille(split.General - split.OwnChips, lab.TrainingSharePermille);
            long own = MathI.MulPermille(split.OwnChips, lab.TrainingSharePermille);
            return bought + MathI.MulPermille(own, MathI.Permille + c.Chips.InHouseTrainingBonusPermille);
        }

        /// <summary>
        /// Whether a lab may put hardware of this tier into a place in this bloc: no export rule in force names the lab's home bloc
        /// and the destination and caps the tier below it.
        /// </summary>
        public static bool ExportAllowed(GameConfig c, GameState state, LabState lab, int tier, string destBlocId)
        {
            return ExportAllowedFrom(c, state, lab.HomeBlocId, tier, destBlocId);
        }

        public static bool ExportAllowedFrom(GameConfig c, GameState state, string homeBlocId, int tier, string destBlocId)
        {
            int year = GameDate.Year(c.Calendar, state.Tick);
            for (int i = 0; i < c.World.ExportRules.Count; i++)
            {
                ExportRuleConfig rule = c.World.ExportRules[i];
                if (year < rule.FromYear || tier <= rule.MaxTier) continue;
                if (rule.HomeBlocs.Contains(homeBlocId) && rule.DestBlocs.Contains(destBlocId)) return false;
            }
            return true;
        }

        public static bool HasTech(LabState lab, string techId) { return lab.Techs.Contains(techId); }

        /// <summary>The summed bonus, in permille, of every researched tech with an effect of this kind.</summary>
        public static int TechBonus(GameConfig c, LabState lab, TechEffectKind kind)
        {
            int total = 0;
            for (int i = 0; i < lab.Techs.Count; i++)
            {
                TechConfig tech = c.Tech(lab.Techs[i]);
                if (tech == null) continue;
                for (int k = 0; k < tech.Effects.Count; k++)
                    if (tech.Effects[k].Kind == kind) total += tech.Effects[k].Amount;
            }
            if (kind == TechEffectKind.CapabilityPermille)
                total += lab.FrontierLevel * c.Endings.Frontier.CapabilityPermillePerLevel;
            return total;
        }

        /// <summary>A tech that makes the lab's alignment work count for more.</summary>
        public static bool IsSafetyTech(TechConfig tech)
        {
            for (int i = 0; i < tech.Effects.Count; i++) if (tech.Effects[i].Kind == TechEffectKind.AlignmentPermille) return true;
            return false;
        }

        /// <summary>
        /// What a tech costs this lab in research points. A safety tech that a lab publishing its safety research already has
        /// costs everybody else less.
        /// </summary>
        public static long TechCost(GameConfig c, GameState state, LabState lab, TechConfig tech)
        {
            if (!IsSafetyTech(tech)) return tech.Cost;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState other = state.Labs[i];
                if (other.Id == lab.Id || other.Controller != LabController.Plan || !HasTech(other, tech.Id)) continue;
                RivalConfig rival = c.Rival(other.RivalId);
                if (rival != null && rival.Plan.PublishesSafetyResearch) return MathI.MulPermille(tech.Cost, MathI.Permille - c.Rivalry.PublishedDiscountPermille);
            }
            return tech.Cost;
        }

        public static bool LineUnlocked(GameConfig c, LabState lab, string lineId)
        {
            ModelLineConfig line = c.Line(lineId);
            if (line == null) return false;
            if (!line.Locked) return true;
            for (int i = 0; i < lab.Techs.Count; i++)
            {
                TechConfig tech = c.Tech(lab.Techs[i]);
                if (tech == null) continue;
                for (int k = 0; k < tech.Effects.Count; k++)
                    if (tech.Effects[k].Kind == TechEffectKind.UnlockLine && tech.Effects[k].Id == lineId) return true;
            }
            return false;
        }

        /// <summary>True when the tech's era has begun, it is not known yet and every prerequisite is.</summary>
        public static bool CanResearch(GameConfig c, LabState lab, TechConfig tech, int tick)
        {
            if (HasTech(lab, tech.Id)) return false;
            if (!EraSystem.TechOpen(c, tech, tick)) return false;
            for (int i = 0; i < tech.Requires.Count; i++) if (!HasTech(lab, tech.Requires[i])) return false;
            return true;
        }

        public static int ActiveRuns(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Training) n++;
            return n;
        }

        /// <summary>The newest finished model of a line, or null. Its capability carries over into the next version.</summary>
        public static ModelState LatestOfLine(LabState lab, string lineId)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != lineId || m.Status == ModelStatus.Training) continue;
                if (best == null || m.Version > best.Version) best = m;
            }
            return best;
        }

        public static int NextVersion(LabState lab, string lineId)
        {
            int v = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].LineId == lineId && lab.Models[i].Version > v) v = lab.Models[i].Version;
            return v + 1;
        }

        /// <summary>The lab's most capable model on sale, or null.</summary>
        public static ModelState BestReleased(LabState lab)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                if (best == null || m.Capability > best.Capability) best = m;
            }
            return best;
        }

        public static bool IsGlitching(GameConfig c, ModelState model, int tick)
        {
            return model.Incidents > 0 && tick - model.LastIncidentTick < c.Safety.GlitchWeeks;
        }

        /// <summary>
        /// The creature's state: Glitching for a while after an incident, Overworked when the lab
        /// has too little compute to serve its customers, Healthy otherwise.
        /// </summary>
        public static ModelMood Mood(GameConfig c, LabState lab, ModelState model, int tick)
        {
            if (model.Status != ModelStatus.Released) return ModelMood.Healthy;
            if (IsGlitching(c, model, tick)) return ModelMood.Glitching;
            if (model.Corruption >= c.Shady.CorruptedMood) return ModelMood.Corrupted;
            if (lab.LastWeek.ServingDemand > 0 && lab.LastWeek.ServedPermille < MathI.Permille) return ModelMood.Overworked;
            return ModelMood.Healthy;
        }

        /// <summary>
        /// How good a model is for one sector: its capability, with the part of it the sector weighs
        /// by safety scaled by the safety meter, and a discount while it is glitching.
        /// </summary>
        public static long Quality(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            return DisplayQueries.Quality(c, sector, model, tick);
        }

        public static long WeeklySalaries(GameConfig c, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                RoleConfig role = c.Role(lab.Staff[i].RoleId);
                if (role != null) total += role.WeeklySalary;
            }
            return total;
        }

        public static long WeeklyRoomUpkeep(GameConfig c, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomTypeConfig type = c.RoomType(lab.Rooms[i].TypeId);
                if (type != null) total += type.UpkeepPerTileWeek * lab.Rooms[i].Tiles;
            }
            return total + WorldQueries.WeeklyHallUpkeep(c, lab);
        }

        public static long WeeklyGpuRent(GameConfig c, GameState state, LabState lab)
        {
            GpuGenerationConfig gen = CurrentGeneration(c, state.Tick);
            long rent = gen.RentPerGpuWeek * lab.RentedGpus;
            return rent + ComputeContractSystem.WeeklyReservedRent(c, state, lab) + ChipQueries.WeeklyCloudChipRent(c, state, lab);
        }

        /// <summary>Upkeep of the hardware racked at the HQ, by each batch's own generation. Hardware at a site pays for power instead.</summary>
        public static long WeeklyGpuUpkeep(GameConfig c, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch batch = lab.Fleet[i];
                GpuGenerationConfig gen = c.Compute.Generation(batch.GpuGenId);
                if (batch.HoldingId == HardwareBatch.Hq && gen != null) total += batch.Count * gen.UpkeepPerGpuWeek;
            }
            return total;
        }

        /// <summary>What the lab pays every week just to stand still.</summary>
        public static long WeeklyRunningCost(GameConfig c, GameState state, LabState lab)
        {
            return WeeklySalaries(c, lab) + WeeklyRoomUpkeep(c, lab) + WeeklyGpuRent(c, state, lab) + WeeklyGpuUpkeep(c, lab) + WorldQueries.WeeklyPowerBill(c, state, lab);
        }

        public static bool SectorOpen(GameConfig c, SectorConfig sector, int tick)
        {
            return GameDate.Year(c.Calendar, tick) >= sector.OpensYear;
        }
    }
}
