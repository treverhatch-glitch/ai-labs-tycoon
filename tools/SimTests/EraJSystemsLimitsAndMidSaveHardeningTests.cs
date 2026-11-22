using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraJSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000;
        g.Me.Reputation = 600;
        return g;
    }

    static void Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
    }

    [Fact]
    public void Data_acquire_refuses_beyond_stock_cap()
    {
        Game g = Lab(38, c => { c.Datasets.MaxBuy = 50; c.Datasets.StockCap = 50; });
        g.Do(Command.AcquireData(g.Id, 1, 40));
        g.Refused(Command.AcquireData(g.Id, 1, 20), CommandError.InvalidArgument);
    }

    [Fact]
    public void Governance_refuses_a_second_permanent_choice()
    {
        Game g = Lab(39);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Refused(Command.SetGovernance(g.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Spot_share_refuses_above_permille_cap()
    {
        Game g = Lab(40);
        g.Refused(Command.SetSpotShare(g.Id, 1200), CommandError.InvalidArgument);
        g.Do(Command.SetSpotShare(g.Id, 400));
        Assert.Equal(400, g.Me.SpotPermille);
    }

    [Fact]
    public void Bankrupt_lab_with_data_governance_and_spot_validates_save()
    {
        Game g = Lab(41);
        Selling(g);
        g.Do(Command.AcquireData(g.Id, 1, 30));
        g.Do(Command.SetUserData(g.Id, 1));
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.SetSpotShare(g.Id, 250));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Datasets_and_user_data_on_same_lab_survive_save_load()
    {
        Game g = Lab(42);
        Selling(g);
        g.Do(Command.AcquireData(g.Id, 2, 35));
        g.Do(Command.SetUserData(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Me.UserPolicy, back.Player.UserPolicy);
        Assert.Equal(g.Me.DataRaw2, back.Player.DataRaw2);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }
}