using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The foundry gives a slot to a bigger customer: a lab the vendor thinks little of waits longer for its own chips.</summary>
public class FoundrySnubTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rivals = false)
    {
        return ChipTests.Lab(2013, rivals: rivals, tweak: c =>
        {
            c.Chips.YieldCrashPermille = 0;
            foreach (ChipNodeConfig n in c.Chips.Nodes) n.RespinPermille = 0;
            c.Chips.SnubChancePermille = 1000;
            if (tweak != null) tweak(c);
        });
    }

    static int Order(Game g, LabState lab, int count = 1)
    {
        ChipDesign d = ChipTests.Ready(g, lab);
        int hall = ChipTests.Hall(g, lab);
        CommandResult r = g.Try(Command.OrderChips(lab.Id, d.Id, hall, count));
        Assert.True(r.Ok);
        return r.CreatedId;
    }

    static void Standing(Game g, LabState lab, int value)
    {
        VendorSystem.EnsureStanding(g.C, g.State, lab.Id).Standing = value;
    }

    [Fact]
    public void A_player_the_vendor_thinks_little_of_is_snubbed_and_the_delivery_slips()
    {
        Game g = Lab();
        Standing(g, g.Me, 200);
        int id = Order(g, g.Me);
        ChipOrder o = g.Me.ChipOrders.Single(x => x.Id == id);
        int now = ChipQueries.MonthIndex(g.C, g.Tick);
        Assert.Equal(ChipQueries.DeliveryTickOf(g.C, g.C.Chips.Node("n28"), now) + g.C.Chips.SnubWeeks, o.DeliveryTick);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ChipSlotSnubbed);
        Assert.Equal(g.C.Chips.SnubWeeks, e.Amount);
        Assert.Equal(id.ToString(), e.Ref);
        Assert.Contains("Foundry snub", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_lab_in_good_standing_is_never_snubbed()
    {
        Game g = Lab();
        Standing(g, g.Me, g.C.Chips.SnubStandingBelow);
        int id = Order(g, g.Me);
        int now = ChipQueries.MonthIndex(g.C, g.Tick);
        Assert.Equal(ChipQueries.DeliveryTickOf(g.C, g.C.Chips.Node("n28"), now), g.Me.ChipOrders.Single(x => x.Id == id).DeliveryTick);
        Assert.DoesNotContain(g.State.Events, x => x.Kind == EventKind.ChipSlotSnubbed);
    }

    [Fact]
    public void A_zero_chance_or_zero_weeks_switches_the_snub_off()
    {
        foreach (Action<GameConfig> off in new Action<GameConfig>[] { c => c.Chips.SnubChancePermille = 0, c => c.Chips.SnubWeeks = 0 })
        {
            Game g = Lab(off);
            Standing(g, g.Me, 0);
            Order(g, g.Me);
            Assert.DoesNotContain(g.State.Events, x => x.Kind == EventKind.ChipSlotSnubbed);
        }
    }

    [Fact]
    public void Rivals_are_never_snubbed()
    {
        Game g = Lab(rivals: true);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Standing(g, rival, 0);
        int before = g.State.Events.Count(x => x.Kind == EventKind.ChipSlotSnubbed);
        Order(g, rival);
        Assert.Equal(before, g.State.Events.Count(x => x.Kind == EventKind.ChipSlotSnubbed));
        Assert.Equal(0, ChipSystem.SnubWeeks(g.C, g.State, rival, 1));
    }

    [Fact]
    public void The_roll_is_a_pure_function_of_the_seed_the_week_the_lab_and_the_order()
    {
        Game g = Lab(c => c.Chips.SnubChancePermille = 500);
        Standing(g, g.Me, 0);
        int[] first = Enumerable.Range(1, 40).Select(i => ChipSystem.SnubWeeks(g.C, g.State, g.Me, i)).ToArray();
        int[] second = Enumerable.Range(1, 40).Select(i => ChipSystem.SnubWeeks(g.C, g.State, g.Me, i)).ToArray();
        Assert.Equal(first, second);
        int snubbed = first.Count(w => w > 0);
        Assert.InRange(snubbed, 8, 32);
    }

    [Fact]
    public void A_snubbed_order_still_arrives_and_holds_its_power_meanwhile()
    {
        Game g = Lab();
        Standing(g, g.Me, 100);
        int id = Order(g, g.Me, 2);
        ChipOrder o = g.Me.ChipOrders.Single(x => x.Id == id);
        Assert.True(o.ReservedKw > 0);
        g.Weeks(o.DeliveryTick - g.Tick + 1);
        Assert.DoesNotContain(g.Me.ChipOrders, x => x.Id == id);
        Assert.Contains(g.Me.Fleet, b => b.ChipDesignId == o.DesignId && b.Count >= 2);
    }

    [Fact]
    public void The_shipped_numbers_are_valid_and_the_validator_catches_bad_ones()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(350, c.Chips.SnubStandingBelow);
        Assert.Equal(250, c.Chips.SnubChancePermille);
        Assert.Equal(20, c.Chips.SnubWeeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Chips.SnubStandingBelow = 1001;
        c.Chips.SnubChancePermille = 1001;
        c.Chips.SnubWeeks = -1;
        var problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("snubStandingBelow"));
        Assert.Contains(problems, p => p.Contains("snubChancePermille"));
        Assert.Contains(problems, p => p.Contains("snubWeeks"));
    }
}
