using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// How a sector fails (see <see cref="SectorFlavourConfig"/>). Every number is read from the data and the model's own record, so nothing
    /// is saved and nothing draws from a dice stream.
    /// </summary>
    public static class SectorFlavours
    {
        /// <summary>The one line on what goes wrong in a sector, or "" when it fails the plain way.</summary>
        public static string Failure(GameConfig c, string sectorId)
        {
            SectorFlavourConfig f = c.Safety.SectorFlavour(sectorId);
            return f == null ? "" : f.Failure;
        }

        public static int GlitchWeeks(GameConfig c, string sectorId)
        {
            SectorFlavourConfig f = c.Safety.SectorFlavour(sectorId);
            return f == null || f.GlitchWeeks <= 0 ? c.Safety.GlitchWeeks : f.GlitchWeeks;
        }

        public static int GlitchQualityPermille(GameConfig c, string sectorId)
        {
            SectorFlavourConfig f = c.Safety.SectorFlavour(sectorId);
            return f == null || f.GlitchQualityPermille <= 0 ? c.Safety.GlitchQualityPermille : f.GlitchQualityPermille;
        }

        /// <summary>True while an incident still weighs on the model's quality in this sector.</summary>
        public static bool IsGlitching(GameConfig c, ModelState model, string sectorId, int tick)
        {
            return model.Incidents > 0 && tick - model.LastIncidentTick < GlitchWeeks(c, sectorId);
        }

        /// <summary>True when this lab pays what a flavour costs: the player's labs always, rivals only when the data says so.</summary>
        public static bool Pays(GameConfig c, GameState state, LabState lab)
        {
            return lab.Id == state.PlayerLabId || c.Safety.SectorFlavoursForRivals;
        }

        /// <summary>The multiplier on the trust an incident costs in this sector.</summary>
        public static int TrustPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (!Pays(c, state, lab)) return MathI.Permille;
            SectorFlavourConfig f = c.Safety.SectorFlavour(sectorId);
            return f == null ? MathI.Permille : f.TrustPermille;
        }

        /// <summary>
        /// The flavour that sets how hard an incident hits a model: of the flavoured sectors the lab sells it in, the one where it takes the most.
        /// Null when none of its sectors has a flavour.
        /// </summary>
        public static SectorFlavourConfig Leading(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Pays(c, state, lab)) return null;
            SectorFlavourConfig best = null;
            long bestTakings = -1;
            for (int i = 0; i < model.Sectors.Count; i++)
            {
                SectorFlavourConfig f = c.Safety.SectorFlavour(model.Sectors[i]);
                if (f == null) continue;
                long takings = Takings(state, lab, model, model.Sectors[i]);
                if (takings > bestTakings)
                {
                    best = f;
                    bestTakings = takings;
                }
            }
            return best;
        }

        /// <summary>What the lab's model took in a sector last week.</summary>
        static long Takings(GameState state, LabState lab, ModelState model, string sectorId)
        {
            SectorState sector = state.Sector(sectorId);
            SectorEntry entry = sector == null ? null : sector.Entry(lab.Id);
            return entry == null || entry.ModelId != model.Id ? 0 : entry.LastRevenue;
        }

        /// <summary>The refunds and credits an incident costs on top of the fine: the make-good weeks of every flavoured sector the model sells in.</summary>
        public static long MakeGood(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long total = 0;
            if (!Pays(c, state, lab)) return 0;
            for (int i = 0; i < model.Sectors.Count; i++)
            {
                SectorFlavourConfig f = c.Safety.SectorFlavour(model.Sectors[i]);
                if (f == null || f.MakeGoodWeeks <= 0) continue;
                total += Takings(state, lab, model, model.Sectors[i]) * f.MakeGoodWeeks;
            }
            return total;
        }
    }
}
