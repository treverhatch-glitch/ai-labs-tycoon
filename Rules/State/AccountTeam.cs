using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>Salespeople assigned to one sector: how many, and the weeks they have had to learn its customers.</summary>
    public sealed class AccountTeam : ISync
    {
        public string SectorId = "";
        public int People;
        public int Weeks;

        public void Sync(Syncer s)
        {
            s.Str("sectorId", ref SectorId);
            s.Int("people", ref People);
            s.Int("weeks", ref Weeks);
        }
    }
}
