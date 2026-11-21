using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The treasury reserve. Cash the player sets aside stops being spendable and earns a small yield every week. Taking it back costs a fee. If the weekly bill leaves the lab
    /// in debt, the reserve pays it first at a steeper fee, so a lab that saved can ride out a bad month that would have closed it. Only the player's lab keeps a reserve,
    /// and an empty one does nothing. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class TreasuryConfig : ISync
    {
        /// <summary>1 when a lab can keep a reserve, 0 when not.</summary>
        public int Enabled;
        /// <summary>The least a lab can set aside or take back in one go.</summary>
        public int MinMove;
        /// <summary>What the reserve earns every week, in parts per million of what is in it.</summary>
        public int WeeklyYieldPpm;
        /// <summary>The share of money taken back on purpose that is lost, in permille.</summary>
        public int DrawFeePermille;
        /// <summary>The share of money the reserve pays out to cover a debt that is lost, in permille. Dearer than a planned draw.</summary>
        public int CoverFeePermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minMove", ref MinMove);
            s.Int("weeklyYieldPpm", ref WeeklyYieldPpm);
            s.Int("drawFeePermille", ref DrawFeePermille);
            s.Int("coverFeePermille", ref CoverFeePermille);
        }
    }
}
