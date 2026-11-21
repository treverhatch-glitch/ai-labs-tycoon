using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>What a new version of a line would aim for today, split into the parts a screen can show.</summary>
    public struct CapabilityForecast
    {
        public int Base;
        public int Tech;
        public int Carryover;

        public int Total { get { return Base + Tech + Carryover; } }
    }

    /// <summary>Research points, the weekly split of compute, and training runs.</summary>
    public static class TrainingSystem
    {
        public static void RunResearch(GameConfig c, LabState lab, int tick)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId)) return;
            long perResearcher = MathI.MulPermille(c.Research.PointsPerResearcherWeek, EraSystem.AtTick(c, tick).ResearchPermille);
            long gained = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember person = lab.Staff[i];
                RoleConfig role = c.Role(person.RoleId);
                if (role == null || role.Function != RoleFunction.Research) continue;
                StaffTraitConfig trait = c.StaffPeople.Trait(person.TraitId);
                int bonus = trait == null ? 0 : trait.ResearchPermille;
                gained += MathI.MulPermille(perResearcher, MathI.Permille + bonus);
            }
            int moraleBonus = StaffSystem.MoraleResearchPermille(c, lab);
            if (moraleBonus != 0 && gained > 0)
                gained = MathI.MulPermille(gained, MathI.Permille + moraleBonus);
            lab.ResearchPoints += gained;
            lab.LastWeek.ResearchGained = gained;
        }

        /// <summary>How much training work a unit of compute does: techs plus engineers, who keep the runs from crashing.</summary>
        public static int SpeedPermille(GameConfig c, GameState state, LabState lab)
        {
            long engineers = (long)LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering) * c.Training.EngineerBonusPermille;
            int total = MathI.Permille + LabQueries.TechBonus(c, lab, TechEffectKind.TrainingSpeedPermille)
                + (int)MathI.Min(engineers, c.Training.EngineerBonusCapPermille)
                + ExpansionSystem.TrainingSpeedBonusPermille(c, state, lab)
                + StaffSystem.MoraleTrainingPermille(c, lab)
                + DeviceWorkshopSystem.TrainingSpeedBonusPermille(c, state, lab);
            FrontierConfig frontier = c.Endings?.Frontier;
            if (frontier != null && frontier.TrainingSpeedPermillePerLevel > 0 && lab.FrontierLevel > 0)
                total += frontier.TrainingSpeedPermillePerLevel * lab.FrontierLevel;
            if (c.Safety.OverworkedTrainingSpeedPermille > 0
                && c.Safety.OverworkedTrainingSpeedPermille < MathI.Permille
                && lab.LastWeek.ServingDemand > 0
                && lab.LastWeek.ServedPermille < MathI.Permille)
                total = (int)MathI.MulPermille(total, c.Safety.OverworkedTrainingSpeedPermille);
            if (c.Safety.GlitchingTrainingSpeedPermille > 0
                && c.Safety.GlitchingTrainingSpeedPermille < MathI.Permille
                && LabHasGlitchingReleased(c, lab, state.Tick))
                total = (int)MathI.MulPermille(total, c.Safety.GlitchingTrainingSpeedPermille);
            return total;
        }

        /// <summary>How much alignment work a unit of compute does: techs plus safety staff.</summary>
        public static int AlignmentWorkPermille(GameConfig c, LabState lab)
        {
            long staff = (long)LabQueries.StaffByFunction(c, lab, RoleFunction.Safety) * c.Safety.SafetyStaffBonusPermille;
            int total = MathI.Permille + LabQueries.TechBonus(c, lab, TechEffectKind.AlignmentPermille)
                + (int)MathI.Min(staff, c.Safety.SafetyStaffBonusCapPermille);
            total += StaffSystem.MoraleAlignmentPermille(c, lab);
            if (c.Safety.OverworkedAlignmentPermille > 0
                && c.Safety.OverworkedAlignmentPermille < MathI.Permille
                && lab.LastWeek.ServingDemand > 0
                && lab.LastWeek.ServedPermille < MathI.Permille)
                total = (int)MathI.MulPermille(total, c.Safety.OverworkedAlignmentPermille);
            return total;
        }

        /// <summary>This week's compute going to training. It is zero when nothing is training: idle GPUs serve customers.</summary>
        public static long TrainingCompute(GameConfig c, GameState state, LabState lab)
        {
            if (LabQueries.ActiveRuns(lab) == 0) return 0;
            return LabQueries.TrainingShareOf(c, state, lab, LabQueries.Split(c, state, lab, false));
        }

        public static CapabilityForecast Forecast(GameConfig c, LabState lab, ModelLineConfig line)
        {
            var f = new CapabilityForecast();
            f.Base = line.BaseCapability;
            f.Tech = (int)MathI.MulPermille(line.BaseCapability, LabQueries.TechBonus(c, lab, TechEffectKind.CapabilityPermille));
            ModelState previous = LabQueries.LatestOfLine(lab, line.Id);
            f.Carryover = previous == null ? 0 : (int)MathI.MulPermille(previous.Capability, c.Training.CarryoverPermille);
            return f;
        }

        /// <summary>
        /// Weeks a new run of this line would take if it started at this tick with today's GPUs, staff and techs,
        /// or -1 when it would never finish.
        /// </summary>
        public static int EstimateWeeks(GameConfig c, GameState state, LabState lab, ModelLineConfig line, int alignmentSharePermille)
        {
            int runs = LabQueries.ActiveRuns(lab) + 1;
            long training = LabQueries.TrainingShareOf(c, state, lab, LabQueries.Split(c, state, lab, false)) / runs;
            long effective = MathI.MulPermille(training, SpeedPermille(c, state, lab));
            long progress = effective - MathI.MulPermille(effective, alignmentSharePermille);
            if (progress <= 0) return -1;
            return (int)((EraSystem.RunCompute(c, line, state.Tick) + progress - 1) / progress);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            long total = LabQueries.WeeklyCompute(c, state, lab);
            long training = TrainingCompute(c, state, lab);
            lab.LastWeek.Compute = total;
            lab.LastWeek.TrainingCompute = training;
            ComputeSplit toCustomers = LabQueries.Split(c, state, lab, true);
            lab.LastWeek.ServingCompute = toCustomers.General - MathI.MulPermille(toCustomers.General, training == 0 ? 0 : lab.TrainingSharePermille) + toCustomers.Inference;

            int runs = LabQueries.ActiveRuns(lab);
            if (runs == 0) return;
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId)) return;

            int speed = SpeedPermille(c, state, lab);
            int alignmentWork = AlignmentWorkPermille(c, lab);
            SyntheticDataLabConfig syntheticCfg = c.Training.SyntheticDataLab;
            long each = training / runs;
            long spare = training - each * runs;

            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Training) continue;

                // The oldest run gets what does not divide evenly, so no compute is lost.
                long allotted = each + spare;
                spare = 0;

                int runSpeed = speed;
                if (syntheticCfg != null && syntheticCfg.TrainingSpeedBonusPermille > 0
                    && SyntheticDataLabSystem.Active(c, state, lab, model.DataSourceId))
                    runSpeed += syntheticCfg.TrainingSpeedBonusPermille;
                long effective = MathI.MulPermille(allotted, runSpeed);
                long alignmentPart = MathI.MulPermille(effective, model.AlignmentSharePermille);
                model.ProgressCompute += effective - alignmentPart;
                model.AlignmentCompute += MathI.MulPermille(alignmentPart, alignmentWork);

                if (MaybeFail(c, state, lab, model)) continue;
                if (model.ProgressCompute >= model.RequiredCompute) Finish(c, state, lab, model);
            }
        }

        public static bool LabHasGlitchingReleased(GameConfig c, LabState lab, int tick)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                if (LabQueries.IsGlitching(c, m, tick)) return true;
            }
            return false;
        }

        /// <summary>Weekly chance a run collapses after early progress. Engineers and alignment share cut the odds.</summary>
        public static int WeeklyFailPermille(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return TrainingQueries.WeeklyFailParts(c, state, lab, model).Effective;
        }

        /// <summary>Player runs use <see cref="RngSet.TrainingCollapse"/>; plan-run rivals use <see cref="RngSet.Rivals"/> so collapse never interferes with player dice.</summary>
        public static Rng CollapseRng(GameState state, LabState lab)
        {
            return lab.Controller == LabController.Plan ? state.Rng.Rivals : state.Rng.TrainingCollapse;
        }

        static bool MaybeFail(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (model.RequiredCompute <= 0) return false;
            long progress = model.ProgressCompute * MathI.Permille;
            if (progress < model.RequiredCompute * c.Training.FailAfterProgressPermille) return false;
            int risk = WeeklyFailPermille(c, state, lab, model);
            if (risk <= 0) return false;
            int roll = CollapseRng(state, lab).Range(0, MathI.Permille - 1);
            if (roll >= risk) return false;
            FailRun(c, state, lab, model);
            return true;
        }

        public static void FailRun(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.Status = ModelStatus.Failed;
            Events.Add(state, EventKind.TrainingFailed, lab.Id, model.Id, model.ProgressCompute, model.Name);
        }

        /// <summary>Plan-run labs drop failed runs so the planner can retry; the player uses <see cref="CommandKind.ScrapFailedRun"/>.</summary>
        public static void ClearFailedRunsForPlans(LabState lab)
        {
            if (lab.Controller != LabController.Plan) return;
            for (int i = lab.Models.Count - 1; i >= 0; i--)
                if (lab.Models[i].Status == ModelStatus.Failed) lab.Models.RemoveAt(i);
        }

        static void Finish(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            ModelLineConfig line = c.Line(model.LineId);
            CapabilityForecast f = Forecast(c, lab, line);
            int luck = c.Training.LuckPermille > 0 ? state.Rng.Training.Range(-c.Training.LuckPermille, c.Training.LuckPermille) : 0;

            model.CapBase = f.Base;
            model.CapTech = f.Tech;
            model.CapCarryover = f.Carryover;
            model.CapLuck = (int)MathI.MulPermille(f.Total, luck);
            if (f.Total + model.CapLuck < 1) model.CapLuck = 1 - f.Total;
            model.Capability = f.Total + model.CapLuck;
            DataSourceConfig source = c.Training.DataSource(model.DataSourceId);
            if (source != null && source.CapabilityBonusPermille != 0)
                model.Capability = (int)MathI.MulPermille(model.Capability, MathI.Permille + source.CapabilityBonusPermille);
            if (model.DistillSourceLabId >= 0 && c.Training.DistillFinishCapabilityPermille > 0)
                model.Capability = (int)MathI.MulPermille(model.Capability, MathI.Permille + c.Training.DistillFinishCapabilityPermille);

            int expansionCap = ExpansionSystem.CapabilityBonusPermille(c, state, lab, model.LineId);
            if (expansionCap > 0)
            {
                int before = model.Capability;
                model.Capability = (int)MathI.MulPermille(model.Capability, MathI.Permille + expansionCap);
                model.CapExpansion = model.Capability - before;
            }

            model.Alignment = AlignmentFor(c, model.RequiredCompute, model.AlignmentCompute);
            model.DisplayedAlignment = model.Alignment;
            ModelState previous = LabQueries.LatestOfLine(lab, model.LineId);
            if (previous != null && previous.Id != model.Id)
                model.Corruption = (int)MathI.MulPermille(previous.Corruption, c.Shady.CarryCorruptionPermille);
            if (source != null)
            {
                int corruption = SyntheticDataLabSystem.EffectiveFinishCorruption(c, state, lab, source);
                if (corruption > 0)
                    model.Corruption = (int)MathI.Min(MathI.Permille, model.Corruption + corruption);
                if (source.FinishHeat > 0)
                    lab.Heat = (int)MathI.Min(MathI.Permille, lab.Heat + source.FinishHeat);
                if (source.HomeAttentionPermille > 0)
                {
                    BlocStanding home = lab.Bloc(lab.HomeBlocId);
                    if (home != null)
                    {
                        int before = home.Attention;
                        home.Attention = (int)MathI.Min(MathI.Permille, before + source.HomeAttentionPermille);
                        if (home.Attention > before)
                            Events.Add(state, EventKind.AttentionRose, lab.Id, -1, home.Attention - before, lab.HomeBlocId);
                    }
                }
            }
            model.ProgressCompute = model.RequiredCompute;
            model.Status = ModelStatus.Trained;
            model.TrainedTick = state.Tick;
            Events.Add(state, EventKind.TrainingFinished, lab.Id, model.Id, model.Capability, model.Name);
        }

        /// <summary>The safety meter a run earns: its alignment work against what a full meter needs for a run this size.</summary>
        public static int AlignmentFor(GameConfig c, long requiredCompute, long alignmentCompute)
        {
            long full = MathI.Max(1, MathI.MulPermille(requiredCompute, c.Safety.FullAlignmentComputePermille));
            return (int)MathI.Min(MathI.Permille, alignmentCompute * MathI.Permille / full);
        }
    }
}
