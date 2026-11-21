using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// What a training run is for (bible ch. 6, the recipe table: "be correct, be engaging, be obedient, be careful"). The player picks one while a
    /// model trains; its effects land when the run finishes. A model with no objective chosen is trained to be correct, which changes nothing.
    /// Rivals and the autopilot never choose one.
    /// </summary>
    public sealed class ObjectivesConfig : ISync
    {
        public string DefaultId = "";
        public List<ObjectiveConfig> Objectives = new List<ObjectiveConfig>();

        public ObjectiveConfig Objective(string id)
        {
            for (int i = 0; i < Objectives.Count; i++) if (Objectives[i].IdValue == id) return Objectives[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Str("defaultId", ref DefaultId);
            s.ObjList("objectives", ref Objectives);
        }
    }

    public sealed class ObjectiveConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>What it asks of the model, in a sentence.</summary>
        public string Text = "";
        /// <summary>Added to the model's real and shown alignment when the run finishes, in points (never below 0 or above 1000).</summary>
        public int AlignmentPoints;
        /// <summary>Added to the model's capability when the run finishes, in permille of it.</summary>
        public int CapabilityPermille;
        /// <summary>Added to what customers feel about the model, in permille of its appeal in every sector.</summary>
        public int AppealPermille;
        /// <summary>Safety culture points moved when the run finishes (negative erodes it).</summary>
        public int CulturePoints;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("text", ref Text);
            s.Int("alignmentPoints", ref AlignmentPoints);
            s.Int("capabilityPermille", ref CapabilityPermille);
            s.Int("appealPermille", ref AppealPermille);
            s.Int("culturePoints", ref CulturePoints);
        }
    }
}
