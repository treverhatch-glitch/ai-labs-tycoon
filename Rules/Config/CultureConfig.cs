using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Safety culture and the lab's own release gate (bible ch. 10). Culture is whether staff report problems or hide them: it rises when the lab
    /// listens and falls when it rushes, and it moves how many incidents are caught before they ship. The gate is a rule the player sets for the
    /// lab and then has to obey: no model goes on sale showing less alignment than the gate, and it cannot be loosened again for a while.
    /// </summary>
    public sealed class CultureConfig : ISync
    {
        /// <summary>The level a lab starts at, from 0 to 1000.</summary>
        public int Start;
        /// <summary>The level at which culture changes nothing; above it catches more incidents, below it fewer.</summary>
        public int NeutralLevel;
        /// <summary>Permille of incident catch gained per point of level above neutral (lost per point below).</summary>
        public int CatchSlopePermille;
        /// <summary>The roles whose people count as the lab listening to its own concerns.</summary>
        public List<string> ListeningRoles = new List<string>();
        public int StaffGainPerPerson;
        public int MaxStaffGain;
        /// <summary>Culture slips this much every week the lab does nothing to keep it.</summary>
        public int DriftPerWeek;
        /// <summary>A model that goes on sale showing less than this alignment is a rushed release.</summary>
        public int RushBelowAlignment;
        public int RushPoints;
        /// <summary>Gained when a model goes on sale through a gate the lab set for itself.</summary>
        public int GateKeptPoints;
        /// <summary>Lost when the lab loosens its own gate.</summary>
        public int GateLoweredPoints;
        public int ComplaintAnsweredPoints;
        public int ComplaintLapsedPoints;
        /// <summary>The highest alignment the gate can be set to.</summary>
        public int MaxGate;
        /// <summary>Weeks after the gate last changed before it can be loosened.</summary>
        public int GateLockWeeks;
        public List<CultureBandConfig> Bands = new List<CultureBandConfig>();

        public void Sync(Syncer s)
        {
            s.Int("start", ref Start);
            s.Int("neutralLevel", ref NeutralLevel);
            s.Int("catchSlopePermille", ref CatchSlopePermille);
            s.StrList("listeningRoles", ref ListeningRoles);
            s.Int("staffGainPerPerson", ref StaffGainPerPerson);
            s.Int("maxStaffGain", ref MaxStaffGain);
            s.Int("driftPerWeek", ref DriftPerWeek);
            s.Int("rushBelowAlignment", ref RushBelowAlignment);
            s.Int("rushPoints", ref RushPoints);
            s.Int("gateKeptPoints", ref GateKeptPoints);
            s.Int("gateLoweredPoints", ref GateLoweredPoints);
            s.Int("complaintAnsweredPoints", ref ComplaintAnsweredPoints);
            s.Int("complaintLapsedPoints", ref ComplaintLapsedPoints);
            s.Int("maxGate", ref MaxGate);
            s.Int("gateLockWeeks", ref GateLockWeeks);
            s.ObjList("bands", ref Bands);
        }
    }

    /// <summary>A name for a range of culture, for the screen: the band whose <see cref="Min"/> is the highest one at or below the level.</summary>
    public sealed class CultureBandConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int Min;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("min", ref Min);
            s.Str("blurb", ref Blurb);
        }
    }
}
