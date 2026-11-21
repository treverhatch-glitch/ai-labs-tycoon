using System;
using System.Collections.Generic;
using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using Xunit;

namespace AiLabs.SimTests.Support;

/// <summary>Where the tests get the game's data and the repository's files.</summary>
public static class TestData
{
    public static string ConfigJson()
    {
        return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "default-config.json"));
    }

    /// <summary>A fresh copy of the shipped config. Each test gets its own, so a tweak never leaks.</summary>
    public static GameConfig Config(Action<GameConfig> tweak = null)
    {
        GameConfig config = GameConfig.FromJson(ConfigJson());
        if (tweak != null)
        {
            tweak(config);
            ConfigValidator.Validate(config);
        }
        return config;
    }

    /// <summary>The repository root: the folder that holds Rules/AiLabs.Rules.csproj.</summary>
    public static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Rules", "AiLabs.Rules.csproj"))) dir = dir.Parent;
        Assert.True(dir != null, "Could not find the repository root above " + AppContext.BaseDirectory);
        return dir.FullName;
    }
}

/// <summary>
/// A game with the player's hands on it, for tests that read like a play session:
/// build a room, hire, train, wait, look at the result.
/// </summary>
public sealed class Game
{
    public readonly Simulation Sim;

    public Game(Simulation sim) { Sim = sim; }

    public GameConfig C { get { return Sim.Config; } }
    public GameState State { get { return Sim.State; } }
    public LabState Me { get { return Sim.Player; } }
    public int Id { get { return Sim.Player.Id; } }
    public int Tick { get { return Sim.Tick; } }

    /// <summary>A game against every rival in the config.</summary>
    public static Game New(long seed = 1, Action<GameConfig> tweak = null)
    {
        return new Game(Simulation.NewGame(TestData.Config(tweak), seed));
    }

    /// <summary>
    /// A game with no rivals, for looking at one mechanic on its own. A lab alone owns the whole market, which would
    /// make every government audit it; the dominance rule is switched off here so that mechanic can be looked at alone too.
    /// </summary>
    public static Game Solo(long seed = 1, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string>() };
        GameConfig config = TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            if (tweak != null) tweak(c);
        });
        return new Game(Simulation.NewGame(config, options));
    }

    public LabState Rival(string rivalId)
    {
        foreach (LabState lab in State.Labs) if (lab.RivalId == rivalId) return lab;
        throw new ArgumentException("No rival " + rivalId);
    }

    public CommandResult Try(Command command) { return Sim.Apply(command); }

    /// <summary>Gives a command that must go through.</summary>
    public CommandResult Do(Command command)
    {
        CommandResult result = Sim.Apply(command);
        Assert.True(result.Ok, command.Kind + " was refused: " + result.Message);
        return result;
    }

    /// <summary>Gives a command that must be refused for this reason, and checks it changed nothing.</summary>
    public void Refused(Command command, CommandError error)
    {
        string before = Sim.Digest();
        CommandResult result = Sim.Apply(command);
        Assert.False(result.Ok, command.Kind + " should have been refused");
        Assert.Equal(error, result.Error);
        Assert.False(string.IsNullOrEmpty(result.Message), "a refusal must say why");
        Assert.Equal(before, Sim.Digest());
    }

    /// <summary>Builds a room in the first free spot and returns its id.</summary>
    public int Build(string roomTypeId, int width, int height)
    {
        Assert.True(LotGrid.FindFreeRect(C.Lot(Me.LotId), Me.Rooms, width, height, out int x, out int y),
            "No free " + width + "x" + height + " spot on the lot");
        return Do(Command.BuildRoom(Id, roomTypeId, x, y, width, height)).CreatedId;
    }

    public void Hire(string roleId, int count) { Do(Command.Hire(Id, roleId, count)); }

    public void Rent(int gpus) { Do(Command.SetRentedGpus(Id, gpus)); }

    public void Research(string techId) { Do(Command.ResearchTech(Id, techId)); }

    /// <summary>Researches every tech the lab can still buy, respecting forks and prerequisites.</summary>
    public void ResearchAllReachable(long points = 9_999_999)
    {
        GrantResearch(points);
        bool bought = true;
        while (bought)
        {
            bought = false;
            foreach (TechConfig tech in C.Research.Techs)
            {
                if (!LabQueries.CanResearch(C, Me, tech, Tick)) continue;
                Research(tech.Id);
                bought = true;
            }
        }
    }

    public ModelState Train(string lineId, int alignmentSharePermille)
    {
        return Me.Model(Do(Command.StartTraining(Id, lineId, alignmentSharePermille)).CreatedId);
    }

    public void Release(ModelState model, params string[] sectors) { Do(Command.ReleaseModel(Id, model.Id, sectors)); }

    public void Weeks(int weeks) { Sim.StepMany(weeks); }

    public void Years(int years) { Sim.StepMany(years * C.Calendar.WeeksPerYear); }

    /// <summary>Steps until something is true. Fails the test when it takes longer than allowed.</summary>
    public int Until(Func<bool> done, int maxWeeks, string what)
    {
        int weeks = 0;
        while (!done())
        {
            Assert.True(weeks < maxWeeks, what + " did not happen within " + maxWeeks + " weeks");
            Sim.Step();
            weeks++;
        }
        return weeks;
    }

    public int UntilTrained(ModelState model, int maxWeeks = 200)
    {
        return Until(() => model.Status != ModelStatus.Training, maxWeeks, model.Name + " finishing training");
    }

    /// <summary>Gives the lab free research points, for tests that are not about earning them.</summary>
    public void GrantResearch(long points) { Me.ResearchPoints += points; }

    /// <summary>
    /// A small working lab: desks for every role, a few people and some rented GPUs.
    /// Costs about $40,000 of the starting cash.
    /// </summary>
    public Game WithStarterLab(int researchers = 2, int engineers = 1, int safety = 0, int sales = 1, int gpus = 6)
    {
        Build("research-lab", 4, 4);
        Build("engineering-bay", 4, 4);
        Build("safety-lab", 4, 3);
        Build("sales-office", 4, 3);
        if (researchers > 0) Hire("researcher", researchers);
        if (engineers > 0) Hire("engineer", engineers);
        if (safety > 0) Hire("safety", safety);
        if (sales > 0) Hire("sales", sales);
        Rent(gpus);
        return this;
    }

    /// <summary>Lets the config's autopilot run the player's lab for a number of weeks.</summary>
    public void Autopilot(int weeks, PlanConfig plan = null)
    {
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(C.Calendar, Sim.Tick)) Sim.RunPlan(Id, plan ?? C.Autopilot);
            Sim.Step();
        }
    }

    public long Share(string sectorId, LabState lab)
    {
        SectorEntry entry = State.Sector(sectorId).Entry(lab.Id);
        return entry == null ? 0 : entry.SharePpm;
    }

    public LabState CrownHolder()
    {
        Assert.True(State.Gauntlets.Count > 0, "No Gauntlet has been held yet");
        return State.Lab(State.Gauntlets[State.Gauntlets.Count - 1].CrownLabId);
    }
}
