using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EvalHarnessTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(6, c =>
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
            c.EvalHarness.MinEra = 0;
            c.ReleaseChecklist.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static ModelState Sold(Game g, int capability = 700, int alignment = 700)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.Capability = capability;
        m.Alignment = alignment;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void A_model_with_no_public_eval_does_nothing_on_its_own()
    {
        Game g = Lab();
        Sold(g);
        g.Weeks(30);
        Assert.Equal(0, g.Me.Models.Sum(m => m.EvalSuite + m.EvalRuns + m.EvalStreak));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.EvalRegistered || e.Kind == EventKind.EvalPublished);
    }

    [Fact]
    public void Register_is_checked_and_costs_cash_and_points()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Refused(Command.RegisterEval(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.RegisterEval(g.Id, 999, 1), CommandError.UnknownId);
        g.Refused(Command.RegisterEval(g.Id, m.Id, 3), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        long train = g.Me.Ledger.Lifetime.Training;
        long points = g.Me.ResearchPoints;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        Assert.Equal(cash - g.C.EvalHarness.Cost, g.Me.Cash);
        Assert.Equal(train - g.C.EvalHarness.Cost, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(points - g.C.EvalHarness.Points, g.Me.ResearchPoints);
        Assert.Equal(1, m.EvalSuite);
        Assert.Equal(g.C.EvalHarness.Cadence, m.EvalLeft);
        GameEvent reg = Assert.Single(g.State.Events, e => e.Kind == EventKind.EvalRegistered);
        Assert.Equal(1, reg.Pick);
        g.Refused(Command.RegisterEval(g.Id, m.Id, 2), CommandError.InvalidArgument);
        Game poor = Lab();
        ModelState pm = Sold(poor);
        poor.Me.Cash = g.C.EvalHarness.Cost - 1;
        poor.Refused(Command.RegisterEval(poor.Id, pm.Id, 1), CommandError.NotEnoughCash);
        Game early = Lab(c => c.EvalHarness.MinEra = c.Eras.Count - 1);
        ModelState em = Sold(early);
        early.Refused(Command.RegisterEval(early.Id, em.Id, 1), CommandError.EraLocked);
        Game off = Lab(c => c.EvalHarness.Enabled = 0);
        ModelState om = Sold(off);
        off.Refused(Command.RegisterEval(off.Id, om.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Registration_works_on_a_trained_model_not_yet_on_sale()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.RegisterEval(g.Id, m.Id, 2));
        Assert.Equal(2, m.EvalSuite);
        Assert.Equal(0, m.EvalRuns);
    }

    [Fact]
    public void Cadence_publishes_scores_and_resets_the_timer()
    {
        Game g = Lab(c => c.EvalHarness.Cadence = 4);
        ModelState m = Sold(g, 800, 800);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(3);
        Assert.Equal(1, m.EvalLeft);
        Assert.Equal(0, m.EvalRuns);
        g.Weeks(1);
        Assert.Equal(1, m.EvalRuns);
        Assert.Equal(g.C.EvalHarness.Cadence, m.EvalLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EvalPublished);
    }

    [Fact]
    public void Beating_the_bar_gains_trust_and_builds_a_streak()
    {
        Game g = Lab(c => { c.EvalHarness.BarNarrow = 400; c.EvalHarness.TrustGain = 20; });
        ModelState m = Sold(g, 900, 900);
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(g.C.EvalHarness.Cadence);
        GameEvent pub = Assert.Single(g.State.Events, e => e.Kind == EventKind.EvalPublished);
        Assert.Equal(1, pub.Pick);
        Assert.Equal(1, m.EvalStreak);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) >= trust + g.C.EvalHarness.TrustGain);
    }

    [Fact]
    public void Missing_the_bar_loses_trust_resets_the_streak_and_pushes_conformity()
    {
        Game g = Lab(c => { c.EvalHarness.BarNarrow = 900; c.EvalHarness.BarBroad = 900; c.EvalHarness.TrustLoss = -15; c.EvalHarness.ConformityPush = 3; c.EvalHarness.Cadence = 1; });
        ModelState m = Sold(g);
        m.Capability = 50;
        m.Alignment = 50;
        m.EvalStreak = 2;
        m.CertStage = Conformity.Review;
        m.CertLeft = 8;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(1);
        Assert.Equal(0, m.EvalStreak);
        Assert.True(m.CertLeft > 8);
        Assert.Equal(0, Assert.Single(g.State.Events, e => e.Kind == EventKind.EvalPublished).Pick);
    }

    [Fact]
    public void A_miss_adds_tension_when_the_board_meets()
    {
        Game g = Lab(c => { c.EvalHarness.BarNarrow = 950; c.EvalHarness.BarBroad = 950; c.EvalHarness.FailTension = 80; });
        ModelState m = Sold(g, 100, 100);
        g.Me.BoardInvestorSeats = 1;
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(g.C.EvalHarness.Cadence);
        Assert.Equal(g.C.EvalHarness.FailTension, g.Me.BoardTension);
        Game quiet = Lab(c => { c.EvalHarness.BarNarrow = 950; c.EvalHarness.BarBroad = 950; c.EvalHarness.FailTension = 80; });
        ModelState qm = Sold(quiet, 100, 100);
        quiet.Do(Command.RegisterEval(quiet.Id, qm.Id, 1));
        quiet.Weeks(quiet.C.EvalHarness.Cadence);
        Assert.Equal(0, quiet.Me.BoardTension);
    }

    [Fact]
    public void A_model_that_leaves_sale_loses_its_eval_cadence()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        m.Status = ModelStatus.Retired;
        m.Sectors.Clear();
        EvalHarness.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, m.EvalSuite);
        Assert.Equal(0, m.EvalLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_public_eval()
    {
        Game g = Lab();
        ModelState m = Sold(g);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(g.C.EvalHarness.Cadence);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 125, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.EvalSuite + x.EvalLeft + x.EvalRuns + x.EvalStreak));
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}