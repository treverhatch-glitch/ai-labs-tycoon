using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Grouped sketch: rules 41–44 (Q25315–Q25320).</summary>
public class RulesBatch392_395SketchLaterTests
{
    [Fact]
    public void Q25315_staff_people_config_has_morale_baseline()
    {
        Assert.True(TestData.Config().StaffPeople.MoraleBaseline > 0);
    }

    [Fact]
    public void Q25316_loans_config_caps_permille()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Loans.LoanToValuePermille, 1, 1000);
    }

    [Fact]
    public void Q25317_office_escape_has_corruption_effect_kind()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig esc = c.Office.Events.First(e => e.Trigger == OfficeTrigger.Escape);
        Assert.Contains(esc.Choices.SelectMany(ch => ch.Effects), e => e.Kind == OfficeEffectKind.Corruption);
    }

    [Fact]
    public void Q25318_scenario_defectors_lawsuit_exists()
    {
        Assert.NotNull(TestData.Config().Scenarios.Scenario("defectors-lawsuit"));
    }

    [Fact]
    public void Q25319_command_take_loan_registered()
    {
        var g = Game.Solo();
        Assert.Equal(CommandKind.TakeLoan, Command.TakeLoan(g.Id, 100).Kind);
    }

    [Fact]
    public void Q25320_run_quits_is_safe_on_empty_roster()
    {
        var g = Game.Solo();
        Assert.Equal(0, Traits.RunQuits(g.C, g.State, g.Me));
    }
}