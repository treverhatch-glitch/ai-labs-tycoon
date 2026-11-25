using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Serving demand split across models. See <see cref="TrafficRouterConfig"/>. Only the player's lab.</summary>
    public static class TrafficRouter
    {
        public static bool Enabled(GameConfig c) { return c.TrafficRouter.Enabled > 0; }

        public static int RouteTotal(LabState lab)
        {
            int total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status == ModelStatus.Released) total += m.RoutePpm;
            }
            return total;
        }

        static int ReleasedCount(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Released) n++;
            return n;
        }

        /// <summary>Share of serving demand this model should get, in ppm.</summary>
        public static int SharePpm(GameConfig c, LabState lab, ModelState model)
        {
            if (!Enabled(c)) return MathI.Ppm;
            if (model.Status != ModelStatus.Released) return 0;
            int total = RouteTotal(lab);
            int count = ReleasedCount(lab);
            if (count <= 0) return 0;
            if (total <= 0) return MathI.Ppm / count;
            return model.RoutePpm;
        }

        public static bool Skewed(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return false;
            int total = RouteTotal(lab);
            if (total <= 0) return false;
            int max = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status == ModelStatus.Released) max = (int)MathI.Max(max, m.RoutePpm);
            }
            return (long)max * MathI.Ppm >= (long)c.TrafficRouter.SkewBar * total;
        }

        public static int IncidentAdd(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return 0;
            if (lab.LastWeek.ServedPermille > c.TrafficRouter.ServedBelow) return 0;
            if (!Skewed(c, lab)) return 0;
            return c.TrafficRouter.OverloadAdd;
        }

        public static void SetShare(GameConfig c, GameState state, LabState lab, ModelState model, int ppm)
        {
            model.RoutePpm = ppm;
            Events.Add(state, EventKind.RouteShareSet, lab.Id, model.Id, ppm, model.Name);
        }
    }
}