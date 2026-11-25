using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ScalingPolicyTests
{
    static Game Lab()
    {
        return Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Security.Enabled = 0;
            c.Support.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    static ModelState Trained(Game g, int capability)
    {
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        m.Capability = capability;
        return m;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_tripwire_rises_with_the_level_and_its_safeguards_are_a_tier_and_safety_people_for_each()
    {
        Game g = Lab();
        Assert.Equal(g.C.Scaling.FirstTripwire, ScalingPolicy.Tripwire(g.C, 1));
        Assert.Equal(g.C.Scaling.FirstTripwire + 2 * g.C.Scaling.TripwireStep, ScalingPolicy.Tripwire(g.C, 3));
        Assert.False(ScalingPolicy.Safeguarded(g.C, g.Me, 1));
        g.Me.SecurityTier = 1;
        Assert.True(ScalingPolicy.Safeguarded(g.C, g.Me, 1));
        Assert.False(ScalingPolicy.Safeguarded(g.C, g.Me, 2));
        g.Me.SecurityTier = 3;
        Assert.Equal(3 >= 2 * g.C.Scaling.SafetyPerTier, ScalingPolicy.Safeguarded(g.C, g.Me, 2));
        Assert.False(ScalingPolicy.Safeguarded(g.C, g.Me, 3));
    }

    [Fact]
    public void Raising_costs_a_fee_for_each_level_and_lowering_costs_reputation_and_is_locked()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.AdoptPolicy(g.Id, 2));
        Assert.Equal(cash - 2 * g.C.Scaling.AdoptFee, g.Me.Cash);
        Assert.Equal(2, g.Me.PolicyLevel);
        Assert.Equal(g.State.Tick + g.C.Scaling.LockWeeks, g.Me.PolicyLockedUntil);
        g.Do(Command.AdoptPolicy(g.Id, 3));
        g.Refused(Command.AdoptPolicy(g.Id, 1), CommandError.InvalidArgument);
        g.Weeks(g.C.Scaling.LockWeeks);
        int rep = g.Me.Reputation;
        g.Do(Command.AdoptPolicy(g.Id, 1));
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(1, g.Me.PolicyLevel);
        Assert.Equal(1, Count(g, EventKind.PolicyChanged) - 2);
    }

    [Fact]
    public void A_level_must_be_in_range_new_and_affordable()
    {
        Game g = Lab();
        g.Refused(Command.AdoptPolicy(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.AdoptPolicy(g.Id, g.C.Scaling.MaxLevel + 1), CommandError.InvalidArgument);
        g.Refused(Command.AdoptPolicy(g.Id, -1), CommandError.InvalidArgument);
        g.Me.Cash = 1;
        Assert.Equal(CommandError.NotEnoughCash, g.Try(Command.AdoptPolicy(g.Id, 1)).Error);
    }

    [Fact]
    public void A_trained_model_at_a_tripwire_is_held_back_and_so_is_every_new_run_until_the_safeguards_stand()
    {
        Game g = Lab();
        g.C.Scaling.SafetyPerTier = 1;
        g.Do(Command.AdoptPolicy(g.Id, 2));
        ModelState m = Trained(g, ScalingPolicy.Tripwire(g.C, 2) + 10);
        g.Weeks(1);
        Assert.Equal(2, g.Me.PolicyHold);
        Assert.Equal(1, Count(g, EventKind.TripwireHit));
        Assert.True(ScalingPolicy.Holds(g.C, g.Me, m));
        Assert.Equal(CommandError.InvalidArgument, g.Try(Command.ReleaseModel(g.Id, m.Id, "game-ai")).Error);
        Assert.Equal(CommandError.InvalidArgument, g.Try(Command.StartTraining(g.Id, "mini", 100)).Error);
        g.Weeks(3);
        Assert.Equal(1, Count(g, EventKind.TripwireHit));
        g.Me.SecurityTier = 2;
        Assert.True(ScalingPolicy.Safeguarded(g.C, g.Me, 2));
        g.Weeks(1);
        Assert.Equal(0, g.Me.PolicyHold);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.TripwireCleared).Pick);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
    }

    [Fact]
    public void A_model_under_the_tripwire_or_without_a_policy_is_never_held()
    {
        Game g = Lab();
        ModelState m = Trained(g, 900);
        Assert.False(ScalingPolicy.Holds(g.C, g.Me, m));
        g.Do(Command.AdoptPolicy(g.Id, 1));
        m.Capability = ScalingPolicy.Tripwire(g.C, 1) - 1;
        Assert.False(ScalingPolicy.Holds(g.C, g.Me, m));
        Assert.Equal(0, ScalingPolicy.Tripped(g.C, g.Me));
        m.Capability = ScalingPolicy.Tripwire(g.C, 2) + 50;
        Assert.Equal(1, ScalingPolicy.Tripped(g.C, g.Me));
        g.Do(Command.AdoptPolicy(g.Id, 3));
        Assert.Equal(2, ScalingPolicy.Tripped(g.C, g.Me));
    }

    [Fact]
    public void Breaking_the_policy_lets_the_model_go_at_a_price_in_reputation_and_safety_morale()
    {
        Game g = Lab();
        g.Do(Command.AdoptPolicy(g.Id, 2));
        ModelState m = Trained(g, ScalingPolicy.Tripwire(g.C, 2) + 10);
        g.Weeks(1);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        int rep = g.Me.Reputation;
        g.Do(Command.BreakPolicy(g.Id));
        Assert.Equal(0, g.Me.PolicyLevel);
        Assert.Equal(1, g.Me.PolicyBreaks);
        Assert.True(g.Me.Reputation < rep);
        Assert.All(g.Me.Staff.Where(p => p.RoleId == "safety"), p => Assert.Equal(800 - g.C.Scaling.OverrideMorale, p.Morale));
        Assert.All(g.Me.Staff.Where(p => p.RoleId != "safety"), p => Assert.Equal(800, p.Morale));
        Assert.True(g.Me.PolicyHold <= g.Me.PolicyLevel);
        g.Refused(Command.BreakPolicy(g.Id), CommandError.InvalidArgument);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        Assert.Equal("break", g.State.Events.Last(e => e.Kind == EventKind.PolicyChanged).Ref);
        g.Refused(Command.AdoptPolicy(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_policy_softens_scandals_and_weakens_negligence_claims_by_level()
    {
        Game g = Lab();
        int bare = PressSystem.Damp(g.C, g.Me, 100);
        g.Me.PolicyLevel = 2;
        Assert.True(PressSystem.Damp(g.C, g.Me, 100) < bare);
        g.Me.PolicyLevel = 0;
        ModelState m = Trained(g, 300);
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        int open = g.Me.Cases.Single().Strength;
        g.Me.Cases.Clear();
        g.Me.PolicyLevel = 3;
        Litigation.OnIncident(g.C, g.State, g.Me, m, 50_000);
        Assert.Equal(open - 3 * g.C.Scaling.ClaimStrength, g.Me.Cases.Single().Strength);
    }

    [Fact]
    public void Lowering_the_policy_while_a_model_is_held_clears_the_hold_without_the_safeguards()
    {
        Game g = Lab();
        g.Do(Command.AdoptPolicy(g.Id, 1));
        Trained(g, ScalingPolicy.Tripwire(g.C, 1) + 10);
        g.Weeks(g.C.Scaling.LockWeeks);
        Assert.Equal(1, g.Me.PolicyHold);
        g.Do(Command.AdoptPolicy(g.Id, 0));
        Assert.Equal(0, g.Me.PolicyHold);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(1);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.TripwireCleared).Pick);
    }

    [Fact]
    public void Nothing_happens_when_it_is_switched_off()
    {
        Game g = Lab();
        g.C.Scaling.Enabled = 0;
        g.Refused(Command.AdoptPolicy(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.BreakPolicy(g.Id), CommandError.InvalidArgument);
        Assert.False(ScalingPolicy.Holds(g.C, g.Me, Trained(g, 5000)));
        Assert.Equal(0, ScalingPolicy.Tripped(g.C, g.Me));
    }

    [Fact]
    public void A_policy_with_a_hold_saves_loads_and_plays_on_the_same_and_old_saves_have_none()
    {
        Game g = Lab();
        g.Do(Command.AdoptPolicy(g.Id, 3));
        Trained(g, ScalingPolicy.Tripwire(g.C, 3) + 10);
        g.Weeks(2);
        Assert.Equal(3, g.Me.PolicyHold);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 84, g.Sim.Save());
        Assert.DoesNotContain("policyLevel", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.PolicyLevel);
        Assert.Equal(0, lab.PolicyHold);
    }
}
