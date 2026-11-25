using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DatasetsUserDataGovernanceBuybackSpotTogetherTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Safety.SectorFlavours.Clear();
        c.Chiefs.MinStaff = 0;
        c.Support.Enabled = 0;
        c.Security.Enabled = 0;
        c.Tax.Enabled = 0;
        c.Standards.Enabled = 0;
        c.Reliability.Enabled = 0;
        c.GovContracts.Enabled = 0;
        c.Shocks.Enabled = 0;
        foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
    }

    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(4, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 600;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static void SuitTick(Game g)
    {
        int w = g.C.Datasets.SuitWeeks;
        int tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.State.Tick = tick - tick % w;
    }

    static void ComplaintTick(Game g)
    {
        int w = g.C.UserData.ComplaintWeeks;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.State.Tick -= g.State.Tick % w;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void Independent_directors_take_a_share_of_the_fine_off_a_data_suit_each()
    {
        Game plain = Lab();
        plain.Do(Command.AcquireData(plain.Id, 1, 60));
        long before = plain.Me.Ledger.Lifetime.Fines;
        SuitTick(plain);
        Datasets.RunWeek(plain.C, plain.State, plain.Me);
        Assert.Equal(before - 60L * plain.C.Datasets.SuitFine, plain.Me.Ledger.Lifetime.Fines);

        Game led = Lab();
        led.Do(Command.AcquireData(led.Id, 1, 60));
        led.Me.GovDirectors = 2;
        long beforeLed = led.Me.Ledger.Lifetime.Fines;
        SuitTick(led);
        Datasets.RunWeek(led.C, led.State, led.Me);
        long full = 60L * led.C.Datasets.SuitFine;
        Assert.Equal(beforeLed - full * (1000 - 2 * led.C.Datasets.DirectorSuitPermille) / 1000, led.Me.Ledger.Lifetime.Fines);

        Game off = Lab(c => c.Governance.Enabled = 0);
        off.Do(Command.AcquireData(off.Id, 1, 60));
        off.Me.GovDirectors = 2;
        long beforeOff = off.Me.Ledger.Lifetime.Fines;
        SuitTick(off);
        Datasets.RunWeek(off.C, off.State, off.Me);
        Assert.Equal(beforeOff - full, off.Me.Ledger.Lifetime.Fines);
    }

    [Fact]
    public void A_privacy_complaint_costs_the_lab_an_independent_director()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.SetUserData(g.Id, 2));
        g.Me.GovDirectors = 2;
        g.Me.UserBanked = g.C.UserData.ComplaintMin + 10;
        ComplaintTick(g);
        g.Me.LastWeek.Revenue = 0;
        UserData.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.UserComplaints);
        Assert.Equal(1, g.Me.GovDirectors);
        Assert.Equal(1, g.Me.GovResigned);
        Assert.Equal(1, Count(g, EventKind.DirectorResigned));

        Game none = Lab();
        Selling(none);
        none.Do(Command.SetUserData(none.Id, 2));
        none.Me.UserBanked = none.C.UserData.ComplaintMin + 10;
        ComplaintTick(none);
        none.Me.LastWeek.Revenue = 0;
        UserData.RunWeek(none.C, none.State, none.Me);
        Assert.Equal(1, none.Me.UserComplaints);
        Assert.Equal(0, Count(none, EventKind.DirectorResigned));

        Game off = Lab(c => c.UserData.ComplaintResigns = 0);
        Selling(off);
        off.Do(Command.SetUserData(off.Id, 2));
        off.Me.GovDirectors = 2;
        off.Me.UserBanked = off.C.UserData.ComplaintMin + 10;
        ComplaintTick(off);
        off.Me.LastWeek.Revenue = 0;
        UserData.RunWeek(off.C, off.State, off.Me);
        Assert.Equal(2, off.Me.GovDirectors);
    }

    [Fact]
    public void A_reclaim_wave_holds_a_cleaning_job_for_as_many_weeks_as_it_lasts()
    {
        Game g = Lab();
        g.Do(Command.AcquireData(g.Id, 1, 50));
        g.Do(Command.CleanData(g.Id, 1, 50));
        int left = g.Me.DataJobLeft;
        g.Me.SpotOut = 2;
        Datasets.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(left, g.Me.DataJobLeft);
        g.Me.SpotOut = 0;
        Datasets.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(left - 1, g.Me.DataJobLeft);

        Game off = Lab(c => c.Datasets.WaveStallsCleaning = 0);
        off.Do(Command.AcquireData(off.Id, 1, 50));
        off.Do(Command.CleanData(off.Id, 1, 50));
        off.Me.SpotOut = 2;
        Datasets.RunWeek(off.C, off.State, off.Me);
        Assert.Equal(left - 1, off.Me.DataJobLeft);
    }

    [Fact]
    public void A_real_wave_delays_the_end_of_a_cleaning_job()
    {
        int Weeks(bool spot)
        {
            Game g = Lab(c => { c.Spot.PreemptPermille = 1000; c.Spot.MaxPermille = 1000; });
            g.Train("mini", 0);
            g.Do(Command.AcquireData(g.Id, 1, 50));
            g.Do(Command.CleanData(g.Id, 1, 50));
            if (spot) g.Do(Command.SetSpotShare(g.Id, 1000));
            int w = 0;
            while (g.Me.DataJobLeft > 0 && w < 40) { g.Weeks(1); w++; }
            return w;
        }
        int calm = Weeks(false);
        int stormy = Weeks(true);
        Assert.True(stormy > calm);
    }

    [Fact]
    public void Directors_will_not_back_a_buyback_in_the_middle_of_a_reclaim_wave()
    {
        Game g = Lab(c => { c.Funding.Round("angel").DilutionPermille = 400; c.Funding.Round("series-a").DilutionPermille = 400; c.Buyback.DirectorVote = 300; });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        g.Me.FounderSharePermille = 300;
        g.Me.GovDirectors = 2;
        FundingRoundConfig angel = g.C.Funding.Round("angel");
        g.Me.Cash = Buyback.Price(g.C, g.State, angel) * 1000;
        long calm = Buyback.YesVotes(g.C, g.State, g.Me, angel);
        g.Me.SpotOut = 1;
        long wave = Buyback.YesVotes(g.C, g.State, g.Me, angel);
        Assert.Equal(calm - 2 * g.C.Buyback.DirectorVote, wave);
        Assert.True(Buyback.Passes(g.C, g.State, g.Me, angel) == (wave * 1000 > Buyback.TotalVotes(g.C, g.Me) * g.C.Buyback.PassPermille));
        Game off = Lab(c => { c.Funding.Round("angel").DilutionPermille = 400; c.Funding.Round("series-a").DilutionPermille = 400; c.Buyback.DirectorVote = 300; c.Buyback.WaveBlocksDirectors = 0; });
        off.Do(Command.RaiseFunding(off.Id, "angel"));
        off.Do(Command.RaiseFunding(off.Id, "series-a"));
        off.Me.FounderSharePermille = 300;
        off.Me.GovDirectors = 2;
        off.Me.Cash = Buyback.Price(off.C, off.State, angel) * 1000;
        off.Me.SpotOut = 1;
        Assert.Equal(calm, Buyback.YesVotes(off.C, off.State, off.Me, off.C.Funding.Round("angel")));
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Game.New(5, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Assert.Equal(0, g.Me.DataRaw1 + g.Me.DataRaw2 + g.Me.DataRaw3 + g.Me.DataRaw4 + g.Me.DataSuits + g.Me.UserPolicy + g.Me.UserTotal + g.Me.GovForm + g.Me.GovDirectors + g.Me.GovSearch + g.Me.BoughtRounds.Count + g.Me.BoardWon + g.Me.BoardLost + g.Me.SpotPermille + g.Me.SpotOut + g.Me.SpotWaves);
        foreach (EventKind k in new[] { EventKind.DataAcquired, EventKind.DataSuit, EventKind.UserDataChanged, EventKind.UserDataComplaint, EventKind.GovernanceChanged, EventKind.DirectorSeated, EventKind.DirectorResigned, EventKind.BoardVoted, EventKind.RoundBoughtBack, EventKind.SpotPreempted })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.AcquireData(g.Id, 1, 60));
        g.Do(Command.AcquireData(g.Id, 2, 30));
        g.Do(Command.CleanData(g.Id, 1, 40));
        g.Do(Command.SetUserData(g.Id, 2));
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.SeatDirector(g.Id));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        g.Do(Command.SetSpotShare(g.Id, 400));
        g.Train("mini", 0);
        g.Weeks(8);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Dual_class_directors_and_a_buyback_play_out_in_order_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.SeatDirector(g.Id));
        g.Weeks(g.C.Governance.DirectorWeeks);
        Assert.Equal(1, g.Me.GovDirectors);
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        Assert.True(Governance.ControlShare(g.C, g.Me) > g.Me.FounderSharePermille);
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        Assert.Equal(1, g.Me.BoardWon);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 12);
        g.State.Tick = (2022 - 1995) * g.C.Calendar.WeeksPerYear;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int bought = 0, cleaned = 0, policy = 0, directors = 0, spot = 0, rounds = 0;
        string[] roundIds = g.C.Funding.Rounds.Select(r => r.Id).ToArray();
        for (int week = 0; week < 300 && !g.Me.Bankrupt; week++)
        {
            g.Me.ResearchPoints += 20_000;
            if (week % 25 == 3) g.Try(Command.StartTraining(g.Id, "mini", 0));
            if (week % 7 == 1) g.Try(Command.AcquireData(g.Id, 1 + week / 7 % 3, 40));
            if (week % 9 == 2 && g.Try(Command.CleanData(g.Id, 1 + week / 9 % 3, 30)).Ok) cleaned++;
            if (week % 40 == 5) g.Try(Command.LicenseScraped(g.Id, 20));
            if (week % 30 == 4 && g.Try(Command.SetUserData(g.Id, week / 30 % 3)).Ok) policy++;
            if (week % 55 == 6 && g.Try(Command.RaiseFunding(g.Id, roundIds[week / 55 % roundIds.Length])).Ok) rounds++;
            if (week == 20) g.Try(Command.SetGovernance(g.Id, 1));
            if (week % 35 == 9 && g.Try(Command.SeatDirector(g.Id)).Ok) directors++;
            if (week % 45 == 12 && g.Try(Command.BuyBackRound(g.Id, roundIds[week / 45 % roundIds.Length])).Ok) bought++;
            if (week % 20 == 3 && g.Try(Command.SetSpotShare(g.Id, week / 20 % 4 * 200)).Ok) spot++;
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(cleaned > 0, "no cleaning job was ever started");
        Assert.True(policy > 0, "no users data policy was ever set");
        Assert.True(rounds > 0, "no round was ever raised");
        Assert.True(directors > 0, "no director was ever sought");
        Assert.True(spot > 0, "no spot share was ever set");
        Assert.Equal(bought, g.Me.BoardWon);
    }
}
