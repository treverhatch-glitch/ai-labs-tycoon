using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Schema 178–180 edge paths (Q511–Q513).</summary>
public class BatchTwentySixEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(140, c =>
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
            c.HearingShareEraDampen.Enabled = 1;
            c.HearingShareEraDampen.MinEra = 0;
            c.HearingShareEraDampen.DampenPermille = 100;
            c.HearingSharePrice.MinFloatPpm = 1;
        });
        g.Me.PublicFloatPpm = 50_000;
        Assert.True(HearingShareEraDampen.DampenPermille(g.C, g.State, -5) > 0);
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.HearingShareEraDampen.Enabled = 0;
            c.OfficeReleaseEraGrace.Enabled = 0;
            c.DataSourceAcquireEraWeeks.Enabled = 0;
        });
        Assert.Equal(0, HearingShareEraDampen.DampenPermille(g.C, g.State, -1));
        Assert.Equal(g.C.Office.ExpireWeeks, OfficeReleaseEraGrace.AnswerWeeks(g.C, g.State));
        Assert.Equal(0, DataSourceAcquireEraWeeks.BonusWeeks(g.C, g.State));
    }

    [Fact]
    public void Max_dampen_caps_bubble_cut()
    {
        Game g = Lab(c =>
        {
            c.HearingShareEraDampen.Enabled = 1;
            c.HearingShareEraDampen.MinEra = 0;
            c.HearingShareEraDampen.DampenPermille = 1000;
        });
        int raw = g.C.HearingSharePrice.DownPermille;
        int applied = HearingShareEraDampen.AppliedCut(raw, 1000);
        Assert.Equal(0, applied);
    }

    [Fact]
    public void Max_bonus_weeks_on_acquire()
    {
        Game g = Lab(c =>
        {
            c.DataSourceAcquireEraWeeks.Enabled = 1;
            c.DataSourceAcquireEraWeeks.MinEra = 0;
            c.DataSourceAcquireEraWeeks.BonusWeeks = 52;
        });
        Assert.Equal(52, DataSourceAcquireEraWeeks.BonusWeeks(g.C, g.State));
    }

    [Fact]
    public void Bankrupt_lab_skips_player_acquire()
    {
        Game g = Lab(c =>
        {
            c.DataSourceAcquireEraWeeks.Enabled = 1;
            c.DataSourceAcquireEraWeeks.MinEra = 0;
            c.DataSourceAcquireEraWeeks.BonusWeeks = 8;
        });
        g.Me.Bankrupt = true;
        g.Refused(Command.AcquireDataSource(g.Id, "licensed-corpus"), CommandError.LabBankrupt);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.DataSourceAcquireEraWeeksBonused);
    }

    [Fact]
    public void Save_load_preserves_schema_183round_trip()
    {
        Game g = Lab(c =>
        {
            c.DataSourceAcquireEraWeeks.Enabled = 1;
            c.DataSourceAcquireEraWeeks.MinEra = 0;
            c.DataSourceAcquireEraWeeks.BonusWeeks = 4;
        });
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        string save = g.Sim.Save();
        Assert.Equal(197, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(197, loaded.State.Schema);
        Assert.Contains(loaded.State.Events, e => e.Kind == EventKind.DataSourceAcquireEraWeeksBonused);
    }

    [Fact]
    public void Save_pretend_176_migrates_to_current_schema()
    {
        Game g = Lab();
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 177, g.Sim.Save()));
        Assert.Equal(GameState.CurrentSchema, older.State.Schema);
    }
}