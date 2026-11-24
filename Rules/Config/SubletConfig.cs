using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Subletting compute (bible ch. 3: what the lab does not use, someone else will). The player lets a share of the lab's general compute
    /// go to outside customers for a share of what the same GPUs would rent for. The sublet compute leaves training, research-adjacent work and
    /// the lab's own customers' serving, so the lab is slower and can be overloaded. Leases need notice, so the share cannot be flipped every
    /// week. Zero share changes nothing.
    /// </summary>
    public sealed class SubletConfig : ISync
    {
        /// <summary>The most of the general compute the lab can sublet, in permille. Zero turns subletting off.</summary>
        public int MaxPermille;
        /// <summary>What a unit of sublet compute earns, as a share of what GPUs of the current generation rent for, in permille.</summary>
        public int RevenuePermille;
        /// <summary>Weeks after a change before the share can be changed again.</summary>
        public int NoticeWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxPermille", ref MaxPermille);
            s.Int("revenuePermille", ref RevenuePermille);
            s.Int("noticeWeeks", ref NoticeWeeks);
        }
    }
}
