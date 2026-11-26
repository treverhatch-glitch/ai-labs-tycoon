using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT #12+ explain: chat/glow, room ageing, rival log, chips (batch 370).</summary>
public class HostExplainNext370LaterTests
{
  static Game WithRival()
    => new Game(Simulation.NewGame(TestData.Config(c => c.Rivalry.LogMax = 64), new NewGameOptions { Seed = 370, RivalIds = new List<string> { "openaye" } }));

    [Fact]
    public void Company_chat_feed_respects_config_line_catalogue()
    {
        var g = Game.Solo().WithStarterLab();
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, g.Me, g.C.Chat.DefaultLines);
        Assert.InRange(feed.Count, 0, g.C.Chat.DefaultLines);
        Assert.True(g.C.Chat.Lines.Count >= 1);
    }

    [Fact]
    public void Glow_stage_matches_worst_model_incident_risk()
    {
        var g = Game.Solo(tweak: c =>
        {
            foreach (EraConfig era in c.Eras) era.IncidentLoadPermille = 1000;
        }).WithStarterLab();
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "m1", Status = ModelStatus.Released,
            Capability = 100, Alignment = 600, DisplayedAlignment = 600, Corruption = 0, LastIncidentTick = -1
        });
        Glow.Report report = Glow.Of(g.C, g.State, g.Me);
        Assert.Equal("hum", report.StageId);
        Assert.True(g.C.Glow.Stages.Count >= 1);
    }

    [Fact]
    public void Rival_log_lines_and_chips_config_expose_depth()
    {
        Game g = WithRival();
        List<string> lines = DisplayQueries.RivalLogLines(g.C, g.State, g.Rival("openaye"), 8);
        Assert.NotNull(lines);
        Assert.True(g.C.Chips.FoundryVendorSharePermille >= 0);
        Assert.True(g.C.Rivalry.RetrenchRunwayWeeks > 0);
    }
}