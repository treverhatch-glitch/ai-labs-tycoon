using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Who the player chose to be and how the lab is good (bible ch. 5). The choice is two ids on the lab; everything here is read from the
    /// config each time, so a founder's lasting edge and cost need no state of their own. A lab with neither choice (every rival, and a
    /// player who picked none) reads zero everywhere.
    /// </summary>
    public static class Founding
    {
        /// <summary>No lab ever reads less appeal than this from founder and style together, so a bad pairing hurts but never ends a game by itself.</summary>
        public const int AppealFloorPermille = -500;

        public static FounderConfig FounderOf(GameConfig c, LabState lab)
        {
            return string.IsNullOrEmpty(lab.FounderId) ? null : c.Founder(lab.FounderId);
        }

        public static StyleConfig StyleOf(GameConfig c, LabState lab)
        {
            return string.IsNullOrEmpty(lab.StyleId) ? null : c.Style(lab.StyleId);
        }

        public static int ResearchPermille(GameConfig c, LabState lab)
        {
            FounderConfig f = FounderOf(c, lab);
            return f == null ? 0 : f.ResearchPermille;
        }

        public static int HireFeePermille(GameConfig c, LabState lab)
        {
            FounderConfig f = FounderOf(c, lab);
            return f == null ? 0 : f.HireFeePermille;
        }

        /// <summary>The founder's and the style's share of a model's appeal in a sector: signed, never below <see cref="AppealFloorPermille"/>.</summary>
        public static int AppealPermille(GameConfig c, LabState lab, string sectorId)
        {
            int total = 0;
            FounderConfig f = FounderOf(c, lab);
            if (f != null) total += f.AppealPermille;
            StyleConfig st = StyleOf(c, lab);
            if (st != null) total += st.FocusSectors.Contains(sectorId) ? st.FocusAppealPermille : st.OtherAppealPermille;
            return (int)MathI.Max(AppealFloorPermille, total);
        }

        /// <summary>Applies the start-of-game part of a founder: money, hype, grant and the enemy. Called once, when the game is made.</summary>
        public static void ApplyStart(GameConfig c, GameState state, LabState player, FounderConfig founder)
        {
            player.FounderId = founder.Id;
            player.Cash = MathI.MulPermille(player.Cash, founder.CashPermille);
            player.Ledger.StartCash = player.Cash;
            player.Reputation = (int)MathI.Clamp(player.Reputation + founder.ReputationDelta, 0, MathI.Permille);
            player.GrantPerWeek = MathI.MulPermille(player.GrantPerWeek, founder.GrantPermille);
            if (string.IsNullOrEmpty(founder.EnemyRivalId)) return;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.RivalId != founder.EnemyRivalId) continue;
                lab.FeelingToPlayer = (int)MathI.Clamp(founder.EnemyFeeling, -MathI.Permille, MathI.Permille);
            }
        }
    }
}
