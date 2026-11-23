using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Exit-debt breakdown (schema 141) survives saves with era-fifteen load (Q467 / Q464).</summary>
public class EraOBatchFifteenExitDebtHardeningTests
{
    static Game DebtLab(long seed = 150)
    {
        Game g = Game.Solo(seed, c =>
        {
            c.Endings.ExitCovenantCallPermille = 500;
            c.CampaignDeadline.MinEra = 0;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 6);
        g.Build("server-closet", 4, 4);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Me.Ledger.ThisMonth.Revenue = 200_000;
        g.Me.Cash = 80_000;
        return g;
    }

    static void AssertBreakdown(GameConfig c, GameState state, LabState lab)
    {
        var b = DisplayQueries.ExitPriceParts(c, state, lab);
        Assert.Equal(b.RevenueYears + b.FleetResale + b.CashOnHand - b.StaffShare - b.Loans - b.Notes - b.Rbf - b.CovenantCalls, b.Total);
        Assert.Equal(EndingSystem.SellPrice(c, state, lab), b.Total);
    }

    [Fact]
    public void Loan_balance_mid_save_load_matches_breakdown()
    {
        Game g = DebtLab();
        g.Do(Command.TakeLoan(g.Id, 600));
        long loans = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Loans;
        Assert.True(loans > 0);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(loans, DisplayQueries.ExitPriceParts(g.C, back.State, back.Player).Loans);
        AssertBreakdown(g.C, back.State, back.Player);
    }

    [Fact]
    public void Note_owed_mid_save_load_matches_breakdown()
    {
        Game g = DebtLab(151);
        g.C.ConvertibleNotes.Enabled = 1;
        if (g.C.ConvertibleNotes.Enabled > 0) Notes.Issue(g.C, g.State, g.Me, 1, 120_000);
        long notes = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Notes;
        Assert.True(notes > 0);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(notes, DisplayQueries.ExitPriceParts(g.C, back.State, back.Player).Notes);
        AssertBreakdown(g.C, back.State, back.Player);
    }

    [Fact]
    public void Rbf_owed_mid_save_load_matches_breakdown()
    {
        Game g = DebtLab(152);
        g.C.Rbf.Enabled = 1;
        g.Me.Ledger.LastWeek.Revenue = 500_000;
        RevenueFinancing.RunWeek(g.C, g.State, g.Me);
        g.Do(Command.TakeRevenueAdvance(g.Id, 2, 400));
        long rbf = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Rbf;
        Assert.True(rbf > 0);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(rbf, DisplayQueries.ExitPriceParts(g.C, back.State, back.Player).Rbf);
        AssertBreakdown(g.C, back.State, back.Player);
    }

    [Fact]
    public void Covenant_call_mid_save_load_matches_breakdown()
    {
        Game g = DebtLab(153);
        g.Do(Command.TakeLoan(g.Id, 700));
        g.Me.CovBreach = 1;
        long calls = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).CovenantCalls;
        Assert.True(calls > 0);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(calls, DisplayQueries.ExitPriceParts(g.C, back.State, back.Player).CovenantCalls);
        AssertBreakdown(g.C, back.State, back.Player);
    }

    [Fact]
    public void Combined_debt_mid_save_still_matches_sell_price()
    {
        Game g = DebtLab(154);
        g.C.ConvertibleNotes.Enabled = 1;
        g.Do(Command.TakeLoan(g.Id, 600));
        if (g.C.ConvertibleNotes.Enabled > 0) Notes.Issue(g.C, g.State, g.Me, 1, 30_000);
        long before = EndingSystem.SellPrice(g.C, g.State, g.Me);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(before, EndingSystem.SellPrice(g.C, back.State, back.Player));
        AssertBreakdown(g.C, back.State, back.Player);
    }

    [Fact]
    public void Exit_debt_totals_mid_save_match_ExitDebt_helper()
    {
        Game g = DebtLab(155);
        g.Do(Command.TakeLoan(g.Id, 550));
        g.Me.CovBreach = 1;
        long helper = ExitDebt.Total(g.C, g.Me);
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(helper, ExitDebt.Total(g.C, back.Player));
        var parts = DisplayQueries.ExitPriceParts(g.C, back.State, back.Player);
        Assert.Equal(parts.Loans + parts.Notes + parts.Rbf + parts.CovenantCalls, helper);
    }

    [Fact]
    public void Schema_141_pretend_keeps_exit_loan_breakdown()
    {
        Game g = DebtLab(156);
        g.Do(Command.TakeLoan(g.Id, 500));
        string current = g.Sim.Save();
        string old = SaveSchemaPretend.At(g.C, 141, current);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Empty(SaveGame.Problems(g.C, sim.State));
        AssertBreakdown(g.C, sim.State, sim.Player);
        Assert.True(DisplayQueries.ExitPriceParts(g.C, sim.State, sim.Player).Loans > 0);
    }

    [Fact]
    public void Mid_save_play_weeks_keeps_breakdown_aligned()
    {
        Game g = DebtLab(157);
        g.Do(Command.TakeLoan(g.Id, 450));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        AssertBreakdown(back.Config, back.State, back.Player);
    }
}