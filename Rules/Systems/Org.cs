using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's structure. See <see cref="OrgConfig"/>. Only the player's lab.</summary>
    public static class Org
    {
        public const int Flat = 1;
        public const int Functional = 2;
        public const int Squads = 3;

        public static bool Enabled(GameConfig c) { return c.Org.Enabled > 0; }

        public static bool Active(GameConfig c, LabState lab) { return Enabled(c) && lab.OrgForm > 0; }

        static int Heads(LabState lab) { return lab.Staff.Count; }

        static int Seniors(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].Senior == 1) n++;
            return n;
        }

        static int Roles(LabState lab)
        {
            var seen = new System.Collections.Generic.List<string>();
            for (int i = 0; i < lab.Staff.Count; i++) if (!seen.Contains(lab.Staff[i].RoleId)) seen.Add(lab.Staff[i].RoleId);
            return seen.Count;
        }

        /// <summary>The functions that have people, and how many of them have a senior to lead them.</summary>
        public static void Functions(GameConfig c, LabState lab, out int teams, out int led)
        {
            teams = 0;
            led = 0;
            foreach (RoleFunction f in System.Enum.GetValues(typeof(RoleFunction)))
            {
                if (LabQueries.StaffByFunction(c, lab, f) == 0) continue;
                teams++;
                if (Seniority.Seniors(c, lab, f) > 0) led++;
            }
        }

        public static int Squad(GameConfig c, LabState lab) { return Heads(lab) / c.Org.SquadSize; }

        /// <summary>Weekly loss of the structure in permille, before the cap: coordination, missing leads and a reorganisation that is still settling.</summary>
        public static int Loss(GameConfig c, LabState lab)
        {
            if (!Active(c, lab)) return 0;
            OrgConfig k = c.Org;
            long loss = 0;
            int heads = Heads(lab);
            if (lab.OrgForm == Flat)
            {
                loss += (long)MathI.Max(0, heads - k.FlatCap) * k.FlatStep;
            }
            else if (lab.OrgForm == Functional)
            {
                int teams, led;
                Functions(c, lab, out teams, out led);
                loss += (long)(teams - led) * k.LeadlessLoss;
                loss += (long)MathI.Max(0, heads - k.SpanSize * teams) * k.SpanStep;
            }
            else
            {
                int squads = Squad(c, lab);
                loss += (long)MathI.Max(0, squads - Seniors(lab)) * k.SquadLeadLoss + (long)squads * k.CrossTeam;
            }
            if (lab.OrgDisrupt > 0) loss += k.DisruptLoss;
            return (int)MathI.Min(k.LossCap, loss);
        }

        public static int ResearchBonus(GameConfig c, LabState lab)
        {
            if (!Active(c, lab) || lab.OrgForm != Functional) return 0;
            int teams, led;
            Functions(c, lab, out teams, out led);
            return teams == 0 ? 0 : (int)((long)c.Org.FuncResearchBonus * led / teams);
        }

        public static int TrainingBonus(GameConfig c, LabState lab)
        {
            if (!Active(c, lab) || lab.OrgForm != Squads || Roles(lab) < c.Org.SquadMinRoles) return 0;
            int squads = Squad(c, lab);
            if (squads == 0) return 0;
            int ledSquads = (int)MathI.Min(squads, Seniors(lab));
            return (int)((long)c.Org.SquadTrainingBonus * ledSquads / squads);
        }

        public static int ResearchPermille(GameConfig c, LabState lab) { return Active(c, lab) ? ResearchBonus(c, lab) - Loss(c, lab) : 0; }

        public static int TrainingPermille(GameConfig c, LabState lab) { return Active(c, lab) ? TrainingBonus(c, lab) - Loss(c, lab) : 0; }

        public static void Set(GameConfig c, GameState state, LabState lab, int form)
        {
            bool first = lab.OrgForm == 0;
            lab.OrgForm = form;
            lab.OrgSince = state.Tick;
            lab.OrgDisrupt = c.Org.ReorgWeeks;
            if (!first) lab.OrgReorgs++;
            Events.Add(state, EventKind.OrgChanged, lab.Id, form, c.Org.ReorgWeeks, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(c, lab) || lab.Id != state.PlayerLabId) return;
            if (lab.OrgDisrupt > 0) lab.OrgDisrupt--;
            int loss = Loss(c, lab);
            if (loss * 4 >= c.Org.LossCap && c.Org.LossCap > 0 && state.Tick % 13 == 0)
                Events.Add(state, EventKind.OrgStrained, lab.Id, lab.OrgForm, loss, "");
        }
    }
}
