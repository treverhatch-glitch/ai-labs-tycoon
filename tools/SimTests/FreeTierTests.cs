using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FreeTierTests
{
    static Game Lab(bool model = true, System.Action<GameConfig> more = null)
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
        g.Me.Models.Clear();
        if (model) g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "Free" });
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    [Fact]
    public void Opening_needs_a_released_model_and_a_level_and_costs_nothing_up_front()
    {
        Game none = Lab(false);
        none.Refused(Command.OpenFreeTier(none.Id, 1), CommandError.MissingPrerequisite);
        Game g = Lab();
        g.Refused(Command.OpenFreeTier(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.OpenFreeTier(g.Id, 4), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.OpenFreeTier(g.Id, 2));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(2, g.Me.FtLevel);
        Assert.Equal(0, g.Me.FtReach);
        g.Refused(Command.OpenFreeTier(g.Id, 1), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Reach_grows_by_the_levels_step_and_stops_at_the_top()
    {
        Game g = Lab();
        g.Do(Command.OpenFreeTier(g.Id, 3));
        g.Weeks(3);
        Assert.Equal(3 * g.C.FreeTier.StepC, g.Me.FtReach);
        g.Weeks(20);
        Assert.Equal(1000, g.Me.FtReach);
    }

    [Fact]
    public void Serving_costs_more_as_the_reach_grows_and_is_recorded()
    {
        Game g = Lab();
        g.Do(Command.OpenFreeTier(g.Id, 1));
        long first = FreeTier.WeeklyCost(g.C, g.Me);
        g.Weeks(10);
        Assert.True(FreeTier.WeeklyCost(g.C, g.Me) > first);
        Assert.True(g.Me.FtSpent >= 10 * first);
    }

    [Fact]
    public void A_higher_level_has_a_higher_ceiling_faster_reach_and_a_higher_bill()
    {
        Game g = Lab();
        Assert.True(g.C.FreeTier.LiftC > g.C.FreeTier.LiftB && g.C.FreeTier.LiftB > g.C.FreeTier.LiftA);
        Assert.True(g.C.FreeTier.StepC > g.C.FreeTier.StepB && g.C.FreeTier.StepB > g.C.FreeTier.StepA);
        Assert.True(g.C.FreeTier.CostC > g.C.FreeTier.CostB && g.C.FreeTier.CostB > g.C.FreeTier.CostA);
        g.Do(Command.OpenFreeTier(g.Id, 3));
        g.Me.FtReach = 1000;
        Assert.Equal(MathI.MulPermille(1_000_000, g.C.FreeTier.LiftC), FreeTier.RevenueLift(g.C, g.State, g.Me, 1_000_000));
        g.Me.FtReach = 500;
        Assert.Equal(MathI.MulPermille(MathI.MulPermille(1_000_000, g.C.FreeTier.LiftC), 500), FreeTier.RevenueLift(g.C, g.State, g.Me, 1_000_000));
        Assert.Equal(0, FreeTier.RevenueLift(g.C, g.State, g.Me, 0));
    }

    static Game Played(bool tier)
    {
        Game g = Game.New(1);
        g.Autopilot(60);
        if (tier) g.Do(Command.OpenFreeTier(g.Id, 2));
        return g;
    }

    [Fact]
    public void In_a_real_game_the_tier_adds_revenue_and_cost_to_the_player_only()
    {
        Game with = Played(true);
        Game without = Played(false);
        Assert.Equal(without.Me.Ledger.Lifetime.Revenue, with.Me.Ledger.Lifetime.Revenue);
        with.Weeks(12);
        without.Weeks(12);
        Assert.True(with.Me.Ledger.Lifetime.Revenue > without.Me.Ledger.Lifetime.Revenue);
        Assert.True(with.Me.FtEarned > 0);
        Assert.True(with.Me.FtSpent > 0);
        LabState rivalWith = with.State.Labs.First(l => l.Id != with.Id);
        LabState rivalWithout = without.State.Labs.First(l => l.Id != without.Id);
        Assert.Equal(rivalWithout.Ledger.Lifetime.Revenue, rivalWith.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Shutting_it_costs_reputation_by_the_reach_and_resets_it()
    {
        Game g = Lab();
        g.Do(Command.OpenFreeTier(g.Id, 3));
        g.Weeks(8);
        long rep = g.Me.Reputation;
        long earned = g.Me.FtEarned;
        g.Do(Command.CloseFreeTier(g.Id));
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(0, g.Me.FtLevel);
        Assert.Equal(0, g.Me.FtReach);
        Assert.Equal(earned, g.Me.FtEarned);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.FreeTierClosed && e.Pick == 1);
        g.Refused(Command.CloseFreeTier(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Losing_the_model_closes_it_without_a_penalty()
    {
        Game g = Lab();
        g.Do(Command.OpenFreeTier(g.Id, 1));
        g.Weeks(5);
        long rep = g.Me.Reputation;
        g.Me.Models[0].Status = ModelStatus.Retired;
        g.Weeks(1);
        Assert.Equal(0, g.Me.FtLevel);
        Assert.Equal(rep, g.Me.Reputation);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.FreeTierClosed && e.Pick == 0);
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.OpenFreeTier(rival.Id, 1)).Ok);
        Game off = Lab(true, c => c.FreeTier.Enabled = 0);
        off.Refused(Command.OpenFreeTier(off.Id, 1), CommandError.InvalidArgument);
        Game s = Lab();
        s.Do(Command.OpenFreeTier(s.Id, 2));
        s.Weeks(6);
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 102, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == s.Id).FtLevel);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
