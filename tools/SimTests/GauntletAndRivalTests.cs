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

/// <summary>The yearly tournament, and the parody labs who play by the player's rules.</summary>
public class GauntletAndRivalTests
{
    [Fact]
    public void A_gauntlet_is_held_at_the_end_of_every_year()
    {
        Game g = Game.New(seed: 1);
        g.Weeks(47);
        Assert.Empty(g.State.Gauntlets);
        g.Weeks(1);
        Assert.Single(g.State.Gauntlets);
        Assert.Equal(1995, g.State.Gauntlets[0].Year);

        g.Years(3);
        Assert.Equal(new[] { 1995, 1996, 1997, 1998 }, g.State.Gauntlets.Select(r => r.Year).ToArray());
    }

    [Fact]
    public void The_crown_ranking_lists_each_labs_best_model_best_first()
    {
        Game g = Game.New(seed: 1);
        g.Autopilot(3 * 48);

        List<GauntletEntry> crown = GauntletSystem.Latest(g.State, GauntletSystem.CrownBracket);
        Assert.Equal(g.State.Labs.Count(l => LabQueries.BestReleased(l) != null), crown.Count);
        Assert.Equal(Enumerable.Range(1, crown.Count).ToArray(), crown.Select(e => e.Rank).ToArray());
        for (int i = 1; i < crown.Count; i++) Assert.True(crown[i - 1].Score >= crown[i].Score);
        Assert.Equal(crown.Count, crown.Select(e => e.LabId).Distinct().Count());

        GauntletRecord record = g.State.Gauntlets.Last();
        Assert.Equal(crown[0].LabId, record.CrownLabId);
        Assert.Equal(crown[0].ModelId, record.CrownModelId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GauntletCrown && e.LabId == record.CrownLabId);
    }

    [Fact]
    public void Scores_are_true_quality_give_or_take_stage_nerves()
    {
        Game g = Game.New(seed: 1);
        g.Autopilot(2 * 48);
        foreach (GauntletEntry entry in GauntletSystem.Latest(g.State, GauntletSystem.CrownBracket))
        {
            long truth = g.State.Lab(entry.LabId).Model(entry.ModelId).Capability;
            long band = truth * g.C.Gauntlet.JitterPermille / 1000 + 1;
            Assert.InRange(entry.Score, truth - band, truth + band);
        }
    }

    [Fact]
    public void Every_sector_with_models_on_sale_is_a_bracket_and_its_winner_gets_a_pull_on_customers()
    {
        Game g = Game.New(seed: 1);
        g.Autopilot(2 * 48);

        GauntletRecord record = g.State.Gauntlets.Last();
        foreach (SectorState sector in g.State.Sectors.Where(s => s.Entries.Count > 0))
        {
            List<GauntletEntry> bracket = record.Entries.Where(e => e.Bracket == sector.Id).ToList();
            Assert.Equal(sector.Entries.Count, bracket.Count);
            Assert.Equal(bracket.Single(e => e.Rank == 1).LabId, sector.ChampionLabId);

            LabState champion = g.State.Lab(sector.ChampionLabId);
            ModelState model = champion.Model(sector.Entry(champion.Id).ModelId);
            SectorConfig config = g.C.Sector(sector.Id);
            DisplayQueries.AppealBreakdown crowned = DisplayQueries.AppealParts(g.C, g.State, config, sector, champion, model);
            Assert.True(crowned.ChampionBoost > 0);
            int savedChampion = sector.ChampionLabId;
            sector.ChampionLabId = -1;
            DisplayQueries.AppealBreakdown plain = DisplayQueries.AppealParts(g.C, g.State, config, sector, champion, model);
            Assert.Equal(0, plain.ChampionBoost);
            Assert.True(plain.Total < crowned.Total);
            sector.ChampionLabId = savedChampion;
        }
        Assert.DoesNotContain(record.Entries, e => e.Bracket == "spam-search");
    }

    [Fact]
    public void Winning_the_crown_and_being_the_safest_lab_both_raise_reputation()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 8);
        ModelState mini = g.Train("mini", 300);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");
        g.Until(() => g.State.Gauntlets.Count == 1, 60, "the first Gauntlet");

        // The only lab in the world wins both prizes.
        Assert.Equal(g.Id, g.State.Gauntlets[0].CrownLabId);
        Assert.Equal(g.Id, g.State.Gauntlets[0].SafestLabId);
        GameEvent demo = g.State.Events.Single(e => e.Kind == EventKind.GauntletDemo);
        Assert.Equal(1, demo.Pick);
        Assert.Equal(g.C.Safety.ReputationBaseline + g.C.Gauntlet.CrownReputation + g.C.Gauntlet.SafestReputation + g.C.Gauntlet.DemoTriumphReputation, g.Me.Reputation);
    }

    [Fact]
    public void With_nothing_on_sale_nobody_is_crowned()
    {
        Game g = Game.Solo();
        g.Years(1);
        Assert.Equal(-1, g.State.Gauntlets[0].CrownLabId);
        Assert.Empty(g.State.Gauntlets[0].Entries);
    }

    [Fact]
    public void The_three_parody_labs_join_every_new_game_unless_told_otherwise()
    {
        Game g = Game.New();
        Assert.Equal(new[] { "Fable Labs", "OpenAye", "AntHopticsAI", "xAy" }, g.State.Labs.Select(l => l.Name).ToArray());
        Assert.Equal(LabController.Player, g.Me.Controller);
        Assert.All(g.State.Labs.Skip(1), l => Assert.Equal(LabController.Plan, l.Controller));
        Assert.Equal("ChatGBT", g.Rival("openaye").FamilyName);

        var options = new NewGameOptions { Seed = 1, LabName = "Hatch Labs", FamilyName = "Egg", RivalIds = new List<string> { "xay" } };
        var custom = new Game(Simulation.NewGame(TestData.Config(), options));
        Assert.Equal(new[] { "Hatch Labs", "xAy" }, custom.State.Labs.Select(l => l.Name).ToArray());
        Assert.Equal("Egg", custom.Me.FamilyName);
    }

    [Fact]
    public void Rivals_build_hire_train_and_ship_on_their_own()
    {
        Game g = Game.New(seed: 1);
        g.Years(2);
        foreach (LabState rival in g.State.Labs.Skip(1))
        {
            Assert.True(rival.Rooms.Count >= 3, rival.Name + " should have built rooms");
            Assert.True(LabQueries.TotalStaff(rival) >= 5, rival.Name + " should have hired");
            Assert.True(LabQueries.TotalGpus(rival) > 0, rival.Name + " should have GPUs");
            Assert.True(rival.Techs.Count > 0, rival.Name + " should have researched something");
            Assert.True(LabQueries.BestReleased(rival) != null, rival.Name + " should have a model on sale");
            Assert.StartsWith(rival.FamilyName + " ", LabQueries.BestReleased(rival).Name);
        }
    }

    [Fact]
    public void Rivals_live_by_the_players_rules()
    {
        // No free desks, no free racks, no rooms on top of each other, and every dollar on the ledger.
        Game g = Game.New(seed: 6);
        for (int week = 0; week < 6 * 48; week++)
        {
            g.Weeks(1);
            foreach (LabState lab in g.State.Labs.Skip(1))
            {
                for (int r = 0; r < g.C.Roles.Count; r++)
                {
                    string roleId = g.C.Roles[r].Id;
                    int have = LabQueries.StaffCount(lab, roleId);
                    if (have > 0)
                        Assert.True(have <= LabQueries.Seats(g.C, lab, roleId), lab.Name + " has more " + roleId + " than desks");
                }
                Assert.True(LabQueries.HqGpus(lab) <= LabQueries.GpuSlots(g.C, lab), lab.Name + " owns more GPUs than it has racks");
                Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
                Assert.True(LabQueries.ActiveRuns(lab) <= g.C.Training.MaxConcurrentRuns);

                LotConfig lot = g.C.Lot(lab.LotId);
                for (int a = 0; a < lab.Rooms.Count; a++)
                {
                    RoomState room = lab.Rooms[a];
                    Assert.True(LotGrid.InBounds(lot, room.X, room.Y, room.Width, room.Height));
                    for (int b = 0; b < a; b++)
                        Assert.False(LotGrid.Overlaps(lab.Rooms[b], room.X, room.Y, room.Width, room.Height), lab.Name + " has overlapping rooms");
                }
            }
        }
    }

    [Fact]
    public void Each_rival_plays_to_its_character()
    {
        Game g = Game.New(seed: 1);
        g.Years(4);
        LabState openaye = g.Rival("openaye"), anthoptics = g.Rival("anthoptics"), xay = g.Rival("xay");

        // The careful sniper ships the safest models.
        int Safety(LabState lab) { return LabQueries.BestReleased(lab).Alignment; }
        Assert.True(Safety(anthoptics) > Safety(openaye) && Safety(anthoptics) > Safety(xay));
        Assert.True(Safety(anthoptics) >= 900);
        Assert.Contains(g.State.Gauntlets, r => r.SafestLabId == anthoptics.Id);

        // Ships-first is first to market and has the incidents to show for it.
        int FirstRelease(LabState lab) { return lab.Models.Where(m => m.ReleasedTick >= 0).Min(m => m.ReleasedTick); }
        Assert.True(FirstRelease(openaye) < FirstRelease(anthoptics) && FirstRelease(openaye) < FirstRelease(xay));
        Assert.True(openaye.Models.Sum(m => m.Incidents) > anthoptics.Models.Sum(m => m.Incidents));

        // The builder owns its metal; the others rent.
        Assert.True(LabQueries.OwnedUnits(xay) > 0);
        Assert.Equal(0, LabQueries.OwnedUnits(openaye));
        Assert.Equal(0, LabQueries.OwnedUnits(anthoptics));
        Assert.True(LabQueries.TotalGpus(xay) > LabQueries.TotalGpus(openaye));
    }

    [Fact]
    public void Rivals_do_not_all_start_on_the_same_day_and_the_seed_moves_their_start()
    {
        var starts = new HashSet<string>();
        for (long seed = 1; seed <= 10; seed++)
        {
            Game g = Game.New(seed);
            foreach (LabState rival in g.State.Labs.Skip(1))
            {
                PlanConfig plan = g.C.Rival(rival.RivalId).Plan;
                Assert.InRange(rival.PlanStartTick, plan.StartDelayWeeks, plan.StartDelayWeeks + plan.StartJitterWeeks);
            }
            starts.Add(string.Join(",", g.State.Labs.Skip(1).Select(l => l.PlanStartTick)));
        }
        Assert.True(starts.Count > 3, "ten seeds should give several different line-ups");
    }

    [Fact]
    public void A_rival_does_nothing_before_its_start_week()
    {
        Game g = Game.New(seed: 1);
        LabState xay = g.Rival("xay");
        g.Weeks(xay.PlanStartTick);
        Assert.Empty(xay.Rooms);
        Assert.Equal(g.C.Rival("xay").StartCash + xay.PlanStartTick * g.C.Rival("xay").BackerPerWeek, xay.Cash);
        g.Weeks(5);
        Assert.NotEmpty(xay.Rooms);
    }

    [Fact]
    public void A_plan_in_debt_cuts_back_instead_of_growing()
    {
        Game g = Game.New(seed: 1);
        g.Years(1);
        LabState rival = g.Rival("openaye");
        int staff = LabQueries.TotalStaff(rival);
        int rented = rival.RentedGpus;

        rival.Ledger.StartCash -= rival.Cash + 50000;
        rival.Cash = -50000;
        rival.WeeksInDebt = 1;
        g.Weeks(4);

        Assert.True(LabQueries.TotalStaff(rival) < staff);
        Assert.True(rival.RentedGpus < rented);
    }

    [Fact]
    public void The_autopilot_can_run_the_players_lab_with_the_same_planner_the_rivals_use()
    {
        Game g = Game.Solo();
        g.Autopilot(2 * 48);
        Assert.NotNull(LabQueries.BestReleased(g.Me));
        Assert.True(g.Me.Ledger.Lifetime.Revenue > 0);
        Assert.True(g.Sim.Journal.Entries.Count > 10, "the autopilot's orders are journaled like a player's");
    }
}
