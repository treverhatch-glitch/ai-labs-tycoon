using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A voluntary incident register. A lab that joins must report each incident within weeks. A summary closes the report at once but leaves a finding; a full report takes safety people weeks to prepare, clears a finding and earns more trust. Late reports are fined and leave findings, and inspections fine labs with too many findings. A lab that never joins has no register and none of this applies. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class DisclosureConfig : ISync
    {
        public int Enabled;
        /// <summary>Reports a lab can have waiting at once; an incident beyond that goes unreported and counts as a finding.</summary>
        public int MaxOpen;
        /// <summary>Weeks after an incident to file the report.</summary>
        public int ReportWeeks;
        /// <summary>Weeks to prepare a full report.</summary>
        public int FullWeeks;
        /// <summary>Safety people a full report needs.</summary>
        public int FullStaff;
        /// <summary>Trust gained in the sectors of the incident model by a summary filed in time.</summary>
        public int TrustGain;
        /// <summary>Trust gained by a full report finished in time.</summary>
        public int TrustFull;
        /// <summary>Fine for a late report, times the findings on file (up to 5).</summary>
        public int LateFine;
        /// <summary>Weeks between inspections.</summary>
        public int InspectWeeks;
        /// <summary>Findings at which an inspection fines the lab.</summary>
        public int InspectLimit;
        /// <summary>Fine for each finding when an inspection fines the lab.</summary>
        public int InspectFine;

        /// <summary>Findings an incident on a model with a conformity mark leaves at once.</summary>
        public int CertifiedFindings;

        /// <summary>Weeks off a full report in a functional lab whose safety team has a senior.</summary>
        public int OrgFasterWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("reportWeeks", ref ReportWeeks);
            s.Int("fullWeeks", ref FullWeeks);
            s.Int("fullStaff", ref FullStaff);
            s.Int("trustGain", ref TrustGain);
            s.Int("trustFull", ref TrustFull);
            s.Int("lateFine", ref LateFine);
            s.Int("inspectWeeks", ref InspectWeeks);
            s.Int("inspectLimit", ref InspectLimit);
            s.Int("inspectFine", ref InspectFine);
            s.Int("certifiedFindings", ref CertifiedFindings);
            s.Int("orgFasterWeeks", ref OrgFasterWeeks);
        }
    }
}
