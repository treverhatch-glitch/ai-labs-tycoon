using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>What the lab's comfort rooms give the people in it, worked out from the rooms each time (nothing is saved).</summary>
    public struct WorkplaceReport
    {
        /// <summary>Morale points the rooms add to the baseline, after the cap.</summary>
        public int MoraleLift;
        /// <summary>Where morale settles and where a new hire starts.</summary>
        public int RestingMorale;
        /// <summary>The share off a rival's chance to make an offer, after the cap.</summary>
        public int PoachDefencePermille;
        /// <summary>The chance, in permille, that a feuding rival's weekly roll ends in an offer today.</summary>
        public int PoachChancePermille;
        /// <summary>The average morale of the people who do research; 0 when there are none.</summary>
        public int ResearchMorale;
        /// <summary>The share of normal research points the researchers add up to at that average morale (1000 is normal).</summary>
        public int ResearchPermille;
    }

    public static class Workplace
    {
        public static int MoraleLift(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(RoomPerks.Permille(c, lab, RoomPerkKind.MoraleLift), c.StaffPeople.MaxMoraleLift);
        }

        public static int RestingMorale(GameConfig c, LabState lab)
        {
            return c.StaffPeople.MoraleBaseline + MoraleLift(c, lab);
        }

        public static int PoachDefencePermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(RoomPerks.Permille(c, lab, RoomPerkKind.PoachDefence), c.StaffPeople.MaxPoachDefencePermille);
        }

        /// <summary>A rival's weekly chance to make an offer once the lab's rooms have taken their share off.</summary>
        public static int PoachChancePermille(GameConfig c, LabState lab)
        {
            return (int)MathI.MulPermille(c.StaffPeople.PoachChancePermille, MathI.Permille - PoachDefencePermille(c, lab) + Traits.Report(c, lab).OfferBoostPermille);
        }

        /// <summary>
        /// How much of a normal week's research one person does at this morale. Happy people do more at the gain slope; unhappy people do
        /// less at the loss slope (zero in the shipped data), never below a tenth.
        /// </summary>
        public static int ResearchPermille(GameConfig c, int morale)
        {
            int away = morale - c.StaffPeople.MoraleBaseline;
            int slope = away >= 0 ? c.StaffPeople.MoraleResearchSlopePermille : c.StaffPeople.MoraleResearchLossSlopePermille;
            long off = (long)away * slope / MathI.Permille;
            return (int)MathI.Max(100, MathI.Permille + off);
        }

        public static WorkplaceReport Report(GameConfig c, LabState lab)
        {
            var r = new WorkplaceReport();
            r.MoraleLift = MoraleLift(c, lab);
            r.RestingMorale = c.StaffPeople.MoraleBaseline + r.MoraleLift;
            r.PoachDefencePermille = PoachDefencePermille(c, lab);
            r.PoachChancePermille = PoachChancePermille(c, lab);
            long morale = 0;
            long share = 0;
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                RoleConfig role = c.Role(lab.Staff[i].RoleId);
                if (role == null || role.Function != RoleFunction.Research) continue;
                morale += lab.Staff[i].Morale;
                share += ResearchPermille(c, lab.Staff[i].Morale);
                n++;
            }
            r.ResearchMorale = n == 0 ? 0 : (int)(morale / n);
            r.ResearchPermille = n == 0 ? MathI.Permille : (int)(share / n);
            return r;
        }
    }
}
