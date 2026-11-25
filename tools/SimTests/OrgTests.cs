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

public class OrgTests
{
    static Game Lab(int researchers = 4, int engineers = 4, int safety = 3, int sales = 3, Action<GameConfig> more = null)
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
        }).WithStarterLab(researchers: researchers, engineers: engineers, safety: safety, sales: sales, gpus: 6);
        g.Me.Cash = 1_000_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.Senior = 0; p.Morale = 500; }
        return g;
    }

    static void Lead(Game g, string role)
    {
        g.Me.Staff.First(p => p.RoleId == role && p.Senior == 0).Senior = 1;
    }

    [Fact]
    public void A_lab_that_never_chooses_has_no_structure_and_no_effect()
    {
        Game g = Lab();
        Assert.Equal(0, g.Me.OrgForm);
        Assert.Equal(0, Org.Loss(g.C, g.Me));
        Assert.Equal(0, Org.ResearchPermille(g.C, g.Me));
        Assert.Equal(0, Org.TrainingPermille(g.C, g.Me));
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        g.Weeks(5);
        Assert.Equal(before, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.Me));
    }

    [Fact]
    public void The_choice_is_checked_and_cannot_be_repeated_soon()
    {
        Game g = Lab();
        g.Refused(Command.SetOrg(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetOrg(g.Id, 4), CommandError.InvalidArgument);
        g.Do(Command.SetOrg(g.Id, 1));
        Assert.Equal(1, g.Me.OrgForm);
        Assert.Equal(0, g.Me.OrgReorgs);
        g.Refused(Command.SetOrg(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.SetOrg(g.Id, 2), CommandError.InvalidArgument);
        g.Weeks(g.C.Org.CooldownWeeks);
        g.Do(Command.SetOrg(g.Id, 2));
        Assert.Equal(1, g.Me.OrgReorgs);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OrgChanged && e.SubjectId == 2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_flat_lab_pays_for_every_head_beyond_the_cap()
    {
        Game small = Lab(2, 2, 1, 1);
        small.Do(Command.SetOrg(small.Id, 1));
        small.Weeks(small.C.Org.ReorgWeeks);
        Assert.Equal(0, Org.Loss(small.C, small.Me));
        Game big = Lab();
        big.Do(Command.SetOrg(big.Id, 1));
        big.Weeks(big.C.Org.ReorgWeeks);
        int heads = big.Me.Staff.Count;
        Assert.Equal((heads - big.C.Org.FlatCap) * big.C.Org.FlatStep, Org.Loss(big.C, big.Me));
        Assert.True(Org.ResearchPermille(big.C, big.Me) < 0);
    }

    [Fact]
    public void A_reorganisation_disrupts_the_lab_for_a_while()
    {
        Game g = Lab(2, 2, 1, 1);
        g.Do(Command.SetOrg(g.Id, 1));
        Assert.Equal(g.C.Org.DisruptLoss, Org.Loss(g.C, g.Me));
        long settled = 0;
        g.Weeks(g.C.Org.ReorgWeeks);
        settled = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.Equal(0, g.Me.OrgDisrupt);
        Assert.Equal(0, Org.Loss(g.C, g.Me));
        Game plain = Lab(2, 2, 1, 1);
        Assert.Equal(TrainingSystem.ResearchPerWeek(plain.C, plain.Me, g.Tick), settled);
    }

    [Fact]
    public void A_functional_lab_needs_a_senior_for_each_function_and_gains_when_they_are_there()
    {
        Game g = Lab(3, 3, 2, 2);
        g.Do(Command.SetOrg(g.Id, 2));
        g.Weeks(g.C.Org.ReorgWeeks);
        int teams, led;
        Org.Functions(g.C, g.Me, out teams, out led);
        Assert.Equal(4, teams);
        Assert.Equal(0, led);
        Assert.Equal(4 * g.C.Org.LeadlessLoss, Org.Loss(g.C, g.Me));
        Assert.Equal(0, Org.ResearchBonus(g.C, g.Me));
        foreach (string role in g.Me.Staff.Select(p => p.RoleId).Distinct().ToList()) Lead(g, role);
        Org.Functions(g.C, g.Me, out teams, out led);
        Assert.Equal(teams, led);
        Assert.Equal(0, Org.Loss(g.C, g.Me));
        Assert.Equal(g.C.Org.FuncResearchBonus, Org.ResearchBonus(g.C, g.Me));
        Assert.True(Org.ResearchPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void A_functional_lab_loses_to_span_when_it_has_grown_too_big()
    {
        Game g = Lab(4, 4, 3, 3, c => c.Org.SpanSize = 2);
        g.Do(Command.SetOrg(g.Id, 2));
        g.Weeks(g.C.Org.ReorgWeeks);
        foreach (string role in g.Me.Staff.Select(p => p.RoleId).Distinct().ToList()) Lead(g, role);
        int heads = g.Me.Staff.Count;
        int teams, led;
        Org.Functions(g.C, g.Me, out teams, out led);
        Assert.Equal((heads - 2 * teams) * g.C.Org.SpanStep, Org.Loss(g.C, g.Me));
    }

    [Fact]
    public void Squads_gain_training_speed_only_with_enough_roles_and_a_lead_for_each_squad()
    {
        Game g = Lab(4, 4, 3, 3);
        g.Do(Command.SetOrg(g.Id, 3));
        g.Weeks(g.C.Org.ReorgWeeks);
        int squads = Org.Squad(g.C, g.Me);
        Assert.Equal(g.Me.Staff.Count / g.C.Org.SquadSize, squads);
        Assert.Equal(0, Org.TrainingBonus(g.C, g.Me));
        Assert.Equal(squads * g.C.Org.SquadLeadLoss + squads * g.C.Org.CrossTeam, Org.Loss(g.C, g.Me));
        for (int i = 0; i < squads; i++) g.Me.Staff.First(p => p.Senior == 0).Senior = 1;
        Assert.Equal(g.C.Org.SquadTrainingBonus, Org.TrainingBonus(g.C, g.Me));
        Assert.Equal(squads * g.C.Org.CrossTeam, Org.Loss(g.C, g.Me));
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        Game plain = Lab(4, 4, 3, 3);
        for (int i = 0; i < squads; i++) plain.Me.Staff.First(p => p.Senior == 0).Senior = 1;
        Assert.Equal(speed - (g.C.Org.SquadTrainingBonus - squads * g.C.Org.CrossTeam), TrainingSystem.SpeedPermille(plain.C, plain.Me));
        Game narrow = Lab(4, 4, 3, 3, c => c.Org.SquadMinRoles = 9);
        narrow.Do(Command.SetOrg(narrow.Id, 3));
        for (int i = 0; i < 3; i++) narrow.Me.Staff.First(p => p.Senior == 0).Senior = 1;
        Assert.Equal(0, Org.TrainingBonus(narrow.C, narrow.Me));
    }

    [Fact]
    public void The_loss_is_capped()
    {
        Game g = Lab(4, 4, 3, 3, c => { c.Org.FlatCap = 0; c.Org.FlatStep = 200; });
        g.Do(Command.SetOrg(g.Id, 1));
        Assert.Equal(g.C.Org.LossCap, Org.Loss(g.C, g.Me));
        g.Weeks(14);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OrgStrained);
    }

    [Fact]
    public void The_switch_and_older_saves_are_handled()
    {
        Game off = Lab(2, 2, 1, 1, c => c.Org.Enabled = 0);
        off.Refused(Command.SetOrg(off.Id, 1), CommandError.InvalidArgument);
        Game g = Lab();
        g.Do(Command.SetOrg(g.Id, 2));
        g.Weeks(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 111, g.Sim.Save()));
        Assert.Equal(0, older.State.Labs.First(l => l.Id == g.Id).OrgForm);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
