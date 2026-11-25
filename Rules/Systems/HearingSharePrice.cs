using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class HearingSharePrice
    {
        public static bool Enabled(GameConfig c) { return c.HearingSharePrice.Enabled > 0; }

        public static bool HasFloat(LabState lab, GameConfig c)
        {
            return lab.PublicFloatPpm >= c.HearingSharePrice.MinFloatPpm || Listing.Listed(lab);
        }

        public static void OnAnswered(GameConfig c, GameState state, LabState lab, OfficeEventConfig e, OfficeChoiceConfig choice)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || e.Trigger != OfficeTrigger.Hearing) return;
            if (!HasFloat(lab, c)) return;
            int rep = 0;
            for (int i = 0; i < choice.Effects.Count; i++)
                if (choice.Effects[i].Kind == OfficeEffectKind.Reputation) rep += choice.Effects[i].Amount;
            HearingSharePriceConfig cfg = c.HearingSharePrice;
            int delta = rep >= 0 ? cfg.UpPermille : cfg.DownPermille;
            if (delta <= 0) return;
            if (rep < 0) state.BubblePermille = (int)MathI.Max(0, state.BubblePermille - delta);
            else BubbleSystem.Boost(c, state, delta);
            Events.Add(state, EventKind.HearingShareNudged, lab.Id, delta, rep, "");
        }
    }
}