using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SpecialistEraHooks
    {
        public static bool Enabled(GameConfig c) { return c.SpecialistEraHooks.Enabled > 0; }

        static bool EraOpen(GameConfig c, GameState state)
        {
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            return era >= c.SpecialistEraHooks.MinEra;
        }

        static bool SpecInSector(GameConfig c, ModelState model, string sectorId)
        {
            if (model.SpecKind <= 0) return false;
            int idx = model.SpecKind - 1;
            if (idx < 0 || idx >= c.SpecialistLines.Kinds.Count) return false;
            SpecialistKindConfig k = c.SpecialistLines.Kinds[idx];
            for (int i = 0; i < k.Sectors.Count; i++)
                if (k.Sectors[i] == sectorId) return true;
            return false;
        }

        static bool WeightsInSector(ModelState model, string sectorId)
        {
            if (model.WgtOpen <= 0) return false;
            for (int i = 0; i < model.Sectors.Count; i++)
                if (model.Sectors[i] == sectorId) return true;
            return false;
        }

        public static int AppealPermille(GameConfig c, GameState state, ModelState model, string sectorId)
        {
            if (!Enabled(c) || !SpecialistLines.Enabled(c) || !EraOpen(c, state)) return 0;
            int add = 0;
            if (SpecInSector(c, model, sectorId)) add += c.SpecialistEraHooks.SpecialistPermille;
            if (OpenWeights.Enabled(c) && WeightsInSector(model, sectorId)) add += c.SpecialistEraHooks.OpenWeightsPermille;
            return add;
        }

        public static void OnEraBegan(GameConfig c, GameState state, int eraIndex)
        {
            if (!Enabled(c)) return;
            if (eraIndex != c.SpecialistEraHooks.MinEra) return;
            Events.Add(state, EventKind.SpecialistEraUnlocked, state.PlayerLabId, eraIndex, 0, c.Eras[eraIndex].Id);
        }
    }
}