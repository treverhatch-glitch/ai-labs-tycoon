using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraNSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 95)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.DataSources.MinEra = 0;
            c.OpenWeights.MinEra = 0;
            c.SectorPricing.MinEra = 0;
            c.PlayerStart.Cash = 50_000_000;
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    static ModelState Sold(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void Specialist_refuses_unknown_branch()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Refused(Command.BranchSpecialist(g.Id, m.Id, "nope"), CommandError.UnknownId);
    }

    [Fact]
    public void Data_source_refuses_unknown_id()
    {
        Game g = Lab();
        g.Refused(Command.AcquireDataSource(g.Id, "missing"), CommandError.UnknownId);
    }

    [Fact]
    public void Sector_price_refuses_unknown_sector()
    {
        Game g = Lab();
        g.Refused(Command.SetSectorPrice(g.Id, "not-a-sector", 2), CommandError.UnknownId);
    }
}