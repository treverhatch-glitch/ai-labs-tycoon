using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-fourteen combination rules (Q461) keep their cross-system effects through saves.</summary>
public class EraNBatchFourteenTogetherHardeningTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Chiefs.MinStaff = 0;
        c.SpecialistLines.MinEra = 0;
        c.DataSources.MinEra = 0;
        c.OpenWeights.MinEra = 0;
        c.SectorPricing.MinEra = 0;
        c.PublicMetrics.MinEra = 0;
    }

    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(219, c =>
        {
            Quiet(c);
            more?.Invoke(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 3, sales: 0, gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    [Fact]
    public void Specialist_sector_boost_mid_save_still_beats_plain_lab_appeal()
    {
        Game bare = Lab();
        Game staffed = Lab();
        staffed.Build("sales-office", 6, 4);
        staffed.Hire("sales", 4);
        foreach (Game g in new[] { bare, staffed })
        {
            ModelState m = g.Train("mini", 0);
            g.UntilTrained(m);
            g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
            g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        }
        GameConfig c = bare.C;
        string bareMid = bare.Sim.Save();
        string staffedMid = staffed.Sim.Save();
        Simulation bareBack = Simulation.Load(c, bareMid);
        Simulation staffedBack = Simulation.Load(c, staffedMid);
        bareBack.StepMany(4);
        staffedBack.StepMany(4);
        string sector = "code-completion";
        long plain = DisplayQueries.Appeal(c, bareBack.State, c.Sector(sector), bareBack.State.Sector(sector), bareBack.Player, bareBack.Player.Models[0]);
        long stacked = DisplayQueries.Appeal(c, staffedBack.State, c.Sector(sector), staffedBack.State.Sector(sector), staffedBack.Player, staffedBack.Player.Models[0]);
        Assert.True(stacked > plain);
    }

    [Fact]
    public void Data_source_and_dataset_mid_save_still_lift_training_forecast()
    {
        Game a = Lab(c => c.DataSources.Sources[0].QualityPermille = 400);
        Game b = Lab(c =>
        {
            c.DataSources.Sources[0].QualityPermille = 400;
            c.Datasets.BonusPer100 = 120;
        });
        a.Do(Command.AcquireDataSource(a.Id, "licensed-corpus"));
        b.Do(Command.AcquireDataSource(b.Id, "licensed-corpus"));
        b.Me.DataClean1 = 200;
        GameConfig c = a.C;
        string aMid = a.Sim.Save();
        string bMid = b.Sim.Save();
        Simulation aBack = Simulation.Load(c, aMid);
        Simulation bBack = Simulation.Load(c, bMid);
        var line = c.Line("mini");
        Assert.True(TrainingSystem.Forecast(c, bBack.Player, line).Tech > TrainingSystem.Forecast(c, aBack.Player, line).Tech);
    }

    [Fact]
    public void Open_weights_and_metrics_mid_save_still_raise_trust_in_order()
    {
        Game g = Lab(c =>
        {
            c.OpenWeights.CopyChancePermille = 0;
            c.PublicMetrics.Bar = 500;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        int t0 = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, 1));
        int t1 = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        GameConfig c = g.C;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(c, mid);
        back.StepMany(4);
        Assert.True(t1 > t0);
        Game after = new Game(back);
        after.Do(Command.PublishMetrics(after.Id, after.Me.Models[0].Id));
        after.Me.LastWeek.ServedPermille = 950;
        PublicMetrics.RunWeek(c, after.State, after.Me);
        Assert.True(TrustSystem.Of(c, after.Me, after.Me.Models[0].Sectors[0]) > t1);
    }

    [Fact]
    public void Sector_pricing_mid_save_still_adds_lab_overload()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        int b = SectorPricing.LabOverloadPermille(g.C, g.Me);
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 3));
        GameConfig c = g.C;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(c, mid);
        back.StepMany(3);
        Assert.True(SectorPricing.LabOverloadPermille(c, back.Player) > b);
    }
}