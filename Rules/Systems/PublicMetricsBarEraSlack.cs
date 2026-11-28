using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PublicMetricsBarEraSlack
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetricsBarEraSlack.Enabled > 0 && PublicMetrics.Enabled(c); }

        public static int EffectiveBar(GameConfig c, GameState state)
        {
            int bar = c.PublicMetrics.Bar;
            if (!Enabled(c) || state == null) return bar;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.PublicMetricsBarEraSlack.MinEra) return bar;
            int cut = c.PublicMetricsBarEraSlack.SlackPermille;
            if (cut <= 0) return bar;
            return (int)MathI.MulPermille(bar, MathI.Permille - cut);
        }

        public static void OnWeek(GameConfig c, GameState state, LabState lab, ModelState model, int listBar, int appliedBar)
        {
            if (!Enabled(c) || appliedBar >= listBar) return;
            int saved = listBar - appliedBar;
            if (saved <= 0) return;
            Events.Add(state, EventKind.PublicMetricsBarEraSlacked, lab.Id, model.Id, saved, model.Name);
        }
    }
}