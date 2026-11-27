using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q518 combination wiring for batch twenty-seven systems.</summary>
public class BatchTwentySevenTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(131, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Escape_crisis_gets_era_grace()
    {
        Game g = Lab(c =>
        {
            c.EscapeCrisisEraGrace.Enabled = 1;
            c.EscapeCrisisEraGrace.MinEra = 0;
            c.EscapeCrisisEraGrace.GraceWeeks = 5;
        });
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EscapeCrisisEraGraced);
        Assert.Equal(g.Tick + g.C.Office.ExpireWeeks + 5, g.Me.OfficeEventExpires);
    }

    [Fact]
    public void Law_shape_fires_era_amplify()
    {
        Game g = Lab(c =>
        {
            c.LawShaping.Enabled = 1;
            c.LawShaping.MinEra = 0;
            c.LawShapingEraAmplify.Enabled = 1;
            c.LawShapingEraAmplify.MinEra = 0;
            c.LawShapingEraAmplify.AmplifyPermille = 200;
        });
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Strict));
        g.Do(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawShapingEraAmplified);
        Assert.True(g.Me.LawShapePending > g.C.LawShaping.StrictDelta);
    }

    [Fact]
    public void Data_source_renew_discount()
    {
        Game g = Lab(c =>
        {
            c.DataSourceRenewEraDiscount.Enabled = 1;
            c.DataSourceRenewEraDiscount.MinEra = 0;
            c.DataSourceRenewEraDiscount.DiscountPermille = 250;
        });
        const string sid = "expert-labels";
        g.Do(Command.AcquireDataSource(g.Id, sid));
        long cash = g.Me.Cash;
        g.Do(Command.RenewDataSource(g.Id, sid));
        long cost = g.C.DataSources.Sources.Find(s => s.Id == sid).WeeklyCost;
        long expected = cost - cost * 250 / 1000;
        Assert.Equal(cash - expected, g.Me.Cash);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSourceRenewEraDiscounted);
    }
}