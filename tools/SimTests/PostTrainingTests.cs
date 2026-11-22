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

public class PostTrainingTests
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
            c.Shocks.Enabled = 0;
            c.PostTraining.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 1, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.DataClean4 = 500;
        g.Me.DataClean2 = 100;
        g.Me.DataClean1 = 100;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    [Fact]
    public void A_model_nobody_tunes_goes_on_sale_as_it_is()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Equal(0, m.PostKind + m.PostPref + m.PostSafety);
        Assert.Equal(0, PostTraining.NetLift(g.C, m));
        Assert.Equal(0, PostTraining.IncidentPermille(g.C, m));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.PostTrainingStarted);
    }

    [Fact]
    public void A_tuning_run_is_checked()
    {
        Game g = Lab();
        g.Refused(Command.PostTrain(g.Id, 999, 1), CommandError.UnknownId);
        ModelState m = Trained(g);
        g.Refused(Command.PostTrain(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.PostTrain(g.Id, m.Id, 3), CommandError.InvalidArgument);
        g.Me.DataClean4 = g.C.PostTraining.Units - 1;
        g.Refused(Command.PostTrain(g.Id, m.Id, 1), CommandError.MissingPrerequisite);
        g.Me.DataClean4 = 500;
        g.Me.Cash = 10;
        g.Refused(Command.PostTrain(g.Id, m.Id, 1), CommandError.NotEnoughCash);
        g.Me.Cash = 1_000_000_000_000;
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Refused(Command.PostTrain(g.Id, m.Id, 2), CommandError.InvalidArgument);
        Game late = Lab(c => c.PostTraining.MinEra = 99);
        ModelState lm = Trained(late);
        late.Refused(Command.PostTrain(late.Id, lm.Id, 1), CommandError.EraLocked);
        Game off = Lab(c => c.PostTraining.Enabled = 0);
        ModelState om = Trained(off);
        off.Refused(Command.PostTrain(off.Id, om.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_run_uses_data_and_cash_takes_weeks_and_blocks_the_release()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        Assert.Equal(500 - g.C.PostTraining.Units, g.Me.DataClean4);
        Assert.Equal(before - g.C.PostTraining.Cost, g.Me.Ledger.Lifetime.Training);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"), CommandError.InvalidArgument);
        g.Weeks(g.C.PostTraining.Weeks - 1);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"), CommandError.InvalidArgument);
        g.Weeks(1);
        Assert.Equal(0, m.PostKind);
        Assert.Equal(1, m.PostPref);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PostTrainingDone && e.Pick == 1);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Abandoning_a_run_frees_the_model_and_the_data_is_gone()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        g.Weeks(1);
        g.Do(Command.AbandonPostTraining(g.Id, m.Id));
        Assert.Equal(0, m.PostKind);
        Assert.Equal(0, m.PostSafety);
        Assert.Equal(200 - g.C.PostTraining.Units, g.Me.DataClean2 + g.Me.DataClean1);
        g.Refused(Command.AbandonPostTraining(g.Id, m.Id), CommandError.InvalidArgument);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
    }

    [Fact]
    public void Safety_tuning_raises_the_alignment_by_less_each_time_and_takes_licensed_data_first()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        int a0 = m.Alignment;
        int shown0 = m.DisplayedAlignment;
        g.Me.DataClean2 = 10;
        g.Me.DataClean1 = 200;
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        Assert.Equal(0, g.Me.DataClean2);
        Assert.Equal(200 - (g.C.PostTraining.Units - 10), g.Me.DataClean1);
        g.Weeks(g.C.PostTraining.Weeks);
        int first = m.Alignment - a0;
        Assert.Equal(Math.Min(1000 - a0, g.C.PostTraining.SafetyGain), first);
        Assert.Equal(m.Alignment - a0, m.DisplayedAlignment - shown0);
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        g.Weeks(g.C.PostTraining.Weeks);
        Assert.Equal(2, m.PostSafety);
        if (m.Alignment < 1000) Assert.True(m.Alignment - a0 - first < first);
    }

    [Fact]
    public void A_model_takes_only_so_many_runs_of_each_kind()
    {
        Game g = Lab(c => { c.PostTraining.MaxPasses = 2; c.PostTraining.Weeks = 1; });
        ModelState m = Trained(g);
        for (int i = 0; i < 2; i++)
        {
            g.Do(Command.PostTrain(g.Id, m.Id, 1));
            g.Weeks(1);
        }
        g.Refused(Command.PostTrain(g.Id, m.Id, 1), CommandError.InvalidArgument);
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        Assert.Equal(2, m.PostPref);
    }

    [Fact]
    public void Preference_runs_lift_revenue_by_less_each_time_and_safety_runs_cost_it()
    {
        Game g = Lab(c => c.PostTraining.Weeks = 1);
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(1);
        int one = PostTraining.NetLift(g.C, m);
        Assert.Equal(g.C.PostTraining.PrefLift, one);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(1);
        int two = PostTraining.NetLift(g.C, m);
        Assert.True(two > one);
        Assert.True(two - one < one);
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        g.Weeks(1);
        Assert.Equal(two - g.C.PostTraining.Refuse, PostTraining.NetLift(g.C, m));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Weeks(2);
        long rev = MarketSystem.ModelRevenue(g.State, g.Id, m.Id);
        Assert.Equal(rev * PostTraining.NetLift(g.C, m) / 1000, PostTraining.Lift(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_net_change_never_falls_below_the_floor()
    {
        Game g = Lab(c => { c.PostTraining.Refuse = 400; c.PostTraining.Weeks = 1; c.PostTraining.MaxPasses = 4; });
        ModelState m = Trained(g);
        for (int i = 0; i < 4; i++)
        {
            g.Do(Command.PostTrain(g.Id, m.Id, 2));
            g.Weeks(1);
        }
        Assert.Equal(g.C.PostTraining.MinLift, PostTraining.NetLift(g.C, m));
    }

    [Fact]
    public void Preference_tuning_makes_the_model_fail_in_front_of_customers_more_often()
    {
        Game g = Lab(c => c.PostTraining.Weeks = 1);
        ModelState m = Trained(g);
        Game twin = Lab(c => c.PostTraining.Weeks = 1);
        ModelState tm = Trained(twin);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(1);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(1);
        Assert.Equal(2 * g.C.PostTraining.Syco, PostTraining.IncidentPermille(g.C, m));
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) > SafetySystem.WeeklyChancePermille(twin.C, twin.State, twin.Me, tm));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_tuning()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 121, g.Sim.Save()));
        ModelState om = older.State.Labs.First(l => l.Id == g.Id).Models.First(x => x.Id == m.Id);
        Assert.Equal(0, om.PostKind + om.PostLeft + om.PostPref + om.PostSafety);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
