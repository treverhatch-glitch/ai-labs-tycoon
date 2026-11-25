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

public class LicenceTests
{
    static Game Lab(Action<GameConfig> tweak = null, int year = 2027, int alignment = 800)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Safety.IncidentChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        m.Alignment = alignment;
        g.State.Tick = (year - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Reputation = 500;
        return g;
    }

    static GameEvent Incident(Game g)
    {
        ModelState best = g.Me.Models.First(m => m.Status == ModelStatus.Released);
        int kept = best.Alignment;
        best.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        int before = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        g.C.Safety.IncidentChancePermille = 0;
        best.Alignment = kept;
        return g.State.Events.Skip(before).First(e => e.Kind == EventKind.Incident);
    }

    static void Hold(Game g, int regime)
    {
        g.Do(Command.ApplyLicence(g.Id, regime));
        g.Weeks(g.C.Licence.ReviewWeeks);
    }

    [Fact]
    public void A_regime_opens_with_its_era_and_the_fee_is_paid_up_front()
    {
        Game early = Lab(null, 2000);
        early.Refused(Command.ApplyLicence(early.Id, 1), CommandError.EraLocked);
        Game g = Lab(null, 2013);
        g.Refused(Command.ApplyLicence(g.Id, 2), CommandError.EraLocked);
        g.Refused(Command.ApplyLicence(g.Id, 0), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.ApplyLicence(g.Id, 1));
        Assert.Equal(cash - g.C.Licence.FeeA, g.Me.Cash);
        Assert.Equal(Licence.Review, g.Me.LicPhase);
        Assert.Equal(g.C.Licence.ReviewWeeks, g.Me.LicLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Game late = Lab(null, 2027);
        late.Do(Command.ApplyLicence(late.Id, 3));
    }

    [Fact]
    public void One_application_at_a_time_and_the_lab_needs_the_cash()
    {
        Game g = Lab();
        g.Do(Command.ApplyLicence(g.Id, 1));
        g.Refused(Command.ApplyLicence(g.Id, 2), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = 10;
        poor.Refused(Command.ApplyLicence(poor.Id, 1), CommandError.NotEnoughCash);
    }

    [Fact]
    public void A_licence_is_granted_when_the_best_model_is_aligned_enough()
    {
        Game g = Lab(null, 2027, 800);
        Hold(g, 2);
        Assert.Equal(Licence.Licensed, g.Me.LicPhase);
        Assert.Equal(g.C.Licence.TermB, g.Me.LicLeft);
        Assert.Equal(1, g.Me.LicGranted);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceDecided && e.Pick == 1 && e.SubjectId == 2);
    }

    [Fact]
    public void A_stricter_regime_refuses_a_model_that_would_pass_the_easier_one_and_the_fee_is_lost()
    {
        Game g = Lab(null, 2027, 400);
        long cash = g.Me.Cash;
        g.Do(Command.ApplyLicence(g.Id, 3));
        Assert.Equal(cash - g.C.Licence.FeeC, g.Me.Cash);
        g.Weeks(g.C.Licence.ReviewWeeks);
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Equal(1, g.Me.LicRefused);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceDecided && e.Pick == 0);
        g.Refused(Command.ApplyLicence(g.Id, 1), CommandError.InvalidArgument);
        g.Weeks(g.C.Licence.CooldownWeeks);
        Hold(g, 1);
        Assert.Equal(Licence.Licensed, g.Me.LicPhase);
    }

    [Fact]
    public void A_held_licence_cuts_the_fine_of_an_incident_by_its_regime()
    {
        Game plain = Lab();
        plain.Weeks(plain.C.Licence.ReviewWeeks);
        long gross = Incident(plain).Amount;
        Game g = Lab();
        Hold(g, 3);
        Assert.Equal(g.C.Licence.CutC, Licence.FineCutPermille(g.C, g.Me));
        long net = Incident(g).Amount;
        Assert.True(net < gross);
    }

    [Fact]
    public void Each_incident_while_licensed_is_a_strike_and_the_limit_suspends_it()
    {
        Game g = Lab();
        Hold(g, 2);
        long rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        Incident(g);
        Assert.Equal(1, g.Me.LicStrikes);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceStrike);
        Incident(g);
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Equal(1, g.Me.LicSuspended);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceEnded && e.Pick == 2);
        Assert.True(g.Me.Reputation < rep);
        Assert.True(cash - g.Me.Cash >= 2 * g.C.Licence.FeeB);
        g.Refused(Command.ApplyLicence(g.Id, 2), CommandError.InvalidArgument);
        Assert.Equal(0, Licence.FineCutPermille(g.C, g.Me));
    }

    [Fact]
    public void Renewing_is_only_for_the_last_weeks_needs_the_bar_and_clears_the_strikes()
    {
        Game g = Lab();
        Hold(g, 1);
        g.Refused(Command.RenewLicence(g.Id), CommandError.InvalidArgument);
        Incident(g);
        Assert.Equal(1, g.Me.LicStrikes);
        g.Weeks(g.C.Licence.TermA - g.C.Licence.RenewWindow);
        g.Me.Models[0].Alignment = 10;
        g.Refused(Command.RenewLicence(g.Id), CommandError.MissingPrerequisite);
        g.Me.Models[0].Alignment = 800;
        long cash = g.Me.Cash;
        g.Do(Command.RenewLicence(g.Id));
        Assert.Equal(cash - g.C.Licence.FeeA, g.Me.Cash);
        Assert.Equal(0, g.Me.LicStrikes);
        Assert.Equal(g.C.Licence.TermA, g.Me.LicLeft);
    }

    [Fact]
    public void An_unrenewed_licence_lapses_and_a_surrender_ends_it_at_once()
    {
        Game g = Lab();
        Hold(g, 1);
        g.Weeks(g.C.Licence.TermA);
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceEnded && e.Pick == 0);
        Hold(g, 1);
        g.Do(Command.SurrenderLicence(g.Id));
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceEnded && e.Pick == 1);
        g.Refused(Command.SurrenderLicence(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.RenewLicence(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_cannot_apply_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.ApplyLicence(rival.Id, 1)).Ok);
        Game off = Lab(c => c.Licence.Enabled = 0);
        off.Refused(Command.ApplyLicence(off.Id, 1), CommandError.InvalidArgument);
        Game s = Lab();
        Hold(s, 2);
        Incident(s);
        Assert.Empty(SaveGame.Problems(s.C, s.State));
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 107, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == s.Id).LicRegime);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
