using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Surfacing rival decision-log lines on the news ticker (Q475, schema 150).</summary>
    public sealed class RivalIntentNewsConfig : ISync
    {
        public int Enabled;
        public int MinWeeksBetween;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minWeeksBetween", ref MinWeeksBetween);
        }
    }
}