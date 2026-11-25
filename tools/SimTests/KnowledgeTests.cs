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

public class KnowledgeTests
{
    static Game Lab(Action<GameConfig> tweak = null)
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
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 3, safety: 1, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Model(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    static StaffMember Person(Game g, string role, int skip = 0)
    {
        return g.Me.Staff.Where(p => p.RoleId == role).OrderBy(p => p.Id).Skip(skip).First();
    }

    [Fact]
    public void A_lab_that_names_nobody_has_no_owners_no_gaps_and_no_cost()
    {
        Game g = Lab();
        ModelState m = Model(g);
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Assert.Equal(0, m.OwnerId + m.DocLevel + m.KnowGap + m.Handovers);
        Assert.Equal(0, Knowledge.IncidentPermille(g.C, m));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OwnerLeft);
    }

    [Fact]
    public void An_owner_is_checked()
    {
        Game g = Lab();
        ModelState m = Model(g);
        StaffMember r = Person(g, "researcher");
        g.Refused(Command.AssignOwner(g.Id, 999, r.Id), CommandError.UnknownId);
        g.Refused(Command.AssignOwner(g.Id, m.Id, 987654), CommandError.UnknownId);
        g.Refused(Command.AssignOwner(g.Id, m.Id, Person(g, "sales").Id), CommandError.InvalidArgument);
        g.Refused(Command.AssignOwner(g.Id, m.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        Assert.Equal(r.Id, m.OwnerId);
        g.Refused(Command.AssignOwner(g.Id, m.Id, r.Id), CommandError.InvalidArgument);
        g.Do(Command.AssignOwner(g.Id, m.Id, 0));
        Assert.Equal(0, m.OwnerId);
        Game off = Lab(c => c.Knowledge.Enabled = 0);
        ModelState om = Model(off);
        off.Refused(Command.AssignOwner(off.Id, om.Id, Person(off, "researcher").Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_person_owns_only_so_many_models()
    {
        Game g = Lab(c => c.Knowledge.MaxOwned = 1);
        ModelState a = Model(g);
        ModelState b = Model(g);
        StaffMember r = Person(g, "researcher");
        g.Do(Command.AssignOwner(g.Id, a.Id, r.Id));
        g.Refused(Command.AssignOwner(g.Id, b.Id, r.Id), CommandError.InvalidArgument);
        g.Do(Command.AssignOwner(g.Id, b.Id, Person(g, "researcher", 1).Id));
    }

    [Fact]
    public void Documentation_needs_an_owner_costs_money_and_has_a_top_level()
    {
        Game g = Lab();
        ModelState m = Model(g);
        g.Refused(Command.WriteDocs(g.Id, m.Id), CommandError.MissingPrerequisite);
        g.Do(Command.AssignOwner(g.Id, m.Id, Person(g, "researcher").Id));
        long before = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.WriteDocs(g.Id, m.Id));
        Assert.Equal(before - g.C.Knowledge.DocCost, g.Me.Ledger.Lifetime.Training);
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Refused(Command.WriteDocs(g.Id, m.Id), CommandError.InvalidArgument);
        Assert.Equal(3, m.DocLevel);
        Game poor = Lab();
        ModelState pm = Model(poor);
        poor.Do(Command.AssignOwner(poor.Id, pm.Id, Person(poor, "engineer").Id));
        poor.Me.Cash = 5;
        poor.Refused(Command.WriteDocs(poor.Id, pm.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void When_the_owner_is_let_go_the_model_keeps_a_gap_and_fails_more_often()
    {
        Game g = Lab();
        ModelState m = Model(g);
        Game twin = Lab();
        ModelState tm = Model(twin);
        StaffMember r = Person(g, "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        int chance = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Me.Staff.RemoveAll(p => p.Id != r.Id && p.RoleId == "researcher");
        g.Do(Command.Fire(g.Id, "researcher", 1));
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == r.Id);
        Assert.Equal(0, m.OwnerId);
        Assert.Equal(g.C.Knowledge.GapBase, m.KnowGap);
        Assert.Equal(1, m.Handovers);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OwnerLeft);
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) > chance);
        Assert.Equal(SafetySystem.WeeklyChancePermille(twin.C, twin.State, twin.Me, tm), chance);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Documentation_makes_the_gap_smaller()
    {
        Game g = Lab();
        ModelState m = Model(g);
        StaffMember r = Person(g, "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Me.Staff.Remove(r);
        Alumni.Record(g.C, g.State, g.Me, r, Alumni.Quit);
        Assert.Equal(g.C.Knowledge.GapBase * (1000 - 2 * g.C.Knowledge.DocCut) / 1000, m.KnowGap);
    }

    [Fact]
    public void A_new_owner_closes_the_gap_week_by_week_and_nobody_does_without_one()
    {
        Game g = Lab();
        ModelState m = Model(g);
        m.KnowGap = 400;
        g.Weeks(3);
        Assert.Equal(400, m.KnowGap);
        g.Do(Command.AssignOwner(g.Id, m.Id, Person(g, "engineer").Id));
        g.Weeks(4);
        Assert.Equal(400 - 4 * g.C.Knowledge.Ramp, m.KnowGap);
        g.Weeks(20);
        Assert.Equal(0, m.KnowGap);
    }

    [Fact]
    public void Documentation_goes_stale_a_level_at_a_time()
    {
        Game g = Lab(c => c.Knowledge.DocStale = 5);
        ModelState m = Model(g);
        g.Do(Command.AssignOwner(g.Id, m.Id, Person(g, "researcher").Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Weeks(6);
        Assert.Equal(1, m.DocLevel);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DocsStale);
        g.Weeks(6);
        Assert.Equal(0, m.DocLevel);
        Assert.Equal(0, m.DocAge);
    }

    [Fact]
    public void A_model_with_a_big_gap_cannot_be_changed()
    {
        Game g = Lab();
        ModelState m = Model(g);
        Assert.False(Knowledge.Untouchable(g.C, m));
        m.KnowGap = g.C.Knowledge.GapRiskAt - 1;
        Assert.False(Knowledge.Untouchable(g.C, m));
        m.KnowGap = g.C.Knowledge.GapRiskAt;
        Assert.True(Knowledge.Untouchable(g.C, m));
    }

    [Fact]
    public void Nobody_hands_a_model_with_a_big_gap_to_agents_or_tunes_it()
    {
        Game g = Lab(c => { c.Agency.MinEra = 0; c.PostTraining.MinEra = 0; });
        g.Me.DataClean4 = 500;
        ModelState tuned = Model(g);
        tuned.KnowGap = g.C.Knowledge.GapRiskAt;
        g.Refused(Command.PostTrain(g.Id, tuned.Id, 1), CommandError.MissingPrerequisite);
        tuned.KnowGap = g.C.Knowledge.GapRiskAt - 1;
        g.Do(Command.PostTrain(g.Id, tuned.Id, 1));
        ModelState sold = Model(g);
        g.Do(Command.ReleaseModel(g.Id, sold.Id, 0, "game-ai"));
        sold.KnowGap = g.C.Knowledge.GapRiskAt;
        g.Refused(Command.SetAutonomy(g.Id, sold.Id, 1), CommandError.MissingPrerequisite);
        sold.KnowGap = 0;
        g.Do(Command.SetAutonomy(g.Id, sold.Id, 1));
    }

    [Fact]
    public void A_pivot_that_loses_the_owner_leaves_a_gap()
    {
        Game g = Lab(c => c.Pivot.StaffLossPermille = 340);
        ModelState m = Model(g);
        StaffMember r = Person(g, "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        r.Morale = 1;
        g.Do(Command.PivotStyle(g.Id, "shotgun"));
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == r.Id);
        Assert.Equal(g.C.Knowledge.GapBase, m.KnowGap);
        Assert.Equal(0, m.OwnerId);
    }

    [Fact]
    public void Saves_round_trip_an_owner_who_is_gone_is_a_problem_and_older_saves_have_none()
    {
        Game g = Lab();
        ModelState m = Model(g);
        g.Do(Command.AssignOwner(g.Id, m.Id, Person(g, "researcher").Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        m.OwnerId = 424242;
        Assert.NotEmpty(SaveGame.Problems(g.C, g.State));
        m.OwnerId = 0;
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 122, g.Sim.Save()));
        ModelState om = older.State.Labs.First(l => l.Id == g.Id).Models.First(x => x.Id == m.Id);
        Assert.Equal(0, om.OwnerId + om.DocLevel + om.DocAge + om.KnowGap + om.Handovers);
    }
}
