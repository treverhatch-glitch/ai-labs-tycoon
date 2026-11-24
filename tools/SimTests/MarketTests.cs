using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Releasing models into sectors, winning customers over time, and getting paid.</summary>
public class MarketTests
{
    /// <summary>A solo lab with a trained Mini 1, ready to release.</summary>
    static Game WithTrainedMini(out ModelState model, int alignment = 200, int gpus = 8)
    {
        Game g = Game.Solo().WithStarterLab(gpus: gpus);
        model = g.Train("mini", alignment);
        g.UntilTrained(model);
        return g;
    }

    [Fact]
    public void The_first_dollar_a_lab_earns_is_game_ai_licensed_to_game_studios()
    {
        Game g = WithTrainedMini(out ModelState mini);
        Assert.Equal(0, g.Me.Ledger.Lifetime.Revenue);

        g.Release(mini, "game-ai");
        g.Weeks(1);

        Assert.Equal(ModelStatus.Released, mini.Status);
        Assert.True(g.Me.LastWeek.Revenue > 0, "the first week on sale should already pay something");
        Assert.Equal(g.Me.LastWeek.Revenue, g.State.Sector("game-ai").Entry(g.Id).LastRevenue);
        Assert.Equal(g.Me.LastWeek.Revenue, g.Me.Ledger.LastWeek.Revenue);
        Assert.Equal(1995, g.Sim.Year);
    }

    [Fact]
    public void Customers_come_over_gradually_not_all_on_launch_day()
    {
        Game g = WithTrainedMini(out ModelState mini);
        g.Release(mini, "game-ai");

        g.Weeks(1);
        long week1 = g.Share("game-ai", g.Me);
        g.Weeks(9);
        long week10 = g.Share("game-ai", g.Me);
        g.Weeks(90);
        long week100 = g.Share("game-ai", g.Me);

        Assert.Equal(g.C.Market.SwitchRatePermille * 1000, week1);
        Assert.True(week10 > week1 && week10 < 600000, "after ten weeks the only seller has some of the market, got " + week10);
        Assert.True(week100 > 990000, "the only seller ends up with nearly all of it, got " + week100);
    }

    [Fact]
    public void A_weak_model_only_reaches_the_part_of_a_sector_that_has_a_use_for_it()
    {
        Game g = WithTrainedMini(out ModelState mini);
        g.Release(mini, "game-ai");
        g.Weeks(1);

        SectorConfig sector = g.C.Sector("game-ai");
        SectorState state = g.State.Sector("game-ai");
        long quality = LabQueries.Quality(g.C, sector, mini, g.Sim.Tick);
        Assert.Equal(quality * 1000 / sector.SaturationCapability, state.AdoptionPermille);
        Assert.InRange(state.AdoptionPermille, 100, 400);
    }

    [Fact]
    public void A_model_below_a_sectors_bar_or_ahead_of_its_time_cannot_be_sold_there()
    {
        Game g = WithTrainedMini(out ModelState mini);
        g.Refused(Command.ReleaseModel(g.Id, mini.Id, "expert-systems"), CommandError.SectorClosed);
        g.Refused(Command.ReleaseModel(g.Id, mini.Id, "game-ai", "game-ai"), CommandError.InvalidArgument);
        g.Refused(Command.ReleaseModel(g.Id, mini.Id), CommandError.InvalidArgument);
        g.Refused(Command.ReleaseModel(g.Id, mini.Id, "moon-mining"), CommandError.UnknownId);
        g.Refused(Command.ReleaseModel(g.Id, 12345, "game-ai"), CommandError.UnknownId);

        g.Years(1);
        Assert.Equal(1996, g.Sim.Year);
        // Expert systems are open now, but a Mini 1 is not good enough for a bank.
        g.Refused(Command.ReleaseModel(g.Id, mini.Id, "expert-systems"), CommandError.BelowSectorBar);

        // And naming one bad sector refuses the whole order: nothing is half-released.
        g.Refused(Command.ReleaseModel(g.Id, mini.Id, "game-ai", "expert-systems"), CommandError.BelowSectorBar);
        Assert.Equal(ModelStatus.Trained, mini.Status);
    }

    [Fact]
    public void A_model_still_in_training_cannot_be_released()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 0);
        g.Refused(Command.ReleaseModel(g.Id, model.Id, "game-ai"), CommandError.WrongModelStatus);
    }

    [Fact]
    public void Replacing_a_model_keeps_the_customers_and_retires_the_old_one()
    {
        Game g = WithTrainedMini(out ModelState first);
        g.Release(first, "game-ai", "game-bots");
        g.Weeks(20);
        long shareBefore = g.Share("game-ai", g.Me);

        ModelState second = g.Train("mini", 200);
        g.UntilTrained(second);
        long shareAtSwap = g.Share("game-ai", g.Me);
        Assert.True(shareAtSwap > shareBefore);

        g.Release(second, "game-ai");
        Assert.Equal(shareAtSwap, g.Share("game-ai", g.Me));
        Assert.Equal(ModelStatus.Released, first.Status);
        Assert.Equal(new[] { "game-bots" }, first.Sectors.ToArray());

        g.Release(second, "game-bots");
        Assert.Equal(ModelStatus.Retired, first.Status);
        Assert.Empty(first.Sectors);
        Assert.Equal(new[] { "game-ai", "game-bots" }, second.Sectors.ToArray());
    }

    [Fact]
    public void A_better_model_pulls_customers_away_from_a_worse_one()
    {
        Game g = Game.New(seed: 3);
        g.WithStarterLab(researchers: 3, engineers: 2, gpus: 10);
        ModelState mini = g.Train("mini", 100);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");

        // Then the player stops improving and cuts back to a skeleton crew, while the rivals keep shipping.
        g.Do(Command.Fire(g.Id, "researcher", 3));
        g.Do(Command.Fire(g.Id, "engineer", 2));
        g.Rent(2);
        g.Years(3);
        Assert.False(g.Me.Bankrupt);
        LabState best = g.State.Labs.Skip(1).OrderByDescending(l => g.Share("game-ai", l)).First();
        ModelState theirs = best.Model(g.State.Sector("game-ai").Entry(best.Id).ModelId);

        Assert.True(theirs.Capability > mini.Capability);
        Assert.True(g.Share("game-ai", best) > g.Share("game-ai", g.Me) * 3,
            best.Name + " should hold several times the share of a three-year-old Mini 1");
    }

    [Fact]
    public void Shares_in_a_sector_never_add_up_to_more_than_everyone()
    {
        Game g = Game.New(seed: 5);
        for (int week = 0; week < 6 * 48; week++)
        {
            g.Autopilot(1);
            foreach (SectorState sector in g.State.Sectors)
            {
                long total = sector.Entries.Sum(e => (long)e.SharePpm);
                Assert.InRange(total, 0, 1000000);
                Assert.All(sector.Entries, e => Assert.InRange(e.SharePpm, 0, 1000000));
                Assert.InRange(sector.AdoptionPermille, 0, 1000);
            }
        }
    }

    [Fact]
    public void A_lab_without_the_compute_to_serve_its_customers_earns_less_and_its_model_is_overworked()
    {
        Game roomy = WithTrainedMini(out ModelState a, alignment: 300);
        Game starved = WithTrainedMini(out ModelState b, alignment: 300);
        roomy.Release(a, "game-ai", "game-bots");
        starved.Release(b, "game-ai", "game-bots");
        roomy.Weeks(30);
        starved.Weeks(30);
        Assert.Equal(roomy.Me.LastWeek.Revenue, starved.Me.LastWeek.Revenue);
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(starved.C, starved.Me, b, starved.Sim.Tick));

        // Hand back all the GPUs: the customers are still there, but nothing can serve them.
        starved.Rent(0);
        starved.C.Safety.OverloadIncidentPermille = 0;
        roomy.Weeks(1);
        starved.Weeks(1);

        Assert.Equal(0, starved.Me.LastWeek.Revenue);
        Assert.Equal(0, starved.Me.LastWeek.ServedPermille);
        Assert.True(starved.Me.LastWeek.ServingDemand > 0);
        Assert.True(roomy.Me.LastWeek.Revenue > 0);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(starved.C, starved.Me, b, starved.Sim.Tick));
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(roomy.C, roomy.Me, a, roomy.Sim.Tick));
    }

    [Fact]
    public void A_lab_is_paid_exactly_what_its_sector_entries_say()
    {
        Game g = Game.New(seed: 9);
        g.Autopilot(3 * 48);
        foreach (LabState lab in g.State.Labs)
        {
            long fromEntries = g.State.Sectors.Sum(s => s.Entry(lab.Id) == null ? 0 : s.Entry(lab.Id).LastRevenue);
            Assert.Equal(lab.LastWeek.Revenue, fromEntries);
            Assert.Equal(lab.Ledger.LastWeek.Revenue, fromEntries);
        }
    }

    [Fact]
    public void Sales_staff_and_reputation_both_pull_customers()
    {
        Game g = Game.Solo().WithStarterLab(sales: 1);
        int one = MarketSystem.SalesPermille(g.C, g.Me);
        g.Hire("sales", 2);
        Assert.Equal(one + 2 * g.C.Market.SalesBonusPermille, MarketSystem.SalesPermille(g.C, g.Me));

        ModelState mini = g.Train("mini", 0);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");
        SectorConfig sector = g.C.Sector("game-ai");
        SectorState state = g.State.Sector("game-ai");
        long normal = MarketSystem.Appeal(g.C, g.State, sector, state, g.Me, mini);
        g.Me.Reputation = 200;
        Assert.True(MarketSystem.Appeal(g.C, g.State, sector, state, g.Me, mini) < normal);
    }

    [Fact]
    public void Sectors_grow_a_little_every_year()
    {
        Game g = Game.Solo();
        long before = g.State.Sector("game-ai").PoolPerWeek;
        g.Years(1);
        Assert.Equal(before * (1000 + g.C.Sector("game-ai").GrowthPermillePerYear) / 1000, g.State.Sector("game-ai").PoolPerWeek);
    }
}
