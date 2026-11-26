using System.Linq;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q156: RenouncePatent command.</summary>
public class RenouncePatentCommandTests
{
    internal static Game WithPatent()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 1_000_000;
        g.GrantResearch(500);
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        return g;
    }

    [Fact]
    public void Q156_main_clears_active_patent()
    {
        Game g = WithPatent();
        g.Do(Command.RenouncePatent(g.Id, "state-machines"));
        Assert.Empty(g.Me.Patents);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PatentExpired && e.Ref == "state-machines");
    }

    [Fact]
    public void Q156_zero_refused_without_patent()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.RenouncePatent(g.Id, "state-machines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q156_max_rival_can_research_after_renounce()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        rival.Cash = 2_000_000;
        rival.ResearchPoints = 500;
        g.Me.Cash = 2_000_000;
        g.GrantResearch(500);
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        g.Refused(Command.ResearchTech(rival.Id, "state-machines"), CommandError.PatentBlocked);
        g.Do(Command.RenouncePatent(g.Id, "state-machines"));
        Assert.True(g.Do(Command.ResearchTech(rival.Id, "state-machines")).Ok);
    }

    [Fact]
    public void Q156_bankrupt_refused()
    {
        Game g = WithPatent();
        g.Me.Bankrupt = true;
        g.Refused(Command.RenouncePatent(g.Id, "state-machines"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q156_save_load_patent_gone()
    {
        Game g = WithPatent();
        g.Do(Command.RenouncePatent(g.Id, "state-machines"));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Empty(loaded.Me.Patents);
    }

    [Fact]
    public void Q156_cross_refile_after_renounce()
    {
        Game g = WithPatent();
        g.Do(Command.RenouncePatent(g.Id, "state-machines"));
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        Assert.Single(g.Me.Patents);
    }

    [Fact]
    public void Q156_refused_unknown_tech()
    {
        Game g = WithPatent();
        g.Refused(Command.RenouncePatent(g.Id, "void-tech"), CommandError.UnknownId);
    }

    [Fact]
    public void Q156_refused_when_frozen()
    {
        Game g = WithPatent();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.RenouncePatent(g.Id, "state-machines"), CommandError.PermitFrozen);
    }
}