using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ReleaseChecklistTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(11, c =>
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
            c.ReleaseChecklist.MinEra = 0;
            c.EvalHarness.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.Agency.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    static void RedActive(Game g, ModelState m)
    {
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
    }

    static void ReadyAll(Game g, ModelState m)
    {
        RedActive(g, m);
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        m.ToolMask = 1;
        m.AgentLevel = 1;
    }

    [Fact]
    public void A_lab_with_no_checklist_releases_as_before()
    {
        Game g = Lab(c => c.ReleaseChecklist.Enabled = 0);
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Equal(0, m.ChkDone);
        Assert.Equal(0, m.ChkMissed);
    }

    [Fact]
    public void Release_is_blocked_until_every_item_is_signed()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"), CommandError.InvalidArgument);
        ReadyAll(g, m);
        foreach (int item in new[] { ReleaseChecklist.Red, ReleaseChecklist.Docs, ReleaseChecklist.Eval, ReleaseChecklist.Policy })
            g.Do(Command.SignChecklistItem(g.Id, m.Id, item));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Equal(ReleaseChecklist.All, m.ChkDone);
        Assert.Equal(0, m.ChkMissed);
    }

    [Fact]
    public void Each_item_is_checked_and_needs_its_evidence()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Refused(Command.SignChecklistItem(g.Id, m.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SignChecklistItem(g.Id, 999, ReleaseChecklist.Red), CommandError.UnknownId);
        g.Refused(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red), CommandError.MissingPrerequisite);
        RedActive(g, m);
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red));
        g.Refused(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red), CommandError.InvalidArgument);
        g.Refused(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs), CommandError.MissingPrerequisite);
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs));
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Eval));
        m.ToolMask = ToolPolicy.Web;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Policy));
        Assert.Equal(ReleaseChecklist.All, m.ChkDone);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChecklistSigned);
        Game off = Lab(c => c.ReleaseChecklist.Enabled = 0);
        ModelState om = Trained(off);
        off.Refused(Command.SignChecklistItem(off.Id, om.Id, ReleaseChecklist.Red), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_first_release_remembers_what_was_missed()
    {
        Game g = Lab(c => c.ReleaseChecklist.MinEra = c.Eras.Count);
        ModelState m = Trained(g);
        RedActive(g, m);
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red));
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Equal(ReleaseChecklist.Eval | ReleaseChecklist.Policy, m.ChkMissed);
    }

    [Fact]
    public void Missed_items_cost_trust_every_week_after_release()
    {
        Game g = Lab(c => c.ReleaseChecklist.MissTrust = -5);
        ModelState m = Trained(g);
        g.C.ReleaseChecklist.Enabled = 0;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.ChkMissed = ReleaseChecklist.Red | ReleaseChecklist.Docs;
        g.C.ReleaseChecklist.Enabled = 1;
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        ReleaseChecklist.RunWeek(g.C, g.State, g.Me);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) <= trust - 10);
    }

    [Fact]
    public void Blocks_respects_min_era_and_a_full_checklist()
    {
        Game g = Lab(c => c.ReleaseChecklist.MinEra = c.Eras.Count);
        ModelState m = Trained(g);
        Assert.False(ReleaseChecklist.Blocks(g.C, g.State, m));
        g.C.ReleaseChecklist.MinEra = 0;
        Assert.True(ReleaseChecklist.Blocks(g.C, g.State, m));
        m.ChkDone = ReleaseChecklist.All;
        Assert.False(ReleaseChecklist.Blocks(g.C, g.State, m));
    }

    [Fact]
    public void Can_sign_reflects_red_team_docs_eval_and_policy()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        Assert.False(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Red));
        RedActive(g, m);
        Assert.True(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Red));
        Assert.False(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Eval));
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        Assert.True(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Eval));
        m.ToolMask = 0;
        Assert.False(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Policy));
        m.ToolMask = 2;
        Assert.True(ReleaseChecklist.CanSign(g.C, g.State, m, ReleaseChecklist.Policy));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_checklist_bits()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        ReadyAll(g, m);
        foreach (int item in new[] { ReleaseChecklist.Red, ReleaseChecklist.Docs, ReleaseChecklist.Eval, ReleaseChecklist.Policy })
            g.Do(Command.SignChecklistItem(g.Id, m.Id, item));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 128, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.ChkDone + x.ChkMissed));
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}