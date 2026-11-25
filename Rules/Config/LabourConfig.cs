using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Labour relations. Pressure builds in the player's lab from layoffs, crunch time and low morale. When it tips the staff organise: the lab recognises the union (a settled contract with better pay, dearer layoffs, no crunch and a little more research), or resists it (legal fees and a bet that morale recovers before pressure reaches a strike). A strike halts research and training and ends with a harder contract. Rivals never organise. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LabourConfig : ISync
    {
        public int Enabled;
        /// <summary>Pressure builds only in a lab of at least this many people.</summary>
        public int MinStaff;
        /// <summary>A week with the average morale under this adds <c>LowMoralePerWeek</c> pressure.</summary>
        public int MoraleBelow;
        public int LowMoralePerWeek;
        /// <summary>A week of crunch time adds this much pressure.</summary>
        public int CrunchPerWeek;
        /// <summary>A layoff round adds this much pressure for letting the whole staff go, scaled by the share it lets go.</summary>
        public int LayoffPerFull;
        /// <summary>Pressure falls by this much every week.</summary>
        public int DecayPerWeek;
        /// <summary>At this pressure (of 1000) the staff organise.</summary>
        public int DrivePressure;
        /// <summary>Weeks the player has to recognise or resist a drive; after that it counts as resisting.</summary>
        public int WindowWeeks;
        /// <summary>Weeks a resisted drive stays live. If pressure is back under the drive level at the end, it collapses.</summary>
        public int ResistWeeks;
        /// <summary>What resisting does to the weekly pressure added, in permille (2000 doubles it).</summary>
        public int ResistFactorPermille;
        /// <summary>Legal fees for resisting.</summary>
        public int ResistCost;
        /// <summary>The pressure left when a resisted drive collapses.</summary>
        public int HeldPressure;
        /// <summary>Weeks of a strike.</summary>
        public int StrikeWeeks;
        /// <summary>Morale every person loses when a strike starts.</summary>
        public int StrikeMorale;
        /// <summary>Research and training lost while the staff are out, in permille.</summary>
        public int StrikeCutPermille;
        /// <summary>Morale every person gains when the lab recognises the union.</summary>
        public int RecogniseMorale;
        /// <summary>What the contract adds to the pay bill.</summary>
        public int PayPermille;
        /// <summary>The same for a contract forced by a strike.</summary>
        public int HardPayPermille;
        /// <summary>What the contract does to severance, in permille (2000 doubles it).</summary>
        public int SeverancePermille;
        /// <summary>The same for a contract forced by a strike.</summary>
        public int HardSeverancePermille;
        /// <summary>Research a settled team adds, in permille.</summary>
        public int ResearchPermille;
        /// <summary>The same for a contract forced by a strike.</summary>
        public int HardResearchPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minStaff", ref MinStaff);
            s.Int("moraleBelow", ref MoraleBelow);
            s.Int("lowMoralePerWeek", ref LowMoralePerWeek);
            s.Int("crunchPerWeek", ref CrunchPerWeek);
            s.Int("layoffPerFull", ref LayoffPerFull);
            s.Int("decayPerWeek", ref DecayPerWeek);
            s.Int("drivePressure", ref DrivePressure);
            s.Int("windowWeeks", ref WindowWeeks);
            s.Int("resistWeeks", ref ResistWeeks);
            s.Int("resistFactorPermille", ref ResistFactorPermille);
            s.Int("resistCost", ref ResistCost);
            s.Int("heldPressure", ref HeldPressure);
            s.Int("strikeWeeks", ref StrikeWeeks);
            s.Int("strikeMorale", ref StrikeMorale);
            s.Int("strikeCutPermille", ref StrikeCutPermille);
            s.Int("recogniseMorale", ref RecogniseMorale);
            s.Int("payPermille", ref PayPermille);
            s.Int("hardPayPermille", ref HardPayPermille);
            s.Int("severancePermille", ref SeverancePermille);
            s.Int("hardSeverancePermille", ref HardSeverancePermille);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("hardResearchPermille", ref HardResearchPermille);
        }
    }
}
