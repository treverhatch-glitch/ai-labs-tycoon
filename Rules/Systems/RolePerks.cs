using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// What the later roles do beyond their function (the recruiter wins hiring fees back, and so on). A perk is the number of people in
    /// its role times a share, held under a cap; it is worked out from the staff list each time, so a lab that lets the role go loses it
    /// the same week and nothing is saved.
    /// </summary>
    public static class RolePerks
    {
        /// <summary>A hiring fee taken apart: the role's list price, what the era does to it, and what the lab's recruiters win back. The parts add up to the total.</summary>
        public struct HireFeeBreakdown
        {
            public long ListPrice;
            public long EraShift;
            public long PerkDiscount;

            public long Total { get { return ListPrice + EraShift + PerkDiscount; } }
        }

        /// <summary>The perks a role carries.</summary>
        public static List<RolePerkConfig> Of(GameConfig c, string roleId)
        {
            var list = new List<RolePerkConfig>();
            for (int i = 0; i < c.RolePerks.Count; i++) if (c.RolePerks[i].RoleId == roleId) list.Add(c.RolePerks[i]);
            return list;
        }

        /// <summary>What one perk gives this lab right now, in permille: people in the role times the share, under the cap.</summary>
        public static int Permille(GameConfig c, LabState lab, RolePerkConfig perk)
        {
            long raw = (long)LabQueries.StaffCount(lab, perk.RoleId) * perk.PerStaffPermille;
            return (int)MathI.Min(raw, perk.CapPermille);
        }

        /// <summary>What every perk of one kind gives this lab right now, added up.</summary>
        public static int Permille(GameConfig c, LabState lab, RolePerkKind kind)
        {
            long total = 0;
            for (int i = 0; i < c.RolePerks.Count; i++)
                if (c.RolePerks[i].Kind == kind) total += Permille(c, lab, c.RolePerks[i]);
            return (int)total;
        }

        /// <summary>What the lab's product people and specialists add to a model's appeal in one sector, in permille.</summary>
        public static int SectorAppealPermille(GameConfig c, LabState lab, string sectorId)
        {
            long total = 0;
            for (int i = 0; i < c.RolePerks.Count; i++)
            {
                RolePerkConfig perk = c.RolePerks[i];
                if (perk.Kind != RolePerkKind.SectorAppeal) continue;
                if (perk.Sectors.Count > 0 && !perk.Sectors.Contains(sectorId)) continue;
                total += Permille(c, lab, perk);
            }
            return (int)total + RoomPerks.SectorAppealPermille(c, lab, sectorId);
        }

        /// <summary>The fee to hire one person of a role: the era's fee, less what the lab's recruiters win back.</summary>
        public static long HireFee(GameConfig c, LabState lab, RoleConfig role, int tick)
        {
            long eraFee = EraRules.HireFee(c, role, tick);
            int discount = (int)MathI.Min(Permille(c, lab, RolePerkKind.HireDiscount), MathI.Permille);
            long fee = MathI.MulPermille(eraFee, MathI.Permille - discount);
            return MathI.MulPermille(fee, (int)MathI.Max(1, MathI.Permille + Founding.HireFeePermille(c, lab)));
        }

        /// <summary>A model's weekly incident odds taken apart: before the lab's red-teamers, what they catch, and what is left. All in permille.</summary>
        public struct IncidentOddsBreakdown
        {
            public int BeforeRedTeam;
            public int CaughtByRedTeam;

            public int Total { get { return BeforeRedTeam + CaughtByRedTeam; } }
        }

        /// <summary>The share of incidents the lab's red-teamers catch before they happen, in permille (never more than 900).</summary>
        public static int IncidentCatchPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(Permille(c, lab, RolePerkKind.IncidentCut) + RoomPerks.Permille(c, lab, RoomPerkKind.IncidentCut) + Traits.Report(c, lab).IncidentCatchPermille, 0, 900);
        }

        public static IncidentOddsBreakdown IncidentOdds(GameConfig c, LabState lab, int tick, ModelState model)
        {
            var b = new IncidentOddsBreakdown();
            b.BeforeRedTeam = SafetySystem.IncidentChancePermille(c, tick, model);
            b.CaughtByRedTeam = -(int)MathI.MulPermille(b.BeforeRedTeam, IncidentCatchPermille(c, lab));
            return b;
        }

        public static HireFeeBreakdown HireFeeParts(GameConfig c, LabState lab, RoleConfig role, int tick)
        {
            var b = new HireFeeBreakdown();
            b.ListPrice = role.HireCost;
            b.EraShift = EraRules.HireFee(c, role, tick) - role.HireCost;
            b.PerkDiscount = HireFee(c, lab, role, tick) - EraRules.HireFee(c, role, tick);
            return b;
        }

        public static long HireCost(GameConfig c, LabState lab, RoleConfig role, int count, int tick)
        {
            return HireFee(c, lab, role, tick) * count;
        }
    }
}
