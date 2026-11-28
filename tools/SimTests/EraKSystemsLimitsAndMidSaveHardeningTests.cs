using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraKSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PostTraining.MinEra = 0;
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000;
        g.Me.BoardInvestorSeats = 1;
        g.Me.DataClean4 = 500;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    [Fact]
    public void Knowledge_refuses_more_models_than_max_owned_per_person()
    {
        Game g = Lab(48, c => c.Knowledge.MaxOwned = 1);
        ModelState a = Trained(g);
        ModelState b = Trained(g);
        StaffMember r = g.Me.Staff.First(s => s.RoleId == "researcher");
        g.Do(Command.AssignOwner(g.Id, a.Id, r.Id));
        g.Refused(Command.AssignOwner(g.Id, b.Id, r.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Post_training_refuses_a_second_tune_while_one_runs()
    {
        Game g = Lab(49, c => c.PostTraining.MaxPasses = 4);
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Refused(Command.PostTrain(g.Id, m.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Board_meetings_refuse_without_investor_seats()
    {
        Game g = Lab(50);
        g.Me.BoardInvestorSeats = 0;
        g.Refused(Command.HoldBoardMeetings(g.Id, 1), CommandError.MissingPrerequisite);
    }

    [Fact]
    public void Bankrupt_lab_with_annotation_board_and_owner_validates_save()
    {
        Game g = Lab(51);
        g.Do(Command.AcquireData(g.Id, 1, 40));
        g.Do(Command.CleanData(g.Id, 1, 40));
        g.Weeks(g.C.Datasets.CleanWeeks);
        g.Do(Command.ContractAnnotators(g.Id, 1));
        ModelState m = Trained(g);
        g.Do(Command.AssignOwner(g.Id, m.Id, g.Me.Staff.First(s => s.RoleId == "engineer").Id));
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Annotation_and_post_training_on_same_lab_survive_save_load()
    {
        Game g = Lab(52);
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Do(Command.ContractAnnotators(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(10);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(10);
        Assert.Equal(g.Me.AnnVendor, back.Player.AnnVendor);
        Assert.Equal(g.Me.Models[0].PostKind, back.Player.Models[0].PostKind);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }
}