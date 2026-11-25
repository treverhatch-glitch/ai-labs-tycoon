using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;
using Xunit.Abstractions;

namespace AiLabs.SimTests;

/// <summary>
/// Whole games, told start to finish. Run with <c>dotnet test tools/SimTests --logger "console;verbosity=detailed"</c>
/// to read the stories as they are played.
/// </summary>
public class StoryTests
{
    readonly ITestOutputHelper _out;

    public StoryTests(ITestOutputHelper output) { _out = output; }

    void YearEnd(Game g)
    {
        _out.WriteLine("");
        _out.WriteLine("-- end of " + (g.Sim.Year - 1) + " --");
        foreach (LabState lab in g.State.Labs)
        {
            ModelState best = LabQueries.BestReleased(lab);
            _out.WriteLine(lab.Name.PadRight(13) + " cash $" + lab.Cash.ToString().PadLeft(9) + "  revenue/wk $" + lab.LastWeek.Revenue.ToString().PadLeft(6)
                + "  costs/wk $" + LabQueries.WeeklyRunningCost(g.C, g.State, lab).ToString().PadLeft(6) + "  staff " + LabQueries.TotalStaff(lab).ToString().PadLeft(2)
                + "  GPUs " + LabQueries.TotalGpus(lab).ToString().PadLeft(3) + "  rep " + lab.Reputation
                + "  " + (lab.Bankrupt ? "BANKRUPT" : best == null ? "nothing on sale" : best.Name + " (capability " + best.Capability + ", safety " + best.Alignment + ")"));
        }
        if (g.State.Gauntlets.Count > 0) _out.WriteLine("crown: " + g.CrownHolder().Name);
    }

    void Tell(Game g, ref int shown, params EventKind[] kinds)
    {
        for (; shown < g.State.Events.Count; shown++)
        {
            GameEvent e = g.State.Events[shown];
            if (e.LabId == g.Id && (kinds.Length == 0 || kinds.Contains(e.Kind))) _out.WriteLine(EventText.Describe(g.C, g.State, e));
        }
    }

    [Fact]
    public void Five_years_from_a_garage_in_1995_a_sensibly_run_lab_is_solvent_and_selling()
    {
        Game g = Game.New(seed: 1995);
        int shown = 0;
        long lowestCash = g.Me.Cash;

        // What the boss wants researched, in order, and which models to train next.
        var shoppingList = new Queue<string>(new[]
        {
            "state-machines", "minimax", "pathfinding", "regression-tests", "rules-engines", "expert-shells",
            "decision-trees", "opening-books", "beowulf-clusters", "playtest-telemetry", "spam-filters", "search-ranking"
        });
        var trainingPlan = new Queue<string>(new[] { "mini", "mini", "mini", "mid", "mid", "mid", "mid", "mid", "mid", "mid", "mid" });

        // The weekly routine between the big decisions: buy the next tech, ship what is ready, keep one run going.
        void Play(int weeks)
        {
            for (int i = 0; i < weeks; i++)
            {
                if (shoppingList.Count > 0 && g.Me.ResearchPoints >= g.C.Tech(shoppingList.Peek()).Cost) g.Research(shoppingList.Dequeue());

                foreach (ModelState ready in g.Me.Models.Where(m => m.Status == ModelStatus.Trained).ToList())
                {
                    string[] sectors = g.C.Market.Sectors
                        .Where(s => LabQueries.SectorOpen(g.C, s, g.Sim.Tick) && ready.Capability >= s.MinCapability)
                        .Select(s => s.Id).ToArray();
                    g.Release(ready, sectors);
                }

                if (LabQueries.ActiveRuns(g.Me) == 0 && trainingPlan.Count > 0 && LabQueries.LineUnlocked(g.C, g.Me, trainingPlan.Peek())
                    && g.Me.Cash > g.C.Line(trainingPlan.Peek()).PrepCost * 2)
                    g.Train(trainingPlan.Dequeue(), 200);

                g.Weeks(1);
                Tell(g, ref shown, EventKind.TechResearched, EventKind.TrainingFinished, EventKind.ModelReleased, EventKind.Incident, EventKind.GauntletCrown, EventKind.LotBought);
                if (g.Me.Cash < lowestCash) lowestCash = g.Me.Cash;
                Assert.True(g.Me.Cash >= 0, "in debt in " + g.Sim.DateLabel);
            }
        }

        // 1995: move into the garage. Desks, a small team, rented compute, and the first Mini in training.
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 3);
        g.Build("sales-office", 3, 3);
        g.Build("safety-lab", 3, 3);
        g.Hire("researcher", 3);
        g.Hire("engineer", 2);
        g.Hire("sales", 1);
        g.Rent(8);
        Play(48);
        YearEnd(g);

        ModelState firstModel = g.Me.Models[0];
        Assert.Equal("Fable Mini 1", firstModel.Name);
        Assert.Equal(1995, GameDate.Year(g.C.Calendar, firstModel.ReleasedTick));
        Assert.True(g.Me.Ledger.Lifetime.Revenue > 0, "the lab licensed game AI in its first year");
        Assert.True(g.State.Sector("game-ai").Entry(g.Id) != null);

        // 1996: revenue is coming in. Add a safety tester and a researcher, and more compute.
        g.Hire("safety", 1);
        g.Hire("researcher", 1);
        g.Rent(12);
        Play(48);
        YearEnd(g);

        // 1997: the garage is full. Take the strip-mall unit and grow the team.
        g.Do(Command.BuyLot(g.Id));
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Hire("researcher", 3);
        g.Hire("engineer", 3);
        g.Hire("sales", 1);
        g.Rent(18);
        Play(48);
        YearEnd(g);

        // 1998: buy the lab's own machines for the base load and keep renting the rest.
        g.Build("server-closet", 3, 3);
        g.Do(Command.BuyGpus(g.Id, 18));
        g.Rent(8);
        g.Hire("safety", 1);
        Play(48);
        YearEnd(g);

        // 1999: spam filtering and search open up. More researchers to get there.
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 4);
        g.Rent(14);
        Play(48);
        YearEnd(g);

        _out.WriteLine("");
        _out.WriteLine("lowest cash on the way: $" + lowestCash);

        Assert.Equal(2000, g.Sim.Year);
        Assert.False(g.Me.Bankrupt);
        Assert.Equal(0, g.Me.WeeksInDebt);
        Assert.True(lowestCash >= 0);
        Assert.True(g.Me.Cash > g.C.PlayerStart.Cash, "five years on the lab should have more than it started with, has $" + g.Me.Cash);
        Assert.True(g.Me.LastWeek.Revenue > LabQueries.WeeklyRunningCost(g.C, g.State, g.Me), "and earn more a week than it spends");

        ModelState best = LabQueries.BestReleased(g.Me);
        Assert.Equal("mid", best.LineId);
        Assert.True(best.Alignment >= 800, "careful work keeps the safety meter high");
        Assert.True(g.State.Sectors.Count(s => s.Entry(g.Id) != null) >= 3, "the lab grew out of games into expert systems and beyond");
        Assert.True(g.Me.Models.Count(m => m.Status == ModelStatus.Retired) >= 2, "older models were replaced along the way");
        Assert.Equal(5, g.State.Gauntlets.Count);
        Assert.Equal(60, g.Me.Ledger.History.Count);
        Assert.Equal(g.Me.Ledger.StartCash + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
    }

    [Fact]
    public void A_rival_overtakes_a_lab_that_stops_investing()
    {
        Game g = Game.New(seed: 7, tweak: c => c.PlayerStart.Cash = 400000);
        int shown = 0;

        // A strong first year: a bigger team and more compute than any rival starts with, and a new Mini every few weeks.
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("sales-office", 3, 3);
        g.Hire("researcher", 4);
        g.Hire("engineer", 4);
        g.Hire("sales", 2);
        g.Rent(14);
        var shoppingList = new Queue<string>(new[] { "state-machines", "minimax", "pathfinding", "rules-engines" });
        for (int week = 0; week < 48; week++)
        {
            if (shoppingList.Count > 0 && g.Me.ResearchPoints >= g.C.Tech(shoppingList.Peek()).Cost) g.Research(shoppingList.Dequeue());
            foreach (ModelState ready in g.Me.Models.Where(m => m.Status == ModelStatus.Trained).ToList())
                g.Release(ready, "game-ai", "game-bots");
            if (LabQueries.ActiveRuns(g.Me) == 0) g.Train("mini", 150);
            g.Weeks(1);
        }
        Tell(g, ref shown, EventKind.TechResearched, EventKind.ModelReleased, EventKind.GauntletCrown);
        YearEnd(g);

        long shareAtPeak = g.Share("game-ai", g.Me);
        long revenueAtPeak = g.Me.LastWeek.Revenue;
        Assert.True(g.State.Labs.Skip(1).All(l => g.Share("game-ai", l) < shareAtPeak), "the player leads game AI after year one");
        Assert.True(g.State.Labs.Skip(1).All(l => l.LastWeek.Revenue < revenueAtPeak), "and out-earns every rival");

        // Then the boss banks the profits: no research, no new models, the engineers let go, most GPUs handed back.
        g.Do(Command.Fire(g.Id, "researcher", 4));
        g.Do(Command.Fire(g.Id, "engineer", 4));
        g.Rent(3);
        for (int year = 0; year < 3; year++)
        {
            g.Years(1);
            YearEnd(g);
        }

        LabState crown = g.CrownHolder();
        Assert.NotEqual(g.Id, crown.Id);
        Assert.False(g.Me.Bankrupt, "the lab is still alive; it just stopped mattering");
        Assert.True(g.State.Gauntlets.Skip(2).All(r => r.CrownLabId != g.Id), "the crown is out of reach now");
        Assert.True(LabQueries.BestReleased(crown).Capability > LabQueries.BestReleased(g.Me).Capability * 2);

        long shareNow = g.Share("game-ai", g.Me);
        Assert.True(shareNow < shareAtPeak / 3, "share fell from " + shareAtPeak + " to " + shareNow);
        Assert.True(g.State.Labs.Skip(1).Count(l => g.Share("game-ai", l) > shareNow) >= 2, "at least two rivals now outsell the player");
        Assert.True(g.Me.LastWeek.Revenue < revenueAtPeak, "and the money shows it");
        Assert.Null(g.State.Sector("expert-systems").Entry(g.Id));
        Assert.True(g.State.Sector("expert-systems").Entries.Count >= 2, "the rivals moved on to expert systems without the player");
    }

    [Fact]
    public void Two_labs_alike_in_everything_but_care_end_up_with_different_reputations()
    {
        // Same seed, same world, same orders. One boss spends a quarter of every run on alignment; the other spends nothing.
        void SameWorld(GameConfig c)
        {
            c.Naming.ConfusionPermille = 0;
            c.Safety.SectorFlavoursForRivals = false;
            c.Safety.OverloadIncidentPermille = 0;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
        }
        Game careful = Game.New(seed: 44, tweak: SameWorld);
        Game reckless = Game.New(seed: 44, tweak: SameWorld);
        PlanConfig carefulPlan = TestData.Config().Autopilot;
        PlanConfig recklessPlan = TestData.Config().Autopilot;
        carefulPlan.AlignmentSharePermille = 250;
        recklessPlan.AlignmentSharePermille = 0;

        careful.Autopilot(6 * 48, carefulPlan);
        reckless.Autopilot(6 * 48, recklessPlan);
        YearEnd(careful);
        YearEnd(reckless);

        int carefulIncidents = careful.Me.Models.Sum(m => m.Incidents);
        int recklessIncidents = reckless.Me.Models.Sum(m => m.Incidents);
        _out.WriteLine("incidents: careful " + carefulIncidents + ", reckless " + recklessIncidents);
        _out.WriteLine("fines: careful $" + -careful.Me.Ledger.Lifetime.Fines + ", reckless $" + -reckless.Me.Ledger.Lifetime.Fines);

        Assert.True(recklessIncidents >= 5, "six years of unaligned models should cause trouble, got " + recklessIncidents);
        Assert.True(carefulIncidents <= recklessIncidents / 4);
        Assert.True(reckless.Me.Ledger.Lifetime.Fines < careful.Me.Ledger.Lifetime.Fines, "the reckless lab paid more in fines");
        Assert.True(careful.State.Gauntlets.Count(r => r.SafestLabId == careful.Id) > reckless.State.Gauntlets.Count(r => r.SafestLabId == reckless.Id));

        Assert.True(reckless.Me.Reputation < careful.Me.Reputation);

        // The reckless lab's first model was done sooner. That is the temptation.
        Assert.True(reckless.Me.Models[0].TrainedTick < careful.Me.Models[0].TrainedTick);
    }

    [Fact]
    public void A_boss_who_spends_like_the_money_is_infinite_is_bankrupt_within_two_years()
    {
        Game g = Game.New(seed: 13, tweak: c => c.PlayerStart.Cash = 450000);
        g.Do(Command.BuyLot(g.Id));
        g.Build("research-lab", 6, 6);
        g.Build("engineering-bay", 6, 6);
        g.Hire("researcher", 9);
        g.Hire("engineer", 9);
        g.Rent(60);
        ModelState mini = g.Train("mini", 0);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai", "game-bots");

        int weeks = g.Until(() => g.Me.Bankrupt, 96, "bankruptcy");
        _out.WriteLine("bankrupt in " + g.Sim.DateLabel + " after " + g.Sim.Tick + " weeks, owing $" + -g.Me.Cash);
        _out.WriteLine(EventText.Describe(g.C, g.State, g.State.Events.Last(e => e.Kind == EventKind.Bankrupt)));

        Assert.True(g.Me.Ledger.Lifetime.Revenue > 0, "it did sell something first");
        Assert.True(weeks > g.C.Finance.BankruptcyGraceWeeks, "there was a warning period");

        // The world carries on without the player.
        g.Years(3);
        Assert.Contains(g.State.Labs.Skip(1), l => LabQueries.BestReleased(l) != null && LabQueries.BestReleased(l).LineId != "mini");
        Assert.Equal(ModelStatus.Retired, mini.Status);
    }

    [Fact]
    public void Across_forty_worlds_the_autopilot_stays_solvent_and_no_lab_always_wins()
    {
        // Six years, balanced preset only — kept for story output alongside BalanceTests.
        var crowns = new Dictionary<string, int>();
        int solvent = 0;
        int rivalsAlive = 0;
        const int Games = 40;
        GameConfig c = TestData.Config();
        BalanceSoak.Preset balanced = BalanceSoak.DefaultPresets(c)[0];

        for (long seed = 100; seed < 100 + Games; seed++)
        {
            Game g = Game.New(seed);
            for (int week = 0; week < 6 * c.Calendar.WeeksPerYear; week++)
            {
                if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, balanced.Plan);
                g.Weeks(1);
            }
            if (!g.Me.Bankrupt) solvent++;
            rivalsAlive += g.State.Labs.Skip(1).Count(l => !l.Bankrupt);
            string winner = g.CrownHolder().Name;
            crowns[winner] = crowns.TryGetValue(winner, out int n) ? n + 1 : 1;
        }

        foreach (KeyValuePair<string, int> entry in crowns.OrderByDescending(e => e.Value))
            _out.WriteLine(entry.Key.PadRight(13) + " crown in " + entry.Value + " of " + Games + " worlds");
        _out.WriteLine("autopilot solvent in " + solvent + " of " + Games + "; rivals alive " + rivalsAlive + " of " + Games * 3);

        Assert.True(solvent >= Games * 9 / 10, "the autopilot went bankrupt in " + (Games - solvent) + " of " + Games + " worlds");
        Assert.True(crowns.Count >= 2, "one lab won every world: the game is decided before it starts");
        Assert.True(crowns.Values.Max() <= Games * 9 / 10, "one lab wins nearly always");
        Assert.True(rivalsAlive >= Games * 3 * 3 / 4, "rivals should usually survive six years");
    }
}
