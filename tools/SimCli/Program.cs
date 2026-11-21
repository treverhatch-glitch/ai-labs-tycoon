using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.SimCli;

/// <summary>
/// Plays the rules without Unity. Add --era <id> to start in a later era (for example punchline).
///   simcli run   [--years 5] [--seed 1] [--player autopilot|idle] [--events]   a yearly table for every lab
///   simcli soak  [--games 50] [--years 5] [--seed 1] [--player autopilot|idle]  many seeds: who survives, who wins
///   simcli probe [--years 5] [--seed 1] [--player autopilot|idle]              "tick N digest" once a year
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        string mode = args.Length > 0 ? args[0] : "run";
        int years = int.Parse(Option(args, "--years", "5"), CultureInfo.InvariantCulture);
        long seed = long.Parse(Option(args, "--seed", "1"), CultureInfo.InvariantCulture);
        bool autopilot = Option(args, "--player", "autopilot") == "autopilot";
        bool events = Array.IndexOf(args, "--events") >= 0;

        if (mode == "soak") return Soak(args, years, seed, autopilot);
        if (mode != "run" && mode != "probe")
        {
            Console.Error.WriteLine("Usage: simcli run|soak|probe [--years N] [--seed S] [--player autopilot|idle] [--events]");
            return 2;
        }

        GameConfig config = GameConfig.FromJson(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "default-config.json")));
        Simulation sim = Simulation.NewGame(config, NewGame(args, seed));
        int weeksPerYear = config.Calendar.WeeksPerYear;
        int shown = 0;

        for (int year = 0; year < years; year++)
        {
            for (int week = 0; week < weeksPerYear; week++)
            {
                if (autopilot && GameDate.IsMonthStart(config.Calendar, sim.Tick)) sim.RunPlan(sim.Player.Id, config.Autopilot);
                sim.Step();
                if (events && mode == "run")
                    for (; shown < sim.State.Events.Count; shown++)
                        Console.WriteLine("  " + EventText.Describe(config, sim.State, sim.State.Events[shown]));
            }
            if (mode == "probe") Console.WriteLine("tick " + sim.Tick + " " + sim.Digest());
            else PrintYear(config, sim);
        }
        return 0;
    }

    /// <summary>Plays many seeds and prints how the labs fared: a quick look at balance.</summary>
    static int Soak(string[] args, int years, long firstSeed, bool autopilot)
    {
        int gamesPerPreset = int.Parse(Option(args, "--games", "50"), CultureInfo.InvariantCulture);
        string presetId = Option(args, "--preset", "");
        GameConfig config = GameConfig.FromJson(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "default-config.json")));
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(config);
        if (presetId != "")
        {
            presets = new[] { PickPreset(presets, presetId) ?? presets[0] };
            gamesPerPreset = int.Parse(Option(args, "--games", "200"), CultureInfo.InvariantCulture);
        }
        if (!autopilot)
        {
            Console.Error.WriteLine("soak ignores --player idle; rival plans still run on their own.");
        }

        BalanceSoak.Result result = BalanceSoak.Run(config, gamesPerPreset, years, firstSeed, presets);
        int total = result.TotalGames;
        Console.WriteLine($"{total} games of {years} years across {result.Presets.Count} preset(s), seeds from {firstSeed}");
        foreach (BalanceSoak.PresetResult row in result.Presets)
        {
            Console.WriteLine($"  preset {row.PresetId}: rival survived {row.RivalSurvivedGames}/{row.Games}, money pressure {row.MoneyMattersGames}/{row.Games}, max crown {(BalanceSoak.MaxCrownSharePermille(row) / 10.0):F1}%");
            foreach (KeyValuePair<string, int> crown in row.Crowns.OrderByDescending(e => e.Value))
                Console.WriteLine($"    {crown.Key,-14}{crown.Value}");
        }
        return 0;
    }

    static BalanceSoak.Preset PickPreset(BalanceSoak.Preset[] presets, string id)
    {
        for (int i = 0; i < presets.Length; i++) if (presets[i].Id == id) return presets[i];
        return null;
    }

    static NewGameOptions NewGame(string[] args, long seed)
    {
        return new NewGameOptions { Seed = seed, StartEraId = Option(args, "--era", "") };
    }

    static string Option(string[] args, string name, string fallback)
    {
        int at = Array.IndexOf(args, name);
        return at >= 0 && at + 1 < args.Length ? args[at + 1] : fallback;
    }

    static void PrintYear(GameConfig c, Simulation sim)
    {
        GameState state = sim.State;
        Console.WriteLine();
        Console.WriteLine("== End of " + (sim.Year - 1) + " (" + EraSystem.AtYear(c, sim.Year - 1).Name + ") ==");
        Console.WriteLine($"{"lab",-14}{"cash",20}{"rev/wk",15}{"cost/wk",12}{"staff",7}{"gpus",6}{"rep",5}{"techs",6}  best model");
        foreach (LabState lab in state.Labs)
        {
            ModelState best = LabQueries.BestReleased(lab);
            string model = lab.Bankrupt ? "(bankrupt)" : best == null ? "-" : $"{best.Name} cap {best.Capability} safety {best.Alignment} {LabQueries.Mood(c, lab, best, state.Tick)}";
            Console.WriteLine($"{lab.Name,-14}{lab.Cash,20}{lab.LastWeek.Revenue,15}{LabQueries.WeeklyRunningCost(c, state, lab),12}{LabQueries.TotalStaff(lab),7}{LabQueries.TotalGpus(lab),6}{lab.Reputation,5}{lab.Techs.Count,6}  {model}");
        }
        foreach (SectorState sector in state.Sectors)
        {
            if (sector.Entries.Count == 0) continue;
            var parts = new List<string>();
            foreach (SectorEntry entry in sector.Entries) parts.Add($"{state.Lab(entry.LabId).Name} {entry.SharePpm / 10000}%");
            Console.WriteLine($"  {c.Sector(sector.Id).Name}: adoption {sector.AdoptionPermille / 10}%, {string.Join(", ", parts)}");
        }
        List<GauntletEntry> crown = GauntletSystem.Latest(state, GauntletSystem.CrownBracket);
        if (crown.Count > 0) Console.WriteLine($"  {c.Gauntlet.Name}: crown to {crown[0].ModelName} ({state.Lab(crown[0].LabId).Name})");
    }
}
