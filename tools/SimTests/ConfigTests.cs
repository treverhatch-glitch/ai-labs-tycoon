using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The data file is the one place the game's numbers live. These tests keep it honest and on-design.</summary>
public class ConfigTests
{
    [Fact]
    public void The_shipped_config_loads_and_has_no_problems()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.False(string.IsNullOrEmpty(c.Version));
    }

    [Fact]
    public void The_config_survives_being_written_and_read_again()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(c.ToJson(), GameConfig.FromJson(c.ToJson()).ToJson());
    }

    [Fact]
    public void The_game_starts_in_1995_the_dial_up_days()
    {
        Assert.Equal(1995, TestData.Config().Calendar.StartYear);
    }

    [Fact]
    public void The_first_products_are_game_ai()
    {
        // Owner decision: the lab starts out licensing game AI to game studios and selling game-playing bots.
        GameConfig c = TestData.Config();
        List<SectorConfig> openAtStart = c.Market.Sectors.Where(s => s.OpensYear <= c.Calendar.StartYear).ToList();
        Assert.Equal(new[] { "game-ai", "game-bots" }, openAtStart.Select(s => s.Id).ToArray());
        Assert.Contains("game studios", c.Sector("game-ai").Buyers);

        // Simple expert systems come next, and the bars rise sector by sector: the lab evolves upward.
        SectorConfig expert = c.Sector("expert-systems");
        Assert.Equal(1996, expert.OpensYear);
        Assert.True(c.Sector("game-ai").MinCapability < expert.MinCapability);
        Assert.True(expert.MinCapability < c.Sector("spam-search").MinCapability);
    }

    [Fact]
    public void A_brand_new_lab_can_reach_game_ai_research_first()
    {
        GameConfig c = TestData.Config();
        string[] noPrerequisites = c.Research.Techs.Where(t => t.Requires.Count == 0).Select(t => t.Id).ToArray();
        Assert.Contains("state-machines", noPrerequisites);
        Assert.Contains("minimax", noPrerequisites);
        Assert.Contains("rules-engines", noPrerequisites);
        Assert.Equal(new[] { "state-machines" }, c.Tech("pathfinding").Requires.ToArray());
        Assert.All(c.Research.Techs, t => Assert.False(string.IsNullOrEmpty(t.Blurb), t.Id + " needs a blurb"));
    }

    [Fact]
    public void The_model_family_has_mini_mid_and_flagship_lines_and_only_mini_is_open_at_the_start()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "mini", "mid", "flagship", "code-specialist" }, c.Training.Lines.Select(l => l.Id).ToArray());
        Assert.False(c.Line("mini").Locked);
        Assert.True(c.Line("mid").Locked);
        Assert.True(c.Line("flagship").Locked);
        ModelLineConfig[] core = { c.Line("mini"), c.Line("mid"), c.Line("flagship") };
        for (int i = 1; i < core.Length; i++)
        {
            Assert.True(core[i].BaseCapability > core[i - 1].BaseCapability);
            Assert.True(core[i].ComputeRequired > core[i - 1].ComputeRequired);
        }
        Assert.True(c.Line("code-specialist").Locked);
    }

    [Fact]
    public void Owning_a_gpu_pays_for_itself_in_exactly_seventy_two_weeks_in_every_generation()
    {
        // Design table (bible v0.2): break-even near 18 months. The price is 72 weeks of what owning saves over renting.
        ComputeConfig compute = TestData.Config().Compute;
        foreach (GpuGenerationConfig gen in compute.GpuGenerations)
        {
            Assert.Equal(72 * (gen.RentPerGpuWeek - gen.UpkeepPerGpuWeek), gen.BuyPrice);
            Assert.InRange(gen.UpkeepPerGpuWeek * 1000 / gen.RentPerGpuWeek, 170, 195);
        }
    }

    [Fact]
    public void The_rivals_are_the_parody_cast()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "OpenAye", "AntHopticsAI", "xAy", "Metta", "Macrosoft", "Palantear", "Goggle DeepMined", "DeepSneak", "Second Wind Labs" }, c.Rivals.Select(r => r.Name).ToArray());
        Assert.Equal(new[] { 0, 0, 0, 2007, 2012, 2017, 2010, 2023, RivalConfig.NeverJoins }, c.Rivals.Select(r => r.JoinsYear).ToArray());
        Assert.Equal("Marka Zuckerg", c.Rival("metta").Leader);
        Assert.Equal("Taya Nadeva", c.Rival("macrosoft").Leader);
        Assert.Equal("K. Palant", c.Rival("palantear").Leader);
        Assert.Equal("Sunder Pitchai", c.Rival("goggle").Leader);
        Assert.Equal("The Quiet Committee", c.Rival("deepsneak").Leader);
        Assert.Equal("Jam Saltmen", c.Rival("openaye").Leader);
        Assert.Equal("Durian Amodayo", c.Rival("anthoptics").Leader);
        Assert.Equal("Elton Husk", c.Rival("xay").Leader);
        Assert.Equal("Envydia", c.Compute.VendorName);
        Assert.Equal("Jensun Hwang", c.Compute.VendorBoss);
    }

    [Fact]
    public void A_typo_in_the_data_file_is_caught_when_it_loads()
    {
        string json = TestData.ConfigJson().Replace("\"weeksPerMonth\"", "\"weeksPerMonht\"");
        ConfigException e = Assert.Throws<ConfigException>(() => GameConfig.FromJson(json));
        Assert.Contains("weeksPerMonth", e.Message);
    }

    [Fact]
    public void An_extra_key_in_the_data_file_is_caught_too()
    {
        string json = TestData.ConfigJson().Replace("\"eventLogMax\": 500,", "\"eventLogMax\": 500, \"evntLogMax\": 5,");
        ConfigException e = Assert.Throws<ConfigException>(() => GameConfig.FromJson(json));
        Assert.Contains("evntLogMax", e.Message);
    }

    [Fact]
    public void A_fraction_in_the_data_file_is_refused_because_the_rules_are_whole_numbers()
    {
        string json = TestData.ConfigJson().Replace("\"computePerGpuWeek\": 10", "\"computePerGpuWeek\": 10.5");
        Assert.Throws<ConfigException>(() => GameConfig.FromJson(json));
    }

    [Fact]
    public void Every_dangling_reference_is_reported_at_once()
    {
        GameConfig c = TestData.Config();
        c.Tech("pathfinding").Requires.Add("teleportation");
        c.RoomType("sales-office").SeatsRole = "wizard";
        c.Rival("xay").Plan.Sectors.Add("moon-mining");
        c.PlayerStart.LotId = "volcano-lair";

        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("teleportation"));
        Assert.Contains(problems, p => p.Contains("wizard"));
        Assert.Contains(problems, p => p.Contains("moon-mining"));
        Assert.Contains(problems, p => p.Contains("volcano-lair"));
        Assert.Contains(problems, p => p.Contains("role sales has no room type"));
    }

    [Fact]
    public void A_locked_line_nobody_can_unlock_and_a_tech_loop_are_config_problems()
    {
        GameConfig c = TestData.Config();
        c.Tech("expert-shells").Effects.Clear();
        c.Tech("state-machines").Requires.Add("pathfinding");

        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("line mid is locked and no tech unlocks it"));
        Assert.Contains(problems, p => p.Contains("requires itself through other techs"));
    }

    [Fact]
    public void Sector_weights_must_add_up()
    {
        GameConfig c = TestData.Config();
        c.Sector("game-ai").SafetyWeightPermille = 300;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("must add up to 1000"));
    }

    [Fact]
    public void Config_text_never_puts_words_in_a_real_persons_mouth()
    {
        // Parody guardrail: the cast is spoofed names only. No real lab, boss or product name appears in the data.
        string json = TestData.ConfigJson();
        foreach (string real in new[] { "OpenAI", "Anthropic", "Altman", "Amodei", "Musk", "Nvidia", "ChatGPT", "Claude\"", "Grok\"", "Huang" })
            Assert.DoesNotContain(real, json);

        // The arrivals and the chip designer add more names to keep out. Whole words, so "Voltage" is not "Volta".
        foreach (string real in new[] { "Google", "DeepMind", "Gemini", "Pichai", "Hassabis", "DeepSeek", "Liang", "TSMC", "CUDA", "Blackwell", "Hopper", "Volta", "Kepler", "GeForce", "Voodoo" })
            Assert.False(System.Text.RegularExpressions.Regex.IsMatch(json, "\\b" + real + "\\b"), "the config names " + real);
    }
}
