using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Sector trust: a lab's standing with the customers of one sector. Slow to win (a little every week it sells there, and a bracket win),
    /// fast to lose (an incident, being exposed at the Gauntlet), and it carries between sectors of the same family. Trust moves appeal.
    /// A customer who is let down also writes to the player's desk, and the player can answer.
    /// </summary>
    public sealed class TrustConfig : ISync
    {
        /// <summary>Where trust starts, and where it drifts back to in a sector the lab has left. Appeal is neutral here.</summary>
        public int BaselineTrust;
        /// <summary>Trust a lab gains each week in a sector where it has a model on sale.</summary>
        public int GainPerWeek;
        /// <summary>Trust a lab moves back towards the baseline each week in a sector it no longer sells in.</summary>
        public int IdleDecayPerWeek;
        /// <summary>Trust lost in each sector an incident's model is on sale in.</summary>
        public int IncidentDrop;
        /// <summary>The share of any trust move that also lands in the other sectors of the same family where the lab has a model on sale.</summary>
        public int CarryPermille;
        /// <summary>Trust a lab wins in a sector by taking its Gauntlet bracket.</summary>
        public int BracketWinPoints;
        /// <summary>Trust lost in each bracket whose tuned win was exposed.</summary>
        public int ExposedDrop;
        /// <summary>Appeal moves by this share of the distance from the baseline: 400 makes full trust worth +20% and none worth -20%.</summary>
        public int AppealSlopePermille;
        /// <summary>Weeks a complaint waits for an answer; after that it lapses and nothing changes.</summary>
        public int ComplaintWeeks;
        public int MaxOpenComplaints;
        public int MinSuccessPermille;

        public List<ComplaintResponseConfig> Responses = new List<ComplaintResponseConfig>();

        public ComplaintResponseConfig Response(string id)
        {
            for (int i = 0; i < Responses.Count; i++) if (Responses[i].Id == id) return Responses[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("baselineTrust", ref BaselineTrust);
            s.Int("gainPerWeek", ref GainPerWeek);
            s.Int("idleDecayPerWeek", ref IdleDecayPerWeek);
            s.Int("incidentDrop", ref IncidentDrop);
            s.Int("carryPermille", ref CarryPermille);
            s.Int("bracketWinPoints", ref BracketWinPoints);
            s.Int("exposedDrop", ref ExposedDrop);
            s.Int("appealSlopePermille", ref AppealSlopePermille);
            s.Int("complaintWeeks", ref ComplaintWeeks);
            s.Int("maxOpenComplaints", ref MaxOpenComplaints);
            s.Int("minSuccessPermille", ref MinSuccessPermille);
            s.ObjList("responses", ref Responses);
        }
    }

    /// <summary>One way to answer an angry customer: what it costs, how likely it is to work, how much trust it wins back and how much it loses if it does not.</summary>
    public sealed class ComplaintResponseConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Blurb = "";
        /// <summary>The cost in hiring fees of a sales rep of the era, paid whether or not it works.</summary>
        public int CostHireFees;
        /// <summary>Plus this share of what the model last earned in that sector in a week (a refund is paid out of the sector's takings).</summary>
        public int CostRevenuePermille;
        public int BaseSuccessPermille;
        /// <summary>Per permille the lab's reputation is above 500, the chance moves by this share.</summary>
        public int ReputationSlopePermille;
        /// <summary>The share of the trust lost to that incident won back when it works.</summary>
        public int RestorePermille;
        /// <summary>The share of the trust lost to that incident lost again when it does not.</summary>
        public int BackfirePermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.Int("costHireFees", ref CostHireFees);
            s.Int("costRevenuePermille", ref CostRevenuePermille);
            s.Int("baseSuccessPermille", ref BaseSuccessPermille);
            s.Int("reputationSlopePermille", ref ReputationSlopePermille);
            s.Int("restorePermille", ref RestorePermille);
            s.Int("backfirePermille", ref BackfirePermille);
        }
    }
}
