using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>News and chat lines for batch-fourteen systems (Q461) survive saves and migrations.</summary>
public class EraNBatchFourteenNewsChatHardeningTests
{
    static readonly EventKind[] BatchFourteenKinds =
    {
        EventKind.SpecialistBranched,
        EventKind.DataSourceAcquired,
        EventKind.DataSourceRenewed,
        EventKind.DataSourceLapsed,
        EventKind.WeightsOpened,
        EventKind.WeightsClosed,
        EventKind.WeightsCopied,
        EventKind.SectorPriceSet,
    };

    static Game World(long seed = 201)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.SpecialistLines.MinEra = 0;
            c.DataSources.MinEra = 0;
            c.OpenWeights.MinEra = 0;
            c.SectorPricing.MinEra = 0;
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, string text = "mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 7000, text);
        e.Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind + " " + pick);
        return ticker[0].Text;
    }

    [Fact]
    public void Batch_fourteen_config_counts_and_validator_stay_clean_after_mid_save()
    {
        Game g = World();
        GameConfig c = TestData.Config();
        Assert.Equal(386, c.News.Headlines.Count);
        Assert.Equal(342, c.Chat.Lines.Count);
        foreach (EventKind kind in BatchFourteenKinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Empty(ConfigValidator.Problems(g.C));
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Specialist_headline_still_names_lab_after_schema_138_round_trip()
    {
        Game g = World(202);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 135; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            GameEvent e = Events.Add(sim.State, EventKind.SpecialistBranched, sim.Player.Id, -1, 7000, "mini");
            e.Pick = -1;
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Data_source_lapsed_headline_survives_load_and_weekly_step()
    {
        Game g = World(203);
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        string before = Headline(g, EventKind.DataSourceLapsed, -1);
        back.State.Events.Clear();
        GameEvent e = Events.Add(back.State, EventKind.DataSourceLapsed, back.Player.Id, -1, 7000, "mini");
        e.Pick = -1;
        List<Headline> ticker = NewsTicker.Headlines(g.C, back.State, 1);
        Assert.Equal(before.ToUpperInvariant(), ticker[0].Text.ToUpperInvariant());
    }
}