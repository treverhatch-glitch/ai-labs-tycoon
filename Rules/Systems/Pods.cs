using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

using System.Collections.Generic;

namespace AiLabs.Rules.Systems
{
    /// <summary>Project pods and their chemistry. See <see cref="PodsConfig"/>. Only the player's lab.</summary>
    public static class Pods
    {
        public static bool Enabled(GameConfig c) { return c.Pods.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.PodIds.Count > 0; }

        public static StaffMember Person(LabState lab, int id)
        {
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].Id == id) return lab.Staff[i];
            return null;
        }

        /// <summary>The chemistry of a group of people with a number of weeks together: the mix, the clashes, the weeks and the morale.</summary>
        public static int Chemistry(GameConfig c, LabState lab, List<int> ids, int weeks)
        {
            var roles = new HashSet<string>();
            int seniors = 0;
            int juniors = 0;
            long morale = 0;
            int people = 0;
            var quirks = new Dictionary<string, int>();
            for (int i = 0; i < ids.Count; i++)
            {
                StaffMember p = Person(lab, ids[i]);
                if (p == null) continue;
                people++;
                roles.Add(p.RoleId);
                if (p.Senior == 1) seniors++; else juniors++;
                morale += p.Morale;
                if (!string.IsNullOrEmpty(p.TraitId))
                {
                    int n;
                    quirks.TryGetValue(p.TraitId, out n);
                    quirks[p.TraitId] = n + 1;
                }
            }
            if (people == 0) return 0;
            long chem = c.Pods.BaseChem;
            chem += (long)MathI.Min(3, roles.Count - 1) * c.Pods.RolePoints;
            if (seniors > 0 && juniors > 0) chem += c.Pods.MixPoints;
            foreach (KeyValuePair<string, int> q in quirks) chem -= (long)(q.Value * (q.Value - 1) / 2) * c.Pods.ClashPoints;
            chem += MathI.Min(c.Pods.TenureCap, (long)weeks * c.Pods.TenureStep);
            chem += (morale / people - 500) * c.Pods.MoraleShare / 1000;
            return (int)MathI.Clamp(chem, 0, 1000);
        }

        /// <summary>The research points a week the pod adds (or takes away): each person's share, scaled by how far the chemistry is from 500.</summary>
        public static long Weekly(GameConfig c, LabState lab)
        {
            return lab.PodIds.Count * (long)c.Pods.PointsPerMember * (lab.PodChem - 500) / 500;
        }

        public static void Form(GameConfig c, GameState state, LabState lab, List<int> ids)
        {
            lab.PodIds = new List<int>(ids);
            lab.PodWeeks = 0;
            lab.PodChem = Chemistry(c, lab, ids, 0);
            lab.PodsFormed++;
            Finance.Spend(lab, LedgerLine.Hiring, ids.Count * (long)c.Pods.KickoffCost);
            Events.Add(state, EventKind.PodFormed, lab.Id, ids.Count, lab.PodChem, "");
        }

        public static void Swap(GameConfig c, GameState state, LabState lab, int outId, int inId)
        {
            lab.PodIds[lab.PodIds.IndexOf(outId)] = inId;
            lab.PodWeeks /= 2;
            lab.PodChem = Chemistry(c, lab, lab.PodIds, lab.PodWeeks);
            lab.PodSwapAfter = state.Tick + c.Pods.SwapCooldown;
            Events.Add(state, EventKind.PodSwapped, lab.Id, inId, lab.PodChem, "");
        }

        public static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            Events.Add(state, EventKind.PodEnded, lab.Id, lab.PodIds.Count, lab.PodPoints, "").Pick = pick;
            lab.PodIds = new List<int>();
            lab.PodWeeks = 0;
            lab.PodChem = 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab)) return;
            lab.PodIds.RemoveAll(id => Person(lab, id) == null);
            if (lab.PodIds.Count < c.Pods.MinSize)
            {
                End(c, state, lab, 0);
                return;
            }
            lab.PodWeeks++;
            lab.PodChem = Chemistry(c, lab, lab.PodIds, lab.PodWeeks);
            long delta = Weekly(c, lab);
            long before = lab.ResearchPoints;
            lab.ResearchPoints = MathI.Max(0, lab.ResearchPoints + delta);
            lab.PodPoints += lab.ResearchPoints - before;
        }
    }
}
