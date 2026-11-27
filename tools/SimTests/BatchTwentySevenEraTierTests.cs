using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Schema 180–182 edge paths (Q515–Q517).</summary>
public class BatchTwentySevenEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(141, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Main_paths_emit_events_when_era_met()
    {
        Game g = Lab(c =>
        {
            c.EscapeCrisisEraGrace.Enabled = 1;
            c.EscapeCrisisEraGrace.MinEra = 0;
            c.EscapeCrisisEraGrace.GraceWeeks = 1;
        });
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EscapeCrisisEraGraced);
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.EscapeCrisisEraGrace.Enabled = 0;
            c.LawShapingEraAmplify.Enabled = 0;
            c.DataSourceRenewEraDiscount.Enabled = 0;
        });
        Assert.Equal(g.C.Office.ExpireWeeks, EscapeCrisisEraGrace.AnswerWeeks(g.C, g.State));
        int delta = g.C.LawShaping.StrictDelta;
        Assert.Equal(delta, LawShapingEraAmplify.ShapeDelta(g.C, g.State, delta));
        Assert.Equal(1000L, DataSourceRenewEraDiscount.RenewCost(g.C, g.State, 1000));
    }

    [Fact]
    public void Max_amplify_boosts_delta()
    {
        Game g = Lab(c =>
        {
            c.LawShapingEraAmplify.Enabled = 1;
            c.LawShapingEraAmplify.MinEra = 0;
            c.LawShapingEraAmplify.AmplifyPermille = 500;
        });
        int baseDelta = 100;
        Assert.Equal(150, LawShapingEraAmplify.ShapeDelta(g.C, g.State, baseDelta));
    }

    [Fact]
    public void Max_discount_on_renew()
    {
        Game g = Lab(c =>
        {
            c.DataSourceRenewEraDiscount.Enabled = 1;
            c.DataSourceRenewEraDiscount.MinEra = 0;
            c.DataSourceRenewEraDiscount.DiscountPermille = 900;
        });
        Assert.Equal(100, DataSourceRenewEraDiscount.RenewCost(g.C, g.State, 1000));
    }
}