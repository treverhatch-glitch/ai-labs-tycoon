using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class RunQueueTests
{
    static Game Lab(int runs = 3, System.Action<GameConfig> more = null)
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
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 40);
        g.Me.Models.Clear();
        for (int i = 0; i < runs; i++)
            g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Status = ModelStatus.Training, Name = "Run" + i, RequiredCompute = 1_000_000_000_000, Alignment = 500 });
        g.Me.TrainingSharePermille = 1000;
        g.Me.Cash = 1_000_000_000;
        return g;
    }

    static long[] Gains(Game g)
    {
        long[] before = g.Me.Models.Select(m => m.ProgressCompute).ToArray();
        g.Weeks(1);
        return g.Me.Models.Select((m, i) => m.ProgressCompute - before[i]).ToArray();
    }

    [Fact]
    public void Without_levels_every_run_gets_an_equal_share_as_before()
    {
        Game g = Lab();
        long[] gain = Gains(g);
        Assert.True(gain.All(x => x > 0));
        Assert.True(gain.Max() - gain.Min() <= 3);
    }

    [Fact]
    public void A_rush_run_gets_more_and_a_background_run_less_in_proportion_to_the_weights()
    {
        Game g = Lab();
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 3));
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[2].Id, 1));
        long[] gain = Gains(g);
        Assert.True(gain[0] > gain[1] && gain[1] > gain[2]);
        Assert.InRange(gain[0] / (double)gain[1], g.C.RunQueue.WeightRush / (double)g.C.RunQueue.WeightNormal - 0.05, g.C.RunQueue.WeightRush / (double)g.C.RunQueue.WeightNormal + 0.05);
    }

    [Fact]
    public void Queueing_moves_compute_between_runs_and_loses_none()
    {
        Game plain = Lab();
        long[] equal = Gains(plain);
        Game g = Lab();
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 3));
        long[] gain = Gains(g);
        Assert.True(gain[0] > equal[0]);
        Assert.True(gain[1] < equal[1]);
        Assert.True(System.Math.Abs(gain.Sum() - equal.Sum()) <= 3);
    }

    [Fact]
    public void Changing_a_level_costs_part_of_the_runs_progress()
    {
        Game g = Lab();
        ModelState m = g.Me.Models[1];
        m.ProgressCompute = 1_000_000;
        g.Do(Command.SetRunPriority(g.Id, m.Id, 3));
        Assert.Equal(1_000_000 - MathI.MulPermille(1_000_000, g.C.RunQueue.SwitchCostPermille), m.ProgressCompute);
        Assert.Equal(MathI.MulPermille(1_000_000, g.C.RunQueue.SwitchCostPermille), g.State.Events.Last(e => e.Kind == EventKind.RunPriorityChanged).Amount);
        Assert.Equal(3, g.State.Events.Last(e => e.Kind == EventKind.RunPriorityChanged).Pick);
    }

    [Fact]
    public void Only_so_many_runs_may_be_rushed_and_a_run_cannot_be_set_to_where_it_is()
    {
        Game g = Lab();
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 3));
        g.Refused(Command.SetRunPriority(g.Id, g.Me.Models[1].Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetRunPriority(g.Id, g.Me.Models[1].Id, 2), CommandError.InvalidArgument);
        g.Refused(Command.SetRunPriority(g.Id, g.Me.Models[1].Id, 0), CommandError.InvalidArgument);
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 1));
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[1].Id, 3));
    }

    [Fact]
    public void Only_a_run_in_training_and_a_known_model_can_be_set()
    {
        Game g = Lab();
        g.Me.Models[2].Status = ModelStatus.Trained;
        g.Refused(Command.SetRunPriority(g.Id, g.Me.Models[2].Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetRunPriority(g.Id, 99999, 3), CommandError.UnknownId);
    }

    [Fact]
    public void A_background_run_still_finishes_when_the_others_are_done()
    {
        Game g = Lab(2);
        g.Do(Command.SetRunPriority(g.Id, g.Me.Models[0].Id, 1));
        g.Me.Models[1].Status = ModelStatus.Trained;
        long before = g.Me.Models[0].ProgressCompute;
        g.Weeks(1);
        Assert.True(g.Me.Models[0].ProgressCompute > before);
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.SetRunPriority(rival.Id, 1, 3)).Ok);
        Game off = Lab(2, c => c.RunQueue.Enabled = 0);
        off.Refused(Command.SetRunPriority(off.Id, off.Me.Models[0].Id, 3), CommandError.InvalidArgument);
        Game s = Lab();
        s.Do(Command.SetRunPriority(s.Id, s.Me.Models[0].Id, 3));
        s.Weeks(3);
        Assert.Empty(SaveGame.Problems(s.C, s.State));
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 105, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == s.Id).Models[0].QueueLevel);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
