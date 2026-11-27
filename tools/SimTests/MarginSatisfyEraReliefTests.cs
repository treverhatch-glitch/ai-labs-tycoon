using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class MarginSatisfyEraReliefTests
{
    static Game Lab()
    {
        Game g = Game.Solo(95, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 2;
            c.Loans.LoanToValuePermille = 800;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 2);
        g.Me.Cash = 500_000_000;
        return g;
    }

    [Fact]
    public void Era_relief_lowers_cash_due_and_fires_event_on_pay()
    {
        Game g = Lab();
        g.C.MarginSatisfyEraRelief.Enabled = 1;
        g.C.MarginSatisfyEraRelief.MinEra = 0;
        g.C.MarginSatisfyEraRelief.ReliefPermille = 200;
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 950));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        long raw = MarginCall.RawCashDue(g.C, g.State, g.Me);
        long due = MarginCall.CashDue(g.C, g.State, g.Me);
        Assert.True(due < raw);
        g.Do(Command.SatisfyMarginCall(g.Id, 0, (int)due));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MarginSatisfyEraRelieved);
    }

    [Fact]
    public void Save_at_169_migrates_to_current_schema()
    {
        Game g = Lab();
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 169, g.Sim.Save()));
        Assert.Equal(GameState.CurrentSchema, older.State.Schema);
        SaveSchemaPretend.AssertRoundTrip(older);
    }
}