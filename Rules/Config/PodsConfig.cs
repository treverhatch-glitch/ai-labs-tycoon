using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Project pods. The player picks a few of the lab's own people to work as one pod. The pod's chemistry comes from who is in it: different roles and a mix of senior and junior help, two people with the same quirk clash, the weeks together add, and the members' morale goes in. A pod with good chemistry adds research points every week; one with bad chemistry takes them away. A member can be swapped (the pod's weeks together are halved) and a pod that loses people breaks up. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class PodsConfig : ISync
    {
        public int Enabled;
        /// <summary>Fewest people in a pod.</summary>
        public int MinSize;
        /// <summary>Most people in a pod.</summary>
        public int MaxSize;
        /// <summary>Cash for each person to set a pod up.</summary>
        public int KickoffCost;
        /// <summary>Research points a week each member adds at the best chemistry (and takes away at the worst).</summary>
        public int PointsPerMember;
        /// <summary>Chemistry a pod starts from before its mix (0 to 1000).</summary>
        public int BaseChem;
        /// <summary>Chemistry for each different role in the pod beyond the first (up to 3 beyond).</summary>
        public int RolePoints;
        /// <summary>Chemistry when the pod has both a senior and a junior.</summary>
        public int MixPoints;
        /// <summary>Chemistry lost for each pair of members with the same quirk.</summary>
        public int ClashPoints;
        /// <summary>Chemistry a pod gains each week it stays together.</summary>
        public int TenureStep;
        /// <summary>Most chemistry tenure can add.</summary>
        public int TenureCap;
        /// <summary>Share of how far the members average morale is from 500 that goes into chemistry (permille).</summary>
        public int MoraleShare;
        /// <summary>Weeks between swaps of a member.</summary>
        public int SwapCooldown;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minSize", ref MinSize);
            s.Int("maxSize", ref MaxSize);
            s.Int("kickoffCost", ref KickoffCost);
            s.Int("pointsPerMember", ref PointsPerMember);
            s.Int("baseChem", ref BaseChem);
            s.Int("rolePoints", ref RolePoints);
            s.Int("mixPoints", ref MixPoints);
            s.Int("clashPoints", ref ClashPoints);
            s.Int("tenureStep", ref TenureStep);
            s.Int("tenureCap", ref TenureCap);
            s.Int("moraleShare", ref MoraleShare);
            s.Int("swapCooldown", ref SwapCooldown);
        }
    }
}
