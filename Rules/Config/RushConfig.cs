using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Rushing a release (bible ch. 6: "rushing wins the launch window and raises the odds of a public incident"; ch. 10: rushing a release to beat
    /// a rival erodes control). A normal release is a tested one. The player can instead skip some of the testing: the model sells better for a
    /// quarter and is more likely to have an incident for half a year. Rivals and the autopilot never rush.
    /// </summary>
    public sealed class RushConfig : ISync
    {
        /// <summary>The most weeks of testing that can be skipped in one release.</summary>
        public int MaxWeeks;
        /// <summary>How long after a rushed launch the sales boost lasts, in weeks.</summary>
        public int LaunchWeeks;
        /// <summary>The boost to the model's appeal for each week skipped, in permille.</summary>
        public int LaunchAppealPermillePerWeek;
        /// <summary>How long after a rushed launch the extra incident chance lasts, in weeks.</summary>
        public int IncidentWeeks;
        /// <summary>Added to the weekly incident chance, in permille, for each week skipped.</summary>
        public int IncidentPermillePerWeek;
        /// <summary>Safety culture points lost for each week skipped, once, at launch.</summary>
        public int CulturePointsPerWeek;

        public void Sync(Syncer s)
        {
            s.Int("maxWeeks", ref MaxWeeks);
            s.Int("launchWeeks", ref LaunchWeeks);
            s.Int("launchAppealPermillePerWeek", ref LaunchAppealPermillePerWeek);
            s.Int("incidentWeeks", ref IncidentWeeks);
            s.Int("incidentPermillePerWeek", ref IncidentPermillePerWeek);
            s.Int("culturePointsPerWeek", ref CulturePointsPerWeek);
        }
    }
}
