using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LawShaping
    {
        public const string Strict = "strict";
        public const string Loose = "loose";

        public static bool Enabled(GameConfig c) { return c.LawShaping.Enabled > 0; }

        public static bool IsGoal(string goal) { return goal == Strict || goal == Loose; }

        public static int DampPermille(GameConfig c, LabState lab)
        {
            LawShapingConfig cfg = c.LawShaping;
            BlocStanding home = lab.Bloc(lab.HomeBlocId);
            if (home == null || home.Attention < cfg.RegulatorAttentionDampFrom) return MathI.Permille;
            long over = home.Attention - cfg.RegulatorAttentionDampFrom;
            long cut = over * cfg.RegulatorDampPermille / MathI.Permille;
            return (int)MathI.Max(100, MathI.Permille - cut);
        }

        public static void Apply(GameConfig c, GameState state, LabState lab, BlocStanding standing, string goal)
        {
            LawShapingConfig cfg = c.LawShaping;
            int delta = goal == Strict ? cfg.StrictDelta : -cfg.LooseDelta;
            delta = (int)MathI.MulPermille(delta, DampPermille(c, lab));
            lab.LawShapePending = delta;
            standing.LobbyUntilTick = state.Tick + c.Lobbying.CooldownWeeks;
            Events.Add(state, EventKind.LawShaped, lab.Id, delta, goal == Strict ? 1 : 0, standing.BlocId);
        }

        public static void OnEraBegan(GameConfig c, GameState state, LabState lab, string eraId)
        {
            if (lab.Id != state.PlayerLabId) return;
            if (lab.LawShapePending != 0)
            {
                lab.LawShapeDelta = lab.LawShapePending;
                lab.LawShapeEra = eraId;
                lab.LawShapePending = 0;
            }
            else if (lab.LawShapeEra != eraId)
            {
                lab.LawShapeDelta = 0;
                lab.LawShapeEra = "";
            }
        }

        public static int ReleaseBarDelta(GameConfig c, LabState lab, int tick)
        {
            if (!Enabled(c) || string.IsNullOrEmpty(lab.LawShapeEra)) return 0;
            if (lab.LawShapeEra != EraSystem.AtTick(c, tick).Id) return 0;
            return lab.LawShapeDelta;
        }
    }
}