using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class IncidentRungConfig : ISync, IHasId
    {
        public string IdValue = "";
        public int Rung;
        public int ReputationLoss;
        /// <summary>Fine multiplier in permille applied to the usual revenue-weeks formula.</summary>
        public int FinePermille;
        /// <summary>When true, displayed alignment is snapped to the true meter after this incident.</summary>
        public bool ClosesGap;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("rung", ref Rung);
            s.Int("reputationLoss", ref ReputationLoss);
            s.Int("finePermille", ref FinePermille);
            s.Bool("closesGap", ref ClosesGap);
        }
    }
}