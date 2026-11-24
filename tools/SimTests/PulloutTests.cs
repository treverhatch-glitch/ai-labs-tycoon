using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Pulling a model out of a sector: a break fee, lost trust and share, and the load and exposure leave with it.</summary>
public class PulloutTests
{
    static Game Selling(Action<GameConfig> tweak = null, int weeks = 30)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 2, gpus: 8);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai", "game-bots"));
        g.Weeks(weeks);
        return g;
    }

    static ModelState Model(Game g) { return g.Me.Models.First(m => m.Status == ModelStatus.Released); }

    static SectorEntry Entry(Game g, string sector) { return g.State.Sector(sector).Entry(g.Id); }

    static List<GameEvent> Out(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.SectorPulledOut).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Pullout.Enabled);
        Assert.Equal(4, c.Pullout.FeeWeeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Pullout.Enabled = 2;
        c.Pullout.FeeWeeks = 53;
        c.Pullout.TrustDrop = 1001;
        c.Pullout.ReputationLoss = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "feeWeeks", "trustDrop", "reputationLoss" })
            Assert.Contains(problems, p => p.Contains("pullout." + key));
    }

    [Fact]
    public void Nothing_leaves_a_sector_by_itself()
    {
        Game g = Selling();
        Assert.NotNull(Entry(g, "game-ai"));
        Assert.NotNull(Entry(g, "game-bots"));
        g.Weeks(60);
        Assert.Equal(2, Model(g).Sectors.Count);
        Assert.Empty(Out(g));
    }

    [Fact]
    public void Leaving_one_sector_pays_the_break_fee_costs_trust_and_reputation_and_keeps_the_model_on_sale()
    {
        Game g = Selling();
        SectorEntry entry = Entry(g, "game-bots");
        Assert.True(entry.LastRevenue > 0);
        long fee = entry.LastRevenue * 4;
        long cash = g.Me.Cash;
        int trust = TrustSystem.Of(g.C, g.Me, "game-bots");
        int other = TrustSystem.Of(g.C, g.Me, "game-ai");
        int rep = g.Me.Reputation;
        g.Do(Command.PullOutOfSector(g.Id, Model(g).Id, "game-bots"));
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Null(Entry(g, "game-bots"));
        Assert.NotNull(Entry(g, "game-ai"));
        Assert.Equal(new[] { "game-ai" }, Model(g).Sectors.ToArray());
        Assert.Equal(ModelStatus.Released, Model(g).Status);
        Assert.Equal(trust - 100, TrustSystem.Of(g.C, g.Me, "game-bots"));
        Assert.Equal(other, TrustSystem.Of(g.C, g.Me, "game-ai"));
        Assert.Equal(rep - PressSystem.Damp(g.C, g.Me, 10), g.Me.Reputation);
        GameEvent e = Out(g).Single();
        Assert.Equal(fee, e.Amount);
        Assert.Equal(0, e.Pick);
        Assert.Contains("pulled out of", EventText.Describe(g.C, g.State, e));
        Assert.Contains("$" + fee, EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Leaving_the_last_sector_retires_the_model_and_the_revenue_stops()
    {
        Game g = Selling();
        ModelState model = Model(g);
        Assert.True(g.Me.LastWeek.Revenue > 0);
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-bots"));
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-ai"));
        Assert.Equal(ModelStatus.Retired, model.Status);
        Assert.Empty(model.Sectors);
        Assert.Equal(1, Out(g).Last().Pick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ModelRetired && e.SubjectId == model.Id);
        g.Weeks(2);
        Assert.Equal(0, g.Me.LastWeek.Revenue);
    }

    [Fact]
    public void What_the_model_earned_serves_and_is_fined_against_leaves_with_it()
    {
        Game g = Selling();
        ModelState model = Model(g);
        long revenue = g.Me.LastWeek.Revenue;
        long demand = g.Me.LastWeek.ServingDemand;
        long exposure = MarketSystem.ModelRevenue(g.State, g.Id, model.Id);
        Assert.True(demand > 0 && exposure > 0);
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-bots"));
        g.Weeks(2);
        Assert.True(g.Me.LastWeek.Revenue < revenue);
        Assert.True(g.Me.LastWeek.ServingDemand < demand);
        Assert.True(MarketSystem.ModelRevenue(g.State, g.Id, model.Id) < exposure);
    }

    [Fact]
    public void Going_back_in_starts_from_no_share_at_all()
    {
        Game g = Selling();
        int share = Entry(g, "game-bots").SharePpm;
        Assert.True(share > 0);
        g.Do(Command.PullOutOfSector(g.Id, Model(g).Id, "game-bots"));
        g.Do(Command.ReleaseModel(g.Id, Model(g).Id, "game-bots"));
        Assert.True(Entry(g, "game-bots").SharePpm < share);
    }

    [Fact]
    public void Refused_pullouts_leave_the_game_exactly_as_it_was()
    {
        Game g = Selling();
        ModelState model = Model(g);
        string d = g.Sim.Digest();
        g.Refused(Command.PullOutOfSector(g.Id, 9999, "game-ai"), CommandError.UnknownId);
        g.Refused(Command.PullOutOfSector(g.Id, model.Id, "nowhere"), CommandError.UnknownId);
        Command two = Command.PullOutOfSector(g.Id, model.Id, "game-ai");
        two.Ids.Add("game-bots");
        g.Refused(two, CommandError.InvalidArgument);
        Command none = Command.PullOutOfSector(g.Id, model.Id, "game-ai");
        none.Ids.Clear();
        g.Refused(none, CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-bots"));
        d = g.Sim.Digest();
        g.Refused(Command.PullOutOfSector(g.Id, model.Id, "game-bots"), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());

        Game poor = Selling();
        poor.Me.Cash = 100;
        d = poor.Sim.Digest();
        poor.Refused(Command.PullOutOfSector(poor.Id, Model(poor).Id, "game-ai"), CommandError.NotEnoughCash);
        Assert.Equal(d, poor.Sim.Digest());

        Game off = Selling(c => c.Pullout.Enabled = 0, 5);
        off.Refused(Command.PullOutOfSector(off.Id, Model(off).Id, "game-ai"), CommandError.InvalidArgument);

        Game fresh = Game.Solo(6).WithStarterLab(researchers: 2, engineers: 1, safety: 1, sales: 1, gpus: 6);
        ModelState trained = fresh.Train("mini", 300);
        fresh.UntilTrained(trained);
        fresh.Refused(Command.PullOutOfSector(fresh.Id, trained.Id, "game-ai"), CommandError.WrongModelStatus);

        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.PullOutOfSector(rival.Id, 1, "game-ai")).Ok);
    }

    [Fact]
    public void The_save_stays_sound_and_loads_the_same_after_a_pullout()
    {
        Game g = Selling();
        g.Do(Command.PullOutOfSector(g.Id, Model(g).Id, "game-bots"));
        g.Weeks(3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Selling(null, 10);
        g.Do(Command.PullOutOfSector(g.Id, Model(g).Id, "game-bots"));
        g.Weeks(8);
        g.Do(Command.ReleaseModel(g.Id, Model(g).Id, "game-bots"));
        g.Weeks(20);
        g.Do(Command.PullOutOfSector(g.Id, Model(g).Id, "game-ai"));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_of_leaving_and_coming_back_stays_sound_and_the_books_balance()
    {
        Game g = Selling(null, 5);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 300; i++)
        {
            g.Weeks(1);
            ModelState m = Model(g);
            if (i % 30 == 10) g.Try(Command.PullOutOfSector(g.Id, m.Id, "game-bots"));
            if (i % 30 == 20) g.Try(Command.ReleaseModel(g.Id, m.Id, "game-bots"));
        }
        Assert.True(Out(g).Count >= 5);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
