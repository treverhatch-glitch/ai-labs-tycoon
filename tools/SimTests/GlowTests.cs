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

/// <summary>The glow: the building shows the worst model's real risk, even when the dashboard looks calmer.</summary>
public class GlowTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c =>
        {
            foreach (EraConfig era in c.Eras) era.IncidentLoadPermille = 1000;
            if (tweak != null) tweak(c);
        });
        g.Me.Models.Clear();
        return g;
    }

    static ModelState Model(Game g, int alignment, int displayed = -1, int corruption = 0, long capability = 100, ModelStatus status = ModelStatus.Released, string line = "mini", int version = 1)
    {
        var m = new ModelState
        {
            Id = g.State.TakeId(), LineId = line, Version = version, Name = "m" + version, Status = status, Capability = (int)capability,
            Alignment = alignment, DisplayedAlignment = displayed < 0 ? alignment : displayed, Corruption = corruption, LastIncidentTick = -1
        };
        g.Me.Models.Add(m);
        return m;
    }

    static Glow.Report Of(Game g) => Glow.Of(g.C, g.State, g.Me);

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    [Fact]
    public void The_table_has_seven_stages_that_follow_the_ladder_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "calm", "hum", "flicker", "empty-corridor", "doors-locked", "power-dip", "quiet" }, c.Glow.Stages.Select(s => s.IdValue).ToArray());
        Assert.Equal(10000, c.Glow.FullAtCapability);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(9, 1, 0)]
    [InlineData(10, 1, 1)]
    [InlineData(1000, 1, 1)]
    [InlineData(0, 2, 2)]
    [InlineData(0, 3, 3)]
    [InlineData(0, 4, 4)]
    [InlineData(0, 5, 5)]
    [InlineData(0, 6, 6)]
    [InlineData(500, 6, 6)]
    public void A_chance_and_a_rung_pick_the_highest_stage_that_applies(int chance, int rung, int stage)
    {
        Assert.Equal(stage, Glow.StageFor(TestData.Config(), chance, rung));
    }

    [Fact]
    public void A_lab_with_no_model_to_read_has_no_glow()
    {
        Game g = Lab();
        Glow.Report r = Of(g);
        Assert.Null(r.StageId);
        Assert.Equal("", r.Sign ?? "");
        Assert.Equal(-1, r.WorstModelId);
        Assert.Equal(0, r.IntensityPermille);
        Assert.False(r.ChartIsMisleading);
    }

    [Fact]
    public void A_safe_model_leaves_the_building_calm()
    {
        Game g = Lab();
        Model(g, 1000);
        Glow.Report r = Of(g);
        Assert.Equal("calm", r.StageId);
        Assert.Equal(0, r.StageIndex);
        Assert.Equal("calm", r.DashboardStageId);
        Assert.False(r.ChartIsMisleading);
        Assert.Contains("hums", r.Sign);
    }

    [Fact]
    public void A_poor_alignment_makes_the_lamps_flicker()
    {
        Game g = Lab();
        ModelState m = Model(g, 300);
        Glow.Report r = Of(g);
        Assert.Equal("flicker", r.StageId);
        Assert.Equal(m.Id, r.WorstModelId);
        Assert.Equal(2, r.WorstRung);
        Assert.False(r.ChartIsMisleading);
    }

    [Fact]
    public void A_middling_alignment_hums_before_it_flickers()
    {
        Game g = Lab();
        Model(g, 600);
        Assert.Equal("hum", Of(g).StageId);
    }

    [Fact]
    public void A_model_that_sandbags_empties_the_corridor_while_the_chart_looks_fine()
    {
        Game g = Lab();
        Model(g, 400, displayed: 800);
        Glow.Report r = Of(g);
        Assert.Equal(3, r.WorstRung);
        Assert.Equal("empty-corridor", r.StageId);
        Assert.Equal("calm", r.DashboardStageId);
        Assert.True(r.ChartIsMisleading);
    }

    [Fact]
    public void A_corrupted_model_locks_the_doors_and_the_chart_does_not_see_it()
    {
        Game g = Lab();
        Model(g, 900, corruption: 300);
        Glow.Report r = Of(g);
        Assert.Equal("doors-locked", r.StageId);
        Assert.Equal(4, r.WorstRung);
        Assert.True(r.StageIndex > r.DashboardStageIndex);
        Assert.True(r.ChartIsMisleading);
    }

    [Theory]
    [InlineData(250, "doors-locked")]
    [InlineData(450, "power-dip")]
    [InlineData(700, "quiet")]
    public void The_corruption_rungs_open_the_last_stages(int corruption, string stage)
    {
        Game g = Lab();
        Model(g, 900, corruption: corruption);
        Assert.Equal(stage, Of(g).StageId);
    }

    [Fact]
    public void An_honest_model_reads_the_same_in_the_room_and_on_the_chart()
    {
        Game g = Lab();
        Model(g, 300);
        Glow.Report r = Of(g);
        Assert.Equal(r.StageIndex, r.DashboardStageIndex);
        Assert.False(r.ChartIsMisleading);
    }

    [Fact]
    public void The_worst_model_sets_the_glow()
    {
        Game g = Lab();
        Model(g, 1000, version: 1);
        ModelState bad = Model(g, 200, version: 2);
        Model(g, 800, version: 3);
        Glow.Report r = Of(g);
        Assert.Equal(bad.Id, r.WorstModelId);
        Assert.Equal("flicker", r.StageId);
    }

    [Fact]
    public void A_model_still_training_or_retired_does_not_count()
    {
        Game g = Lab();
        Model(g, 1000);
        Model(g, 0, status: ModelStatus.Training, version: 2);
        Model(g, 0, status: ModelStatus.Retired, version: 3);
        Assert.Equal("calm", Of(g).StageId);
    }

    [Fact]
    public void A_trained_model_that_is_not_on_sale_still_shows()
    {
        Game g = Lab();
        Model(g, 100, status: ModelStatus.Trained);
        Assert.Equal("flicker", Of(g).StageId);
    }

    [Fact]
    public void Intensity_follows_the_latest_model_of_each_line_up_to_a_ceiling()
    {
        Game g = Lab();
        Model(g, 1000, capability: 1000, version: 1);
        Model(g, 1000, capability: 2500, version: 2);
        Glow.Report r = Of(g);
        Assert.Equal(2500, r.Capability);
        Assert.Equal(250, r.IntensityPermille);
        Model(g, 1000, capability: 4000, line: "mid", version: 1);
        Assert.Equal(650, Of(g).IntensityPermille);
        Model(g, 1000, capability: 50000, line: "flagship", version: 1);
        Assert.Equal(1000, Of(g).IntensityPermille);
    }

    [Fact]
    public void The_glow_does_not_change_the_state()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        string before = g.Sim.Digest();
        Of(g);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void Every_lab_in_a_played_game_has_a_glow_with_a_sign()
    {
        Game g = Game.New(seed: 3);
        g.Autopilot(3 * 48);
        foreach (LabState lab in g.State.Labs.Where(l => l.Models.Any(m => m.Status == ModelStatus.Released || m.Status == ModelStatus.Trained)))
        {
            Glow.Report r = Glow.Of(g.C, g.State, lab);
            Assert.NotNull(r.StageId);
            Assert.NotEmpty(r.Sign);
            Assert.InRange(r.IntensityPermille, 0, 1000);
        }
    }

    [Fact]
    public void The_validator_names_each_glow_number_that_is_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.Glow.FullAtCapability = 0), "glow.fullAtCapability");
        AssertProblem(Problems(c => c.Glow.Stages.Clear()), "glow.stages is empty");
        AssertProblem(Problems(c => c.Glow.Stages[1].MinChancePermille = 1001), "minChancePermille");
        AssertProblem(Problems(c => c.Glow.Stages[2].MinRung = 7), "minRung");
        AssertProblem(Problems(c => { c.Glow.Stages[2].MinRung = 0; }), "never applies");
        AssertProblem(Problems(c => c.Glow.Stages[0].MinChancePermille = 5), "first glow stage");
        AssertProblem(Problems(c => c.Glow.Stages[1].MinChancePermille = 0), "does not start above the chance");
        AssertProblem(Problems(c => c.Glow.Stages[3].MinRung = 2), "does not start above the rung");
        AssertProblem(Problems(c => c.Glow.Stages[1].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Glow.Stages[1].Sign = ""), "has no sign");
        AssertProblem(Problems(c => c.Glow.Stages[1].IdValue = "calm"), "glow.stages");
    }
}
