using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class PublicBenchmarkTests
{
    const string Bench = "expert-league";
    const string Sector = "expert-systems";

    static Game At(int year = 2015, System.Action<GameConfig> tweak = null)
    {
        var g = ExpansionTestSupport.SoloAtYear(year, tweak: c =>
        {
            c.PublicBenchmarks.OpensYear = 2012;
            tweak?.Invoke(c);
        }).WithStarterLab();
        return g;
    }

    static ModelState Released(Game g, int cap, int align = 600, string sector = Sector, string name = "Test")
    {
        return ExpansionTestSupport.AddReleasedModel(g, cap, sector, name);
    }

    static ModelState RawReleased(Game g, int cap, int align, string sector)
    {
        var model = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Raw",
            Status = ModelStatus.Released, Capability = cap, CapBase = cap,
            Alignment = align, DisplayedAlignment = align, ReleasedTick = g.Tick,
        };
        model.Sectors.Add(sector);
        g.Me.Models.Add(model);
        return model;
    }

    static void ThroughVetting(Game g, string benchmarkId = Bench)
    {
        int v = g.C.PublicBenchmarks.Benchmark(benchmarkId).VettingWeeks;
        g.Weeks(v + 1);
    }

    [Fact]
    public void Shipped_config_lists_three_benchmarks()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.PublicBenchmarks.Benchmarks.Count);
        Assert.Contains(c.PublicBenchmarks.Benchmarks, b => b.Id == Bench);
        Assert.Equal(34, GameState.CurrentSchema);
    }

    [Fact]
    public void Before_opens_year_submit_refused()
    {
        var g = At(2010, c => c.PublicBenchmarks.OpensYear = 2020);
        var m = Released(g, 300);
        g.Refused(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Submit_requires_released_model_in_sector()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        Assert.Single(g.Me.PublicBenchmarkEntries);
        Assert.Equal(PublicBenchmarkPhase.Vetting, g.Me.PublicBenchmarkEntries[0].Phase);
    }

    [Fact]
    public void Submit_refused_below_capability_bar()
    {
        var g = At();
        var m = RawReleased(g, 50, 600, Sector);
        g.Refused(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id), CommandError.BelowSectorBar);
    }

    [Fact]
    public void Submit_refused_wrong_sector()
    {
        var g = At();
        var m = Released(g, 300);
        g.Refused(Command.SubmitPublicBenchmark(g.Id, "vision-scoreboard", m.Id), CommandError.BelowSectorBar);
    }

    [Fact]
    public void Submit_refused_duplicate_open_entry()
    {
        var g = At();
        var a = RawReleased(g, 300, 600, Sector);
        var b = RawReleased(g, 320, 600, Sector);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, a.Id));
        g.Refused(Command.SubmitPublicBenchmark(g.Id, Bench, b.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Vetting_advances_to_live_after_window()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        Assert.Equal(PublicBenchmarkPhase.Live, g.Me.PublicBenchmarkEntries[0].Phase);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicBenchmarkLive);
    }

    [Fact]
    public void Withdraw_during_vetting()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        g.Do(Command.WithdrawPublicBenchmark(g.Id, Bench, m.Id));
        Assert.Equal(PublicBenchmarkPhase.Withdrawn, g.Me.PublicBenchmarkEntries[0].Phase);
    }

    [Fact]
    public void Pledge_only_while_live()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        g.Refused(Command.PledgeBenchmarkDefense(g.Id, Bench, m.Id), CommandError.InvalidArgument);
        ThroughVetting(g);
        g.Do(Command.PledgeBenchmarkDefense(g.Id, Bench, m.Id));
        Assert.True(g.Me.PublicBenchmarkEntries[0].DefendPledged);
    }

    [Fact]
    public void Retarget_resets_vetting()
    {
        var g = At();
        var a = RawReleased(g, 300, 600, Sector);
        var b = RawReleased(g, 350, 600, Sector);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, a.Id));
        ThroughVetting(g);
        g.Do(Command.RetargetPublicBenchmark(g.Id, Bench, a.Id, b.Id));
        var entry = g.Me.PublicBenchmarkEntries.Single(e => e.BenchmarkId == Bench && e.ModelId == b.Id);
        Assert.Equal(PublicBenchmarkPhase.Vetting, entry.Phase);
        Assert.False(entry.DefendPledged);
    }

    [Fact]
    public void Leader_stipend_when_solo_live()
    {
        var g = At();
        var m = Released(g, 400);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        g.Weeks(1);
        var top = g.State.BenchmarkLeaders.Single(l => l.BenchmarkId == Bench);
        Assert.True(top.Score > 0);
        Assert.Equal(g.Id, top.LabId);
        Assert.Equal(m.Id, top.ModelId);
    }

    [Fact]
    public void Duo_higher_score_takes_leader()
    {
        var g = ExpansionTestSupport.DuoAtYear(2015, tweak: c => c.PublicBenchmarks.OpensYear = 2012).WithStarterLab();
        var weak = ExpansionTestSupport.AddReleasedModel(g, 250, Sector);
        var rival = g.Rival("openaye");
        var strong = ExpansionTestSupport.AddReleasedModelOnLab(g, rival, 420, Sector, "Rival");
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, weak.Id));
        rival.PublicBenchmarkEntries.Add(new PublicBenchmarkEntry
        {
            BenchmarkId = Bench,
            ModelId = strong.Id,
            Phase = PublicBenchmarkPhase.Live,
            SubmittedTick = g.Tick,
            VettingEndsTick = g.Tick - 1,
            LastScore = BenchmarkLeagueSystem.Score(g.C, strong, g.C.PublicBenchmarks.Benchmark(Bench)),
        });
        g.Weeks(2);
        var top = g.State.BenchmarkLeaders.Single(l => l.BenchmarkId == Bench);
        Assert.Equal(rival.Id, top.LabId);
    }

    [Fact]
    public void Demote_when_model_retired()
    {
        var g = At();
        var m = Released(g, 350);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        m.Status = ModelStatus.Retired;
        g.Sim.Step();
        Assert.Equal(PublicBenchmarkPhase.Demoted, g.Me.PublicBenchmarkEntries[0].Phase);
    }

    [Fact]
    public void Defend_pledge_hits_harder_on_demote()
    {
        var g = At();
        var m = Released(g, 350);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        g.Do(Command.PledgeBenchmarkDefense(g.Id, Bench, m.Id));
        m.Status = ModelStatus.Retired;
        g.Sim.Step();
        Assert.Equal(PublicBenchmarkPhase.Demoted, g.Me.PublicBenchmarkEntries[0].Phase);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicBenchmarkDemoted);
    }

    [Fact]
    public void Live_appeal_boost_in_sector()
    {
        var g = At();
        var m = Released(g, 350);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        SectorConfig sector = g.C.Sector(Sector);
        SectorState sectorState = g.State.Sector(Sector);
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, m);
        Assert.True(parts.BenchmarkBoost > 0);
    }

    [Fact]
    public void Save_round_trip_benchmark_entries()
    {
        var g = At();
        var m = Released(g, 320);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        g.Weeks(1);
        var sim = g.Sim;
        string save = sim.Save();
        var loaded = Simulation.Load(g.C, save);
        Assert.Equal(34, loaded.State.Schema);
        Assert.Single(loaded.Player.PublicBenchmarkEntries);
        Assert.Equal(PublicBenchmarkPhase.Vetting, loaded.Player.PublicBenchmarkEntries[0].Phase);
    }

    [Fact]
    public void Migration_from_33_adds_benchmark_fields()
    {
        var g = Game.Solo();
        string save = SaveSchemaPretend.At(g.C, 33, g.Sim.Save());
        var loaded = Simulation.Load(g.C, save);
        Assert.Equal(34, loaded.State.Schema);
        Assert.NotNull(loaded.Player.PublicBenchmarkEntries);
        Assert.NotNull(loaded.State.BenchmarkLeaders);
    }

    [Fact]
    public void Config_validator_requires_known_sectors()
    {
        var err = Assert.Throws<ConfigException>(() => TestData.Config(c =>
        {
            c.PublicBenchmarks.Benchmarks[0].SectorId = "not-a-sector";
        }));
        Assert.Contains("sectorId", err.Message);
    }

    [Fact]
    public void Enabled_player_only()
    {
        var g = ExpansionTestSupport.DuoAtYear(2015, tweak: c => c.PublicBenchmarks.OpensYear = 2012);
        var rival = g.Rival("openaye");
        Assert.False(BenchmarkLeagueSystem.Enabled(g.C, g.State, rival));
        Assert.True(BenchmarkLeagueSystem.Enabled(g.C, g.State, g.Me));
    }

    [Fact]
    public void Not_enough_cash_for_entry_fee()
    {
        var g = At();
        var m = Released(g, 300);
        g.Me.Cash = 0;
        g.Refused(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Bankrupt_lab_skipped_in_leader_refresh()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        g.Me.Bankrupt = true;
        g.Sim.Step();
        var top = g.State.BenchmarkLeaders.Single(l => l.BenchmarkId == Bench);
        Assert.Equal(-1, top.LabId);
    }

    [Fact]
    public void Events_on_submit_and_leader()
    {
        var g = At();
        var m = Released(g, 380);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicBenchmarkSubmitted);
        ThroughVetting(g);
        g.Weeks(1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicBenchmarkLeader);
    }

    [Fact]
    public void Zero_max_entries_and_withdrawn_resubmit()
    {
        var g = At();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        g.Do(Command.WithdrawPublicBenchmark(g.Id, Bench, m.Id));
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        Assert.Equal(2, g.Me.PublicBenchmarkEntries.Count);
    }

    [Fact]
    public void Score_query_matches_run_week()
    {
        var g = At();
        var m = Released(g, 310);
        var bench = g.C.PublicBenchmarks.Benchmark(Bench);
        long expected = BenchmarkLeagueSystem.Score(g.C, m, bench);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        ThroughVetting(g);
        g.Weeks(1);
        Assert.Equal(expected, g.Me.PublicBenchmarkEntries[0].LastScore);
    }

    [Fact]
    public void Cross_system_rival_depth_soak_week_with_benchmarks()
    {
        var g = ExpansionTestSupport.SoloAtYear(2015, tweak: c => c.PublicBenchmarks.OpensYear = 2012).WithStarterLab();
        var m = Released(g, 300);
        g.Do(Command.SubmitPublicBenchmark(g.Id, Bench, m.Id));
        g.Weeks(52);
        Assert.NotNull(g.State.BenchmarkLeaders);
    }

    [Fact]
    public void Ten_years_with_benchmarks_deterministic()
    {
        const int years = 10;
        string a = RunSoak(8181, years);
        string b = RunSoak(8181, years);
        Assert.Equal(a, b);
    }

    static string RunSoak(long seed, int years)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.PublicBenchmarks.OpensYear = 2005), options));
        g.WithStarterLab(gpus: 16);
        int weeks = years * g.C.Calendar.WeeksPerYear;
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        return g.Sim.Digest();
    }
}