using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DataSourceSpecialistPairingTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(84, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.DataSources.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Acquire_fires_pair_event_for_matching_specialist()
    {
        Game g = Lab(c =>
        {
            c.DataSourceSpecialistPairing.Enabled = 1;
            c.DataSourceSpecialistPairing.MinEra = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        g.Do(Command.AcquireDataSource(g.Id, "expert-labels"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSourceSpecialistPaired);
    }

    [Fact]
    public void Bonus_raises_training_quality_permille()
    {
        Game g = Lab(c =>
        {
            c.DataSourceSpecialistPairing.Enabled = 1;
            c.DataSourceSpecialistPairing.MinEra = 0;
            c.DataSourceSpecialistPairing.Pairs[0].BonusPermille = 100;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        g.Do(Command.AcquireDataSource(g.Id, "expert-labels"));
        int bonus = DataSourceSpecialistPairing.BonusPermille(g.C, g.State, g.Me);
        Assert.True(bonus >= 100);
    }
}