using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class RunawayPlaybookTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.RunawayPlaybook.MinEra = 0;
            c.Agency.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 3, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

  [Fact]
    public void Playbook_level_zero_does_nothing_until_set()
    {
        Game g = Lab();
        Assert.Equal(0, RunawayPlaybook.BlowupCutPermille(g.C, g.Me));
        Assert.Equal(0, RunawayPlaybook.TrustKeepPermille(g.C, g.Me));
    }

    [Fact]
    public void Set_playbook_is_checked_and_costs_on_raises()
    {
        Game g = Lab();
        g.Refused(Command.SetPlaybook(g.Id, 9), CommandError.InvalidArgument);
        g.Do(Command.SetPlaybook(g.Id, 2));
        Assert.Equal(2, g.Me.PlayLevel);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PlaybookSet);
        g.Refused(Command.SetPlaybook(g.Id, 2), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = g.C.RunawayPlaybook.SetupPerLevel - 1;
        poor.Refused(Command.SetPlaybook(poor.Id, 1), CommandError.NotEnoughCash);
        Game off = Lab(c => c.RunawayPlaybook.Enabled = 0);
        off.Refused(Command.SetPlaybook(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Arming_needs_a_level_staff_and_respects_cooldown()
    {
        Game g = Lab();
        g.Refused(Command.ArmPlaybook(g.Id), CommandError.InvalidArgument);
        g.Do(Command.SetPlaybook(g.Id, 2));
        Game thin = Lab(c => c.RunawayPlaybook.Staff = 20);
        thin.Do(Command.SetPlaybook(thin.Id, 2));
        thin.Refused(Command.ArmPlaybook(thin.Id), CommandError.NotEnoughStaff);
        g.Do(Command.ArmPlaybook(g.Id));
        Assert.True(RunawayPlaybook.Armed(g.Me));
        Assert.Equal(g.C.RunawayPlaybook.ArmWeeks, g.Me.PlayLeft);
        g.Refused(Command.ArmPlaybook(g.Id), CommandError.InvalidArgument);
        g.Me.PlayArmed = 0;
        g.Me.PlayLeft = 0;
        g.Me.PlayArmUntil = g.Tick + 5;
        g.Refused(Command.ArmPlaybook(g.Id), CommandError.BountyLocked);
    }

    [Fact]
    public void Armed_playbook_cuts_blowup_chance()
    {
        Game g = Lab(c => { c.RunawayPlaybook.BlowupCut = 200; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        int bare = Agency.BlowupChance(g.C, g.Me, m);
        g.Do(Command.SetPlaybook(g.Id, 2));
        g.Do(Command.ArmPlaybook(g.Id));
        int armed = Agency.BlowupChance(g.C, g.Me, m);
        Assert.True(armed < bare);
    }

    [Fact]
    public void Armed_playbook_ticks_down_and_disarms()
    {
        Game g = Lab(c => c.RunawayPlaybook.ArmWeeks = 3);
        g.Do(Command.SetPlaybook(g.Id, 1));
        g.Do(Command.ArmPlaybook(g.Id));
        g.Weeks(2);
        Assert.True(RunawayPlaybook.Armed(g.Me));
        g.Weeks(1);
        Assert.False(RunawayPlaybook.Armed(g.Me));
    }

    [Fact]
    public void Trust_keep_softens_blowup_trust_loss_while_armed()
    {
        Game g = Lab(c => { c.RunawayPlaybook.TrustKeep = 500; c.Agency.BlowupTrust = 20; });
        g.Do(Command.SetPlaybook(g.Id, 1));
        g.Do(Command.ArmPlaybook(g.Id));
        Assert.Equal(500, RunawayPlaybook.TrustKeepPermille(g.C, g.Me));
    }

    [Fact]
    public void Era_lock_and_player_only_apply()
    {
        Game early = Lab(c => c.RunawayPlaybook.MinEra = c.Eras.Count - 1);
        early.Refused(Command.SetPlaybook(early.Id, 1), CommandError.EraLocked);
        Game g = Lab();
        g.Refused(Command.SetPlaybook(9999, 1), CommandError.UnknownLab);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_playbook_fields()
    {
        Game g = Lab();
        g.Do(Command.SetPlaybook(g.Id, 2));
        g.Do(Command.ArmPlaybook(g.Id));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 130, g.Sim.Save()));
        Assert.Equal(0, older.Player.PlayLevel + older.Player.PlayArmed + older.Player.PlayLeft);
    }
}