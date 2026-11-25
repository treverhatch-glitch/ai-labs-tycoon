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

/// <summary>Open requisitions: people ordered for a future date at a locked price, seated and paid for on the day.</summary>
public class RequisitionsTests
{
    static Game Lab(Action<GameConfig> tweak = null, int spare = 2)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (spare > 0)
        {
            g.Do(Command.Fire(g.Id, "engineer", spare));
            g.Me.Alumni.Clear();
        }
        return g;
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Requisitions.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Requisitions.Enabled = 2;
        c.Requisitions.MaxOpen = 0;
        c.Requisitions.MaxCount = 0;
        c.Requisitions.LeadWeeks = 0;
        c.Requisitions.FeePermille = 1000;
        c.Requisitions.DepositPermille = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxOpen", "maxCount", "leadWeeks", "feePermille", "depositPermille" })
            Assert.True(problems.Any(p => p.Contains("requisitions." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void An_order_pays_a_deposit_now_and_is_checked()
    {
        Game g = Lab();
        g.Refused(Command.OpenRequisition(g.Id, "nobody", 1), CommandError.UnknownId);
        g.Refused(Command.OpenRequisition(g.Id, "engineer", 0), CommandError.InvalidArgument);
        g.Refused(Command.OpenRequisition(g.Id, "engineer", g.C.Requisitions.MaxCount + 1), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        long fee = Requisitions.LockedFee(g.C, g.State, g.Me, g.C.Role("engineer"));
        g.Do(Command.OpenRequisition(g.Id, "engineer", 2));
        Assert.True(fee < RolePerks.HireCost(g.C, g.Me, g.C.Role("engineer"), 1, g.State.Tick));
        Assert.Equal(cash - Requisitions.DepositEach(g.C, fee) * 2, g.Me.Cash);
        Assert.Single(g.Me.Requisitions);
        Assert.Equal(fee, g.Me.Requisitions[0].Fee);
        for (int i = 0; i < 2; i++) g.Do(Command.OpenRequisition(g.Id, "safety", 1));
        g.Refused(Command.OpenRequisition(g.Id, "sales", 1), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.Cash = 0;
        poor.Refused(Command.OpenRequisition(poor.Id, "engineer", 1), CommandError.NotEnoughCash);
        Game off = Lab(c => c.Requisitions.Enabled = 0);
        off.Refused(Command.OpenRequisition(off.Id, "engineer", 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void On_the_day_the_people_arrive_for_the_locked_price_when_seats_and_cash_are_there()
    {
        Game g = Lab();
        int staff = g.Me.Staff.Count;
        g.Do(Command.OpenRequisition(g.Id, "engineer", 2));
        long fee = g.Me.Requisitions[0].Fee;
        g.Weeks(g.C.Requisitions.LeadWeeks - 1);
        Assert.Equal(staff, g.Me.Staff.Count);
        g.Weeks(1);
        Assert.Equal(staff + 2, g.Me.Staff.Count);
        Assert.Empty(g.Me.Requisitions);
        GameEvent end = Of(g, EventKind.RequisitionClosed).Single();
        Assert.Equal(1, end.Pick);
        Assert.Equal(2, end.Amount);
        Assert.True(fee > 0);
    }

    [Fact]
    public void The_price_is_locked_the_day_of_the_order()
    {
        Game g = Lab();
        g.Do(Command.OpenRequisition(g.Id, "engineer", 1));
        long locked = g.Me.Requisitions[0].Fee;
        g.C.Roles.First(r => r.Id == "engineer").HireCost *= 3;
        long today = RolePerks.HireCost(g.C, g.Me, g.C.Role("engineer"), 1, g.State.Tick);
        Assert.True(locked < today);
        g.Weeks(g.C.Requisitions.LeadWeeks);
        Assert.Equal(1, Of(g, EventKind.RequisitionClosed).Single().Pick);
        Assert.True(locked - Requisitions.DepositEach(g.C, locked) < today);
    }

    [Fact]
    public void Without_a_seat_or_the_cash_the_person_is_lost_and_so_is_the_deposit()
    {
        Game g = Lab(spare: 1);
        g.Do(Command.OpenRequisition(g.Id, "engineer", 2));
        int staff = g.Me.Staff.Count;
        g.Weeks(g.C.Requisitions.LeadWeeks);
        GameEvent end = Of(g, EventKind.RequisitionClosed).Single();
        Assert.Equal(0, end.Pick);
        Assert.Equal(1, end.Amount);
        Assert.Equal(staff + 1, g.Me.Staff.Count);
        Game broke = Lab();
        broke.Do(Command.OpenRequisition(broke.Id, "engineer", 1));
        broke.Weeks(broke.C.Requisitions.LeadWeeks - 1);
        broke.Me.Cash = 0;
        broke.Weeks(1);
        Assert.Equal(0, Of(broke, EventKind.RequisitionClosed).Single().Amount);
    }

    [Fact]
    public void Cancelling_forfeits_the_deposit_and_closes_the_order()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.OpenRequisition(g.Id, "engineer", 1));
        long afterDeposit = g.Me.Cash;
        g.Refused(Command.CancelRequisition(g.Id, 3), CommandError.UnknownId);
        g.Do(Command.CancelRequisition(g.Id, 0));
        Assert.Empty(g.Me.Requisitions);
        Assert.Equal(afterDeposit, g.Me.Cash);
        Assert.True(g.Me.Cash < cash);
        Assert.Equal(-1, Of(g, EventKind.RequisitionClosed).Single().Pick);
    }

    [Fact]
    public void A_save_with_open_orders_loads_and_plays_on_the_same_and_an_old_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.OpenRequisition(g.Id, "engineer", 2));
        g.Weeks(3);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Do(Command.OpenRequisition(g.Id, "safety", 1));
        g.Me.Requisitions[0].Count = 99;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("order for people"));
        Game fresh = Lab();
        string old = SaveSchemaPretend.At(fresh.C, 80, fresh.Sim.Save());
        Assert.DoesNotContain("requisitions", old);
        Simulation back = Simulation.Load(fresh.C, old);
        Assert.Empty(back.State.Labs.First(l => l.Id == fresh.Id).Requisitions);
    }
}
