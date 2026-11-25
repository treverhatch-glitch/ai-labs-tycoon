using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ExitDebtTests
{
    [Fact]
    public void Breakdown_parts_sum_to_total()
    {
        Game g = Game.Solo(43, c => c.Endings.ExitCovenantCallPermille = 500);
        g.Build("server-closet", 4, 4);
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Me.Ledger.ThisMonth.Revenue = 100_000;
        g.Me.Cash = 50_000;
        g.Do(Command.TakeLoan(g.Id, 500));
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(b.RevenueYears + b.FleetResale + b.CashOnHand - b.StaffShare - b.Loans - b.Notes - b.Rbf - b.CovenantCalls, b.Total);
        Assert.Equal(EndingSystem.SellPrice(g.C, g.State, g.Me), b.Total);
        Assert.True(b.Loans > 0);
    }

    [Fact]
    public void Notes_and_rbf_subtract_from_exit()
    {
        Game g = Game.Solo(tweak: c => { c.ConvertibleNotes.Enabled = 1; c.Rbf.Enabled = 1; }).WithStarterLab();
        g.Me.Ledger.ThisMonth.Revenue = 1_000_000;
        if (g.C.ConvertibleNotes.Enabled > 0) Notes.Issue(g.C, g.State, g.Me, 1, 100_000);
        long before = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        g.Me.NoteOwed += 50_000;
        long after = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).Total;
        Assert.True(after < before);
    }
}