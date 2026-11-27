using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-sixteen headlines and chat survive saves (Q473).</summary>
public class EraPBatchSixteenNewsChatHardeningTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.MarginCall,
        EventKind.MarginCallSatisfied,
        EventKind.HearingShareNudged,
        EventKind.ScenarioGoalMet,
        EventKind.ScenarioGoalMissed,
        EventKind.LawTargetSet,
    };

    [Fact]
    public void Margin_call_headline_after_schema_144_round_trip()
    {
        Game g = Game.Solo(152, c => { c.MarginCall.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab(gpus: 4);
        g.Me.Cash = 500_000_000;
        Events.Add(g.State, EventKind.MarginCall, g.Id, -1, 0, "");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 144; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.MarginCall, sim.Player.Id, -1, 0, "");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Batch_sixteen_counts_survive_mid_save()
    {
        Game g = Game.Solo(150, c => c.Office.ChancePermille = 0).WithStarterLab();
        GameConfig c = TestData.Config();
        Assert.Equal(424, c.News.Headlines.Count);
        Assert.Equal(377, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            if (kind != EventKind.MarginCallDefaulted)
                Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Empty(ConfigValidator.Problems(g.C));
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Margin_call_satisfied_chat_after_schema_144_round_trip()
    {
        Game g = Game.Solo(155, c => { c.MarginCall.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab(researchers: 2);
        Events.Add(g.State, EventKind.MarginCallSatisfied, g.Id, -1, 0, "");
        string mid = g.Sim.Save();
        for (int schema = 144; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.MarginCallSatisfied, sim.Player.Id, -1, 0, "");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.MarginCallSatisfied);
        }
    }

    [Fact]
    public void Scenario_goal_met_chat_after_schema_147_round_trip()
    {
        Game g = Game.Solo(156, c => { c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioGoalMet, g.Id, -1, 0, "ten-staff");
        string mid = g.Sim.Save();
        for (int schema = 147; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioGoalMet, sim.Player.Id, -1, 0, "ten-staff");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.ScenarioGoalMet);
        }
    }

    [Fact]
    public void Scenario_goal_missed_chat_after_schema_147_round_trip()
    {
        Game g = Game.Solo(157, c => { c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioGoalMissed, g.Id, -1, 0, "ten-staff");
        string mid = g.Sim.Save();
        for (int schema = 147; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioGoalMissed, sim.Player.Id, -1, 0, "ten-staff");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.ScenarioGoalMissed);
        }
    }

    [Fact]
    public void Scenario_goal_missed_headline_after_schema_147_round_trip()
    {
        Game g = Game.Solo(158, c => { c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioGoalMissed, g.Id, -1, 0, "ten-staff");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 147; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioGoalMissed, sim.Player.Id, -1, 0, "ten-staff");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Scenario_goal_met_headline_after_schema_147_round_trip()
    {
        Game g = Game.Solo(159, c => { c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioGoalMet, g.Id, -1, 0, "ten-staff");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 147; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioGoalMet, sim.Player.Id, -1, 0, "ten-staff");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Law_target_set_headline_after_schema_148_round_trip()
    {
        Game g = Game.Solo(160, c => c.Office.ChancePermille = 0).WithStarterLab();
        Events.Add(g.State, EventKind.LawTargetSet, g.Id, -1, 0, LawShaping.Strict);
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 148; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.LawTargetSet, sim.Player.Id, -1, 0, LawShaping.Strict);
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Law_target_set_chat_after_schema_148_round_trip()
    {
        Game g = Game.Solo(154).WithStarterLab(researchers: 2);
        Events.Add(g.State, EventKind.LawTargetSet, g.Id, -1, 0, LawShaping.Strict);
        string mid = g.Sim.Save();
        for (int schema = 148; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.LawTargetSet, sim.Player.Id, -1, 0, LawShaping.Loose);
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.LawTargetSet);
        }
    }

    [Fact]
    public void Hearing_share_chat_after_schema_145_round_trip()
    {
        Game g = Game.Solo(153, c => { c.Office.ChancePermille = 0; c.HearingSharePrice.MinFloatPpm = 1; }).WithStarterLab(researchers: 2);
        Events.Add(g.State, EventKind.HearingShareNudged, g.Id, 0, 0, "hear");
        string mid = g.Sim.Save();
        for (int schema = 145; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.HearingShareNudged, sim.Player.Id, 0, 0, "hear");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.HearingShareNudged);
        }
    }

    [Fact]
    public void Hearing_share_headline_after_schema_145_round_trip()
    {
        Game g = Game.Solo(151, c => { c.Office.ChancePermille = 0; c.HearingSharePrice.MinFloatPpm = 1; }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        string choice = q.Choices.First(ch => ch.Effects.Any(f => f.Kind == OfficeEffectKind.Reputation && f.Amount > 0)).IdValue;
        g.Do(Command.AnswerOfficeEvent(g.Id, choice));
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 145; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.HearingShareNudged, sim.Player.Id, 0, 0, "hear");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Margin_call_defaulted_headline_after_schema_144_round_trip()
    {
        Game g = Game.Solo(161, c => { c.MarginCall.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.MarginCallDefaulted, g.Id, -1, 0, "");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 144; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.MarginCallDefaulted, sim.Player.Id, -1, 0, "");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Campaign_deadline_hit_headline_after_schema_139_round_trip()
    {
        Game g = Game.Solo(162, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.CampaignDeadlineHit, g.Id, -1, 0, "");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.CampaignDeadlineHit, sim.Player.Id, -1, 0, "");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Campaign_deadline_hit_chat_after_schema_139_round_trip()
    {
        Game g = Game.Solo(163, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.CampaignDeadlineHit, g.Id, -1, 0, "");
        string mid = g.Sim.Save();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.CampaignDeadlineHit, sim.Player.Id, -1, 0, "");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.CampaignDeadlineHit);
        }
    }

    [Fact]
    public void Law_shaped_chat_after_schema_140_round_trip()
    {
        Game g = Game.Solo(164, c => { c.LawShaping.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.LawShaped, g.Id, -1, 0, LawShaping.Loose);
        string mid = g.Sim.Save();
        for (int schema = 140; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.LawShaped, sim.Player.Id, -1, 0, LawShaping.Loose);
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.LawShaped);
        }
    }

    [Fact]
    public void Law_shaped_headline_after_schema_140_round_trip()
    {
        Game g = Game.Solo(165, c => { c.LawShaping.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.LawShaped, g.Id, -1, 0, LawShaping.Strict);
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 140; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.LawShaped, sim.Player.Id, -1, 0, LawShaping.Strict);
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Campaign_deadline_set_chat_after_schema_139_round_trip()
    {
        Game g = Game.Solo(166, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.CampaignDeadlineSet, g.Id, -1, 0, "48");
        string mid = g.Sim.Save();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.CampaignDeadlineSet, sim.Player.Id, -1, 0, "48");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.CampaignDeadlineSet);
        }
    }

    [Fact]
    public void Scenario_baggage_picked_chat_after_schema_143_round_trip()
    {
        Game g = Game.Solo(167, c => { c.ScenarioBaggage.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioBaggagePicked, g.Id, -1, 0, "early-lawsuit");
        string mid = g.Sim.Save();
        for (int schema = 143; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioBaggagePicked, sim.Player.Id, -1, 0, "early-lawsuit");
            List<ChatMessage> feed = CompanyChat.Feed(g.C, sim.State, sim.Player, 5);
            Assert.Contains(feed, m => m.Kind == EventKind.ScenarioBaggagePicked);
        }
    }

    [Fact]
    public void Campaign_deadline_set_headline_after_schema_139_round_trip()
    {
        Game g = Game.Solo(168, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.CampaignDeadlineSet, g.Id, -1, 0, "48");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 139; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.CampaignDeadlineSet, sim.Player.Id, -1, 0, "48");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }

    [Fact]
    public void Scenario_baggage_picked_headline_after_schema_143_round_trip()
    {
        Game g = Game.Solo(169, c => { c.ScenarioBaggage.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab();
        Events.Add(g.State, EventKind.ScenarioBaggagePicked, g.Id, -1, 0, "early-lawsuit");
        string mid = g.Sim.Save();
        string lab = g.Me.Name.ToUpperInvariant();
        for (int schema = 143; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, mid);
            Simulation sim = Simulation.Load(g.C, old);
            sim.State.Events.Clear();
            Events.Add(sim.State, EventKind.ScenarioBaggagePicked, sim.Player.Id, -1, 0, "early-lawsuit");
            List<Headline> ticker = NewsTicker.Headlines(g.C, sim.State, 1);
            Assert.StartsWith(lab, ticker[0].Text.ToUpperInvariant());
        }
    }
}