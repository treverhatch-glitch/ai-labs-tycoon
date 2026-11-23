using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Governance commands (batch 15–16) refuse rival labs and spectate mode.</summary>
public class EraGovernancePlayerOnlyHardeningTests
{
    static Game WithRival(long seed = 180)
    {
        return Game.New(seed, c =>
        {
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.MarginCall.Enabled = 1;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.ScenarioBaggage.Enabled = 1;
            c.TakeoffCrisis.MinEra = 0;
            c.RungSixCrisis.MinEra = 0;
            c.Office.ChancePermille = 0;
        });
    }

    [Fact]
    public void Campaign_deadline_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SetCampaignDeadline(rival.Id, g.Tick + 50), CommandError.InvalidArgument);
    }

    [Fact]
    public void Shape_law_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.ShapeLaw(rival.Id, rival.HomeBlocId, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pick_scenario_baggage_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.PickScenarioBaggage(rival.Id, "early-lawsuit"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Respond_takeoff_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.RespondTakeoff(rival.Id, "contain"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Set_law_target_refuses_rival_lab_id()
    {
        Game g = WithRival();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SetLawTarget(rival.Id, LawShaping.Strict), CommandError.InvalidArgument);
    }

    [Fact]
    public void Satisfy_margin_call_refuses_rival_lab_id()
    {
        Game g = WithRival(181);
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SatisfyMarginCall(rival.Id, 0, 1000), CommandError.InvalidArgument);
    }

    [Fact]
    public void Respond_rung_six_refuses_rival_lab_id()
    {
        Game g = WithRival(182);
        LabState rival = g.Rival("openaye");
        g.Refused(Command.RespondRungSix(rival.Id, "hold"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Spectate_refuses_set_campaign_deadline()
    {
        Game g = WithRival(183);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SetCampaignDeadline(playerId, g.Tick + 40), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_shape_law()
    {
        Game g = WithRival(184);
        int playerId = g.Id;
        string home = g.Me.HomeBlocId;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.ShapeLaw(playerId, home, LawShaping.Loose), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_set_law_target()
    {
        Game g = WithRival(185);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SetLawTarget(playerId, LawShaping.Strict), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_satisfy_margin_call()
    {
        Game g = WithRival(186);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.SatisfyMarginCall(playerId, 0, 1), CommandError.Spectating);
    }

    [Fact]
    public void Spectate_refuses_respond_rung_six()
    {
        Game g = WithRival(187);
        int playerId = g.Id;
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Do(Command.SellLab(playerId, "spectate"));
        g.Refused(Command.RespondRungSix(playerId, "hold"), CommandError.Spectating);
    }
}