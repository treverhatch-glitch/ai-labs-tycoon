using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class PublicMetricsTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(11, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PublicMetrics.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Sell(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    [Fact]
    public void Models_do_not_publish_by_default()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        Assert.Equal(0, m.PubOn);
    }

    [Fact]
    public void Publish_metrics_is_checked()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Refused(Command.PublishMetrics(g.Id, 999), CommandError.UnknownId);
        long cash = g.Me.Cash;
        g.Me.LastWeek.ServedPermille = 800;
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Assert.Equal(cash - g.C.PublicMetrics.PublishCost, g.Me.Cash);
        Assert.Equal(1, m.PubOn);
        g.Refused(Command.PublishMetrics(g.Id, m.Id), CommandError.InvalidArgument);
        Game poor = Lab();
        ModelState pm = Sell(poor);
        poor.Me.Cash = g.C.PublicMetrics.PublishCost - 1;
        poor.Refused(Command.PublishMetrics(poor.Id, pm.Id), CommandError.NotEnoughCash);
        Game off = Lab(c => c.PublicMetrics.Enabled = 0);
        off.Refused(Command.PublishMetrics(off.Id, Sell(off).Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Stop_metrics_turns_publishing_off()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Do(Command.StopMetrics(g.Id, m.Id));
        Assert.Equal(0, m.PubOn);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MetricsStopped);
    }

    [Fact]
    public void Run_week_updates_score_from_served_permille()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Me.LastWeek.ServedPermille = 600;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(600, m.PubScore);
        g.Me.LastWeek.ServedPermille = 900;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(900, m.PubScore);
    }

    [Fact]
    public void Incidents_mark_a_miss_on_published_models()
    {
        Game g = Lab(c => c.PublicMetrics.MissTrust = -5);
        ModelState m = Sell(g);
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        PublicMetrics.OnIncident(g.C, g.State, g.Me, m);
        Assert.Equal(1, m.PubMiss);
    }

    [Fact]
    public void Bad_scores_push_conformity_review()
    {
        Game g = Lab(c => { c.PublicMetrics.Bar = 900; c.PublicMetrics.ConformityPush = 3; });
        ModelState m = Sell(g);
        m.CertStage = Conformity.Review;
        m.CertLeft = 4;
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Me.LastWeek.ServedPermille = 400;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(7, m.CertLeft);
    }

    [Fact]
    public void Unreleased_models_drop_publishing()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Do(Command.PullOutOfSector(g.Id, m.Id, "game-ai"));
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, m.PubOn);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_pub_fields()
    {
        Game g = Lab();
        ModelState m = Sell(g);
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 133, g.Sim.Save()));
        Assert.All(older.Player.Models, x => Assert.Equal(0, x.PubOn + x.PubScore + x.PubMiss));
    }
}