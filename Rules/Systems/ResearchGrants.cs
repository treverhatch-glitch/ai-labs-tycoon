using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Research grants. See <see cref="ResearchGrantsConfig"/>. Only the player's lab applies, and nothing runs until it does.</summary>
    public static class ResearchGrants
    {
        public const int None = 0;
        public const int InReview = 1;
        public const int Owed = 2;

        public static bool Enabled(GameConfig c) { return c.ResearchGrants.MaxTier > 0 && Prestige.Enabled(c); }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.ResearchGrantLockedUntil; }

        public static long ApplyCost(GameConfig c, int tier) { return tier * c.ResearchGrants.ApplyCostPerTier; }

        public static long Award(GameConfig c, int tier) { return tier * c.ResearchGrants.AwardPerTier; }

        public static int Bar(GameConfig c, int tier) { return tier * c.ResearchGrants.BarPerTier; }

        /// <summary>What the panel would give the lab now: its prestige plus a share of its reputation.</summary>
        public static int Score(GameConfig c, LabState lab)
        {
            return Prestige.Level(c, lab) + (int)MathI.MulPermille(MathI.Clamp(lab.Reputation, 0, MathI.Permille), c.ResearchGrants.ReputationWeightPermille);
        }

        public static bool WouldPass(GameConfig c, LabState lab, int tier) { return Score(c, lab) >= Bar(c, tier); }

        public static long Clawback(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(Charter.GrantAward(c, lab, Award(c, lab.ResearchGrantTier)), c.ResearchGrants.ClawbackPermille);
        }

        public static void Apply(GameConfig c, GameState state, LabState lab, int tier, long cost)
        {
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            lab.ResearchGrantStage = InReview;
            lab.ResearchGrantTier = tier;
            lab.ResearchGrantOwed = 0;
            lab.ResearchGrantTick = state.Tick + c.ResearchGrants.ReviewWeeks;
            Events.Add(state, EventKind.GrantApplied, lab.Id, tier, cost, "").Pick = 1;
        }

        /// <summary>A paper came out: it counts against what an awarded grant is owed.</summary>
        public static void OnPaper(GameConfig c, GameState state, LabState lab)
        {
            if (lab.ResearchGrantStage != Owed || lab.ResearchGrantOwed <= 0) return;
            lab.ResearchGrantOwed--;
            if (lab.ResearchGrantOwed > 0) return;
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + c.ResearchGrants.CompletedReputation, 0, MathI.Permille);
            Events.Add(state, EventKind.GrantSettled, lab.Id, lab.ResearchGrantTier, 0, "").Pick = 1;
            Close(c, state, lab);
        }

        static void Close(GameConfig c, GameState state, LabState lab)
        {
            lab.ResearchGrantStage = None;
            lab.ResearchGrantTier = 0;
            lab.ResearchGrantOwed = 0;
            lab.ResearchGrantTick = 0;
            lab.ResearchGrantLockedUntil = state.Tick + c.ResearchGrants.CooldownWeeks;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.ResearchGrantStage == None) return;
            if (state.Tick + 1 < lab.ResearchGrantTick) return;
            int tier = lab.ResearchGrantTier;
            if (lab.ResearchGrantStage == InReview)
            {
                if (WouldPass(c, lab, tier))
                {
                    long award = Charter.GrantAward(c, lab, Award(c, tier));
                    Finance.Earn(lab, LedgerLine.Grants, award);
                    lab.ResearchGrantStage = Owed;
                    lab.ResearchGrantOwed = tier;
                    lab.ResearchGrantTick = state.Tick + c.ResearchGrants.DeliverWeeks + 1;
                    Events.Add(state, EventKind.GrantDecided, lab.Id, tier, award, "").Pick = 1;
                }
                else
                {
                    Events.Add(state, EventKind.GrantDecided, lab.Id, tier, 0, "").Pick = 0;
                    Close(c, state, lab);
                }
                return;
            }
            long back = Clawback(c, lab);
            if (back > 0) Finance.Spend(lab, LedgerLine.Fines, back);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.ResearchGrants.FailedReputation);
            Events.Add(state, EventKind.GrantSettled, lab.Id, tier, back, "").Pick = 0;
            Close(c, state, lab);
        }
    }
}
