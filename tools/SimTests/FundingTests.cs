using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FundingTests
{
  [Fact]
  public void Angel_round_pays_cash_and_dilutes_the_founder()
  {
    var g = Game.Solo(tweak: c =>
    {
      var r = c.Funding.Round("angel");
      r.OpensYear = 1995;
      r.MinReputation = 0;
    });
    int before = g.Me.FounderSharePermille;
    long cash = g.Me.Cash;
    long raise = g.C.Funding.Round("angel").Cash;
    int dilution = g.C.Funding.Round("angel").DilutionPermille;
    g.Do(Command.RaiseFunding(g.Id, "angel"));
    Assert.Contains("angel", g.Me.FundingRounds);
    Assert.Equal(cash + raise, g.Me.Cash);
    Assert.Equal(MathI.MulPermille(before, MathI.Permille - dilution), g.Me.FounderSharePermille);
    Assert.Equal(0, g.Me.BoardInvestorSeats);
  }

  [Fact]
  public void Venture_rounds_need_order_reputation_and_year()
  {
    var g = Game.Solo(tweak: c =>
    {
      c.Funding.Round("angel").OpensYear = 1995;
      c.Funding.Round("angel").MinReputation = 0;
      c.Funding.Round("series-a").OpensYear = 1995;
      c.Funding.Round("series-a").MinReputation = 0;
    });
    g.Refused(Command.RaiseFunding(g.Id, "series-a"), CommandError.MissingPrerequisite);
    g.Do(Command.RaiseFunding(g.Id, "angel"));
    g.Do(Command.RaiseFunding(g.Id, "series-a"));
    Assert.Equal(2, g.Me.BoardInvestorSeats);
    g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.InvalidArgument);
  }

  [Fact]
  public void Losing_control_records_the_ousted_ending()
  {
    var g = Game.Solo(tweak: c =>
    {
      foreach (FundingRoundConfig r in c.Funding.Rounds)
      {
        r.OpensYear = 1995;
        r.MinReputation = 0;
      }
      c.Funding.Round("series-b").DilutionPermille = 350;
      c.Funding.ControlPermille = 500;
    });
    string leader = g.Me.Leader;
    g.Do(Command.RaiseFunding(g.Id, "angel"));
    g.Do(Command.RaiseFunding(g.Id, "series-a"));
    g.Do(Command.RaiseFunding(g.Id, "series-b"));
    Assert.True(g.Me.FounderSharePermille <= g.C.Funding.ControlPermille);
    Assert.Single(g.State.Endings, e => e.EndingId == "ousted");
    Assert.Equal(g.C.Funding.OustedSuccessor, g.Me.Leader);
    Assert.NotEqual(leader, g.Me.Leader);
  }

  [Fact]
  public void Schema_7_saves_gain_founder_share_and_empty_rounds()
  {
    Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
    Assert.Equal(1000, sim.Player.FounderSharePermille);
    Assert.Empty(sim.Player.FundingRounds);
    Assert.Equal(0, sim.Player.BoardInvestorSeats);
    Assert.Equal(8, sim.State.Schema);
  }
}