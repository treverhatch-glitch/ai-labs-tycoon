using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class WaitlistTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    static ModelState Add(Game g, ModelStatus status)
    {
        ModelState m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = status, Alignment = 500, Name = "Waiting" };
        g.Me.Models.Add(m);
        return m;
    }

    [Fact]
    public void Opening_costs_nothing_and_hype_builds_each_week()
    {
        Game g = Lab();
        ModelState m = Add(g, ModelStatus.Training);
        long cash = g.Me.Cash;
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 10));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(10, g.Me.WlLeft);
        g.Weeks(3);
        Assert.Equal(3 * g.C.Waitlist.HypeStep, g.Me.WlHype);
        Assert.Equal(7, g.Me.WlLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_an_unreleased_own_model_and_a_sane_promise_are_allowed()
    {
        Game g = Lab();
        ModelState out1 = Add(g, ModelStatus.Released);
        ModelState gone = Add(g, ModelStatus.Retired);
        ModelState wip = Add(g, ModelStatus.Trained);
        g.Refused(Command.OpenWaitlist(g.Id, 99999, 10), CommandError.UnknownId);
        g.Refused(Command.OpenWaitlist(g.Id, out1.Id, 10), CommandError.InvalidArgument);
        g.Refused(Command.OpenWaitlist(g.Id, gone.Id, 10), CommandError.InvalidArgument);
        g.Refused(Command.OpenWaitlist(g.Id, wip.Id, g.C.Waitlist.MinWeeks - 1), CommandError.InvalidArgument);
        g.Refused(Command.OpenWaitlist(g.Id, wip.Id, g.C.Waitlist.MaxWeeks + 1), CommandError.InvalidArgument);
        g.Do(Command.OpenWaitlist(g.Id, wip.Id, 10));
        g.Refused(Command.OpenWaitlist(g.Id, wip.Id, 10), CommandError.InvalidArgument);
    }

    [Fact]
    public void Shipping_in_time_cashes_the_hype_in_as_reputation()
    {
        Game g = Lab();
        ModelState m = Add(g, ModelStatus.Trained);
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 20));
        g.Weeks(8);
        int hype = g.Me.WlHype;
        long rep = g.Me.Reputation;
        m.Status = ModelStatus.Released;
        g.Weeks(1);
        Assert.Equal(0, g.Me.WlModel);
        Assert.Equal(1, g.Me.WlWins);
        Assert.True(g.Me.Reputation >= rep + MathI.MulPermille(hype, g.C.Waitlist.WinPerThousand) - 5);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.WaitlistEnded && e.Pick == 1);
    }

    [Fact]
    public void A_longer_wait_builds_more_hype_to_win_or_lose()
    {
        Game a = Lab();
        ModelState ma = Add(a, ModelStatus.Trained);
        a.Do(Command.OpenWaitlist(a.Id, ma.Id, 30));
        a.Weeks(4);
        Game b = Lab();
        ModelState mb = Add(b, ModelStatus.Trained);
        b.Do(Command.OpenWaitlist(b.Id, mb.Id, 30));
        b.Weeks(12);
        Assert.True(b.Me.WlHype > a.Me.WlHype);
    }

    [Fact]
    public void A_lapsed_promise_costs_more_than_closing_the_list()
    {
        Game lapse = Lab();
        ModelState ml = Add(lapse, ModelStatus.Training);
        lapse.Do(Command.OpenWaitlist(lapse.Id, ml.Id, 12));
        long before = lapse.Me.Reputation;
        lapse.Weeks(13);
        Assert.Equal(1, lapse.Me.WlFlops);
        long lapsed = before - lapse.Me.Reputation;
        Game close = Lab();
        ModelState mc = Add(close, ModelStatus.Training);
        close.Do(Command.OpenWaitlist(close.Id, mc.Id, 12));
        close.Weeks(11);
        long beforeC = close.Me.Reputation;
        close.Do(Command.CloseWaitlist(close.Id));
        Assert.Equal(0, close.Me.WlModel);
        Assert.Equal(0, close.Me.WlFlops);
        Assert.True(lapsed > beforeC - close.Me.Reputation);
        Assert.Contains(close.State.Events, e => e.Kind == EventKind.WaitlistEnded && e.Pick == 2);
    }

    [Fact]
    public void A_retired_model_lapses_the_list()
    {
        Game g = Lab();
        ModelState m = Add(g, ModelStatus.Trained);
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 20));
        g.Weeks(3);
        m.Status = ModelStatus.Retired;
        g.Weeks(1);
        Assert.Equal(0, g.Me.WlModel);
        Assert.Equal(1, g.Me.WlFlops);
        g.Refused(Command.CloseWaitlist(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_cannot_and_the_switch_turns_it_off()
    {
        Game g = Game.New(3);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Try(Command.OpenWaitlist(rival.Id, 1, 10)).Ok);
        Game off = Lab(c => c.Waitlist.Enabled = 0);
        ModelState m = Add(off, ModelStatus.Trained);
        off.Refused(Command.OpenWaitlist(off.Id, m.Id, 10), CommandError.InvalidArgument);
    }

    [Fact]
    public void An_open_list_survives_save_and_an_older_save_has_none()
    {
        Game g = Lab();
        ModelState m = Add(g, ModelStatus.Trained);
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 20));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 100, g.Sim.Save());
        Simulation o = Simulation.Load(g.C, old);
        Assert.Equal(0, o.State.Labs.First(l => l.Id == g.Id).WlModel);
        Assert.Empty(SaveGame.Problems(g.C, o.State));
    }
}
