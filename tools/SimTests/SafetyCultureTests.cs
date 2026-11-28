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

/// <summary>Safety culture and the release gate: a lab that listens catches more, and a rule it sets for itself has to be obeyed.</summary>
public class SafetyCultureTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.RolePerks.Clear();
    }

    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c => { Quiet(c); if (tweak != null) tweak(c); }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static void SetLevel(Game g, int level) => g.Me.CultureShift = level - g.C.Culture.Start;

    static int Level(Game g) => SafetyCulture.Level(g.C, g.Me);

    static void Staff(Game g, string role, int count)
    {
        for (int i = 0; i < count; i++) g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 });
    }

    static ModelState Trained(Game g, int alignment)
    {
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        model.Alignment = alignment;
        model.DisplayedAlignment = alignment;
        return model;
    }

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

    // ---- the table

    [Fact]
    public void Culture_starts_at_five_hundred_and_changes_nothing_there()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(500, c.Culture.Start);
        Assert.Equal(500, c.Culture.NeutralLevel);
        Game g = Lab();
        Assert.Equal(500, Level(g));
        Assert.Equal(0, SafetyCulture.CatchPermille(g.C, g.Me));
        Assert.Equal("steady", SafetyCulture.Of(g.C, g.Me).BandId);
        Assert.Equal(0, g.Me.ReleaseGate);
    }

    [Fact]
    public void Five_bands_name_the_range_and_the_listening_roles_exist()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "cover-up", "hurried", "steady", "careful", "open-book" }, c.Culture.Bands.Select(b => b.Id).ToArray());
        Assert.Equal(new[] { "safety", "red-teamer", "alignment-fellow" }, c.Culture.ListeningRoles.ToArray());
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Theory]
    [InlineData(0, "cover-up")]
    [InlineData(199, "cover-up")]
    [InlineData(200, "hurried")]
    [InlineData(399, "hurried")]
    [InlineData(400, "steady")]
    [InlineData(599, "steady")]
    [InlineData(600, "careful")]
    [InlineData(799, "careful")]
    [InlineData(800, "open-book")]
    [InlineData(1000, "open-book")]
    public void A_level_has_the_band_whose_floor_is_the_highest_one_below_it(int level, string band)
    {
        GameConfig c = TestData.Config();
        Assert.Equal(band, SafetyCulture.Band(c, level).Id);
    }

    // ---- the validator

    [Fact]
    public void The_validator_names_each_culture_number_that_is_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.Culture.Start = -1), "culture.start");
        AssertProblem(Problems(c => c.Culture.Start = 1001), "culture.start");
        AssertProblem(Problems(c => c.Culture.NeutralLevel = 1001), "culture.neutralLevel");
        AssertProblem(Problems(c => c.Culture.CatchSlopePermille = -1), "culture.catchSlopePermille");
        AssertProblem(Problems(c => c.Culture.CatchSlopePermille = 1001), "culture.catchSlopePermille");
        AssertProblem(Problems(c => c.Culture.StaffGainPerPerson = 101), "culture.staffGainPerPerson");
        AssertProblem(Problems(c => c.Culture.MaxStaffGain = 1001), "culture.maxStaffGain");
        AssertProblem(Problems(c => c.Culture.DriftPerWeek = 101), "culture.driftPerWeek");
        AssertProblem(Problems(c => c.Culture.RushBelowAlignment = 1001), "culture.rushBelowAlignment");
        AssertProblem(Problems(c => c.Culture.RushPoints = -1), "culture.rushPoints");
        AssertProblem(Problems(c => c.Culture.GateKeptPoints = -1), "culture.gateKeptPoints");
        AssertProblem(Problems(c => c.Culture.GateLoweredPoints = -1), "culture.gateLoweredPoints");
        AssertProblem(Problems(c => c.Culture.ComplaintAnsweredPoints = -1), "culture.complaintAnsweredPoints");
        AssertProblem(Problems(c => c.Culture.ComplaintLapsedPoints = -1), "culture.complaintLapsedPoints");
        AssertProblem(Problems(c => c.Culture.MaxGate = 1001), "culture.maxGate");
        AssertProblem(Problems(c => c.Culture.GateLockWeeks = 521), "culture.gateLockWeeks");
        AssertProblem(Problems(c => c.Culture.ListeningRoles.Add("no-such-role")), "unknown role");
    }

    [Fact]
    public void The_validator_names_each_way_the_bands_can_be_wrong()
    {
        AssertProblem(Problems(c => c.Culture.Bands.Clear()), "bands is empty");
        AssertProblem(Problems(c => c.Culture.Bands[0].Min = 10), "first culture band must start at 0");
        AssertProblem(Problems(c => c.Culture.Bands[2].Min = c.Culture.Bands[1].Min), "does not start above");
        AssertProblem(Problems(c => c.Culture.Bands[1].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Culture.Bands[1].Blurb = ""), "has no blurb");
        AssertProblem(Problems(c => c.Culture.Bands[4].Min = 1001), "must be from 0 to 1000");
        AssertProblem(Problems(c => c.Culture.Bands[1].IdValue = "cover-up"), "twice");
    }

    [Fact]
    public void The_edges_of_every_range_are_allowed()
    {
        Assert.Empty(Problems(c =>
        {
            CultureConfig u = c.Culture;
            u.Start = 0; u.NeutralLevel = 0; u.CatchSlopePermille = 0; u.StaffGainPerPerson = 0; u.MaxStaffGain = 0; u.DriftPerWeek = 0;
            u.RushBelowAlignment = 0; u.RushPoints = 0; u.GateKeptPoints = 0; u.GateLoweredPoints = 0; u.ComplaintAnsweredPoints = 0;
            u.ComplaintLapsedPoints = 0; u.MaxGate = 0; u.GateLockWeeks = 0; u.ListeningRoles.Clear();
        }));
        Assert.Empty(Problems(c =>
        {
            CultureConfig u = c.Culture;
            u.Start = 1000; u.NeutralLevel = 1000; u.CatchSlopePermille = 1000; u.StaffGainPerPerson = 100; u.MaxStaffGain = 1000; u.DriftPerWeek = 100;
            u.RushBelowAlignment = 1000; u.RushPoints = 1000; u.GateKeptPoints = 1000; u.GateLoweredPoints = 1000; u.ComplaintAnsweredPoints = 1000;
            u.ComplaintLapsedPoints = 1000; u.MaxGate = 1000; u.GateLockWeeks = 520;
        }));
    }

    // ---- level and catch

    [Fact]
    public void Culture_never_leaves_the_range_whatever_moves_it()
    {
        Game g = Lab();
        SafetyCulture.Move(g.C, g.Me, 100_000);
        Assert.Equal(1000, Level(g));
        SafetyCulture.Move(g.C, g.Me, -100_000);
        Assert.Equal(0, Level(g));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_move_of_nothing_changes_nothing()
    {
        Game g = Lab();
        string digest = g.Sim.Digest();
        SafetyCulture.Move(g.C, g.Me, 0);
        Assert.Equal(digest, g.Sim.Digest());
    }

    [Fact]
    public void A_start_other_than_five_hundred_is_the_level_a_new_lab_has()
    {
        Game g = Lab(c => c.Culture.Start = 800);
        SetLevel(g, 800);
        Assert.Equal(800, Level(g));
        Assert.Equal("open-book", SafetyCulture.Of(g.C, g.Me).BandId);
        Assert.Equal(60, SafetyCulture.CatchPermille(g.C, g.Me));
    }

    [Fact]
    public void Culture_above_neutral_adds_to_the_catch_and_below_takes_from_it()
    {
        Game g = Lab();
        SetLevel(g, 1000);
        Assert.Equal(100, SafetyCulture.CatchPermille(g.C, g.Me));
        SetLevel(g, 0);
        Assert.Equal(-100, SafetyCulture.CatchPermille(g.C, g.Me));
        SetLevel(g, 250);
        Assert.Equal(-50, SafetyCulture.CatchPermille(g.C, g.Me));
        SetLevel(g, 503);
        Assert.Equal(0, SafetyCulture.CatchPermille(g.C, g.Me));
        SetLevel(g, 501);
        Assert.Equal(0, SafetyCulture.CatchPermille(g.C, g.Me));
    }

    [Fact]
    public void The_catch_the_lab_has_is_its_roles_and_rooms_and_traits_and_culture_together()
    {
        Game g = Lab(c => c.RolePerks.AddRange(TestData.Config().RolePerks));
        Staff(g, "red-teamer", 3);
        SetLevel(g, 500);
        int plain = RolePerks.IncidentCatchPermille(g.C, g.Me);
        Assert.True(plain > 0);
        SetLevel(g, 1000);
        Assert.Equal(plain + 100, RolePerks.IncidentCatchPermille(g.C, g.Me));
        SetLevel(g, 0);
        Assert.Equal(Math.Max(0, plain - 100), RolePerks.IncidentCatchPermille(g.C, g.Me));
        SetLevel(g, 400);
        Assert.Equal(plain - 20, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void A_cover_up_culture_cannot_push_the_catch_below_nothing_and_a_great_one_cannot_pass_ninety_per_cent()
    {
        Game g = Lab();
        SetLevel(g, 0);
        Assert.Equal(0, RolePerks.IncidentCatchPermille(g.C, g.Me));
        Game h = Lab(c => { c.Culture.CatchSlopePermille = 1000; c.Culture.Start = 1000; });
        SetLevel(h, 1000);
        Assert.Equal(500, SafetyCulture.CatchPermille(h.C, h.Me));
        Staff(h, "red-teamer", 100);
        h.Me.Staff.Add(new StaffMember { Id = h.State.TakeId(), RoleId = "researcher", TraitId = "reads-the-logs", Morale = 500 });
        Assert.InRange(RolePerks.IncidentCatchPermille(h.C, h.Me), 0, 900);
    }

    // ---- the weeks

    [Fact]
    public void Without_anyone_listening_culture_slips_every_week()
    {
        Game g = Lab();
        SetLevel(g, 500);
        for (int i = 0; i < 10; i++) SafetyCulture.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(480, Level(g));
        Assert.Equal(-2, SafetyCulture.Of(g.C, g.Me).WeeklyChange);
    }

    [Fact]
    public void Each_person_on_the_safety_side_pays_back_a_point_a_week_up_to_a_ceiling()
    {
        Game g = Lab();
        Staff(g, "safety", 3);
        SetLevel(g, 500);
        for (int i = 0; i < 10; i++) SafetyCulture.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(510, Level(g));

        Staff(g, "red-teamer", 20);
        Assert.Equal(6, SafetyCulture.StaffGain(g.C, g.Me));
        SetLevel(g, 500);
        for (int i = 0; i < 10; i++) SafetyCulture.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(540, Level(g));
    }

    [Fact]
    public void Only_the_listening_roles_count()
    {
        Game g = Lab();
        int before = SafetyCulture.ListeningStaff(g.C, g.Me);
        Staff(g, "researcher", 10);
        Staff(g, "sales", 10);
        Assert.Equal(before, SafetyCulture.ListeningStaff(g.C, g.Me));
        Staff(g, "alignment-fellow", 2);
        Staff(g, "red-teamer", 1);
        Staff(g, "safety", 4);
        Assert.Equal(before + 7, SafetyCulture.ListeningStaff(g.C, g.Me));
    }

    [Fact]
    public void Culture_stops_at_the_floor_and_the_ceiling_over_a_long_time()
    {
        Game g = Lab();
        SetLevel(g, 20);
        for (int i = 0; i < 100; i++) SafetyCulture.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, Level(g));
        Staff(g, "safety", 50);
        for (int i = 0; i < 1000; i++) SafetyCulture.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1000, Level(g));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Letting_the_safety_team_go_lets_culture_slide()
    {
        Game g = Lab();
        Staff(g, "safety", 6);
        SetLevel(g, 700);
        g.Weeks(20);
        int kept = Level(g);
        g.Me.Staff.RemoveAll(s => s.RoleId == "safety");
        g.Weeks(20);
        Assert.True(Level(g) < kept, "a lab with nobody listening loses culture");
    }

    [Fact]
    public void Rival_labs_keep_the_culture_they_started_with()
    {
        Game g = Game.New(1);
        g.Weeks(48);
        foreach (LabState lab in g.State.Labs.Where(l => l.Id != g.Id)) Assert.Equal(0, lab.CultureShift);
    }

    [Fact]
    public void A_bankrupt_lab_does_not_move_its_culture_through_the_weeks()
    {
        Game g = Lab();
        SetLevel(g, 500);
        g.Me.Bankrupt = true;
        g.Weeks(4);
        Assert.Equal(500, Level(g));
    }

    // ---- the gate

    [Fact]
    public void A_gate_can_be_set_and_is_written_down_with_the_week_it_was_set()
    {
        Game g = Lab();
        g.Weeks(5);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        Assert.Equal(600, g.Me.ReleaseGate);
        Assert.Equal(g.Tick, g.Me.GateSetTick);
        Assert.Equal(g.Tick + g.C.Culture.GateLockWeeks, SafetyCulture.Of(g.C, g.Me).GateLockedUntilTick);
        Assert.Equal(600, SafetyCulture.Of(g.C, g.Me).Gate);
    }

    [Fact]
    public void A_gate_outside_its_range_is_refused_and_changes_nothing()
    {
        Game g = Lab();
        g.Refused(Command.SetReleaseGate(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetReleaseGate(g.Id, g.C.Culture.MaxGate + 1), CommandError.InvalidArgument);
        g.Do(Command.SetReleaseGate(g.Id, g.C.Culture.MaxGate));
        Assert.Equal(900, g.Me.ReleaseGate);
    }

    [Fact]
    public void A_model_that_shows_less_than_the_gate_cannot_go_on_sale()
    {
        Game g = Lab();
        ModelState model = Trained(g, 550);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "game-ai"), CommandError.BelowReleaseGate);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void A_model_exactly_at_the_gate_goes_through()
    {
        Game g = Lab();
        ModelState model = Trained(g, 600);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Release(model, "game-ai");
        Assert.Equal(ModelStatus.Released, model.Status);
    }

    [Fact]
    public void Claiming_a_better_meter_at_the_door_does_not_get_past_the_gate()
    {
        Game g = Lab();
        ModelState model = Trained(g, 300);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Refused(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"), CommandError.BelowReleaseGate);
    }

    [Fact]
    public void With_no_gate_nothing_is_blocked()
    {
        Game g = Lab();
        ModelState model = Trained(g, 100);
        g.Release(model, "game-ai");
        Assert.Equal(ModelStatus.Released, model.Status);
    }

    [Fact]
    public void The_gate_and_the_laws_bar_both_apply_and_the_higher_one_wins()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(Quiet), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        int bar = EraRules.ReleaseBar(g.C, g.Tick);
        Assert.Equal(120, bar);
        ModelState model = Trained(g, 100);
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"), CommandError.BelowEraSafetyBar);
        g.Do(Command.SetReleaseGate(g.Id, 50));
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"), CommandError.BelowEraSafetyBar);
        g.Do(Command.SetReleaseGate(g.Id, 300));
        model.Alignment = 200;
        model.DisplayedAlignment = 200;
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"), CommandError.BelowReleaseGate);
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Release(model, "consumer-assistant");
        Assert.Equal(ModelStatus.Released, model.Status);
    }

    [Fact]
    public void A_gate_that_holds_stops_every_model_not_just_the_first()
    {
        Game g = Lab();
        ModelState one = Trained(g, 500);
        ModelState two = Trained(g, 500);
        g.Do(Command.SetReleaseGate(g.Id, 800));
        g.Refused(Command.ReleaseModel(g.Id, one.Id, "game-ai"), CommandError.BelowReleaseGate);
        g.Refused(Command.ReleaseModel(g.Id, two.Id, "game-ai"), CommandError.BelowReleaseGate);
    }

    // ---- loosening it

    [Fact]
    public void The_gate_cannot_be_loosened_while_the_lock_runs_but_can_be_raised()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Weeks(5);
        string digest = g.Sim.Digest();
        g.Refused(Command.SetReleaseGate(g.Id, 500), CommandError.GateLocked);
        g.Refused(Command.SetReleaseGate(g.Id, 0), CommandError.GateLocked);
        Assert.Equal(digest, g.Sim.Digest());
        g.Do(Command.SetReleaseGate(g.Id, 700));
        Assert.Equal(700, g.Me.ReleaseGate);
        Assert.Equal(g.Tick, g.Me.GateSetTick);
    }

    [Fact]
    public void Raising_the_gate_starts_the_lock_again()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Weeks(g.C.Culture.GateLockWeeks - 1);
        g.Do(Command.SetReleaseGate(g.Id, 650));
        g.Weeks(2);
        g.Refused(Command.SetReleaseGate(g.Id, 600), CommandError.GateLocked);
    }

    [Fact]
    public void After_the_lock_the_gate_can_be_loosened_at_a_price_in_culture()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Weeks(g.C.Culture.GateLockWeeks);
        SetLevel(g, 500);
        g.Do(Command.SetReleaseGate(g.Id, 400));
        Assert.Equal(400, g.Me.ReleaseGate);
        Assert.Equal(500 - g.C.Culture.GateLoweredPoints, Level(g));
    }

    [Fact]
    public void Taking_the_gate_away_is_loosening_it_and_costs_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Weeks(g.C.Culture.GateLockWeeks);
        SetLevel(g, 500);
        g.Do(Command.SetReleaseGate(g.Id, 0));
        Assert.Equal(0, g.Me.ReleaseGate);
        Assert.Equal(443, Level(g));
    }

    [Fact]
    public void The_lock_lifts_on_exactly_the_week_it_says()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        int until = SafetyCulture.Of(g.C, g.Me).GateLockedUntilTick;
        g.Weeks(until - g.Tick - 1);
        g.Refused(Command.SetReleaseGate(g.Id, 500), CommandError.GateLocked);
        g.Weeks(1);
        g.Do(Command.SetReleaseGate(g.Id, 500));
    }

    [Fact]
    public void Setting_the_same_gate_again_changes_nothing_and_does_not_restart_the_lock()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 600));
        int tick = g.Me.GateSetTick;
        g.Weeks(4);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        Assert.Equal(tick, g.Me.GateSetTick);
    }

    [Fact]
    public void A_lock_of_no_weeks_lets_the_gate_go_at_once()
    {
        Game g = Lab(c => c.Culture.GateLockWeeks = 0);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        g.Do(Command.SetReleaseGate(g.Id, 0));
        Assert.Equal(0, g.Me.ReleaseGate);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_set_a_gate()
    {
        Game g = Lab();
        g.Me.Bankrupt = true;
        string digest = g.Sim.Digest();
        Assert.False(g.Try(Command.SetReleaseGate(g.Id, 600)).Ok);
        Assert.Equal(digest, g.Sim.Digest());
    }

    // ---- releases move culture

    [Fact]
    public void Keeping_your_own_gate_earns_culture_once_per_model()
    {
        Game g = Lab();
        ModelState model = Trained(g, 700);
        g.Do(Command.SetReleaseGate(g.Id, 600));
        SetLevel(g, 500);
        g.Release(model, "game-ai");
        Assert.Equal(500 + g.C.Culture.GateKeptPoints, Level(g));
        g.Release(model, "game-ai", "game-bots");
        Assert.Equal(510, Level(g));
    }

    [Fact]
    public void A_release_through_no_gate_at_a_decent_meter_changes_nothing()
    {
        Game g = Lab();
        ModelState model = Trained(g, 700);
        SetLevel(g, 500);
        g.Release(model, "game-ai");
        Assert.Equal(500, Level(g));
    }

    [Fact]
    public void Rushing_a_model_out_under_the_rush_line_costs_culture_once()
    {
        Game g = Lab();
        ModelState model = Trained(g, 300);
        SetLevel(g, 500);
        g.Release(model, "game-ai");
        Assert.Equal(500 - g.C.Culture.RushPoints, Level(g));
        g.Release(model, "game-ai", "game-bots");
        Assert.Equal(460, Level(g));
    }

    [Fact]
    public void A_rushed_release_through_a_low_gate_is_still_rushed_and_earns_nothing()
    {
        Game g = Lab();
        ModelState model = Trained(g, 400);
        g.Do(Command.SetReleaseGate(g.Id, 300));
        SetLevel(g, 500);
        g.Release(model, "game-ai");
        Assert.Equal(460, Level(g));
    }

    [Fact]
    public void A_model_exactly_on_the_rush_line_is_not_rushed()
    {
        Game g = Lab();
        ModelState model = Trained(g, g.C.Culture.RushBelowAlignment);
        SetLevel(g, 500);
        g.Release(model, "game-ai");
        Assert.Equal(500, Level(g));
    }

    [Fact]
    public void A_refused_release_costs_nothing()
    {
        Game g = Lab();
        ModelState model = Trained(g, 300);
        g.Do(Command.SetReleaseGate(g.Id, 800));
        SetLevel(g, 500);
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "game-ai"), CommandError.BelowReleaseGate);
        Assert.Equal(500, Level(g));
    }

    // ---- the customer desk

    static Complaint Complain(Game g, int expires)
    {
        var complaint = new Complaint { Id = g.State.TakeId(), SectorId = "game-ai", ModelId = 1, Stake = 50, ExpiresTick = expires, Ref = "x" };
        g.Me.Complaints.Add(complaint);
        return complaint;
    }

    [Fact]
    public void Answering_a_customer_earns_culture_and_leaving_them_to_lapse_costs_it()
    {
        Game g = Lab();
        SetLevel(g, 500);
        Complaint answered = Complain(g, g.Tick + 10);
        g.Do(Command.AnswerComplaint(g.Id, answered.Id, g.C.Trust.Responses[0].Id));
        Assert.Equal(500 + g.C.Culture.ComplaintAnsweredPoints, Level(g));

        SetLevel(g, 500);
        Complain(g, g.Tick);
        TrustSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.Complaints);
        Assert.Equal(500 - g.C.Culture.ComplaintLapsedPoints, Level(g));
    }

    [Fact]
    public void A_complaint_that_has_not_yet_lapsed_costs_nothing()
    {
        Game g = Lab();
        SetLevel(g, 500);
        Complain(g, g.Tick + 5);
        TrustSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(500, Level(g));
        Assert.Single(g.Me.Complaints);
    }

    // ---- other systems

    [Fact]
    public void A_listening_lab_has_fewer_incidents_than_a_hiding_one()
    {
        Func<int, int> incidents = level =>
        {
            Game g = Lab(c => c.Safety.IncidentChancePermille = 1000);
            ModelState model = Trained(g, 300);
            g.Release(model, "game-ai");
            model.Alignment = 300;
            model.DisplayedAlignment = 300;
            int n = 0;
            for (int i = 0; i < 300; i++)
            {
                SetLevel(g, level);
                int before = model.Incidents;
                SafetySystem.RunWeek(g.C, g.State, g.Me);
                if (model.Incidents > before) n++;
            }
            return n;
        };
        int listening = incidents(1000);
        int hiding = incidents(0);
        Assert.True(listening < hiding, listening + " incidents when staff report problems, " + hiding + " when they hide them");
    }

    [Fact]
    public void The_autopilot_never_sets_a_gate_for_anyone()
    {
        Game g = Game.New(2);
        g.Autopilot(2 * 48);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.ReleaseGate));
    }

    [Fact]
    public void The_incident_odds_screen_counts_what_culture_catches()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 500);
        ModelState model = Trained(g, 300);
        g.Release(model, "game-ai");
        SetLevel(g, 500);
        int plain = RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model).CaughtByRedTeam;
        SetLevel(g, 1000);
        Assert.True(RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model).CaughtByRedTeam < plain);
    }

    // ---- save, load, migration

    [Fact]
    public void Culture_and_the_gate_survive_a_save_and_a_load()
    {
        Game g = Lab();
        g.Do(Command.SetReleaseGate(g.Id, 640));
        SetLevel(g, 733);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(733, SafetyCulture.Level(g.C, again.Player));
        Assert.Equal(640, again.Player.ReleaseGate);
        Assert.Equal(g.Me.GateSetTick, again.Player.GateSetTick);
        for (int i = 0; i < 30; i++) { g.Sim.Step(); again.Step(); }
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_the_usual_culture_and_no_gate()
    {
        Game g = Lab();
        g.Weeks(10);
        string old = SaveSchemaPretend.At(g.C, 27, g.Sim.Save());
        Assert.DoesNotContain("releaseGate", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        Assert.All(loaded.State.Labs, lab => { Assert.Equal(0, lab.CultureShift); Assert.Equal(0, lab.ReleaseGate); });
        Assert.Equal(500, SafetyCulture.Level(g.C, loaded.Player));
    }

    [Fact]
    public void A_save_with_a_gate_past_the_maximum_or_a_culture_out_of_range_is_reported()
    {
        Game g = Lab();
        g.Me.ReleaseGate = 950;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("release gate"));
        g.Me.ReleaseGate = 0;
        g.Me.CultureShift = 600;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("culture shift"));
        g.Me.CultureShift = -501;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("culture shift"));
        g.Me.CultureShift = 0;
        g.Me.GateSetTick = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("gate set at tick"));
    }

    [Fact]
    public void Two_runs_of_the_same_game_land_on_the_same_digest()
    {
        Func<string> run = () =>
        {
            Game g = Lab();
            Staff(g, "safety", 4);
            g.Do(Command.SetReleaseGate(g.Id, 500));
            ModelState model = Trained(g, 600);
            g.Release(model, "game-ai");
            g.Weeks(60);
            return g.Sim.Digest();
        };
        Assert.Equal(run(), run());
    }

    [Fact]
    public void The_config_round_trips_with_its_culture_numbers()
    {
        GameConfig c = TestData.Config();
        GameConfig again = GameConfig.FromJson(c.ToJson());
        Assert.Equal(c.Culture.Bands.Count, again.Culture.Bands.Count);
        Assert.Equal(26, again.Culture.GateLockWeeks);
        Assert.Equal(c.Culture.ListeningRoles, again.Culture.ListeningRoles);
    }

    // ---- the page

    [Fact]
    public void The_page_lists_every_band_and_listening_role()
    {
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "safety-culture.md"));
        GameConfig c = TestData.Config();
        foreach (CultureBandConfig b in c.Culture.Bands)
        {
            Assert.Contains("`" + b.Id + "`", page);
            Assert.Contains(b.Blurb, page);
        }
        foreach (string role in c.Culture.ListeningRoles) Assert.Contains("`" + role + "`", page);
    }
}
