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
    public void Schema_9_staff_pools_become_named_people()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(10, sim.State.Schema);
        int people = sim.Player.Staff.Count;
        Assert.True(people > 0);
        Assert.All(sim.Player.Staff, p => Assert.False(string.IsNullOrEmpty(p.TraitId)));
    }
}