using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FundingFollowOnLaterTests
{
    static void OpenRounds(GameConfig c)
    {
        c.Funding.Round("angel").OpensYear = 1995;
        c.Funding.Round("angel").MinReputation = 0;
        c.Funding.Round("series-a").OpensYear = 1995;
        c.Funding.Round("series-a").MinReputation = 0;
        c.Funding.Round("series-b").OpensYear = 1995;
        c.Funding.Round("series-b").MinReputation = 0;
    }

    [Fact]
    public void A_hot_bubble_triggers_down_round_cash_and_extra_dilution()
    {
        var g = Game.Solo(tweak: c => { OpenRounds(c); c.Funding.FollowOn.DownRoundBubblePermille = 500; });
        g.State.BubblePermille = 700;
        int beforeShare = g.Me.FounderSharePermille;
        long nominal = g.C.Funding.Round("angel").Cash;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(MathI.MulPermille(nominal, g.C.Funding.FollowOn.DownRoundCashPermille), g.Me.Ledger.Lifetime.CapitalIn);
        int expected = (int)MathI.MulPermille(beforeShare, MathI.Permille - g.C.Funding.Round("angel").DilutionPermille - g.C.Funding.FollowOn.DownRoundExtraDilutionPermille);
        Assert.Equal(expected, g.Me.FounderSharePermille);
    }

    [Fact]
    public void A_cool_bubble_pays_full_round_cash()
    {
        var g = Game.Solo(tweak: c => { OpenRounds(c); c.Funding.FollowOn.DownRoundBubblePermille = 900; });
        g.State.BubblePermille = 100;
        long nominal = g.C.Funding.Round("angel").Cash;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(nominal, g.Me.Ledger.Lifetime.CapitalIn);
    }

    [Fact]
    public void Down_round_marks_the_funding_closed_event()
    {
        var g = Game.Solo(tweak: OpenRounds);
        g.State.BubblePermille = 800;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        GameEvent ev = g.State.Events.Last(e => e.Kind == EventKind.FundingClosed);
        Assert.Equal(1, ev.Pick);
        Assert.Equal("angel", ev.Ref);
    }

    [Fact]
    public void Board_lock_refuses_player_commands()
    {
        var g = Game.Solo();
        g.Me.BoardLockedUntilTick = g.Tick + 4;
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.BoardLocked);
    }

    [Fact]
    public void Investor_board_drama_can_lock_the_player_for_four_weeks()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Funding.FollowOn.BoardDramaPermillePerYear = 12000;
            c.Funding.FollowOn.BoardDramaWeeks = 4;
        });
        g.Me.BoardInvestorSeats = 2;
        FundingSystem.RunMonth(g.C, g.State);
        Assert.True(g.Me.BoardLockedUntilTick > g.Tick);
    }

    [Fact]
    public void Board_drama_does_not_run_without_investor_seats()
    {
        var g = Game.Solo(tweak: c => c.Funding.FollowOn.BoardDramaPermillePerYear = 12000);
        g.Me.BoardInvestorSeats = 0;
        FundingSystem.RunMonth(g.C, g.State);
        Assert.Equal(-1, g.Me.BoardLockedUntilTick);
    }

    [Fact]
    public void Investor_events_need_at_least_two_closed_rounds()
    {
        var g = Game.Solo(tweak: c =>
        {
            OpenRounds(c);
            c.Funding.FollowOn.InvestorEventPermillePerMonth = 1000;
            c.Funding.FollowOn.InvestorEventMinRounds = 2;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        int events = g.State.Events.Count;
        FundingSystem.RunMonth(g.C, g.State);
        Assert.Equal(events, g.State.Events.Count);
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        FundingSystem.RunMonth(g.C, g.State);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.InvestorBoardVisit);
    }

    [Fact]
    public void Investor_board_visit_raises_reputation()
    {
        var g = Game.Solo(tweak: c =>
        {
            OpenRounds(c);
            c.Funding.FollowOn.InvestorEventPermillePerMonth = 1000;
            c.Funding.FollowOn.InvestorEventReputation = 12;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        int before = g.Me.Reputation;
        FundingSystem.RunMonth(g.C, g.State);
        Assert.Equal(before + 12, g.Me.Reputation);
    }

    [Fact]
    public void Investor_board_visit_nudges_the_bubble_meter()
    {
        var g = Game.Solo(tweak: c =>
        {
            OpenRounds(c);
            c.Funding.FollowOn.InvestorEventPermillePerMonth = 1000;
            c.Funding.FollowOn.InvestorEventBubblePermille = 20;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        int before = g.State.BubblePermille;
        FundingSystem.RunMonth(g.C, g.State);
        Assert.Equal(before + 20, g.State.BubblePermille);
    }

    [Fact]
    public void Effective_cash_and_dilution_match_the_down_round_rules()
    {
        var g = Game.Solo();
        g.State.BubblePermille = g.C.Funding.FollowOn.DownRoundBubblePermille;
        FundingRoundConfig angel = g.C.Funding.Round("angel");
        Assert.True(FundingSystem.IsDownRound(g.C, g.State));
        Assert.Equal(MathI.MulPermille(angel.Cash, g.C.Funding.FollowOn.DownRoundCashPermille), FundingSystem.EffectiveCash(g.C, g.State, angel));
        Assert.Equal(angel.DilutionPermille + g.C.Funding.FollowOn.DownRoundExtraDilutionPermille, FundingSystem.EffectiveDilution(g.C, g.State, angel));
    }

    [Fact]
    public void ContinueAfterEnding_still_works_while_the_board_is_locked()
    {
        var g = Game.Solo();
        g.Me.GrantPerWeek = 0;
        g.Me.GrantEndsTick = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt" && e.Choice == "");
        g.Me.BoardLockedUntilTick = g.Tick + 4;
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "bailout"));
        Assert.False(g.Me.Bankrupt);
    }

    [Fact]
    public void Schema_16_migration_adds_board_lock_tick()
    {
        var g = Game.Solo();
        g.Me.BoardInvestorSeats = 1;
        string old = SaveSchemaPretend.At(g.C, 15, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(-1, sim.Player.BoardLockedUntilTick);
    }
}