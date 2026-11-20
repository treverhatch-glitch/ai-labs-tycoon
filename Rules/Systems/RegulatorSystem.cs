using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class RegulatorSystem
    {
        public static void RunMonth(GameConfig c, GameState state)
        {
            RegulatorsConfig r = c.Regulators;
            if (r == null || state.BubblePermille < r.BubbleScrutinyAtPermille) return;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Bankrupt) continue;
                if (GovernmentSystem.RevenueSharePpm(state, lab) < r.ScrutinyRevenuePpm) continue;
                BlocStanding home = lab.Bloc(lab.HomeBlocId);
                if (home == null) continue;
                home.Attention = (int)MathI.Min(MathI.Permille, home.Attention + r.ScrutinyAttentionPerMonth);
            }
        }

        public static void OnMonopoly(GameConfig c, GameState state, LabState lab)
        {
            RegulatorsConfig r = c.Regulators;
            if (r == null || r.MonopolyRegulatorAttention <= 0) return;
            for (int b = 0; b < lab.Blocs.Count; b++)
            {
                BlocStanding standing = lab.Blocs[b];
                standing.Attention = (int)MathI.Min(MathI.Permille, standing.Attention + r.MonopolyRegulatorAttention);
            }
        }
    }
}