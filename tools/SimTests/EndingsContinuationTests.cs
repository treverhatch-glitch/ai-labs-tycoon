using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Endings that continue: bailout, exit, monopoly, frontier, museum, ousted, and config stubs.</summary>
public class EndingsContinuationTests
{
    [Fact]
    public void Bailout_sets_reputation_from_config_and_unbankrupts()
    {
        var g = Game.Solo(seed: 3);
        g.Me.GrantPerWeek = 0;
        g.Me.Reputation = 900;
        g.Me.Cash = -10_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "bailout"));
        Assert.False(g.Me.Bankrupt);
        Assert.Equal(g.C.Endings.BailoutReputation, g.Me.Reputation);
    }

    [Fact]
    public void Ousted_records_once_and_the_lab_keeps_planning()
    {
        var g = Game.Solo(tweak: c =>
        {
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            c.Funding.Round("series-b").DilutionPermille = 400;
            c.Funding.ControlPermille = 500;
        });
        string leader = g.Me.Leader;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        g.Do(Command.RaiseFunding(g.Id, "series-b"));
        Assert.Single(g.State.Endings, e => e.EndingId == "ousted" && e.Choice == "done");
        Assert.NotEqual(leader, g.Me.Leader);
        Assert.False(g.Me.Bankrupt);
        int tick = g.Tick;
        g.Weeks(24);
        Assert.True(g.Tick > tick);
    }

    [Fact]
    public void SellLab_found_again_replays_to_the_same_digest()
    {
        var g = Game.Solo(seed: 9, tweak: c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 1_500_000;
        g.Me.Cash = 80_000;
        g.Do(Command.SellLab(g.Id, "found-again"));
        string save = g.Sim.Save();
        var loaded = Simulation.Load(g.C, save);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void SellLab_spectate_leaves_the_world_running_for_ten_years()
    {
        var g = Game.New(seed: 12);
        int rivals = g.State.Labs.Count;
        g.Me.Ledger.ThisMonth.Revenue = 5_000_000;
        g.Do(Command.SellLab(g.Id, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        int tick = g.Tick;
        g.Years(10);
        Assert.Equal(tick + 10 * g.C.Calendar.WeeksPerYear, g.Tick);
        Assert.Equal(rivals, g.State.Labs.Count);
        Assert.Contains(g.State.Endings, e => e.EndingId == "exit");
    }

    [Fact]
    public void Stub_endings_are_in_config_for_later_triggers()
    {
        GameConfig c = TestData.Config();
        string[] stubs = { "treaty", "nationalised" };
        foreach (string id in stubs)
        {
            EndingDefConfig def = c.Endings.Def(id);
            Assert.NotNull(def);
            Assert.False(string.IsNullOrEmpty(def.Name));
            Assert.Empty(def.ContinueModes);
        }
    }

    [Fact]
    public void Frontier_stays_locked_until_every_open_tech_is_owned()
    {
        var g = Game.Solo();
        g.Me.ResearchPoints = 1_000_000;
        string open = null;
        foreach (TechConfig tech in g.C.Research.Techs)
            if (EraSystem.TechOpen(g.C, tech, g.Tick)) { open = tech.Id; break; }
        Assert.NotNull(open);
        g.Me.Techs.Remove(open);
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.FrontierUnavailable);
        g.Research(open);
        if (EndingSystem.AllOpenTechsDone(g.C, g.Me, g.Tick)) g.Do(Command.ResearchFrontier(g.Id));
        else g.ResearchAllReachable();
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(1, g.Me.FrontierLevel);
    }

    [Fact]
    public void Sell_price_counts_revenue_fleet_and_cash()
    {
        var g = Game.Solo(tweak: c => c.Endings.ExitRevenueYears = 2);
        g.Me.Ledger.ThisMonth.Revenue = 100_000;
        g.Me.Cash = 50_000;
        long price = EndingSystem.SellPrice(g.C, g.State, g.Me);
        Assert.True(price > 50_000);
        Assert.True(price >= 2 * 100_000 + 50_000);
    }

    [Fact]
    public void Monopoly_compute_share_uses_last_week_across_all_labs()
    {
        var g = Game.New(seed: 4);
        g.Me.LastWeek.Compute = 100;
        for (int i = 1; i < g.State.Labs.Count; i++) g.State.Labs[i].LastWeek.Compute = 9000;
        Assert.True(EndingSystem.ComputeSharePpm(g.State, g.Me) < 100_000);
        g.Me.LastWeek.Compute = 90_000;
        for (int i = 1; i < g.State.Labs.Count; i++) g.State.Labs[i].LastWeek.Compute = 100;
        Assert.True(EndingSystem.ComputeSharePpm(g.State, g.Me) >= 800_000);
    }

    [Fact]
    public void After_sell_the_successor_plan_can_still_hire()
    {
        var g = Game.Solo(seed: 6, tweak: c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 3_000_000;
        g.Do(Command.SellLab(g.Id, "found-again"));
        LabState sold = g.State.Labs.First(l => l.RivalId == "successor");
        sold.PlanStartTick = 0;
        sold.Cash = 5_000_000;
        int staffBefore = LabQueries.TotalStaff(sold);
        g.Sim.RunPlan(sold.Id, g.C.Rival("successor").Plan);
        Assert.True(LabQueries.TotalStaff(sold) >= staffBefore);
    }

    [Fact]
    public void Museum_prunes_retired_but_never_released_models()
    {
        var g = Game.Solo();
        string line = g.C.Training.Lines[0].Id;
        var released = new ModelState { LineId = line, Version = 1, Status = ModelStatus.Released, Name = "On sale" };
        g.Me.Models.Add(released);
        for (int v = 0; v < 10; v++)
        {
            var m = new ModelState { LineId = line, Version = v + 2, Status = ModelStatus.Retired, Name = "Old " + v };
            g.Me.Models.Add(m);
        }
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.Me.Models, m => m.Status == ModelStatus.Released);
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == line && m.Status == ModelStatus.Retired));
    }

    [Fact]
    public void A_handcuffs_ending_does_not_stop_rival_weeks()
    {
        var g = Game.New(seed: 2);
        for (long seed = 1; seed < 100; seed++)
        {
            g.Me.Heat = 1000;
            g.Me.CaseEndsTick = g.Tick + g.C.Shady.CaseWeeks;
            g.State.Tick = g.Me.CaseEndsTick;
            g.State.Rng = RngSet.FromSeed(seed);
            ShadySystem.RunMonth(g.C, g.State, g.Me);
            if (g.State.Endings.Any(e => e.EndingId == "handcuffs")) break;
        }
        Assert.Contains(g.State.Endings, e => e.EndingId == "handcuffs");
        int tick = g.Tick;
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = 0;
        long cashBefore = rival.Cash;
        g.Weeks(8);
        Assert.True(g.Tick > tick);
        Assert.True(rival.Cash != cashBefore || rival.Techs.Count > 0);
    }

    [Fact]
    public void ContinueAfterEnding_refuses_an_unknown_mode()
    {
        var g = Game.Solo();
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        g.Refused(Command.ContinueAfterEnding(g.Me.Id, idx, "found-again"), CommandError.EndingUnavailable);
    }
}