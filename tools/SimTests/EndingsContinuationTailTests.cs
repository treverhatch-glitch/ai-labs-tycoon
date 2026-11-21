using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EndingsContinuationTailTests
{
    static ModelState SuperRelease(Game g, int alignment) => new ModelState
    {
        Id = g.State.TakeId(),
        Capability = 950,
        Alignment = alignment,
        DisplayedAlignment = alignment,
        Status = ModelStatus.Released,
        LineId = "mini",
        Version = 1
    };

    [Fact]
    public void Monopoly_offers_acknowledge_and_leaves_the_lab_playable()
    {
        var g = Game.Solo();
        EndingSystem.TryMonopoly(g.C, g.State);
        g.State.Endings.Add(new EndingRecord { EndingId = "monopoly", LabId = g.Id, Tick = g.Tick });
        int idx = g.State.Endings.Count - 1;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        Assert.Equal("acknowledge", g.State.Endings[idx].Choice);
        Assert.Equal(g.Id, g.State.PlayerLabId);
    }

    [Fact]
    public void Monopoly_refuses_an_unlisted_continue_mode()
    {
        var g = Game.Solo();
        g.State.Endings.Add(new EndingRecord { EndingId = "monopoly", LabId = g.Id, Tick = g.Tick, Choice = "" });
        int idx = g.State.Endings.Count - 1;
        g.Refused(Command.ContinueAfterEnding(g.Id, idx, "bailout"), CommandError.EndingUnavailable);
    }

    [Fact]
    public void Paperclipped_spectate_drops_the_player_lab_id()
    {
        var g = Game.Solo();
        g.Me.Models.Add(SuperRelease(g, 50));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == EndingSystem.PaperclippedId);
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
    }

    [Fact]
    public void Paperclipped_keep_going_keeps_control()
    {
        var g = Game.Solo();
        g.Me.Models.Add(SuperRelease(g, 50));
        EndingSystem.TrySuperintelligence(g.C, g.State);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == EndingSystem.PaperclippedId);
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "keep-going"));
        Assert.Equal(g.Id, g.State.PlayerLabId);
    }

    [Fact]
    public void Long_summer_lists_both_continue_modes_in_config()
    {
        EndingDefConfig def = TestData.Config().Endings.Def("long-summer");
        Assert.Contains("keep-going", def.ContinueModes);
        Assert.Contains("spectate", def.ContinueModes);
    }

    [Fact]
    public void Loose_spectate_leaves_the_world_stepping()
    {
        var g = Game.New(seed: 3);
        g.Me.Models.Add(SuperRelease(g, 400));
        g.Me.Rooms.Clear();
        EndingSystem.TrySuperintelligence(g.C, g.State);
        int idx = g.State.Endings.FindIndex(e => e.LabId == g.Id && e.EndingId == EndingSystem.LooseId);
        Assert.True(idx >= 0);
        int tick = g.Tick;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "spectate"));
        g.Weeks(20);
        Assert.True(g.Tick > tick);
    }

    [Fact]
    public void A_recorded_monopoly_keeps_choice_empty_until_continue()
    {
        var g = Game.Solo();
        g.State.Endings.Add(new EndingRecord { EndingId = "monopoly", LabId = g.Id, Tick = g.Tick });
        Assert.Equal("", g.State.Endings.Last().Choice);
    }

    [Fact]
    public void ContinueAfterEnding_refuses_a_second_pick()
    {
        var g = Game.Solo();
        g.State.Endings.Add(new EndingRecord { EndingId = "monopoly", LabId = g.Id, Tick = g.Tick });
        int idx = g.State.Endings.Count - 1;
        g.Do(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"));
        g.Refused(Command.ContinueAfterEnding(g.Id, idx, "acknowledge"), CommandError.AlreadyChose);
    }

    [Fact]
    public void Handcuffs_still_needs_no_continue_modes()
    {
        Assert.Empty(TestData.Config().Endings.Def("handcuffs").ContinueModes);
    }

    [Fact]
    public void Ousted_still_auto_closes_with_done()
    {
        var g = Game.Solo(tweak: c =>
        {
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            c.Funding.Round("series-b").DilutionPermille = 400;
            c.Funding.ControlPermille = 500;
        });
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        g.Do(Command.RaiseFunding(g.Id, "series-b"));
        Assert.Equal("done", g.State.Endings.Single(e => e.EndingId == "ousted").Choice);
    }

    [Fact]
    public void Exit_still_happens_inside_SellLab_not_continue()
    {
        Assert.Empty(TestData.Config().Endings.Def("exit").ContinueModes);
    }

    [Fact]
    public void Rules_keep_stepping_while_a_monopoly_choice_is_open()
    {
        var g = Game.New(seed: 1);
        g.State.Endings.Add(new EndingRecord { EndingId = "monopoly", LabId = g.State.PlayerLabId, Tick = g.Tick });
        int tick = g.Tick;
        g.Weeks(12);
        Assert.True(g.Tick > tick);
    }
}