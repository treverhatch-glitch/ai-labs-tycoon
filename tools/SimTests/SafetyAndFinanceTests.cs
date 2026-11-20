using System.Linq;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Incidents and the safety meter; the ledger, debt and bankruptcy.</summary>
public class SafetyAndFinanceTests
{
    /// <summary>Plays a solo lab that releases one Mini with this much alignment work and sells it for some years.</summary>
    static Game SellOneMini(long seed, int alignmentShare, int years, out ModelState model)
    {
        Game g = Game.Solo(seed).WithStarterLab(gpus: 8);
        model = g.Train("mini", alignmentShare);
        g.UntilTrained(model);
        g.Release(model, "game-ai", "game-bots");
        g.Years(years);
        return g;
    }

    [Fact]
    public void A_model_with_a_full_safety_meter_never_has_an_incident()
    {
        Game g = SellOneMini(seed: 1, alignmentShare: 300, years: 10, out ModelState model);
        Assert.Equal(1000, model.Alignment);
        Assert.Equal(0, SafetySystem.IncidentChancePermille(g.C, model));
        Assert.Equal(0, model.Incidents);
        Assert.Equal(0, g.Me.Ledger.Lifetime.Fines);
    }

    [Fact]
    public void The_emptier_the_safety_meter_the_more_incidents()
    {
        int reckless = 0, middling = 0, careful = 0;
        for (long seed = 1; seed <= 20; seed++)
        {
            SellOneMini(seed, 0, 3, out ModelState a);
            SellOneMini(seed, 150, 3, out ModelState b);
            SellOneMini(seed, 300, 3, out ModelState c);
            reckless += a.Incidents;
            middling += b.Incidents;
            careful += c.Incidents;
        }

        // Twenty labs, three years each. An empty meter is a 3% chance a week: about four incidents per lab.
        Assert.InRange(reckless, 60, 120);
        Assert.True(middling < reckless * 3 / 4, "some alignment work should cut incidents clearly: " + middling + " vs " + reckless);
        Assert.True(middling > 0);
        Assert.Equal(0, careful);
    }

    [Fact]
    public void An_incident_costs_reputation_and_a_fine_and_the_model_glitches_for_a_while()
    {
        // Find the first incident of a reckless lab and look at the week it happened.
        Game g = Game.Solo(seed: 4).WithStarterLab(gpus: 8);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai", "game-bots");

        int reputationBefore = 0;
        long finesBefore = 0;
        g.Until(() =>
        {
            if (model.Incidents > 0) return true;
            reputationBefore = g.Me.Reputation;
            finesBefore = g.Me.Ledger.Lifetime.Fines;
            return false;
        }, 400, "the first incident");

        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        Assert.Equal(g.Sim.Tick - 1, model.LastIncidentTick);
        // The week's healing toward normal comes first, then the incident takes its bite.
        int healed = reputationBefore + System.Math.Sign(g.C.Safety.ReputationBaseline - reputationBefore) * g.C.Safety.ReputationRecoveryPerWeek;
        Assert.Equal(healed - g.C.Safety.IncidentReputationLoss, g.Me.Reputation);
        Assert.True(incident.Amount >= g.C.Safety.IncidentFineMin);
        Assert.Equal(finesBefore - incident.Amount, g.Me.Ledger.Lifetime.Fines);
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, model, g.Sim.Tick));

        string headline = EventText.Describe(g.C, g.State, incident);
        Assert.Contains("Fable Mini 1", headline);
        Assert.DoesNotContain("{model}", headline);

        // A glitching model is worth less to customers until it settles down.
        long glitching = LabQueries.Quality(g.C, g.C.Sector("game-ai"), model, g.Sim.Tick);
        long settled = LabQueries.Quality(g.C, g.C.Sector("game-ai"), model, model.LastIncidentTick + g.C.Safety.GlitchWeeks);
        Assert.Equal(settled * g.C.Safety.GlitchQualityPermille / 1000, glitching);
    }

    [Fact]
    public void Reputation_drifts_back_to_normal_when_a_lab_behaves()
    {
        Game g = Game.Solo();
        g.Me.Reputation = 300;
        g.Weeks(50);
        Assert.Equal(300 + 50 * g.C.Safety.ReputationRecoveryPerWeek, g.Me.Reputation);
        g.Me.Reputation = 900;
        g.Weeks(1000);
        Assert.Equal(g.C.Safety.ReputationBaseline, g.Me.Reputation);
    }

    [Fact]
    public void Cash_is_always_the_founding_cash_plus_every_ledger_line()
    {
        Game g = Game.New(seed: 2);
        for (int week = 0; week < 8 * 48; week++)
        {
            g.Autopilot(1);
            foreach (LabState lab in g.State.Labs)
            {
                Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
                Assert.True(lab.Ledger.Lifetime.Revenue >= 0 && lab.Ledger.Lifetime.Salaries <= 0);
            }
        }
        Assert.True(g.State.Labs.All(l => l.Ledger.Lifetime.Revenue > 0), "every lab sold something in eight years");
    }

    [Fact]
    public void The_weekly_bill_is_salaries_rooms_and_gpus()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 2, engineers: 1, sales: 1, gpus: 6);
        g.Weeks(1);
        long cashAfterFirstWeek = g.Me.Cash;
        g.Weeks(1);

        LedgerTotals week = g.Me.Ledger.LastWeek;
        Assert.Equal(-(2 * 1100 + 1000 + 800), week.Salaries);
        Assert.Equal(-LabQueries.WeeklyRoomUpkeep(g.C, g.Me), week.RoomUpkeep);
        Assert.Equal(-6 * g.C.Compute.GpuGenerations[0].RentPerGpuWeek, week.GpuRent);
        Assert.Equal(g.C.PlayerStart.GrantPerWeek, week.Grants);
        Assert.Equal(cashAfterFirstWeek + week.Net, g.Me.Cash);
        Assert.Equal(-LabQueries.WeeklyRunningCost(g.C, g.State, g.Me), week.Net - week.Grants);
        Assert.Equal(0, week.Construction + week.Hiring);
    }

    [Fact]
    public void The_grant_stops_after_its_last_week()
    {
        Game g = Game.Solo();
        int weeks = g.C.PlayerStart.GrantWeeks;
        g.Weeks(weeks);
        Assert.Equal(weeks * g.C.PlayerStart.GrantPerWeek, g.Me.Ledger.Lifetime.Grants);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GrantEnded);

        g.Weeks(10);
        Assert.Equal(weeks * g.C.PlayerStart.GrantPerWeek, g.Me.Ledger.Lifetime.Grants);
    }

    [Fact]
    public void Each_month_is_closed_into_the_books()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Years(1);

        Assert.Equal(12, g.Me.Ledger.History.Count);
        Assert.Equal(new[] { 1, 2, 3 }, g.Me.Ledger.History.Take(3).Select(m => m.Month).ToArray());
        Assert.All(g.Me.Ledger.History, m => Assert.Equal(1995, m.Year));
        Assert.Equal(g.Me.Cash, g.Me.Ledger.History.Last().EndCash);
        Assert.Equal(g.Me.Ledger.Lifetime.Net, g.Me.Ledger.History.Sum(m => m.Totals.Net));

        // The first month also holds what was spent setting up, before the first week was played.
        Assert.True(g.Me.Ledger.History[0].Totals.Construction < 0);
        Assert.Equal(0, g.Me.Ledger.History[1].Totals.Construction);
    }

    [Fact]
    public void The_books_keep_a_limited_number_of_months()
    {
        Game g = Game.Solo(tweak: c => c.Finance.HistoryMonths = 6);
        g.Years(2);
        Assert.Equal(6, g.Me.Ledger.History.Count);
        Assert.Equal(1996, g.Me.Ledger.History[0].Year);
        Assert.Equal(7, g.Me.Ledger.History[0].Month);
    }

    [Fact]
    public void A_lab_that_stays_in_debt_too_long_goes_bankrupt()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.GrantPerWeek = 0).WithStarterLab(researchers: 4, engineers: 4, gpus: 40);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");

        g.Until(() => g.Me.Cash < 0, 100, "running out of money");
        Assert.False(g.Me.Bankrupt);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.InDebt);

        // A lab in debt can still act: letting people go is how it gets out.
        g.Do(Command.Fire(g.Id, "engineer", 1));

        g.Weeks(g.C.Finance.BankruptcyGraceWeeks - 2);
        Assert.False(g.Me.Bankrupt);
        g.Weeks(1);
        Assert.True(g.Me.Bankrupt);
        Assert.Equal(ModelStatus.Retired, model.Status);
        Assert.Empty(g.State.Sector("game-ai").Entries);
        Assert.Equal(0, LabQueries.TotalStaff(g.Me));
        Assert.Equal(0, g.Me.RentedGpus);
        g.Refused(Command.SetRentedGpus(g.Id, 1), CommandError.LabBankrupt);

        // The world goes on, and the closed lab neither earns nor pays.
        long cash = g.Me.Cash;
        g.Weeks(20);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void Getting_back_above_zero_resets_the_debt_clock()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 0);
        g.Me.Cash = -1000;
        g.Me.Ledger.StartCash = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.Fire(g.Id, "researcher", 2));
        g.Do(Command.Fire(g.Id, "engineer", 1));
        g.Do(Command.Fire(g.Id, "sales", 1));

        g.Weeks(1);
        Assert.Equal(1, g.Me.WeeksInDebt);
        g.Until(() => g.Me.Cash >= 0, 20, "the grant pulling the lab out of debt");
        Assert.Equal(0, g.Me.WeeksInDebt);
        Assert.False(g.Me.Bankrupt);
    }
}
