using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Protest headlines add home-bloc attention (Q50).</summary>
public class ProtestWorldEventTests
{
    static WorldEventConfig Protest(GameConfig c, string id) => c.WorldEvent(id);

  static void FireHeadline(Game g, string eventId)
    {
        g.State.Tick = EraSystem.EventTick(g.C, g.C.WorldEvent(eventId)) - 1;
        g.Weeks(2);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == eventId);
    }

    [Fact]
    public void Shipped_teen_chatbot_event_carries_protest_attention()
    {
        WorldEventConfig e = Protest(TestData.Config(), "teen-chatbot");
        Assert.Equal(35, e.ProtestAttentionPermille);
        Assert.True(string.IsNullOrEmpty(e.ProtestBlocId));
    }

    [Fact]
    public void Shipped_pause_letter_event_carries_protest_attention()
    {
        WorldEventConfig e = Protest(TestData.Config(), "pause-letter");
        Assert.Equal(45, e.ProtestAttentionPermille);
    }

    [Fact]
    public void Protest_system_bumps_home_attention_on_event_day()
    {
        Game g = Game.New(9);
        g.State.Tick = EraSystem.EventTick(g.C, g.C.WorldEvent("teen-chatbot")) - 1;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Weeks(2);
        int after = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Assert.True(after >= before + 35);
    }

    [Fact]
    public void Protest_fires_attention_rose_for_player_lab()
    {
        Game g = Game.New(11);
        FireHeadline(g, "pause-letter");
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AttentionRose && e.LabId == g.Id && e.Amount == 45);
    }

    [Fact]
    public void Protest_hits_every_lab_home_bloc_when_bloc_blank()
    {
        Game g = Game.New(2);
        FireHeadline(g, "teen-chatbot");
        int rose = 0;
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Bankrupt) continue;
            if (lab.Controller == LabController.Plan && g.Tick < lab.PlanStartTick) continue;
            Assert.Contains(g.State.Events,
                e => e.Kind == EventKind.AttentionRose && e.LabId == lab.Id && e.Amount == 35 && e.Ref == lab.HomeBlocId);
            rose++;
        }
        Assert.True(rose >= 4);
    }

    [Fact]
    public void Named_bloc_protest_only_touches_that_bloc()
    {
        Game g = Game.Solo();
        var e = new WorldEventConfig
        {
            IdValue = "test-protest",
            Year = GameDate.Year(g.C.Calendar, g.Tick),
            Month = GameDate.Month(g.C.Calendar, g.Tick) % g.C.Calendar.MonthsPerYear + 1,
            Headline = "test",
            ProtestAttentionPermille = 50,
            ProtestBlocId = "eu"
        };
        int euBefore = g.Me.Bloc("eu").Attention;
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.Equal(euBefore + 50, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Zero_protest_permille_is_no_op()
    {
        Game g = Game.Solo();
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        var e = Protest(g.C, "garage-search");
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.Equal(before, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void World_news_still_fires_for_protest_headline_week()
    {
        Game g = Game.New(4);
        FireHeadline(g, "teen-chatbot");
    }

    [Fact]
    public void Protest_does_not_move_chip_shortage_clock()
    {
        Game g = Game.New();
        int tick = EraSystem.EventTick(g.C, g.C.WorldEvent("pause-letter"));
        while (g.Tick < tick) g.Sim.Step();
        Assert.False(ChipShortageSystem.Active(g.State));
    }

    [Fact]
    public void Config_validator_rejects_unknown_protest_bloc()
    {
        var c = TestData.Config();
        c.WorldEvents.Add(new WorldEventConfig
        {
            IdValue = "bad-protest",
            Year = 2030,
            Month = 1,
            Headline = "bad",
            ProtestAttentionPermille = 10,
            ProtestBlocId = "narnia"
        });
        Assert.Contains(ConfigValidator.Problems(c), s => s.Contains("bad-protest") && s.Contains("narnia"));
    }

    [Fact]
    public void Attention_capped_at_permille()
    {
        Game g = Game.Solo();
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Attention = 990;
        var e = Protest(g.C, "pause-letter");
        ProtestSystem.OnWorldEvent(g.C, g.State, e);
        Assert.Equal(1000, home.Attention);
    }

    [Fact]
    public void Bankrupt_lab_skipped_for_protest()
    {
        Game g = Game.Solo();
        g.Me.Bankrupt = true;
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ProtestSystem.OnWorldEvent(g.C, g.State, Protest(g.C, "pause-letter"));
        Assert.Equal(before, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }
}