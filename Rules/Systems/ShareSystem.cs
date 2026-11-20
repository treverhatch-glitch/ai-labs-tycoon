using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ShareSystem
    {
        public static void RunMonth(GameConfig c, GameState state)
        {
            SharesConfig s = c.Shares;
            if (s == null || s.IpoWhenBubbleAtPermille <= 0) return;
            if (state.BubblePermille < s.IpoWhenBubbleAtPermille) return;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Bankrupt || lab.Controller != LabController.Plan) continue;
                if (lab.PublicFloatPpm >= s.RivalMaxPublicFloatPpm) continue;
                int next = (int)MathI.Min(s.RivalMaxPublicFloatPpm, lab.PublicFloatPpm + s.IpoStepPpm);
                if (next <= lab.PublicFloatPpm) continue;
                lab.PublicFloatPpm = next;
                if (s.IpoBubbleBoostPermille > 0)
                    state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + s.IpoBubbleBoostPermille);
            }
        }

        /// <summary>How much of a lab's equity the public owns, in ppm.</summary>
        public static int PublicFloatPpm(GameConfig c, LabState lab)
        {
            if (lab.Controller == LabController.Player)
                return (int)MathI.Max(0, (MathI.Permille - lab.FounderSharePermille) * MathI.Ppm / MathI.Permille);
            return lab.PublicFloatPpm;
        }
    }
}