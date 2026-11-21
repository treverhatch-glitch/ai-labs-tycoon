using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's academic prestige from published papers. See <see cref="PrestigeConfig"/>. Only the player's lab publishes, and nothing runs until it does.</summary>
    public static class Prestige
    {
        public static bool Enabled(GameConfig c) { return c.Prestige.Max > 0 && c.Prestige.PaperPrestige > 0; }

        public static int Level(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.Prestige, 0, c.Prestige.Max);
        }

        /// <summary>People on the payroll who do research and so can write a paper.</summary>
        public static int Writers(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Research); }

        /// <summary>The research points a paper costs now: the data's weeks of the lab's research output, never under the floor.</summary>
        public static long PaperCost(GameConfig c, GameState state, LabState lab)
        {
            long weekly = TrainingSystem.ResearchPerWeek(c, lab, state.Tick);
            return MathI.Max(c.Prestige.MinPoints, weekly * c.Prestige.PaperResearchWeeks);
        }

        static int Scaled(GameConfig c, LabState lab, int atMax)
        {
            return c.Prestige.Max <= 0 ? 0 : (int)(Level(c, lab) * (long)atMax / c.Prestige.Max);
        }

        public static int MoraleOffset(GameConfig c, LabState lab) { return Scaled(c, lab, c.Prestige.MoraleAtMax); }

        public static int HireDiscountPermille(GameConfig c, LabState lab) { return Scaled(c, lab, c.Prestige.HireDiscountAtMax); }

        public static int PoachPenaltyPermille(GameConfig c, LabState lab) { return Scaled(c, lab, c.Prestige.PoachAtMax); }

        public static int CounterBonusPermille(GameConfig c, LabState lab) { return Scaled(c, lab, c.Prestige.CounterAtMax); }

        public static void Publish(GameConfig c, GameState state, LabState lab, long cost)
        {
            lab.ResearchPoints -= cost;
            lab.Prestige = (int)MathI.Min(c.Prestige.Max, Level(c, lab) + (long)c.Prestige.PaperPrestige);
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + c.Prestige.PaperReputation, 0, MathI.Permille);
            Events.Add(state, EventKind.PaperPublished, lab.Id, lab.Prestige, cost, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Prestige <= 0) return;
            lab.Prestige = (int)MathI.Clamp(lab.Prestige - c.Prestige.DecayPerWeek, 0, c.Prestige.Max);
        }
    }
}
