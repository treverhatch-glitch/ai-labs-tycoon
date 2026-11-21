using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Harness-era synthetic data lab cuts corruption and collapse odds when training on the synthetic mix.</summary>
    public static class SyntheticDataLabSystem
    {
        public static bool HasRoom(LabState lab, string roomTypeId)
        {
            for (int i = 0; i < lab.Rooms.Count; i++)
                if (lab.Rooms[i].TypeId == roomTypeId) return true;
            return false;
        }

        public static bool Active(GameConfig c, GameState state, LabState lab, string dataSourceId)
        {
            if (lab.Bankrupt) return false;
            SyntheticDataLabConfig cfg = c.Training.SyntheticDataLab;
            if (cfg == null || (cfg.CorruptionCutPermille < 1 && cfg.FailRiskCutPermille < 1)) return false;
            if (dataSourceId != cfg.DataSourceId) return false;
            if (!EraSystem.Open(c, cfg.EraId, state.Tick)) return false;
            return HasRoom(lab, cfg.RoomTypeId);
        }

        public static int EffectiveFinishCorruption(GameConfig c, GameState state, LabState lab, DataSourceConfig source)
        {
            if (source == null || source.FinishCorruption < 1) return 0;
            SyntheticDataLabConfig cfg = c.Training.SyntheticDataLab;
            if (!Active(c, state, lab, source.Id)) return source.FinishCorruption;
            return cfg == null ? source.FinishCorruption
                : (int)Core.MathI.MulPermille(source.FinishCorruption, Core.MathI.Permille - cfg.CorruptionCutPermille);
        }

        public static int EffectiveWeeklyFailRiskPermille(GameConfig c, GameState state, LabState lab, DataSourceConfig source)
        {
            if (source == null) return 0;
            int risk = source.WeeklyFailRiskPermille;
            SyntheticDataLabConfig cfg = c.Training.SyntheticDataLab;
            if (!Active(c, state, lab, source.Id) || cfg == null || cfg.FailRiskCutPermille < 1) return risk;
            return (int)Core.MathI.MulPermille(risk, Core.MathI.Permille - cfg.FailRiskCutPermille);
        }
    }
}