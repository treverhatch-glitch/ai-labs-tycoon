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
            lab.ResearchPoints += gained;
            lab.LastWeek.ResearchGained = gained;
        }

        /// <summary>How much training work a unit of compute does: techs plus engineers, who keep the runs from crashing.</summary>
        public static int SpeedPermille(GameConfig c, GameState state, LabState lab)
        {
            long engineers = (long)LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering) * c.Training.EngineerBonusPermille;
            return MathI.Permille + LabQueries.TechBonus(c, lab, TechEffectKind.TrainingSpeedPermille)
                + (int)MathI.Min(engineers, c.Training.EngineerBonusCapPermille)
                + ExpansionSystem.TrainingSpeedBonusPermille(c, state, lab);
        }

        /// <summary>How much alignment work a unit of compute does: techs plus safety staff.</summary>
        public static int AlignmentWorkPermille(GameConfig c, LabState lab)
        {
            long staff = (long)LabQueries.StaffByFunction(c, lab, RoleFunction.Safety) * c.Safety.SafetyStaffBonusPermille;
            return MathI.Permille + LabQueries.TechBonus(c, lab, TechEffectKind.AlignmentPermille)
                + (int)MathI.Min(staff, c.Safety.SafetyStaffBonusCapPermille);
        }

        /// <summary>This week's compute going to training. It is zero when nothing is training: idle GPUs serve customers.</summary>
        public static long TrainingCompute(GameConfig c, GameState state, LabState lab)
        {
            if (LabQueries.ActiveRuns(lab) == 0) return 0;
            return LabQueries.TrainingShareOf(c, lab, LabQueries.Split(c, state, lab, false));
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
            long training = LabQueries.TrainingShareOf(c, lab, LabQueries.Split(c, state, lab, false)) / runs;
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
            long each = training / runs;
            long spare = training - each * runs;

            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Training) continue;

                // The oldest run gets what does not divide evenly, so no compute is lost.
                long allotted = each + spare;
                spare = 0;

                long effective = MathI.MulPermille(allotted, speed);
                long alignmentPart = MathI.MulPermille(effective, model.AlignmentSharePermille);
                model.ProgressCompute += effective - alignmentPart;
                model.AlignmentCompute += MathI.MulPermille(alignmentPart, alignmentWork);

                if (MaybeFail(c, state, lab, model)) continue;
                if (model.ProgressCompute >= model.RequiredCompute) Finish(c, state, lab, model);
            }
        }

        /// <summary>Weekly chance a run collapses after early progress. Engineers and alignment share cut the odds.</summary>
        public static int WeeklyFailPermille(GameConfig c, LabState lab, ModelState model)
        {
            DataSourceConfig source = c.Training.DataSource(model.DataSourceId);
            int risk = c.Training.BaseFailPermille + (source == null ? 0 : source.WeeklyFailRiskPermille);
            int engineers = LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering);
            risk -= engineers * c.Training.FailEngineerCutPermille;
            risk -= model.AlignmentSharePermille / 100 * c.Training.FailAlignmentCutPermille;
            return (int)MathI.Clamp(risk, 0, 950);
        }

        static bool MaybeFail(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (model.RequiredCompute <= 0) return false;
            long progress = model.ProgressCompute * MathI.Permille;
            if (progress < model.RequiredCompute * c.Training.FailAfterProgressPermille) return false;
            int risk = WeeklyFailPermille(c, lab, model);
            if (risk <= 0) return false;
            int roll = state.Rng.TrainingCollapse.Range(0, MathI.Permille - 1);
            if (roll >= risk) return false;
            FailRun(c, state, lab, model);
            return true;
        }

        public static void FailRun(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.Status = ModelStatus.Failed;
            Events.Add(state, EventKind.TrainingFailed, lab.Id, model.Id, model.ProgressCompute, model.Name);
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
                if (source.FinishCorruption > 0)
                    model.Corruption = (int)MathI.Min(MathI.Permille, model.Corruption + source.FinishCorruption);
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
