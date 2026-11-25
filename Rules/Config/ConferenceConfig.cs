using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Conference submissions. Beside the instant paper, the player can send a paper to a venue tier (workshop, main conference, flagship) with an effort from 1 to 3 and wait for the decision. The research points are paid up front (more for a better venue and more effort) and up to <c>MaxPending</c> papers wait at once. On decision day the paper is accepted when the lab's prestige then plus a bonus for the effort reaches the venue's bar: nothing is random, but prestige fades in the weeks of review. Accepted papers add prestige and reputation by venue; rejected ones get part of the points back. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ConferenceConfig : ISync
    {
        public int Enabled;
        /// <summary>Papers the lab can have out for review at once.</summary>
        public int MaxPending;
        /// <summary>What a main-conference paper costs, as a multiple of a plain paper (<c>Prestige</c> paper cost); a workshop paper is 1 and a flagship paper is <c>FlagshipCost</c>.</summary>
        public int MainCost;
        public int FlagshipCost;
        /// <summary>Weeks from submitting to the decision, by venue.</summary>
        public int WorkshopWeeks;
        public int MainWeeks;
        public int FlagshipWeeks;
        /// <summary>The standing a paper needs on decision day: the lab's prestige then plus <c>EffortPrestige</c> for each step of effort. A workshop takes anything.</summary>
        public int MainBar;
        public int FlagshipBar;
        /// <summary>What each step of effort (1 to 3, and the same multiple of the cost) adds to a paper's standing.</summary>
        public int EffortPrestige;
        /// <summary>Prestige an accepted paper adds, times the venue number (1 workshop, 2 main, 3 flagship).</summary>
        public int AcceptPrestige;
        /// <summary>Reputation an accepted paper adds, times the venue number.</summary>
        public int AcceptReputation;
        /// <summary>The share of the research points a rejected paper gets back, in permille.</summary>
        public int RefundPermille;
        /// <summary>The share a withdrawn paper gets back, in permille.</summary>
        public int WithdrawPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPending", ref MaxPending);
            s.Int("mainCost", ref MainCost);
            s.Int("flagshipCost", ref FlagshipCost);
            s.Int("workshopWeeks", ref WorkshopWeeks);
            s.Int("mainWeeks", ref MainWeeks);
            s.Int("flagshipWeeks", ref FlagshipWeeks);
            s.Int("mainBar", ref MainBar);
            s.Int("flagshipBar", ref FlagshipBar);
            s.Int("effortPrestige", ref EffortPrestige);
            s.Int("acceptPrestige", ref AcceptPrestige);
            s.Int("acceptReputation", ref AcceptReputation);
            s.Int("refundPermille", ref RefundPermille);
            s.Int("withdrawPermille", ref WithdrawPermille);
        }
    }
}
