using System.IO;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The content tables in docs/content are read by people who plan the game. They must not drift from the data.</summary>
public class ContentDocsTests
{
    static string Page(string name)
    {
        return File.ReadAllText(Path.Combine(TestData.RepoRoot(), "docs", "content", name));
    }

    static void AssertListsEvery(string page, string what, string[] ids)
    {
        string text = Page(page);
        foreach (string id in ids) Assert.True(text.Contains("`" + id + "`"), page + " does not mention " + what + " `" + id + "`");
    }

    [Fact]
    public void The_eras_page_lists_every_era_tech_sector_and_room()
    {
        GameConfig c = TestData.Config();
        AssertListsEvery("eras.md", "era", c.Eras.Select(e => e.Id).ToArray());
        AssertListsEvery("eras.md", "tech", c.Research.Techs.Select(t => t.Id).ToArray());
        AssertListsEvery("eras.md", "sector", c.Market.Sectors.Select(s => s.Id).ToArray());
        AssertListsEvery("eras.md", "room", c.RoomTypes.Select(r => r.Id).ToArray());
    }

    [Fact]
    public void The_detail_pages_list_every_tech_sector_room_role_lot_and_world_event()
    {
        GameConfig c = TestData.Config();
        AssertListsEvery("techs.md", "tech", c.Research.Techs.Select(t => t.Id).ToArray());
        AssertListsEvery("sectors.md", "sector", c.Market.Sectors.Select(s => s.Id).ToArray());
        AssertListsEvery("rooms.md", "room", c.RoomTypes.Select(r => r.Id).ToArray());
        AssertListsEvery("rooms.md", "role", c.Roles.Select(r => r.Id).ToArray());
        AssertListsEvery("rooms.md", "lot", c.Lots.Select(l => l.Id).ToArray());
        AssertListsEvery("events.md", "world event", c.WorldEvents.Select(w => w.Id).ToArray());
    }

    [Fact]
    public void The_world_page_lists_every_bloc_site_power_deal_and_export_rule()
    {
        GameConfig c = TestData.Config();
        AssertListsEvery("world.md", "bloc", c.World.Blocs.Select(b => b.Id).ToArray());
        AssertListsEvery("world.md", "site", c.World.Sites.Select(s => s.Id).ToArray());
        AssertListsEvery("world.md", "power deal", c.World.PowerDeals.Select(d => d.Id).ToArray());
        AssertListsEvery("world.md", "export rule", c.World.ExportRules.Select(r => r.Id).ToArray());
    }

    [Fact]
    public void The_chips_page_lists_every_target_process_and_option()
    {
        GameConfig c = TestData.Config();
        AssertListsEvery("chips.md", "chip target", c.Chips.Targets.Select(t => t.IdValue).ToArray());
        AssertListsEvery("chips.md", "process", c.Chips.Nodes.Select(n => n.IdValue).ToArray());
        AssertListsEvery("chips.md", "chip option", c.Chips.Options.Select(o => o.IdValue).ToArray());
    }

    [Fact]
    public void The_rivals_page_lists_every_rival_and_the_vendor_supply_years()
    {
        GameConfig c = TestData.Config();
        AssertListsEvery("rivals.md", "rival", c.Rivals.Select(r => r.Id).ToArray());
        string text = Page("rivals.md");
        foreach (RivalConfig r in c.Rivals) Assert.Contains(r.Name, text);
        foreach (YearAmountConfig y in c.Vendor.SupplyByYear)
            Assert.Contains(y.Year + " " + y.Amount.ToString("N0", System.Globalization.CultureInfo.InvariantCulture), text);
    }

    [Fact]
    public void The_events_page_carries_every_incident_headline_and_world_headline()
    {
        GameConfig c = TestData.Config();
        string text = Page("events.md");
        foreach (EraConfig era in c.Eras)
            foreach (string headline in era.IncidentHeadlines) Assert.Contains(headline, text);
        foreach (WorldEventConfig e in c.WorldEvents) Assert.Contains(e.Headline, text);
    }

    [Fact]
    public void The_tech_page_carries_the_current_costs()
    {
        GameConfig c = TestData.Config();
        string text = Page("techs.md");
        foreach (TechConfig t in c.Research.Techs) Assert.Contains("| `" + t.Id + "` | " + t.Name + " | " + t.Cost.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + " |", text);
    }
}
