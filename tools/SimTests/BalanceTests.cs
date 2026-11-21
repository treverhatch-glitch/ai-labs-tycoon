using System;
using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.SimTests.Support;
using Xunit;
using Xunit.Abstractions;

namespace AiLabs.SimTests;

public class BalanceTests
{
    readonly ITestOutputHelper _out;

    public BalanceTests(ITestOutputHelper output) { _out = output; }

    static int GamesPerPreset()
    {
        string env = Environment.GetEnvironmentVariable("BALANCE_SOAK_GAMES_PER_PRESET");
        if (!string.IsNullOrEmpty(env) && int.TryParse(env, out int n) && n > 0) return n;
        return 24;
    }

    [Fact]
    public void Eight_year_soak_keeps_crowns_fair_rivals_alive_and_money_pressured()
    {
        var c = TestData.Config();
        int games = GamesPerPreset();
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(c);
        BalanceSoak.Result result = BalanceSoak.Run(c, games, years: 8, firstSeed: 1000, presets);
        const int MaxWinPermille = 600;
        const int RivalSurvivePermille = 950;
        const int MoneyMattersPermille = 850;

        foreach (BalanceSoak.PresetResult row in result.Presets)
        {
            int maxCrowns = BalanceSoak.MaxCrownSharePermille(row);
            _out.WriteLine(row.PresetId + ": max crown share " + (maxCrowns / 10.0) + "%, rival games " + row.RivalSurvivedGames + "/" + row.Games
                + ", money pressure " + row.MoneyMattersGames + "/" + row.Games);
            foreach (var crown in row.Crowns.OrderByDescending(e => e.Value))
                _out.WriteLine("  " + crown.Key.PadRight(14) + crown.Value);

            Assert.True(maxCrowns <= MaxWinPermille, row.PresetId + ": one lab won more than 60% of crowns (" + maxCrowns + " permille)");
            Assert.True(row.RivalSurvivedGames * MathI.Permille >= row.Games * RivalSurvivePermille,
                row.PresetId + ": a rival survived in only " + row.RivalSurvivedGames + " of " + row.Games + " games");
            Assert.True(row.MoneyMattersGames * MathI.Permille >= row.Games * MoneyMattersPermille,
                row.PresetId + ": year-eight budget pressure showed in only " + row.MoneyMattersGames + " of " + row.Games + " games");
        }
    }

    [Fact]
    [Trait("Category", "BalanceScale")]
    public void Two_hundred_games_per_preset_soak()
    {
        string env = Environment.GetEnvironmentVariable("BALANCE_SOAK_FULL");
        if (env != "1" && env != "true") return;

        var c = TestData.Config();
        BalanceSoak.Result result = BalanceSoak.Run(c, gamesPerPreset: 200, years: 8, firstSeed: 1, BalanceSoak.DefaultPresets(c));
        foreach (BalanceSoak.PresetResult row in result.Presets)
        {
            Assert.True(BalanceSoak.MaxCrownSharePermille(row) <= 600, row.PresetId + " exceeded 60% crown share");
            Assert.True(row.RivalSurvivedGames >= row.Games * 95 / 100, row.PresetId + " rival survival");
            Assert.True(row.MoneyMattersGames >= row.Games * 85 / 100, row.PresetId + " money matters");
        }
    }
}