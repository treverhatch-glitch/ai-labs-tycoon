using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Named starts (bible ch. 15, "Scenarios: start as a named archetype in a specific year"): an era, a founder, a style, a home and a name, ready to pick.</summary>
    public sealed class ScenariosConfig : ISync
    {
        public List<ScenarioConfig> Scenarios = new List<ScenarioConfig>();

        public ScenarioConfig Scenario(string id)
        {
            for (int i = 0; i < Scenarios.Count; i++) if (Scenarios[i].IdValue == id) return Scenarios[i];
            return null;
        }

        public void Sync(Syncer s) { s.ObjList("scenarios", ref Scenarios); }
    }

    public sealed class ScenarioConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>What the scenario is about, in a sentence or two, for the picker.</summary>
        public string Blurb = "";
        /// <summary>The era the game starts in (a startable era's id).</summary>
        public string EraId = "";
        public string FounderId = "";
        public string StyleId = "";
        /// <summary>The bloc the lab is incorporated in.</summary>
        public string HomeBlocId = "";
        public string LabName = "";
        public string Leader = "";
        public string FamilyName = "";
        /// <summary>The rivals to play against; empty for all of them.</summary>
        public List<string> RivalIds = new List<string>();

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.Str("eraId", ref EraId);
            s.Str("founderId", ref FounderId);
            s.Str("styleId", ref StyleId);
            s.Str("homeBlocId", ref HomeBlocId);
            s.Str("labName", ref LabName);
            s.Str("leader", ref Leader);
            s.Str("familyName", ref FamilyName);
            s.StrList("rivalIds", ref RivalIds);
        }
    }
}
