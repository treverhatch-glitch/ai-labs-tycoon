using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The compliance office. See <see cref="ComplianceConfig"/>. Only the player's lab runs one, and nothing runs until it asks.</summary>
    public static class Compliance
    {
        public static int Level(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.ComplianceLevel, 0, c.Compliance.MaxLevel);
        }

        /// <summary>The highest level the lab's policy staff cover right now.</summary>
        public static int StaffCovers(GameConfig c, LabState lab)
        {
            int per = c.Compliance.StaffPerLevel;
            int covered = per <= 0 ? c.Compliance.MaxLevel : WorldQueries.PolicyStaff(c, lab) / per;
            return (int)MathI.Min(c.Compliance.MaxLevel, covered);
        }

        public static long WeeklyCost(GameConfig c, LabState lab)
        {
            return Level(c, lab) * c.Compliance.WeeklyCostPerLevel;
        }

        /// <summary>The share the office takes off the monthly attention gain, in permille.</summary>
        public static int AttentionReliefPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Level(c, lab) * (long)c.Compliance.AttentionPermillePerLevel);
        }

        /// <summary>The share the office takes off an audit fine, in permille.</summary>
        public static int AuditReliefPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(MathI.Permille, Level(c, lab) * (long)c.Compliance.AuditFinePermillePerLevel);
        }

        /// <summary>The reputation points the office adds to how a hearing goes.</summary>
        public static int HearingPoints(GameConfig c, LabState lab)
        {
            return (int)(Level(c, lab) * (long)c.Compliance.HearingPointsPerLevel);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || Level(c, lab) == 0) return;
            int covered = StaffCovers(c, lab);
            if (covered < Level(c, lab))
            {
                int was = Level(c, lab);
                lab.ComplianceLevel = covered;
                Events.Add(state, EventKind.ComplianceLapsed, lab.Id, covered, was, "");
            }
            Finance.Spend(lab, LedgerLine.Salaries, WeeklyCost(c, lab));
        }
    }
}
