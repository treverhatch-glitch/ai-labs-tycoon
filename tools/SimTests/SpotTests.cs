using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SpotTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(6, c =>
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
            c.Runs.EngineerSavingPermillePerStaff = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 2, safety: 1, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void A_lab_that_sets_nothing_pays_full_rent_and_is_never_preempted()
    {
        Game g = Lab(c => c.Spot.PreemptPermille = 1000);
        ModelState m = g.Train("mini", 0);
        g.Weeks(8);
        Assert.Equal(0, g.Me.SpotPreempts);
        Assert.Equal(0, g.Me.SpotSaved);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.SpotPreempted);
        Assert.True(m.ProgressCompute > 0);
    }

    [Fact]
    public void A_spot_share_cuts_the_rent_and_the_saving_adds_up()
    {
        Game g = Lab();
        long full = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        Assert.True(full > 0);
        g.Do(Command.SetSpotShare(g.Id, 500));
        long cut = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        Assert.Equal(full - full * 500 / 1000 * g.C.Spot.DiscountPermille / 1000, cut);
        g.Weeks(3);
        Assert.Equal((full - cut) * 3, g.Me.SpotSaved);
    }

    [Fact]
    public void The_share_is_checked_and_cannot_be_flipped_every_week()
    {
        Game g = Lab();
        g.Refused(Command.SetSpotShare(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetSpotShare(g.Id, g.C.Spot.MaxPermille + 1), CommandError.InvalidArgument);
        g.Do(Command.SetSpotShare(g.Id, 300));
        g.Refused(Command.SetSpotShare(g.Id, 100), CommandError.InvalidArgument);
        g.Do(Command.SetSpotShare(g.Id, 300));
        g.Weeks(g.C.Spot.ChangeWeeks);
        g.Do(Command.SetSpotShare(g.Id, 100));
        Assert.Equal(100, g.Me.SpotPermille);
        Game off = Lab(c => c.Spot.Enabled = 0);
        off.Refused(Command.SetSpotShare(off.Id, 100), CommandError.InvalidArgument);
    }

    static Game Stormy()
    {
        return Lab(c => { c.Spot.PreemptPermille = 1000; c.Spot.MaxPermille = 1000; c.RunQueue.MaxRush = 1; });
    }

    [Fact]
    public void A_reclaim_wave_interrupts_the_runs_that_are_not_rushed_by_their_checkpoint_habit()
    {
        Game g = Stormy();
        ModelState normal = g.Train("mini", 0);
        ModelState rushed = g.Train("mini", 0);
        g.Do(Command.SetCheckpoints(g.Id, normal.Id, "often"));
        g.Do(Command.SetRunPriority(g.Id, rushed.Id, 3));
        g.Weeks(2);
        g.Do(Command.SetSpotShare(g.Id, 1000));
        long before = normal.ProgressCompute;
        long rushedBefore = rushed.ProgressCompute;
        Assert.True(before > 0);
        Spot.RunWeek(g.C, g.State, g.Me);
        long share = g.C.Runs.Checkpoint("often").LossPermille;
        Assert.Equal(before - before * share / 1000, normal.ProgressCompute);
        Assert.Equal(rushedBefore, rushed.ProgressCompute);
        Assert.Equal(1, g.Me.SpotWaves);
        Assert.Equal(g.C.Spot.ReclaimWeeks, g.Me.SpotOut);
        Assert.Equal(1, g.Me.SpotPreempts);
        Assert.Equal(before * share / 1000, g.Me.SpotLost);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SpotPreempted && e.SubjectId == normal.Id);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.SpotPreempted && e.SubjectId == rushed.Id);
    }

    [Fact]
    public void During_a_wave_the_spot_share_of_a_run_does_no_work_but_a_rushed_run_keeps_all_of_it()
    {
        long[] Week(int share, bool wave)
        {
            Game g = Lab(c => { c.Spot.PreemptPermille = 0; c.RunQueue.MaxRush = 1; });
            ModelState a = g.Train("mini", 0);
            ModelState b = g.Train("mini", 0);
            g.Do(Command.SetRunPriority(g.Id, b.Id, 3));
            g.Weeks(1);
            g.Do(Command.SetSpotShare(g.Id, share));
            if (wave) g.Me.SpotOut = 2;
            long a0 = a.ProgressCompute, b0 = b.ProgressCompute;
            g.Weeks(1);
            return new[] { a.ProgressCompute - a0, b.ProgressCompute - b0 };
        }
        long[] calm = Week(0, false);
        long[] storm = Week(500, true);
        Assert.Equal(calm[1], storm[1]);
        Assert.True(storm[0] * 100 < calm[0] * 55);
        Assert.True(storm[0] * 100 > calm[0] * 45);
    }

    [Fact]
    public void A_wave_is_not_called_with_nothing_training_or_on_top_of_another()
    {
        Game g = Stormy();
        g.Do(Command.SetSpotShare(g.Id, 1000));
        g.Weeks(5);
        Assert.Equal(0, g.Me.SpotWaves);
        g.Train("mini", 0);
        g.Weeks(1);
        Assert.Equal(1, g.Me.SpotWaves);
        Assert.Equal(g.C.Spot.ReclaimWeeks, g.Me.SpotOut);
        g.Weeks(1);
        Assert.Equal(1, g.Me.SpotWaves);
        Assert.Equal(g.C.Spot.ReclaimWeeks - 1, g.Me.SpotOut);
    }

    [Fact]
    public void Without_a_habit_set_the_default_loss_applies_and_often_loses_less()
    {
        Game g = Stormy();
        ModelState m = g.Train("mini", 0);
        Assert.Equal(g.C.Spot.DefaultLossPermille, Spot.LossShare(g.C, m));
        g.Do(Command.SetCheckpoints(g.Id, m.Id, "often"));
        Assert.True(Spot.LossShare(g.C, m) < g.C.Spot.DefaultLossPermille);
    }

    [Fact]
    public void Engineers_cut_the_chance_of_preemption()
    {
        Game g = Lab(c => { c.Runs.EngineerSavingPermillePerStaff = 100; c.Runs.EngineerSavingCapPermille = 500; });
        g.Do(Command.SetSpotShare(g.Id, 600));
        int chance = Spot.PreemptChance(g.C, g.Me);
        Assert.True(TrainingRuns.EngineerSavingPermille(g.C, g.Me) > 0);
        Assert.Equal(g.C.Spot.PreemptPermille * 600L / 1000 * (1000 - TrainingRuns.EngineerSavingPermille(g.C, g.Me)) / 1000, chance);
        Assert.True(chance < g.C.Spot.PreemptPermille * 600 / 1000);
    }

    [Fact]
    public void The_same_seed_gives_the_same_preemptions()
    {
        long Run()
        {
            Game g = Lab(c => c.Spot.PreemptPermille = 400);
            g.Train("mini", 0);
            g.Do(Command.SetSpotShare(g.Id, 600));
            g.Weeks(20);
            return g.Me.SpotLost * 1000 + g.Me.SpotPreempts;
        }
        Assert.Equal(Run(), Run());
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_run_no_spot()
    {
        Game g = Lab();
        g.Train("mini", 0);
        g.Do(Command.SetSpotShare(g.Id, 500));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 118, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.SpotPermille + lab.SpotSaved + lab.SpotPreempts + lab.SpotOut + lab.SpotWaves);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
