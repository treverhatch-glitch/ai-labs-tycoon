using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>NEXT items 9–11 host explain: sector flavours, safety culture, version naming (batch 366).</summary>
public class HostExplainNext366LaterTests
{
    [Fact]
    public void Sector_flavours_expose_trust_permille_for_each_sector()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Safety.SectorFlavours.Count >= 1);
        var g = Game.Solo().WithStarterLab();
        foreach (SectorFlavourConfig f in c.Safety.SectorFlavours)
            Assert.InRange(SectorFlavours.TrustPermille(c, g.State, g.Me, f.Sector), 0, 2000);
    }

    [Fact]
    public void Culture_band_matches_current_level()
    {
        var g = Game.Solo();
        int level = SafetyCulture.Level(g.C, g.Me);
        CultureBandConfig band = SafetyCulture.Band(g.C, level);
        Assert.False(string.IsNullOrEmpty(band.Id));
        Assert.True(level >= band.Min);
    }

    [Fact]
    public void Naming_mess_and_appeal_permille_track_config()
    {
        var g = Game.Solo().WithStarterLab();
        Assert.True(VersionNaming.Mess(g.C, g.Me) >= 0);
        Assert.InRange(VersionNaming.AppealPermille(g.C, g.Me), 1000 - g.C.Naming.ConfusionPermille, 1000);
    }
}