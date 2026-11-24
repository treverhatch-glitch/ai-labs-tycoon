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

/// <summary>Version naming rot: every new version makes the names harder to tell apart, customers notice, and the marketing team can tidy up.</summary>
public class VersionNamingTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Training.MaxConcurrentRuns = 40;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static ModelState Start(Game g)
    {
        return g.Train("mini", 600);
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

    // ---- the table

    [Fact]
    public void The_table_has_ten_words_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(10, c.Naming.Suffixes.Count);
        Assert.Equal("Pro", c.Naming.Suffixes[0]);
        Assert.Equal(70, c.Naming.MessPerVersion);
        Assert.Equal(1000, c.Naming.MaxMess);
        Assert.Equal(80, c.Naming.ConfusionPermille);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_new_lab_has_tidy_names_and_customers_are_not_confused()
    {
        Game g = Lab();
        Assert.Equal(0, g.Me.NamingMess);
        Assert.Equal(0, VersionNaming.SuffixCount(g.C, g.Me));
        Assert.Equal(1000, VersionNaming.AppealPermille(g.C, g.Me));
        var report = VersionNaming.Of(g.C, g.Me, g.Tick);
        Assert.Equal(0, report.Mess);
        Assert.Equal(0, report.ConfusionPermille);
        Assert.Equal(0, report.CleanPoints);
    }

    // ---- the names

    [Fact]
    public void The_first_versions_are_named_plainly()
    {
        Game g = Lab();
        ModelState a = Start(g);
        ModelState b = Start(g);
        Assert.Equal(g.Me.FamilyName + " Mini 1", a.Name);
        Assert.Equal(g.Me.FamilyName + " Mini 2", b.Name);
    }

    [Fact]
    public void Each_new_run_adds_mess_and_the_third_version_gets_its_first_word()
    {
        Game g = Lab();
        Start(g);
        Assert.Equal(70, g.Me.NamingMess);
        Start(g);
        Assert.Equal(140, g.Me.NamingMess);
        ModelState third = Start(g);
        Assert.EndsWith("Mini 3 Pro", third.Name);
        Assert.Equal(210, g.Me.NamingMess);
    }

    [Fact]
    public void Words_stack_up_in_order()
    {
        Game g = Lab();
        ModelState last = null;
        for (int i = 0; i < 8; i++) last = Start(g);
        // Run 8 is named at 7 * 70 = 490 mess: four words.
        Assert.EndsWith("Mini 8 Pro Max Turbo Ultra", last.Name);
    }

    [Fact]
    public void The_words_run_out_and_the_list_holds_at_its_end()
    {
        Game g = Lab(c => { c.Naming.MessPerVersion = 1000; c.Naming.SuffixEveryPoints = 100; });
        ModelState a = Start(g);
        ModelState b = Start(g);
        Assert.Equal(1000, g.Me.NamingMess);
        Assert.EndsWith("Mini 2 Pro Max Turbo Ultra Plus v2 Final (new) Final-FINAL Reborn", b.Name);
        ModelState c = Start(g);
        Assert.Equal(1000, g.Me.NamingMess);
        Assert.EndsWith("Mini 3 Pro Max Turbo Ultra Plus v2 Final (new) Final-FINAL Reborn", c.Name);
        Assert.Equal(g.Me.FamilyName + " Mini 1", a.Name);
    }

    [Fact]
    public void Mess_stops_at_the_cap()
    {
        Game g = Lab(c => { c.Naming.MessPerVersion = 700; });
        Start(g); Start(g); Start(g);
        Assert.Equal(1000, g.Me.NamingMess);
    }

    [Fact]
    public void A_name_preview_matches_the_name_the_next_run_gets()
    {
        Game g = Lab();
        for (int i = 0; i < 4; i++) Start(g);
        ModelLineConfig line = g.C.Line("mini");
        string preview = VersionNaming.NameFor(g.C, g.Me, line, LabQueries.NextVersion(g.Me, "mini"));
        ModelState next = Start(g);
        Assert.Equal(preview, next.Name);
    }

    [Fact]
    public void Other_lines_share_the_same_mess()
    {
        Game g = Lab();
        Start(g); Start(g); Start(g);
        Assert.Equal(210, g.Me.NamingMess);
        string preview = VersionNaming.NameFor(g.C, g.Me, g.C.Line("mid"), 1);
        Assert.EndsWith("Mid 1 Pro Max", preview);
    }

    [Fact]
    public void A_run_that_is_refused_adds_no_mess()
    {
        Game g = Lab(c => c.Training.MaxConcurrentRuns = 1);
        Start(g);
        int before = g.Me.NamingMess;
        g.Refused(Command.StartTraining(g.Id, "mini", 600), CommandError.TooManyRuns);
        Assert.Equal(before, g.Me.NamingMess);
    }

    // ---- confusion

    [Theory]
    [InlineData(0, 1000)]
    [InlineData(500, 960)]
    [InlineData(1000, 920)]
    public void Confusion_grows_with_the_mess(int mess, int keeps)
    {
        Game g = Lab();
        g.Me.NamingMess = mess;
        Assert.Equal(keeps, VersionNaming.AppealPermille(g.C, g.Me));
        Assert.Equal(1000 - keeps, VersionNaming.Of(g.C, g.Me, g.Tick).ConfusionPermille);
    }

    [Fact]
    public void Confusion_is_its_own_part_of_the_appeal_and_the_parts_still_add_up()
    {
        Game g = Lab();
        ModelState m = Start(g);
        g.UntilTrained(m);
        m.Capability = 1_000_000;
        m.Alignment = 800;
        m.DisplayedAlignment = 800;
        g.Release(m, "game-ai");
        SectorConfig sector = g.C.Sector("game-ai");
        SectorState state = g.State.Sector("game-ai");

        g.Me.NamingMess = 0;
        var tidy = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        Assert.Equal(0, tidy.NamingConfusion);

        g.Me.NamingMess = 1000;
        var messy = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        Assert.True(messy.NamingConfusion < 0);
        Assert.InRange(messy.Total, tidy.Total * 920 / 1000 - 5, tidy.Total * 920 / 1000 + 5);
        foreach (var parts in new[] { tidy, messy })
            Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost + parts.NamingConfusion, parts.Total);
    }

    [Fact]
    public void Confusion_costs_the_lab_its_share_against_a_rival()
    {
        const string Chat = "consumer-assistant";
        Func<int, long> sales = mess =>
        {
            var options = new NewGameOptions { Seed = 7, StartEraId = "chat-boom" };
            var g = new Game(Simulation.NewGame(TestData.Config(c => c.RolePerks.Clear()), options));
            foreach (LabState lab in g.State.Labs) lab.Models.Clear();
            foreach (SectorState sector in g.State.Sectors) sector.Entries.Clear();
            LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
            foreach (LabState lab in new[] { g.Me, rival })
            {
                var m = new ModelState
                {
                    Id = 9000 + lab.Id, LineId = "mini", Version = 1, Name = lab.Name, Status = ModelStatus.Released, Capability = 1000,
                    Alignment = 1000, DisplayedAlignment = 1000, ReleasedTick = g.Tick, LastIncidentTick = -1
                };
                lab.Models.Add(m);
                m.Sectors.Add(Chat);
                g.State.Sector(Chat).Entries.Add(new SectorEntry { LabId = lab.Id, ModelId = m.Id });
            }
            g.Me.NamingMess = mess;
            g.Weeks(8);
            return g.Me.Ledger.Lifetime.Revenue;
        };
        long tidy = sales(0), messy = sales(1000);
        Assert.True(tidy > messy, tidy + " vs " + messy);
    }

    // ---- the clean-up

    [Fact]
    public void A_clean_up_takes_four_hundred_points_off_and_costs_marketing_time()
    {
        Game g = Lab();
        g.Me.NamingMess = 900;
        long cash = g.Me.Cash;
        long cost = VersionNaming.CleanCost(g.C, g.Tick);
        Assert.True(cost > 0);
        g.Do(Command.CleanUpNaming(g.Id));
        Assert.Equal(500, g.Me.NamingMess);
        Assert.Equal(cash - cost, g.Me.Cash);
    }

    [Fact]
    public void A_clean_up_costs_four_sales_rep_hiring_fees_of_the_era()
    {
        Game g = Lab();
        RoleConfig sales = g.C.Role("sales");
        Assert.Equal(EraRules.HireFee(g.C, sales, g.Tick) * 4, VersionNaming.CleanCost(g.C, g.Tick));
    }

    [Fact]
    public void A_clean_up_cannot_take_the_mess_below_zero()
    {
        Game g = Lab();
        g.Me.NamingMess = 150;
        g.Do(Command.CleanUpNaming(g.Id));
        Assert.Equal(0, g.Me.NamingMess);
    }

    [Fact]
    public void A_clean_up_is_refused_when_the_names_are_already_tidy_and_changes_nothing()
    {
        Game g = Lab();
        g.Refused(Command.CleanUpNaming(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_clean_up_is_refused_without_the_cash_and_changes_nothing()
    {
        Game g = Lab();
        g.Me.NamingMess = 500;
        g.Me.Cash = 0;
        g.Refused(Command.CleanUpNaming(g.Id), CommandError.NotEnoughCash);
        Assert.Equal(500, g.Me.NamingMess);
    }

    [Fact]
    public void After_a_clean_up_the_next_name_is_shorter()
    {
        Game g = Lab();
        for (int i = 0; i < 6; i++) Start(g);
        ModelState before = Start(g);
        Assert.Contains("Pro Max Turbo", before.Name);
        g.Do(Command.CleanUpNaming(g.Id));
        g.Do(Command.CleanUpNaming(g.Id));
        ModelState after = Start(g);
        Assert.EndsWith("Mini 8", after.Name);
    }

    [Fact]
    public void The_clean_up_spends_on_the_ledger_so_cash_is_still_explained()
    {
        Game g = Lab();
        g.Me.NamingMess = 800;
        g.Do(Command.CleanUpNaming(g.Id));
        g.Weeks(2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    // ---- rivals

    [Fact]
    public void Rivals_keep_plain_names_and_no_confusion()
    {
        Game g = Game.New(tweak: c => c.Training.MaxConcurrentRuns = 40);
        g.Years(3);
        foreach (LabState lab in g.State.Labs.Where(l => l.Controller != LabController.Player))
        {
            Assert.Equal(0, lab.NamingMess);
            Assert.Equal(1000, VersionNaming.AppealPermille(g.C, lab));
            foreach (ModelState m in lab.Models) Assert.Equal(lab.FamilyName + " " + g.C.Line(m.LineId).Name + " " + m.Version, m.Name);
        }
    }

    [Fact]
    public void A_rival_lab_given_mess_still_feels_none()
    {
        Game g = Game.New();
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        rival.NamingMess = 900;
        Assert.Equal(1000, VersionNaming.AppealPermille(g.C, rival));
        Assert.Equal(rival.FamilyName + " Mini 5", VersionNaming.NameFor(g.C, rival, g.C.Line("mini"), 5));
    }

    // ---- the validator

    [Fact]
    public void The_validator_names_each_naming_number_that_is_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.Naming.MaxMess = 0), "naming.maxMess");
        AssertProblem(Problems(c => c.Naming.MaxMess = 1001), "naming.maxMess");
        AssertProblem(Problems(c => c.Naming.MessPerVersion = -1), "naming.messPerVersion");
        AssertProblem(Problems(c => c.Naming.MessPerVersion = 1001), "naming.messPerVersion");
        AssertProblem(Problems(c => c.Naming.SuffixEveryPoints = 0), "naming.suffixEveryPoints");
        AssertProblem(Problems(c => c.Naming.ConfusionPermille = -1), "naming.confusionPermille");
        AssertProblem(Problems(c => c.Naming.ConfusionPermille = 901), "naming.confusionPermille");
        AssertProblem(Problems(c => c.Naming.CleanPoints = -1), "naming.cleanPoints");
        AssertProblem(Problems(c => c.Naming.CleanFeeHireFees = -1), "naming.cleanFeeHireFees");
        AssertProblem(Problems(c => c.Naming.Suffixes.Clear()), "naming.suffixes is empty");
        AssertProblem(Problems(c => c.Naming.Suffixes[2] = " "), "empty word");
    }

    // ---- saving

    [Fact]
    public void The_mess_survives_a_save_and_a_load()
    {
        Game g = Lab();
        for (int i = 0; i < 4; i++) Start(g);
        g.Weeks(3);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(280, again.Player.NamingMess);
        for (int i = 0; i < 20; i++) { g.Sim.Step(); again.Step(); }
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_tidy_names()
    {
        Game g = Lab();
        g.Weeks(10);
        string old = SaveSchemaPretend.At(g.C, 28, g.Sim.Save());
        Assert.DoesNotContain("namingMess", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        Assert.All(loaded.State.Labs, lab => Assert.Equal(0, lab.NamingMess));
    }

    [Fact]
    public void A_save_with_a_mess_outside_the_range_is_reported()
    {
        Game g = Lab();
        g.Me.NamingMess = 1001;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("naming mess"));
        g.Me.NamingMess = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("naming mess"));
    }

    [Fact]
    public void Two_runs_of_the_same_game_land_on_the_same_digest()
    {
        Func<string> run = () =>
        {
            Game g = Lab();
            for (int i = 0; i < 5; i++) Start(g);
            g.Do(Command.CleanUpNaming(g.Id));
            g.Weeks(5);
            return g.Sim.Digest();
        };
        Assert.Equal(run(), run());
    }
}
