using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Industry standards. The industry drafts a standard of the next level every so often and its support grows each week; when it reaches what the level needs, the standard is ratified and binds every first release of a capable model. The player's lab takes a part in the draft (observer, member, sponsor or obstructor), which speeds it up or holds it back for dues, and complies by adopting the standards (a cost, and an upkeep for each level); a capable model released without complying is fined and costs reputation. Adopted levels weaken negligence claims and stand in for safety people in a scaling policy's safeguards. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class StandardsConfig : ISync
    {
        public int Enabled;
        /// <summary>The highest standard level.</summary>
        public int MaxLevel;
        /// <summary>Weeks before the industry drafts the first standard.</summary>
        public int FirstWeeks;
        /// <summary>Weeks after one is ratified before the next is drafted.</summary>
        public int EveryWeeks;
        /// <summary>Support a draft of level 1 needs; each level above needs this much again.</summary>
        public int SupportNeeded;
        /// <summary>Support a draft gains each week without the lab.</summary>
        public int IndustryRate;
        /// <summary>Support a week a member adds.</summary>
        public int MemberRate;
        /// <summary>Support a week a sponsor adds.</summary>
        public int SponsorRate;
        /// <summary>Support a week an obstructor takes away, never below nothing in all.</summary>
        public int ObstructRate;
        /// <summary>Dues a week while a draft is open, for a member.</summary>
        public int MemberDues;
        /// <summary>Dues a week while a draft is open, for a sponsor.</summary>
        public int SponsorDues;
        /// <summary>What obstructing costs a week while a draft is open.</summary>
        public int ObstructDues;
        /// <summary>What it costs to become a sponsor.</summary>
        public int SponsorFee;
        /// <summary>Weeks between the reputation costs of obstructing.</summary>
        public int ObstructEvery;
        /// <summary>Reputation obstructing costs each time, before the press damps it.</summary>
        public int ObstructReputation;
        /// <summary>What adopting costs for each level.</summary>
        public int AdoptCost;
        /// <summary>Weekly upkeep for each level adopted.</summary>
        public int UpkeepPerLevel;
        /// <summary>The capability from which a first release is bound by the standards.</summary>
        public int BindingCapability;
        /// <summary>Fine for each level a bound release lacks.</summary>
        public int LapseFine;
        /// <summary>Reputation lost for each level a bound release lacks, before the press damps it.</summary>
        public int LapseReputation;
        /// <summary>Strength each adopted level takes off a negligence claim.</summary>
        public int ClaimStrength;
        /// <summary>Safety people each adopted level stands in for in a scaling policy's safeguards.</summary>
        public int SafetyRelief;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxLevel", ref MaxLevel);
            s.Int("firstWeeks", ref FirstWeeks);
            s.Int("everyWeeks", ref EveryWeeks);
            s.Int("supportNeeded", ref SupportNeeded);
            s.Int("industryRate", ref IndustryRate);
            s.Int("memberRate", ref MemberRate);
            s.Int("sponsorRate", ref SponsorRate);
            s.Int("obstructRate", ref ObstructRate);
            s.Int("memberDues", ref MemberDues);
            s.Int("sponsorDues", ref SponsorDues);
            s.Int("obstructDues", ref ObstructDues);
            s.Int("sponsorFee", ref SponsorFee);
            s.Int("obstructEvery", ref ObstructEvery);
            s.Int("obstructReputation", ref ObstructReputation);
            s.Int("adoptCost", ref AdoptCost);
            s.Int("upkeepPerLevel", ref UpkeepPerLevel);
            s.Int("bindingCapability", ref BindingCapability);
            s.Int("lapseFine", ref LapseFine);
            s.Int("lapseReputation", ref LapseReputation);
            s.Int("claimStrength", ref ClaimStrength);
            s.Int("safetyRelief", ref SafetyRelief);
        }
    }
}
