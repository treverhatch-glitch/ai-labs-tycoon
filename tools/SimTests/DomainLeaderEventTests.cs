using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q68: EventKind.DomainLeaderChanged when weekly domain leader lab changes.</summary>
public class DomainLeaderEventTests
{
    static Game At(int year) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_event_kind_listed_in_validator()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Market.CapabilityDomains, d => d.Id == "language");
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void First_leader_emits_event_with_prior_minus_one()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 35_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.DomainLeaderChanged);
        Assert.Equal(g.Id, e.LabId);
        Assert.Equal(-1, e.SubjectId);
        Assert.Equal("language", e.Ref);
        Assert.True(e.Amount > 0);
    }

    [Fact]
    public void Zero_models_emits_no_domain_events()
    {
        Game g = At(2028);
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.DomainLeaderChanged);
    }

    [Fact]
    public void Zero_change_second_run_week_silent()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int n = g.State.Events.Count;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(n, g.State.Events.Count);
    }

    [Fact]
    public void Max_three_domains_first_week_three_events()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        ExpansionTestSupport.AddReleasedModel(g, 42_000, "image-design");
        ExpansionTestSupport.AddReleasedModel(g, 50_000, "expert-systems");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Equal(3, g.State.Events.Count(e => e.Kind == EventKind.DomainLeaderChanged));
    }

    [Fact]
    public void Bankrupt_sole_leader_clears_domain_board()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.NotNull(CapabilityDomainSystem.Leader(g.State, "language"));
        g.Me.Bankrupt = true;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        Assert.Null(CapabilityDomainSystem.Leader(g.State, "language"));
    }

    [Fact]
    public void Bankrupt_prior_leader_rival_takes_over_emits_event()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModel(g, 30_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        g.Me.Bankrupt = true;
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 45_000, "code-completion", "Rival");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.DomainLeaderChanged);
        Assert.Equal(rival.Id, ev.LabId);
        Assert.Equal(g.Id, ev.SubjectId);
    }

    [Fact]
    public void Save_load_preserves_domain_leader_events()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 33_000, "doc-reading");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        int count = g.State.Events.Count(e => e.Kind == EventKind.DomainLeaderChanged);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(count, loaded.State.Events.Count(e => e.Kind == EventKind.DomainLeaderChanged));
        GameEvent ev = loaded.State.Events.First(e => e.Kind == EventKind.DomainLeaderChanged);
        Assert.Contains("Language", EventText.Describe(loaded.C, loaded.State, ev));
    }

    [Fact]
    public void Schema_20_migration_then_run_week_still_emits()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        ExpansionTestSupport.AddReleasedModel(g, 28_000, "code-completion");
        g.State.Schema = 20;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(23, loaded.State.Schema);
        Assert.Empty(loaded.State.DomainLeaders);
        CapabilityDomainSystem.RunWeek(loaded.C, loaded.State);
        Assert.Contains(loaded.State.Events, e => e.Kind == EventKind.DomainLeaderChanged);
    }

    [Fact]
    public void Sim_step_emits_via_simulation_run_week()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 36_000, "code-completion");
        g.Weeks(1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DomainLeaderChanged);
    }

    [Fact]
    public void Rival_overtake_emits_event_for_new_lab()
    {
        Game g = ExpansionTestSupport.DuoAtYear(2028).WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ExpansionTestSupport.AddReleasedModel(g, 35_000, "code-completion");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 120_000, "code-completion", "Big");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.DomainLeaderChanged);
        Assert.Equal(rival.Id, ev.LabId);
        Assert.Equal(g.Id, ev.SubjectId);
    }

    [Fact]
    public void Event_text_names_domain()
    {
        Game g = At(2028);
        ExpansionTestSupport.AddReleasedModel(g, 30_000, "image-design");
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.DomainLeaderChanged);
        Assert.Contains("Vision", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Market_appeal_rises_when_domain_leader_wins()
    {
        Game g = At(2028);
        ModelState m = ExpansionTestSupport.AddReleasedModel(g, 40_000, "code-completion");
        var sector = g.C.Sector("code-completion");
        var sectorState = g.State.Sector("code-completion");
        long before = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, m).Total;
        CapabilityDomainSystem.RunWeek(g.C, g.State);
        long after = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, m).Total;
        Assert.True(after > before);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, m).DomainLeaderBoost > 0);
    }
}