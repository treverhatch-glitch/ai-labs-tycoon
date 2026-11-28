using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT rules 41–44: staff traits, loans, escape crisis, scenarios (batch 392–395).</summary>
public class HostExplainNext390LaterTests
{
    [Fact]
    public void Staff_trait_round2_ids_ship_in_config()
    {
        GameConfig c = TestData.Config();
        Assert.NotNull(c.StaffPeople.Trait("posts-through-it"));
        Assert.NotNull(c.StaffPeople.Trait("quits-by-blog-post"));
        Assert.NotNull(c.StaffPeople.Trait("only-interesting"));
    }

    [Fact]
    public void Chip_loan_cap_tracks_owned_gpu_value()
    {
        var g = Game.Solo();
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 4, HardwareBatch.Hq));
        long cap = Loans.Capacity(g.C, g.State, g.Me);
        Assert.True(cap > 0);
        Assert.True(Loans.Outstanding(g.Me) == 0);
    }

    [Fact]
    public void Escape_office_event_is_configured()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig esc = c.Office.Events.First(e => e.Trigger == OfficeTrigger.Escape);
        Assert.False(string.IsNullOrEmpty(esc.IdValue));
        Assert.True(esc.Choices.Count >= 1);
        Assert.NotNull(esc.Choice(esc.LapseChoice));
    }

    [Fact]
    public void Scenario_list_matches_seven_named_starts()
    {
        GameConfig c = TestData.Config();
        var list = Scenarios.List(c);
        Assert.True(list.Count >= 7);
        Assert.All(list, s => Assert.False(string.IsNullOrEmpty(s.Name)));
    }
}