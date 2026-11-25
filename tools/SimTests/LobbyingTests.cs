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

/// <summary>Paying a government to hear the lab out: a quiet word, a subsidy push, a cooldown and a scandal that can follow.</summary>
public class LobbyingTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool policy = false)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Office.ChancePermille = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        if (policy) { g.Build("policy-office", 4, 3); g.Hire("policy", 2); }
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static string Home(Game g) { return g.Me.HomeBlocId; }

    static BlocStanding Standing(Game g, string bloc = null) { return g.Me.Bloc(bloc ?? Home(g)); }

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
    public void The_shipped_numbers_validate_and_lobbying_is_a_gamble_with_a_floor()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Lobbying.BackfirePermille > c.Lobbying.BackfireFloorPermille && c.Lobbying.BackfireFloorPermille > 0, "even a well-staffed push can go wrong");
        Assert.True(c.Lobbying.SubsidyReturnWeeks > c.Lobbying.SubsidyCostWeeks, "a won subsidy pays for the push");
        Assert.True(c.Lobbying.SubsidyChanceCapPermille < MathI.Permille, "a subsidy is never certain");
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_lobbying_data()
    {
        AssertProblem(Problems(c => c.Lobbying.QuietCostWeeks = 0), "lobbying.quietCostWeeks");
        AssertProblem(Problems(c => c.Lobbying.QuietAttention = 0), "lobbying.quietAttention");
        AssertProblem(Problems(c => c.Lobbying.SubsidyCostWeeks = 0), "lobbying.subsidyCostWeeks");
        AssertProblem(Problems(c => c.Lobbying.SubsidyReturnWeeks = 0), "lobbying.subsidyReturnWeeks");
        AssertProblem(Problems(c => c.Lobbying.CooldownWeeks = -1), "lobbying.cooldownWeeks");
        AssertProblem(Problems(c => c.Lobbying.SubsidyChancePermille = 1001), "lobbying.subsidyChancePermille");
        AssertProblem(Problems(c => c.Lobbying.SubsidyChanceCapPermille = 100), "subsidyChanceCapPermille");
        AssertProblem(Problems(c => c.Lobbying.BackfirePermille = 2000), "lobbying.backfirePermille");
        AssertProblem(Problems(c => c.Lobbying.BackfireFloorPermille = 999), "backfireFloorPermille");
        AssertProblem(Problems(c => c.Lobbying.BackfireAttention = 1001), "lobbying.backfireAttention");
        AssertProblem(Problems(c => c.Lobbying.BackfireReputation = -1), "lobbying.backfireReputation");
        AssertProblem(Problems(c => c.Lobbying.SubsidyChancePermillePerStaff = -1), "subsidyChancePermillePerStaff");
        AssertProblem(Problems(c => c.Lobbying.BackfireSavingPermillePerStaff = -1), "backfireSavingPermillePerStaff");
        AssertProblem(Problems(c => c.Lobbying.MinWeekCost = -1), "lobbying.minWeekCost");
        AssertProblem(Problems(c => c.Lobbying = null), "lobbying is missing");
    }

    // ---- a quiet word

    [Fact]
    public void A_quiet_word_takes_attention_off_and_costs_weeks_of_running_the_lab()
    {
        Game g = Lab();
        Standing(g).Attention = 600;
        long cash = g.Me.Cash;
        long cost = Lobbying.Cost(g.C, g.State, g.Me, Lobbying.Quiet);
        Assert.Equal(g.C.Lobbying.QuietCostWeeks * Lobbying.WeekCost(g.C, g.State, g.Me), cost);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(450, Standing(g).Attention);
        Assert.Equal(cash - cost, g.Me.Cash);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.Lobbied);
        Assert.Equal(cost, e.Amount);
        Assert.Equal(Home(g) + "|quiet|landed", e.Ref);
        Assert.Contains("lobbied", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_quiet_word_never_takes_attention_below_nothing()
    {
        Game g = Lab();
        Standing(g).Attention = 40;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(0, Standing(g).Attention);
    }

    [Fact]
    public void A_quiet_word_with_nothing_to_quieten_is_refused_and_costs_nothing()
    {
        Game g = Lab();
        Standing(g).Attention = 0;
        g.Refused(Command.Lobby(g.Id, Home(g), Lobbying.Quiet), CommandError.InvalidArgument);
    }

    [Fact]
    public void Any_government_can_be_lobbied_not_only_the_home_one()
    {
        Game g = Lab();
        string other = g.C.World.Blocs.First(b => b.Id != Home(g)).Id;
        Standing(g, other).Attention = 500;
        g.Do(Command.Lobby(g.Id, other, Lobbying.Quiet));
        Assert.Equal(350, Standing(g, other).Attention);
        Assert.True(Standing(g).LobbyUntilTick == 0, "only the lobbied government is on cooldown");
    }

    // ---- a subsidy push

    [Fact]
    public void A_subsidy_that_is_won_pays_more_than_the_push_cost()
    {
        Game g = Lab(c => { c.Lobbying.SubsidyChancePermille = 1000; c.Lobbying.SubsidyChanceCapPermille = 1000; });
        long cash = g.Me.Cash;
        long week = Lobbying.WeekCost(g.C, g.State, g.Me);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Subsidy));
        Assert.Equal(cash + (g.C.Lobbying.SubsidyReturnWeeks - g.C.Lobbying.SubsidyCostWeeks) * week, g.Me.Cash);
        Assert.Equal(Home(g) + "|subsidy|landed", g.State.Events.Last(x => x.Kind == EventKind.Lobbied).Ref);
    }

    [Fact]
    public void A_subsidy_that_is_missed_costs_the_push_and_nothing_else()
    {
        Game g = Lab(c => { c.Lobbying.SubsidyChancePermille = 0; c.Lobbying.SubsidyChanceCapPermille = 0; });
        Standing(g).Attention = 300;
        long cash = g.Me.Cash;
        int rep = g.Me.Reputation;
        long cost = Lobbying.Cost(g.C, g.State, g.Me, Lobbying.Subsidy);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Subsidy));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(300, Standing(g).Attention);
        Assert.Equal(rep, g.Me.Reputation);
        Assert.Equal(Home(g) + "|subsidy|missed", g.State.Events.Last(x => x.Kind == EventKind.Lobbied).Ref);
    }

    [Fact]
    public void Policy_staff_lift_the_subsidy_odds_up_to_the_cap_and_cut_the_backfire_odds_down_to_the_floor()
    {
        Game plain = Lab();
        Game staffed = Lab(null, true);
        int staff = WorldQueries.PolicyStaff(staffed.C, staffed.Me);
        Assert.True(staff >= 2);
        Assert.Equal(plain.C.Lobbying.SubsidyChancePermille, Lobbying.SubsidyChancePermille(plain.C, plain.Me));
        Assert.Equal(plain.C.Lobbying.SubsidyChancePermille + staff * plain.C.Lobbying.SubsidyChancePermillePerStaff, Lobbying.SubsidyChancePermille(staffed.C, staffed.Me));
        Game capped = Lab(c => { c.Lobbying.SubsidyChanceCapPermille = c.Lobbying.SubsidyChancePermille + 10; }, true);
        Assert.Equal(capped.C.Lobbying.SubsidyChanceCapPermille, Lobbying.SubsidyChancePermille(capped.C, capped.Me));

        Game risky = Lab(c => { c.Lobbying.BackfirePermille = 200; c.Lobbying.BackfireFloorPermille = 30; });
        Game riskyStaffed = Lab(c => { c.Lobbying.BackfirePermille = 200; c.Lobbying.BackfireFloorPermille = 30; }, true);
        Assert.Equal(200, Lobbying.BackfireChancePermille(risky.C, risky.Me));
        Assert.Equal(200 - staff * risky.C.Lobbying.BackfireSavingPermillePerStaff, Lobbying.BackfireChancePermille(riskyStaffed.C, riskyStaffed.Me));
        Game floored = Lab(c => { c.Lobbying.BackfirePermille = 100; c.Lobbying.BackfireFloorPermille = 90; }, true);
        Assert.Equal(90, Lobbying.BackfireChancePermille(floored.C, floored.Me));
    }

    // ---- a scandal

    [Fact]
    public void A_backfire_adds_attention_and_costs_reputation_instead_of_helping()
    {
        Game g = Lab(c => { c.Lobbying.BackfirePermille = 1000; c.Lobbying.BackfireFloorPermille = 1000; });
        g.Me.Reputation = 500;
        Standing(g).Attention = 100;
        long cash = g.Me.Cash;
        long cost = Lobbying.Cost(g.C, g.State, g.Me, Lobbying.Quiet);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(100 + g.C.Lobbying.BackfireAttention, Standing(g).Attention);
        Assert.Equal(500 - PressSystem.Damp(g.C, g.Me, g.C.Lobbying.BackfireReputation), g.Me.Reputation);
        Assert.Equal(cash - cost, g.Me.Cash);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.Lobbied);
        Assert.EndsWith("backfired", e.Ref);
        Assert.Contains("scandal", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_backfire_ruins_a_subsidy_push_too()
    {
        Game g = Lab(c => { c.Lobbying.BackfirePermille = 1000; c.Lobbying.BackfireFloorPermille = 1000; c.Lobbying.SubsidyChancePermille = 1000; c.Lobbying.SubsidyChanceCapPermille = 1000; });
        long cash = g.Me.Cash;
        long cost = Lobbying.Cost(g.C, g.State, g.Me, Lobbying.Subsidy);
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Subsidy));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.EndsWith("backfired", g.State.Events.Last(x => x.Kind == EventKind.Lobbied).Ref);
    }

    [Fact]
    public void The_outcome_is_worked_out_without_the_shared_dice()
    {
        Game g = Lab(c => { c.Lobbying.BackfirePermille = 500; c.Lobbying.BackfireFloorPermille = 500; });
        Standing(g).Attention = 300;
        string before = g.Sim.Save();
        string rngBefore = Json.Parse(before).Get("rng").ToString();
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(rngBefore, Json.Parse(g.Sim.Save()).Get("rng").ToString());
    }

    [Fact]
    public void The_same_week_gives_the_same_answer_and_the_answers_vary_across_weeks()
    {
        var results = new HashSet<string>();
        for (int w = 0; w < 40; w++)
        {
            Game g = Lab(c => { c.Lobbying.BackfirePermille = 500; c.Lobbying.BackfireFloorPermille = 500; });
            g.Weeks(w);
            Standing(g).Attention = 300;
            Game twin = Lab(c => { c.Lobbying.BackfirePermille = 500; c.Lobbying.BackfireFloorPermille = 500; });
            twin.Weeks(w);
            Standing(twin).Attention = 300;
            g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
            twin.Do(Command.Lobby(twin.Id, Home(twin), Lobbying.Quiet));
            string r = g.State.Events.Last(x => x.Kind == EventKind.Lobbied).Ref;
            Assert.Equal(r, twin.State.Events.Last(x => x.Kind == EventKind.Lobbied).Ref);
            results.Add(r.Split('|')[2]);
        }
        Assert.Equal(2, results.Count);
    }

    // ---- the cooldown

    [Fact]
    public void A_government_cannot_be_lobbied_again_until_the_cooldown_ends()
    {
        Game g = Lab();
        Standing(g).Attention = 900;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Equal(g.State.Tick + g.C.Lobbying.CooldownWeeks, Standing(g).LobbyUntilTick);
        g.Refused(Command.Lobby(g.Id, Home(g), Lobbying.Quiet), CommandError.LobbyCooling);
        g.Refused(Command.Lobby(g.Id, Home(g), Lobbying.Subsidy), CommandError.LobbyCooling);
        g.Weeks(g.C.Lobbying.CooldownWeeks);
        Standing(g).Attention = 900;
        Assert.True(g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet)).Ok);
    }

    [Fact]
    public void Lobbying_one_government_leaves_the_others_open()
    {
        Game g = Lab();
        string other = g.C.World.Blocs.First(b => b.Id != Home(g)).Id;
        Standing(g).Attention = 900;
        Standing(g, other).Attention = 900;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.True(g.Do(Command.Lobby(g.Id, other, Lobbying.Quiet)).Ok);
    }

    // ---- refusals

    [Fact]
    public void Bad_orders_are_refused_and_change_nothing()
    {
        Game g = Lab();
        Standing(g).Attention = 500;
        g.Refused(Command.Lobby(g.Id, "atlantis", Lobbying.Quiet), CommandError.UnknownId);
        g.Refused(Command.Lobby(g.Id, Home(g), "favours"), CommandError.UnknownId);
        g.Refused(Command.Lobby(g.Id, Home(g), ""), CommandError.UnknownId);
        g.Me.Cash = 10;
        g.Refused(Command.Lobby(g.Id, Home(g), Lobbying.Quiet), CommandError.NotEnoughCash);
        g.Refused(Command.Lobby(g.Id, Home(g), Lobbying.Subsidy), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Only_the_player_lobbies()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string> { TestData.Config().Rivals[0].Id } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        string before = g.Sim.Digest();
        CommandResult r = g.Sim.Apply(Command.Lobby(rival.Id, "us", Lobbying.Quiet));
        Assert.False(r.Ok);
        Assert.Equal(before, g.Sim.Digest());
    }

    // ---- nothing else moves

    [Fact]
    public void A_game_that_never_lobbies_is_exactly_as_it_was()
    {
        Game g = Lab();
        Game twin = Lab();
        g.Weeks(60);
        twin.Weeks(60);
        Assert.Equal(g.Sim.Digest(), twin.Sim.Digest());
        Assert.All(g.State.Labs, l => Assert.All(l.Blocs, b => Assert.Equal(0, b.LobbyUntilTick)));
    }

    [Fact]
    public void The_chat_has_a_lobbying_line()
    {
        Game g = Lab();
        Standing(g).Attention = 500;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        Assert.Contains(g.C.Chat.Lines, l => l.Kind == "Lobbied");
    }

    // ---- saving

    [Fact]
    public void The_cooldown_survives_a_save_and_the_game_carries_on_the_same()
    {
        Game g = Lab();
        Standing(g).Attention = 500;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        g.Weeks(5);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(Standing(g).LobbyUntilTick, again.Player.Bloc(Home(g)).LobbyUntilTick);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(40);
        again.StepMany(40);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_government_lobbied()
    {
        Game g = Lab();
        Standing(g).Attention = 500;
        g.Do(Command.Lobby(g.Id, Home(g), Lobbying.Quiet));
        string old = SaveSchemaPretend.At(g.C, 36, g.Sim.Save());
        Assert.DoesNotContain("lobbyUntilTick", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => Assert.All(l.Blocs, b => Assert.Equal(0, b.LobbyUntilTick)));
    }
}
