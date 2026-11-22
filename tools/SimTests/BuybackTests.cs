using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BuybackTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 2, safety: 1, gpus: 4);
        g.Me.Cash = 1_000_000_000;
        return g;
    }

    [Fact]
    public void Nothing_is_bought_unless_the_player_asks()
    {
        Game g = Lab();
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Weeks(30);
        Assert.Empty(g.Me.BoughtRounds);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RoundBoughtBack);
    }

    [Fact]
    public void Buying_a_round_back_costs_cash_and_returns_the_share_and_the_seats()
    {
        Game g = Lab();
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        FundingRoundConfig round = g.C.Funding.Round("angel");
        g.Me.BoardInvestorSeats = round.BoardSeats + 1;
        int share = g.Me.FounderSharePermille;
        long cash = g.Me.Cash;
        long price = Buyback.Price(g.C, g.State, round);
        Assert.True(price > round.Cash * g.C.Buyback.PriceMult / 1000);
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        Assert.Equal(cash - price, g.Me.Cash);
        Assert.True(g.Me.FounderSharePermille > share);
        Assert.Equal(1, g.Me.BoardInvestorSeats);
        Assert.Contains("angel", g.Me.BoughtRounds);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RoundBoughtBack && e.Ref == "angel" && e.Amount == price);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_round_can_be_bought_once_and_only_if_it_was_raised()
    {
        Game g = Lab();
        g.Refused(Command.BuyBackRound(g.Id, "angel"), CommandError.MissingPrerequisite);
        g.Refused(Command.BuyBackRound(g.Id, "nowhere"), CommandError.UnknownId);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        g.Refused(Command.BuyBackRound(g.Id, "angel"), CommandError.InvalidArgument);
        g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_bubble_makes_the_price_dearer()
    {
        Game g = Lab();
        FundingRoundConfig round = g.C.Funding.Round("angel");
        g.State.BubblePermille = 0;
        long calm = Buyback.Price(g.C, g.State, round);
        g.State.BubblePermille = 500;
        long hot = Buyback.Price(g.C, g.State, round);
        Assert.True(hot > calm);
        Assert.Equal(round.Cash * (g.C.Buyback.PriceMult + 500L * g.C.Buyback.BubblePrice / 1000) / 1000, hot);
    }

    [Fact]
    public void A_lab_that_cannot_pay_is_refused_and_nothing_changes()
    {
        Game g = Lab();
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Me.Cash = 1000;
        int share = g.Me.FounderSharePermille;
        g.Refused(Command.BuyBackRound(g.Id, "angel"), CommandError.NotEnoughCash);
        Assert.Equal(share, g.Me.FounderSharePermille);
        Assert.Empty(g.Me.BoughtRounds);
    }

    [Fact]
    public void Buying_back_lifts_a_lab_out_of_danger_of_being_ousted()
    {
        Game g = Lab();
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Me.FounderSharePermille = g.C.Funding.ControlPermille + 10;
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        Assert.True(g.Me.FounderSharePermille > g.C.Funding.ControlPermille + 10);
    }

    [Fact]
    public void Switching_it_off_refuses_the_command()
    {
        Game g = Lab(c => c.Buyback.Enabled = 0);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Refused(Command.BuyBackRound(g.Id, "angel"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_bought_nothing()
    {
        Game g = Lab();
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 117, g.Sim.Save()));
        Assert.Empty(older.State.Labs.First(l => l.Id == g.Id).BoughtRounds);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
