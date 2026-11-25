using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LaunchWatchTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.New(5, c =>
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
        });
        g.Me.Models.Clear();
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static LabState Rival(Game g) { return g.State.Labs.First(l => l.Id != g.State.PlayerLabId); }

    static void Mine(Game g, int capability, int alignment = 500)
    {
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = capability, Status = ModelStatus.Released, Alignment = alignment, Name = "Ours" });
    }

    static void Launch(Game g, int capability)
    {
        LabState r = Rival(g);
        ModelState m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = capability, Status = ModelStatus.Released, Alignment = 500, Name = "Theirs" };
        r.Models.Add(m);
        Events.Add(g.State, EventKind.ModelReleased, r.Id, m.Id, 1, m.Name);
        g.State.Tick++;
        LaunchWatch.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void The_desk_costs_cash_then_a_weekly_bill_and_closing_stops_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.OpenWatchDesk(g.Id));
        Assert.Equal(cash - g.C.LaunchWatch.OpenCost, g.Me.Cash);
        g.Refused(Command.OpenWatchDesk(g.Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Do(Command.CloseWatchDesk(g.Id));
        Assert.Equal(0, g.Me.LwOn);
        g.Refused(Command.CloseWatchDesk(g.Id), CommandError.InvalidArgument);
        g.Me.Cash = 10;
        g.Refused(Command.OpenWatchDesk(g.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Without_a_desk_a_launch_is_not_noticed()
    {
        Game g = Lab();
        Mine(g, 300);
        Launch(g, 900);
        Assert.Equal(0, g.Me.LwRival);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.RivalLaunchSeen);
    }

    [Fact]
    public void A_launch_that_beats_the_lab_waits_for_an_answer_with_its_lead()
    {
        Game g = Lab();
        Mine(g, 500);
        g.Do(Command.OpenWatchDesk(g.Id));
        Launch(g, 1000);
        Assert.Equal(Rival(g).Id + 1, g.Me.LwRival);
        Assert.Equal(500, g.Me.LwLead);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalLaunchSeen && e.Pick == 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_launch_the_lab_already_beats_earns_a_little_reputation()
    {
        Game g = Lab();
        Mine(g, 900);
        g.Do(Command.OpenWatchDesk(g.Id));
        long rep = g.Me.Reputation;
        Launch(g, 400);
        Assert.Equal(0, g.Me.LwRival);
        Assert.True(g.Me.Reputation > rep);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalLaunchSeen && e.Pick == 0);
    }

    [Fact]
    public void Holding_takes_the_whole_loss_counter_programming_takes_less_for_cash()
    {
        Game hold = Lab();
        Mine(hold, 500);
        hold.Do(Command.OpenWatchDesk(hold.Id));
        Launch(hold, 1000);
        long rep = hold.Me.Reputation;
        hold.Do(Command.AnswerLaunch(hold.Id, 2));
        long held = rep - hold.Me.Reputation;
        Assert.True(held > 0);
        Assert.Equal(0, hold.Me.LwRival);
        Assert.Equal(1, hold.Me.LwHits);
        Game ctr = Lab();
        Mine(ctr, 500);
        ctr.Do(Command.OpenWatchDesk(ctr.Id));
        Launch(ctr, 1000);
        long cash = ctr.Me.Cash;
        long rep2 = ctr.Me.Reputation;
        long cost = LaunchWatch.CounterCost(ctr.C, ctr.Me);
        ctr.Do(Command.AnswerLaunch(ctr.Id, 1));
        Assert.Equal(cash - cost, ctr.Me.Cash);
        Assert.True(rep2 - ctr.Me.Reputation < held);
        Assert.Contains(ctr.State.Events, e => e.Kind == EventKind.LaunchAnswered && e.Pick == 1);
        ctr.Me.Cash = 0;
    }

    [Fact]
    public void Owning_the_safety_story_needs_a_safe_enough_model_and_wins_reputation()
    {
        Game weak = Lab();
        Mine(weak, 500, alignment: 100);
        weak.Do(Command.OpenWatchDesk(weak.Id));
        Launch(weak, 1000);
        weak.Refused(Command.AnswerLaunch(weak.Id, 3), CommandError.MissingPrerequisite);
        Game safe = Lab();
        Mine(safe, 500, alignment: 900);
        safe.Do(Command.OpenWatchDesk(safe.Id));
        Launch(safe, 1000);
        long rep = safe.Me.Reputation;
        safe.Do(Command.AnswerLaunch(safe.Id, 3));
        Assert.Equal(rep + safe.C.LaunchWatch.SafetyGain, safe.Me.Reputation);
        Assert.Equal(0, safe.Me.LwHits);
    }

    [Fact]
    public void An_unanswered_launch_lands_after_the_wait_and_a_bigger_lead_hurts_more()
    {
        Game g = Lab();
        Mine(g, 900);
        g.Do(Command.OpenWatchDesk(g.Id));
        Launch(g, 1000);
        int small = LaunchWatch.Loss(g.C, g.Me);
        long rep = g.Me.Reputation;
        for (int i = 0; i < g.C.LaunchWatch.WaitWeeks; i++) g.Weeks(1);
        Assert.Equal(0, g.Me.LwRival);
        Assert.True(g.Me.Reputation < rep);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LaunchAnswered && e.Pick == 0);
        Game big = Lab();
        big.Do(Command.OpenWatchDesk(big.Id));
        Launch(big, 1000);
        Assert.True(LaunchWatch.Loss(big.C, big.Me) > small);
        big.Do(Command.CloseWatchDesk(big.Id));
        Assert.Equal(0, big.Me.LwRival);
        Assert.Equal(1, big.Me.LwHits);
    }

    [Fact]
    public void In_a_real_game_the_desk_sees_rivals_and_changes_nothing_they_do()
    {
        Game with = Game.New(2);
        Game without = Game.New(2);
        with.Me.Cash = 1_000_000_000;
        without.Me.Cash = 1_000_000_000;
        with.Do(Command.OpenWatchDesk(with.Id));
        with.Autopilot(160);
        without.Autopilot(160);
        Assert.Contains(with.State.Events, e => e.Kind == EventKind.RivalLaunchSeen);
        foreach (LabState r in with.State.Labs.Where(l => l.Id != with.Id))
        {
            LabState other = without.State.Labs.First(l => l.Id == r.Id);
            Assert.Equal(other.Models.Count, r.Models.Count);
            Assert.Equal(other.Techs.Count, r.Techs.Count);
        }
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Lab();
        Assert.False(g.Try(Command.OpenWatchDesk(Rival(g).Id)).Ok);
        Game off = Lab(c => c.LaunchWatch.Enabled = 0);
        off.Refused(Command.OpenWatchDesk(off.Id), CommandError.InvalidArgument);
        Mine(g, 500);
        g.Do(Command.OpenWatchDesk(g.Id));
        Launch(g, 1000);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 103, g.Sim.Save());
        Simulation o = Simulation.Load(g.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == g.Id).LwOn);
        Assert.Empty(SaveGame.Problems(g.C, o.State));
    }
}
