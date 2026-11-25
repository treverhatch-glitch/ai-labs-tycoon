using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// How the company is controlled (bible ch. 5, control of your own company). A lab can adopt dual-class shares (a founder share counts for several votes) or a non-profit parent (the founders cannot be outvoted), each once and only early, at the price of cash from every later round. Independent directors take weeks to find, cost a fee and a weekly sum, and calm the board. A lab that never changes anything is an ordinary company and none of this applies. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class GovernanceConfig : ISync
    {
        public int Enabled;
        /// <summary>Most funding rounds a lab can have raised and still switch to dual-class shares.</summary>
        public int DualMaxRounds;
        /// <summary>Votes a founder share counts for under dual-class shares (permille, 4000 is four votes a share).</summary>
        public int DualVoteMult;
        /// <summary>Share of every later round cash investors take off for dual-class shares (permille).</summary>
        public int DualCashCut;
        /// <summary>Most funding rounds a lab can have raised and still put a non-profit parent over it.</summary>
        public int NonprofitMaxRounds;
        /// <summary>Share of every later round cash investors take off for a non-profit parent (permille).</summary>
        public int NonprofitCashCut;
        /// <summary>Weeks to find and seat an independent director.</summary>
        public int DirectorWeeks;
        /// <summary>Most independent directors.</summary>
        public int MaxDirectors;
        /// <summary>Search fee for a director.</summary>
        public int DirectorFee;
        /// <summary>What each seated director costs a week.</summary>
        public int DirectorWeekly;
        /// <summary>Reputation the lab needs before anyone independent will join.</summary>
        public int DirectorMinRep;
        /// <summary>Share of the yearly chance of board drama each director takes away (permille).</summary>
        public int DramaCut;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("dualMaxRounds", ref DualMaxRounds);
            s.Int("dualVoteMult", ref DualVoteMult);
            s.Int("dualCashCut", ref DualCashCut);
            s.Int("nonprofitMaxRounds", ref NonprofitMaxRounds);
            s.Int("nonprofitCashCut", ref NonprofitCashCut);
            s.Int("directorWeeks", ref DirectorWeeks);
            s.Int("maxDirectors", ref MaxDirectors);
            s.Int("directorFee", ref DirectorFee);
            s.Int("directorWeekly", ref DirectorWeekly);
            s.Int("directorMinRep", ref DirectorMinRep);
            s.Int("dramaCut", ref DramaCut);
        }
    }
}
