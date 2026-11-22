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

public class GovernanceTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 2, safety: 1, gpus: 4);
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 600;
        return g;
    }

    static bool Ousted(Game g) { return g.State.Endings.Any(e => e.EndingId == "ousted"); }

    [Fact]
    public void An_ordinary_company_changes_nothing_and_is_ousted_below_the_control_line()
    {
        Game g = Lab();
        Assert.Equal(g.Me.FounderSharePermille, Governance.ControlShare(g.C, g.Me));
        Assert.Equal(1234, Governance.RaiseCash(g.C, g.Me, 1234));
        Assert.Equal(500, Governance.DramaChance(g.C, g.Me, 500));
        g.Me.FounderSharePermille = g.C.Funding.ControlPermille;
        FundingSystem.TryOust(g.C, g.State, g.Me);
        Assert.True(Ousted(g));
    }

    [Fact]
    public void The_choice_is_checked_permanent_and_only_early()
    {
        Game off = Lab(c => c.Governance.Enabled = 0);
        off.Refused(Command.SetGovernance(off.Id, 1), CommandError.InvalidArgument);
        Game g = Lab();
        g.Refused(Command.SetGovernance(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetGovernance(g.Id, 3), CommandError.InvalidArgument);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Refused(Command.SetGovernance(g.Id, 2), CommandError.InvalidArgument);
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Refused(Command.SetGovernance(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.SetGovernance(g.Id, 2), CommandError.InvalidArgument);
        Game late = Lab();
        late.Do(Command.RaiseFunding(late.Id, "angel"));
        late.Do(Command.RaiseFunding(late.Id, "series-a"));
        late.Refused(Command.SetGovernance(late.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(0, late.Me.GovForm);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Dual_class_shares_keep_control_at_a_low_share_and_cost_cash_in_later_rounds()
    {
        Game plain = Lab();
        plain.Do(Command.RaiseFunding(plain.Id, "angel"));
        Game dual = Lab();
        dual.Do(Command.SetGovernance(dual.Id, 1));
        dual.Do(Command.RaiseFunding(dual.Id, "angel"));
        long cut = 500_000L * (1000 - dual.C.Governance.DualCashCut) / 1000;
        Assert.Equal(cut, dual.Me.Ledger.Lifetime.CapitalIn);
        Assert.True(dual.Me.Ledger.Lifetime.CapitalIn < plain.Me.Ledger.Lifetime.CapitalIn);
        dual.Me.FounderSharePermille = dual.C.Funding.ControlPermille / 4 + 10;
        Assert.True(Governance.ControlShare(dual.C, dual.Me) > dual.C.Funding.ControlPermille);
        FundingSystem.TryOust(dual.C, dual.State, dual.Me);
        Assert.False(Ousted(dual));
        dual.Me.FounderSharePermille = dual.C.Funding.ControlPermille / 4 - 10;
        FundingSystem.TryOust(dual.C, dual.State, dual.Me);
        Assert.True(Ousted(dual));
    }

    [Fact]
    public void A_non_profit_parent_cannot_be_outvoted_and_costs_more_cash()
    {
        Game g = Lab();
        g.Do(Command.SetGovernance(g.Id, 2));
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(500_000L * (1000 - g.C.Governance.NonprofitCashCut) / 1000, g.Me.Ledger.Lifetime.CapitalIn);
        g.Me.FounderSharePermille = 1;
        Assert.Equal(1000, Governance.ControlShare(g.C, g.Me));
        FundingSystem.TryOust(g.C, g.State, g.Me);
        Assert.False(Ousted(g));
    }

    [Fact]
    public void Finding_a_director_takes_weeks_a_fee_and_a_reputation()
    {
        Game g = Lab();
        long before = g.Me.Ledger.Lifetime.Hiring;
        g.Do(Command.SeatDirector(g.Id));
        Assert.Equal(before - g.C.Governance.DirectorFee, g.Me.Ledger.Lifetime.Hiring);
        g.Refused(Command.SeatDirector(g.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Governance.DirectorWeeks - 1);
        Assert.Equal(0, g.Me.GovDirectors);
        g.Weeks(1);
        Assert.Equal(1, g.Me.GovDirectors);
        Assert.Equal(0, g.Me.GovSearch);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DirectorSeated);
        Game low = Lab();
        low.Me.Reputation = 0;
        low.Refused(Command.SeatDirector(low.Id), CommandError.MissingPrerequisite);
        Game poor = Lab();
        poor.Me.Cash = 0;
        poor.Refused(Command.SeatDirector(poor.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Directors_are_capped_cost_a_weekly_sum_and_calm_the_board()
    {
        Game g = Lab();
        for (int i = 0; i < g.C.Governance.MaxDirectors; i++)
        {
            g.Do(Command.SeatDirector(g.Id));
            g.Weeks(g.C.Governance.DirectorWeeks);
        }
        Assert.Equal(g.C.Governance.MaxDirectors, g.Me.GovDirectors);
        g.Refused(Command.SeatDirector(g.Id), CommandError.InvalidArgument);
        long before = g.Me.Ledger.Lifetime.Salaries;
        g.Weeks(1);
        Assert.True(before - g.Me.Ledger.Lifetime.Salaries >= Governance.WeeklyCost(g.C, g.Me));
        Assert.Equal(1000, Governance.DramaChance(g.C, new LabState(), 1000));
        Assert.Equal(1000 * (1000 - g.C.Governance.MaxDirectors * g.C.Governance.DramaCut) / 1000, Governance.DramaChance(g.C, g.Me, 1000));
    }

    [Fact]
    public void A_director_resigns_when_the_lab_cannot_pay()
    {
        Game g = Lab();
        g.Do(Command.SeatDirector(g.Id));
        g.Weeks(g.C.Governance.DirectorWeeks);
        g.Me.Cash = 0;
        Governance.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.GovDirectors);
        Assert.Equal(1, g.Me.GovResigned);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DirectorResigned);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_are_ordinary_companies()
    {
        Game g = Lab();
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.SeatDirector(g.Id));
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 116, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.GovForm + lab.GovDirectors + lab.GovSearch);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
