using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The ethics board: outside names the player seats to look over the lab's shoulder. Level 1 refuses a rushed launch, takes some of the
    /// sting out of an incident's reputation loss, helps at hearings and eases regulator attention. Level 2 also refuses to let a model go on
    /// sale for the first time if its true alignment (not the displayed one) is under the data's bar. The board costs a weekly fee, needs safety
    /// staff to cover each level, and cannot be waved away: stepping a level down, or losing the staff to keep it, is a public resignation that
    /// costs reputation and keeps the board away for a year. Only the player's lab has one. Zero levels turns it off.
    /// </summary>
    public sealed class EthicsBoardConfig : ISync
    {
        /// <summary>The highest level. Zero turns the board off.</summary>
        public int MaxLevel;
        /// <summary>Cash for each level seated (paid for the levels added).</summary>
        public long SetupCostPerLevel;
        /// <summary>Cash every week for each level.</summary>
        public long WeeklyCostPerLevel;
        /// <summary>The safety people each level needs on the payroll.</summary>
        public int SafetyStaffPerLevel;
        /// <summary>The share each level takes off an incident's reputation loss, in permille.</summary>
        public int ScandalCutPermillePerLevel;
        /// <summary>The reputation points each level adds to how a hearing goes.</summary>
        public int HearingPointsPerLevel;
        /// <summary>The share each level takes off the monthly regulator attention gain, in permille.</summary>
        public int AttentionPermillePerLevel;
        /// <summary>From this level on the board vetoes a first release whose true alignment is under <see cref="VetoAlignment"/>.</summary>
        public int VetoLevel;
        /// <summary>The true alignment (0 to 1000) a first release needs once the board can veto.</summary>
        public int VetoAlignment;
        /// <summary>The reputation lost when the board is stepped down or resigns.</summary>
        public int ResignationReputation;
        /// <summary>The weeks after a step down or a resignation before a board can be seated again.</summary>
        public int LockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("setupCostPerLevel", ref SetupCostPerLevel);
            s.Long("weeklyCostPerLevel", ref WeeklyCostPerLevel);
            s.Int("safetyStaffPerLevel", ref SafetyStaffPerLevel);
            s.Int("scandalCutPermillePerLevel", ref ScandalCutPermillePerLevel);
            s.Int("hearingPointsPerLevel", ref HearingPointsPerLevel);
            s.Int("attentionPermillePerLevel", ref AttentionPermillePerLevel);
            s.Int("vetoLevel", ref VetoLevel);
            s.Int("vetoAlignment", ref VetoAlignment);
            s.Int("resignationReputation", ref ResignationReputation);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }
}
