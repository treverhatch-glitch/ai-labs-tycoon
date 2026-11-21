using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>An overworked lab (it cannot serve its customers) has a less reliable model: more incidents until it catches up.</summary>
public class OverloadTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rivals = false)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        if (!rivals) options.RivalIds = new System.Collections.Generic.List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        return g;
    }

    static void Strain(LabState lab, int servedPermille, long demand = 1000)
    {
        lab.LastWeek.ServingDemand = demand;
        lab.LastWeek.ServedPermille = servedPermille;
    }

    [Fact]
    public void A_lab_that_serves_everything_has_no_overload_risk()
    {
        Game g = Lab();
        Strain(g.Me, 1000);
        Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_lab_with_no_customers_has_none_either()
    {
        Game g = Lab();
        Strain(g.Me, 0, demand: 0);
        Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_risk_scales_with_the_share_left_unserved()
    {
        Game g = Lab();
        Strain(g.Me, 0);
        Assert.Equal(60, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
        Strain(g.Me, 500);
        Assert.Equal(30, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
        Strain(g.Me, 900);
        Assert.Equal(6, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Rivals_are_not_overloaded_unless_the_data_says_so()
    {
        Game g = Lab(rivals: true);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Strain(rival, 0);
        Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, rival));
        g.C.Safety.OverloadForRivals = true;
        Assert.Equal(60, SafetySystem.OverloadPermille(g.C, g.State, rival));
    }

    [Fact]
    public void Zero_in_the_data_switches_it_off()
    {
        Game g = Lab(c => c.Safety.OverloadIncidentPermille = 0);
        Strain(g.Me, 0);
        Assert.Equal(0, SafetySystem.OverloadPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_weekly_roll_adds_it_to_the_models_own_chance_and_never_passes_certainty()
    {
        Game g = Lab();
        var model = new ModelState { Alignment = 500, DisplayedAlignment = 500, Status = ModelStatus.Released };
        int own = SafetySystem.IncidentChancePermille(g.C, g.State.Tick, model);
        Strain(g.Me, 0);
        Assert.Equal(own + 60, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, model));
        g.C.Safety.OverloadIncidentPermille = 1000;
        g.C.Safety.IncidentChancePermille = 1000;
        Assert.Equal(1000, SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, model));
    }

    [Fact]
    public void The_odds_breakdown_shows_the_overload_and_adds_up()
    {
        Game g = Lab();
        var model = new ModelState { Alignment = 500, DisplayedAlignment = 500, Status = ModelStatus.Released };
        Strain(g.Me, 250);
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.State, g.Me, model);
        Assert.Equal(45, b.FromOverload);
        Assert.Equal(DisplayQueries.IncidentParts(g.C, g.State.Tick, model).Total + 45, b.Total);
    }

    [Fact]
    public void An_overloaded_lab_has_more_incidents_over_a_long_run_than_one_that_keeps_up()
    {
        int Count(bool overloaded)
        {
            Game g = Lab(c => { c.Safety.OverloadIncidentPermille = 300; c.Safety.IncidentChancePermille = 20; });
            ModelState m = g.Train("mini", 100);
            g.UntilTrained(m);
            g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
            int before = g.State.Events.Count(e => e.Kind == EventKind.Incident);
            for (int i = 0; i < 200; i++)
            {
                if (overloaded) Strain(g.Me, 0); else Strain(g.Me, 1000);
                g.State.Tick++;
                SafetySystem.RunWeek(g.C, g.State, g.Me);
            }
            return g.State.Events.Count(e => e.Kind == EventKind.Incident) - before;
        }
        int calm = Count(false);
        int strained = Count(true);
        Assert.True(strained > calm + 10, strained + " incidents when overloaded and " + calm + " when not");
    }

    [Fact]
    public void The_shipped_values_are_valid_and_a_bad_one_is_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(60, c.Safety.OverloadIncidentPermille);
        Assert.False(c.Safety.OverloadForRivals);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Safety.OverloadIncidentPermille = 1001;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("overloadIncidentPermille"));
    }
}
