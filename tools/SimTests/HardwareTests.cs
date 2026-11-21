using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Envydia's GPU generations and the fleet of hardware batches.</summary>
public class HardwareTests
{
    static Game SoloIn(string eraId)
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = new List<string>(), StartEraId = eraId };
        return new Game(Simulation.NewGame(TestData.Config(), options));
    }

    [Fact]
    public void The_newest_generation_on_sale_follows_the_calendar()
    {
        GameConfig c = TestData.Config();
        foreach (GpuGenerationConfig gen in c.Compute.GpuGenerations)
        {
            int opens = (gen.OpensYear - c.Calendar.StartYear) * c.Calendar.WeeksPerYear;
            Assert.Equal(gen.Id, LabQueries.CurrentGeneration(c, opens).Id);
            if (gen.Tier > 0) Assert.Equal(c.Compute.GpuGenerations[gen.Tier - 1].Id, LabQueries.CurrentGeneration(c, opens - 1).Id);
        }
        Assert.Equal("voodude", LabQueries.CurrentGeneration(c, 0).Id);
    }

    [Fact]
    public void A_purchase_is_the_generation_on_sale_in_the_year_it_happens()
    {
        Game old = Game.Solo();
        old.Build("server-closet", 2, 2);
        long before = old.Me.Cash;
        old.Do(Command.BuyGpus(old.Id, 3));
        Assert.Equal(before - 3 * old.C.Compute.Generation("voodude").BuyPrice, old.Me.Cash);

        Game later = SoloIn("punchline");
        later.Build("server-closet", 2, 2);
        before = later.Me.Cash;
        later.Do(Command.BuyGpus(later.Id, 3));
        Assert.Equal(before - 3 * later.C.Compute.Generation("teslo").BuyPrice, later.Me.Cash);
        Assert.Equal("teslo", later.Me.Fleet.Single().GpuGenId);
    }

    [Fact]
    public void Buying_twice_in_the_same_generation_joins_one_batch()
    {
        Game g = Game.Solo();
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 3));
        g.Weeks(5);
        g.Do(Command.BuyGpus(g.Id, 2));
        HardwareBatch batch = g.Me.Fleet.Single();
        Assert.Equal(5, batch.Count);
        Assert.Equal(HardwareBatch.Hq, batch.HoldingId);
    }

    [Fact]
    public void Resale_is_the_resale_share_cut_once_more_for_every_generation_behind()
    {
        GameConfig c = TestData.Config();
        GpuGenerationConfig first = c.Compute.Generation("voodude");
        int kepla = (c.Compute.Generation("kepla").OpensYear - c.Calendar.StartYear) * c.Calendar.WeeksPerYear;
        Assert.Equal(first.BuyPrice * 400 / 1000, LabQueries.ResalePerUnit(c, first, 0));
        // 14,760 x 0.4 = 5,904; x 0.65 = 3,837; x 0.65 = 2,494; x 0.65 = 1,621. Rounded down at every step.
        Assert.Equal(1621, LabQueries.ResalePerUnit(c, first, kepla));
        Assert.Equal(c.Compute.Generation("kepla").BuyPrice * 400 / 1000, LabQueries.ResalePerUnit(c, c.Compute.Generation("kepla"), kepla));
    }

    [Fact]
    public void Selling_gives_up_the_oldest_generation_first_at_its_own_decayed_price()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 2000000);
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 4));
        int tick = (g.C.Compute.Generation("geforks").OpensYear - g.C.Calendar.StartYear) * g.C.Calendar.WeeksPerYear;
        g.Weeks(tick - g.Sim.Tick);
        g.Do(Command.BuyGpus(g.Id, 4));
        Assert.Equal(2, g.Me.Fleet.Count);

        long before = g.Me.Cash;
        long oldPrice = LabQueries.ResalePerUnit(g.C, g.C.Compute.Generation("voodude"), g.Sim.Tick);
        long newPrice = LabQueries.ResalePerUnit(g.C, g.C.Compute.Generation("geforks"), g.Sim.Tick);
        Assert.True(oldPrice < newPrice);

        g.Do(Command.SellGpus(g.Id, 6));
        Assert.Equal(before + 4 * oldPrice + 2 * newPrice, g.Me.Cash);
        HardwareBatch left = g.Me.Fleet.Single();
        Assert.Equal("geforks", left.GpuGenId);
        Assert.Equal(2, left.Count);
        Assert.Equal(g.Me.Ledger.StartCash + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
    }

    [Fact]
    public void Every_batch_works_at_the_speed_of_its_own_generation()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 2000000);
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 4));
        int tick = (g.C.Compute.Generation("geforks").OpensYear - g.C.Calendar.StartYear) * g.C.Calendar.WeeksPerYear;
        g.Weeks(tick - g.Sim.Tick);
        g.Do(Command.BuyGpus(g.Id, 4));
        g.Rent(2);

        long expected = 4 * g.C.Compute.Generation("voodude").ComputePerGpuWeek
            + 4 * g.C.Compute.Generation("geforks").ComputePerGpuWeek
            + 2 * g.C.Compute.Generation("geforks").ComputePerGpuWeek;
        Assert.Equal(expected, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_schema_1_save_becomes_one_batch_at_headquarters_and_plays_on_as_before()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema1Save());
        HardwareBatch batch = sim.Player.Fleet.Single();
        Assert.Equal(12, batch.Count);
        Assert.Equal("voodude", batch.GpuGenId);
        Assert.Equal(HardwareBatch.Hq, batch.HoldingId);
        Assert.Equal(6, sim.Player.RentedGpus);
        Assert.Equal(180, LabQueries.WeeklyCompute(sim.Config, sim.State, sim.Player));

        sim.Step();
        Assert.Equal(302732, sim.Player.Cash);
        Assert.Equal(-540, sim.Player.Ledger.LastWeek.GpuUpkeep);
        Assert.Equal(-1500, sim.Player.Ledger.LastWeek.GpuRent);
    }

    [Fact]
    public void Cash_stays_the_founding_cash_plus_the_ledger_through_buying_selling_and_years()
    {
        Game g = Game.New(tweak: c => c.PlayerStart.Cash = 3000000).WithStarterLab();
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 8));
        for (int week = 0; week < 3 * g.C.Calendar.WeeksPerYear; week++)
        {
            if (week == 20) g.Do(Command.SellGpus(g.Id, 3));
            g.Weeks(1);
            foreach (LabState lab in g.State.Labs) Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
        }
    }

    [Fact]
    public void A_gpu_generation_list_that_is_out_of_order_is_reported()
    {
        GameConfig c = TestData.Config();
        c.Compute.GpuGenerations[2].OpensYear = c.Compute.GpuGenerations[1].OpensYear;
        c.Compute.GpuGenerations[3].ComputePerGpuWeek = 1;
        c.Compute.GpuGenerations[4].Tier = 9;
        c.Compute.GpuGenerations[5].KwPerGpu = 0;
        c.Compute.GpuGenerations[6].IdValue = c.Compute.GpuGenerations[0].IdValue;
        string problems = string.Join("\n", ConfigValidator.Problems(c));
        Assert.Contains("teslo", problems);
        Assert.Contains("kepla", problems);
        Assert.Contains("voltage", problems);
        Assert.Contains("hoppa", problems);
        Assert.Contains("voodude", problems);
    }

    [Fact]
    public void A_config_that_starts_before_the_first_generation_is_reported()
    {
        GameConfig c = TestData.Config();
        c.Compute.GpuGenerations[0].OpensYear = c.Calendar.StartYear + 1;
        Assert.Contains(ConfigValidator.Problems(c), line => line.Contains("after the game starts"));
    }
}
