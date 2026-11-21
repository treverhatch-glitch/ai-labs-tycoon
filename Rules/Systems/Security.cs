using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Information security. See <see cref="SecurityConfig"/>. Only the player's lab keeps any.</summary>
    public static class Security
    {
        public const int Ransom = 0;
        public const int Disclose = 1;
        public const int Lockdown = 2;

        public static bool Enabled(GameConfig c) { return c.Security.Enabled > 0; }

        public static bool Open(LabState lab) { return lab.BreachDue > 0; }

        public static long TierPrice(GameConfig c, int tier) { return c.Security.TierCost * tier; }

        public static long RansomCost(GameConfig c, LabState lab) { return c.Security.Ransom * (lab.Ransoms + 1L); }

        public static long LockdownCost(GameConfig c, LabState lab) { return TierPrice(c, lab.SecurityTier + 1) * 2; }

        public static long Upkeep(GameConfig c, LabState lab) { return c.Security.UpkeepPerTier * lab.SecurityTier; }

        public static int OnSale(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Released) n++;
            return n;
        }

        /// <summary>What a week adds to the exposure: the models on sale, less what the tiers take off. Never below nothing.</summary>
        public static int WeeklyExposure(GameConfig c, LabState lab)
        {
            return (int)MathI.Max(0, OnSale(lab) * (long)c.Security.ExposurePerModel - lab.SecurityTier * (long)c.Security.ReliefPerTier);
        }

        public static void Build(GameConfig c, GameState state, LabState lab)
        {
            lab.SecurityBuilding = lab.SecurityTier + 1;
            lab.SecurityDue = state.Tick + c.Security.BuildWeeks;
            Finance.Spend(lab, LedgerLine.Insurance, TierPrice(c, lab.SecurityBuilding));
        }

        static void SueFor(GameConfig c, GameState state, LabState lab, bool leaked)
        {
            int strength = 450 + (leaked ? 250 : 0) + lab.Ransoms * 100 - lab.SecurityTier * 60;
            Litigation.File(c, state, lab, Litigation.Privacy, c.Litigation.PrivacyClaim, strength, "");
        }

        static void Expose(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.OnExposed(c, state, lab, m.Sectors[s]);
            }
        }

        public static void Answer(GameConfig c, GameState state, LabState lab, int choice)
        {
            long cost = 0;
            if (choice == Ransom)
            {
                cost = RansomCost(c, lab);
                Finance.Spend(lab, LedgerLine.Fines, cost);
                lab.Ransoms++;
            }
            else if (choice == Disclose)
            {
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Security.DiscloseReputation));
                Expose(c, state, lab);
                lab.Ransoms = 0;
                SueFor(c, state, lab, false);
            }
            else
            {
                cost = LockdownCost(c, lab);
                Finance.Spend(lab, LedgerLine.Insurance, cost);
                lab.SecurityTier++;
                Events.Add(state, EventKind.SecurityBuilt, lab.Id, -1, lab.SecurityTier, "");
            }
            lab.Exposure = 0;
            lab.BreachDue = 0;
            Events.Add(state, EventKind.BreachAnswered, lab.Id, -1, cost, "").Pick = choice;
        }

        static void Leak(GameConfig c, GameState state, LabState lab)
        {
            int before = lab.Reputation;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Security.LeakReputation));
            Expose(c, state, lab);
            SueFor(c, state, lab, true);
            GovContracts.OnLeak(c, state, lab);
            lab.Exposure = 0;
            lab.BreachDue = 0;
            Events.Add(state, EventKind.BreachLeaked, lab.Id, -1, before - lab.Reputation, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.SecurityTier > 0) Finance.Spend(lab, LedgerLine.Insurance, Upkeep(c, lab));
            if (lab.SecurityBuilding > 0 && state.Tick + 1 >= lab.SecurityDue)
            {
                lab.SecurityTier = lab.SecurityBuilding;
                lab.SecurityBuilding = 0;
                lab.SecurityDue = 0;
                Events.Add(state, EventKind.SecurityBuilt, lab.Id, -1, lab.SecurityTier, "");
            }
            if (Open(lab))
            {
                if (state.Tick + 1 >= lab.BreachDue) Leak(c, state, lab);
                return;
            }
            lab.Exposure += WeeklyExposure(c, lab);
            if (lab.Exposure < c.Security.Threshold) return;
            lab.BreachDue = state.Tick + 1 + c.Security.AnswerWeeks;
            Events.Add(state, EventKind.BreachOpened, lab.Id, -1, lab.BreachDue, "");
        }
    }
}
