using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A lab's trust with one sector's customers, from 0 to 1000. A sector with no record sits at the baseline.</summary>
    public sealed class SectorTrust : ISync
    {
        public string SectorId = "";
        public int Trust;

        public void Sync(Syncer s)
        {
            s.Str("sectorId", ref SectorId);
            s.Int("trust", ref Trust);
        }
    }

    /// <summary>A customer let down by an incident, waiting for an answer until <see cref="ExpiresTick"/>.</summary>
    public sealed class Complaint : ISync
    {
        public int Id;
        public string SectorId = "";
        public int ModelId;
        /// <summary>The trust the incident cost in that sector: what a good answer can win back and a bad one can lose a share of.</summary>
        public int Stake;
        public int ExpiresTick;
        /// <summary>The model's name.</summary>
        public string Ref = "";

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("sectorId", ref SectorId);
            s.Int("modelId", ref ModelId);
            s.Int("stake", ref Stake);
            s.Int("expiresTick", ref ExpiresTick);
            s.Str("ref", ref Ref);
        }
    }
}
