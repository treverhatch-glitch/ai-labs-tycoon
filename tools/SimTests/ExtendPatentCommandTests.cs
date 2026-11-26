using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q161: ExtendPatent command.</summary>
public class ExtendPatentCommandTests
{
    static Game WithPatent() => RenouncePatentCommandTests.WithPatent();

    [Fact]
    public void Q161_main_extends_expiry_and_spends_cash()
    {
        Game g = WithPatent();
        int before = g.Me.Patents[0].ExpiresTick;
        long cash = g.Me.Cash;
        g.Do(Command.ExtendPatent(g.Id, "state-machines"));
        Assert.Equal(before + g.C.Research.Patents.ExtendWeeks, g.Me.Patents[0].ExpiresTick);
        Assert.Equal(cash - g.C.Research.Patents.ExtendCashCost, g.Me.Cash);
    }

    [Fact]
    public void Q161_zero_refused_without_patent()
    {
        var g = Game.Solo().WithStarterLab();
        g.Refused(Command.ExtendPatent(g.Id, "state-machines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q161_zero_weeks_disabled()
    {
        Game g = WithPatent();
        g.C.Research.Patents.ExtendWeeks = 0;
        g.Refused(Command.ExtendPatent(g.Id, "state-machines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q161_max_double_extend_stacks()
    {
        Game g = WithPatent();
        int t0 = g.Me.Patents[0].ExpiresTick;
        g.Do(Command.ExtendPatent(g.Id, "state-machines"));
        g.Do(Command.ExtendPatent(g.Id, "state-machines"));
        Assert.Equal(t0 + 2 * g.C.Research.Patents.ExtendWeeks, g.Me.Patents[0].ExpiresTick);
    }

    [Fact]
    public void Q161_bankrupt_refused()
    {
        Game g = WithPatent();
        g.Me.Bankrupt = true;
        g.Refused(Command.ExtendPatent(g.Id, "state-machines"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q161_save_load_expiry()
    {
        Game g = WithPatent();
        g.Do(Command.ExtendPatent(g.Id, "state-machines"));
        int exp = g.Me.Patents[0].ExpiresTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(exp, loaded.Me.Patents[0].ExpiresTick);
    }

    [Fact]
    public void Q161_cross_blocks_rival_after_extend()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        rival.ResearchPoints = 500;
        g.Me.Cash = 2_000_000;
        g.GrantResearch(500);
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        g.Do(Command.ExtendPatent(g.Id, "state-machines"));
        g.Refused(Command.ResearchTech(rival.Id, "state-machines"), CommandError.PatentBlocked);
    }

    [Fact]
    public void Q161_needs_cash()
    {
        Game g = WithPatent();
        g.Me.Cash = g.C.Research.Patents.ExtendCashCost - 1;
        g.Refused(Command.ExtendPatent(g.Id, "state-machines"), CommandError.NotEnoughCash);
    }
}