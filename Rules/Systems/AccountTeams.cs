using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Sector account teams. See <see cref="AccountsConfig"/>. Only the player's lab assigns them.</summary>
    public static class AccountTeams
    {
        public static bool Enabled(GameConfig c) { return c.Accounts.Enabled > 0; }

        public static AccountTeam Team(LabState lab, string sectorId)
        {
            for (int i = 0; i < lab.AccountTeams.Count; i++)
                if (lab.AccountTeams[i].SectorId == sectorId) return lab.AccountTeams[i];
            return null;
        }

        public static int Assigned(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.AccountTeams.Count; i++) n += lab.AccountTeams[i].People;
            return n;
        }

        /// <summary>The salespeople who can be on account teams: the lab's salespeople less those who always stay on the lab-wide force.</summary>
        public static int Allowed(GameConfig c, LabState lab)
        {
            return (int)MathI.Max(0, LabQueries.StaffByFunction(c, lab, RoleFunction.Sales) - c.Accounts.KeepFree);
        }

        /// <summary>True when the person is on a team: the first salespeople, in the order the lab lists them, up to the people assigned.</summary>
        public static bool Away(GameConfig c, LabState lab, StaffMember person)
        {
            if (lab.AccountTeams.Count == 0) return false;
            RoleConfig role = c.Role(person.RoleId);
            if (role == null || role.Function != RoleFunction.Sales) return false;
            int assigned = Assigned(lab);
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember other = lab.Staff[i];
                RoleConfig otherRole = c.Role(other.RoleId);
                if (otherRole == null || otherRole.Function != RoleFunction.Sales) continue;
                if (other == person) return n < assigned;
                n++;
            }
            return false;
        }

        /// <summary>The share of its full effect a team has after this many weeks, in permille.</summary>
        public static int Familiarity(GameConfig c, int weeks)
        {
            long w = MathI.Clamp(weeks, 0, c.Accounts.MaxWeeks);
            return (int)(c.Accounts.StartPermille + (long)(MathI.Permille - c.Accounts.StartPermille) * w / c.Accounts.MaxWeeks);
        }

        /// <summary>What the team's people and familiarity add to the sector's appeal, in permille, before it is combined with anything else.</summary>
        public static int TeamBonus(GameConfig c, AccountTeam team)
        {
            return (int)MathI.MulPermille((long)team.People * c.Accounts.PerPersonPermille, Familiarity(c, team.Weeks));
        }

        /// <summary>
        /// The appeal an account-level deal already adds in the sector, in permille. On this branch there is no such deal, so it is zero. When Unity's enterprise
        /// seat deals (Q208-Q214) are merged, this returns the deal's sector bonus so the two share <see cref="AccountsConfig.ComboCapPermille"/>.
        /// </summary>
        public static int ExternalBonusPermille(GameConfig c, GameState state, LabState lab, string sectorId) { return 0; }

        /// <summary>The team's gain once the other deal is counted: together they never pass the cap, and the other deal is never reduced.</summary>
        public static int Effective(GameConfig c, int teamBonus, int otherBonus)
        {
            long room = MathI.Max(0, c.Accounts.ComboCapPermille - MathI.Max(0, otherBonus));
            return (int)MathI.Min(MathI.Max(0, teamBonus), room);
        }

        public static int AppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (lab.AccountTeams.Count == 0 || !Enabled(c)) return 0;
            AccountTeam team = Team(lab, sectorId);
            if (team == null) return 0;
            return Effective(c, TeamBonus(c, team), ExternalBonusPermille(c, state, lab, sectorId));
        }

        /// <summary>The weeks a team keeps when its size changes: what it built is spread over the larger of the two sizes.</summary>
        public static int DilutedWeeks(AccountTeam team, int people)
        {
            if (team.People < 1 || people < 1) return 0;
            return (int)((long)team.Weeks * MathI.Min(team.People, people) / MathI.Max(team.People, people));
        }

        static GameEvent Note(GameConfig c, GameState state, LabState lab, string sectorId, int weeks, int people, int pick)
        {
            SectorConfig sector = c.Sector(sectorId);
            GameEvent e = Events.Add(state, EventKind.AccountTeamChanged, lab.Id, weeks, people, sector == null ? sectorId : sector.Name);
            e.Pick = pick;
            return e;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, string sectorId, int people)
        {
            AccountTeam team = Team(lab, sectorId);
            if (people == 0)
            {
                lab.AccountTeams.Remove(team);
                Note(c, state, lab, sectorId, 0, 0, 2);
                return;
            }
            bool created = team == null;
            if (created)
            {
                team = new AccountTeam();
                team.SectorId = sectorId;
                lab.AccountTeams.Add(team);
            }
            else team.Weeks = DilutedWeeks(team, people);
            team.People = people;
            Note(c, state, lab, sectorId, team.Weeks, people, created ? 1 : 0);
        }

        /// <summary>One week: teams in sectors the lab has left are disbanded, teams are thinned to the salespeople the lab still has, and the rest learn.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.AccountTeams.Count == 0 || lab.Id != state.PlayerLabId) return;
            for (int i = lab.AccountTeams.Count - 1; i >= 0; i--)
            {
                AccountTeam team = lab.AccountTeams[i];
                SectorState sector = state.Sector(team.SectorId);
                if (sector != null && sector.Entry(lab.Id) != null) continue;
                lab.AccountTeams.RemoveAt(i);
                Note(c, state, lab, team.SectorId, 0, 0, 4);
            }
            int allowed = Allowed(c, lab);
            while (Assigned(lab) > allowed)
            {
                AccountTeam weakest = lab.AccountTeams[0];
                for (int i = 1; i < lab.AccountTeams.Count; i++)
                    if (lab.AccountTeams[i].Weeks < weakest.Weeks) weakest = lab.AccountTeams[i];
                int left = weakest.People - 1;
                if (left == 0) lab.AccountTeams.Remove(weakest);
                else
                {
                    weakest.Weeks = DilutedWeeks(weakest, left);
                    weakest.People = left;
                }
                Note(c, state, lab, weakest.SectorId, left == 0 ? 0 : weakest.Weeks, left, left == 0 ? 2 : 3);
            }
            for (int i = 0; i < lab.AccountTeams.Count; i++)
                lab.AccountTeams[i].Weeks = (int)MathI.Min(c.Accounts.MaxWeeks, lab.AccountTeams[i].Weeks + 1);
        }
    }
}
