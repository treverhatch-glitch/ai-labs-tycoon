using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class CampaignDeadlineTests
{
    static Game Lab()
    {
        Game g = Game.Solo(41, c => { c.CampaignDeadline.MinEra = 0; c.Office.ChancePermille = 0; }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Set_deadline_and_hit_fires_continuing_ending()
    {
        Game g = Lab();
        int stop = g.Tick + 4;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        Assert.Equal(stop, g.Me.CampaignDeadlineTick);
        g.Weeks(3);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == g.C.CampaignDeadline.EndingId);
        g.Weeks(1);
        Assert.Contains(g.State.Endings, e => e.EndingId == g.C.CampaignDeadline.EndingId && e.Choice == "");
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit);
    }

    [Fact]
    public void Zero_deadline_is_sandbox()
    {
        Game g = Lab();
        g.Do(Command.SetCampaignDeadline(g.Id, 0));
        g.Weeks(20);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == g.C.CampaignDeadline.EndingId);
    }

    [Fact]
    public void Save_at_138_has_no_deadline_field()
    {
        Game g = Lab();
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 138, g.Sim.Save()));
        Assert.Equal(0, older.Player.CampaignDeadlineTick);
    }
}