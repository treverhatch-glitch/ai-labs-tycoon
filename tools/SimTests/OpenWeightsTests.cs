using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class OpenWeightsTests
{
    static Game Lab()
    {
        Game g = Game.Solo(9, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.OpenWeights.MinEra = 0;
            c.OpenWeights.CopyChancePermille = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

  static ModelState Sell(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void Set_weights_policy_is_checked()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        Assert.Equal(1, m.WgtOpen);
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 0));
        Assert.Equal(0, m.WgtOpen);
    }

    [Fact]
    public void Older_save_has_no_wgt_open()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 136, g.Sim.Save()));
        Assert.All(older.Player.Models, x => Assert.Equal(0, x.WgtOpen));
    }
}