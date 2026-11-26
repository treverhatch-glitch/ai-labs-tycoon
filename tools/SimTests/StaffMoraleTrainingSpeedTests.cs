using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q84: average staff morale scales training speed permille.</summary>
public class StaffMoraleTrainingSpeedTests
{
    static Game WithEngineers(int engineers = 3)
    {
        return Game.Solo().WithStarterLab(researchers: 1, engineers: engineers, safety: 0, sales: 0, gpus: 12);
    }

    [Fact]
    public void Shipped_morale_training_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.StaffPeople.MoraleTrainingPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_training_permille()
    {
        Game g = WithEngineers();
        Assert.Equal(0, StaffSystem.MoraleTrainingPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_raises_speed_permille()
    {
        Game baseLab = WithEngineers();
        Game happy = WithEngineers();
        foreach (StaffMember p in happy.Me.Staff) p.Morale = happy.C.StaffPeople.MoraleBaseline + 50;
        Assert.True(TrainingSystem.SpeedPermille(happy.C, happy.State, happy.Me)
            > TrainingSystem.SpeedPermille(baseLab.C, baseLab.State, baseLab.Me));
    }

    [Fact]
    public void Low_morale_reduces_speed_permille()
    {
        Game baseLab = WithEngineers();
        Game sad = WithEngineers();
        foreach (StaffMember p in sad.Me.Staff) p.Morale = sad.C.StaffPeople.MoraleBaseline - 60;
        Assert.True(TrainingSystem.SpeedPermille(sad.C, sad.State, sad.Me)
            < TrainingSystem.SpeedPermille(baseLab.C, baseLab.State, baseLab.Me));
    }

    [Fact]
    public void Zero_scale_config_disables_morale_hook()
    {
        Game g = WithEngineers();
        g.C.StaffPeople.MoraleTrainingPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleTrainingPermille(g.C, g.Me));
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Game plain = WithEngineers();
        Assert.Equal(speed, TrainingSystem.SpeedPermille(plain.C, plain.State, plain.Me));
    }

    [Fact]
    public void Happy_team_trains_faster_than_glum()
    {
        Game fast = WithEngineers();
        Game slow = WithEngineers();
        foreach (StaffMember p in fast.Me.Staff) p.Morale = fast.C.StaffPeople.MoraleBaseline + 80;
        foreach (StaffMember p in slow.Me.Staff) p.Morale = slow.C.StaffPeople.MoraleBaseline - 40;
        ModelState fm = fast.Train("mini", 0);
        ModelState sm = slow.Train("mini", 0);
        fast.Weeks(4);
        slow.Weeks(4);
        Assert.True(fm.ProgressCompute > sm.ProgressCompute);
    }

    [Fact]
    public void No_staff_zero_permille()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoraleTrainingPermille(g.C, g.Me));
    }

    [Fact]
    public void Morale_training_formula_uses_baseline_delta()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 20 });
        Assert.Equal(20 * c.StaffPeople.MoraleTrainingPermillePerPoint, StaffSystem.MoraleTrainingPermille(c, lab));
    }

    [Fact]
    public void Save_load_preserves_speed_permille()
    {
        Game g = WithEngineers();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 700;
        Assert.Equal(speed, TrainingSystem.SpeedPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Bankrupt_lab_speed_query_still_defined()
    {
        Game g = WithEngineers();
        g.Me.Bankrupt = true;
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me) >= 1000);
    }

    [Fact]
    public void Engineer_bonus_stacks_with_morale()
    {
        Game g = WithEngineers(4);
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 30;
        Game few = WithEngineers(1);
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me)
            > TrainingSystem.SpeedPermille(few.C, few.State, few.Me));
    }

    [Fact]
    public void Sim_week_progress_respects_morale_speed()
    {
        Game happy = WithEngineers();
        Game meh = WithEngineers();
        foreach (StaffMember p in happy.Me.Staff) p.Morale = happy.C.StaffPeople.MoraleBaseline + 100;
        ModelState hm = happy.Train("mini", 0);
        ModelState mm = meh.Train("mini", 0);
        happy.Weeks(1);
        meh.Weeks(1);
        Assert.True(hm.ProgressCompute >= mm.ProgressCompute);
    }

    [Fact]
    public void Negative_delta_below_baseline_reduces_speed()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline - 50 });
        Assert.True(StaffSystem.MoraleTrainingPermille(c, lab) < 0);
    }
}