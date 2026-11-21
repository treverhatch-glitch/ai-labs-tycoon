using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q145: player command to spend cash and raise chip software maturity (no save schema).</summary>
public class ChipSoftwareTuneTests
{
    static Game AtChips(long cash = 2_000_000)
    {
        var g = ExpansionTestSupport.SoloAtYear(2018, cash).WithStarterLab(gpus: 8);
        if (!g.Me.Techs.Contains("chip-team")) g.Me.Techs.Add("chip-team");
        ExpansionTestSupport.BuildExpansionRoom(g, "chip-design-lab", 4, 3);
        return g;
    }

    [Fact]
    public void Tune_requires_architect()
    {
        Game g = AtChips();
        g.Me.SoftwarePermille = 200;
        g.Refused(Command.TuneChipSoftware(g.Id), CommandError.NotEnoughStaff);
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        Assert.True(g.Do(Command.TuneChipSoftware(g.Id)).Ok);
    }

    [Fact]
    public void Tune_spends_cash_and_raises_software()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        int before = g.Me.SoftwarePermille;
        long cash = g.Me.Cash;
        var r = g.Do(Command.TuneChipSoftware(g.Id));
        Assert.True(r.Ok);
        Assert.Equal(before + g.C.Chips.SoftwareTuneGainPermille, g.Me.SoftwarePermille);
        Assert.Equal(cash - g.C.Chips.SoftwareTuneCashCost, g.Me.Cash);
    }

    [Fact]
    public void Tune_refused_at_cap()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Me.SoftwarePermille = g.C.Chips.SoftwareCap;
        g.Refused(Command.TuneChipSoftware(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Tune_clamps_at_cap()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Me.SoftwarePermille = g.C.Chips.SoftwareCap - 10;
        Assert.True(g.Do(Command.TuneChipSoftware(g.Id)).Ok);
        Assert.Equal(g.C.Chips.SoftwareCap, g.Me.SoftwarePermille);
    }

    [Fact]
    public void Tune_needs_cash_when_cost_positive()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Me.Cash = g.C.Chips.SoftwareTuneCashCost - 1;
        g.Refused(Command.TuneChipSoftware(g.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Tune_save_load_unchanged_schema()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Do(Command.TuneChipSoftware(g.Id));
        int soft = g.Me.SoftwarePermille;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(soft, loaded.Me.SoftwarePermille);
        Assert.Equal(24, loaded.State.Schema);
    }

    [Fact]
    public void Config_validates_tune_fields()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chips.SoftwareTuneGainPermille > 0);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Bankrupt_lab_tune_refused()
    {
        Game g = AtChips();
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Me.Bankrupt = true;
        g.Refused(Command.TuneChipSoftware(g.Id), CommandError.LabBankrupt);
    }
}