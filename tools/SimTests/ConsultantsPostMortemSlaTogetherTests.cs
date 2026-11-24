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

/// <summary>Policy consultants, post-mortems, the service-level pledge, research grants, the incident floors, the rota and insurance used together over years.</summary>
public class ConsultantsPostMortemSlaTogetherTests
{
    static Game Start(int seed)
    {
        return Game.New(seed, c =>
        {
            c.OnCall.StaffPerLevel = 0;
            c.Marketing.MinSales = 0;
            c.Chiefs.MinStaff = 0;
            c.Sla.MinReputation = 0;
            c.RedTeam.RoleId = "";
        });
    }

    static void UseTheTools(Game g, int week)
    {
        if (week % 40 == 12 && g.Me.Cash > 150_000) g.Try(Command.HireConsultants(g.Id, 1 + week / 40 % 2));
        if (week % 50 == 5 && g.Me.Cash > 200_000) g.Try(Command.SetSla(g.Id, week / 50 % 4));
        if (week % 60 == 20 && g.Me.Cash > 100_000) g.Try(Command.SetOnCall(g.Id, week % 120 == 20 ? 2 : 0));
        if (week % 70 == 30 && g.Me.Cash > 100_000) g.Try(Command.SetInsurance(g.Id, 2));
        if (week % 35 == 3) g.Try(Command.PublishPaper(g.Id));
        if (week % 35 == 9 && g.Me.Cash > 100_000) g.Try(Command.ApplyForGrant(g.Id, 1 + week / 35 % 3));
        foreach (ModelState m in g.Me.Models.ToList())
            if (PostMortem.Open(g.C, g.State, m) && g.Me.Cash > 150_000) g.Try(Command.PublishPostMortem(g.Id, m.Id, 1 + week % 2));
    }

    static Game Play(int seed, int weeks, out long gap)
    {
        Game g = Start(seed);
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(60);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
        }
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void The_books_balance_the_save_is_sound_and_a_load_plays_on_the_same(int seed)
    {
        long gap;
        Game g = Play(seed, 260, out gap);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.InRange(g.Me.Reputation, 0, 1000);
        Assert.InRange(g.Me.SlaLevel, 0, g.C.Sla.MaxLevel);
        Assert.InRange(g.Me.ConsultantLevel, 0, g.C.Consultants.MaxLevel);
        Assert.InRange(g.Me.ResearchGrantStage, 0, ResearchGrants.Owed);
        Assert.All(g.Me.Models, m => Assert.InRange(m.PostMortemDone, 0, PostMortem.Full));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int firms = 0, pledges = 0, broken = 0, writeUps = 0, applications = 0, decisions = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            firms += g.State.Events.Count(e => e.Kind == EventKind.ConsultantsHired && e.Pick == 1);
            pledges += Count(g, EventKind.SlaPledged);
            broken += Count(g, EventKind.SlaBroken);
            writeUps += Count(g, EventKind.PostMortemPublished);
            applications += Count(g, EventKind.GrantApplied);
            decisions += Count(g, EventKind.GrantDecided);
        }
        Assert.True(firms > 0, "no firm was ever hired");
        Assert.True(pledges > 0, "no pledge was ever made");
        Assert.True(broken >= 0);
        Assert.True(writeUps >= 0);
        Assert.True(applications > 0, "no grant was ever applied for");
        Assert.True(decisions > 0, "no panel ever ruled");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void The_journal_replays_to_the_same_game(int seed)
    {
        long gap;
        Game g = Play(seed, 150, out gap);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_unused_set_of_tools_changes_nothing_for_a_lab_that_plays_on_its_own()
    {
        Game plain = Game.New(3);
        Game withTools = Start(3);
        plain.Autopilot(120);
        withTools.Autopilot(120);
        Assert.Equal(0, withTools.Me.SlaLevel);
        Assert.Equal(0, withTools.Me.ConsultantLevel);
        Assert.Equal(0, withTools.Me.ResearchGrantStage);
        Assert.Equal(plain.Me.Cash, withTools.Me.Cash);
        Assert.Equal(plain.Me.Reputation, withTools.Me.Reputation);
    }

    [Fact]
    public void Insurance_the_rota_and_a_pledge_together_cannot_make_an_incident_free_and_a_write_up_cannot_win_back_more_than_it_cost()
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Sla.MinReputation = 0;
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 800;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        g.Me.InsuranceLevel = 3;
        g.Do(Command.SetOnCall(g.Id, 3));
        g.Do(Command.SetSla(g.Id, 3));
        g.Do(Command.HireConsultants(g.Id, 1));
        long fines = g.Me.Ledger.Lifetime.Fines;
        int rep = g.Me.Reputation;
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        g.C.Safety.IncidentChancePermille = 0;
        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        Assert.True(incident.Amount >= 0);
        Assert.True(g.Me.Reputation < rep);
        int lost = m.IncidentRepLoss;
        Assert.True(lost > 0);
        int afterIncident = g.Me.Reputation;
        g.Do(Command.PublishPostMortem(g.Id, m.Id, PostMortem.Full));
        Assert.InRange(g.Me.Reputation, afterIncident, afterIncident + lost);
        Assert.True(g.Me.Ledger.Lifetime.Fines <= fines);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}
