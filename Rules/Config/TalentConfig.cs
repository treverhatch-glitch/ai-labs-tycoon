using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The talent market (bible ch. 7, "poaching is constant"): a rival's offer to one of the player's people waits a few weeks for an answer,
    /// the answer is a counter (money, compute, a title, a mission), and the player can try the same on a rival's people.
    /// </summary>
    public sealed class TalentConfig : ISync
    {
        /// <summary>Weeks a rival's offer waits for an answer before the person leaves. Zero means the person leaves at once.</summary>
        public int OfferWeeks;
        /// <summary>The most offers waiting at one time; a rival that wants to poach more has to wait.</summary>
        public int MaxOpenOffers;
        /// <summary>What a person's package is worth, in hiring fees of the era (so a talent war makes everyone dearer).</summary>
        public int PackageMultiple;
        /// <summary>A star's package as a share of an ordinary person's: 2000 is double.</summary>
        public int StarPackagePermille;
        /// <summary>No counter is ever less likely than this, whatever the morale or the name of the lab.</summary>
        public int MinSuccessPermille;
        /// <summary>The headhunter's fee for trying to poach from a rival, as a share of the package, paid win or lose.</summary>
        public int PoachBackFeePermille;
        public int PoachBackBasePermille;
        /// <summary>Per permille the target's morale is below the baseline (or above, if negative), the chance moves by this share.</summary>
        public int PoachBackMoraleSlopePermille;
        /// <summary>Per permille the player's reputation is above 500, the chance moves by this share.</summary>
        public int PoachBackReputationSlopePermille;
        /// <summary>How much more the rival dislikes the player after a poach that worked, and after one that did not.</summary>
        public int PoachBackFeelingHit;
        public int PoachBackFailFeelingHit;
        public List<CounterConfig> Counters = new List<CounterConfig>();

        public CounterConfig Counter(string id)
        {
            for (int i = 0; i < Counters.Count; i++) if (Counters[i].Id == id) return Counters[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("offerWeeks", ref OfferWeeks);
            s.Int("maxOpenOffers", ref MaxOpenOffers);
            s.Int("packageMultiple", ref PackageMultiple);
            s.Int("starPackagePermille", ref StarPackagePermille);
            s.Int("minSuccessPermille", ref MinSuccessPermille);
            s.Int("poachBackFeePermille", ref PoachBackFeePermille);
            s.Int("poachBackBasePermille", ref PoachBackBasePermille);
            s.Int("poachBackMoraleSlopePermille", ref PoachBackMoraleSlopePermille);
            s.Int("poachBackReputationSlopePermille", ref PoachBackReputationSlopePermille);
            s.Int("poachBackFeelingHit", ref PoachBackFeelingHit);
            s.Int("poachBackFailFeelingHit", ref PoachBackFailFeelingHit);
            s.ObjList("counters", ref Counters);
        }
    }

    /// <summary>One way to answer a rival's offer. It costs a share of the package, paid whether or not it works; the chance is a base plus the person's morale and the lab's name.</summary>
    public sealed class CounterConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Blurb = "";
        public int CostPermille;
        public int BaseSuccessPermille;
        /// <summary>Per permille the person's morale is above the baseline, the chance moves by this share.</summary>
        public int MoraleSlopePermille;
        /// <summary>Per permille the lab's reputation is above 500, the chance moves by this share.</summary>
        public int ReputationSlopePermille;
        /// <summary>Morale the person gains if they stay.</summary>
        public int MoraleGain;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.Int("costPermille", ref CostPermille);
            s.Int("baseSuccessPermille", ref BaseSuccessPermille);
            s.Int("moraleSlopePermille", ref MoraleSlopePermille);
            s.Int("reputationSlopePermille", ref ReputationSlopePermille);
            s.Int("moraleGain", ref MoraleGain);
        }
    }
}
