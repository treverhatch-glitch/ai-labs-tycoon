using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SpecialistEraHooksTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(78, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Appeal_boost_when_era_and_specialist_match()
    {
        Game g = Lab(c =>
        {
            c.SpecialistEraHooks.Enabled = 1;
            c.SpecialistEraHooks.MinEra = 0;
            c.SpecialistEraHooks.SpecialistPermille = 100;
            c.SpecialistLines.Enabled = 1;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        long with = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, m);
        g.C.SpecialistEraHooks.Enabled = 0;
        long without = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, m);
        Assert.True(with > without);
    }

    [Fact]
    public void Era_began_fires_unlock_event_at_min_era()
    {
        Game g = Lab(c =>
        {
            c.SpecialistEraHooks.Enabled = 1;
            c.SpecialistEraHooks.MinEra = 3;
        });
        int tick = EraSystem.StartTick(g.C, g.C.Eras[3]);
        g.State.Tick = tick;
        EraSystem.RunWeek(g.C, g.State);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SpecialistEraUnlocked);
    }
}