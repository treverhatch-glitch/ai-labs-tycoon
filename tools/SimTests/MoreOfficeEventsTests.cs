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

/// <summary>Round three of the office events: a goodbye, a rival's Friday, the model asking what off means, a reply, a chip's first benchmark, a landlord's note.</summary>
public class MoreOfficeEventsTests
{
    static readonly string[] NewIds = { "researcher-leaves", "ceo-friday", "model-off", "husk-lol", "minesweeper", "landlord-note" };

    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            c.Office.ChancePermille = 0;
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static void Pose(Game g, string id)
    {
        g.Me.OfficeEventId = id;
        g.Me.OfficeEventExpires = g.State.Tick + g.C.Office.ExpireWeeks;
        Assert.True(OfficeEvents.IsPending(g.Me));
    }

    static ModelState Released(Game g)
    {
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        return model;
    }

    [Fact]
    public void The_six_new_events_ship_with_three_answers_and_the_data_is_clean()
    {
        GameConfig c = TestData.Config();
        foreach (string id in NewIds)
        {
            OfficeEventConfig e = c.Office.Event(id);
            Assert.NotNull(e);
            Assert.Equal(3, e.Choices.Count);
            Assert.Equal(OfficeTrigger.Random, e.Trigger);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_ready_chip_design_of_the_labs_own_is_what_the_chip_event_needs()
    {
        Game g = Lab();
        Assert.False(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.OwnChip));
        g.Me.ChipDesigns.Add(new ChipDesign { Id = 1, Status = ChipDesignStatus.Designing });
        Assert.False(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.OwnChip));
        g.Me.ChipDesigns.Add(new ChipDesign { Id = 2, Status = ChipDesignStatus.Cancelled });
        Assert.False(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.OwnChip));
        g.Me.ChipDesigns[0].Status = ChipDesignStatus.Ready;
        Assert.True(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.OwnChip));
    }

    [Fact]
    public void Rented_gpus_are_what_the_landlord_event_needs()
    {
        Game g = Lab();
        g.Me.RentedGpus = 0;
        Assert.False(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.RentedCompute));
        g.Me.RentedGpus = 4;
        Assert.True(OfficeEvents.MeetsRequirement(g.C, g.Me, OfficeRequires.RentedCompute));
    }

    [Fact]
    public void The_chip_and_landlord_events_wait_for_their_needs_and_the_others_do_not()
    {
        Game g = Lab();
        g.Me.RentedGpus = 0;
        List<string> ids = OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).ToList();
        Assert.DoesNotContain("minesweeper", ids);
        Assert.DoesNotContain("landlord-note", ids);
        Assert.Contains("ceo-friday", ids);
        Assert.Contains("researcher-leaves", ids);
        Assert.DoesNotContain("model-off", ids);

        g.Me.RentedGpus = 2;
        g.Me.ChipDesigns.Add(new ChipDesign { Id = 1, Status = ChipDesignStatus.Ready });
        Released(g);
        ids = OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).ToList();
        foreach (string id in NewIds) Assert.Contains(id, ids);
    }

    [Fact]
    public void Letting_the_model_off_the_hook_raises_its_corruption_and_telling_it_the_truth_lowers_it()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Corruption = 300;
        Pose(g, "model-off");
        g.Do(Command.AnswerOfficeEvent(g.Id, "change-subject"));
        Assert.Equal(380, m.Corruption);

        Pose(g, "model-off");
        g.Do(Command.AnswerOfficeEvent(g.Id, "explain"));
        Assert.Equal(330, m.Corruption);
    }

    [Fact]
    public void Corruption_from_the_off_question_never_leaves_its_range()
    {
        Game g = Lab();
        ModelState m = Released(g);
        m.Corruption = 20;
        Pose(g, "model-off");
        g.Do(Command.AnswerOfficeEvent(g.Id, "explain"));
        Assert.Equal(0, m.Corruption);
        m.Corruption = 990;
        Pose(g, "model-off");
        g.Do(Command.AnswerOfficeEvent(g.Id, "change-subject"));
        Assert.Equal(1000, m.Corruption);
    }

    [Fact]
    public void A_counter_offer_to_the_leaver_costs_two_weeks_of_standing_still_and_some_morale()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        Pose(g, "researcher-leaves");
        int morale0 = g.Me.Staff.Min(p => p.Morale);
        g.Do(Command.AnswerOfficeEvent(g.Id, "counter"));
        Assert.True(g.Me.Cash < cash);
        Assert.True(g.Me.Staff.Min(p => p.Morale) <= morale0);
        Assert.False(OfficeEvents.IsPending(g.Me));
    }

    [Fact]
    public void A_send_off_lifts_reputation_and_morale()
    {
        Game g = Lab();
        g.Me.Reputation = 400;
        Pose(g, "researcher-leaves");
        g.Do(Command.AnswerOfficeEvent(g.Id, "send-off"));
        Assert.Equal(410, g.Me.Reputation);
    }

    [Fact]
    public void Every_new_event_answer_is_accepted_and_logged()
    {
        foreach (string id in NewIds)
        {
            OfficeEventConfig e = TestData.Config().Office.Event(id);
            foreach (OfficeChoiceConfig choice in e.Choices)
            {
                Game g = Lab();
                Released(g);
                Pose(g, id);
                g.Do(Command.AnswerOfficeEvent(g.Id, choice.IdValue));
                Assert.False(OfficeEvents.IsPending(g.Me), id + "/" + choice.IdValue);
                Assert.Equal(id + "|" + choice.IdValue, g.State.Events.Last(x => x.Kind == EventKind.OfficeEventAnswered).Ref);
            }
        }
    }

    [Fact]
    public void The_shipped_list_still_starts_with_the_old_events_in_the_old_order()
    {
        GameConfig c = TestData.Config();
        Assert.Equal("intern-repo", c.Office.Events[0].IdValue);
        int escape = c.Office.Events.FindIndex(e => e.IdValue == "escape-attempt");
        Assert.Equal(NewIds, c.Office.Events.Skip(escape + 1).Take(NewIds.Length).Select(e => e.IdValue).ToArray());
    }

    [Fact]
    public void The_needs_are_saved_by_name_and_survive_a_save_and_load()
    {
        Game g = Lab();
        string json = g.Sim.Save();
        Assert.Contains("\"requires\": \"OwnChip\"", TestData.ConfigJson());
        Simulation back = Simulation.Load(g.C, json);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }
}
