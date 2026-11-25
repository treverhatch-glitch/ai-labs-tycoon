using System.IO;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EndingsTests
{
    [Fact]
    public void Player_bankruptcy_records_one_ending_and_bailout_restores_cash_invariant()
    {
        var g = Game.Solo(seed: 91);
        g.Me.GrantPerWeek = 0;
        g.Me.GrantEndsTick = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        Assert.True(g.Me.Bankrupt);
        Assert.Single(g.State.Endings, e => e.EndingId == "bankrupt" && e.Choice == "");
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "bailout"));
        Assert.False(g.Me.Bankrupt);
        Assert.True(g.Me.Ledger.Lifetime.CapitalIn >= g.C.Endings.BailoutCash);
        Assert.True(g.Me.Cash > 0);
        g.Refused(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"), CommandError.AlreadyChose);
    }

    [Fact]
    public void The_world_keeps_stepping_after_an_ending()
    {
        var g = Game.New(seed: 22);
        int rivalRuns = g.State.Labs.Count(l => l.Controller == LabController.Plan);
        g.Me.Cash = -1;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int tick = g.Tick;
        g.Weeks(20);
        Assert.True(g.Tick > tick);
        Assert.True(g.State.Labs.Count(l => l.Controller == LabController.Plan) == rivalRuns);
    }

    [Fact]
    public void SellLab_found_again_puts_a_new_player_lab_first()
    {
        var g = Game.Solo(seed: 5, tweak: c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Me.Cash = 100_000;
        long price = EndingSystem.SellPrice(g.C, g.State, g.Me);
        g.Do(Command.SellLab(g.Id, "found-again"));
        Assert.Equal(2, g.State.Labs.Count(l => l.Controller == LabController.Player || l.RivalId == "successor"));
        Assert.Equal(g.State.PlayerLabId, g.State.Labs[0].Id);
        Assert.True(g.State.Labs[0].Cash >= price - 1);
        Assert.Equal("successor", g.State.Labs.First(l => l.RivalId == "successor").RivalId);
    }

    [Fact]
    public void SellLab_spectate_refuses_player_commands_but_rivals_keep_planning()
    {
        var g = Game.New(seed: 8);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 3_000_000;
        g.Me.Cash = 50_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        g.Refused(Command.Hire(playerId, "researcher", 1), CommandError.Spectating);
        LabState rival = g.Rival("openaye");
        int before = rival.Log.Count;
        for (int m = 0; m < 12 && rival.Log.Count <= before; m++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);
            g.Weeks(1);
        }
        Assert.True(rival.Log.Count > before);
    }

    [Fact]
    public void Bankrupt_spectate_saves_cleanly()
    {
        var g = Game.Solo(seed: 11);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        Assert.Single(g.State.Endings, e => e.EndingId == "bankrupt");
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
    }

    [Fact]
    public void SellLab_found_again_keeps_the_ledger_in_balance()
    {
        var g = Game.Solo(seed: 5, tweak: c => c.Endings.ExitRevenueYears = 1);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2010);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Me.Cash = 100_000;
        long price = EndingSystem.SellPrice(g.C, g.State, g.Me);
        g.Do(Command.SellLab(g.Id, "found-again"));
        LabState fresh = g.Me;
        Assert.Equal(price, fresh.Ledger.Lifetime.CapitalIn);
        Assert.Equal(price, fresh.Cash);
        Assert.Equal(0, fresh.Ledger.StartCash);
        Assert.Equal(fresh.Cash, fresh.Ledger.StartCash + fresh.Ledger.Lifetime.Net);
        Assert.NotEmpty(fresh.Techs);
        Assert.NotNull(g.State.Vendor.Standing(fresh.Id));
    }

    [Fact]
    public void Frontier_grows_by_140_percent_and_needs_all_techs_first()
    {
        var g = Game.Solo();
        long c0 = EndingSystem.FrontierCost(g.C, g.Me);
        Assert.True(c0 > 0);
        g.Refused(Command.ResearchFrontier(g.Id), CommandError.FrontierUnavailable);
        g.Me.ResearchPoints = 9_999_999;
        foreach (TechConfig tech in g.C.Research.Techs)
            if (EraSystem.TechOpen(g.C, tech, g.Tick) && !g.Me.Techs.Contains(tech.Id)) g.Research(tech.Id);
        int techBefore = LabQueries.TechBonus(g.C, g.Me, TechEffectKind.CapabilityPermille);
        g.Do(Command.ResearchFrontier(g.Id));
        Assert.Equal(1, g.Me.FrontierLevel);
        long c1 = EndingSystem.FrontierCost(g.C, g.Me);
        Assert.Equal(MathI.MulPermille(c0, 1400), c1);
        Assert.Equal(techBefore + 50, LabQueries.TechBonus(g.C, g.Me, TechEffectKind.CapabilityPermille));
    }

    [Fact]
    public void Monopoly_fires_when_the_player_dominates_and_not_twice_within_ten_years()
    {
        var g = Game.Solo(seed: 40, tweak: c =>
        {
            c.Endings.MonopolyRevenuePpm = 400_000;
            c.Endings.MonopolyComputePpm = 400_000;
        });
        for (int y = 0; y < 3; y++)
        {
            g.State.Gauntlets.Add(new GauntletRecord { Year = 1995 + y, CrownLabId = g.Id });
            g.Me.Ledger.History.Add(new MonthRecord
            {
                Year = 1995 + y,
                Month = 12,
                Totals = { Revenue = 9_000_000 },
            });
            for (int i = 0; i < g.State.Labs.Count; i++)
                g.State.Labs[i].LastWeek.Compute = i == 0 ? 9000 : 100;
        }
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1998) + g.C.Calendar.WeeksPerYear - 1;
        EndingSystem.TryMonopoly(g.C, g.State);
        Assert.Contains(g.State.Endings, e => e.EndingId == "monopoly");
        int att = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        EndingSystem.TryMonopoly(g.C, g.State);
        Assert.Equal(att, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Schema_five_bankrupt_save_gets_a_bankrupt_ending_record()
    {
        string json = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema5-bankrupt-save.json"));
        var sim = Simulation.Load(TestData.Config(), json);
        Assert.Single(sim.State.Endings, e => e.EndingId == "bankrupt" && e.Choice == "");
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
    }

    [Fact]
    public void Museum_keeps_six_retired_models_per_line()
    {
        var g = Game.Solo();
        string line = g.C.Training.Lines[0].Id;
        for (int v = 0; v < 8; v++)
        {
            g.Do(Command.StartTraining(g.Id, line, 100));
            g.Me.Models.Last(m => m.LineId == line).Status = ModelStatus.Retired;
        }
        MuseumSystem.RunWeek(g.C, g.Me);
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == line && m.Status == ModelStatus.Retired));
    }

    [Fact]
    public void A_sixty_year_soak_from_1995_stays_bounded_and_keeps_releasing()
    {
        var g = Game.New(seed: 77);
        int start = g.Tick;
        int cap = 0;
        for (int w = 0; w < 60 * 48; w++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Sim.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Weeks(1);
            cap = System.Math.Max(cap, g.Sim.Save().Length);
        }
        Assert.Equal(60 * 48, g.Tick - start);
        Assert.True(cap < 5_000_000, "save grew to " + cap);
        Assert.Contains(g.Me.Models, m => m.Status == ModelStatus.Released || m.Status == ModelStatus.Trained);
    }
}