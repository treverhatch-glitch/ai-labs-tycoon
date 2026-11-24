using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Version naming rot. See <see cref="NamingConfig"/>. Only the player's labs let their names rot.</summary>
    public static class VersionNaming
    {
        public struct Report
        {
            public int Mess;
            public int Suffixes;
            public int ConfusionPermille;
            public long CleanCost;
            public int CleanPoints;
        }

        static bool Rots(LabState lab) { return lab.Controller == LabController.Player; }

        public static int Mess(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.NamingMess, 0, c.Naming.MaxMess);
        }

        /// <summary>How many words are stacked on the end of a model named now.</summary>
        public static int SuffixCount(GameConfig c, LabState lab)
        {
            if (c.Naming.SuffixEveryPoints <= 0) return 0;
            return (int)MathI.Min(c.Naming.Suffixes.Count, Mess(c, lab) / c.Naming.SuffixEveryPoints);
        }

        /// <summary>The share of appeal customers keep, in permille: 1000 for tidy names, less as the mess grows.</summary>
        public static int AppealPermille(GameConfig c, LabState lab)
        {
            if (!Rots(lab)) return MathI.Permille;
            int mess = Mess(c, lab);
            if (c.Naming.MaxMess <= 0) return MathI.Permille;
            return MathI.Permille - (int)((long)mess * c.Naming.ConfusionPermille / c.Naming.MaxMess);
        }

        /// <summary>The name a new version of this line would be given today.</summary>
        public static string NameFor(GameConfig c, LabState lab, ModelLineConfig line, int version)
        {
            string name = lab.FamilyName + " " + line.Name + " " + version;
            if (!Rots(lab)) return name;
            int n = SuffixCount(c, lab);
            for (int i = 0; i < n; i++) name += " " + c.Naming.Suffixes[i];
            return name;
        }

        public static long CleanCost(GameConfig c, int tick)
        {
            RoleConfig sales = c.Role("sales");
            return sales == null ? 0 : EraRules.HireFee(c, sales, tick) * c.Naming.CleanFeeHireFees;
        }

        public static Report Of(GameConfig c, LabState lab, int tick)
        {
            var r = new Report();
            r.Mess = Mess(c, lab);
            r.Suffixes = SuffixCount(c, lab);
            r.ConfusionPermille = MathI.Permille - AppealPermille(c, lab);
            r.CleanCost = CleanCost(c, tick);
            r.CleanPoints = (int)MathI.Min(r.Mess, c.Naming.CleanPoints);
            return r;
        }

        /// <summary>A new training run was named: the next one will be harder to tell apart.</summary>
        public static void OnVersionNamed(GameConfig c, LabState lab)
        {
            if (!Rots(lab)) return;
            lab.NamingMess = (int)MathI.Min(c.Naming.MaxMess, (long)Mess(c, lab) + c.Naming.MessPerVersion);
        }

        public static void Clean(GameConfig c, LabState lab)
        {
            lab.NamingMess = (int)MathI.Max(0, (long)Mess(c, lab) - c.Naming.CleanPoints);
        }
    }
}
