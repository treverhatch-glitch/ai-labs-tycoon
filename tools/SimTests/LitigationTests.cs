using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LitigationTests
{
    static Game Lab()
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Security.Enabled = 0;
            c.Support.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        return g;
    }

    static LawCase File(Game g, int strength, long claim = 100_000, int kind = Litigation.Incident)
    {
        return Litigation.File(g.C, g.State, g.Me, kind, claim, strength, "Mini");
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_filed_claim_has_a_hearing_a_floor_and_a_cap()
    {
        Game g = Lab();
        LawCase small = File(g, 400, 5);
        Assert.Equal(g.C.Litigation.MinClaim, small.Claim);
        Assert.Equal(g.State.Tick + g.C.Litigation.HearingWeeks, small.Due);
        Assert.Equal(1, small.Id);
        GameEvent filed = g.State.Events.Single(e => e.Kind == EventKind.CaseFiled);
        Assert.Equal(small.Claim, filed.Amount);
        Assert.Equal(Litigation.Incident, filed.Pick);
        for (int i = 1; i < g.C.Litigation.MaxOpen; i++) Assert.NotNull(File(g, 400));
        Assert.Null(File(g, 400));
        Assert.Equal(g.C.Litigation.MaxOpen, g.Me.Cases.Count);
    }

    [Fact]
    public void Settling_costs_a_share_and_ends_it_but_not_after_a_lost_hearing()
    {
        Game g = Lab();
        LawCase a = File(g, 900);
        long cash = g.Me.Cash;
        g.Do(Command.SettleCase(g.Id, a.Id));
        Assert.Equal(cash - Litigation.SettleCost(g.C, a), g.Me.Cash);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(Litigation.Settled, g.State.Events.Last(e => e.Kind == EventKind.CaseClosed).Pick);
        g.Refused(Command.SettleCase(g.Id, a.Id), CommandError.UnknownId);
        LawCase b = File(g, 900);
        g.Weeks(g.C.Litigation.HearingWeeks);
        Assert.Equal(1, b.Stage);
        g.Refused(Command.SettleCase(g.Id, b.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_claim_the_in_house_counsel_beats_is_won_and_costs_nothing()
    {
        Game g = Lab();
        int weakest = g.C.Litigation.JudgeAt + g.C.Litigation.InHouseStrength - 1;
        LawCase a = File(g, weakest);
        int rep = g.Me.Reputation;
        g.Weeks(g.C.Litigation.HearingWeeks);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(Litigation.Won, g.State.Events.Last(e => e.Kind == EventKind.CaseClosed).Pick);
        Assert.True(g.Me.Reputation >= rep);
        Assert.Equal(0, g.Me.Ledger.Lifetime.Fines);
    }

    [Fact]
    public void A_strong_claim_loses_the_hearing_and_is_paid_when_the_appeal_weeks_run_out()
    {
        Game g = Lab();
        LawCase a = File(g, 900);
        g.Weeks(g.C.Litigation.HearingWeeks);
        Assert.Equal(1, a.Stage);
        Assert.Equal(1, Count(g, EventKind.CaseJudged));
        Assert.Equal(Litigation.Judgment(g.C, a), g.State.Events.Last(e => e.Kind == EventKind.CaseJudged).Amount);
        int rep = g.Me.Reputation;
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Weeks(g.C.Litigation.AppealWeeks);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(fines - Litigation.Judgment(g.C, a), g.Me.Ledger.Lifetime.Fines);
        Assert.True(g.Me.Reputation < rep);
        GameEvent closed = g.State.Events.Last(e => e.Kind == EventKind.CaseClosed);
        Assert.Equal(Litigation.Lost, closed.Pick);
        Assert.Equal(Litigation.Judgment(g.C, a), closed.Amount);
    }

    [Fact]
    public void An_outside_firm_turns_a_claim_the_lab_would_lose_and_can_be_retained_once()
    {
        Game g = Lab();
        int strength = g.C.Litigation.JudgeAt + g.C.Litigation.InHouseStrength + 50;
        LawCase a = File(g, strength);
        LawCase b = File(g, strength);
        long cash = g.Me.Cash;
        g.Do(Command.RetainFirm(g.Id, a.Id));
        Assert.Equal(cash - Litigation.FirmFee(g.C, a), g.Me.Cash);
        g.Refused(Command.RetainFirm(g.Id, a.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Litigation.HearingWeeks);
        Assert.DoesNotContain(g.Me.Cases, x => x.Id == a.Id);
        Assert.Equal(1, b.Stage);
    }

    [Fact]
    public void One_appeal_can_overturn_a_judgment_and_costs_a_fee_either_way()
    {
        Game g = Lab();
        int strength = g.C.Litigation.JudgeAt + g.C.Litigation.InHouseStrength + 10;
        LawCase a = File(g, strength);
        g.Refused(Command.AppealCase(g.Id, a.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Litigation.HearingWeeks);
        Assert.Equal(1, a.Stage);
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Do(Command.AppealCase(g.Id, a.Id));
        Assert.Equal(fines - Litigation.AppealFee(g.C, a), g.Me.Ledger.Lifetime.Fines);
        g.Refused(Command.AppealCase(g.Id, a.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Litigation.AppealWeeks);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(Litigation.Overturned, g.State.Events.Last(e => e.Kind == EventKind.CaseClosed).Pick);
        Assert.Equal(fines - Litigation.AppealFee(g.C, a), g.Me.Ledger.Lifetime.Fines);
    }

    [Fact]
    public void An_appeal_that_cannot_turn_the_claim_pays_the_judgment_on_top_of_the_fee()
    {
        Game g = Lab();
        LawCase a = File(g, 1000);
        g.Weeks(g.C.Litigation.HearingWeeks);
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Do(Command.AppealCase(g.Id, a.Id));
        g.Weeks(g.C.Litigation.AppealWeeks);
        Assert.Equal(fines - Litigation.AppealFee(g.C, a) - Litigation.Judgment(g.C, a), g.Me.Ledger.Lifetime.Fines);
        Assert.Equal(Litigation.Lost, g.State.Events.Last(e => e.Kind == EventKind.CaseClosed).Pick);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.CaseJudged && e.Pick == 1));
    }

    [Fact]
    public void A_layoff_round_draws_a_dismissal_claim_stronger_when_morale_is_low()
    {
        Game g = Lab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        g.Do(Command.Layoff(g.Id, 300));
        LawCase happy = g.Me.Cases.Single();
        Assert.Equal(Litigation.Dismissal, happy.Kind);
        Game h = Lab();
        h.Do(Command.Layoff(h.Id, 300));
        foreach (StaffMember p in h.Me.Staff) p.Morale = 100;
        h.Me.Cases.Clear();
        Litigation.OnLayoff(h.C, h.State, h.Me, 300, 10_000, 3);
        Assert.True(h.Me.Cases.Single().Strength > happy.Strength);
    }

    [Fact]
    public void An_incident_draws_a_negligence_claim_stronger_where_the_culture_is_weak()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 300);
        g.Me.CultureShift = 400;
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        int strong = g.Me.Cases.Single().Strength;
        g.Me.Cases.Clear();
        g.Me.CultureShift = -300;
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        Assert.True(g.Me.Cases.Single().Strength > strong);
        Assert.Equal(Litigation.Incident, g.Me.Cases.Single().Kind);
    }

    [Fact]
    public void Nothing_is_filed_when_it_is_switched_off_or_the_lab_is_a_rival()
    {
        Game g = Lab();
        g.C.Litigation.Enabled = 0;
        Assert.Null(File(g, 900));
        g.C.Litigation.Enabled = 1;
        LabState rival = g.State.Labs.FirstOrDefault(l => l.Id != g.Id);
        if (rival != null) Assert.Null(Litigation.File(g.C, g.State, rival, 0, 100_000, 900, ""));
        g.C.Litigation.Enabled = 0;
        g.Refused(Command.SettleCase(g.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Claims_in_every_stage_save_load_play_on_the_same_and_old_saves_have_none()
    {
        Game g = Lab();
        LawCase a = File(g, 900);
        LawCase b = File(g, 900);
        LawCase c = File(g, 300);
        g.Do(Command.RetainFirm(g.Id, c.Id));
        g.Weeks(g.C.Litigation.HearingWeeks - 1);
        g.Do(Command.SettleCase(g.Id, b.Id));
        g.Weeks(1);
        Assert.Equal(1, a.Stage);
        g.Do(Command.AppealCase(g.Id, a.Id));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 81, g.Sim.Save());
        Assert.DoesNotContain("caseSeq", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Empty(lab.Cases);
        Assert.Equal(0, lab.CaseSeq);
    }
}
