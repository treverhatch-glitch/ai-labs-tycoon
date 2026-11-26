using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class StaffPeopleConfig : ISync
    {
        public int MoraleBaseline;
        public int MoraleDriftPerWeek;
        /// <summary>Extra morale lost each week while the lab is in debt.</summary>
        public int MoraleDebtDriftPerWeek;
        /// <summary>Morale lost by each staff member when a released model has an incident.</summary>
        public int MoraleIncidentDrop;
        /// <summary>Morale lost by remaining staff when a rival poaches a star.</summary>
        public int MoralePoachWitnessDrop;
        public int PoachChancePermille;
        /// <summary>Chance each hire is a star trait, in permille.</summary>
        public int StarHirePermille;
        /// <summary>Research output permille per point of average morale above <see cref="MoraleBaseline"/> (can go negative).</summary>
        public int MoraleResearchPermillePerPoint;
        /// <summary>Alignment work permille per point of average morale above baseline.</summary>
        public int MoraleAlignmentPermillePerPoint;
        /// <summary>Training speed permille per point of average morale above baseline.</summary>
        public int MoraleTrainingPermillePerPoint;
        /// <summary>Each point of morale above baseline cuts weekly training-fail risk by this much (permille points).</summary>
        public int MoraleFailCutPermillePerPoint;
        public int MoralePoachResistPermillePerPoint;
        public int MoraleIncidentMitigationPermillePerPoint;
        /// <summary>Mitigates <see cref="MoralePoachWitnessDrop"/> when morale is above baseline.</summary>
        public int MoralePoachWitnessMitigationPermillePerPoint;
        /// <summary>Cuts <see cref="MoraleDebtDriftPerWeek"/> when morale is above baseline.</summary>
        public int MoraleDebtMitigationPermillePerPoint;
        public List<StaffTraitConfig> Traits = new List<StaffTraitConfig>();

        public StaffTraitConfig Trait(string id)
        {
            for (int i = 0; i < Traits.Count; i++) if (Traits[i].Id == id) return Traits[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("moraleBaseline", ref MoraleBaseline);
            s.Int("moraleDriftPerWeek", ref MoraleDriftPerWeek);
            s.Int("moraleDebtDriftPerWeek", ref MoraleDebtDriftPerWeek);
            s.Int("moraleIncidentDrop", ref MoraleIncidentDrop);
            s.Int("moralePoachWitnessDrop", ref MoralePoachWitnessDrop);
            s.Int("poachChancePermille", ref PoachChancePermille);
            s.Int("starHirePermille", ref StarHirePermille);
            s.Int("moraleResearchPermillePerPoint", ref MoraleResearchPermillePerPoint);
            s.Int("moraleAlignmentPermillePerPoint", ref MoraleAlignmentPermillePerPoint);
            s.Int("moraleTrainingPermillePerPoint", ref MoraleTrainingPermillePerPoint);
            s.Int("moraleFailCutPermillePerPoint", ref MoraleFailCutPermillePerPoint);
            s.Int("moralePoachResistPermillePerPoint", ref MoralePoachResistPermillePerPoint);
            s.Int("moraleIncidentMitigationPermillePerPoint", ref MoraleIncidentMitigationPermillePerPoint);
            s.Int("moralePoachWitnessMitigationPermillePerPoint", ref MoralePoachWitnessMitigationPermillePerPoint);
            s.Int("moraleDebtMitigationPermillePerPoint", ref MoraleDebtMitigationPermillePerPoint);
            s.ObjList("traits", ref Traits);
        }
    }

    public sealed class StaffTraitConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int ResearchPermille;
        public bool Star;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("researchPermille", ref ResearchPermille);
            s.Bool("star", ref Star);
        }
    }
}