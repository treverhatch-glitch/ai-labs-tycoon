using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-fifteen headlines and chat survive saves (Q467).</summary>
public class EraOBatchFifteenNewsChatHardeningTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.CampaignDeadlineSet,
        EventKind.CampaignDeadlineHit,
        EventKind.LawShaped,
        EventKind.ScenarioBaggagePicked,
    };

    [Fact]
    public void Batch_fifteen_counts_survive_mid_save()
    {
        Game g = Game.Solo(130, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        GameConfig c = TestData.Config();
        Assert.Equal(398, c.News.Headlines.Count);
        Assert.Equal(352, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Empty(ConfigValidator.Problems(g.C));
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Law_shaped_headline_after_schema_143_round_trip()
    {
        Game g = Game.Solo(131, c =>
        {
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        }).WithStarterLab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.LawShaped, sim.Player.Id, -1, 0, "shape");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }
}