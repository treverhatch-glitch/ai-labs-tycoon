using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q80: average staff morale scales weekly research output.</summary>
public class StaffMoraleResearchTests
{
    static Game WithResearchers(int count = 3)
    {
        Game g = Game.Solo().WithStarterLab(researchers: count, engineers: 0, safety: 0, sales: 0, gpus: 4);
        return g;
    }

    [Fact]
    public void Shipped_morale_research_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(4, c.StaffPeople.MoraleResearchPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_research_permille()
    {
        Game g = WithResearchers();
        Assert.Equal(0, StaffSystem.MoraleResearchPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_raises_weekly_research()
    {
        Game low = WithResearchers();
        Game high = WithResearchers();
        foreach (StaffMember p in high.Me.Staff) p.Morale = high.C.StaffPeople.MoraleBaseline + 50;
        low.Weeks(1);
        high.Weeks(1);
        Assert.True(high.Me.LastWeek.ResearchGained > low.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Low_morale_reduces_weekly_research()
    {
        Game baseLab = WithResearchers();
        Game sad = WithResearchers();
        foreach (StaffMember p in sad.Me.Staff) p.Morale = sad.C.StaffPeople.MoraleBaseline - 100;
        baseLab.Weeks(1);
        sad.Weeks(1);
        Assert.True(sad.Me.LastWeek.ResearchGained < baseLab.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Zero_scale_config_disables_morale_hook()
    {
        Game g = WithResearchers();
        g.C.StaffPeople.MoraleResearchPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleResearchPermille(g.C, g.Me));
        g.Weeks(1);
        Game plainGame = WithResearchers();
        plainGame.Weeks(1);
        Assert.Equal(plainGame.Me.LastWeek.ResearchGained, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Average_morale_query_matches_staff()
    {
        Game g = WithResearchers(4);
        g.Me.Staff[0].Morale = 400;
        g.Me.Staff[1].Morale = 600;
        g.Me.Staff[2].Morale = 500;
        g.Me.Staff[3].Morale = 500;
        Assert.Equal(500, StaffSystem.AverageMorale(g.Me));
    }

    [Fact]
    public void No_staff_zero_average_and_permille()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.AverageMorale(g.Me));
        Assert.Equal(0, StaffSystem.MoraleResearchPermille(g.C, g.Me));
    }

    [Fact]
    public void Frozen_bloc_still_zero_research_regardless_of_morale()
    {
        Game g = WithResearchers();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(0, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Save_load_preserves_morale_research_outcome()
    {
        Game g = WithResearchers();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        g.Weeks(1);
        long gained = g.Me.LastWeek.ResearchGained;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 700;
        loaded.Weeks(1);
        Assert.Equal(gained, loaded.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Morale_permille_formula_uses_baseline_delta()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 25 });
        Assert.Equal(25 * c.StaffPeople.MoraleResearchPermillePerPoint, StaffSystem.MoraleResearchPermille(c, lab));
    }

    [Fact]
    public void Trait_bonus_stacks_with_morale_on_research()
    {
        Game g = WithResearchers(2);
        StaffTraitConfig star = g.C.StaffPeople.Traits.Find(t => t.Star);
        Assert.NotNull(star);
        foreach (StaffMember p in g.Me.Staff)
        {
            p.Morale = g.C.StaffPeople.MoraleBaseline + 40;
            p.TraitId = star.Id;
        }
        Game meh = WithResearchers(2);
        g.Weeks(1);
        meh.Weeks(1);
        Assert.True(g.Me.LastWeek.ResearchGained > meh.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Bankrupt_lab_research_not_run_by_sim()
    {
        Game g = WithResearchers();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        g.Me.Bankrupt = true;
        long before = g.Me.ResearchPoints;
        g.Weeks(1);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Sim_week_matches_direct_run_research_hook()
    {
        Game g = WithResearchers();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline + 30;
        TrainingSystem.RunResearch(g.C, g.Me, g.Tick);
        long direct = g.Me.LastWeek.ResearchGained;
        g.Me.ResearchPoints = 0;
        g.Me.LastWeek.ResearchGained = 0;
        g.Weeks(1);
        Assert.Equal(direct, g.Me.LastWeek.ResearchGained);
    }
}