using System.IO;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>
/// Cash is StartCash plus the lifetime net. The schema-5 bankrupt fixture once loaded with cash -100 against a start of 1000 and an empty lifetime: a hand-made file that never booked its own loss, not a state any version of the game saved (Q378). Migrations never touch cash or the ledger's totals, so they carry whatever gap a save had.
/// </summary>
public class LedgerIdentityMigrationTests
{
    static long Gap(LabState lab) { return lab.Cash - lab.Ledger.StartCash - lab.Ledger.Lifetime.Net; }

    [Fact]
    public void The_bankrupt_schema_five_fixture_explains_its_own_cash()
    {
        string json = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema5-bankrupt-save.json"));
        Simulation sim = Simulation.Load(TestData.Config(), json);
        LabState lab = sim.State.Labs.Single();
        Assert.True(lab.Bankrupt);
        Assert.Equal(-100, lab.Cash);
        Assert.Equal(1000, lab.Ledger.StartCash);
        Assert.Equal(0, Gap(lab));
    }

    [Fact]
    public void Loading_a_save_from_any_older_schema_leaves_cash_and_the_lifetime_net_exactly_as_they_were()
    {
        Game g = Game.Solo(11, c => c.Support.Enabled = 0).WithStarterLab();
        g.Autopilot(120);
        string now = g.Sim.Save();
        long gap = Gap(g.Me);
        long net = g.Me.Ledger.Lifetime.Net;
        Assert.NotEqual(0, net);
        for (int schema = 5; schema < GameState.CurrentSchema; schema++)
        {
            Simulation old = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, schema, now));
            LabState lab = old.State.Labs.First(l => l.Id == g.Id);
            Assert.True(gap == Gap(lab), "schema " + schema + " moved the books");
            Assert.Equal(net, lab.Ledger.Lifetime.Net);
            Assert.Equal(g.Me.Cash, lab.Cash);
        }
    }

    [Fact]
    public void A_bankrupt_lab_keeps_its_books_through_a_save_and_a_load()
    {
        Game g = Game.Solo(4).WithStarterLab();
        long delta = -50_000_000 - g.Me.Cash;
        g.Me.Cash += delta;
        g.Me.Ledger.StartCash += delta;
        long gap = Gap(g.Me);
        g.Weeks(40);
        Assert.True(g.Me.Bankrupt);
        Assert.Equal(gap, Gap(g.Me));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(gap, Gap(back.State.Labs.First(l => l.Id == g.Id)));
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }
}
