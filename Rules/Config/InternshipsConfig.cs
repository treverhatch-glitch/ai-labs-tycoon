using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Internships. The player can take on a cohort of interns in one role: they cost a stipend every week, hold no seat and do no work, and when the weeks are up each one is offered a job for a share of the hiring fee, if there is a free seat and the cash. Those who cannot be taken on go into the alumni pool on good terms. One cohort at a time; only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class InternshipsConfig : ISync
    {
        public int Enabled;
        /// <summary>Most interns in a cohort.</summary>
        public int MaxCohort;
        /// <summary>Weeks a cohort stays.</summary>
        public int Weeks;
        /// <summary>Stipend paid over the weeks, per intern, as a share of the role's hiring fee.</summary>
        public int StipendPermille;
        /// <summary>What an intern costs to take on, as a share of the hiring fee.</summary>
        public int ConvertPermille;
        /// <summary>Morale a taken-on intern arrives with, added to where people settle.</summary>
        public int ConvertMorale;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxCohort", ref MaxCohort);
            s.Int("weeks", ref Weeks);
            s.Int("stipendPermille", ref StipendPermille);
            s.Int("convertPermille", ref ConvertPermille);
            s.Int("convertMorale", ref ConvertMorale);
        }
    }
}
