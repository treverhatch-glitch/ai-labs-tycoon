using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SpecialistLinesTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Branch_specialist_is_checked_and_boosts_in_sector_appeal()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        Assert.Equal(3, m.SpecKind);
        long inSector = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("code-completion"), g.State.Sector("code-completion"), g.Me, m);
        long outSector = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, m);
        Assert.True(inSector >= outSector);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_strip_spec_kind()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "image"));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 134, g.Sim.Save()));
        Assert.All(older.Player.Models, x => Assert.Equal(0, x.SpecKind));
    }
}