using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraOSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 123)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.Lobbying.CooldownWeeks = 1;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.ScenarioBaggage.Enabled = 1;
        }).WithStarterLab();
        g.Me.Cash = 500_000_000_000;
        return g;
    }

    [Fact]
    public void Scenario_baggage_refuses_unknown_id()
    {
        Game g = Lab();
        g.Refused(Command.PickScenarioBaggage(g.Id, "missing"), CommandError.UnknownId);
    }

    [Fact]
    public void Law_shape_refuses_while_cooling()
    {
        Game g = Lab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Strict));
        g.Refused(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict), CommandError.LobbyCooling);
    }

    [Fact]
    public void Bankrupt_lab_with_deadline_and_baggage_validates_save()
    {
        Game g = Lab(124);
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 30));
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        g.Me.Cash = -20_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        back.StepMany(2);
        Assert.NotNull(back.Player);
    }
}