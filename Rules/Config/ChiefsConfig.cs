using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Chief officers: the player names one of the lab's own people as chief scientist (a researcher) or chief safety officer (a safety person).
    /// A chief costs a fee to appoint and a pay premium every week, and gives the lab something: the scientist lifts research, the safety chief
    /// cuts the weekly chance of an incident. A chief is also a target: rivals find a lab with chiefs easier to raid, and when a chief goes (fired,
    /// poached, retrained, quit) the whole team loses morale. Only the player's lab appoints chiefs. Turning it off in the data removes the slots.
    /// </summary>
    public sealed class ChiefsConfig : ISync
    {
        /// <summary>False removes the chief slots.</summary>
        public bool Enabled;
        /// <summary>The people on the payroll needed before a lab has anyone worth a title.</summary>
        public int MinStaff;
        /// <summary>Cash for naming a chief (and again for naming a different person to the same title).</summary>
        public long Fee;
        /// <summary>Cash every week for each sitting chief, on top of their salary.</summary>
        public long WeeklyPremium;
        /// <summary>The research a sitting chief scientist adds, in permille of the lab's research.</summary>
        public int ResearchPermille;
        /// <summary>The share a sitting chief safety officer takes off the weekly chance of an incident, in permille.</summary>
        public int IncidentCutPermille;
        /// <summary>The share each sitting chief takes off the lab's poach defence, in permille.</summary>
        public int PoachPenaltyPermille;
        /// <summary>The morale every other person loses when a chief goes, and the chief loses when stepped down.</summary>
        public int ShockMorale;

        public void Sync(Syncer s)
        {
            s.Bool("enabled", ref Enabled);
            s.Int("minStaff", ref MinStaff);
            s.Long("fee", ref Fee);
            s.Long("weeklyPremium", ref WeeklyPremium);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("incidentCutPermille", ref IncidentCutPermille);
            s.Int("poachPenaltyPermille", ref PoachPenaltyPermille);
            s.Int("shockMorale", ref ShockMorale);
        }
    }
}
