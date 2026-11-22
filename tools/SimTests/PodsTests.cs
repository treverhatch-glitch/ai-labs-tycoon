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

public class PodsTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        int n = 0;
        foreach (StaffMember p in g.Me.Staff)
        {
            p.TraitId = g.C.StaffPeople.Traits[n++ % g.C.StaffPeople.Traits.Count].Id;
            p.Senior = 0;
            p.Morale = 500;
        }
        return g;
    }

    static List<StaffMember> Of(Game g, string role) { return g.Me.Staff.Where(p => p.RoleId == role).ToList(); }

    static int Chem(Game g, params StaffMember[] people)
    {
        return Pods.Chemistry(g.C, g.Me, people.Select(p => p.Id).ToList(), 0);
    }

    static string AnyOtherRole(Game g, string role) { return g.Me.Staff.First(p => p.RoleId != role).RoleId; }

    [Fact]
    public void Mixed_roles_and_a_senior_with_a_junior_help_chemistry()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        StaffMember other = g.Me.Staff.First(p => p.RoleId != "researcher");
        int same = Chem(g, r[0], r[1]);
        int mixed = Chem(g, r[0], other);
        Assert.Equal(g.C.Pods.BaseChem, same);
        Assert.Equal(same + g.C.Pods.RolePoints, mixed);
        r[0].Senior = 1;
        Assert.Equal(same + g.C.Pods.MixPoints, Chem(g, r[0], r[1]));
    }

    [Fact]
    public void Two_people_with_the_same_quirk_clash()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        int plain = Chem(g, r[0], r[1], r[2]);
        r[0].TraitId = "star";
        r[1].TraitId = "star";
        Assert.Equal(plain - g.C.Pods.ClashPoints, Chem(g, r[0], r[1], r[2]));
        r[2].TraitId = "star";
        Assert.Equal(plain - 3 * g.C.Pods.ClashPoints, Chem(g, r[0], r[1], r[2]));
        r[2].TraitId = "tenx";
        r[1].TraitId = "tenx";
        Assert.Equal(plain - g.C.Pods.ClashPoints, Chem(g, r[0], r[1], r[2]));
    }

    [Fact]
    public void Weeks_together_and_morale_add_and_it_never_leaves_zero_to_a_thousand()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        List<int> ids = new List<int> { r[0].Id, r[1].Id };
        Assert.Equal(g.C.Pods.BaseChem + 10 * g.C.Pods.TenureStep, Pods.Chemistry(g.C, g.Me, ids, 10));
        Assert.Equal(g.C.Pods.BaseChem + g.C.Pods.TenureCap, Pods.Chemistry(g.C, g.Me, ids, 1000));
        r[0].Morale = 900;
        r[1].Morale = 900;
        Assert.Equal(g.C.Pods.BaseChem + 400 * g.C.Pods.MoraleShare / 1000, Pods.Chemistry(g.C, g.Me, ids, 0));
        r[0].Morale = 0;
        r[1].Morale = 0;
        g.C.Pods.BaseChem = 10;
        Assert.Equal(0, Pods.Chemistry(g.C, g.Me, ids, 0));
    }

    [Fact]
    public void Forming_costs_cash_for_each_person_and_sets_the_chemistry()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        long cash = g.Me.Cash;
        g.Do(Command.FormPod(g.Id, r[0].Id, r[1].Id, r[2].Id));
        Assert.Equal(cash - 3 * g.C.Pods.KickoffCost, g.Me.Cash);
        Assert.Equal(3, g.Me.PodIds.Count);
        Assert.Equal(g.C.Pods.BaseChem, g.Me.PodChem);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PodFormed && e.SubjectId == 3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_pod_needs_real_distinct_people_in_range_and_cash_one_pod_at_a_time()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        g.Refused(Command.FormPod(g.Id, r[0].Id), CommandError.InvalidArgument);
        g.Refused(Command.FormPod(g.Id, r[0].Id, r[0].Id), CommandError.InvalidArgument);
        g.Refused(Command.FormPod(g.Id, r[0].Id, 99999), CommandError.UnknownId);
        g.Refused(Command.FormPod(g.Id, g.Me.Staff.Take(6).Select(p => p.Id).ToArray()), CommandError.InvalidArgument);
        g.Me.Cash = 10;
        g.Refused(Command.FormPod(g.Id, r[0].Id, r[1].Id), CommandError.NotEnoughCash);
        g.Me.Cash = 1_000_000;
        g.Do(Command.FormPod(g.Id, r[0].Id, r[1].Id));
        g.Refused(Command.FormPod(g.Id, r[2].Id, r[3].Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Good_chemistry_adds_research_points_and_bad_chemistry_takes_them()
    {
        Game good = Lab();
        List<StaffMember> r = Of(good, "researcher");
        StaffMember other = good.Me.Staff.First(p => p.RoleId != "researcher");
        r[0].Senior = 1;
        good.Do(Command.FormPod(good.Id, r[0].Id, r[1].Id, other.Id));
        Game plain = Lab();
        good.Weeks(10);
        plain.Weeks(10);
        Assert.True(good.Me.PodPoints > 0);
        Assert.True(good.Me.ResearchPoints > plain.Me.ResearchPoints);
        Game bad = Lab(c => c.Pods.BaseChem = 100);
        List<StaffMember> b = Of(bad, "researcher");
        bad.Do(Command.FormPod(bad.Id, b[0].Id, b[1].Id));
        bad.Me.ResearchPoints = 1000;
        bad.Weeks(5);
        Assert.True(bad.Me.PodPoints < 0);
        Assert.True(bad.Me.ResearchPoints >= 0);
        Assert.True(Pods.Weekly(bad.C, bad.Me) < 0);
    }

    [Fact]
    public void Swapping_halves_the_weeks_together_and_waits_a_cooldown()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        g.Do(Command.FormPod(g.Id, r[0].Id, r[1].Id));
        g.Weeks(10);
        Assert.Equal(10, g.Me.PodWeeks);
        g.Do(Command.SwapPodMember(g.Id, r[1].Id, r[2].Id));
        Assert.Equal(5, g.Me.PodWeeks);
        Assert.Contains(r[2].Id, g.Me.PodIds);
        Assert.DoesNotContain(r[1].Id, g.Me.PodIds);
        g.Refused(Command.SwapPodMember(g.Id, r[2].Id, r[1].Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Pods.SwapCooldown);
        g.Refused(Command.SwapPodMember(g.Id, r[0].Id, r[2].Id), CommandError.InvalidArgument);
        g.Refused(Command.SwapPodMember(g.Id, r[3].Id, r[1].Id), CommandError.InvalidArgument);
        g.Refused(Command.SwapPodMember(g.Id, r[0].Id, 99999), CommandError.UnknownId);
        g.Do(Command.SwapPodMember(g.Id, r[0].Id, r[1].Id));
    }

    [Fact]
    public void A_pod_that_loses_people_breaks_up_and_a_disband_is_remembered()
    {
        Game g = Lab();
        List<StaffMember> r = Of(g, "researcher");
        g.Do(Command.FormPod(g.Id, r[0].Id, r[1].Id));
        g.Weeks(3);
        g.Me.Staff.Remove(r[1]);
        g.Weeks(1);
        Assert.Empty(g.Me.PodIds);
        Assert.Equal(0, g.Me.PodChem);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PodEnded && e.Pick == 0);
        Assert.Equal(1, g.Me.PodsFormed);
        g.Do(Command.FormPod(g.Id, r[0].Id, r[2].Id));
        g.Do(Command.DisbandPod(g.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PodEnded && e.Pick == 1);
        g.Refused(Command.DisbandPod(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_cannot_the_switch_turns_it_off_and_saves_round_trip()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.FormPod(rival.Id, 1, 2)).Ok);
        Game off = Lab(c => c.Pods.Enabled = 0);
        off.Refused(Command.FormPod(off.Id, off.Me.Staff[0].Id, off.Me.Staff[1].Id), CommandError.InvalidArgument);
        Game s = Lab();
        List<StaffMember> r = Of(s, "researcher");
        s.Do(Command.FormPod(s.Id, r[0].Id, r[1].Id));
        s.Weeks(6);
        Assert.Empty(SaveGame.Problems(s.C, s.State));
        Simulation back = Simulation.Load(s.C, s.Sim.Save());
        Assert.Equal(s.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(s.C, 106, s.Sim.Save());
        Simulation o = Simulation.Load(s.C, old);
        Assert.Empty(o.State.Labs.First(l => l.Id == s.Id).PodIds);
        Assert.Empty(SaveGame.Problems(s.C, o.State));
    }
}
