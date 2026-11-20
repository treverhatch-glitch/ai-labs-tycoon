using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Research points buy techs; compute trains models; a model is a member of the lab's family with its own numbers.</summary>
public class ResearchAndTrainingTests
{
    [Fact]
    public void Researchers_earn_research_points_every_week()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3);
        g.Weeks(5);
        Assert.Equal(5 * 3 * g.C.Research.PointsPerResearcherWeek, g.Me.ResearchPoints);
        Assert.Equal(3 * g.C.Research.PointsPerResearcherWeek, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void A_tech_costs_points_and_needs_its_prerequisites()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.ResearchTech(g.Id, "state-machines"), CommandError.NotEnoughResearch);

        g.GrantResearch(1000);
        g.Refused(Command.ResearchTech(g.Id, "pathfinding"), CommandError.MissingPrerequisite);
        g.Research("state-machines");
        g.Research("pathfinding");
        Assert.Equal(1000 - g.C.Tech("state-machines").Cost - g.C.Tech("pathfinding").Cost, g.Me.ResearchPoints);

        g.Refused(Command.ResearchTech(g.Id, "pathfinding"), CommandError.AlreadyResearched);
        g.Refused(Command.ResearchTech(g.Id, "time-travel"), CommandError.UnknownId);
    }

    [Fact]
    public void The_mid_line_is_locked_until_expert_system_shells_are_researched()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.StartTraining(g.Id, "mid", 0), CommandError.LineLocked);

        g.GrantResearch(5000);
        g.Research("rules-engines");
        g.Research("expert-shells");
        Assert.True(LabQueries.LineUnlocked(g.C, g.Me, "mid"));
        Assert.Equal("Fable Mid 1", g.Train("mid", 0).Name);
    }

    [Fact]
    public void A_training_run_takes_the_weeks_the_lab_was_told_it_would()
    {
        Game g = Game.Solo().WithStarterLab(engineers: 2, gpus: 8);
        int estimate = TrainingSystem.EstimateWeeks(g.C, g.State, g.Me, g.C.Line("mini"), 200);
        long cash = g.Me.Cash;

        ModelState model = g.Train("mini", 200);
        Assert.Equal(cash - g.C.Line("mini").PrepCost, g.Me.Cash);
        Assert.Equal(ModelStatus.Training, model.Status);
        Assert.Equal(0, model.Capability);

        int weeks = g.UntilTrained(model);
        Assert.Equal(estimate, weeks);
        Assert.Equal(ModelStatus.Trained, model.Status);
        Assert.Equal(g.Sim.Tick - 1, model.TrainedTick);
    }

    [Fact]
    public void A_models_capability_is_the_sum_of_parts_a_screen_can_show()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 10);
        g.GrantResearch(1000);
        g.Research("state-machines");
        g.Research("minimax");

        ModelState first = g.Train("mini", 100);
        g.UntilTrained(first);
        ModelState second = g.Train("mini", 100);
        g.UntilTrained(second);

        foreach (ModelState m in new[] { first, second })
            Assert.Equal(m.Capability, m.CapBase + m.CapTech + m.CapCarryover + m.CapLuck);

        ModelLineConfig mini = g.C.Line("mini");
        Assert.Equal(mini.BaseCapability, second.CapBase);
        Assert.Equal(mini.BaseCapability * 250 / 1000, second.CapTech);
        Assert.Equal(0, first.CapCarryover);
        Assert.Equal(first.Capability * g.C.Training.CarryoverPermille / 1000, second.CapCarryover);
        int aimedFor = second.CapBase + second.CapTech + second.CapCarryover;
        Assert.True(System.Math.Abs(second.CapLuck) <= aimedFor * g.C.Training.LuckPermille / 1000, "luck stays inside its band");
        Assert.True(second.Capability > first.Capability, "the second Mini builds on the first");
    }

    [Fact]
    public void Models_are_named_after_the_family_the_line_and_the_version()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 12);
        ModelState a = g.Train("mini", 0);
        ModelState b = g.Train("mini", 0);
        Assert.Equal("Fable Mini 1", a.Name);
        Assert.Equal("Fable Mini 2", b.Name);
        g.Refused(Command.StartTraining(g.Id, "mini", 0), CommandError.TooManyRuns);
    }

    [Fact]
    public void Two_runs_share_the_training_compute_so_each_goes_half_as_fast()
    {
        Game one = Game.Solo().WithStarterLab(gpus: 10);
        Game two = Game.Solo().WithStarterLab(gpus: 10);
        ModelState alone = one.Train("mini", 0);
        ModelState shared = two.Train("mini", 0);
        two.Train("mini", 0);

        one.Weeks(3);
        two.Weeks(3);
        Assert.InRange(shared.ProgressCompute, alone.ProgressCompute / 2 - 3, alone.ProgressCompute / 2 + 3);
        Assert.Equal(one.Me.LastWeek.TrainingCompute, two.Me.LastWeek.TrainingCompute);
    }

    [Fact]
    public void Engineers_and_cluster_tech_make_training_faster()
    {
        Game slow = Game.Solo().WithStarterLab(engineers: 0);
        Game fast = Game.Solo().WithStarterLab(engineers: 4);
        Assert.True(TrainingSystem.SpeedPermille(fast.C, fast.Me) > TrainingSystem.SpeedPermille(slow.C, slow.Me));

        int before = TrainingSystem.EstimateWeeks(fast.C, fast.State, fast.Me, fast.C.Line("mini"), 0);
        fast.GrantResearch(2000);
        fast.Research("beowulf-clusters");
        Assert.True(TrainingSystem.EstimateWeeks(fast.C, fast.State, fast.Me, fast.C.Line("mini"), 0) < before);

        // Engineers help up to a cap: a hundred of them are no faster than the cap allows.
        fast.Me.Staff.Find(s => s.RoleId == "engineer").Count = 100;
        Assert.Equal(1000 + 250 + fast.C.Training.EngineerBonusCapPermille, TrainingSystem.SpeedPermille(fast.C, fast.Me));
    }

    [Fact]
    public void Time_spent_on_alignment_fills_the_safety_meter_and_makes_the_run_longer()
    {
        Game rushed = Game.Solo().WithStarterLab(gpus: 10);
        Game careful = Game.Solo().WithStarterLab(gpus: 10);
        ModelState yolo = rushed.Train("mini", 0);
        ModelState checkedTwice = careful.Train("mini", 250);

        int rushedWeeks = rushed.UntilTrained(yolo);
        int carefulWeeks = careful.UntilTrained(checkedTwice);

        Assert.Equal(0, yolo.Alignment);
        Assert.True(checkedTwice.Alignment > 900, "a quarter of the run on alignment should nearly fill the meter, got " + checkedTwice.Alignment);
        Assert.True(carefulWeeks > rushedWeeks);
    }

    [Fact]
    public void Safety_staff_and_test_tech_get_more_alignment_out_of_the_same_compute()
    {
        Game plain = Game.Solo().WithStarterLab(gpus: 10);
        Game staffed = Game.Solo().WithStarterLab(gpus: 10, safety: 2);
        staffed.GrantResearch(500);
        staffed.Research("regression-tests");

        ModelState a = plain.Train("mini", 100);
        ModelState b = staffed.Train("mini", 100);
        plain.UntilTrained(a);
        staffed.UntilTrained(b);

        Assert.True(b.Alignment > a.Alignment + 100, "staffed " + b.Alignment + " vs plain " + a.Alignment);
        Assert.InRange(a.Alignment, 1, 999);
    }

    [Fact]
    public void A_run_cannot_be_all_alignment_and_needs_its_prep_cost()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.StartTraining(g.Id, "mini", 950), CommandError.InvalidArgument);
        g.Refused(Command.StartTraining(g.Id, "mega", 0), CommandError.UnknownId);
        g.Me.Cash = g.C.Line("mini").PrepCost - 1;
        g.Refused(Command.StartTraining(g.Id, "mini", 0), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Without_gpus_a_run_just_sits_there()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 0);
        ModelState model = g.Train("mini", 0);
        g.Weeks(10);
        Assert.Equal(ModelStatus.Training, model.Status);
        Assert.Equal(0, model.ProgressCompute);
        Assert.Equal(-1, TrainingSystem.EstimateWeeks(g.C, g.State, g.Me, g.C.Line("mini"), 0));
    }

    [Fact]
    public void Compute_is_split_between_training_and_customers_only_while_something_trains()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 10);
        g.Do(Command.SetTrainingShare(g.Id, 600));
        g.Weeks(1);
        Assert.Equal(100, g.Me.LastWeek.Compute);
        Assert.Equal(0, g.Me.LastWeek.TrainingCompute);
        Assert.Equal(100, g.Me.LastWeek.ServingCompute);

        g.Train("mini", 0);
        g.Weeks(1);
        Assert.Equal(60, g.Me.LastWeek.TrainingCompute);
        Assert.Equal(40, g.Me.LastWeek.ServingCompute);
        g.Refused(Command.SetTrainingShare(g.Id, 1001), CommandError.InvalidArgument);
    }
}
