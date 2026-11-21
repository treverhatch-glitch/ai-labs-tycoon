using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The glow: how the building shows the risk of the lab's worst model. The room reads the true meters; the dashboard reads the displayed ones,
    /// so a model that sandbags or has been corrupted looks calmer on the chart than it does in the corridor. Never saved.
    /// </summary>
    public static class Glow
    {
        public struct Report
        {
            public string StageId;
            public int StageIndex;
            /// <summary>What the building shows. Empty when the lab has no model to read.</summary>
            public string Sign;
            /// <summary>The stage the dashboard's numbers alone would suggest.</summary>
            public string DashboardStageId;
            public int DashboardStageIndex;
            /// <summary>True when the building is more worried than the chart.</summary>
            public bool ChartIsMisleading;
            public int WorstModelId;
            public int WorstChancePermille;
            public int WorstRung;
            /// <summary>How bright the glow is, 0 to 1000, from the lab's combined capability.</summary>
            public int IntensityPermille;
            public long Capability;
        }

        public static Report Of(GameConfig c, GameState state, LabState lab)
        {
            var r = new Report();
            r.WorstModelId = -1;
            int roomBest = -1, dashBest = -1;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released && model.Status != ModelStatus.Trained) continue;
                int chance = SafetySystem.IncidentChancePermille(c, state.Tick, model);
                int rung = SafetySystem.PickIncidentRung(c, model);
                int stage = StageFor(c, chance, rung);
                if (stage > roomBest || (stage == roomBest && chance > r.WorstChancePermille))
                {
                    roomBest = stage;
                    r.WorstModelId = model.Id;
                    r.WorstChancePermille = chance;
                    r.WorstRung = rung;
                }
                int shown = StageFor(c, DashboardChance(c, state.Tick, model), DashboardRung(c, model));
                if (shown > dashBest) dashBest = shown;
            }
            for (int i = 0; i < c.Training.Lines.Count; i++)
            {
                ModelState latest = LatestReady(lab, c.Training.Lines[i].Id);
                if (latest != null) r.Capability += latest.Capability;
            }
            r.IntensityPermille = c.Glow.FullAtCapability <= 0 ? 0 : (int)MathI.Min(MathI.Permille, r.Capability * MathI.Permille / c.Glow.FullAtCapability);
            if (roomBest < 0) return r;
            r.StageIndex = roomBest;
            r.StageId = c.Glow.Stages[roomBest].IdValue;
            r.Sign = c.Glow.Stages[roomBest].Sign;
            r.DashboardStageIndex = dashBest;
            r.DashboardStageId = c.Glow.Stages[dashBest].IdValue;
            r.ChartIsMisleading = roomBest > dashBest;
            return r;
        }

        /// <summary>The highest stage that applies to a model with this weekly chance and incident rung.</summary>
        public static int StageFor(GameConfig c, int chancePermille, int rung)
        {
            int best = 0;
            for (int i = 0; i < c.Glow.Stages.Count; i++)
            {
                GlowStageConfig s = c.Glow.Stages[i];
                bool byChance = s.MinChancePermille >= 0 && chancePermille >= s.MinChancePermille;
                bool byRung = s.MinRung > 0 && rung >= s.MinRung;
                if (byChance || byRung) best = i;
            }
            return best;
        }

        static ModelState AsTheDashboardSees(ModelState model)
        {
            return new ModelState { Alignment = SafetySystem.DisplayedAlignment(model), DisplayedAlignment = SafetySystem.DisplayedAlignment(model), Corruption = 0 };
        }

        static int DashboardChance(GameConfig c, int tick, ModelState model) { return SafetySystem.IncidentChancePermille(c, tick, AsTheDashboardSees(model)); }

        static int DashboardRung(GameConfig c, ModelState model) { return SafetySystem.PickIncidentRung(c, AsTheDashboardSees(model)); }

        static ModelState LatestReady(LabState lab, string lineId)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != lineId || (m.Status != ModelStatus.Released && m.Status != ModelStatus.Trained)) continue;
                if (best == null || m.Version > best.Version) best = m;
            }
            return best;
        }
    }
}
