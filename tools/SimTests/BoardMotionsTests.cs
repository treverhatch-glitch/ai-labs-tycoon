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

public class BoardMotionsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(8, c =>
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
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 1, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000;
        g.Me.BoardInvestorSeats = 1;
        return g;
    }

    static bool Ousted(Game g) { return g.State.Endings.Any(e => e.EndingId == EndingSystem.OustedId); }

    static void ToMotion(Game g)
    {
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(g.C.BoardAgenda.MeetingWeeks);
    }

    [Fact]
    public void A_lab_that_never_holds_meetings_has_no_motions_and_no_tension()
    {
        Game g = Lab();
        g.Weeks(40);
        Assert.Equal(0, g.Me.MeetingsOn + g.Me.BoardTension + g.Me.MotionPend + g.Me.MotionKind + g.Me.MotionsTabled);
        Assert.Equal(0, BoardMotions.OustMargin(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.MotionTabled);
    }

    [Fact]
    public void Holding_meetings_is_checked()
    {
        Game g = Lab();
        g.Me.BoardInvestorSeats = 0;
        g.Refused(Command.HoldBoardMeetings(g.Id, 1), CommandError.MissingPrerequisite);
        g.Me.BoardInvestorSeats = 1;
        g.Refused(Command.HoldBoardMeetings(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.HoldBoardMeetings(g.Id, 2), CommandError.InvalidArgument);
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Refused(Command.HoldBoardMeetings(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.AnswerMotion(g.Id, 1), CommandError.InvalidArgument);
        g.Weeks(g.C.BoardAgenda.MeetingWeeks);
        g.Refused(Command.HoldBoardMeetings(g.Id, 0), CommandError.InvalidArgument);
        Game off = Lab(c => c.BoardAgenda.Enabled = 0);
        off.Refused(Command.HoldBoardMeetings(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void The_board_tables_a_motion_each_quarter_and_the_kinds_take_turns()
    {
        Game g = Lab(c => { c.BoardAgenda.AnswerWeeks = 1; c.BoardAgenda.IgnoreTension = 0; });
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(g.C.BoardAgenda.MeetingWeeks - 1);
        Assert.Equal(0, g.Me.MotionPend);
        g.Weeks(1);
        Assert.Equal(BoardMotions.Calm, g.Me.MotionPend);
        Assert.Equal(0, g.Me.MotionPendGoal);
        g.Weeks(g.C.BoardAgenda.MeetingWeeks);
        Assert.Equal(BoardMotions.Runway, g.Me.MotionPend);
        Assert.True(g.Me.MotionPendGoal > g.C.BoardAgenda.RunwayFloor);
        g.Weeks(g.C.BoardAgenda.MeetingWeeks);
        Assert.Equal(BoardMotions.SafetyStaff, g.Me.MotionPend);
        Assert.Equal(LabQueries.StaffCount(g.Me, "safety") + g.C.BoardAgenda.SafetyAdd, g.Me.MotionPendGoal);
        Assert.Equal(3, g.Me.MotionsTabled);
    }

    [Fact]
    public void Accepting_makes_a_commitment_and_eases_the_tension_declining_and_ignoring_add_to_it()
    {
        Game g = Lab();
        g.Me.BoardTension = 300;
        ToMotion(g);
        g.Do(Command.AnswerMotion(g.Id, 1));
        Assert.Equal(300 - g.C.BoardAgenda.AcceptRelief, g.Me.BoardTension);
        Assert.Equal(BoardMotions.Calm, g.Me.MotionKind);
        Assert.Equal(0, g.Me.MotionPend);
        Game d = Lab();
        ToMotion(d);
        d.Do(Command.AnswerMotion(d.Id, 0));
        Assert.Equal(d.C.BoardAgenda.DeclineTension, d.Me.BoardTension);
        Assert.Equal(1, d.Me.MotionsDeclined);
        Game n = Lab();
        ToMotion(n);
        n.Weeks(n.C.BoardAgenda.AnswerWeeks);
        Assert.Equal(n.C.BoardAgenda.IgnoreTension, n.Me.BoardTension);
        Assert.Equal(0, n.Me.MotionPend);
        Assert.Contains(n.State.Events, e => e.Kind == EventKind.MotionAnswered && e.Pick == 2);
    }

    [Fact]
    public void A_kept_commitment_eases_the_tension_and_a_broken_one_adds_to_it()
    {
        Game g = Lab();
        g.Me.BoardTension = 400;
        ToMotion(g);
        g.Do(Command.AnswerMotion(g.Id, 1));
        g.Weeks(g.C.BoardAgenda.CommitWeeks);
        Assert.Equal(1, g.Me.MotionsKept);
        Assert.Equal(400 - g.C.BoardAgenda.AcceptRelief - g.C.BoardAgenda.KeptRelief, g.Me.BoardTension);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MotionJudged && e.Pick == 1);

        Game b = Lab();
        ToMotion(b);
        b.Do(Command.AnswerMotion(b.Id, 1));
        b.Me.Staff.Clear();
        ModelState m = new ModelState { Id = b.State.TakeId(), Name = "x", Status = ModelStatus.Retired, Incidents = 0 };
        b.Me.Models.Add(m);
        m.Incidents = 3;
        b.Weeks(b.C.BoardAgenda.CommitWeeks);
        Assert.Equal(1, b.Me.MotionsBroken);
        Assert.Equal(b.C.BoardAgenda.BrokenTension, b.Me.BoardTension);
    }

    [Fact]
    public void Runway_and_safety_commitments_are_judged_on_cash_and_on_staff()
    {
        Game g = Lab(c => { c.BoardAgenda.MeetingWeeks = 6; c.BoardAgenda.AnswerWeeks = 6; c.BoardAgenda.CommitWeeks = 4; });
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(6);
        g.Do(Command.AnswerMotion(g.Id, 0));
        g.Weeks(6);
        Assert.Equal(BoardMotions.Runway, g.Me.MotionPend);
        g.Do(Command.AnswerMotion(g.Id, 1));
        g.Me.Cash = g.Me.MotionGoal + 1_000_000_000;
        g.Weeks(4);
        Assert.Equal(1, g.Me.MotionsKept);
        g.Weeks(2);
        Assert.Equal(BoardMotions.SafetyStaff, g.Me.MotionPend);
        g.Do(Command.AnswerMotion(g.Id, 1));
        g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "safety", TraitId = "steady", Morale = 500 });
        g.Weeks(4);
        Assert.Equal(2, g.Me.MotionsKept);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Independent_directors_take_some_of_the_tension_off()
    {
        Game g = Lab();
        g.Me.GovDirectors = 2;
        ToMotion(g);
        g.Do(Command.AnswerMotion(g.Id, 0));
        Assert.Equal(g.C.BoardAgenda.DeclineTension * (1000 - 2 * g.C.BoardAgenda.DirectorCut) / 1000, g.Me.BoardTension);
        Game many = Lab();
        many.Me.GovDirectors = 20;
        ToMotion(many);
        many.Do(Command.AnswerMotion(many.Id, 0));
        Assert.Equal(many.C.BoardAgenda.DeclineTension * 200 / 1000, many.Me.BoardTension);
    }

    [Fact]
    public void A_tense_board_forces_a_vote_the_founders_lose_unless_they_hold_enough_control()
    {
        Game g = Lab();
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Me.FounderSharePermille = g.C.Funding.ControlPermille + 100;
        g.Me.BoardTension = g.C.BoardAgenda.OustAt - 1;
        g.Weeks(1);
        Assert.False(Ousted(g));
        g.Me.BoardTension = 1000;
        Assert.True(BoardMotions.OustMargin(g.C, g.Me) > 100);
        g.Weeks(1);
        Assert.True(Ousted(g));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.BoardForcedVote);
        Assert.Equal(0, g.Me.MeetingsOn);

        Game s = Lab();
        s.Do(Command.HoldBoardMeetings(s.Id, 1));
        s.Me.FounderSharePermille = s.C.Funding.ControlPermille + 100;
        s.Me.BoardTension = 1000;
        s.Me.GovForm = 1;
        s.Weeks(1);
        Assert.False(Ousted(s));

        Game np = Lab();
        np.Do(Command.HoldBoardMeetings(np.Id, 1));
        np.Me.FounderSharePermille = 1;
        np.Me.BoardTension = 1000;
        np.Me.GovForm = 2;
        np.Weeks(1);
        Assert.False(Ousted(np));
    }

    [Fact]
    public void A_calm_board_that_was_kept_its_word_adds_to_a_funding_round()
    {
        Game g = Lab();
        long plain = BoardMotions.RaiseCash(g.C, g.Me, 1_000_000);
        Assert.Equal(1_000_000, plain);
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Me.MotionsKept = 1;
        g.Me.BoardTension = g.C.BoardAgenda.CalmBelow;
        Assert.Equal(1_000_000 * (1000 + g.C.BoardAgenda.RaiseBonus) / 1000, BoardMotions.RaiseCash(g.C, g.Me, 1_000_000));
        g.Me.BoardTension = g.C.BoardAgenda.CalmBelow + 1;
        Assert.Equal(1_000_000, BoardMotions.RaiseCash(g.C, g.Me, 1_000_000));
    }

    [Fact]
    public void Saves_round_trip_and_a_motion_without_meetings_is_a_problem()
    {
        Game g = Lab();
        ToMotion(g);
        g.Do(Command.AnswerMotion(g.Id, 1));
        g.Weeks(3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Me.MeetingsOn = 0;
        Assert.NotEmpty(SaveGame.Problems(g.C, g.State));
        g.Me.MeetingsOn = 1;
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 123, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.MeetingsOn + lab.BoardTension + lab.MotionKind + lab.MotionsTabled);
    }
}
