using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The people react to what the player's care tools do: crunch ending, insurance, the offsite, red teams, findings and pay.</summary>
public class CareToolsChatTests
{
    static Game Lab(params string[] roles)
    {
        Game g = Game.Solo();
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        foreach (string role in roles) g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 });
        return g;
    }

    static ChatMessage Say(Game g, EventKind kind, int pick = 0, long amount = 0, string reference = "Orion 2")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, amount, reference);
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void Fourteen_lines_cover_seven_kinds_and_the_data_validates()
    {
        GameConfig c = TestData.Config();
        foreach (string kind in new[] { "CrunchEnded", "InsuranceClaimPaid", "InsuranceDropped", "OffsiteHeld", "RedTeamRan", "GapFound", "PayChanged" })
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        Assert.Equal(374, c.Chat.Lines.Count);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Crunch_ending_for_a_tired_team_and_a_stopped_one_have_their_own_lines_and_the_rest_get_the_plain_one()
    {
        Assert.Equal("crunch-tired", Say(Lab("researcher"), EventKind.CrunchEnded, 1, 9, "tired").LineId);
        Assert.Equal("crunch-tired", Say(Lab("engineer"), EventKind.CrunchEnded, 1, 9, "tired").LineId);
        Assert.Equal("crunch-stopped", Say(Lab("sales"), EventKind.CrunchEnded, 0, 4, "stopped").LineId);
        Assert.Equal("crunch-ended", Say(Lab("sales"), EventKind.CrunchEnded, -1, 26, "spent").LineId);
        Assert.Equal("crunch-ended", Say(Lab("sales"), EventKind.CrunchEnded, -1, 3, "empty").LineId);
    }

    [Fact]
    public void A_tired_crunch_with_no_researcher_or_engineer_falls_to_the_plain_line()
    {
        Assert.Equal("crunch-ended", Say(Lab("sales"), EventKind.CrunchEnded, 1, 9, "tired").LineId);
    }

    [Fact]
    public void A_real_crunch_that_gives_out_is_heard_by_the_researcher()
    {
        Game g = Game.Solo(2, c => c.Office.ChancePermille = 0).WithStarterLab(researchers: 3, engineers: 1, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Do(Command.SetCrunch(g.Id, true));
        for (int i = 0; i < 40 && g.Me.CrunchWeeks > 0; i++) g.Weeks(1);
        GameEvent ended = g.State.Events.Last(e => e.Kind == EventKind.CrunchEnded);
        Assert.Equal(ended.Ref == "tired" ? 1 : -1, ended.Pick);
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.Kind == EventKind.CrunchEnded);
    }

    [Fact]
    public void Insurance_news_is_the_insurers_own_words()
    {
        Assert.Equal("insurance-claim-chat", Say(Lab("sales"), EventKind.InsuranceClaimPaid, 0, 5000).LineId);
        Assert.StartsWith("The insurer has dropped us", Say(Lab("sales"), EventKind.InsuranceDropped, 0, 3).Text);
    }

    [Fact]
    public void A_researcher_or_engineer_answers_an_offsite_by_name_and_anyone_else_by_the_plain_line()
    {
        Assert.Equal("offsite-chat-eng", Say(Lab("sales", "researcher"), EventKind.OffsiteHeld).LineId);
        Assert.Equal("offsite-chat", Say(Lab("sales"), EventKind.OffsiteHeld).LineId);
    }

    [Fact]
    public void A_red_teamer_or_a_safety_person_names_the_model_and_the_others_use_the_plain_line()
    {
        ChatMessage m = Say(Lab("red-teamer"), EventKind.RedTeamRan, 0, 15000, "Orion 2");
        Assert.Equal("red-team-chat-safety", m.LineId);
        Assert.StartsWith("Orion 2 lasted forty minutes", m.Text);
        Assert.Equal("red-team-chat", Say(Lab("sales"), EventKind.RedTeamRan).LineId);
    }

    [Fact]
    public void A_finding_is_read_by_the_researchers_and_shrugged_at_by_the_rest()
    {
        Assert.Equal("gap-found-chat-research", Say(Lab("researcher"), EventKind.GapFound).LineId);
        Assert.Equal("gap-found-chat-research", Say(Lab("alignment-fellow"), EventKind.GapFound).LineId);
        Assert.Equal("gap-found-chat", Say(Lab("sales"), EventKind.GapFound).LineId);
    }

    [Fact]
    public void A_raise_and_a_cut_get_different_lines_and_a_researcher_has_one_more_for_a_cut()
    {
        Assert.Equal("pay-raise-chat", Say(Lab("researcher"), EventKind.PayChanged, 1).LineId);
        Assert.Equal("pay-cut-chat-research", Say(Lab("researcher"), EventKind.PayChanged, 0).LineId);
        Assert.Equal("pay-cut-chat", Say(Lab("sales"), EventKind.PayChanged, 0).LineId);
    }

    [Fact]
    public void A_real_pay_change_sets_the_pick_for_a_raise_and_a_cut()
    {
        Game g = Game.Solo(1, c => c.Office.ChancePermille = 0).WithStarterLab(researchers: 2, engineers: 1, gpus: 4);
        g.Me.Cash = 1_000_000_000_000;
        g.Do(Command.SetPayLevel(g.Id, 1));
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.PayChanged).Pick);
        g.Weeks(26);
        g.Do(Command.SetPayLevel(g.Id, -1));
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.PayChanged).Pick);
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.Kind == EventKind.PayChanged);
    }

    [Fact]
    public void A_lab_with_nobody_to_speak_says_nothing_and_events_of_another_lab_are_ignored()
    {
        Game g = Lab();
        Events.Add(g.State, EventKind.PayChanged, g.Id, 1, 100, "");
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
        Game h = Lab("researcher");
        Events.Add(h.State, EventKind.RedTeamRan, 999, 1, 100, "Orion 2");
        Assert.Empty(CompanyChat.Feed(h.C, h.State, h.Me, 5));
    }
}
