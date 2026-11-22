using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Alumni network. See <see cref="AlumniConfig"/>. Only the player's lab keeps one.</summary>
    public static class Alumni
    {
        public const int Good = 1;
        public const int LaidOff = 0;
        public const int Quit = -1;

        public static bool Enabled(GameConfig c) { return c.Alumni.Enabled > 0; }

        public static Alumnus Find(LabState lab, int id)
        {
            for (int i = 0; i < lab.Alumni.Count; i++)
                if (lab.Alumni[i].Id == id) return lab.Alumni[i];
            return null;
        }

        /// <summary>The people on the payroll now, so those who are gone afterwards can be found.</summary>
        public static List<StaffMember> Roster(LabState lab) { return new List<StaffMember>(lab.Staff); }

        public static void Record(GameConfig c, GameState state, LabState lab, StaffMember person, int terms)
        {
            Knowledge.OnLeft(c, state, lab, person.Id);
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            var gone = new Alumnus();
            gone.Id = person.Id;
            gone.RoleId = person.RoleId;
            gone.TraitId = person.TraitId;
            gone.Left = state.Tick;
            gone.Terms = terms;
            lab.Alumni.Add(gone);
            while (lab.Alumni.Count > c.Alumni.MaxSize) lab.Alumni.RemoveAt(0);
        }

        /// <summary>Records everyone in the earlier roster who is no longer on the payroll.</summary>
        public static void RecordGone(GameConfig c, GameState state, LabState lab, List<StaffMember> before, int terms)
        {
            for (int i = 0; i < before.Count; i++)
                if (!lab.Staff.Contains(before[i])) Record(c, state, lab, before[i], terms);
        }

        public static int FeePermille(GameConfig c, int terms)
        {
            return terms == Good ? c.Alumni.FeeGoodPermille : terms == LaidOff ? c.Alumni.FeeLaidOffPermille : c.Alumni.FeeQuitPermille;
        }

        public static int MoraleOffset(GameConfig c, int terms)
        {
            return terms == Good ? c.Alumni.MoraleGood : terms == LaidOff ? c.Alumni.MoraleLaidOff : c.Alumni.MoraleQuit;
        }

        public static int PeerOffset(GameConfig c, int terms)
        {
            return terms == Good ? c.Alumni.PeerGood : terms == LaidOff ? c.Alumni.PeerLaidOff : c.Alumni.PeerQuit;
        }

        public static long Fee(GameConfig c, GameState state, LabState lab, Alumnus gone)
        {
            RoleConfig role = c.Role(gone.RoleId);
            return MathI.MulPermille(RolePerks.HireCost(c, lab, role, 1, state.Tick), FeePermille(c, gone.Terms));
        }

        public static void Rehire(GameConfig c, GameState state, LabState lab, Alumnus gone, long fee)
        {
            RoleConfig role = c.Role(gone.RoleId);
            lab.Alumni.Remove(gone);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + PeerOffset(c, gone.Terms), 0, MathI.Permille);
            var person = new StaffMember();
            person.Id = state.TakeId();
            person.RoleId = role.Id;
            person.TraitId = gone.TraitId;
            person.Morale = (int)MathI.Clamp((long)Traits.RestingMorale(c, lab, person) + MoraleOffset(c, gone.Terms), 0, MathI.Permille);
            lab.Staff.Add(person);
            Events.Add(state, EventKind.AlumniRehired, lab.Id, gone.Id, fee, role.Id).Pick = gone.Terms;
        }

        /// <summary>One week: leavers whose time in the pool is up drop out.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Alumni.Count == 0) return;
            for (int i = lab.Alumni.Count - 1; i >= 0; i--)
                if (state.Tick + 1 - lab.Alumni[i].Left >= c.Alumni.ExpireWeeks) lab.Alumni.RemoveAt(i);
        }
    }
}
