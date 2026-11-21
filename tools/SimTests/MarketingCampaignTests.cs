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

/// <summary>Marketing campaigns: a boost in one sector for some weeks, a hangover after it, and a collapse if the model has an incident in the meantime.</summary>
public class MarketingCampaignTests
{
    const string Sector = "game-ai";

    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, Sector, "game-bots"));
        return m;
    }

    static List<GameEvent> Runs(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.CampaignRun).ToList(); }

    static DisplayQueries.AppealBreakdown Appeal(Game g, ModelState m, string sectorId)
    {
        SectorConfig sector = g.C.Sector(sectorId);
        return DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector(sectorId), g.Me, m);
    }

    static void Run(Game g, int weeks) { for (int i = 0; i < weeks; i++) MarketingCampaign.RunWeek(g.C, g.State, g.Me); }

    static void Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
    }

    // ---- nothing happens by itself

    [Fact]
    public void No_campaign_runs_until_the_player_pays_for_one()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Weeks(30);
        Assert.Equal("", g.Me.CampaignSector);
        Assert.Equal(0, g.Me.CampaignLevel);
        Assert.False(MarketingCampaign.Running(g.Me));
        Assert.False(MarketingCampaign.Hungover(g.Me));
        Assert.Equal(1000, MarketingCampaign.AppealPermille(g.C, g.State, g.Me, Sector));
        Assert.Equal(0, Appeal(g, m, Sector).CampaignEffect);
        Assert.Empty(Runs(g));
    }

    // ---- the command

    [Fact]
    public void A_campaign_costs_the_budget_sets_the_state_and_logs_it()
    {
        Game g = Lab();
        Selling(g);
        long cash = g.Me.Cash;
        g.Do(Command.RunCampaign(g.Id, Sector, 2));
        Assert.Equal(cash - 80000, g.Me.Cash);
        Assert.Equal(Sector, g.Me.CampaignSector);
        Assert.Equal(2, g.Me.CampaignLevel);
        Assert.Equal(8, g.Me.CampaignWeeks);
        Assert.Equal(g.Tick + 26, g.Me.CampaignLockedUntil);
        GameEvent e = Runs(g).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(80000, e.Amount);
        Assert.Equal(Sector, e.Ref);
        Assert.Contains("started a level 2 marketing campaign in game-ai for $80000", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Bad_levels_unknown_sectors_and_sectors_the_lab_does_not_sell_in_are_refused_and_change_nothing()
    {
        Game g = Lab();
        Selling(g);
        long cash = g.Me.Cash;
        g.Refused(Command.RunCampaign(g.Id, Sector, 0), CommandError.InvalidArgument);
        g.Refused(Command.RunCampaign(g.Id, Sector, 4), CommandError.InvalidArgument);
        g.Refused(Command.RunCampaign(g.Id, "nowhere", 1), CommandError.UnknownId);
        g.Refused(Command.RunCampaign(g.Id, "expert-systems", 1), CommandError.NoModelForJob);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal("", g.Me.CampaignSector);
        Assert.Empty(Runs(g));
    }

    [Fact]
    public void A_lab_with_no_sales_people_a_poor_lab_a_rival_and_a_game_without_campaigns_refuse_it()
    {
        Game quiet = Lab();
        Selling(quiet);
        quiet.Me.Staff.RemoveAll(p => p.RoleId == "sales");
        quiet.Refused(Command.RunCampaign(quiet.Id, Sector, 1), CommandError.NotEnoughStaff);

        Game poor = Lab(rich: false);
        Selling(poor);
        poor.Me.Cash = 39_999;
        poor.Refused(Command.RunCampaign(poor.Id, Sector, 1), CommandError.NotEnoughCash);
        Assert.Equal("", poor.Me.CampaignSector);

        Game game = Game.New(1);
        LabState rival = game.State.Labs.First(l => l.Id != game.State.PlayerLabId);
        game.Refused(Command.RunCampaign(rival.Id, Sector, 1), CommandError.InvalidArgument);

        Game off = Lab(c => c.Marketing.MaxLevel = 0);
        Selling(off);
        off.Refused(Command.RunCampaign(off.Id, Sector, 1), CommandError.InvalidArgument);
        Assert.False(MarketingCampaign.Enabled(off.C));

        Game broke = Lab();
        Selling(broke);
        broke.Me.Bankrupt = true;
        broke.Refused(Command.RunCampaign(broke.Id, Sector, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void Only_one_campaign_at_a_time_and_a_cooldown_after_it()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        g.Refused(Command.RunCampaign(g.Id, "game-bots", 1), CommandError.InvalidArgument);
        Run(g, 8);
        Assert.True(MarketingCampaign.Hungover(g.Me));
        g.Refused(Command.RunCampaign(g.Id, "game-bots", 1), CommandError.InvalidArgument);
        Run(g, 8);
        Assert.Equal("", g.Me.CampaignSector);
        g.Refused(Command.RunCampaign(g.Id, "game-bots", 1), CommandError.CampaignLocked);
        g.Weeks(26);
        Assert.True(g.Tick >= g.Me.CampaignLockedUntil);
        g.Do(Command.RunCampaign(g.Id, "game-bots", 1));
        Assert.Equal("game-bots", g.Me.CampaignSector);
    }

    // ---- what a campaign does

    [Fact]
    public void While_it_runs_the_model_is_more_appealing_in_that_sector_only_and_the_breakdown_shows_it()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        long before = Appeal(g, m, Sector).Total;
        long beforeOther = Appeal(g, m, "game-bots").Total;
        g.Do(Command.RunCampaign(g.Id, Sector, 2));
        DisplayQueries.AppealBreakdown parts = Appeal(g, m, Sector);
        int boost = MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector);
        Assert.True(boost > 0);
        Assert.Equal(before, parts.Total - parts.CampaignEffect);
        Assert.Equal(MathI.MulPermille(before, 1000 + boost) - before, parts.CampaignEffect);
        Assert.True(parts.CampaignEffect > 0);
        Assert.Equal(0, Appeal(g, m, "game-bots").CampaignEffect);
        Assert.Equal(beforeOther, Appeal(g, m, "game-bots").Total);
    }

    [Fact]
    public void The_boost_is_biggest_for_a_lab_nobody_knows_and_falls_to_nothing_at_the_ceiling()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 3));
        SectorEntry entry = g.State.Sector(Sector).Entry(g.Id);
        entry.SharePpm = 0;
        Assert.Equal(360, MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector));
        entry.SharePpm = 300_000;
        Assert.Equal(180, MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector));
        entry.SharePpm = 600_000;
        Assert.Equal(0, MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector));
        entry.SharePpm = 900_000;
        Assert.Equal(0, MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector));
    }

    [Fact]
    public void A_bigger_budget_means_a_bigger_boost()
    {
        int Boost(int level)
        {
            Game g = Lab();
            Selling(g);
            g.State.Sector(Sector).Entry(g.Id).SharePpm = 0;
            g.Do(Command.RunCampaign(g.Id, Sector, level));
            return MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector);
        }
        Assert.Equal(120, Boost(1));
        Assert.Equal(240, Boost(2));
        Assert.Equal(360, Boost(3));
    }

    [Fact]
    public void Against_rivals_the_campaign_wins_the_lab_a_bigger_share_over_the_weeks()
    {
        Game g = Game.New(2);
        g.Autopilot(150);
        SectorState sector = g.State.Sectors.FirstOrDefault(x => x.Entry(g.Id) != null && x.Entries.Count > 1 && x.Entry(g.Id).SharePpm < 400_000);
        Assert.NotNull(sector);
        string json = g.Sim.Save();
        Simulation with = Simulation.Load(g.C, json);
        Simulation without = Simulation.Load(g.C, json);
        with.Player.Cash += 10_000_000;
        Assert.True(with.Apply(Command.RunCampaign(with.Player.Id, sector.Id, 3)).Ok);
        for (int i = 0; i < 8; i++) { with.Step(); without.Step(); }
        Assert.True(with.State.Sector(sector.Id).Entry(with.Player.Id).SharePpm > without.State.Sector(sector.Id).Entry(without.Player.Id).SharePpm);
    }

    [Fact]
    public void It_ends_after_the_data_s_weeks_and_the_hangover_runs_down_then_clears()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 2));
        Run(g, 7);
        Assert.True(MarketingCampaign.Running(g.Me));
        Run(g, 1);
        Assert.False(MarketingCampaign.Running(g.Me));
        Assert.True(MarketingCampaign.Hungover(g.Me));
        Assert.Equal(8, g.Me.HangoverWeeks);
        GameEvent end = Runs(g).Last();
        Assert.Equal(0, end.Pick);
        Assert.Contains("is over", EventText.Describe(g.C, g.State, end));
        Assert.Equal(100, MarketingCampaign.HangoverPermille(g.C, g.Me, Sector));
        Assert.Equal(900, MarketingCampaign.AppealPermille(g.C, g.State, g.Me, Sector));
        Assert.Equal(0, MarketingCampaign.HangoverPermille(g.C, g.Me, "game-bots"));
        DisplayQueries.AppealBreakdown parts = Appeal(g, m, Sector);
        Assert.True(parts.CampaignEffect < 0);
        Run(g, 8);
        Assert.Equal("", g.Me.CampaignSector);
        Assert.Equal(0, g.Me.CampaignLevel);
        Assert.Equal(0, g.Me.HangoverWeeks);
        Assert.Equal(0, Appeal(g, m, Sector).CampaignEffect);
        Assert.Equal(1000, MarketingCampaign.AppealPermille(g.C, g.State, g.Me, Sector));
    }

    // ---- incidents

    [Fact]
    public void An_incident_in_the_sector_ends_the_campaign_at_once_and_costs_reputation()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 2));
        Run(g, 3);
        int plain;
        {
            Game h = Lab();
            ModelState hm = Selling(h);
            h.Me.Reputation = 500;
            Incident(h, hm);
            plain = h.Me.Reputation;
        }
        g.Me.Reputation = 500;
        Incident(g, m);
        Assert.Equal(plain - 15, g.Me.Reputation);
        Assert.False(MarketingCampaign.Running(g.Me));
        Assert.True(MarketingCampaign.Hungover(g.Me));
        GameEvent e = Runs(g).Last();
        Assert.Equal(2, e.Pick);
        Assert.Contains("collapsed when its model had an incident", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void An_incident_outside_a_campaign_or_during_the_hangover_costs_nothing_extra()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Me.Reputation = 500;
        MarketingCampaign.OnIncident(g.C, g.State, g.Me, m);
        Assert.Equal(500, g.Me.Reputation);
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        Run(g, 8);
        Assert.True(MarketingCampaign.Hungover(g.Me));
        MarketingCampaign.OnIncident(g.C, g.State, g.Me, m);
        Assert.Equal(500, g.Me.Reputation);
        Assert.Equal(8, g.Me.HangoverWeeks);
    }

    [Fact]
    public void An_incident_in_another_sector_leaves_the_campaign_alone()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        ModelState other = g.Train("mini", 0);
        g.UntilTrained(other);
        g.Do(Command.ReleaseModel(g.Id, other.Id, 0, "game-bots"));
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        g.Me.Reputation = 500;
        MarketingCampaign.OnIncident(g.C, g.State, g.Me, other);
        Assert.Equal(500, g.Me.Reputation);
        Assert.True(MarketingCampaign.Running(g.Me));
        Assert.NotNull(m);
    }

    // ---- edge cases

    [Fact]
    public void Zero_values_in_the_data_make_a_campaign_a_cost_with_no_boost_and_no_hangover()
    {
        Game g = Lab(c => { c.Marketing.AppealPermillePerLevel = 0; c.Marketing.HangoverPermillePerLevel = 0; c.Marketing.HangoverWeeks = 0; });
        ModelState m = Selling(g);
        long before = Appeal(g, m, Sector).Total;
        g.Do(Command.RunCampaign(g.Id, Sector, 3));
        Assert.Equal(before, Appeal(g, m, Sector).Total);
        Run(g, 8);
        Assert.Equal("", g.Me.CampaignSector);
        Assert.Equal(0, g.Me.HangoverWeeks);
        Assert.Equal(0, Runs(g).Last().Pick);
        Assert.Equal(Sector, Runs(g).Last().Ref);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_zero_cost_campaign_is_free_and_the_boost_never_goes_below_nothing()
    {
        Game g = Lab(c => c.Marketing.CostPerLevel = 0);
        Selling(g);
        long cash = g.Me.Cash;
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        Assert.Equal(cash, g.Me.Cash);
        g.State.Sector(Sector).Entry(g.Id).SharePpm = 1_000_000;
        Assert.Equal(0, MarketingCampaign.BoostPermille(g.C, g.State, g.Me, Sector));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_campaign_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 2));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(g.Me.CampaignWeeks, back.Player.CampaignWeeks);
        Assert.Equal(Sector, back.Player.CampaignSector);
        g.Weeks(14);
        for (int i = 0; i < 14; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Marketing.CostPerLevel = 100);
        g.Autopilot(60);
        string sector = g.State.Sectors.First(s => s.Entry(g.Id) != null).Id;
        Assert.True(g.Try(Command.RunCampaign(g.Id, sector, 2)).Ok);
        g.Autopilot(40);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(g.Me.CampaignLockedUntil, again.Player.CampaignLockedUntil);
    }

    [Fact]
    public void An_older_save_loads_with_no_campaign()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        string old = SaveSchemaPretend.At(g.C, 54, g.Sim.Save());
        Assert.DoesNotContain("campaignSector", old);
        Assert.DoesNotContain("campaignWeeks", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal("", lab.CampaignSector); Assert.Equal(0, lab.CampaignLevel); Assert.Equal(0, lab.CampaignWeeks); Assert.Equal(0, lab.HangoverWeeks); Assert.Equal(0, lab.CampaignLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_campaign_that_does_not_fit_the_data_is_reported()
    {
        Game g = Lab();
        Selling(g);
        g.Do(Command.RunCampaign(g.Id, Sector, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
        g.Me.CampaignLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
        g.Me.CampaignLevel = 1;
        g.Me.CampaignWeeks = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
        g.Me.CampaignWeeks = 3;
        g.Me.CampaignSector = "nowhere";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
        g.Me.CampaignSector = "";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
        g.Me.CampaignSector = Sector;
        g.Me.CampaignWeeks = 0;
        g.Me.HangoverWeeks = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("marketing campaign"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Marketing.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Marketing.MaxLevel = 11;
        c.Marketing.CostPerLevel = -1;
        c.Marketing.Weeks = 53;
        c.Marketing.AppealPermillePerLevel = 1000;
        c.Marketing.ShareCeilingPermille = 0;
        c.Marketing.HangoverWeeks = 53;
        c.Marketing.HangoverPermillePerLevel = 1000;
        c.Marketing.BacklashReputation = 101;
        c.Marketing.CooldownWeeks = 5;
        c.Marketing.MinSales = -1;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "costPerLevel", "weeks", "appealPermillePerLevel", "shareCeilingPermille", "hangoverWeeks", "hangoverPermillePerLevel", "backlashReputation", "cooldownWeeks", "minSales" })
            Assert.Contains(problems, p => p.Contains("marketing." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_run_campaigns()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => { Assert.Equal("", lab.CampaignSector); Assert.Equal(0, lab.CampaignLevel); });
        Assert.Empty(Runs(g));
    }

    [Fact]
    public void A_game_with_repeated_campaigns_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(60);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int run = 0;
        for (int i = 0; i < 260; i++)
        {
            g.Autopilot(1);
            if (i % 40 == 3)
            {
                SectorState sector = g.State.Sectors.FirstOrDefault(s => s.Entry(g.Id) != null);
                if (sector != null && g.Try(Command.RunCampaign(g.Id, sector.Id, 2)).Ok) run++;
            }
        }
        Assert.True(run >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
