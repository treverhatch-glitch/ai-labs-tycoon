using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class StaffPeopleConfig : ISync
    {
        public int MoraleBaseline;
        public int MoraleDriftPerWeek;
        /// <summary>Extra morale lost each week while the lab is in debt.</summary>
        public int MoraleDebtDriftPerWeek;
        /// <summary>Morale lost by each staff member when a released model has an incident.</summary>
        public int MoraleIncidentDrop;
        /// <summary>Morale lost by remaining staff when a rival poaches a star.</summary>
        public int MoralePoachWitnessDrop;
        public int PoachChancePermille;
        /// <summary>Chance each hire is a star trait, in permille.</summary>
        public int StarHirePermille;
        /// <summary>The most morale points the comfort rooms can add to the level morale settles at.</summary>
        public int MaxMoraleLift;
        /// <summary>The most the rooms can take off the chance of a rival's offer, in permille.</summary>
        public int MaxPoachDefencePermille;
        /// <summary>How far research rises for every 1000 points a researcher's morale sits above the baseline, in permille.</summary>
        public int MoraleResearchSlopePermille;
        /// <summary>How far research falls for every 1000 points a researcher's morale sits below the baseline, in permille. Zero in the shipped data (see Q180).</summary>
        public int MoraleResearchLossSlopePermille;
        /// <summary>The most the traits can add to, or take off, the lab's training speed or alignment work, in permille.</summary>
        public int MaxTraitSwingPermille;
        /// <summary>The most the mentors can add to every researcher's output, in permille.</summary>
        public int MaxMentorPermille;
        /// <summary>The most the people who are secretly interviewing can add to the chance of a rival's offer, in permille of the usual chance.</summary>
        public int MaxOfferBoostPermille;
        /// <summary>Research lost for each doomer and accelerationist pair, in permille.</summary>
        public int FrictionPerPairPermille;
        /// <summary>The most research arguing can cost, in permille.</summary>
        public int MaxFrictionPermille;
        /// <summary>Added to a hire's id before the trait roll. Any whole number does; a different one gives different people the same traits.</summary>
        public int TraitRollSalt;
        /// <summary>The most the people who post through it can add to a launch's buzz, and to what a scandal costs, each in permille.</summary>
        public int MaxPostsPermille;
        /// <summary>Reputation points a person quitting by blog post costs the lab, before the press room has its say.</summary>
        public int QuitReputation;
        /// <summary>Extra research the whole team makes in crunch time, in permille.</summary>
        public int CrunchResearchPermille;
        /// <summary>Extra training speed in crunch time, in permille.</summary>
        public int CrunchTrainingPermille;
        /// <summary>Morale every person loses each week of crunch, before the weekly drift pulls it back.</summary>
        public int CrunchMoraleDrop;
        /// <summary>Weekly incident chance a lab in crunch adds for each week it has been in crunch, in permille.</summary>
        public int CrunchIncidentPerWeek;
        /// <summary>The most crunch time adds to the weekly incident chance, in permille.</summary>
        public int CrunchIncidentCap;
        /// <summary>Crunch cannot start while the team's average morale is below this.</summary>
        public int CrunchMinMorale;
        /// <summary>Crunch ends by itself when the average morale falls below this.</summary>
        public int CrunchBreakMorale;
        /// <summary>Crunch ends by itself after this many weeks.</summary>
        public int CrunchMaxWeeks;
        public List<StaffTraitConfig> Traits = new List<StaffTraitConfig>();

        public StaffTraitConfig Trait(string id)
        {
            for (int i = 0; i < Traits.Count; i++) if (Traits[i].Id == id) return Traits[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("moraleBaseline", ref MoraleBaseline);
            s.Int("moraleDriftPerWeek", ref MoraleDriftPerWeek);
            s.Int("moraleDebtDriftPerWeek", ref MoraleDebtDriftPerWeek);
            s.Int("moraleIncidentDrop", ref MoraleIncidentDrop);
            s.Int("moralePoachWitnessDrop", ref MoralePoachWitnessDrop);
            s.Int("poachChancePermille", ref PoachChancePermille);
            s.Int("starHirePermille", ref StarHirePermille);
            s.Int("maxMoraleLift", ref MaxMoraleLift);
            s.Int("maxPoachDefencePermille", ref MaxPoachDefencePermille);
            s.Int("moraleResearchSlopePermille", ref MoraleResearchSlopePermille);
            s.Int("moraleResearchLossSlopePermille", ref MoraleResearchLossSlopePermille);
            s.Int("maxTraitSwingPermille", ref MaxTraitSwingPermille);
            s.Int("maxMentorPermille", ref MaxMentorPermille);
            s.Int("maxOfferBoostPermille", ref MaxOfferBoostPermille);
            s.Int("frictionPerPairPermille", ref FrictionPerPairPermille);
            s.Int("maxFrictionPermille", ref MaxFrictionPermille);
            s.Int("traitRollSalt", ref TraitRollSalt);
            s.Int("maxPostsPermille", ref MaxPostsPermille);
            s.Int("quitReputation", ref QuitReputation);
            s.Int("crunchResearchPermille", ref CrunchResearchPermille);
            s.Int("crunchTrainingPermille", ref CrunchTrainingPermille);
            s.Int("crunchMoraleDrop", ref CrunchMoraleDrop);
            s.Int("crunchIncidentPerWeek", ref CrunchIncidentPerWeek);
            s.Int("crunchIncidentCap", ref CrunchIncidentCap);
            s.Int("crunchMinMorale", ref CrunchMinMorale);
            s.Int("crunchBreakMorale", ref CrunchBreakMorale);
            s.Int("crunchMaxWeeks", ref CrunchMaxWeeks);
            s.ObjList("traits", ref Traits);
        }
    }

    public sealed class StaffTraitConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int ResearchPermille;
        public bool Star;
        /// <summary>The share of the player's hires that arrive with this trait, in permille. The star trait is rolled apart by starHirePermille.</summary>
        public int HirePermille;
        /// <summary>What this person adds to the lab's training speed, in permille (negative slows it).</summary>
        public int TrainingSpeedPermille;
        /// <summary>What this person adds to the lab's alignment work, in permille (negative loses some).</summary>
        public int AlignmentWorkPermille;
        /// <summary>What this person adds to the share of incidents the lab catches, in permille (negative loses some).</summary>
        public int IncidentCatchPermille;
        /// <summary>What this person adds to every researcher's output, in permille.</summary>
        public int MentorPermille;
        /// <summary>What this person adds to the chance a rival's offer arrives, in permille of the usual chance.</summary>
        public int OfferPermille;
        /// <summary>Morale points added to the level this person settles at (negative lowers it).</summary>
        public int RestMoraleOffset;
        /// <summary>The camp this person is in ("doom" or "accel"); a lab with both camps starts arguing. Empty for everyone else.</summary>
        public string Camp = "";
        /// <summary>What this person adds to the launch buzz of the lab's models, in permille of the usual buzz (the live posts).</summary>
        public int BuzzPermille;
        /// <summary>What this person adds to the reputation a scandal costs the lab, in permille of the loss (the live posts).</summary>
        public int ScandalPermille;
        /// <summary>Above zero: this person quits, by blog post, the first week their morale is below this.</summary>
        public int QuitBelowMorale;
        /// <summary>Above zero: this person's own research is this much higher while the lab has a variant run training, and this much lower while it does not.</summary>
        public int InterestingPermille;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("researchPermille", ref ResearchPermille);
            s.Bool("star", ref Star);
            s.Int("hirePermille", ref HirePermille);
            s.Int("trainingSpeedPermille", ref TrainingSpeedPermille);
            s.Int("alignmentWorkPermille", ref AlignmentWorkPermille);
            s.Int("incidentCatchPermille", ref IncidentCatchPermille);
            s.Int("mentorPermille", ref MentorPermille);
            s.Int("offerPermille", ref OfferPermille);
            s.Int("restMoraleOffset", ref RestMoraleOffset);
            s.Str("camp", ref Camp);
            s.Int("buzzPermille", ref BuzzPermille);
            s.Int("scandalPermille", ref ScandalPermille);
            s.Int("quitBelowMorale", ref QuitBelowMorale);
            s.Int("interestingPermille", ref InterestingPermille);
            s.Str("blurb", ref Blurb);
        }
    }
}