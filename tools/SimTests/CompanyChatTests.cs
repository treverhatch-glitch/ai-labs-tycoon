using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The company chat: staff react to what happened to the lab, in character, read off the event log.</summary>
public class CompanyChatTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: tweak);
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        return g;
    }

    static StaffMember Add(Game g, string role, string trait = "steady")
    {
        var person = new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = trait, Morale = 500 };
        g.Me.Staff.Add(person);
        return person;
    }

    static GameEvent Event(Game g, EventKind kind, int subject = -1, long amount = 0, string reference = "", int pick = 0, int lab = -2)
    {
        GameEvent e = Events.Add(g.State, kind, lab == -2 ? g.Id : lab, subject, amount, reference);
        e.Pick = pick;
        return e;
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    static ChatMessage Only(Game g)
    {
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, g.Me);
        Assert.Single(feed);
        return feed[0];
    }

    // ---- the table

    [Fact]
    public void The_table_has_its_lines_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(132, c.Chat.Lines.Count);
        Assert.Equal(12, c.Chat.DefaultLines);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_line_has_a_real_event_kind_and_a_plain_line_for_each_kind_with_a_trait_line()
    {
        GameConfig c = TestData.Config();
        foreach (ChatLineConfig line in c.Chat.Lines) Assert.True(CompanyChat.IsEventKind(line.Kind), line.IdValue);
        foreach (var group in c.Chat.Lines.GroupBy(l => l.Kind))
        {
            bool hasTraitLine = group.Any(l => l.Trait.Length > 0);
            bool hasOpenLine = group.Any(l => l.Trait.Length == 0 && l.Roles.Count == 0);
            if (hasTraitLine) Assert.True(hasOpenLine || group.Any(l => l.Trait.Length == 0), group.Key + " has trait lines and nothing for everyone else");
        }
    }

    [Fact]
    public void A_line_for_a_trait_comes_before_the_plain_line_of_its_kind()
    {
        GameConfig c = TestData.Config();
        foreach (var group in c.Chat.Lines.GroupBy(l => l.Kind))
        {
            List<ChatLineConfig> lines = group.ToList();
            int firstPlain = lines.FindIndex(l => l.Trait.Length == 0);
            if (firstPlain < 0) continue;
            for (int i = firstPlain; i < lines.Count; i++) Assert.True(lines[i].Trait.Length == 0, lines[i].IdValue + " hides behind a plain line");
        }
    }

    [Fact]
    public void Every_placeholder_in_every_line_is_filled_in()
    {
        Game g = Lab();
        ModelState model = new ModelState { Id = g.State.TakeId(), Name = "Fable Mini 1", LineId = "mini", Status = ModelStatus.Released };
        g.Me.Models.Add(model);
        foreach (ChatLineConfig line in g.C.Chat.Lines)
        {
            g.Me.Staff.Clear();
            Add(g, g.C.Role(line.Roles.Count > 0 ? line.Roles[0] : "researcher").Id, line.Trait.Length > 0 ? line.Trait : "steady");
            EventKind kind = (EventKind)Enum.Parse(typeof(EventKind), line.Kind);
            var e = new GameEvent { Tick = 1, Kind = kind, LabId = line.About == "lab" ? g.Id : 99, SubjectId = line.About == "subject" ? g.Id : model.Id, Amount = 7, Pick = Math.Max(0, line.Pick), Ref = "game-ai" };
            ChatMessage m;
            Assert.True(CompanyChat.Say(g.C, g.State, g.Me, e, out m), line.IdValue);
            Assert.DoesNotContain("{", m.Text);
            Assert.NotEmpty(m.Text);
        }
    }

    // ---- who speaks

    [Fact]
    public void With_nobody_on_the_payroll_the_chat_is_silent()
    {
        Game g = Lab();
        Event(g, EventKind.Hired, -1, 1, "researcher");
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_person_on_the_payroll_answers_a_hire()
    {
        Game g = Lab();
        StaffMember p = Add(g, "engineer");
        Event(g, EventKind.Hired, -1, 1, "engineer").Tick = GameDate.TickOfYear(g.C.Calendar, 2012);
        ChatMessage m = Only(g);
        Assert.Equal("hired", m.LineId);
        Assert.Equal(p.Id, m.StaffId);
        Assert.Equal("Engineer " + p.Id, m.Speaker);
        Assert.Contains("coffee machine", m.Text);
    }

    [Fact]
    public void A_line_for_a_trait_is_said_by_someone_with_that_trait()
    {
        Game g = Lab();
        Add(g, "researcher");
        StaffMember sleeper = Add(g, "engineer", "sleeps-under-desk");
        Event(g, EventKind.Hired, -1, 1, "researcher");
        ChatMessage m = Only(g);
        Assert.Equal("hired-sleeper", m.LineId);
        Assert.Equal(sleeper.Id, m.StaffId);
        Assert.Equal("sleeps-under-desk", m.TraitId);
    }

    [Fact]
    public void A_line_for_a_trait_is_skipped_when_nobody_has_it()
    {
        Game g = Lab();
        Add(g, "researcher");
        Event(g, EventKind.Hired, -1, 1, "researcher").Tick = GameDate.TickOfYear(g.C.Calendar, 2012);
        Assert.Equal("hired", Only(g).LineId);
    }

    [Fact]
    public void A_line_for_a_role_falls_through_to_the_plain_line_without_that_role()
    {
        Game g = Lab();
        Add(g, "sales");
        Event(g, EventKind.Incident, -1, 5000, "", 0);
        Assert.Equal("incident", Only(g).LineId);
        Add(g, "red-teamer");
        Assert.Equal("incident-safety", Only(g).LineId);
        Assert.Equal("red-teamer", Only(g).RoleId);
    }

    [Fact]
    public void The_first_trait_in_the_list_that_someone_has_wins()
    {
        Game g = Lab();
        Add(g, "researcher", "accelerationist");
        Add(g, "researcher", "reads-the-logs");
        Event(g, EventKind.Incident, -1, 100);
        Assert.Equal("incident-logs", Only(g).LineId);
    }

    [Fact]
    public void A_yes_or_no_event_picks_its_own_line()
    {
        Game g = Lab();
        Add(g, "sales");
        Event(g, EventKind.PressAnswered, 1, 10, "honest|nell|launch", 1);
        Assert.Equal("press-good", Only(g).LineId);
        g.State.Events.Clear();
        Event(g, EventKind.PressAnswered, 1, -10, "spin|nell|launch", 0);
        Assert.Equal("press-bad", Only(g).LineId);
    }

    [Fact]
    public void An_event_kind_nobody_wrote_a_line_for_is_not_in_the_chat()
    {
        Game g = Lab();
        Add(g, "researcher");
        Event(g, EventKind.LandBought, -1, 100, "somewhere");
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me));
        ChatMessage m;
        Assert.False(CompanyChat.Say(g.C, g.State, g.Me, g.State.Events[0], out m));
    }

    // ---- what they say

    [Fact]
    public void The_model_in_a_line_is_the_models_name()
    {
        Game g = Lab();
        Add(g, "safety");
        var model = new ModelState { Id = g.State.TakeId(), Name = "Fable Mini 4 Pro", LineId = "mini", Status = ModelStatus.Released };
        g.Me.Models.Add(model);
        Event(g, EventKind.Incident, model.Id, 5000);
        Assert.Contains("Fable Mini 4 Pro", Only(g).Text);
    }

    [Fact]
    public void The_amount_and_the_sector_are_filled_in()
    {
        Game g = Lab();
        Add(g, "researcher");
        Add(g, "sales");
        Event(g, EventKind.TrainingFinished, -1, 612, "Fable Mini 1");
        Assert.Contains("Fable Mini 1 finished at capability 612", Only(g).Text);
        g.State.Events.Clear();
        Event(g, EventKind.TrustLost, -1, 80, "game-ai");
        Assert.Contains(g.C.Sector("game-ai").Name, Only(g).Text);
    }

    // ---- whose events

    [Fact]
    public void Another_labs_events_are_not_in_the_chat()
    {
        Game g = Lab();
        Add(g, "researcher");
        Event(g, EventKind.Hired, -1, 1, "researcher", 0, lab: 999);
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me));
    }

    [Fact]
    public void A_poach_is_about_the_lab_it_was_done_to()
    {
        Game g = Lab();
        Add(g, "researcher");
        Event(g, EventKind.StaffPoached, g.Id, 1, "researcher", 0, lab: 999);
        Assert.Equal("poached", Only(g).LineId);
        g.State.Events.Clear();
        Event(g, EventKind.StaffPoached, 12345, 1, "researcher", 0, lab: g.Id);
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me));
    }

    // ---- the feed

    [Fact]
    public void The_feed_is_oldest_first_and_keeps_only_the_newest_lines()
    {
        Game g = Lab();
        Add(g, "researcher");
        for (int i = 0; i < 20; i++) { g.State.Tick = i; Event(g, EventKind.RoomBuilt, 100 + i, 5000, "lab"); }
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, g.Me, 5);
        Assert.Equal(new[] { 15, 16, 17, 18, 19 }, feed.Select(m => m.Tick).ToArray());
        Assert.Equal(12, CompanyChat.Feed(g.C, g.State, g.Me).Count);
    }

    [Fact]
    public void Events_with_nothing_said_do_not_use_up_the_feeds_lines()
    {
        Game g = Lab();
        Add(g, "researcher");
        Event(g, EventKind.RoomBuilt, 1, 100, "lab");
        for (int i = 0; i < 20; i++) Event(g, EventKind.LandBought, -1, 1, "x");
        Assert.Single(CompanyChat.Feed(g.C, g.State, g.Me, 3));
    }

    [Fact]
    public void The_speaker_for_an_event_is_stable_and_the_whole_payroll_gets_a_turn()
    {
        Game g = Lab();
        for (int i = 0; i < 8; i++) Add(g, "researcher");
        for (int i = 0; i < 40; i++) { g.State.Tick = i; Event(g, EventKind.TechResearched, -1, i, "frontier"); }
        List<ChatMessage> a = CompanyChat.Feed(g.C, g.State, g.Me, 40);
        List<ChatMessage> b = CompanyChat.Feed(g.C, g.State, g.Me, 40);
        Assert.Equal(a.Select(m => m.StaffId), b.Select(m => m.StaffId));
        Assert.True(a.Select(m => m.StaffId).Distinct().Count() >= 5);
    }

    [Fact]
    public void Reading_the_chat_changes_nothing()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Weeks(30);
        string before = g.Sim.Digest();
        CompanyChat.Feed(g.C, g.State, g.Me, 50);
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void A_played_game_fills_the_chat_with_finished_sentences()
    {
        Game g = Game.New(seed: 3);
        g.Autopilot(3 * 48);
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, g.Me, 30);
        Assert.NotEmpty(feed);
        Assert.All(feed, m =>
        {
            Assert.NotEmpty(m.Text);
            Assert.DoesNotContain("{", m.Text);
            Assert.NotNull(g.Me.Staff.FirstOrDefault(p => p.Id == m.StaffId));
        });
    }

    [Fact]
    public void Rivals_have_a_chat_too()
    {
        Game g = Game.New(seed: 3);
        g.Autopilot(3 * 48);
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, rival, 30);
        Assert.All(feed, m => Assert.NotNull(rival.Staff.FirstOrDefault(p => p.Id == m.StaffId)));
    }

    // ---- the validator

    [Fact]
    public void The_validator_names_each_chat_line_that_is_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.Chat.DefaultLines = 0), "chat.defaultLines");
        AssertProblem(Problems(c => c.Chat.Lines.Clear()), "chat.lines is empty");
        AssertProblem(Problems(c => c.Chat.Lines[0].Kind = "Nope"), "unknown event kind");
        AssertProblem(Problems(c => c.Chat.Lines[0].Text = " "), "has no text");
        AssertProblem(Problems(c => c.Chat.Lines[0].Pick = 2), "needs pick");
        AssertProblem(Problems(c => c.Chat.Lines[0].About = "nobody"), "needs about");
        AssertProblem(Problems(c => c.Chat.Lines[0].Roles.Add("no-such-role")), "unknown role");
        AssertProblem(Problems(c => c.Chat.Lines[0].Trait = "no-such-trait"), "unknown trait");
        AssertProblem(Problems(c => c.Chat.Lines[1].IdValue = c.Chat.Lines[0].IdValue), "chat.lines");
    }
}
