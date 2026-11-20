using System.Linq;
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
    }

    [Fact]
    public void Schema_10_migration_splits_role_pools_into_people()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        int researchers = LabQueries.StaffCount(sim.Player, "researcher");
        int headCount = sim.Player.Staff.Count;
        Assert.True(headCount >= researchers);
        Assert.Equal(researchers, sim.Player.Staff.Count(p => p.RoleId == "researcher"));
        Assert.All(sim.Player.Staff, p => Assert.False(string.IsNullOrEmpty(p.TraitId)));
    }
}