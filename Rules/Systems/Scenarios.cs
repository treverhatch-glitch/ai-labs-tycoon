using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;

namespace AiLabs.Rules.Systems
{
    /// <summary>Named starts: turns a scenario in the data into the options for a new game. Nothing is stored.</summary>
    public static class Scenarios
    {
        public static List<ScenarioConfig> List(GameConfig c) { return new List<ScenarioConfig>(c.Scenarios.Scenarios); }

        /// <summary>The options that start the scenario with this seed. Throws for an unknown scenario.</summary>
        public static NewGameOptions Options(GameConfig c, string scenarioId, long seed)
        {
            ScenarioConfig s = c.Scenarios.Scenario(scenarioId);
            if (s == null) throw new ArgumentException("There is no scenario \"" + scenarioId + "\" in the config.");
            var o = new NewGameOptions();
            o.Seed = seed;
            o.StartEraId = s.EraId;
            o.FounderId = s.FounderId;
            o.StyleId = s.StyleId;
            o.HomeBlocId = s.HomeBlocId;
            o.LabName = s.LabName;
            o.Leader = s.Leader;
            o.FamilyName = s.FamilyName;
            o.RivalIds = s.RivalIds.Count == 0 ? null : new List<string>(s.RivalIds);
            return o;
        }

        public static Simulation Start(GameConfig c, string scenarioId, long seed)
        {
            return Simulation.NewGame(c, Options(c, scenarioId, seed));
        }
    }
}
