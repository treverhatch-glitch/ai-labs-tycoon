using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q493 combination wiring for batch twenty-one systems.</summary>
public class BatchTwentyOneTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(91, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.LawShaping.Enabled = 1;
            c.DataSources.Enabled = 1;
            c.SectorPricing.Enabled = 1;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Law_cooldown_shortens_after_shape_law()
    {
        Game g = Lab(c =>
        {
            c.LawShaping.Enabled = 1;
            c.LawShaping.MinEra = 0;
            c.LawShapingEraCooldown.Enabled = 1;
            c.LawShapingEraCooldown.MinEra = 0;
            c.LawShapingEraCooldown.CooldownCutWeeks = 5;
            c.Lobbying.CooldownWeeks = 12;
        });
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Strict));
        g.Do(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawShapingCooldownShortened);
    }

    [Fact]
    public void Renew_adds_era_bonus_weeks()
    {
        Game g = Lab(c =>
        {
            c.DataSourceRenewEraBonus.Enabled = 1;
            c.DataSourceRenewEraBonus.MinEra = 0;
            c.DataSourceRenewEraBonus.BonusWeeks = 4;
        });
        const string src = "expert-labels";
        g.Do(Command.AcquireDataSource(g.Id, src));
        g.Do(Command.RenewDataSource(g.Id, src));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSourceRenewEraBonused);
    }

    [Fact]
    public void Sector_tier_set_fires_era_relief()
    {
        Game g = Lab(c =>
        {
            c.SectorPricingEraRelief.Enabled = 1;
            c.SectorPricingEraRelief.MinEra = 0;
            c.SectorPricingEraRelief.ReliefPermille = 10;
        });
        g.Do(Command.SetSectorPrice(g.Id, "game-ai", 2));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SectorPricingEraRelieved);
        int overload = SectorPricing.LabOverloadPermille(g.C, g.State, g.Me);
        Assert.True(overload < 2 * g.C.SectorPricing.OverloadPerTier);
    }
}