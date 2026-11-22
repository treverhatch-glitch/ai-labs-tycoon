using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The voluntary incident register. See <see cref="DisclosureConfig"/>. Only the player's lab.</summary>
    public static class Disclosure
    {
        public static bool Enabled(GameConfig c) { return c.Disclosure.Enabled > 0; }

        public static bool Joined(GameConfig c, LabState lab) { return Enabled(c) && lab.DiscOn > 0; }

        public static int SafetyStaff(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Safety); }

        public static void Join(GameConfig c, GameState state, LabState lab)
        {
            lab.DiscOn = 1;
            lab.DiscInspect = c.Disclosure.InspectWeeks;
            Events.Add(state, EventKind.RegisterChanged, lab.Id, 0, 0, "").Pick = 1;
        }

        public static void Leave(GameConfig c, GameState state, LabState lab)
        {
            lab.DiscOn = 0;
            lab.DiscInspect = 0;
            Events.Add(state, EventKind.RegisterChanged, lab.Id, 0, 0, "").Pick = 0;
        }

        public static void OnIncident(GameConfig c, LabState lab, ModelState model)
        {
            if (!Joined(c, lab) || lab.Id < 0) return;
            lab.DiscModel = model.Id;
            if (model.CertStage == Conformity.Certified) lab.DiscFindings += c.Disclosure.CertifiedFindings;
            if (lab.DiscOpen >= c.Disclosure.MaxOpen)
            {
                lab.DiscFindings++;
                return;
            }
            lab.DiscOpen++;
            if (lab.DiscOpen == 1) lab.DiscLeft = c.Disclosure.ReportWeeks;
        }

        static void Trust(GameConfig c, GameState state, LabState lab, int amount)
        {
            ModelState m = lab.Model(lab.DiscModel);
            if (m == null) return;
            for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], amount, false);
        }

        static void CloseOne(GameConfig c, GameState state, LabState lab, int depth, int trust)
        {
            lab.DiscOpen--;
            lab.DiscFiled++;
            lab.DiscLeft = lab.DiscOpen > 0 ? c.Disclosure.ReportWeeks : 0;
            Trust(c, state, lab, trust);
            Events.Add(state, EventKind.ReportFiled, lab.Id, lab.DiscModel, trust, "").Pick = depth;
        }

        public static void File(GameConfig c, GameState state, LabState lab, int depth)
        {
            if (depth == 1)
            {
                lab.DiscFindings++;
                CloseOne(c, state, lab, 1, c.Disclosure.TrustGain);
                return;
            }
            int weeks = c.Disclosure.FullWeeks;
            if (lab.OrgForm == Org.Functional && lab.OrgDisrupt == 0 && Seniority.Seniors(c, lab, RoleFunction.Safety) > 0)
                weeks = (int)MathI.Max(1, weeks - c.Disclosure.OrgFasterWeeks);
            lab.DiscPrep = weeks;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Joined(c, lab) || lab.Id != state.PlayerLabId) return;
            if (lab.DiscPrep > 0)
            {
                lab.DiscPrep--;
                if (lab.DiscPrep == 0)
                {
                    lab.DiscFindings = (int)MathI.Max(0, lab.DiscFindings - 1);
                    if (lab.DiscOpen > 0) CloseOne(c, state, lab, 2, c.Disclosure.TrustFull);
                }
            }
            if (lab.DiscOpen > 0)
            {
                lab.DiscLeft--;
                if (lab.DiscLeft <= 0)
                {
                    lab.DiscOpen--;
                    lab.DiscFindings++;
                    lab.DiscLeft = lab.DiscOpen > 0 ? c.Disclosure.ReportWeeks : 0;
                    long fine = (long)c.Disclosure.LateFine * MathI.Min(lab.DiscFindings, 5);
                    if (fine > 0) Finance.Spend(lab, LedgerLine.Fines, fine);
                    Events.Add(state, EventKind.ReportLate, lab.Id, lab.DiscModel, fine, "");
                }
            }
            lab.DiscInspect--;
            if (lab.DiscInspect <= 0)
            {
                lab.DiscInspect = c.Disclosure.InspectWeeks;
                bool fined = lab.DiscFindings >= c.Disclosure.InspectLimit;
                long fine = fined ? (long)c.Disclosure.InspectFine * lab.DiscFindings : 0;
                if (fine > 0) Finance.Spend(lab, LedgerLine.Fines, fine);
                if (fined) lab.DiscFindings /= 2;
                Events.Add(state, EventKind.InspectionHeld, lab.Id, 0, fine, "").Pick = fined ? 0 : 1;
            }
        }
    }
}
