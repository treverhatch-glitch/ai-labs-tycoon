using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// How a training run is set up (bible ch. 6, the recipe table: "architecture: ... your own experimental variants. New designs have higher
    /// ceilings and fail more often"; "runs can fail: divergence, a data bug found in week nine, a power cut. You can checkpoint more often at a cost").
    /// The player picks an approach and a checkpoint habit while a model trains. A standard run never fails and a rare-checkpoint habit is free, so a
    /// run nobody touched is exactly what it was. Rivals and the autopilot never choose.
    /// </summary>
    public sealed class RunsConfig : ISync
    {
        public string DefaultApproachId = "";
        public string DefaultCheckpointId = "";
        /// <summary>Each engineer takes this share off a run's weekly failure chance, in permille of the chance (bible ch. 7: engineers mean "fewer failed runs").</summary>
        public int EngineerSavingPermillePerStaff;
        /// <summary>The most the engineers can take off, in permille of the chance.</summary>
        public int EngineerSavingCapPermille;
        public List<ApproachConfig> Approaches = new List<ApproachConfig>();
        public List<CheckpointConfig> Checkpoints = new List<CheckpointConfig>();

        public ApproachConfig Approach(string id)
        {
            for (int i = 0; i < Approaches.Count; i++) if (Approaches[i].IdValue == id) return Approaches[i];
            return null;
        }

        public CheckpointConfig Checkpoint(string id)
        {
            for (int i = 0; i < Checkpoints.Count; i++) if (Checkpoints[i].IdValue == id) return Checkpoints[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Str("defaultApproachId", ref DefaultApproachId);
            s.Str("defaultCheckpointId", ref DefaultCheckpointId);
            s.Int("engineerSavingPermillePerStaff", ref EngineerSavingPermillePerStaff);
            s.Int("engineerSavingCapPermille", ref EngineerSavingCapPermille);
            s.ObjList("approaches", ref Approaches);
            s.ObjList("checkpoints", ref Checkpoints);
        }
    }

    public sealed class ApproachConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Text = "";
        /// <summary>Added to the model's capability when the run finishes, in permille of it.</summary>
        public int CapabilityPermille;
        /// <summary>The chance each training week that the run fails, in permille.</summary>
        public int FailurePermillePerWeek;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("text", ref Text);
            s.Int("capabilityPermille", ref CapabilityPermille);
            s.Int("failurePermillePerWeek", ref FailurePermillePerWeek);
        }
    }

    public sealed class CheckpointConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Text = "";
        /// <summary>The share of the run's progress (and its safety work) a failure throws away, in permille.</summary>
        public int LossPermille;
        /// <summary>What each training week of this habit costs, in permille of a week of running the lab.</summary>
        public int WeeklyCostPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("text", ref Text);
            s.Int("lossPermille", ref LossPermille);
            s.Int("weeklyCostPermille", ref WeeklyCostPermille);
        }
    }
}
