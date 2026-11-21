using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class StandardsTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Draft(Game g)
    {
        g.Me.StdNext = g.State.Tick + 1;
        g.Weeks(1);
        Assert.Equal(g.Me.StdLevel + 1, g.Me.StdDraft);
    }

    [Fact]
    public void The_industry_drafts_a_standard_after_a_wait_and_ratifies_it_when_support_is_enough()
    {
        Game g = Lab();
        g.Weeks(g.C.Standards.FirstWeeks - 1);
        Assert.Equal(0, g.Me.StdDraft);
        g.Weeks(2);
        Assert.Equal(1, g.Me.StdDraft);
        Assert.Equal(1, Count(g, EventKind.StandardDrafted));
        int weeks = g.C.Standards.SupportNeeded / g.C.Standards.IndustryRate;
        g.Weeks(weeks - 2);
        Assert.Equal(0, g.Me.StdLevel);
        g.Weeks(2);
        Assert.Equal(1, g.Me.StdLevel);
        Assert.Equal(0, g.Me.StdDraft);
        Assert.Equal(0, g.Me.StdSupport);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.StandardRatified).Pick);
        g.Weeks(g.C.Standards.EveryWeeks + 2);
        Assert.Equal(2, g.Me.StdDraft);
    }

    [Fact]
    public void Nothing_is_drafted_past_the_top_level()
    {
        Game g = Lab(c => c.Standards.MaxLevel = 1);
        Draft(g);
        g.Weeks(g.C.Standards.SupportNeeded);
        Assert.Equal(1, g.Me.StdLevel);
        g.Weeks(g.C.Standards.EveryWeeks * 2);
        Assert.Equal(0, g.Me.StdDraft);
        Assert.Equal(1, Count(g, EventKind.StandardDrafted));
    }

    [Fact]
    public void A_member_and_a_sponsor_speed_a_draft_up_for_dues_and_the_part_is_logged()
    {
        Game plain = Lab();
        Game member = Lab();
        Game sponsor = Lab();
        foreach (Game g in new[] { plain, member, sponsor }) Draft(g);
        long cash = sponsor.Me.Cash;
        sponsor.Do(Command.TakeStandardsRole(sponsor.Id, Standards.Sponsor));
        Assert.Equal(cash - sponsor.C.Standards.SponsorFee, sponsor.Me.Cash);
        member.Do(Command.TakeStandardsRole(member.Id, Standards.Member));
        foreach (Game g in new[] { plain, member, sponsor }) g.Weeks(5);
        Assert.True(sponsor.Me.StdSupport > member.Me.StdSupport);
        Assert.True(member.Me.StdSupport > plain.Me.StdSupport);
        Assert.Equal(5 * Standards.Rate(member.C, member.Me), member.Me.StdSupport);
        Assert.True(sponsor.Me.Ledger.Lifetime.Fines < member.Me.Ledger.Lifetime.Fines);
        sponsor.Weeks(sponsor.C.Standards.SupportNeeded);
        Assert.Equal(1, sponsor.Me.StdLevel);
        Assert.Equal(Standards.Sponsor, sponsor.State.Events.Last(e => e.Kind == EventKind.StandardRatified).Pick);
        Assert.Equal(Standards.Observer, sponsor.Me.StdRole);
    }

    [Fact]
    public void An_obstructor_holds_a_draft_at_a_stand_for_dues_and_reputation()
    {
        Game g = Lab();
        Draft(g);
        g.Do(Command.TakeStandardsRole(g.Id, Standards.Obstructor));
        Assert.Equal(0, Standards.Rate(g.C, g.Me));
        long fines = g.Me.Ledger.Lifetime.Fines;
        g.Weeks(60);
        Assert.Equal(0, g.Me.StdSupport);
        Assert.Equal(0, g.Me.StdLevel);
        Assert.True(g.Me.Ledger.Lifetime.Fines < fines - 59 * g.C.Standards.ObstructDues);
        g.Do(Command.TakeStandardsRole(g.Id, Standards.Observer));
        g.Weeks(g.C.Standards.SupportNeeded);
        Assert.Equal(1, g.Me.StdLevel);
    }

    [Fact]
    public void A_part_is_refused_when_it_makes_no_sense()
    {
        Game g = Lab();
        g.Refused(Command.TakeStandardsRole(g.Id, Standards.Member), CommandError.InvalidArgument);
        Draft(g);
        g.Refused(Command.TakeStandardsRole(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.TakeStandardsRole(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.TakeStandardsRole(g.Id, Standards.Observer), CommandError.InvalidArgument);
        g.Me.Cash = 1;
        Assert.Equal(CommandError.NotEnoughCash, g.Try(Command.TakeStandardsRole(g.Id, Standards.Sponsor)).Error);
        g.Do(Command.TakeStandardsRole(g.Id, Standards.Member));
        g.Refused(Command.TakeStandardsRole(g.Id, Standards.Member), CommandError.InvalidArgument);
    }

    [Fact]
    public void Adopting_costs_a_price_and_an_upkeep_and_dropping_stops_both()
    {
        Game g = Lab();
        g.Refused(Command.AdoptStandards(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.DropStandards(g.Id), CommandError.InvalidArgument);
        Draft(g);
        g.Weeks(g.C.Standards.SupportNeeded);
        Assert.Equal(1, g.Me.StdLevel);
        long cash = g.Me.Cash;
        g.Do(Command.AdoptStandards(g.Id));
        Assert.Equal(cash - g.C.Standards.AdoptCost, g.Me.Cash);
        Assert.Equal(1, g.Me.StdAdopted);
        g.Refused(Command.AdoptStandards(g.Id), CommandError.InvalidArgument);
        long insurance = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(5);
        Assert.Equal(insurance - 5 * g.C.Standards.UpkeepPerLevel, g.Me.Ledger.Lifetime.Insurance);
        g.Do(Command.DropStandards(g.Id));
        insurance = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(5);
        Assert.Equal(insurance, g.Me.Ledger.Lifetime.Insurance);
        Assert.Equal(0, g.Me.StdAdopted);
    }

    [Fact]
    public void A_capable_first_release_without_the_standards_is_fined_and_one_with_them_is_not()
    {
        Game g = Lab(c => c.Standards.BindingCapability = 0);
        g.Me.StdLevel = 1;
        g.Me.StdNext = g.State.Tick + 1000;
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        long fines = g.Me.Ledger.Lifetime.Fines;
        int rep = g.Me.Reputation;
        g.Release(m, "game-ai");
        Assert.Equal(fines - g.C.Standards.LapseFine, g.Me.Ledger.Lifetime.Fines);
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(1, g.Me.StdLapses);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.StandardLapsed).Pick);

        Game h = Lab(c => c.Standards.BindingCapability = 0);
        h.Me.StdLevel = 1;
        h.Me.StdNext = h.State.Tick + 1000;
        h.Do(Command.AdoptStandards(h.Id));
        ModelState n = h.Train("mini", 300);
        h.UntilTrained(n);
        h.Release(n, "game-ai");
        Assert.Equal(0, h.Me.StdLapses);

        Game k = Lab(c => c.Standards.BindingCapability = 100000);
        k.Me.StdLevel = 1;
        k.Me.StdNext = k.State.Tick + 1000;
        ModelState o = k.Train("mini", 300);
        k.UntilTrained(o);
        k.Release(o, "game-ai");
        Assert.Equal(0, k.Me.StdLapses);
    }

    [Fact]
    public void Adopted_levels_weaken_claims_and_stand_in_for_safety_people_in_a_policy()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        int bare = g.Me.Cases.Single().Strength;
        g.Me.Cases.Clear();
        g.Me.StdLevel = 2;
        g.Me.StdAdopted = 2;
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        Assert.Equal(bare - 2 * g.C.Standards.ClaimStrength, g.Me.Cases.Single().Strength);

        Game h = Lab(c => c.Scaling.SafetyPerTier = 6);
        h.Me.SecurityTier = 1;
        Assert.False(ScalingPolicy.Safeguarded(h.C, h.Me, 1));
        h.Me.StdLevel = 1;
        h.Me.StdAdopted = 1;
        h.C.Standards.SafetyRelief = 2;
        Assert.False(ScalingPolicy.Safeguarded(h.C, h.Me, 1));
        h.C.Standards.SafetyRelief = 3;
        Assert.True(ScalingPolicy.Safeguarded(h.C, h.Me, 1));
        h.C.Standards.Enabled = 0;
        Assert.False(ScalingPolicy.Safeguarded(h.C, h.Me, 1));
    }

    [Fact]
    public void Nothing_happens_when_it_is_switched_off()
    {
        Game g = Lab(c => c.Standards.Enabled = 0);
        g.Weeks(g.C.Standards.FirstWeeks + 10);
        Assert.Equal(0, g.Me.StdDraft);
        g.Refused(Command.AdoptStandards(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.DropStandards(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.TakeStandardsRole(g.Id, Standards.Member), CommandError.InvalidArgument);
        Assert.Equal(0, Count(g, EventKind.StandardDrafted));
    }

    [Fact]
    public void Standards_save_load_and_play_on_the_same_and_an_older_save_has_none()
    {
        Game g = Lab();
        Draft(g);
        g.Do(Command.TakeStandardsRole(g.Id, Standards.Member));
        g.Weeks(15);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(60);
        g.Weeks(60);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 85, g.Sim.Save());
        Assert.DoesNotContain("stdLevel", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.StdLevel);
        Assert.Equal(0, lab.StdDraft);
    }

    [Fact]
    public void A_rival_with_a_part_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.StdLevel = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("industry standards"));
        rival.StdLevel = 0;
        g.Me.StdDraft = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("industry standards"));
        g.Me.StdDraft = 0;
        g.Me.StdSupport = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("industry standards"));
        g.Me.StdSupport = 0;
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Standards.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Standards.Enabled = 2;
        c.Standards.MaxLevel = 0;
        c.Standards.SupportNeeded = 0;
        c.Standards.SponsorDues = c.Standards.MemberDues - 1;
        c.Standards.ObstructEvery = 0;
        c.Standards.AdoptCost = -1;
        c.Standards.LapseReputation = 501;
        c.Standards.SafetyRelief = 21;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxLevel", "supportNeeded", "sponsorDues", "obstructEvery", "adoptCost", "lapseReputation", "safetyRelief" })
            Assert.Contains(problems, p => p.Contains("standards." + key));
    }
}
