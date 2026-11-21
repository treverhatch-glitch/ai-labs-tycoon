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
        /// <summary>Attention permille added by a protest headline, including Q61 revolt bonus when already hot.</summary>
        public static int ProtestAttentionBump(GameConfig c, int attentionBefore, int basePermille)
        {
            if (basePermille < 1) return 0;
            int bump = basePermille;
            AttentionConfig attention = c.World.Attention;
            if (attention != null && attention.ProtestRevoltBonusPermille > 0 && attention.ProtestRevoltAttentionAt > 0
                && attentionBefore >= attention.ProtestRevoltAttentionAt)
                bump += (int)MathI.MulPermille(bump, attention.ProtestRevoltBonusPermille);
            return bump;
        }

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
                int bump = ProtestAttentionBump(c, before, e.ProtestAttentionPermille);
                standing.Attention = (int)MathI.Min(MathI.Permille, before + bump);
                if (standing.Attention > before)
                    Events.Add(state, EventKind.AttentionRose, lab.Id, -1, standing.Attention - before, blocId);
            }
        }
    }
}