using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Home freeze blocks research spending and pauses point accrual (Q43).</summary>
public class FreezeResearchTests
{
    [Fact]
    public void Frozen_home_refuses_research_tech()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.GrantResearch(50_000);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Research_tech_works_when_not_frozen()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.GrantResearch(50_000);
        g.Do(Command.ResearchTech(g.Id, "state-machines"));
        Assert.Contains("state-machines", g.Me.Techs);
    }

    [Fact]
    public void Accrual_paused_under_home_freeze()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3);
        long before = g.Me.ResearchPoints;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Accrual_resumes_after_lift()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Weeks(1);
        long stalled = g.Me.ResearchPoints;
        home.Level = BlocStanding.Letter;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints > stalled);
    }

    [Fact]
    public void Training_still_stalled_under_same_freeze()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        ModelState model = g.Me.Models.Single(m => m.Status == ModelStatus.Training);
        long before = model.ProgressCompute;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(before, model.ProgressCompute);
    }

    [Fact]
    public void Foreign_bloc_freeze_does_not_pause_accrual()
    {
        Game g = WorldTests.Rich("punchline");
        g.WithStarterLab(researchers: 2);
        long before = g.Me.ResearchPoints;
        g.Me.Bloc(g.C.World.Site("china-guizhou").BlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints > before);
    }

    [Fact]
    public void Not_enough_points_checked_only_after_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.ResearchPoints = 0;
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.NotEnoughResearch);
    }

    [Fact]
    public void Gate_survives_save_load()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.GrantResearch(99_999);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Refused(Command.ResearchTech(loaded.Id, "neural-nets"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Accrual_survives_save_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        long points = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(1);
        Assert.Equal(points, loaded.Me.ResearchPoints);
    }

    [Fact]
    public void Last_week_research_gain_zero_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(0, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Last_week_research_gain_positive_when_thawed()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3);
        g.Weeks(1);
        Assert.True(g.Me.LastWeek.ResearchGained > 0);
    }

    [Fact]
    public void Era_locked_still_before_freeze_message()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(100_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1995);
        g.Refused(Command.ResearchTech(g.Id, "attention-arch"), CommandError.EraLocked);
    }

    [Fact]
    public void Already_researched_still_before_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(100_000);
        g.Me.Techs.Add("state-machines");
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.AlreadyResearched);
    }

    [Fact]
    public void Bankrupt_before_freeze()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bankrupt = true;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Many_weeks_frozen_flat_research()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 4);
        long start = g.Me.ResearchPoints;
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        home.Attention = 1000;
        g.Weeks(12);
        Assert.Equal(start, g.Me.ResearchPoints);
    }

    [Fact]
    public void Hire_researcher_does_not_unfreeze_accrual()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        long before = g.Me.ResearchPoints;
        g.Do(Command.Hire(g.Id, "researcher", 2));
        g.Weeks(1);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Start_training_still_refused_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.PermitFrozen);
    }

    [Fact]
    public void Rent_gpus_still_refused_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.BuyGpus(g.Id, 4), CommandError.PermitFrozen);
    }

    [Fact]
    public void Deterministic_accrual_after_thaw()
    {
        Game a = Game.Solo(seed: 9).WithStarterLab(researchers: 2);
        Game b = Game.Solo(seed: 9).WithStarterLab(researchers: 2);
        a.Me.Bloc(a.Me.HomeBlocId).Level = BlocStanding.Freeze;
        b.Me.Bloc(b.Me.HomeBlocId).Level = BlocStanding.Freeze;
        a.Weeks(3);
        b.Weeks(3);
        a.Me.Bloc(a.Me.HomeBlocId).Level = BlocStanding.Letter;
        b.Me.Bloc(b.Me.HomeBlocId).Level = BlocStanding.Letter;
        a.Weeks(2);
        b.Weeks(2);
        Assert.Equal(a.Me.ResearchPoints, b.Me.ResearchPoints);
    }

    [Fact]
    public void Research_tech_spend_not_deducted_when_refused()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        long points = 40_000;
        g.Me.ResearchPoints = points;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.ResearchTech(g.Id, "neural-nets"), CommandError.PermitFrozen);
        Assert.Equal(points, g.Me.ResearchPoints);
    }

    [Fact]
    public void Trait_bonus_does_not_apply_while_frozen()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.Equal(0, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void Zero_researchers_frozen_stays_zero()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 0);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(2);
        Assert.Equal(0, g.Me.ResearchPoints);
    }

    [Fact]
    public void Freeze_at_letter_still_allows_research()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        g.GrantResearch(50_000);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Letter;
        g.Do(Command.ResearchTech(g.Id, "state-machines"));
        Assert.Contains("state-machines", g.Me.Techs);
    }

    [Fact]
    public void Week_tick_advances_while_research_frozen()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2);
        int tick = g.State.Tick;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(3);
        Assert.Equal(tick + 3, g.State.Tick);
    }

    [Fact]
    public void Research_all_functions_still_safe()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 1, engineers: 1, safety: 1);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Weeks(1);
        Assert.True(g.Me.Staff.Count > 0);
    }
}