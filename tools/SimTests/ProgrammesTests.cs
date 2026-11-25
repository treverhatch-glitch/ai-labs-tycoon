using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ProgrammesTests
{
    const string Big = "beowulf-clusters";

    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        g.Me.ResearchPoints = 10_000;
        g.Me.Reputation = 500;
        return g;
    }

    static Game Sure() { return Lab(c => c.Programmes.BoldChancePermille = 999); }

    static Game Doomed() { return Lab(c => c.Programmes.BoldChancePermille = 1); }

    [Fact]
    public void Starting_costs_nothing_and_waits_for_an_approach()
    {
        Game g = Sure();
        long points = g.Me.ResearchPoints;
        g.Do(Command.StartProgramme(g.Id, Big));
        Assert.Equal(points, g.Me.ResearchPoints);
        Assert.Equal(Programmes.Choosing, g.Me.PgPhase);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ProgrammeStarted && e.Ref == Big);
        g.Weeks(5);
        Assert.Equal(Programmes.Choosing, g.Me.PgPhase);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_a_costly_reachable_tech_one_programme_at_a_time()
    {
        Game g = Sure();
        g.Refused(Command.StartProgramme(g.Id, "nope"), CommandError.UnknownId);
        g.Refused(Command.StartProgramme(g.Id, "state-machines"), CommandError.InvalidArgument);
        g.Refused(Command.StartProgramme(g.Id, "search-ranking"), CommandError.MissingPrerequisite);
        g.Do(Command.StartProgramme(g.Id, Big));
        g.Refused(Command.StartProgramme(g.Id, "rules-engines"), CommandError.InvalidArgument);
        g.Me.Techs.Add("rules-engines");
        Game h = Sure();
        h.Me.Techs.Add("rules-engines");
        h.Refused(Command.StartProgramme(h.Id, "rules-engines"), CommandError.AlreadyResearched);
    }

    [Fact]
    public void A_careful_stage_costs_more_and_takes_longer_than_a_bold_one()
    {
        Game g = Sure();
        g.Do(Command.StartProgramme(g.Id, Big));
        long safe = Programmes.StageCost(g.C, g.Me, false);
        long bold = Programmes.StageCost(g.C, g.Me, true);
        Assert.True(safe > bold);
        Assert.True(g.C.Programmes.SafeWeeks > g.C.Programmes.BoldWeeks);
        long points = g.Me.ResearchPoints;
        g.Do(Command.ChooseApproach(g.Id, 1));
        Assert.Equal(points - safe, g.Me.ResearchPoints);
        Assert.Equal(g.C.Programmes.SafeWeeks, g.Me.PgLeft);
        g.Refused(Command.ChooseApproach(g.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_stage_needs_the_points_and_a_valid_approach()
    {
        Game g = Sure();
        g.Do(Command.StartProgramme(g.Id, Big));
        g.Refused(Command.ChooseApproach(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.ChooseApproach(g.Id, 3), CommandError.InvalidArgument);
        g.Me.ResearchPoints = 10;
        g.Refused(Command.ChooseApproach(g.Id, 2), CommandError.NotEnoughResearch);
    }

    static void Stage(Game g, int approach)
    {
        g.Do(Command.ChooseApproach(g.Id, approach));
        g.Weeks(approach == 1 ? g.C.Programmes.SafeWeeks : g.C.Programmes.BoldWeeks);
    }

    [Fact]
    public void Three_careful_stages_give_the_tech_and_no_insight()
    {
        Game g = Sure();
        g.Do(Command.StartProgramme(g.Id, Big));
        long prestige = Prestige.Level(g.C, g.Me);
        for (int i = 0; i < 3; i++) Stage(g, 1);
        Assert.True(LabQueries.HasTech(g.Me, Big));
        Assert.Equal(0, g.Me.PgTech);
        Assert.Equal(1, g.Me.PgWins);
        Assert.Equal(prestige, Prestige.Level(g.C, g.Me));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ProgrammeEnded && e.Pick == 1 && e.Amount == 0);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TechResearched && e.Ref == Big);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Bold_stages_that_work_cost_less_and_pay_points_and_prestige_back()
    {
        Game safe = Sure();
        safe.Do(Command.StartProgramme(safe.Id, Big));
        for (int i = 0; i < 3; i++) Stage(safe, 1);
        long safeSpent = 10_000 - safe.Me.ResearchPoints;
        Game bold = Sure();
        bold.Do(Command.StartProgramme(bold.Id, Big));
        long prestige = Prestige.Level(bold.C, bold.Me);
        for (int i = 0; i < 3; i++) Stage(bold, 2);
        long boldNet = 10_000 - bold.Me.ResearchPoints;
        Assert.True(boldNet < safeSpent);
        Assert.Equal(prestige + 3 * bold.C.Programmes.InsightPrestige, Prestige.Level(bold.C, bold.Me));
        Assert.True(LabQueries.HasTech(bold.Me, Big));
    }

    [Fact]
    public void A_failed_bold_stage_waits_and_can_be_retried_or_fallen_back_from()
    {
        Game g = Doomed();
        g.Do(Command.StartProgramme(g.Id, Big));
        Stage(g, 2);
        Assert.Equal(Programmes.Failed, g.Me.PgPhase);
        Assert.Equal(0, g.Me.PgDone);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ProgrammeStageFailed);
        g.Weeks(10);
        Assert.Equal(Programmes.Failed, g.Me.PgPhase);
        g.Refused(Command.ChooseApproach(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.RecoverStage(g.Id, 0), CommandError.InvalidArgument);
        long points = g.Me.ResearchPoints;
        g.Do(Command.RecoverStage(g.Id, 1));
        Assert.Equal(points - Programmes.StageCost(g.C, g.Me, true), g.Me.ResearchPoints);
        g.Weeks(g.C.Programmes.BoldWeeks);
        Assert.Equal(Programmes.Failed, g.Me.PgPhase);
        points = g.Me.ResearchPoints;
        g.Do(Command.RecoverStage(g.Id, 2));
        Assert.Equal(points - Programmes.StageCost(g.C, g.Me, false), g.Me.ResearchPoints);
        g.Weeks(g.C.Programmes.SafeWeeks);
        Assert.Equal(1, g.Me.PgDone);
        Assert.Equal(0, g.Me.PgInsight);
    }

    [Fact]
    public void Giving_up_takes_part_of_the_points_back_and_loses_the_tech()
    {
        Game g = Doomed();
        g.Do(Command.StartProgramme(g.Id, Big));
        Stage(g, 1);
        Stage(g, 2);
        long spent = g.Me.PgSpent;
        long points = g.Me.ResearchPoints;
        g.Do(Command.RecoverStage(g.Id, 3));
        Assert.Equal(points + MathI.MulPermille(spent, g.C.Programmes.RefundPermille), g.Me.ResearchPoints);
        Assert.False(LabQueries.HasTech(g.Me, Big));
        Assert.Equal(1, g.Me.PgLosses);
        Assert.Equal(0, g.Me.PgTech);
        g.Refused(Command.AbandonProgramme(g.Id), CommandError.InvalidArgument);
        g.Do(Command.StartProgramme(g.Id, Big));
        g.Do(Command.AbandonProgramme(g.Id));
        Assert.Equal(2, g.Me.PgLosses);
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.StartProgramme(rival.Id, Big)).Ok);
        Game off = Lab(c => c.Programmes.Enabled = 0);
        off.Refused(Command.StartProgramme(off.Id, Big), CommandError.InvalidArgument);
        Game s = Sure();
        s.Do(Command.StartProgramme(s.Id, Big));
        Stage(s, 2);
        s.Do(Command.ChooseApproach(s.Id, 1));
        s.Weeks(2);
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 104, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == s.Id).PgTech);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
