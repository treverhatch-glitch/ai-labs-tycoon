using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SharesRegulatorsLaterTests
{
    [Fact]
    public void Rivals_start_with_public_float_from_config()
    {
        var g = Game.New();
        LabState rival = g.Rival("openaye");
        Assert.Equal(g.C.Shares.RivalStartPublicFloatPpm, rival.PublicFloatPpm);
    }

    [Fact]
    public void The_player_public_float_tracks_founder_dilution()
    {
        var g = Game.Solo();
        Assert.Equal(0, ShareSystem.PublicFloatPpm(g.C, g.Me));
        g.Me.FounderSharePermille = 700;
        Assert.Equal(300_000, ShareSystem.PublicFloatPpm(g.C, g.Me));
    }

    [Fact]
    public void A_hot_bubble_grows_rival_float_and_feeds_the_meter()
    {
        var g = Game.New(tweak: c =>
        {
            c.Shares.IpoWhenBubbleAtPermille = 200;
            c.Shares.IpoStepPpm = 50_000;
            c.Shares.IpoBubbleBoostPermille = 30;
        });
        LabState rival = g.Rival("openaye");
        int floatBefore = rival.PublicFloatPpm;
        g.State.BubblePermille = 400;
        int bubbleBefore = g.State.BubblePermille;
        ShareSystem.RunMonth(g.C, g.State);
        Assert.Equal(floatBefore + 50_000, rival.PublicFloatPpm);
        Assert.True(g.State.BubblePermille >= bubbleBefore + 30);
    }

    [Fact]
    public void Rival_float_stops_at_the_configured_cap()
    {
        var g = Game.New(tweak: c =>
        {
            c.Shares.IpoWhenBubbleAtPermille = 100;
            c.Shares.RivalMaxPublicFloatPpm = 150_000;
            c.Shares.IpoStepPpm = 40_000;
        });
        LabState rival = g.Rival("openaye");
        rival.PublicFloatPpm = 130_000;
        g.State.BubblePermille = 800;
        ShareSystem.RunMonth(g.C, g.State);
        Assert.Equal(150_000, rival.PublicFloatPpm);
    }

    [Fact]
    public void A_cold_bubble_skips_rival_ipos()
    {
        var g = Game.New();
        LabState rival = g.Rival("openaye");
        int before = rival.PublicFloatPpm;
        g.State.BubblePermille = 50;
        ShareSystem.RunMonth(g.C, g.State);
        Assert.Equal(before, rival.PublicFloatPpm);
    }

    [Fact]
    public void Regulators_add_scrutiny_when_the_bubble_is_hot_and_the_lab_is_big()
    {
        var g = Game.Solo(tweak: c => c.Regulators.BubbleScrutinyAtPermille = 400);
        g.State.BubblePermille = 600;
        g.Me.Ledger.ThisMonth.Revenue = 900_000;
        for (int i = 0; i < g.State.Labs.Count; i++) g.State.Labs[i].Ledger.ThisMonth.Revenue = i == 0 ? 900_000 : 1;
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        RegulatorSystem.RunMonth(g.C, g.State);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention > att);
    }

    [Fact]
    public void Small_labs_skip_bubble_regulators()
    {
        var g = Game.New(tweak: c => c.Regulators.ScrutinyRevenuePpm = 900_000);
        g.State.BubblePermille = 900;
        for (int i = 0; i < g.State.Labs.Count; i++) g.State.Labs[i].Ledger.ThisMonth.Revenue = 100;
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        RegulatorSystem.RunMonth(g.C, g.State);
        Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Monopoly_regulators_raise_attention_in_every_bloc()
    {
        var g = Game.Solo(tweak: c => c.Regulators.MonopolyRegulatorAttention = 80);
        int us = g.Me.Bloc("us").Attention;
        int eu = g.Me.Bloc("eu").Attention;
        RegulatorSystem.OnMonopoly(g.C, g.State, g.Me);
        Assert.Equal(us + 80, g.Me.Bloc("us").Attention);
        Assert.Equal(eu + 80, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Monopoly_ending_stacks_regulator_scrutiny_on_top_of_the_antitrust_bump()
    {
        var g = Game.Solo(seed: 40, tweak: c =>
        {
            c.Endings.MonopolyRevenuePpm = 400_000;
            c.Endings.MonopolyComputePpm = 400_000;
            c.Regulators.MonopolyRegulatorAttention = 100;
        });
        for (int y = 0; y < 3; y++)
        {
            g.State.Gauntlets.Add(new GauntletRecord { Year = 1995 + y, CrownLabId = g.Id });
            g.Me.Ledger.History.Add(new MonthRecord { Year = 1995 + y, Month = 12, Totals = { Revenue = 9_000_000 } });
            for (int i = 0; i < g.State.Labs.Count; i++) g.State.Labs[i].LastWeek.Compute = i == 0 ? 9000 : 100;
        }
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1998) + g.C.Calendar.WeeksPerYear - 1;
        int home = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        EndingSystem.TryMonopoly(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == "monopoly");
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention >= home + g.C.Endings.MonopolyAttention + g.C.Regulators.MonopolyRegulatorAttention);
    }

    [Fact]
    public void Schema_13_migration_adds_public_float_ppm()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs) Assert.Equal(0, lab.PublicFloatPpm);
    }

    [Fact]
    public void Public_float_survives_save()
    {
        var g = Game.New();
        g.Rival("openaye").PublicFloatPpm = 250_000;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        LabState again = Simulation.Load(g.C, g.Sim.Save()).State.Labs.First(l => l.RivalId == "openaye");
        Assert.Equal(250_000, again.PublicFloatPpm);
    }

    [Fact]
    public void Validator_rejects_an_inverted_shares_cap()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Shares.RivalMaxPublicFloatPpm = 10_000;
        c.Shares.RivalStartPublicFloatPpm = 200_000;
        var problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("public float"));
    }
}