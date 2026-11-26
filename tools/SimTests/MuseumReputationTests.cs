using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q79: full museum lines earn weekly reputation.</summary>
public class MuseumReputationTests
{
    static void AddRetired(Game g, string line, int version)
    {
        int when = g.State.Tick - 10_000 + version * 10;
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = line, Version = version, Name = "v" + version,
            Status = ModelStatus.Retired, Capability = 100, CapBase = 100,
            StartedTick = when, TrainedTick = when, ReleasedTick = when,
        });
    }

    [Fact]
    public void Shipped_museum_reputation_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.Endings.Museum.ReputationPerFullLine);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Full_line_at_cap_grants_reputation()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        int before = g.Me.Reputation;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Endings.Museum.ReputationPerFullLine, g.Me.Reputation);
    }

    [Fact]
    public void Below_cap_grants_no_reputation()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 5; v++) AddRetired(g, "mini", v);
        int before = g.Me.Reputation;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Zero_config_skips_reputation_hook()
    {
        Game g = Game.Solo();
        g.C.Endings.Museum.ReputationPerFullLine = 0;
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        int before = g.Me.Reputation;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Two_full_lines_double_the_weekly_gain()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++)
        {
            AddRetired(g, "mini", v);
            AddRetired(g, "mid", v + 100);
        }
        int before = g.Me.Reputation;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Endings.Museum.ReputationPerFullLine, g.Me.Reputation);
    }

    [Fact]
    public void RetiredOnLine_query_matches_cap_check()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        Assert.Equal(6, MuseumSystem.RetiredOnLine(g.Me, "mini"));
    }

    [Fact]
    public void Prune_drops_below_cap_and_stops_reputation_next_week()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        int afterPrune = g.Me.Reputation;
        Assert.Equal(6, MuseumSystem.RetiredOnLine(g.Me, "mini"));
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(afterPrune + g.C.Endings.Museum.ReputationPerFullLine, g.Me.Reputation);
    }

    [Fact]
    public void Reputation_clamped_at_permille()
    {
        Game g = Game.Solo();
        g.Me.Reputation = 999;
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1000, g.Me.Reputation);
    }

    [Fact]
    public void Save_load_preserves_reputation_after_museum_week()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        int rep = g.Me.Reputation;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(rep, loaded.Me.Reputation);
    }

    [Fact]
    public void Sim_step_runs_museum_reputation_through_full_week()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        int before = g.Me.Reputation;
        g.Weeks(1);
        Assert.True(g.Me.Reputation >= before + g.C.Endings.Museum.ReputationPerFullLine);
    }

    [Fact]
    public void Bankrupt_lab_skips_museum_week_in_simulation()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", v);
        int before = g.Me.Reputation;
        g.Me.Bankrupt = true;
        g.Weeks(1);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Released_models_do_not_count_toward_full_line()
    {
        Game g = Game.Solo();
        for (int v = 0; v < 5; v++) AddRetired(g, "mini", v);
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 99, Status = ModelStatus.Released,
            Capability = 200, CapBase = 200,
        });
        int before = g.Me.Reputation;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void High_per_line_gain_stacks_with_two_lines_before_clamp()
    {
        Game g = Game.Solo();
        g.C.Endings.Museum.ReputationPerFullLine = 400;
        g.Me.Reputation = 700;
        for (int v = 0; v < 6; v++)
        {
            AddRetired(g, "mini", v);
            AddRetired(g, "mid", v + 50);
        }
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1000, g.Me.Reputation);
    }
}