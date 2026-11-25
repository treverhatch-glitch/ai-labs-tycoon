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

public class DrillsTests
{
    static Game Lab(Action<GameConfig> tweak = null, int safety = 2, int seed = 5)
    {
        Game g = Game.Solo(seed, c =>
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
            c.Drills.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: Math.Max(2, safety), sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void A_lab_that_never_drills_stays_at_zero_preparedness()
    {
        Game g = Lab();
        g.Weeks(30);
        Assert.Equal(0, g.Me.DrillPrep + g.Me.DrillLeft + g.Me.DrillPassed + g.Me.DrillFailed);
        Assert.Equal(0, Drills.IncidentCutPermille(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.DrillStarted);
    }

    [Fact]
    public void Starting_a_drill_is_checked()
    {
        Game g = Lab(c => c.Drills.Staff = 99);
        g.Refused(Command.StartDrill(g.Id), CommandError.NotEnoughStaff);
        Game ok = Lab();
        ok.Do(Command.StartDrill(ok.Id));
        g = ok;
        g.Refused(Command.StartDrill(g.Id), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = g.C.Drills.Cost - 1;
        poor.Refused(Command.StartDrill(poor.Id), CommandError.NotEnoughCash);
        Game early = Lab(c => c.Drills.MinEra = c.Eras.Count - 1);
        early.Refused(Command.StartDrill(early.Id), CommandError.EraLocked);
        Game off = Lab(c => c.Drills.Enabled = 0);
        off.Refused(Command.StartDrill(off.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_drill_costs_cash_runs_for_weeks_and_blocks_a_second_one()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.StartDrill(g.Id));
        Assert.Equal(cash - g.C.Drills.Cost, g.Me.Cash);
        Assert.Equal(before - g.C.Drills.Cost, g.Me.Ledger.Lifetime.Training);
        Assert.Equal(g.C.Drills.Weeks, g.Me.DrillLeft);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.DrillStarted);
        g.Weeks(g.C.Drills.Weeks - 1);
        Assert.True(g.Me.DrillLeft > 0);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.DrillFinished);
        g.Weeks(1);
        Assert.Equal(0, g.Me.DrillLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DrillFinished);
    }

    [Fact]
    public void A_passing_drill_raises_preparedness_and_sets_the_next_due_week()
    {
        Game g = Lab(c => c.Drills.PassGain = 200);
        g.Me.DrillPrep = 600;
        g.Do(Command.StartDrill(g.Id));
        g.Weeks(g.C.Drills.Weeks);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.DrillFinished);
        Assert.Equal(1, e.Pick);
        Assert.Equal(800, g.Me.DrillPrep);
        Assert.Equal(1, g.Me.DrillPassed);
        Assert.Equal(g.Tick + g.C.Drills.Interval, g.Me.DrillDue);
    }

    [Fact]
    public void A_failing_drill_lowers_preparedness()
    {
        for (int seed = 0; seed < 80; seed++)
        {
            Game g = Lab(c => { c.Drills.FailLoss = 100; c.Drills.PassGain = 500; }, seed: seed);
            g.Me.DrillPrep = 0;
            g.Do(Command.StartDrill(g.Id));
            g.Weeks(g.C.Drills.Weeks);
            GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.DrillFinished);
            if (e.Pick != 0) continue;
            Assert.Equal(0, g.Me.DrillPrep);
            Assert.Equal(1, g.Me.DrillFailed);
            return;
        }
        Assert.Fail("No seed produced a failing drill in 80 tries.");
    }

    [Fact]
    public void Preparedness_fades_and_fades_faster_past_the_interval()
    {
        Game g = Lab(c => { c.Drills.Decay = 10; c.Drills.IdleDecay = 30; c.Drills.Interval = 4; });
        g.Me.DrillPrep = 500;
        g.Me.DrillDue = g.Tick + 10;
        g.Weeks(1);
        Assert.Equal(490, g.Me.DrillPrep);
        g.Me.DrillDue = g.Tick;
        g.Weeks(1);
        Assert.Equal(450, g.Me.DrillPrep);
    }

    [Fact]
    public void High_preparedness_cuts_incident_chance_and_reputation_loss()
    {
        Game g = Lab();
        g.Me.DrillPrep = 500;
        Assert.Equal(g.C.Drills.IncidentCut * 500 / 1000, Drills.IncidentCutPermille(g.C, g.Me));
        Assert.Equal(g.C.Drills.RepCut * 500 / 1000, Drills.ReputationCutPermille(g.C, g.Me));
        g.Me.DrillPrep = 0;
        Assert.Equal(0, Drills.IncidentCutPermille(g.C, g.Me));
    }

    [Fact]
    public void Preparedness_softens_what_an_incident_costs_in_reputation()
    {
        int Loss(int prep)
        {
            Game g = Lab(c =>
            {
                c.Drills.IncidentCut = 0;
                c.Drills.RepCut = 400;
                c.Drills.OnCallRepStack = 0;
                c.OnCall.MaxLevel = 0;
            });
            ModelState m = Selling(g);
            g.Me.DrillPrep = prep;
            m.Alignment = 0;
            g.Me.Reputation = 500;
            g.C.Safety.IncidentChancePermille = 1000;
            SafetySystem.RunWeek(g.C, g.State, g.Me);
            return 500 - g.Me.Reputation;
        }
        int bare = Loss(0);
        Assert.True(bare > 0);
        Assert.Equal(MathI.MulPermille(bare, 600), Loss(1000));
    }
}