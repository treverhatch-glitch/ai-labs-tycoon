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

/// <summary>The third way to split compute (bible ch. 3): some of it goes to research experiments, and research goes faster for it.</summary>
public class ResearchShareTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Nothing_is_set_aside_until_the_player_says_so()
    {
        Game g = Lab();
        Assert.Equal(0, g.Me.ResearchSharePermille);
        Assert.Equal(0, TrainingSystem.ComputeShareBonusPermille(g.C, g.Me));
    }

    [Fact]
    public void The_command_sets_the_share_inside_the_limit()
    {
        Game g = Lab();
        g.Do(Command.SetResearchShare(g.Id, 200));
        Assert.Equal(200, g.Me.ResearchSharePermille);
        g.Do(Command.SetResearchShare(g.Id, g.C.Research.MaxComputeSharePermille));
        Assert.Equal(300, g.Me.ResearchSharePermille);
        g.Do(Command.SetResearchShare(g.Id, 0));
        Assert.Equal(0, g.Me.ResearchSharePermille);
    }

    [Fact]
    public void A_share_over_the_limit_or_below_zero_is_refused_and_changes_nothing()
    {
        Game g = Lab();
        g.Do(Command.SetResearchShare(g.Id, 100));
        g.Refused(Command.SetResearchShare(g.Id, 301), CommandError.InvalidArgument);
        g.Refused(Command.SetResearchShare(g.Id, -1), CommandError.InvalidArgument);
        Assert.Equal(100, g.Me.ResearchSharePermille);
    }

    [Fact]
    public void Only_the_players_lab_may_set_it()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        CommandResult r = g.Try(Command.SetResearchShare(rival.Id, 100));
        Assert.False(r.Ok);
        Assert.Equal(0, rival.ResearchSharePermille);
    }

    [Fact]
    public void The_share_comes_off_general_compute_before_training_and_customers_divide_it()
    {
        Game g = Lab();
        long before = LabQueries.Split(g.C, g.State, g.Me, false).General;
        Assert.True(before > 0);
        g.Do(Command.SetResearchShare(g.Id, 200));
        Assert.Equal(MathI.MulPermille(before, 800), LabQueries.Split(g.C, g.State, g.Me, false).General);
    }

    [Fact]
    public void Inference_chips_are_not_touched_by_the_share()
    {
        Game g = Lab();
        long inference = LabQueries.Split(g.C, g.State, g.Me, false).Inference;
        g.Do(Command.SetResearchShare(g.Id, 300));
        Assert.Equal(inference, LabQueries.Split(g.C, g.State, g.Me, false).Inference);
    }

    [Fact]
    public void Research_goes_faster_by_the_share_times_the_bonus()
    {
        Game g = Lab();
        long plain = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(plain > 0);
        g.Do(Command.SetResearchShare(g.Id, 100));
        Assert.Equal(120, TrainingSystem.ComputeShareBonusPermille(g.C, g.Me));
        Assert.Equal(MathI.MulPermille(plain, 1120), TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void A_full_share_gives_the_biggest_bonus_and_no_more()
    {
        Game g = Lab();
        g.Me.ResearchSharePermille = 300;
        Assert.Equal(360, TrainingSystem.ComputeShareBonusPermille(g.C, g.Me));
        g.Me.ResearchSharePermille = 900;
        Assert.Equal(360, TrainingSystem.ComputeShareBonusPermille(g.C, g.Me));
    }

    [Fact]
    public void A_lab_with_no_researchers_gains_nothing_from_the_share()
    {
        Game g = Lab();
        g.Me.Staff.RemoveAll(p => g.C.Role(p.RoleId).Function == RoleFunction.Research);
        g.Do(Command.SetResearchShare(g.Id, 300));
        Assert.Equal(0, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void A_training_run_gets_less_compute_while_the_share_is_set()
    {
        Game g = Lab();
        g.Me.TrainingSharePermille = 1000;
        g.Train("mini", 100);
        long full = TrainingSystem.TrainingCompute(g.C, g.State, g.Me);
        Assert.True(full > 0);
        g.Do(Command.SetResearchShare(g.Id, 300));
        Assert.Equal(MathI.MulPermille(full, 700), TrainingSystem.TrainingCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Weeks_of_research_add_up_faster_with_a_share_set()
    {
        long Weeks(int share)
        {
            Game g = Lab();
            g.Do(Command.SetResearchShare(g.Id, share));
            long start = g.Me.ResearchPoints;
            g.Weeks(20);
            return g.Me.ResearchPoints - start;
        }
        Assert.True(Weeks(300) > Weeks(0));
    }

    [Fact]
    public void The_share_is_saved_and_loads_back_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetResearchShare(g.Id, 250));
        g.Weeks(30);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(250, back.Player.ResearchSharePermille);
    }

    [Fact]
    public void A_schema_41_save_loads_with_nothing_set_aside()
    {
        Game g = Lab();
        g.Weeks(4);
        string old = SaveSchemaPretend.At(g.C, 41, g.Sim.Save());
        Assert.DoesNotContain("researchSharePermille", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.ResearchSharePermille);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_share_over_the_limit_is_reported()
    {
        Game g = Lab();
        g.Me.ResearchSharePermille = 301;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("set aside for research"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(300, c.Research.MaxComputeSharePermille);
        Assert.Equal(1200, c.Research.ComputeShareBonusPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Research.MaxComputeSharePermille = 1001;
        c.Research.ComputeShareBonusPermille = -1;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("maxComputeSharePermille"));
        Assert.Contains(problems, p => p.Contains("computeShareBonusPermille"));
    }
}
