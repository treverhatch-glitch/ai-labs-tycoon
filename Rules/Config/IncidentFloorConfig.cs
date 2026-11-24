using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Floors under everything that can make incidents cheaper or rarer. Cash and plans that cut an incident's fine (an on-call rota, insurance,
    /// any later plan) and anything that cuts the weekly chance (a chief safety officer, any later plan) are taken off one after the other, so
    /// without a floor a lab that buys all of them would pay almost nothing and almost never have one. The fine floor is the share of the
    /// incident's gross fine (after the rung, the sector, the risk class and the era) the lab always pays itself; the chance floor is the share
    /// of the uncut weekly chance that always remains. Zero turns a floor off.
    /// </summary>
    public sealed class IncidentFloorConfig : ISync
    {
        /// <summary>The share of the gross fine the lab always pays after every cut and every insurance payout, in permille.</summary>
        public int FineFloorPermille;
        /// <summary>The share of the uncut weekly incident chance that always remains after every cut, in permille.</summary>
        public int ChanceFloorPermille;

        public void Sync(Syncer s)
        {
            s.Int("fineFloorPermille", ref FineFloorPermille);
            s.Int("chanceFloorPermille", ref ChanceFloorPermille);
        }
    }
}
