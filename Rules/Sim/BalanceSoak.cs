using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>Headless multi-game soak for bot balance: crowns, rival survival, and year-eight budget pressure.</summary>
    public static class BalanceSoak
    {
        public sealed class Preset
        {
            public string Id;
            public string Label;
            public PlanConfig Plan;
        }

        public sealed class Result
        {
            public int GamesPerPreset;
            public int Years;
            public long FirstSeed;
            public readonly List<PresetResult> Presets = new List<PresetResult>();

            public int TotalGames { get { int n = 0; for (int i = 0; i < Presets.Count; i++) n += Presets[i].Games; return n; } }
        }

        public sealed class PresetResult
        {
            public string PresetId;
            public int Games;
            public readonly Dictionary<string, int> Crowns = new Dictionary<string, int>();
            public int RivalSurvivedGames;
            public int MoneyMattersGames;
        }

        /// <summary>The three player plans the balance test cycles: balanced autopilot, alignment-heavy, compute-heavy.</summary>
        public static Preset[] DefaultPresets(GameConfig c)
        {
            var careful = ClonePlan(c.Autopilot);
            careful.TrainingSharePermille = 680;
            careful.AlignmentSharePermille = 280;
            careful.ChipProgramYear = 0;

            var hungry = ClonePlan(c.Autopilot);
            hungry.TrainingSharePermille = 780;
            hungry.AlignmentSharePermille = 120;

            return new[]
            {
                new Preset { Id = "balanced", Label = "Balanced autopilot", Plan = c.Autopilot },
                new Preset { Id = "careful", Label = "Alignment-first", Plan = careful },
                new Preset { Id = "hungry", Label = "Compute-first", Plan = hungry },
            };
        }

        public static Result Run(GameConfig c, int gamesPerPreset, int years, long firstSeed, Preset[] presets)
        {
            if (presets == null || presets.Length == 0) presets = DefaultPresets(c);
            var result = new Result { GamesPerPreset = gamesPerPreset, Years = years, FirstSeed = firstSeed };
            int weekTotal = years * c.Calendar.WeeksPerYear;
            int yearEightStart = (years - 1) * c.Calendar.WeeksPerYear;

            for (int p = 0; p < presets.Length; p++)
            {
                Preset preset = presets[p];
                var row = new PresetResult { PresetId = preset.Id, Games = gamesPerPreset };
                result.Presets.Add(row);
                for (int g = 0; g < gamesPerPreset; g++)
                {
                    long seed = firstSeed + (long)p * gamesPerPreset * 997 + g;
                    Simulation sim = Simulation.NewGame(c, new NewGameOptions { Seed = seed });
                    bool rivalAlive = false;
                    bool moneyMatters = false;
                    for (int week = 0; week < weekTotal; week++)
                    {
                        if (GameDate.IsMonthStart(c.Calendar, sim.Tick)) sim.RunPlan(sim.Player.Id, preset.Plan);
                        sim.Step();
                        if (week < yearEightStart) continue;
                        for (int i = 0; i < sim.State.Labs.Count; i++)
                        {
                            LabState lab = sim.State.Labs[i];
                            if (lab.Bankrupt) continue;
                            long revenue = lab.LastWeek.Revenue;
                            long cost = LabQueries.WeeklyRunningCost(c, sim.State, lab);
                            if (lab.WeeksInDebt > 0) moneyMatters = true;
                            else if (revenue > 0 && cost * 2 > revenue * 3) moneyMatters = true;
                            else if (cost > 0 && lab.Cash < cost * 156) moneyMatters = true;
                        }
                    }
                    for (int i = 0; i < sim.State.Labs.Count; i++)
                    {
                        LabState lab = sim.State.Labs[i];
                        if (lab.Controller == LabController.Plan && !lab.Bankrupt) rivalAlive = true;
                    }
                    if (rivalAlive) row.RivalSurvivedGames++;
                    if (moneyMatters) row.MoneyMattersGames++;

                    List<GauntletEntry> crown = GauntletSystem.Latest(sim.State, GauntletSystem.CrownBracket);
                    if (crown.Count > 0)
                    {
                        LabState winner = sim.State.Lab(crown[0].LabId);
                        if (winner != null)
                        {
                            string name = winner.Name;
                            row.Crowns[name] = row.Crowns.TryGetValue(name, out int n) ? n + 1 : 1;
                        }
                    }
                }
            }
            return result;
        }

        public static int MaxCrownSharePermille(PresetResult row)
        {
            if (row.Games == 0) return 0;
            int max = 0;
            foreach (KeyValuePair<string, int> entry in row.Crowns)
                max = Math.Max(max, entry.Value * MathI.Permille / row.Games);
            return max;
        }

        static PlanConfig ClonePlan(PlanConfig plan)
        {
            var copy = new PlanConfig();
            copy.ShadyAppetite = plan.ShadyAppetite;
            copy.StartDelayWeeks = plan.StartDelayWeeks;
            copy.StartJitterWeeks = plan.StartJitterWeeks;
            copy.StaffGrowthPermillePerYear = plan.StaffGrowthPermillePerYear;
            copy.Gpus = plan.Gpus;
            copy.GpuGrowthPermillePerYear = plan.GpuGrowthPermillePerYear;
            copy.WealthGrowthPermillePerMillion = plan.WealthGrowthPermillePerMillion;
            copy.OwnGpus = plan.OwnGpus;
            copy.TrainingSharePermille = plan.TrainingSharePermille;
            copy.AlignmentSharePermille = plan.AlignmentSharePermille;
            copy.ReserveWeeks = plan.ReserveWeeks;
            copy.MaxRunWeeks = plan.MaxRunWeeks;
            copy.MinGainPermille = plan.MinGainPermille;
            copy.Sectors = new List<string>(plan.Sectors);
            copy.SiteAppetitePermille = plan.SiteAppetitePermille;
            copy.SiteBlocs = new List<string>(plan.SiteBlocs);
            copy.MinReleaseAlignment = plan.MinReleaseAlignment;
            copy.PublishesSafetyResearch = plan.PublishesSafetyResearch;
            copy.DramaPermillePerYear = plan.DramaPermillePerYear;
            copy.ChipProgramYear = plan.ChipProgramYear;
            copy.BuysListedChips = plan.BuysListedChips;
            copy.UsesCloudDeals = plan.UsesCloudDeals;
            copy.ListsChipsMarkupPermille = plan.ListsChipsMarkupPermille;
            copy.Staff = new List<PlanStaffConfig>();
            for (int i = 0; i < plan.Staff.Count; i++)
            {
                var s = new PlanStaffConfig();
                s.RoleId = plan.Staff[i].RoleId;
                s.Count = plan.Staff[i].Count;
                s.FromYear = plan.Staff[i].FromYear;
                copy.Staff.Add(s);
            }
            return copy;
        }
    }
}