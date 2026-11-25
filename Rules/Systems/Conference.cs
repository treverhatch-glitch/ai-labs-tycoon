using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Conference submissions. See <see cref="ConferenceConfig"/>. Only the player's lab submits.</summary>
    public static class Conference
    {
        public const int Workshop = 1;
        public const int Main = 2;
        public const int Flagship = 3;

        public static bool Enabled(GameConfig c) { return c.Conference.Enabled > 0 && Prestige.Enabled(c); }

        public static string Venue(int tier) { return tier == Workshop ? "workshop" : tier == Main ? "main conference" : "flagship conference"; }

        public static int ReviewWeeks(GameConfig c, int tier) { return tier == Workshop ? c.Conference.WorkshopWeeks : tier == Main ? c.Conference.MainWeeks : c.Conference.FlagshipWeeks; }

        public static int Bar(GameConfig c, int tier) { return tier == Workshop ? 0 : tier == Main ? c.Conference.MainBar : c.Conference.FlagshipBar; }

        public static int CostMultiple(GameConfig c, int tier) { return tier == Workshop ? 1 : tier == Main ? c.Conference.MainCost : c.Conference.FlagshipCost; }

        public static long Cost(GameConfig c, GameState state, LabState lab, int tier, int effort)
        {
            return Prestige.PaperCost(c, state, lab) * CostMultiple(c, tier) * effort;
        }

        /// <summary>What a paper's standing is on a day: the lab's prestige then plus the effort bonus.</summary>
        public static int Standing(GameConfig c, LabState lab, int effort) { return Prestige.Level(c, lab) + effort * c.Conference.EffortPrestige; }

        public static bool Accepted(GameConfig c, LabState lab, Submission paper) { return Standing(c, lab, paper.Effort) >= Bar(c, paper.Tier); }

        public static void Submit(GameConfig c, GameState state, LabState lab, int tier, int effort, long cost)
        {
            lab.ResearchPoints -= cost;
            var paper = new Submission();
            paper.Tier = tier;
            paper.Effort = effort;
            paper.Cost = cost;
            paper.Due = state.Tick + ReviewWeeks(c, tier);
            lab.Submissions.Add(paper);
            GameEvent e = Events.Add(state, EventKind.PaperSubmitted, lab.Id, tier, cost, Venue(tier));
            e.Pick = effort;
        }

        public static void Withdraw(GameConfig c, GameState state, LabState lab, Submission paper)
        {
            long back = MathI.MulPermille(paper.Cost, c.Conference.WithdrawPermille);
            lab.ResearchPoints += back;
            lab.Submissions.Remove(paper);
            Events.Add(state, EventKind.PaperDecided, lab.Id, paper.Tier, back, Venue(paper.Tier)).Pick = 2;
        }

        static void Decide(GameConfig c, GameState state, LabState lab, Submission paper)
        {
            lab.Submissions.Remove(paper);
            if (Accepted(c, lab, paper))
            {
                int gain = c.Conference.AcceptPrestige * paper.Tier;
                int before = Prestige.Level(c, lab);
                lab.Prestige = (int)MathI.Min(c.Prestige.Max, before + (long)gain);
                lab.Reputation = (int)MathI.Clamp(lab.Reputation + (long)c.Conference.AcceptReputation * paper.Tier, 0, MathI.Permille);
                Events.Add(state, EventKind.PaperDecided, lab.Id, paper.Tier, lab.Prestige - before, Venue(paper.Tier)).Pick = 1;
                ResearchGrants.OnPaper(c, state, lab);
                return;
            }
            long back = MathI.MulPermille(paper.Cost, c.Conference.RefundPermille);
            lab.ResearchPoints += back;
            Events.Add(state, EventKind.PaperDecided, lab.Id, paper.Tier, back, Venue(paper.Tier)).Pick = 0;
        }

        /// <summary>One week: papers whose decision day has come are decided, oldest first.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Submissions.Count == 0 || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Submissions.Count; )
            {
                Submission paper = lab.Submissions[i];
                if (state.Tick + 1 >= paper.Due) Decide(c, state, lab, paper);
                else i++;
            }
        }
    }
}
