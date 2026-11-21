using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DevicesPlatformsCreedShocksReplicationTogetherTests
{
    static Game Lab(Action<GameConfig> more = null, int year = 2027)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Shocks.Enabled = 1;
            c.PlayerStart.GrantPerWeek = 400_000;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.State.Tick = (year - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "Together" });
        g.Me.Reputation = 500;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Strike(Game g, string id) { Shocks.OnWorldEvent(g.C, g.State, g.C.WorldEvents.FindIndex(e => e.Id == id)); }

    [Fact]
    public void Signing_a_platform_deal_and_renewing_it_keep_a_profit_creed()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Profit, Creed.Openness));
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(2);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
        g.Weeks(g.C.Platforms.TermWeeks - g.C.Platforms.WindowWeeks);
        int before = g.Me.CreedIntegrity;
        g.Do(Command.RenewPlatform(g.Id));
        g.Weeks(2);
        Assert.Equal(before + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
    }

    [Fact]
    public void A_recalled_device_breaks_a_safety_creed_and_designing_one_keeps_a_speed_creed()
    {
        Game safe = Lab();
        safe.Do(Command.SetCreed(safe.Id, Creed.Safety, Creed.Profit));
        Events.Add(safe.State, EventKind.DeviceRecalled, safe.Id, 1, 100, "");
        safe.Weeks(2);
        Assert.Equal(safe.C.Creed.StartIntegrity - safe.C.Creed.BrokenPoints, safe.Me.CreedIntegrity);
        Game fast = Lab();
        fast.Do(Command.SetCreed(fast.Id, Creed.Speed, Creed.People));
        fast.Do(Command.DesignDevice(fast.Id, 1, 0));
        fast.Weeks(2);
        Assert.Equal(fast.C.Creed.StartIntegrity + fast.C.Creed.KeptPoints, fast.Me.CreedIntegrity);
    }

    [Fact]
    public void Putting_the_code_out_keeps_an_openness_creed_and_keeping_it_back_breaks_it()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Openness, Creed.Profit));
        g.Me.ReplPending = 2;
        g.Do(Command.ReleaseCode(g.Id, 0));
        g.Weeks(2);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints, g.Me.CreedIntegrity);
        g.Do(Command.WithholdCode(g.Id));
        g.Weeks(2);
        Assert.Equal(g.C.Creed.StartIntegrity + g.C.Creed.KeptPoints - g.C.Creed.BrokenPoints, g.Me.CreedIntegrity);
    }

    static int BacklashLoss(int band)
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        g.Me.CreedBand = band;
        Strike(g, "pause-letter");
        g.Do(Command.AnswerShock(g.Id, 2));
        return 500 - g.Me.Reputation;
    }

    [Fact]
    public void A_lab_held_up_as_an_example_takes_less_of_a_backlash_and_one_with_a_joke_of_a_creed_takes_more()
    {
        Game g = Lab();
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        int proud = BacklashLoss(1);
        int plain = BacklashLoss(0);
        int joke = BacklashLoss(2);
        Assert.True(proud < plain);
        Assert.True(plain < joke);
        Assert.Equal(PressSystem.Damp(g.C, g.Me, (int)MathI.MulPermille(g.C.Shocks.HitReputation, g.C.Creed.ShieldPermille)), proud);
        Assert.Equal(PressSystem.Damp(g.C, g.Me, g.C.Shocks.HitReputation), plain);
        Assert.Equal(PressSystem.Damp(g.C, g.Me, (int)MathI.MulPermille(g.C.Shocks.HitReputation, g.C.Creed.ExposePermille)), joke);
        Game none = Lab(c => c.Creed.Enabled = 0);
        none.Me.CreedBand = 1;
        Assert.Equal(MathI.Permille, Creed.Exposure(none.C, none.Me));
    }

    [Fact]
    public void A_squeeze_and_a_creed_that_is_a_joke_cannot_push_morale_below_nothing()
    {
        Game g = Lab(c => { c.Creed.BrokenPoints = 500; c.Creed.StartIntegrity = 400; c.Shocks.HitMorale = 1000; });
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Openness));
        foreach (StaffMember s in g.Me.Staff) s.Morale = 50;
        Events.Add(g.State, EventKind.ModelRushed, g.Id, 0, 0, "");
        g.Weeks(2);
        Assert.Equal(2, g.Me.CreedBand);
        Strike(g, "card-shortage");
        g.Do(Command.AnswerShock(g.Id, 2));
        Assert.All(g.Me.Staff, s => Assert.True(s.Morale >= 0 && s.Morale <= MathI.Permille));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Device_sales_and_a_platform_lift_both_reach_the_books_without_a_gap()
    {
        Game g = Lab();
        g.Do(Command.DesignDevice(g.Id, 1, 3));
        g.Do(Command.SignPlatform(g.Id, 2));
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Weeks(g.C.Devices.WeeksA + 3 + 12);
        Assert.True(g.Me.DevSold > 0);
        Assert.True(g.Me.Ledger.Lifetime.Revenue > 0);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab();
        g.Do(Command.DesignDevice(g.Id, 1, 1));
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Do(Command.SetCreed(g.Id, Creed.Profit, Creed.People));
        Strike(g, "talent-war");
        g.Me.ReplPending = 1;
        g.Do(Command.ReleaseCode(g.Id, 1));
        g.Weeks(5);
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
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, c =>
        {
            c.Platforms.EraA = 0;
            c.Platforms.EraB = 0;
            c.Platforms.EraC = 0;
            c.Platforms.MinCapability = 0;
            c.Platforms.MinReputation = 0;
            c.Devices.EraA = 0;
            c.Devices.EraB = 0;
            c.Devices.EraC = 0;
            c.Devices.MinCapability = 0;
        });
        g.Autopilot(60);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        string[] news = { "talent-war", "pause-letter", "hundred-million" };
        for (int week = 0; week < 330 && !g.Me.Bankrupt; week++)
        {
            if (week % 50 == 3) g.Try(Command.SignPlatform(g.Id, 1 + week / 50 % 3));
            if (week % 50 == 40) g.Try(Command.RenewPlatform(g.Id));
            if (week % 110 == 80) g.Try(Command.WalkFromPlatform(g.Id));
            if (week % 90 == 10) g.Try(Command.DesignDevice(g.Id, 1 + week / 90 % 3, week / 90 % 4));
            if (week % 25 == 5) g.Try(Command.SetDevicePrice(g.Id, 1 + week / 25 % 3));
            if (week % 140 == 120) g.Try(Command.WithdrawDevice(g.Id));
            if (week % 120 == 15) g.Try(Command.SetCreed(g.Id, 1 + week / 120 % 5, 1 + (week / 120 + 2) % 5));
            if (week % 40 == 22) g.Try(Command.HoldTownHall(g.Id));
            if (week % 100 == 60) g.Try(Command.OwnUp(g.Id));
            if (week % 45 == 7) Strike(g, news[week / 45 % 3]);
            if (week % 45 == 14) g.Try(Command.AnswerShock(g.Id, 1 + week / 45 % 3));
            if (week % 35 == 6) g.Try(Command.PublishPaper(g.Id));
            if (week % 35 == 9) g.Try(Command.ReleaseCode(g.Id, week / 35 % 4));
            if (week % 35 == 20) g.Try(Command.WithholdCode(g.Id));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(Count(g, EventKind.PlatformSigned) > 0, "no platform deal was ever signed");
        Assert.True(Count(g, EventKind.DeviceDesigned) > 0, "no device was ever designed");
        Assert.True(Count(g, EventKind.CreedSet) > 0, "no creed was ever stated");
        Assert.True(Count(g, EventKind.ShockAnswered) > 0, "no shock was ever answered");
    }
}
