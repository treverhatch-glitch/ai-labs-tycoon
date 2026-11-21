using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Endowed university chairs. A big one-off payment buys a permanent research post: more research points every week, a floor under the lab's
    /// academic prestige (papers still push it higher, but it never decays below the floor) and a stipend every week for good. Each chair costs more than
    /// the one before. Only the player's lab endows them. Zero in <c>MaxChairs</c> turns it off.
    /// </summary>
    public sealed class ChairsConfig : ISync
    {
        /// <summary>The most chairs a lab can hold; 0 turns chairs off.</summary>
        public int MaxChairs;
        /// <summary>The price of the first chair. The next one costs twice that, the third three times, and so on.</summary>
        public long CostBase;
        /// <summary>What each chair costs every week, for good.</summary>
        public long StipendPerWeek;
        /// <summary>Research points each chair adds every week.</summary>
        public int ResearchPerWeek;
        /// <summary>The prestige each chair keeps the lab from falling under.</summary>
        public int PrestigeFloor;
        /// <summary>Researchers the lab needs on the payroll before it can endow one, so there is a lab for the chair to sit in.</summary>
        public int MinResearchers;

        public void Sync(Syncer s)
        {
            s.Int("maxChairs", ref MaxChairs);
            s.Long("costBase", ref CostBase);
            s.Long("stipendPerWeek", ref StipendPerWeek);
            s.Int("researchPerWeek", ref ResearchPerWeek);
            s.Int("prestigeFloor", ref PrestigeFloor);
            s.Int("minResearchers", ref MinResearchers);
        }
    }
}
