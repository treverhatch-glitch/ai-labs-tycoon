using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Unity.Hq
{
    /// <summary>Presentation label for garage-era vs campus-era HQ lots.</summary>
    public static class HqCampusPhase
    {
        public static string LabelForEra(GameConfig config, string eraId, LabState lab = null)
        {
            int index = EraIndex(config, eraId);
            long start = config.PlayerStart != null ? config.PlayerStart.Cash : 1;
            if (start < 1) start = 1;
            long cash = lab != null ? lab.Cash : start;
            bool growing = cash >= start / 2;

            if (index <= 1 && !growing) return "Garage lot";
            if (index <= 1 && growing) return "Campus HQ";
            if (index <= 4) return "Campus HQ";
            return "Global campus";
        }

        public static int EraIndex(GameConfig config, string eraId)
        {
            for (int i = 0; i < config.Eras.Count; i++)
                if (config.Eras[i].Id == eraId) return i;
            return -1;
        }
    }
}