using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Red teams (bible ch. 10: "people paid to break it first"). The player pays for a week-long attack on one model: it finds some of what the
    /// model was hiding, fixes a little real alignment, and for the next stretch the model is less likely to have an incident. A model that was
    /// sandbagging comes out with a more honest meter, which can put it below the lab's own release gate.
    /// </summary>
    public sealed class RedTeamConfig : ISync
    {
        /// <summary>The role whose people do the attacking; the lab needs at least <see cref="MinStaff"/> of them. Empty means no one is needed.</summary>
        public string RoleId = "";
        public int MinStaff;
        /// <summary>The bill is this many weeks of what the model sells for.</summary>
        public int RevenueWeeks;
        /// <summary>The bill is never less than this.</summary>
        public long MinCost;
        /// <summary>Weeks the model is safer for. A model cannot be red-teamed again until they are over. Zero means there are no red teams in the game.</summary>
        public int Weeks;
        /// <summary>The share of the model's weekly incident chance taken off while the weeks last, in permille.</summary>
        public int IncidentCutPermille;
        /// <summary>The share of the gap between the shown and the real alignment the attack closes, in permille.</summary>
        public int GapClosePermille;
        /// <summary>Real alignment points the fixes add (the shown meter moves with it).</summary>
        public int AlignmentGain;
        /// <summary>Safety culture points a red team adds for the lab.</summary>
        public int CulturePoints;

        public void Sync(Syncer s)
        {
            s.Str("roleId", ref RoleId);
            s.Int("minStaff", ref MinStaff);
            s.Int("revenueWeeks", ref RevenueWeeks);
            s.Long("minCost", ref MinCost);
            s.Int("weeks", ref Weeks);
            s.Int("incidentCutPermille", ref IncidentCutPermille);
            s.Int("gapClosePermille", ref GapClosePermille);
            s.Int("alignmentGain", ref AlignmentGain);
            s.Int("culturePoints", ref CulturePoints);
        }
    }
}
