using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class StaffPeopleTests
{
    [Fact]
    public void Hire_creates_one_person_per_seat_with_trait_and_morale()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 2);
        Assert.Equal(2, g.Me.Staff.Count);
        Assert.All(g.Me.Staff, p => Assert.Equal("researcher", p.RoleId));
        Assert.All(g.Me.Staff, p => Assert.Equal(g.C.StaffPeople.MoraleBaseline, p.Morale));
        Assert.All(g.Me.Staff, p => Assert.False(string.IsNullOrEmpty(p.TraitId)));
        Assert.Equal(2, LabQueries.StaffCount(g.Me, "researcher"));
    }

    [Fact]
    public void Fire_lets_go_of_the_lowest_morale_first()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 2);
        g.Me.Staff[0].Morale = 300;
        g.Me.Staff[1].Morale = 800;
        int lowId = g.Me.Staff[0].Id;
        g.Do(Command.Fire(g.Id, "researcher", 1));
        Assert.Single(g.Me.Staff);
        Assert.Equal(800, g.Me.Staff[0].Morale);
        Assert.NotEqual(lowId, g.Me.Staff[0].Id);
    }

    [Fact]
    public void Fire_removes_only_the_requested_role()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Hire("researcher", 2);
        g.Hire("engineer", 1);
        g.Do(Command.Fire(g.Id, "researcher", 2));
        Assert.DoesNotContain(g.Me.Staff, p => p.RoleId == "researcher");
        Assert.Single(g.Me.Staff, p => p.RoleId == "engineer");
    }

    [Fact]
    public void Morale_drifts_toward_baseline_each_week()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].Morale = 200;
        int baseline = g.C.StaffPeople.MoraleBaseline;
        int drift = g.C.StaffPeople.MoraleDriftPerWeek;
        g.Weeks(5);
        Assert.Equal(MathI.Min(baseline, 200 + 5 * drift), g.Me.Staff[0].Morale);
    }

    [Fact]
    public void Stars_earn_more_research_points_per_week_than_steady_hires()
    {
        var steady = Game.Solo();
        steady.Build("research-lab", 4, 4);
        steady.Hire("researcher", 3);
        foreach (StaffMember p in steady.Me.Staff) p.TraitId = "steady";

        var stars = Game.Solo(seed: steady.State.Seed);
        stars.Build("research-lab", 4, 4);
        stars.Hire("researcher", 3);
        foreach (StaffMember p in stars.Me.Staff) p.TraitId = "star";

        steady.Weeks(4);
        stars.Weeks(4);
        Assert.True(stars.Me.ResearchPoints > steady.Me.ResearchPoints);
    }

    [Fact]
    public void A_feuding_rival_can_poach_a_star_from_the_player()
    {
        var g = Game.New(seed: 77);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        int before = g.Me.Staff.Count;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        g.C.StaffPeople.PoachChancePermille = 1000;
        g.Until(() => g.State.Events.Any(e => e.Kind == EventKind.StaffPoached), 60, "a poach");
        Assert.True(g.Me.Staff.Count < before);
        Assert.DoesNotContain(g.Me.Staff, p => p.TraitId == "star");
        GameEvent poach = g.State.Events.Last(e => e.Kind == EventKind.StaffPoached);
        Assert.Equal(rival.Id, poach.LabId);
        Assert.Equal(g.Id, poach.SubjectId);
    }

    [Fact]
    public void Poaching_never_happens_without_a_feud_feeling()
    {
        var g = Game.New(seed: 88);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = 0;
        g.C.StaffPeople.PoachChancePermille = 1000;
        for (int i = 0; i < 40; i++)
        {
            RivalSystem.RunMonth(g.C, g.State);
            g.Weeks(4);
        }
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.StaffPoached);
        Assert.Single(g.Me.Staff);
    }

    [Fact]
    public void Poaching_skips_a_bankrupt_player_even_when_a_rival_is_hostile()
    {
        var g = Game.New(seed: 99);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].TraitId = "star";
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 200;
        g.C.StaffPeople.PoachChancePermille = 1000;
        int count = g.Me.Staff.Count;
        g.Me.Bankrupt = true;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(count, g.Me.Staff.Count);
    }

    [Fact]
    public void Poaching_takes_the_unhappiest_star_first()
    {
        var g = Game.New(seed: 3);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 3);
        foreach (StaffMember p in g.Me.Staff) p.TraitId = "star";
        g.Me.Staff[0].Morale = 400;
        g.Me.Staff[1].Morale = 900;
        g.Me.Staff[2].Morale = 600;
        int lowId = g.Me.Staff[0].Id;
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 50;
        g.C.StaffPeople.PoachChancePermille = 1000;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(2, g.Me.Staff.Count);
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == lowId);
    }

    [Fact]
    public void Staff_roster_survives_a_save_round_trip()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 2);
        g.Me.Staff[0].TraitId = "star";
        g.Me.Staff[0].Morale = 777;
        string digest = g.Sim.Digest();
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(digest, again.Digest());
        StaffMember p = again.Player.Staff.Single(s => s.Morale == 777);
        Assert.Equal("star", p.TraitId);
    }

    [Fact]
    public void Schema_10_migration_splits_role_pools_into_people()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(8, sim.Player.Staff.Count);
        Assert.Equal(4, LabQueries.StaffCount(sim.Player, "researcher"));
        Assert.Equal(2, LabQueries.StaffCount(sim.Player, "engineer"));
        Assert.Equal(1, LabQueries.StaffCount(sim.Player, "sales"));
        Assert.Equal(1, LabQueries.StaffCount(sim.Player, "safety"));
        Assert.Equal(8, sim.Player.Staff.Select(p => p.Id).Distinct().Count());
        Assert.All(sim.Player.Staff, p => Assert.True(p.Id >= 44));
        Assert.Equal("steady", sim.Player.Staff[0].TraitId);
    }

    [Fact]
    public void Star_hire_permille_can_roll_a_star_trait_on_hire()
    {
        var g = Game.Solo(tweak: c => c.StaffPeople.StarHirePermille = 1000);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        Assert.Equal("star", g.Me.Staff[0].TraitId);
    }

    [Fact]
    public void Weeks_in_debt_lower_morale_faster_than_baseline_drift()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        g.Me.Staff[0].Morale = 600;
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Weeks(1);
        Assert.True(g.Me.WeeksInDebt > 0);
        Assert.True(g.Me.Staff[0].Morale < 600);
    }

    [Fact]
    public void Loading_from_schema_9_expands_pools_without_losing_headcount()
    {
        var g = Game.Solo();
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Hire("researcher", 3);
        g.Hire("engineer", 1);
        int people = g.Me.Staff.Count;
        string at9 = SaveSchemaPretend.At(g.C, 9, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, at9);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Equal(people, loaded.Player.Staff.Count);
        Assert.Equal(3, LabQueries.StaffCount(loaded.Player, "researcher"));
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }
}