using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>The containment wing slows alignment work loss during agent-era training.</summary>
    public sealed class ContainmentConfig : ISync
    {
        public string RoomId = "containment-wing";
        public int AlignmentPermillePerTile;
        public int AlignmentCapPermille;

        public void Sync(Syncer s)
        {
            s.Str("roomId", ref RoomId);
            s.Int("alignmentPermillePerTile", ref AlignmentPermillePerTile);
            s.Int("alignmentCapPermille", ref AlignmentCapPermille);
        }
    }
}