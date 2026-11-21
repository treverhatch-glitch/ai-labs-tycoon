using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Skunkworks. See <see cref="SkunkworksConfig"/>. Only the player's lab runs a project.</summary>
    public static class Skunkworks
    {
        public const string Research = "research";
        public const string Safety = "safety";
        public const string Engineering = "engineering";

        public static bool Enabled(GameConfig c) { return c.Skunkworks.Enabled > 0; }

        public static bool Active(LabState lab) { return !string.IsNullOrEmpty(lab.SkunkKind); }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.SkunkLockedUntil; }

        /// <summary>The function a kind of project takes people from, or null for a kind the game does not have.</summary>
        public static RoleFunction? FunctionOf(string kind)
        {
            switch (kind)
            {
                case Research: return RoleFunction.Research;
                case Safety: return RoleFunction.Safety;
                case Engineering: return RoleFunction.Engineering;
                default: return null;
            }
        }

        public static int KindNumber(string kind) { return kind == Research ? 1 : kind == Safety ? 2 : kind == Engineering ? 3 : 0; }

        /// <summary>The most people a team can take from a function: the share of its people rounded down, never all of them.</summary>
        public static int MaxPeople(GameConfig c, LabState lab, RoleFunction function)
        {
            int heads = LabQueries.StaffByFunction(c, lab, function);
            return (int)MathI.Min(heads - 1, (long)heads * c.Skunkworks.MaxSharePermille / MathI.Permille);
        }

        public static long Cost(GameConfig c, int people, int weeks) { return (long)people * weeks * c.Skunkworks.CostPerPersonWeek; }

        /// <summary>True when the person is on the project: the first people of the project's function, in the order the lab lists them.</summary>
        public static bool Away(GameConfig c, LabState lab, StaffMember person)
        {
            if (!Active(lab)) return false;
            RoleFunction? f = FunctionOf(lab.SkunkKind);
            RoleConfig role = c.Role(person.RoleId);
            if (f == null || role == null || role.Function != f.Value) return false;
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember other = lab.Staff[i];
                RoleConfig otherRole = c.Role(other.RoleId);
                if (otherRole == null || otherRole.Function != f.Value) continue;
                if (other == person) return n < lab.SkunkPeople;
                n++;
            }
            return false;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, string kind, int people, int weeks)
        {
            Finance.Spend(lab, LedgerLine.Training, Cost(c, people, weeks));
            lab.SkunkKind = kind;
            lab.SkunkPeople = people;
            lab.SkunkStart = state.Tick;
            lab.SkunkDue = state.Tick + weeks;
            lab.SkunkWork = 0;
            GameEvent e = Events.Add(state, EventKind.SkunkworksStarted, lab.Id, KindNumber(kind), people, kind);
            e.Pick = weeks;
        }

        static int Multiplier(GameConfig c, LabState lab, bool early)
        {
            if (early) return c.Skunkworks.EarlyPermille;
            return lab.SkunkDue - lab.SkunkStart >= c.Skunkworks.LongWeeks ? MathI.Permille + c.Skunkworks.LongBonusPermille : MathI.Permille;
        }

        /// <summary>Pays out the person-weeks worked: research points, alignment on the lab's finished models or cleared debt. Ends the project and starts the cooldown.</summary>
        public static void Settle(GameConfig c, GameState state, LabState lab, bool early)
        {
            string kind = lab.SkunkKind;
            long work = lab.SkunkWork;
            int m = Multiplier(c, lab, early);
            long reward = 0;
            if (kind == Research)
            {
                long per = MathI.MulPermille(c.Research.PointsPerResearcherWeek, EraSystem.AtTick(c, state.Tick).ResearchPermille);
                reward = MathI.MulPermille(MathI.MulPermille(work * per, c.Skunkworks.ResearchPayoffPermille), m);
                lab.ResearchPoints += reward;
            }
            else if (kind == Safety)
            {
                reward = MathI.Min(c.Skunkworks.MaxAlignment, MathI.MulPermille(work * c.Skunkworks.AlignmentPerHundred, m) / 100);
                for (int i = 0; i < lab.Models.Count; i++)
                {
                    ModelState model = lab.Models[i];
                    if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) continue;
                    model.Alignment = (int)MathI.Min(MathI.Permille, model.Alignment + reward);
                    if (model.DisplayedAlignment < model.Alignment) model.DisplayedAlignment = model.Alignment;
                }
            }
            else if (kind == Engineering)
            {
                long points = MathI.MulPermille(work * c.Skunkworks.DebtPerPersonWeek, m);
                reward = MathI.Min(points, lab.TechDebt);
                lab.TechDebt = (int)(lab.TechDebt - reward);
            }
            GameEvent e = Events.Add(state, EventKind.SkunkworksDone, lab.Id, KindNumber(kind), reward, kind);
            e.Pick = early ? 0 : 1;
            lab.SkunkKind = "";
            lab.SkunkPeople = 0;
            lab.SkunkStart = 0;
            lab.SkunkDue = 0;
            lab.SkunkWork = 0;
            lab.SkunkLockedUntil = state.Tick + c.Skunkworks.LockWeeks;
        }

        /// <summary>One week of it: the people still on the team add their week, and the due week pays out.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return;
            RoleFunction? f = FunctionOf(lab.SkunkKind);
            int heads = f == null ? 0 : LabQueries.StaffByFunction(c, lab, f.Value);
            lab.SkunkWork += MathI.Min(lab.SkunkPeople, heads);
            if (state.Tick + 1 >= lab.SkunkDue) Settle(c, state, lab, false);
        }
    }
}
