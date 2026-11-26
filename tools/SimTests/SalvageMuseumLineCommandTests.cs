using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q149: player command to salvage one over-cap museum model.</summary>
public class SalvageMuseumLineCommandTests
{
    static Game G() => Game.Solo(1);

    static void AddRetired(Game g, string line, int cap, int version)
    {
        int when = g.State.Tick - 1000 + version;
        g.Me.Models.Add(new ModelState
        {
            Id = g.State.TakeId(), LineId = line, Version = version, Name = "v" + version,
            Status = ModelStatus.Retired, Capability = cap, CapBase = 100,
            StartedTick = when, TrainedTick = when, ReleasedTick = when,
        });
    }

    [Fact]
    public void Q149_main_salvage_removes_one_and_grants_research()
    {
        var g = G();
        for (int v = 0; v < 8; v++) AddRetired(g, "mini", 100, v);
        long before = g.Me.ResearchPoints;
        g.Do(Command.SalvageMuseumLine(g.Id, "mini"));
        Assert.Equal(7, g.Me.Models.Count(m => m.LineId == "mini"));
        Assert.True(g.Me.ResearchPoints > before);
    }

    [Fact]
    public void Q149_main_salvage_spends_config_cash()
    {
        var g = G();
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 100, v);
        long cash = g.Me.Cash;
        g.Do(Command.SalvageMuseumLine(g.Id, "mini"));
        Assert.Equal(cash - g.C.Endings.Museum.ManualSalvageCashCost, g.Me.Cash);
    }

    [Fact]
    public void Q149_zero_refused_at_cap()
    {
        var g = G();
        for (int v = 0; v < 6; v++) AddRetired(g, "mini", 100, v);
        g.Refused(Command.SalvageMuseumLine(g.Id, "mini"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q149_zero_permille_still_prunes_one_model()
    {
        var g = G();
        g.C.Endings.Museum.ResearchPermillePerCapability = 0;
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 50, v);
        g.Do(Command.SalvageMuseumLine(g.Id, "mini"));
        Assert.Equal(6, g.Me.Models.Count(m => m.LineId == "mini"));
    }

    [Fact]
    public void Q149_bankrupt_refused()
    {
        var g = G();
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 100, v);
        g.Me.Bankrupt = true;
        g.Refused(Command.SalvageMuseumLine(g.Id, "mini"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q149_save_load_counts_and_rp()
    {
        var g = G();
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 100, v);
        g.Do(Command.SalvageMuseumLine(g.Id, "mini"));
        long rp = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(rp, loaded.Me.ResearchPoints);
        Assert.Equal(6, loaded.Me.Models.Count(m => m.LineId == "mini"));
    }

    [Fact]
    public void Q149_max_capability_grant_uses_permille()
    {
        var g = Game.Solo(1, c => c.Endings.Museum.ResearchPermillePerCapability = 800);
        g.Me.Models.Clear();
        const string line = "mid";
        for (int v = 0; v < 7; v++) AddRetired(g, line, 100_000, v);
        long before = g.Me.ResearchPoints;
        g.Do(Command.SalvageMuseumLine(g.Id, line));
        long gain = g.Me.ResearchPoints - before;
        Assert.True(g.Me.Models.Count(m => m.LineId == line && m.Status == ModelStatus.Retired) == 6);
        Assert.Equal(MathI.MulPermille(100_000, 800), gain);
    }

    [Fact]
    public void Q149_needs_cash_when_configured()
    {
        var g = G();
        for (int v = 0; v < 7; v++) AddRetired(g, "mini", 100, v);
        g.Me.Cash = g.C.Endings.Museum.ManualSalvageCashCost - 1;
        g.Refused(Command.SalvageMuseumLine(g.Id, "mini"), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Q149_cross_salvage_then_weekly_auto_prune()
    {
        var g = G();
        for (int v = 0; v < 10; v++) AddRetired(g, "mini", 80, v);
        g.Do(Command.SalvageMuseumLine(g.Id, "mini"));
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Models.Count(m => m.LineId == "mini") <= g.C.Endings.Museum.PerLine);
    }
}