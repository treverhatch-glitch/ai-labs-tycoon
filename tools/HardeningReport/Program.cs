using System;
using System.Globalization;
using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;

/// <summary>Headless helper: run BalanceSoak presets and print a short summary for economy hardening reports.</summary>
internal static class Program
{
    static int Main(string[] args)
    {
        int games = 24;
        int years = 8;
        long firstSeed = 1000;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--games" && i + 1 < args.Length && int.TryParse(args[++i], out int n) && n > 0) games = n;
            if (args[i] == "--years" && i + 1 < args.Length && int.TryParse(args[++i], out int y) && y > 0) years = y;
            if (args[i] == "--seed" && i + 1 < args.Length) firstSeed = long.Parse(args[++i], CultureInfo.InvariantCulture);
        }

        string configPath = Path.Combine(AppContext.BaseDirectory, "Data", "default-config.json");
        GameConfig c = GameConfig.FromJson(File.ReadAllText(configPath));
        BalanceSoak.Result result = BalanceSoak.Run(c, games, years, firstSeed, BalanceSoak.DefaultPresets(c));
        foreach (BalanceSoak.PresetResult row in result.Presets)
        {
            int maxCrowns = BalanceSoak.MaxCrownSharePermille(row);
            Console.WriteLine(row.PresetId + ": crown max " + (maxCrowns / 10.0).ToString("F1") + "% rivals " + row.RivalSurvivedGames + "/" + row.Games
                + " money " + row.MoneyMattersGames + "/" + row.Games);
        }
        return 0;
    }
}