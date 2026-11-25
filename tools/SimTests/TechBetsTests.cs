using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TechBetsTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    [Fact]
    public void A_bet_costs_nothing_and_sets_the_weeks()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(g.C.TechBets.Weeks, g.Me.TbLeft);
        Assert.True(g.Me.TbRef > 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TechBetMade && e.Ref == "regression-tests");
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_a_reachable_unresearched_big_enough_tech_may_be_promised_one_at_a_time()
    {
        Game g = Lab();
        g.Refused(Command.BetOnTech(g.Id, "nope"), CommandError.UnknownId);
        g.Refused(Command.BetOnTech(g.Id, "pathfinding"), CommandError.MissingPrerequisite);
        g.Refused(Command.BetOnTech(g.Id, "search-ranking"), CommandError.MissingPrerequisite);
        g.Me.ResearchPoints = 1000;
        g.Research("regression-tests");
        g.Refused(Command.BetOnTech(g.Id, "regression-tests"), CommandError.AlreadyResearched);
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        g.Refused(Command.BetOnTech(g.Id, "rules-engines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_tiny_tech_is_not_worth_a_public_promise()
    {
        Game g = Lab(c => c.TechBets.MinCost = 500);
        g.Refused(Command.BetOnTech(g.Id, "state-machines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Keeping_the_promise_pays_reputation_and_prestige()
    {
        Game g = Lab();
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        g.Me.ResearchPoints = 5000;
        long rep = g.Me.Reputation;
        long prestige = Prestige.Level(g.C, g.Me);
        g.Weeks(2);
        g.Research("minimax");
        g.Weeks(1);
        Assert.Equal(0, g.Me.TbRef);
        Assert.Equal(1, g.Me.TbWins);
        Assert.True(g.Me.Reputation > rep);
        Assert.True(Prestige.Level(g.C, g.Me) > prestige);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TechBetEnded && e.Pick == 1);
    }

    [Fact]
    public void Missing_the_weeks_costs_reputation()
    {
        Game g = Lab(c => c.TechBets.Weeks = 3);
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        long rep = g.Me.Reputation;
        g.Weeks(4);
        Assert.Equal(0, g.Me.TbRef);
        Assert.Equal(1, g.Me.TbLosses);
        Assert.True(g.Me.Reputation < rep);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TechBetEnded && e.Pick == 0);
    }

    [Fact]
    public void Taking_it_back_costs_less_than_missing_it()
    {
        Game a = Lab();
        a.Do(Command.BetOnTech(a.Id, "minimax"));
        long before = a.Me.Reputation;
        a.Do(Command.WithdrawTechBet(a.Id));
        long withdrawn = before - a.Me.Reputation;
        Assert.Equal(0, a.Me.TbRef);
        Assert.Equal(0, a.Me.TbLosses);
        Assert.Contains(a.State.Events, e => e.Kind == EventKind.TechBetEnded && e.Pick == 2);
        Game b = Lab(c => c.TechBets.Weeks = 2);
        b.Do(Command.BetOnTech(b.Id, "minimax"));
        long beforeB = b.Me.Reputation;
        b.Weeks(3);
        Assert.True(beforeB - b.Me.Reputation > withdrawn);
        a.Refused(Command.WithdrawTechBet(a.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_cannot_bet_and_the_switch_turns_it_off()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.BetOnTech(rival.Id, "minimax")).Ok);
        Game off = Lab(c => c.TechBets.Enabled = 0);
        off.Refused(Command.BetOnTech(off.Id, "minimax"), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_pending_bet_survives_save_and_load()
    {
        Game g = Lab();
        g.Do(Command.BetOnTech(g.Id, "minimax"));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(g.Me.TbLeft, back.State.Labs.First(l => l.Id == g.Id).TbLeft);
    }

    [Fact]
    public void An_older_save_has_no_bet()
    {
        Game g = Lab();
        string old = SaveSchemaPretend.At(g.C, 99, g.Sim.Save());
        Simulation back = Simulation.Load(g.C, old);
        Assert.Equal(0, back.State.Labs.First(l => l.Id == g.Id).TbRef);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }
}
