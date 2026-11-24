using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>State requests when a lab has real footprint in China (real country name in data).</summary>
    public sealed class ChinaDealsConfig : ISync
    {
        public string BlocId = "china";
        /// <summary>Installed kW in China before the ministry bothers you.</summary>
        public int MinInstalledKw;
        /// <summary>Grid power in China while the deal is active is billed at this permille of normal.</summary>
        public int ActivePowerPermille;
        /// <summary>Attention added in China when you accept oversight.</summary>
        public int AcceptAttention;
        /// <summary>Attention added when you decline or ignore the request.</summary>
        public int DeclineAttention;
        /// <summary>Months before another offer after a decline or ignored request.</summary>
        public int DeclineCooldownMonths;

        public void Sync(Syncer s)
        {
            s.Str("blocId", ref BlocId);
            s.Int("minInstalledKw", ref MinInstalledKw);
            s.Int("activePowerPermille", ref ActivePowerPermille);
            s.Int("acceptAttention", ref AcceptAttention);
            s.Int("declineAttention", ref DeclineAttention);
            s.Int("declineCooldownMonths", ref DeclineCooldownMonths);
        }
    }
}