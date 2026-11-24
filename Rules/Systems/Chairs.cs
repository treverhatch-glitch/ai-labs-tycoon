using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Endowed university chairs. See <see cref="ChairsConfig"/>. Only the player's lab endows them, and nothing runs until it does.</summary>
    public static class Chairs
    {
        public static bool Enabled(GameConfig c) { return c.Chairs.MaxChairs > 0; }

        public static int Held(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.Chairs, 0, c.Chairs.MaxChairs); }

        /// <summary>What the next chair costs: the base price times the chair it would be.</summary>
        public static long NextCost(GameConfig c, LabState lab) { return c.Chairs.CostBase * (Held(c, lab) + 1); }

        public static int Floor(GameConfig c, LabState lab) { return lab.Chairs <= 0 ? 0 : Held(c, lab) * c.Chairs.PrestigeFloor; }

        public static long Research(GameConfig c, LabState lab) { return lab.Chairs <= 0 ? 0 : (long)Held(c, lab) * c.Chairs.ResearchPerWeek; }

        public static long Stipends(GameConfig c, LabState lab) { return lab.Chairs <= 0 ? 0 : Held(c, lab) * c.Chairs.StipendPerWeek; }

        public static void Endow(GameConfig c, GameState state, LabState lab)
        {
            long cost = NextCost(c, lab);
            Finance.Spend(lab, LedgerLine.Training, cost);
            lab.Chairs++;
            Events.Add(state, EventKind.ChairEndowed, lab.Id, lab.Chairs, cost, "");
        }
    }
}
