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

/// <summary>Three more of the bible's trait ideas: posts through it, will quit by blog post, only works on what is interesting.</summary>
public class MoreTraitsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 3, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static StaffMember Add(Game g, string trait, int morale = -1, string role = "researcher")
    {
        var p = new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = trait, Morale = morale < 0 ? g.C.StaffPeople.MoraleBaseline : morale };
        g.Me.Staff.Add(p);
        return p;
    }

    static long Research(Game g)
    {
        long before = g.Me.ResearchPoints;
        TrainingSystem.RunResearch(g.C, g.Me, g.Tick);
        return g.Me.ResearchPoints - before;
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the data

    [Fact]
    public void The_three_traits_are_in_the_data_with_their_one_job_each()
    {
        StaffPeopleConfig s = TestData.Config().StaffPeople;
        StaffTraitConfig posts = s.Trait("posts-through-it");
        StaffTraitConfig quits = s.Trait("quits-by-blog-post");
        StaffTraitConfig interesting = s.Trait("only-interesting");
        Assert.True(posts.BuzzPermille > 0 && posts.ScandalPermille > 0, "posts add to launches and to scandals");
        Assert.True(quits.QuitBelowMorale > 0 && quits.QuitBelowMorale < s.MoraleBaseline);
        Assert.True(interesting.InterestingPermille > 0);
        Assert.All(new[] { posts, quits, interesting }, t => Assert.True(t.HirePermille > 0 && !t.Star && !string.IsNullOrEmpty(t.Blurb)));
        Assert.Equal(0, s.Trait("steady").BuzzPermille + s.Trait("steady").ScandalPermille + s.Trait("steady").QuitBelowMorale + s.Trait("steady").InterestingPermille);
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }

    [Fact]
    public void The_validator_catches_bad_numbers_on_them()
    {
        AssertProblem(Problems(c => c.StaffPeople.Trait("posts-through-it").BuzzPermille = 501), "buzzPermille");
        AssertProblem(Problems(c => c.StaffPeople.Trait("posts-through-it").ScandalPermille = -1), "scandalPermille");
        AssertProblem(Problems(c => c.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale = c.StaffPeople.MoraleBaseline + 1), "quitBelowMorale");
        AssertProblem(Problems(c => c.StaffPeople.Trait("only-interesting").InterestingPermille = 901), "interestingPermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxPostsPermille = 1001), "maxPostsPermille");
        AssertProblem(Problems(c => c.StaffPeople.QuitReputation = -1), "quitReputation");
    }

    // ---- posts through it

    [Fact]
    public void Posters_add_to_launch_buzz_and_to_what_a_scandal_costs_up_to_a_cap()
    {
        Game g = Lab(c => c.StaffPeople.MaxPostsPermille = 200);
        TraitReport none = Traits.Report(g.C, g.Me);
        Assert.Equal(0, none.BuzzPermille + none.ScandalPermille);
        Add(g, "posts-through-it");
        TraitReport one = Traits.Report(g.C, g.Me);
        Assert.Equal(g.C.StaffPeople.Trait("posts-through-it").BuzzPermille, one.BuzzPermille);
        Assert.Equal(g.C.StaffPeople.Trait("posts-through-it").ScandalPermille, one.ScandalPermille);
        Add(g, "posts-through-it");
        Add(g, "posts-through-it");
        TraitReport three = Traits.Report(g.C, g.Me);
        Assert.Equal(200, three.BuzzPermille);
        Assert.Equal(200, three.ScandalPermille);
    }

    [Fact]
    public void A_poster_makes_a_scandal_cost_more_and_a_launch_louder()
    {
        Game g = Lab();
        int loss = 100;
        int plainLoss = PressSystem.Damp(g.C, g.Me, loss);
        int plainBuzz = PressSystem.Buzz(g.C, g.Me);
        Assert.Equal(loss, plainLoss);
        Add(g, "posts-through-it");
        StaffTraitConfig t = g.C.StaffPeople.Trait("posts-through-it");
        Assert.Equal(loss + loss * t.ScandalPermille / 1000, PressSystem.Damp(g.C, g.Me, loss));
        Assert.Equal(plainBuzz + t.BuzzPermille, PressSystem.Buzz(g.C, g.Me));
        Assert.True(PressSystem.LaunchPoints(g.C, g.Me) > 0);
    }

    [Fact]
    public void Nothing_changes_for_a_lab_with_no_posters()
    {
        Game g = Lab();
        Add(g, "steady");
        Assert.Equal(100, PressSystem.Damp(g.C, g.Me, 100));
        Assert.Equal(0, PressSystem.Buzz(g.C, g.Me));
        Assert.Equal(0, PressSystem.Damp(g.C, g.Me, 0));
    }

    // ---- will quit by blog post

    [Fact]
    public void Someone_below_their_line_quits_costs_reputation_and_is_logged()
    {
        Game g = Lab();
        StaffMember quitter = Add(g, "quits-by-blog-post", 100);
        StaffMember steady = Add(g, "steady", 0);
        StaffMember happy = Add(g, "quits-by-blog-post", g.C.StaffPeople.Trait("quits-by-blog-post").QuitBelowMorale);
        g.Me.Reputation = 500;
        int left = Traits.RunQuits(g.C, g.State, g.Me);
        Assert.Equal(1, left);
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == quitter.Id);
        Assert.Contains(g.Me.Staff, p => p.Id == steady.Id);
        Assert.Contains(g.Me.Staff, p => p.Id == happy.Id);
        Assert.Equal(500 - g.C.StaffPeople.QuitReputation, g.Me.Reputation);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.StaffQuit);
        Assert.Equal(quitter.Id, e.SubjectId);
        Assert.Equal(g.C.StaffPeople.QuitReputation, e.Amount);
        Assert.Equal("researcher|quits-by-blog-post", e.Ref);
        string text = EventText.Describe(g.C, g.State, e);
        Assert.Contains("blog post", text);
        Assert.Equal(0, Traits.RunQuits(g.C, g.State, g.Me));
    }

    [Fact]
    public void Reputation_never_goes_below_nothing_and_the_press_room_can_damp_the_post()
    {
        Game g = Lab();
        Add(g, "quits-by-blog-post", 0);
        g.Me.Reputation = 5;
        Traits.RunQuits(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.Reputation);

        Game twin = Lab(c => c.StaffPeople.QuitReputation = 100);
        Add(twin, "posts-through-it");
        Add(twin, "quits-by-blog-post", 0);
        twin.Me.Reputation = 500;
        Traits.RunQuits(twin.C, twin.State, twin.Me);
        Assert.Equal(500 - PressSystem.Damp(twin.C, twin.Me, 100), twin.Me.Reputation);
        Assert.True(twin.Me.Reputation < 400, "a poster in the room makes the blog post louder");
    }

    [Fact]
    public void A_week_of_sim_lets_the_unhappy_one_go()
    {
        Game g = Lab();
        StaffMember quitter = Add(g, "quits-by-blog-post", 0);
        g.Weeks(1);
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == quitter.Id);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.StaffQuit && e.SubjectId == quitter.Id);
    }

    [Fact]
    public void A_content_one_stays_for_years()
    {
        Game g = Lab();
        StaffMember stayer = Add(g, "quits-by-blog-post");
        g.Weeks(104);
        Assert.Contains(g.Me.Staff, p => p.Id == stayer.Id);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.StaffQuit);
    }

    [Fact]
    public void An_open_offer_to_the_one_who_quit_goes_with_them()
    {
        Game g = Lab();
        StaffMember quitter = Add(g, "quits-by-blog-post", 0);
        g.Me.PoachOffers.Add(new PoachOffer { Id = g.State.TakeId(), StaffId = quitter.Id });
        Traits.RunQuits(g.C, g.State, g.Me);
        g.Weeks(1);
        Assert.DoesNotContain(g.Me.PoachOffers, o => o.StaffId == quitter.Id);
    }

    [Fact]
    public void The_chat_has_a_line_for_a_quit()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Chat.Lines, l => l.Kind == "StaffQuit");
    }

    // ---- only works on what is interesting

    static ModelState Run(Game g, string approach)
    {
        g.WithStarterLab(researchers: 1);
        ModelState m = g.Train("mini", 600);
        if (approach != null) g.Do(Command.SetRunApproach(g.Id, m.Id, approach));
        return m;
    }

    [Fact]
    public void They_do_more_while_a_variant_run_trains_and_less_otherwise()
    {
        Game g = Lab();
        g.WithStarterLab(researchers: 0);
        foreach (StaffMember p in g.Me.Staff.ToList()) p.TraitId = "steady";
        Add(g, "only-interesting");
        long boring = Research(g);
        ModelState m = g.Train("mini", 600);
        Assert.False(Traits.HasSomethingInteresting(g.C, g.Me));
        Assert.Equal(boring, Research(g));
        g.Do(Command.SetRunApproach(g.Id, m.Id, "experimental"));
        Assert.True(Traits.HasSomethingInteresting(g.C, g.Me));
        long keen = Research(g);

        Game twin = Lab();
        twin.WithStarterLab(researchers: 0);
        Add(twin, "steady");
        long normal = Research(twin);

        int shift = g.C.StaffPeople.Trait("only-interesting").InterestingPermille;
        Assert.True(keen > normal && normal > boring);
        Assert.InRange(boring, normal * (1000 - shift) / 1000 - 3, normal * (1000 - shift) / 1000 + 3);
        Assert.InRange(keen, normal * (1000 + shift) / 1000 - 3, normal * (1000 + shift) / 1000 + 3);
    }

    [Fact]
    public void Choosing_the_known_design_again_makes_the_work_boring_again()
    {
        Game g = Lab();
        ModelState m = Run(g, "experimental");
        Add(g, "only-interesting");
        Assert.True(Traits.HasSomethingInteresting(g.C, g.Me));
        g.Do(Command.SetRunApproach(g.Id, m.Id, "standard"));
        Assert.False(Traits.HasSomethingInteresting(g.C, g.Me));
    }

    [Fact]
    public void A_finished_variant_run_no_longer_counts()
    {
        Game g = Lab();
        ModelState m = Run(g, "experimental");
        Assert.True(Traits.HasSomethingInteresting(g.C, g.Me));
        g.UntilTrained(m);
        Assert.False(Traits.HasSomethingInteresting(g.C, g.Me));
    }

    [Fact]
    public void Other_traits_and_other_roles_feel_nothing()
    {
        Game g = Lab();
        Run(g, "experimental");
        Assert.Equal(0, Traits.InterestingShift(g.C, g.Me, g.C.StaffPeople.Trait("steady")));
        Assert.Equal(0, Traits.InterestingShift(g.C, g.Me, null));
        Game twin = Lab();
        Run(twin, null);
        Add(g, "only-interesting", -1, "engineer");
        Add(twin, "only-interesting", -1, "engineer");
        Assert.Equal(Research(twin), Research(g));
    }
}
