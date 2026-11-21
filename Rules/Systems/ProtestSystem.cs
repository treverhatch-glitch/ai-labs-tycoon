using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Dated protest headlines add government attention in a bloc (or each lab's home bloc when the event names none).
    /// </summary>
    public static class ProtestSystem
    {
        public static void OnWorldEvent(GameConfig c, GameState state, WorldEventConfig e)
        {
            if (e.ProtestAttentionPermille < 1) return;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Bankrupt) continue;
                string blocId = string.IsNullOrEmpty(e.ProtestBlocId) ? lab.HomeBlocId : e.ProtestBlocId;
                if (string.IsNullOrEmpty(blocId)) continue;
                BlocStanding standing = lab.Bloc(blocId);
                if (standing == null) continue;
                int before = standing.Attention;
                standing.Attention = (int)MathI.Min(MathI.Permille, before + e.ProtestAttentionPermille);
                if (standing.Attention > before)
                    Events.Add(state, EventKind.AttentionRose, lab.Id, -1, standing.Attention - before, blocId);
            }
        }
    }
}