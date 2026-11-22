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

public class ProgrammesRunQueuePodsLicencePledgesTogetherTests
{
    const string Big = "beowulf-clusters";

    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Safety.SectorFlavours.Clear();
        c.Chiefs.MinStaff = 0;
        c.Support.Enabled = 0;
        c.Security.Enabled = 0;
        c.Tax.Enabled = 0;
        c.Standards.Enabled = 0;
        c.Reliability.Enabled = 0;
        c.GovContracts.Enabled = 0;
        c.Shocks.Enabled = 0;
    }

    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(4, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 500;
        int n = 0;
        foreach (StaffMember p in g.Me.Staff)
        {
            p.TraitId = g.C.StaffPeople.Traits[n++ % g.C.StaffPeople.Traits.Count].Id;
            p.Senior = 0;
            p.Morale = 500;
        }
        return g;
    }

    static ModelState Selling(Game g, int alignment = 800)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = alignment;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        return m;
    }

    static void Pod(Game g)
    {
        int a = g.Me.Staff.First(p => p.RoleId == "researcher").Id;
        int b = g.Me.Staff.First(p => p.RoleId != "researcher").Id;
        g.Do(Command.FormPod(g.Id, a, b));
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_sharp_pod_takes_a_week_off_each_programme_stage_and_a_blunt_one_does_not()
    {
        Game plain = Lab();
        plain.Do(Command.StartProgramme(plain.Id, Big));
        plain.Do(Command.ChooseApproach(plain.Id, 1));
        Assert.Equal(plain.C.Programmes.SafeWeeks, plain.Me.PgLeft);

        Game sharp = Lab(c => c.Pods.SharpChem = 0);
        Pod(sharp);
        Assert.True(Pods.Sharp(sharp.C, sharp.Me));
        sharp.Do(Command.StartProgramme(sharp.Id, Big));
        sharp.Do(Command.ChooseApproach(sharp.Id, 1));
        Assert.Equal(sharp.C.Programmes.SafeWeeks - sharp.C.Pods.ProgrammeWeeksSaved, sharp.Me.PgLeft);
        sharp.Do(Command.AbandonProgramme(sharp.Id));
        sharp.Do(Command.StartProgramme(sharp.Id, Big));
        sharp.Do(Command.ChooseApproach(sharp.Id, 2));
        Assert.Equal(sharp.C.Programmes.BoldWeeks - sharp.C.Pods.ProgrammeWeeksSaved, sharp.Me.PgLeft);

        Game blunt = Lab(c => c.Pods.SharpChem = 1000);
        Pod(blunt);
        Assert.False(Pods.Sharp(blunt.C, blunt.Me));
        blunt.Do(Command.StartProgramme(blunt.Id, Big));
        blunt.Do(Command.ChooseApproach(blunt.Id, 1));
        Assert.Equal(blunt.C.Programmes.SafeWeeks, blunt.Me.PgLeft);
    }

    [Fact]
    public void A_stage_never_takes_less_than_a_week_however_many_weeks_a_pod_saves()
    {
        Game g = Lab(c => { c.Pods.SharpChem = 0; c.Pods.ProgrammeWeeksSaved = 50; });
        Pod(g);
        g.Do(Command.StartProgramme(g.Id, Big));
        g.Do(Command.ChooseApproach(g.Id, 2));
        Assert.Equal(1, g.Me.PgLeft);
    }

    [Fact]
    public void A_rushed_best_model_counts_as_less_aligned_for_the_licence_bar()
    {
        Game g = Lab();
        ModelState m = Selling(g, g.C.Licence.BarC);
        Assert.True(Licence.MeetsBar(g.C, g.Me, 3));
        m.QueueLevel = 3;
        Assert.False(Licence.MeetsBar(g.C, g.Me, 3));
        Assert.True(Licence.MeetsBar(g.C, g.Me, 2));
        m.Alignment = g.C.Licence.BarC + g.C.Licence.RushPenalty;
        Assert.True(Licence.MeetsBar(g.C, g.Me, 3));
        m.QueueLevel = 1;
        m.Alignment = g.C.Licence.BarC;
        Assert.True(Licence.MeetsBar(g.C, g.Me, 3));
    }

    [Fact]
    public void A_rushed_model_is_refused_at_review_and_cannot_renew()
    {
        Game g = Lab();
        ModelState m = Selling(g, g.C.Licence.BarC);
        m.QueueLevel = 3;
        g.Do(Command.ApplyLicence(g.Id, 3));
        g.Weeks(g.C.Licence.ReviewWeeks);
        Assert.Equal(1, g.Me.LicRefused);
        m.QueueLevel = 2;
        g.Weeks(g.C.Licence.CooldownWeeks);
        g.Do(Command.ApplyLicence(g.Id, 3));
        g.Weeks(g.C.Licence.ReviewWeeks);
        Assert.Equal(Licence.Licensed, g.Me.LicPhase);
        g.Weeks(g.C.Licence.TermC - g.C.Licence.RenewWindow);
        m.QueueLevel = 3;
        g.Refused(Command.RenewLicence(g.Id), CommandError.MissingPrerequisite);
    }

    [Fact]
    public void Breaking_a_support_pledge_while_licensed_is_a_strike_on_the_licence()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.ApplyLicence(g.Id, 2));
        g.Weeks(g.C.Licence.ReviewWeeks);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        g.Do(Command.BreakSupportPledge(g.Id, m.Id));
        Assert.Equal(1, g.Me.LicStrikes);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LicenceStrike);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        g.Do(Command.BreakSupportPledge(g.Id, m.Id));
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Equal(1, g.Me.LicSuspended);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_pledge_that_ends_with_the_model_off_sale_also_counts_when_licensed_and_the_switch_turns_it_off()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.ApplyLicence(g.Id, 2));
        g.Weeks(g.C.Licence.ReviewWeeks);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        m.Status = ModelStatus.Retired;
        g.Weeks(1);
        Assert.Equal(1, g.Me.LicStrikes);

        Game off = Lab(c => c.Licence.BreachStrike = 0);
        ModelState om = Selling(off);
        off.Do(Command.ApplyLicence(off.Id, 2));
        off.Weeks(off.C.Licence.ReviewWeeks);
        off.Do(Command.PledgeSupport(off.Id, om.Id, 52));
        off.Do(Command.BreakSupportPledge(off.Id, om.Id));
        Assert.Equal(0, off.Me.LicStrikes);
    }

    [Fact]
    public void A_pledge_without_a_licence_is_no_strike()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        g.Do(Command.BreakSupportPledge(g.Id, m.Id));
        Assert.Equal(0, g.Me.LicStrikes);
        Assert.Equal(1, g.Me.LtsBroken);
    }

    [Fact]
    public void A_sharp_pod_cuts_what_a_pledge_costs_each_week()
    {
        Game plain = Lab();
        ModelState pm = Selling(plain);
        plain.Do(Command.PledgeSupport(plain.Id, pm.Id, 104));
        Game sharp = Lab(c => c.Pods.SharpChem = 0);
        ModelState sm = Selling(sharp);
        Pod(sharp);
        sharp.Do(Command.PledgeSupport(sharp.Id, sm.Id, 104));
        plain.Weeks(10);
        sharp.Weeks(10);
        long plainCost = -plain.Me.Ledger.Lifetime.Salaries;
        long sharpCost = -sharp.Me.Ledger.Lifetime.Salaries;
        Assert.True(sharpCost < plainCost);
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Game.New(5, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Assert.Equal(0, g.Me.PgTech + g.Me.PodIds.Count + g.Me.LicRegime + g.Me.LicGranted + g.Me.LicRefused + g.Me.LtsKept + g.Me.LtsBroken);
        Assert.All(g.Me.Models, m => Assert.Equal(0, m.QueueLevel + m.LtsLeft));
        foreach (EventKind k in new[] { EventKind.ProgrammeStarted, EventKind.RunPriorityChanged, EventKind.PodFormed, EventKind.LicenceApplied, EventKind.SupportPledged })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        ModelState next = g.Train("mini", 0);
        g.Do(Command.SetRunPriority(g.Id, next.Id, 3));
        Pod(g);
        g.Do(Command.StartProgramme(g.Id, Big));
        g.Do(Command.ChooseApproach(g.Id, 1));
        g.Do(Command.ApplyLicence(g.Id, 1));
        g.Do(Command.PledgeSupport(g.Id, m.Id, 52));
        g.Weeks(8);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        g.State.Tick = (2022 - 1995) * g.C.Calendar.WeeksPerYear;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        List<TechConfig> big = g.C.Research.Techs.Where(t => t.Cost >= g.C.Programmes.MinCost).ToList();
        int started = 0, formed = 0, applied = 0, pledged = 0, prioritised = 0;
        for (int week = 0; week < 300 && !g.Me.Bankrupt; week++)
        {
            g.Me.ResearchPoints += 20_000;
            if (week % 10 == 1 && g.Me.PgTech == 0)
            {
                TechConfig pick = big.FirstOrDefault(t => !LabQueries.HasTech(g.Me, t.Id) && EraSystem.TechOpen(g.C, t, g.State.Tick) && t.Requires.All(r => LabQueries.HasTech(g.Me, r)));
                if (pick != null && g.Try(Command.StartProgramme(g.Id, pick.Id)).Ok) started++;
            }
            if (week % 30 >= 2 && week % 30 <= 20 && week % 5 == 2) g.Try(Command.ChooseApproach(g.Id, 1 + week / 5 % 2));
            if (week % 30 >= 3 && week % 30 <= 22 && week % 5 == 3) g.Try(Command.RecoverStage(g.Id, 1 + week / 5 % 3));
            if (week % 90 == 80) g.Try(Command.AbandonProgramme(g.Id));
            if (week % 25 == 3) g.Try(Command.StartTraining(g.Id, "mini", 0));
            ModelState training = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Training);
            if (training != null && week % 7 == 0 && g.Try(Command.SetRunPriority(g.Id, training.Id, 1 + week / 7 % 3)).Ok) prioritised++;
            if (week % 40 == 4)
            {
                var staff = g.Me.Staff.OrderBy(p => p.Id).Take(2 + week / 40 % 3).Select(p => p.Id).ToArray();
                if (g.Try(Command.FormPod(g.Id, staff)).Ok) formed++;
            }
            if (week % 40 == 30) g.Try(Command.DisbandPod(g.Id));
            if (week % 50 == 6 && g.Try(Command.ApplyLicence(g.Id, 1 + week / 50 % 3)).Ok) applied++;
            if (week % 50 == 40) g.Try(Command.RenewLicence(g.Id));
            if (week % 100 == 90) g.Try(Command.SurrenderLicence(g.Id));
            ModelState selling = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released);
            if (selling != null && week % 60 == 9 && g.Try(Command.PledgeSupport(g.Id, selling.Id, 26 + week / 60 % 3 * 26)).Ok) pledged++;
            if (selling != null && week % 60 == 50) g.Try(Command.BreakSupportPledge(g.Id, selling.Id));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(started > 0, "no programme was ever started: " + big.Count);
        Assert.True(prioritised > 0, "no run was ever prioritised");
        Assert.Equal(formed, g.Me.PodsFormed);
        Assert.True(formed > 0, "no pod was ever formed");
        Assert.True(applied > 0, "no licence was ever applied for");
        Assert.True(pledged > 0, "no pledge was ever made");
    }
}
