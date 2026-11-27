using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q514 combination wiring for batch twenty-six systems.</summary>
public class BatchTwentySixTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(130, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Hearing_down_answer_fires_era_dampen()
    {
        Game g = Lab(c =>
        {
            c.HearingSharePrice.MinFloatPpm = 1;
            c.HearingShareEraDampen.Enabled = 1;
            c.HearingShareEraDampen.MinEra = 0;
            c.HearingShareEraDampen.DampenPermille = 250;
        });
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        g.Me.OfficeEventId = "hearing-weapon";
        g.Me.OfficeEventExpires = g.Tick + g.C.Office.ExpireWeeks;
        int bubble = g.State.BubblePermille;
        const string choice = "hypothetical";
        g.Do(Command.AnswerOfficeEvent(g.Id, choice));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HearingShareEraDampened);
        int saved = g.State.Events.Last(e => e.Kind == EventKind.HearingShareEraDampened).SubjectId;
        Assert.True(saved > 0);
        Assert.True(g.State.BubblePermille > bubble - g.C.HearingSharePrice.DownPermille);
    }

    [Fact]
    public void Release_notes_gets_era_grace()
    {
        Game g = Lab(c =>
        {
            c.OfficeReleaseEraGrace.Enabled = 1;
            c.OfficeReleaseEraGrace.MinEra = 0;
            c.OfficeReleaseEraGrace.GraceWeeks = 6;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OfficeReleaseEraGraced);
        Assert.Equal(g.Tick + g.C.Office.ExpireWeeks + 6, g.Me.OfficeEventExpires);
    }

    [Fact]
    public void Data_source_acquire_bonus_weeks()
    {
        Game g = Lab(c =>
        {
            c.DataSourceAcquireEraWeeks.Enabled = 1;
            c.DataSourceAcquireEraWeeks.MinEra = 0;
            c.DataSourceAcquireEraWeeks.BonusWeeks = 8;
        });
        string sid = "licensed-corpus";
        int baseWeeks = g.C.DataSources.Sources.Find(s => s.Id == sid).DurationWeeks;
        g.Do(Command.AcquireDataSource(g.Id, sid));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSourceAcquireEraWeeksBonused);
        GameEvent acq = g.State.Events.Last(e => e.Kind == EventKind.DataSourceAcquired);
        Assert.Equal(baseWeeks + 8, acq.Amount);
    }
}