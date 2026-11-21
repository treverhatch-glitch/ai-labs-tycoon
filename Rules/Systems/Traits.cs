using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>What the traits of a lab's people add up to, split as a screen would show it. Worked out from the staff each time; nothing is saved.</summary>
    public struct TraitReport
    {
        /// <summary>Training speed added (negative takes some off), after the cap.</summary>
        public int TrainingSpeedPermille;
        /// <summary>Alignment work added (negative takes some off), after the cap.</summary>
        public int AlignmentWorkPermille;
        /// <summary>The share of incidents caught that the traits add (negative takes some off), before the 0 to 900 limit on the whole.</summary>
        public int IncidentCatchPermille;
        /// <summary>The share every researcher's output gains from mentors, after the cap.</summary>
        public int MentorPermille;
        /// <summary>The share added to the chance of a rival's offer, after the cap (never negative).</summary>
        public int OfferBoostPermille;
        public int Doomers;
        public int Accelerationists;
        /// <summary>The research lost to arguing: a share per doomer and accelerationist pair, after the cap.</summary>
        public int FrictionPermille;

        /// <summary>What every researcher's output is scaled by on account of traits (1000 is normal).</summary>
        public int ResearchPermille { get { return MathI.Permille + MentorPermille - FrictionPermille; } }
    }

    public static class Traits
    {
        public static TraitReport Report(GameConfig c, LabState lab)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            long training = 0, alignment = 0, catches = 0, mentor = 0, offer = 0;
            int doomers = 0, accels = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffTraitConfig t = cfg.Trait(lab.Staff[i].TraitId);
                if (t == null) continue;
                training += t.TrainingSpeedPermille;
                alignment += t.AlignmentWorkPermille;
                catches += t.IncidentCatchPermille;
                mentor += t.MentorPermille;
                offer += t.OfferPermille;
                if (t.Camp == "doom") doomers++;
                else if (t.Camp == "accel") accels++;
            }
            var r = new TraitReport();
            int swing = cfg.MaxTraitSwingPermille;
            r.TrainingSpeedPermille = (int)MathI.Clamp(training, -swing, swing);
            r.AlignmentWorkPermille = (int)MathI.Clamp(alignment, -swing, swing);
            r.IncidentCatchPermille = (int)catches;
            r.MentorPermille = (int)MathI.Min(mentor, cfg.MaxMentorPermille);
            r.OfferBoostPermille = (int)MathI.Clamp(offer, 0, cfg.MaxOfferBoostPermille);
            r.Doomers = doomers;
            r.Accelerationists = accels;
            int pairs = doomers < accels ? doomers : accels;
            r.FrictionPermille = (int)MathI.Min((long)pairs * cfg.FrictionPerPairPermille, cfg.MaxFrictionPermille);
            return r;
        }

        /// <summary>The level one person's morale settles at: the lab's resting level moved by their trait, kept on the scale.</summary>
        public static int RestingMorale(GameConfig c, LabState lab, StaffMember person)
        {
            StaffTraitConfig t = c.StaffPeople.Trait(person.TraitId);
            int offset = t == null ? 0 : t.RestMoraleOffset;
            return MathI.Clamp(Workplace.RestingMorale(c, lab) + offset, 0, MathI.Permille);
        }

        /// <summary>
        /// The trait a new hire of the player's lab arrives with, apart from the star roll. It is taken from the person's id, not from a dice
        /// stream, so no saved stream moves. Plan-run labs always hire the plain trait.
        /// </summary>
        public static string RollHireTrait(GameConfig c, LabState lab, int personId, string plain)
        {
            if (lab.Controller != LabController.Player) return plain;
            int roll = (int)((((long)personId + c.StaffPeople.TraitRollSalt) * 2654435761L & 0xFFFFFFFFL) >> 8) % MathI.Permille;
            int edge = 0;
            for (int i = 0; i < c.StaffPeople.Traits.Count; i++)
            {
                StaffTraitConfig t = c.StaffPeople.Traits[i];
                if (t.HirePermille <= 0) continue;
                edge += t.HirePermille;
                if (roll < edge) return t.Id;
            }
            return plain;
        }
    }
}
