using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q53: safety.sectorIncidentHeadlines text only (era owns safety.sectorFlavours economics).</summary>
public class SectorIncidentHeadlineTests
{
    static Game RiskyLab() => Game.Solo(tweak: c =>
    {
        c.Safety.IncidentChancePermille = 1000;
        c.Safety.Rung2BelowAlignment = 1000;
        c.Shady.CorruptionIncidentWeight = 0;
    });

    static ModelState ReleaseZeroAlign(Game g, params string[] sectors)
    {
        g.WithStarterLab(gpus: 8);
        int trainSize = sectors.Any(s => s == "expert-systems") ? 300 : 0;
        if (sectors.Any(s => s == "expert-systems"))
            g.Weeks(52);
        ModelState model = g.Train("mini", trainSize);
        g.UntilTrained(model);
        if (sectors.Any(s => s == "expert-systems"))
            model.Capability = 500;
        model.Alignment = 0;
        model.DisplayedAlignment = 0;
        g.Release(model, sectors);
        return model;
    }

    [Fact]
    public void Shipped_sector_flavours_under_safety_config()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(4, c.Safety.SectorIncidentHeadlines.Count);
        Assert.Equal(3, c.Safety.SectorIncidentHeadlines.Single(f => f.SectorId == "game-ai").Headlines.Count);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Game_ai_incident_uses_sector_flavour_headline()
    {
        Game g = RiskyLab();
        ModelState model = ReleaseZeroAlign(g, "game-ai");
        g.Weeks(1);
        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        Assert.Equal("game-ai", incident.Ref);
        string text = EventText.Describe(g.C, g.State, incident);
        Assert.Contains(g.C.Safety.SectorIncidentHeadlines.Single(f => f.SectorId == "game-ai").Headlines
            .Select(h => h.Replace("{model}", model.Name)), h => text.Contains(h));
    }

    [Fact]
    public void Sector_without_flavour_keeps_era_headline_ref()
    {
        Game g = RiskyLab();
        ReleaseZeroAlign(g, "expert-systems");
        g.Weeks(1);
        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        ModelState model = g.Me.Model(incident.SubjectId);
        Assert.Equal(model.Name, incident.Ref);
    }

    [Fact]
    public void SafetyQueries_null_for_unknown_sector() =>
        Assert.Null(SafetyQueries.SectorIncidentHeadlines(TestData.Config(), "not-a-sector"));

    [Fact]
    public void First_sector_wins_when_model_in_two_sectors()
    {
        Game g = RiskyLab();
        ReleaseZeroAlign(g, "game-ai", "expert-systems");
        g.Weeks(1);
        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        Assert.Equal("game-ai", incident.Ref);
    }

    [Fact]
    public void Flavour_does_not_add_fine_multiplier_beyond_rung()
    {
        Game g = RiskyLab();
        ModelState model = ReleaseZeroAlign(g, "game-ai");
        g.Weeks(1);
        GameEvent incident = g.State.Events.Last(e => e.Kind == EventKind.Incident);
        IncidentRungConfig rung = g.C.Safety.IncidentRung(model.LastIncidentRung);
        long baseFine = MathI.Max(g.C.Safety.IncidentFineMin,
            MarketSystem.ModelRevenue(g.State, g.Id, model.Id) * g.C.Safety.IncidentFineRevenueWeeks);
        long expected = MathI.Max(g.C.Safety.IncidentFineMin, MathI.MulPermille(baseFine, rung.FinePermille));
        Assert.Equal(expected, incident.Amount);
    }

    [Fact]
    public void Flavour_does_not_change_reputation_loss()
    {
        Game g = RiskyLab();
        int rep0 = g.Me.Reputation;
        ReleaseZeroAlign(g, "game-ai");
        g.Weeks(1);
        int loss = rep0 - g.Me.Reputation;
        IncidentRungConfig rung = g.C.Safety.IncidentRung(g.Me.Models[0].LastIncidentRung);
        Assert.Equal(rung.ReputationLoss, loss);
    }

    [Fact]
    public void Flavour_still_increments_incidents_this_month()
    {
        Game g = RiskyLab();
        ReleaseZeroAlign(g, "game-ai");
        g.Weeks(1);
        Assert.Equal(1, g.Me.IncidentsThisMonth);
    }

    [Fact]
    public void Validator_rejects_unknown_sector_flavour()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.SectorIncidentHeadlines.Add(new SectorIncidentHeadlineConfig { SectorId = "void-sector", Headlines = new System.Collections.Generic.List<string> { "{model} oops" } });
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("unknown sector"));
    }

    [Fact]
    public void Validator_rejects_headline_without_model_placeholder()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.SectorIncidentHeadlines[0].Headlines.Add("no placeholder");
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("must contain {model}"));
    }

    [Fact]
    public void Validator_rejects_duplicate_sector_flavour()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.SectorIncidentHeadlines.Add(new SectorIncidentHeadlineConfig
        {
            SectorId = "game-ai",
            Headlines = new System.Collections.Generic.List<string> { "{model} again" },
        });
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("twice"));
    }

    [Fact]
    public void Validator_rejects_empty_flavour_list()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.SectorIncidentHeadlines.Add(new SectorIncidentHeadlineConfig { SectorId = "web-ads", Headlines = new System.Collections.Generic.List<string>() });
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("at least one headline"));
    }
}