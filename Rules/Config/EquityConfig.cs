using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Staff equity: the founder gives the team a slice of the company, one level at a time. Each level takes a share off the founder's
    /// stake (the same stake funding rounds dilute) and gives it to the people, who then accept lower salaries, settle at higher morale and
    /// are harder to poach. The grant cannot be taken back, funding rounds dilute it along with the founder's share, and the people's stake
    /// comes out of the price when the lab is sold. Only the player's lab grants equity. Zero levels turns it off.
    /// </summary>
    public sealed class EquityConfig : ISync
    {
        /// <summary>The most levels that can be granted. Zero turns staff equity off.</summary>
        public int MaxLevel;
        /// <summary>The share of the founder's stake each level hands over, in permille of that stake.</summary>
        public int DilutionPermille;
        /// <summary>What each level takes off every salary, in permille.</summary>
        public int SalaryCutPermille;
        /// <summary>The morale each level adds to where morale settles.</summary>
        public int MoraleStep;
        /// <summary>The share each level takes off a rival's chance to make an offer, in permille.</summary>
        public int PoachStepPermille;
        /// <summary>The people on the payroll needed before a grant means anything.</summary>
        public int MinStaff;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("dilutionPermille", ref DilutionPermille);
            s.Int("salaryCutPermille", ref SalaryCutPermille);
            s.Int("moraleStep", ref MoraleStep);
            s.Int("poachStepPermille", ref PoachStepPermille);
            s.Int("minStaff", ref MinStaff);
        }
    }
}
