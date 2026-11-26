using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT #15–#18 explain: control stale, research leads, model repair, rival log + balance (batch 375).</summary>
public class HostExplainNext375LaterTests
{
    static Game WithRival()
        => new Game(Simulation.NewGame(TestData.Config(c => c.Rivalry.LogMax = 64), new NewGameOptions { Seed = 375, RivalIds = new List<string> { "openaye" } }));

    [Fact]
    public void Control_stale_report_tracks_bonus_permille()
    {
        var g = Game.Solo().WithStarterLab();
        ControlStale.Report report = ControlStale.Of(g.C, g.Me, g.State.Tick);
        Assert.InRange(report.BonusPermille, 0, 2000);
        Assert.InRange(report.FreshBonusPermille, 0, 2000);
        Assert.True(g.C.Control.MaxLossPermille > 0);
    }

    [Fact]
    public void Research_lead_rows_start_empty_on_fresh_lab()
    {
        var g = Game.Solo().WithStarterLab();
        List<ResearchLeads.Row> rows = ResearchLeads.Rows(g.C, g.Me, g.State.Tick);
        Assert.NotNull(rows);
        Assert.Empty(rows);
        Assert.True(g.C.Leads.MaxOpen >= 1);
    }

    [Fact]
    public void Model_repair_config_and_glitch_weeks_expose_host_numbers()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Repair.FineTuneMinCost > 0);
        Assert.True(ModelRepair.GlitchWeeks(c, new ModelState { Sectors = new List<string> { "web-ads" } }) > 0);
        Assert.Equal(0, ModelRepair.GlitchWeeksLeft(c, new ModelState { Status = ModelStatus.Trained }, 0));
    }

    [Fact]
    public void Rival_log_and_balance_soak_presets_stay_consistent()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Weeks(8);
        List<string> lines = DisplayQueries.RivalLogLines(g.C, g.State, rival, 6);
        BalanceSoak.Preset[] presets = BalanceSoak.DefaultPresets(g.C);
        Assert.NotNull(lines);
        Assert.True(presets.Length >= 3);
        Assert.Equal(rival.Log.Count, DisplayQueries.RivalRelationSlices(g.C, g.State, 3)[0].LogLineCount);
    }
}