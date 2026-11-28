using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Edge coverage for specialist, data sources, open weights, and sector pricing (schema 135–138).</summary>
public class EraNSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 90)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.DataSources.MinEra = 0;
            c.OpenWeights.MinEra = 0;
            c.SectorPricing.MinEra = 0;
            c.PlayerStart.Cash = 50_000_000;
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    static ModelState Sold(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void Specialist_branch_mid_save_load_matches_kind()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(m.SpecKind, back.Player.Models[0].SpecKind);
    }

    [Fact]
    public void Data_source_mid_save_load_matches_source_weeks()
    {
        Game g = Lab();
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal(g.Me.SrcWeeks0, back.Player.SrcWeeks0);
    }

    [Fact]
    public void Open_weights_mid_save_load_matches_policy()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal(m.WgtOpen, back.Player.Models[0].WgtOpen);
    }

    [Fact]
    public void Schema_135_through_138_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(94);
        ModelState m = Sold(g);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 2));
        string current = g.Sim.Save();
        for (int schema = 135; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 130) HardeningInvariants.AssertEraMStateBounds(g.C, sim.Player);
            if (schema >= 135) HardeningInvariants.AssertEraNStateBounds(g.C, sim.Player);
        }
    }
}