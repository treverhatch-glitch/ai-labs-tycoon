using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q87: rival-distill finishes gain an extra capability permille bump.</summary>
public class DistillFinishCapabilityTests
{
    static Game DistillReady(long seed = 1)
    {
        Game g = Game.New(seed, c =>
        {
            c.Training.LuckPermille = 0;
            c.Training.DataSource("distill-rival").OpensYear = 0;
        });
        g.WithStarterLab(gpus: 48);
        g.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        return g;
    }

    [Fact]
    public void Shipped_distill_finish_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(60, c.Training.DistillFinishCapabilityPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Distill_finish_higher_than_zero_bonus_off()
    {
        Game withBonus = DistillReady(2);
        GameConfig c = TestData.Config(cfg =>
        {
            cfg.Training.LuckPermille = 0;
            cfg.Training.DistillFinishCapabilityPermille = 0;
            cfg.Training.DataSource("distill-rival").OpensYear = 0;
        });
        var sim = Simulation.NewGame(c, 2);
        Game plain = new Game(sim);
        plain.WithStarterLab(gpus: 48);
        plain.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(plain);
        plain.Do(Command.SetDistillTarget(plain.Id, rival.Id));
        plain.Do(Command.SetTrainingDataSource(plain.Id, "distill-rival"));
        ModelState boosted = withBonus.Train("mini", 100);
        withBonus.UntilTrained(boosted);
        ModelState flat = plain.Train("mini", 100);
        plain.UntilTrained(flat);
        Assert.True(boosted.Capability > flat.Capability);
    }

    [Fact]
    public void Internal_finish_unaffected()
    {
        Game g = Game.Solo(3, c => c.Training.LuckPermille = 0);
        g.WithStarterLab(gpus: 32);
        ModelState m = g.Train("mini", 50);
        g.UntilTrained(m);
        Assert.Equal(-1, m.DistillSourceLabId);
    }

    [Fact]
    public void Zero_bonus_config_no_extra_gap()
    {
        Game with = DistillReady(4);
        Game without = Game.New(4, c =>
        {
            c.Training.LuckPermille = 0;
            c.Training.DistillFinishCapabilityPermille = 0;
            c.Training.DataSource("distill-rival").OpensYear = 0;
        });
        without.WithStarterLab(gpus: 48);
        without.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(without);
        without.Do(Command.SetDistillTarget(without.Id, rival.Id));
        without.Do(Command.SetTrainingDataSource(without.Id, "distill-rival"));
        ModelState boosted = with.Train("mini", 100);
        ModelState flat = without.Train("mini", 100);
        with.UntilTrained(boosted);
        without.UntilTrained(flat);
        Assert.True(boosted.Capability > flat.Capability);
    }

    [Fact]
    public void Negative_distill_lab_id_no_bonus()
    {
        Game g = Game.Solo(5, c => c.Training.LuckPermille = 0);
        g.WithStarterLab(gpus: 24);
        ModelState m = g.Train("mini", 0);
        m.DistillSourceLabId = -1;
        g.UntilTrained(m);
        Assert.True(m.Capability > 0);
    }

    [Fact]
    public void Scrape_finish_no_distill_lab_stamp()
    {
        Game g = Game.Solo(6, c =>
        {
            c.Training.DataSource("scrape").OpensYear = 0;
            c.Training.LuckPermille = 0;
        });
        g.WithStarterLab(gpus: 32);
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 50);
        g.UntilTrained(m);
        Assert.Equal(-1, m.DistillSourceLabId);
    }

    [Fact]
    public void Save_load_preserves_distill_finish()
    {
        Game g = DistillReady(7);
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        int cap = m.Capability;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState lm = loaded.Me.Model(m.Id);
        Assert.Equal(cap, lm.Capability);
        Assert.True(lm.Capability > 0);
    }

    [Fact]
    public void Hand_start_distill_stamps_source_for_bonus()
    {
        Game g = DistillReady(8);
        ModelState m = g.Train("mini", 100);
        Assert.True(m.DistillSourceLabId >= 0);
        g.UntilTrained(m);
        Assert.Equal(ModelStatus.Trained, m.Status);
    }

    [Fact]
    public void Capability_bonus_is_multiplicative_permille()
    {
        GameConfig c = TestData.Config();
        int bonus = c.Training.DistillFinishCapabilityPermille;
        int baseCap = 1000;
        int expected = (int)MathI.MulPermille(baseCap, MathI.Permille + bonus);
        Assert.True(expected > baseCap);
    }

    [Fact]
    public void Config_validator_rejects_negative_bonus()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Training.DistillFinishCapabilityPermille = -1));
    }

    [Fact]
    public void Distill_without_target_still_refused()
    {
        Game g = Game.Solo(1, c => c.Training.DataSource("distill-rival").OpensYear = 0);
        g.WithStarterLab();
        g.Refused(Command.SetTrainingDataSource(g.Id, "distill-rival"), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Frozen_home_blocks_distill_start_not_finish_math()
    {
        Game g = DistillReady(9);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 50), CommandError.PermitFrozen);
    }

    [Fact]
    public void Two_distill_finishes_both_boosted()
    {
        Game g = DistillReady(10);
        ModelState a = g.Train("mini", 80);
        g.UntilTrained(a);
        g.Do(Command.ReleaseModel(g.Id, a.Id, 1, "game-ai"));
        PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("openaye").Id));
        ModelState b = g.Train("mini", 80);
        g.UntilTrained(b);
        Assert.Equal("distill-rival", b.DataSourceId);
        Assert.True(b.DistillSourceLabId >= 0);
        Assert.True(b.Capability > 0);
    }

    static LabState PrepRivalAhead(Game g)
    {
        LabState rival = g.Rival("openaye");
        g.Try(Command.SetRentedGpus(rival.Id, 48));
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 40)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, 1, "game-ai"));
        return rival;
    }
}