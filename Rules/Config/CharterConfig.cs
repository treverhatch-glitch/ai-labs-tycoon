using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The lab's charter. It is a for-profit company, a capped-profit one or a nonprofit. A for-profit raises funding in full and wins smaller research grants; a capped-profit raises less, wins full grants and pays investors a share of its weekly profit until a cap is reached; a nonprofit cannot raise funding, wins the biggest grants and gets donations every week from its prestige. Changing form takes a review and a fee; leaving a nonprofit draws a challenge on the litigation docket and costs the staff's morale, becoming one lifts it. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class CharterConfig : ISync
    {
        public int Enabled;
        /// <summary>Weeks a change of charter is under review.</summary>
        public int ReviewWeeks;
        /// <summary>What a change of charter costs, paid now.</summary>
        public int ConvertFee;
        /// <summary>A capped-profit lab raises this share of a round's cash.</summary>
        public int CappedFundingPermille;
        /// <summary>A for-profit lab's research grant, as a share of the grant.</summary>
        public int ForProfitGrantPermille;
        /// <summary>A capped-profit lab's research grant, as a share of the grant.</summary>
        public int CappedGrantPermille;
        /// <summary>A nonprofit's research grant, as a share of the grant.</summary>
        public int NonprofitGrantPermille;
        /// <summary>Weekly donations to a nonprofit for each point of prestige.</summary>
        public int DonationPerPrestige;
        /// <summary>The share of a capped-profit lab's positive week paid to investors.</summary>
        public int DistributionPermille;
        /// <summary>What investors can take in all before the cap is reached.</summary>
        public int CapAmount;
        /// <summary>The claim filed when a nonprofit turns for-profit.</summary>
        public int ChallengeClaim;
        /// <summary>Its strength.</summary>
        public int ChallengeStrength;
        /// <summary>Morale every person loses when a nonprofit turns for-profit.</summary>
        public int ProfitMorale;
        /// <summary>Morale every person gains when a lab becomes a nonprofit.</summary>
        public int MissionMorale;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("reviewWeeks", ref ReviewWeeks);
            s.Int("convertFee", ref ConvertFee);
            s.Int("cappedFundingPermille", ref CappedFundingPermille);
            s.Int("forProfitGrantPermille", ref ForProfitGrantPermille);
            s.Int("cappedGrantPermille", ref CappedGrantPermille);
            s.Int("nonprofitGrantPermille", ref NonprofitGrantPermille);
            s.Int("donationPerPrestige", ref DonationPerPrestige);
            s.Int("distributionPermille", ref DistributionPermille);
            s.Int("capAmount", ref CapAmount);
            s.Int("challengeClaim", ref ChallengeClaim);
            s.Int("challengeStrength", ref ChallengeStrength);
            s.Int("profitMorale", ref ProfitMorale);
            s.Int("missionMorale", ref MissionMorale);
        }
    }
}
