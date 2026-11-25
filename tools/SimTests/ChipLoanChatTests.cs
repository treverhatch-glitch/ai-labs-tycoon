using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The people react to the chip business and the bank: tapeouts, respins, crashes, snubs, deliveries and loans.</summary>
public class ChipLoanChatTests
{
    static Game Lab(params string[] roles)
    {
        Game g = Game.Solo();
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        foreach (string role in roles) g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 });
        return g;
    }

    static ChatMessage Say(Game g, EventKind kind, long amount = 0, string reference = "Pixie 1")
    {
        g.State.Events.Clear();
        Events.Add(g.State, kind, g.Id, 1, amount, reference);
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void Ten_lines_cover_eight_kinds_and_the_data_validates()
    {
        GameConfig c = TestData.Config();
        string[] kinds = { "ChipTapedOut", "ChipRespin", "ChipYieldCrash", "ChipsDelivered", "ChipSlotSnubbed", "LoanTaken", "LoanMissed", "LoanRepaid" };
        foreach (string kind in kinds) Assert.Contains(c.Chat.Lines, l => l.Kind == kind && l.Roles.Count == 0);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_chip_architect_answers_a_tapeout_by_name_and_anyone_else_by_the_plain_line()
    {
        ChatMessage m = Say(Lab("sales", "chip-architect"), EventKind.ChipTapedOut);
        Assert.Equal("chip-taped-out-eng", m.LineId);
        Assert.Equal("chip-architect", m.RoleId);
        Assert.StartsWith("Pixie 1 is at the foundry", m.Text);
        Assert.Equal("chip-taped-out", Say(Lab("sales"), EventKind.ChipTapedOut).LineId);
    }

    [Fact]
    public void A_respin_is_a_one_line_bug_to_an_engineer()
    {
        Assert.Equal("chip-respin-eng", Say(Lab("engineer"), EventKind.ChipRespin).LineId);
        Assert.Equal("chip-respin", Say(Lab("researcher"), EventKind.ChipRespin).LineId);
    }

    [Fact]
    public void A_crash_a_delivery_and_a_snub_say_how_many()
    {
        Game g = Lab("researcher");
        Assert.Equal("Only 800 chips came out. The rest, I am told, are 'theoretical'.", Say(g, EventKind.ChipYieldCrash, 800).Text);
        Assert.Equal("1200 chips arrived. They are warm and slightly smug.", Say(g, EventKind.ChipsDelivered, 1200, "site").Text);
        Assert.Equal("The foundry moved us back 4 weeks. They were very nice about it, which was worse.", Say(g, EventKind.ChipSlotSnubbed, 4, "9").Text);
    }

    [Fact]
    public void A_loan_is_taken_missed_and_repaid_in_the_peoples_words()
    {
        Game g = Lab("researcher");
        Assert.Equal("loan-taken", Say(g, EventKind.LoanTaken, 50_000_000, "").LineId);
        Assert.Equal("loan-missed", Say(g, EventKind.LoanMissed, 2_000_000, "").LineId);
        Assert.Equal("loan-repaid", Say(g, EventKind.LoanRepaid, 50_000_000, "term").LineId);
    }

    [Fact]
    public void Nobody_on_the_payroll_means_nobody_speaks()
    {
        Game g = Lab();
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.LoanTaken, g.Id, 1, 1, "");
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
    }
}
