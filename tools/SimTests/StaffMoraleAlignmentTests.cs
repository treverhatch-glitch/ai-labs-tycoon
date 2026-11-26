using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q82: average staff morale scales alignment work permille.</summary>
public class StaffMoraleAlignmentTests
{
    static Game WithSafety(int safety = 2)
    {
        return Game.Solo().WithStarterLab(researchers: 1, engineers: 1, safety: safety, sales: 0, gpus: 8);
    }

    [Fact]
    public void Shipped_morale_alignment_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.StaffPeople.MoraleAlignmentPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_alignment_permille()
    {
        Game g = WithSafety();
        Assert.Equal(0, StaffSystem.MoraleAlignmentPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_raises_alignment_work_permille()
    {
        Game baseLab = WithSafety();
        Game happy = WithSafety();
        foreach (StaffMember p in happy.Me.Staff) p.Morale = happy.C.StaffPeople.MoraleBaseline + 50;
        Assert.True(TrainingSystem.AlignmentWorkPermille(happy.C, happy.Me)
            > TrainingSystem.AlignmentWorkPermille(baseLab.C, baseLab.Me));
    }

    [Fact]
    public void Low_morale_reduces_alignment_work_permille()
    {
        Game baseLab = WithSafety();
        Game sad = WithSafety();
        foreach (StaffMember p in sad.Me.Staff) p.Morale = sad.C.StaffPeople.MoraleBaseline - 80;
        Assert.True(TrainingSystem.AlignmentWorkPermille(sad.C, sad.Me)
            < TrainingSystem.AlignmentWorkPermille(baseLab.C, baseLab.Me));
    }

    [Fact]
    public void Zero_scale_disables_morale_alignment_hook()
    {
        Game g = WithSafety();
        g.C.StaffPeople.MoraleAlignmentPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleAlignmentPermille(g.C, g.Me));
    }

    [Fact]
    public void Morale_delta_permille_formula()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState { Staff = { new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 20 } } };
        Assert.Equal(20 * c.StaffPeople.MoraleAlignmentPermillePerPoint, StaffSystem.MoraleAlignmentPermille(c, lab));
    }

    [Fact]
    public void Training_run_fills_alignment_faster_with_high_morale()
    {
        Game low = WithSafety(safety: 3);
        Game high = WithSafety(safety: 3);
        foreach (StaffMember p in high.Me.Staff) p.Morale = high.C.StaffPeople.MoraleBaseline + 60;
        low.GrantResearch(500);
        high.GrantResearch(500);
        low.Research("state-machines");
        high.Research("state-machines");
        ModelState mLow = low.Train("mini", 800);
        ModelState mHigh = high.Train("mini", 800);
        low.Weeks(6);
        high.Weeks(6);
        Assert.True(mHigh.AlignmentCompute > mLow.AlignmentCompute);
    }

    [Fact]
    public void No_staff_zero_alignment_permille()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoraleAlignmentPermille(g.C, g.Me));
    }

    [Fact]
    public void Save_load_preserves_alignment_permille_inputs()
    {
        Game g = WithSafety();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 650;
        int before = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 650;
        Assert.Equal(before, TrainingSystem.AlignmentWorkPermille(loaded.C, loaded.Me));
    }

    [Fact]
    public void Bankrupt_lab_alignment_query_still_finite()
    {
        Game g = WithSafety();
        g.Me.Bankrupt = true;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 200;
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me) >= 0);
    }

    [Fact]
    public void Research_and_alignment_hooks_independent()
    {
        Game g = WithSafety();
        g.Me.Staff[0].Morale = g.C.StaffPeople.MoraleBaseline + 30;
        Assert.NotEqual(StaffSystem.MoraleResearchPermille(g.C, g.Me), StaffSystem.MoraleAlignmentPermille(g.C, g.Me));
    }

    [Fact]
    public void Max_morale_alignment_permille_bounded()
    {
        Game g = WithSafety();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 1000;
        int align = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.InRange(align, MathI.Permille, MathI.Permille + 5000);
    }

    [Fact]
    public void Sim_week_does_not_break_alignment_work_hook()
    {
        Game g = WithSafety();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 25;
        int before = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Weeks(2);
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me) >= before - 50);
    }
}