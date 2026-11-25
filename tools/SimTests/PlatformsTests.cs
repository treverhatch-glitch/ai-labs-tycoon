using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class PlatformsTests
{
    static void Quiet(GameConfig c)
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
    }

    static Game Lab(int year = 2027, int capability = 800, System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.State.Tick = (year - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = capability, Status = ModelStatus.Released, Alignment = 500, Name = "Distributed" });
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 600;
        return g;
    }

    [Fact]
    public void Signing_sets_the_first_cut_and_the_term_and_costs_nothing()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SignPlatform(g.Id, 1));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(1, g.Me.PlatPlat);
        Assert.Equal(g.C.Platforms.TermWeeks, g.Me.PlatLeft);
        Assert.Equal(g.C.Platforms.CutA, g.Me.PlatCut);
        Assert.Equal(g.C.Platforms.CutA, g.State.Events.Last(e => e.Kind == EventKind.PlatformSigned).Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_platform_wants_its_era_a_good_model_a_name_and_one_deal_at_a_time()
    {
        Game early = Lab(2015);
        early.Refused(Command.SignPlatform(early.Id, 1), CommandError.MissingPrerequisite);
        Game weak = Lab(2027, 50);
        weak.Refused(Command.SignPlatform(weak.Id, 1), CommandError.MissingPrerequisite);
        Game unknown = Lab();
        unknown.Me.Reputation = 10;
        unknown.Refused(Command.SignPlatform(unknown.Id, 1), CommandError.MissingPrerequisite);
        Game g = Lab();
        g.Refused(Command.SignPlatform(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SignPlatform(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.RenewPlatform(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.WalkFromPlatform(g.Id), CommandError.InvalidArgument);
        g.Do(Command.SignPlatform(g.Id, 2));
        g.Refused(Command.SignPlatform(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.RenewPlatform(g.Id), CommandError.InvalidArgument);
        Game off = Lab(2027, 800, c => c.Platforms.Enabled = 0);
        off.Refused(Command.SignPlatform(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void The_lift_is_the_extra_revenue_less_the_platforms_cut()
    {
        Game g = Lab();
        g.Do(Command.SignPlatform(g.Id, 1));
        long lift = MathI.MulPermille(1_000_000, g.C.Platforms.LiftA);
        long cut = MathI.MulPermille(lift, g.C.Platforms.CutA);
        Assert.Equal(lift - cut, Platforms.Lift(g.C, g.State, g.Me, 1_000_000));
        Assert.Equal(cut, g.Me.PlatPaid);
        Assert.Equal(lift - cut, g.Me.PlatKept);
        Assert.Equal(0, Platforms.Lift(g.C, g.State, g.Me, 0));
    }

    [Fact]
    public void Without_a_deal_the_market_pays_the_same()
    {
        Game g = Lab();
        Assert.Equal(0, Platforms.Lift(g.C, g.State, g.Me, 1_000_000));
    }

    static Game Played(bool deal)
    {
        Game g = Game.New(1, c => { c.Platforms.EraA = 0; c.Platforms.MinCapability = 0; c.Platforms.MinReputation = 0; });
        g.Autopilot(60);
        if (deal) g.Do(Command.SignPlatform(g.Id, 1));
        return g;
    }

    [Fact]
    public void In_a_real_game_the_deal_adds_revenue_to_the_player_only()
    {
        Game with = Played(true);
        Game without = Played(false);
        Assert.Equal(without.Me.Ledger.Lifetime.Revenue, with.Me.Ledger.Lifetime.Revenue);
        long base0 = without.Me.Ledger.Lifetime.Revenue;
        with.Weeks(6);
        without.Weeks(6);
        Assert.True(with.Me.Ledger.Lifetime.Revenue > without.Me.Ledger.Lifetime.Revenue);
        Assert.True(with.Me.PlatKept > 0);
        Assert.True(without.Me.Ledger.Lifetime.Revenue > base0);
        LabState rivalWith = with.State.Labs.First(l => l.Id != with.Id);
        LabState rivalWithout = without.State.Labs.First(l => l.Id != without.Id);
        Assert.Equal(rivalWithout.Ledger.Lifetime.Revenue, rivalWith.Ledger.Lifetime.Revenue);
    }

    [Fact]
    public void Reliance_grows_every_week_to_the_top()
    {
        Game g = Lab(2027, 800, c => c.Platforms.RelianceStep = 400);
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(2);
        Assert.Equal(800, g.Me.PlatReliance);
        g.Weeks(3);
        Assert.Equal(MathI.Permille, g.Me.PlatReliance);
    }

    [Fact]
    public void A_platform_that_is_relied_on_raises_its_price_when_the_deal_comes_up()
    {
        Game g = Lab(2027, 800, c => { c.Platforms.RelianceStep = 100; c.Platforms.TermWeeks = 20; c.Platforms.WindowWeeks = 5; });
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(14);
        Assert.Equal(0, g.Me.PlatHoldup);
        g.Weeks(1);
        Assert.Equal(1, g.Me.PlatHoldup);
        Assert.Equal(g.C.Platforms.CutA + g.C.Platforms.HoldupCut, g.Me.PlatCut);
        Assert.Equal(g.Me.PlatCut, g.State.Events.Last(e => e.Kind == EventKind.PlatformHoldup).Amount);
    }

    [Fact]
    public void A_platform_the_lab_does_not_rely_on_does_not_raise_its_price()
    {
        Game g = Lab(2027, 800, c => { c.Platforms.RelianceStep = 1; c.Platforms.TermWeeks = 20; c.Platforms.WindowWeeks = 5; });
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(16);
        Assert.Equal(1, g.Me.PlatHoldup);
        Assert.Equal(g.C.Platforms.CutA, g.Me.PlatCut);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.PlatformHoldup);
    }

    [Fact]
    public void Renewing_resets_the_term_and_costs_a_bigger_cut_each_time()
    {
        Game g = Lab(2027, 800, c => { c.Platforms.TermWeeks = 20; c.Platforms.WindowWeeks = 5; });
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(16);
        int cut = g.Me.PlatCut;
        g.Do(Command.RenewPlatform(g.Id));
        Assert.Equal(20, g.Me.PlatLeft);
        Assert.Equal(1, g.Me.PlatRenewals);
        Assert.Equal(0, g.Me.PlatHoldup);
        Assert.Equal(cut + g.C.Platforms.RenewCutStep, g.Me.PlatCut);
        Assert.Equal(1, g.Me.PlatPlat);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_cut_never_goes_over_the_most_a_platform_takes()
    {
        Game g = Lab(2027, 800, c => { c.Platforms.TermWeeks = 20; c.Platforms.WindowWeeks = 5; c.Platforms.MaxCut = 400; c.Platforms.RenewCutStep = 200; });
        g.Do(Command.SignPlatform(g.Id, 1));
        for (int i = 0; i < 3; i++)
        {
            g.Weeks(15);
            g.Do(Command.RenewPlatform(g.Id));
        }
        Assert.Equal(400, g.Me.PlatCut);
    }

    [Fact]
    public void Walking_away_loses_reputation_by_how_much_the_lab_relied_on_the_platform()
    {
        Game g = Lab(2027, 800, c => c.Platforms.RelianceStep = 500);
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(1);
        int rep = g.Me.Reputation;
        int loss = PressSystem.Damp(g.C, g.Me, g.C.Platforms.WalkReputation / 2);
        g.Do(Command.WalkFromPlatform(g.Id));
        Assert.Equal(rep - loss, g.Me.Reputation);
        Assert.Equal(0, g.Me.PlatPlat);
        Assert.Equal(0, g.Me.PlatPaid);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.PlatformEnded);
        Assert.Equal(0, e.Pick);
        Assert.Equal(loss, e.Amount);
    }

    [Fact]
    public void A_deal_left_to_run_out_ends_the_same_way_and_a_new_one_can_follow()
    {
        Game g = Lab(2027, 800, c => { c.Platforms.TermWeeks = 6; c.Platforms.WindowWeeks = 2; });
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(6);
        Assert.Equal(0, g.Me.PlatPlat);
        Assert.Equal(1, g.State.Events.Last(x => x.Kind == EventKind.PlatformEnded).Pick);
        g.Do(Command.SignPlatform(g.Id, 3));
        Assert.Equal(3, g.Me.PlatPlat);
        Assert.Equal(g.C.Platforms.CutC, g.Me.PlatCut);
    }

    [Fact]
    public void A_live_deal_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(25);
        g.Weeks(25);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 95, g.Sim.Save());
        Assert.DoesNotContain("platPlat", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).PlatPlat);
    }

    [Fact]
    public void A_rival_deal_and_bad_data_are_caught_and_the_config_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.PlatPlat = 1;
        rival.PlatLeft = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("platform"));
        rival.PlatPlat = 0;
        rival.PlatLeft = 0;
        g.Me.PlatCut = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("platform"));
        g.Me.PlatCut = 0;
        g.Me.PlatPlat = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("platform"));
        g.Me.PlatPlat = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Platforms.Enabled = 2;
        c.Platforms.MinReputation = 2000;
        c.Platforms.LiftB = 5000;
        c.Platforms.MaxCut = 100;
        c.Platforms.WindowWeeks = c.Platforms.TermWeeks;
        c.Platforms.HoldupBar = 2000;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minReputation", "liftB", "maxCut", "windowWeeks", "holdupBar" })
            Assert.Contains(problems, p => p.StartsWith("platforms.") && p.Contains(key));
    }
}
