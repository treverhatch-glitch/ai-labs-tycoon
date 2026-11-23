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

/// <summary>Zero, max, bankrupt, and cross-system edges for era-h systems.</summary>
public class EraHSystemsLimitsAndMidSaveHardeningTests
{
    const string Tech = "beowulf-clusters";

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, long? cash = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 40);
        g.Me.ResearchPoints = 500_000;
        if (cash.HasValue) g.Me.Cash = cash.Value;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = 800;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        return m;
    }

    [Fact]
    public void Rush_run_queue_refuses_beyond_max_rush()
    {
        Game g = Lab(9, c => c.RunQueue.MaxRush = 1);
        ModelState a = g.Train("mini", 0);
        ModelState b = g.Train("mini", 0);
        g.Do(Command.SetRunPriority(g.Id, a.Id, 3));
        g.Refused(Command.SetRunPriority(g.Id, b.Id, 3), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pod_refuses_without_cash_and_forms_when_rich()
    {
        Game broke = Lab(10, cash: 1);
        StaffMember a = broke.Me.Staff[0];
        StaffMember b = broke.Me.Staff[1];
        broke.Refused(Command.FormPod(broke.Id, a.Id, b.Id), CommandError.NotEnoughCash);
        Game rich = Lab(11);
        rich.Do(Command.FormPod(rich.Id, rich.Me.Staff[0].Id, rich.Me.Staff[1].Id));
        Assert.Equal(2, rich.Me.PodIds.Count);
    }

    [Fact]
    public void Bankrupt_lab_with_programme_pod_and_pledge_validates_save()
    {
        Game g = Lab(12);
        g.Do(Command.StartProgramme(g.Id, Tech));
        g.Do(Command.ChooseApproach(g.Id, 1));
        g.Do(Command.FormPod(g.Id, g.Me.Staff[0].Id, g.Me.Staff[1].Id));
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Programme_bold_stage_abandon_mid_save_matches_digest()
    {
        Game straight = Lab(13, c => c.Programmes.BoldChancePermille = 1);
        straight.Do(Command.StartProgramme(straight.Id, Tech));
        straight.Do(Command.ChooseApproach(straight.Id, 2));
        straight.Weeks(6);
        Game choppy = Lab(13, c => c.Programmes.BoldChancePermille = 1);
        choppy.Do(Command.StartProgramme(choppy.Id, Tech));
        choppy.Do(Command.ChooseApproach(choppy.Id, 2));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Licence_and_run_queue_on_same_training_survive_save_load()
    {
        Game g = Lab(14);
        ModelState m = g.Train("mini", 0);
        g.Do(Command.SetRunPriority(g.Id, m.Id, 3));
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = 800;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.Do(Command.ApplyLicence(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraHBounds: true);
    }
}