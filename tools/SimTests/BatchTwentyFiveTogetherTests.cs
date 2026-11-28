using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q509 combination wiring for batch twenty-five systems.</summary>
public class BatchTwentyFiveTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(120, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Exit_debt_haircut_on_sell()
    {
        Game g = Lab(c =>
        {
            c.ExitDebtEraHaircut.Enabled = 1;
            c.ExitDebtEraHaircut.MinEra = 0;
            c.ExitDebtEraHaircut.HaircutPermille = 200;
        });
        g.Build("server-closet", 4, 4);
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Me.Ledger.ThisMonth.Revenue = 500_000;
        g.Do(Command.TakeLoan(g.Id, 950));
        var raw = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.True(raw.Total > g.Me.Cash);
        g.Do(Command.SellLab(g.Id, "spectate"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ExitDebtEraHaircutted);
    }

    [Fact]
    public void Takeoff_answer_shortens_cooldown()
    {
        Game g = Lab(c =>
        {
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.TakeoffAnswerEraCooldown.Enabled = 1;
            c.TakeoffAnswerEraCooldown.MinEra = 0;
            c.TakeoffAnswerEraCooldown.CooldownCutWeeks = 10;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        g.Do(Command.RespondTakeoff(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TakeoffAnswerEraCooldownShortened);
        Assert.Equal(g.Tick + 38, g.Me.TakeoffCooldownUntil);
    }

    [Fact]
    public void Scenario_goal_met_grants_trust()
    {
        Game g = Lab(c =>
        {
            c.ScenarioGoals.Enabled = 1;
            c.ScenarioGoalEraTrust.Enabled = 1;
            c.ScenarioGoalEraTrust.MinEra = 0;
            c.ScenarioGoalEraTrust.TrustPoints = 30;
            c.Scenarios.Scenarios[0].GoalDeadlinePermille = 0;
        });
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        ScenarioGoalConfig goal = g.C.ScenarioGoals.Goals.Find(x => x.IdValue == "ten-staff");
        if (goal != null) goal.Target = g.Me.Staff.Count;
        ScenarioGoals.Check(g.C, g.State, g.Me, deadline: false);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioGoalEraTrusted);
    }
}