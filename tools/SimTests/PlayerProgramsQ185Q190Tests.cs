using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q185–Q190 player programs (six systems; audit/consultant deferred to era).</summary>
public class PlayerProgramsQ185Q190Tests
{
    static Game Solo(Action<GameConfig> tweak = null) => Game.Solo(tweak: tweak).WithStarterLab(gpus: 16);

    // Q185 FundLobbyDefense
    [Fact] public void Q185_main_defense_sets_tick() {
        Game g = Game.New().WithStarterLab();
        g.Do(Command.FundLobbyDefense(g.Id));
        Assert.True(g.Me.LobbyDefenseUntilTick > g.Tick);
    }
    [Fact] public void Q185_zero_defense_disabled() {
        Game g = Solo(c => c.Rivalry.Feelings.LobbyDefenseCash = 0);
        g.Refused(Command.FundLobbyDefense(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q185_max_long_defense_weeks() {
        Game g = Solo(c => c.Rivalry.Feelings.LobbyDefenseWeeks = 30);
        g.Do(Command.FundLobbyDefense(g.Id));
        Assert.True(g.Me.LobbyDefenseUntilTick >= g.Tick + 30);
    }
    [Fact] public void Q185_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.FundLobbyDefense(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q185_save_load_defense_tick() {
        Game g = Solo();
        g.Do(Command.FundLobbyDefense(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.LobbyDefenseUntilTick, loaded.Player.LobbyDefenseUntilTick);
    }
    [Fact] public void Q185_cross_defense_trims_feud_lobby_bump() {
        Game g = Game.New().WithStarterLab();
        LabState rival = g.Rival("openaye");
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        rival.FeelingToPlayer = -800;
        int before = home.Attention;
        g.Do(Command.FundLobbyDefense(g.Id));
        RivalSystem.RunMonth(g.C, g.State);
        int full = g.C.Rivalry.Feelings.FeudAttentionPerMonth;
        int trimmed = (int)MathI.MulPermille(full, MathI.Permille - g.C.Rivalry.Feelings.LobbyDefenseCutPermille);
        Assert.Equal(before + trimmed, home.Attention);
    }

    // Q186 ExtendGrantRunway
    [Fact] public void Q186_main_extends_grant_ends() {
        Game g = Solo();
        int before = g.Me.GrantEndsTick;
        g.Do(Command.ExtendGrantRunway(g.Id));
        Assert.True(g.Me.GrantEndsTick > before);
    }
    [Fact] public void Q186_zero_extension_disabled() {
        Game g = Solo(c => c.Finance.GrantExtensionCash = 0);
        g.Refused(Command.ExtendGrantRunway(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q186_max_many_extension_weeks() {
        Game g = Solo(c => c.Finance.GrantExtensionWeeks = 40);
        int before = g.Me.GrantEndsTick;
        g.Do(Command.ExtendGrantRunway(g.Id));
        Assert.Equal(before + 40, g.Me.GrantEndsTick);
    }
    [Fact] public void Q186_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.ExtendGrantRunway(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q186_save_load_grant_ends() {
        Game g = Solo();
        g.Do(Command.ExtendGrantRunway(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.GrantEndsTick, loaded.Player.GrantEndsTick);
    }
    [Fact] public void Q186_cross_extension_keeps_weekly_grant() {
        Game g = Solo();
        g.Me.GrantEndsTick = g.Tick + 2;
        g.Do(Command.ExtendGrantRunway(g.Id));
        long cash = g.Me.Cash;
        g.Weeks(3);
        Assert.True(g.Me.Cash > cash - g.C.Finance.GrantExtensionCash);
        Assert.True(g.Me.GrantEndsTick > g.Tick);
    }

    // Q187 RequestDebtForbearance
    [Fact] public void Q187_main_sets_forbearance_tick() {
        Game g = Solo();
        g.Do(Command.RequestDebtForbearance(g.Id));
        Assert.True(g.Me.DebtForbearanceUntilTick > g.Tick);
    }
    [Fact] public void Q187_zero_forbearance_disabled() {
        Game g = Solo(c => c.Finance.DebtForbearanceCash = 0);
        g.Refused(Command.RequestDebtForbearance(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q187_max_long_forbearance_weeks() {
        Game g = Solo(c => c.Finance.DebtForbearanceWeeks = 24);
        g.Do(Command.RequestDebtForbearance(g.Id));
        Assert.True(g.Me.DebtForbearanceUntilTick >= g.Tick + 24);
    }
    [Fact] public void Q187_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.RequestDebtForbearance(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q187_save_load_forbearance_tick() {
        Game g = Solo();
        g.Do(Command.RequestDebtForbearance(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.DebtForbearanceUntilTick, loaded.Player.DebtForbearanceUntilTick);
    }
    [Fact] public void Q187_cross_forbearance_pauses_debt_clock() {
        Game g = Solo(c => c.Finance.BankruptcyGraceWeeks = 3);
        g.Me.Cash = 200_000;
        g.Do(Command.RequestDebtForbearance(g.Id));
        g.Me.Cash = -1;
        for (int i = 0; i < 5; i++) Finance.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.WeeksInDebt);
        Assert.False(g.Me.Bankrupt);
    }

    // Q188 ReserveGreyPayout
    static void GreyOp(Game g) {
        ModelState m = g.Train("mini", 200);
        g.UntilTrained(m);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 4000;
        g.Do(Command.BuyGpus(g.Id, 40, hall));
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", m.Id, -1, hall, ""));
    }

    static Game GreyGame(Action<GameConfig> tweak = null)
    {
        Game g = Solo(c =>
        {
            ShadyCardConfig card = c.Shady.Card("grey-compute");
            if (card != null) { card.OpensYear = 0; card.Weeks = 4; card.SuccessPermille = 1000; }
            if (tweak != null) tweak(c);
        });
        g.Me.Cash += 3_000_000;
        return g;
    }

    [Fact] public void Q188_main_reserve_sets_boost() {
        Game g = GreyGame();
        GreyOp(g);
        g.Do(Command.ReserveGreyPayout(g.Id));
        Assert.True(g.Me.GreyPayoutBoostUntilTick > g.Tick);
    }
    [Fact] public void Q188_zero_reserve_disabled() {
        Game g = GreyGame(c => c.Shady.GreyReserveCashCost = 0);
        GreyOp(g);
        g.Refused(Command.ReserveGreyPayout(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q188_max_high_boost_permille() {
        Game g = GreyGame(c => c.Shady.GreyReserveBoostPermille = 500);
        GreyOp(g);
        g.Do(Command.ReserveGreyPayout(g.Id));
        Assert.Equal(500, g.C.Shady.GreyReserveBoostPermille);
    }
    [Fact] public void Q188_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.ReserveGreyPayout(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q188_save_load_boost_tick() {
        Game g = GreyGame();
        GreyOp(g);
        g.Do(Command.ReserveGreyPayout(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.GreyPayoutBoostUntilTick, loaded.Player.GreyPayoutBoostUntilTick);
    }
    [Fact] public void Q188_cross_boost_increases_grey_revenue() {
        Game g = GreyGame(c => c.Shady.GreyReserveBoostPermille = 400);
        GreyOp(g);
        long cash0 = g.Me.Cash;
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        long payBare = g.Me.Cash - cash0;
        Assert.True(payBare > 0);
        g.Do(Command.ReserveGreyPayout(g.Id));
        long cash1 = g.Me.Cash;
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        long payBoosted = g.Me.Cash - cash1;
        Assert.True(payBoosted > payBare);
    }

    // Q189 PauseCloudRoyalties
    static void ListedChip(Game g) {
        var d = new ChipDesign { Id = 1, Status = ChipDesignStatus.Ready, Name = "c1", Version = 1, YieldPermille = 800, Target = "inference", NodeId = "n28", DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air", PointsRequired = 100, PointsDone = 100, ReadyTick = g.Tick };
        g.Me.ChipDesigns.Add(d);
        g.Me.ChipListings.Add(new ChipListing { DesignId = 1, PricePerChip = 100, MaxPerMonth = 10, ListedSinceTick = g.Tick });
    }

    static Game Play(Action<GameConfig> tweak = null) => Game.New(tweak: tweak).WithStarterLab(gpus: 16);

    [Fact] public void Q189_main_pause_sets_tick() {
        Game g = Solo();
        ListedChip(g);
        g.Do(Command.PauseCloudRoyalties(g.Id));
        Assert.True(g.Me.CloudRoyaltyPauseUntilTick > g.Tick);
    }
    [Fact] public void Q189_zero_pause_disabled() {
        Game g = Solo(c => c.Chips.CloudRoyaltyPauseCash = 0);
        ListedChip(g);
        g.Refused(Command.PauseCloudRoyalties(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q189_max_long_pause_weeks() {
        Game g = Solo(c => c.Chips.CloudRoyaltyPauseWeeks = 30);
        ListedChip(g);
        g.Do(Command.PauseCloudRoyalties(g.Id));
        Assert.True(g.Me.CloudRoyaltyPauseUntilTick >= g.Tick + 30);
    }
    [Fact] public void Q189_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.PauseCloudRoyalties(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q189_save_load_pause_tick() {
        Game g = Solo();
        ListedChip(g);
        g.Do(Command.PauseCloudRoyalties(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.CloudRoyaltyPauseUntilTick, loaded.Player.CloudRoyaltyPauseUntilTick);
    }
    [Fact] public void Q189_cross_pause_blocks_landlord_earn() {
        Game g = Play(c => c.Chips.CloudLandlordSharePermille = 500);
        ListedChip(g);
        LabState renter = g.Rival("openaye");
        renter.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 4, DesignLabId = g.Id });
        g.Do(Command.PauseCloudRoyalties(g.Id));
        long cash = g.Me.Cash;
        CloudChipLandlordSystem.RunWeek(g.C, g.State);
        Assert.Equal(cash, g.Me.Cash);
    }

    // Q190 FundProtestOutreach
    [Fact] public void Q190_main_outreach_sets_tick() {
        Game g = Solo();
        g.Do(Command.FundProtestOutreach(g.Id));
        Assert.True(g.Me.ProtestMitigationUntilTick > g.Tick);
    }
    [Fact] public void Q190_zero_outreach_disabled() {
        Game g = Solo(c => c.World.Attention.ProtestOutreachCash = 0);
        g.Refused(Command.FundProtestOutreach(g.Id), CommandError.InvalidArgument);
    }
    [Fact] public void Q190_max_long_outreach_weeks() {
        Game g = Solo(c => c.World.Attention.ProtestOutreachWeeks = 24);
        g.Do(Command.FundProtestOutreach(g.Id));
        Assert.True(g.Me.ProtestMitigationUntilTick >= g.Tick + 24);
    }
    [Fact] public void Q190_bankrupt_refuses() {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.FundProtestOutreach(g.Id), CommandError.LabBankrupt);
    }
    [Fact] public void Q190_save_load_outreach_tick() {
        Game g = Solo();
        g.Do(Command.FundProtestOutreach(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.ProtestMitigationUntilTick, loaded.Player.ProtestMitigationUntilTick);
    }
    [Fact] public void Q190_cross_outreach_trims_protest_bump() {
        Game g = Solo(c => c.World.Attention.ProtestOutreachCutPermille = 600);
        WorldEventConfig e = g.C.WorldEvent("pause-letter");
        Assert.True(e.ProtestAttentionPermille > 0);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        int before = home.Attention;
        g.Do(Command.FundProtestOutreach(g.Id));
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        int full = ProtestSystem.ProtestAttentionBump(g.C, before, e.ProtestAttentionPermille);
        int trimmed = (int)MathI.MulPermille(full, MathI.Permille - g.C.World.Attention.ProtestOutreachCutPermille);
        Assert.Equal(before + trimmed, home.Attention);
    }
}