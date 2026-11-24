using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's research focus. See <see cref="ResearchFocusConfig"/>. Only the player's lab picks one.</summary>
    public static class ResearchFocus
    {
        public static bool Enabled(GameConfig c) { return c.Focus.Enabled; }

        /// <summary>The direction the lab has picked, or null for none (or for one the data no longer has).</summary>
        public static FocusOptionConfig Current(GameConfig c, LabState lab)
        {
            return string.IsNullOrEmpty(lab.ResearchFocus) ? null : c.Focus.Option(lab.ResearchFocus);
        }

        /// <summary>True when any effect of the tech is in some direction the data lists.</summary>
        public static bool InAnyDirection(GameConfig c, TechConfig tech)
        {
            for (int i = 0; i < tech.Effects.Count; i++)
                for (int k = 0; k < c.Focus.Options.Count; k++)
                    if (c.Focus.Options[k].Covers(tech.Effects[i].Kind)) return true;
            return false;
        }

        public static bool InDirection(FocusOptionConfig option, TechConfig tech)
        {
            for (int i = 0; i < tech.Effects.Count; i++) if (option.Covers(tech.Effects[i].Kind)) return true;
            return false;
        }

        /// <summary>What a tech costs under the lab's focus, as a share of its cost in permille: 1000 for no change.</summary>
        public static int CostPermille(GameConfig c, LabState lab, TechConfig tech)
        {
            FocusOptionConfig focus = Current(c, lab);
            if (focus == null || !InAnyDirection(c, tech)) return MathI.Permille;
            return InDirection(focus, tech) ? MathI.Permille - c.Focus.DiscountPermille : MathI.Permille + c.Focus.PenaltyPermille;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, string focusId)
        {
            lab.ResearchFocus = focusId ?? "";
            lab.FocusLockedUntil = state.Tick + c.Focus.LockWeeks;
            Finance.Spend(lab, LedgerLine.Training, c.Focus.SwitchCost);
            GameEvent e = Events.Add(state, EventKind.FocusChanged, lab.Id, -1, c.Focus.SwitchCost, lab.ResearchFocus);
            e.Pick = lab.ResearchFocus.Length > 0 ? 1 : 0;
        }
    }
}
