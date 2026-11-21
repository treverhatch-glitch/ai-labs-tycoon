using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Sector trust and the customer desk. See <see cref="TrustConfig"/>.</summary>
    public static class TrustSystem
    {
        public struct ChanceParts
        {
            public int Base;
            public int ReputationPart;
            public int Clamp;
            public int Total { get { return Base + ReputationPart + Clamp; } }
        }

        /// <summary>A lab's trust in a sector: its record, or the baseline when there is none.</summary>
        public static int Of(GameConfig c, LabState lab, string sectorId)
        {
            SectorTrust t = Record(lab, sectorId);
            return t == null ? c.Trust.BaselineTrust : t.Trust;
        }

        /// <summary>The permille appeal is multiplied by for this lab in this sector.</summary>
        public static int AppealPermille(GameConfig c, LabState lab, string sectorId)
        {
            long moved = (long)(Of(c, lab, sectorId) - c.Trust.BaselineTrust) * c.Trust.AppealSlopePermille / MathI.Permille;
            return (int)MathI.Max(1, MathI.Permille + moved);
        }

        static SectorTrust Record(LabState lab, string sectorId)
        {
            for (int i = 0; i < lab.SectorTrust.Count; i++) if (lab.SectorTrust[i].SectorId == sectorId) return lab.SectorTrust[i];
            return null;
        }

        static bool Sells(GameState state, LabState lab, string sectorId)
        {
            SectorState s = state.Sector(sectorId);
            return s != null && s.Entry(lab.Id) != null;
        }

        /// <summary>Sets a sector's trust directly, without carry. Returns how far it actually moved.</summary>
        static int Move(GameConfig c, LabState lab, string sectorId, int delta)
        {
            SectorTrust t = Record(lab, sectorId);
            int before = t == null ? c.Trust.BaselineTrust : t.Trust;
            int after = (int)MathI.Clamp(before + delta, 0, MathI.Permille);
            if (t == null)
            {
                t = new SectorTrust();
                t.SectorId = sectorId;
                lab.SectorTrust.Add(t);
            }
            t.Trust = after;
            return after - before;
        }

        /// <summary>
        /// Moves a lab's trust in a sector, and (with <paramref name="carry"/>) a share of the same move in each other sector of the same
        /// family where the lab has a model on sale. Returns how far the named sector moved.
        /// </summary>
        public static int Shift(GameConfig c, GameState state, LabState lab, string sectorId, int delta, bool carry)
        {
            int moved = Move(c, lab, sectorId, delta);
            if (!carry) return moved;
            int shared = (int)MathI.MulPermille(delta, c.Trust.CarryPermille);
            if (shared == 0) return moved;
            SectorConfig home = c.Sector(sectorId);
            if (home == null) return moved;
            for (int i = 0; i < c.Market.Sectors.Count; i++)
            {
                SectorConfig other = c.Market.Sectors[i];
                if (other.Id == sectorId || other.Family != home.Family) continue;
                if (Sells(state, lab, other.Id)) Move(c, lab, other.Id, shared);
            }
            return moved;
        }

        public static Complaint Complaint(LabState lab, int complaintId)
        {
            for (int i = 0; i < lab.Complaints.Count; i++) if (lab.Complaints[i].Id == complaintId) return lab.Complaints[i];
            return null;
        }

        /// <summary>What an answer costs now: hiring fees of the era, plus a share of the model's weekly takings in that sector.</summary>
        public static long AnswerCost(GameConfig c, GameState state, LabState lab, Complaint complaint, ComplaintResponseConfig response)
        {
            RoleConfig sales = c.Role("sales");
            long cost = sales == null ? 0 : EraRules.HireFee(c, sales, state.Tick) * response.CostHireFees;
            SectorState s = state.Sector(complaint.SectorId);
            SectorEntry entry = s == null ? null : s.Entry(lab.Id);
            if (entry != null && entry.ModelId == complaint.ModelId) cost += MathI.MulPermille(entry.LastRevenue, response.CostRevenuePermille);
            return cost;
        }

        public static ChanceParts AnswerChance(GameConfig c, LabState lab, ComplaintResponseConfig response)
        {
            var parts = new ChanceParts();
            parts.Base = response.BaseSuccessPermille;
            parts.ReputationPart = (int)(((long)lab.Reputation - 500) * response.ReputationSlopePermille / MathI.Permille);
            int raw = parts.Base + parts.ReputationPart;
            int bounded = (int)MathI.Clamp(raw, c.Trust.MinSuccessPermille, MathI.Permille);
            parts.Clamp = bounded - raw;
            return parts;
        }

        /// <summary>A bracket win earns trust in that sector.</summary>
        public static void OnBracketWin(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (lab.Bankrupt) return;
            Shift(c, state, lab, sectorId, c.Trust.BracketWinPoints, true);
        }

        /// <summary>A tuned win was exposed: customers in that sector stop believing the lab's scores.</summary>
        public static void OnExposed(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            Shift(c, state, lab, sectorId, -c.Trust.ExposedDrop, true);
        }

        /// <summary>An incident costs trust in every sector the model is on sale in. A let-down customer of the player's writes in.</summary>
        public static void OnIncident(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int n = model.Sectors.Count;
            var lost = new int[n];
            for (int i = 0; i < n; i++) lost[i] = -Shift(c, state, lab, model.Sectors[i], -(int)MathI.MulPermille(c.Trust.IncidentDrop, SectorFlavours.TrustPermille(c, state, lab, model.Sectors[i])), true);

            if (lab.Id != state.PlayerLabId) return;
            string worstSector = "";
            int stake = 0;
            long best = -1;
            for (int i = 0; i < n; i++)
            {
                SectorState s = state.Sector(model.Sectors[i]);
                SectorEntry entry = s == null ? null : s.Entry(lab.Id);
                long revenue = entry == null ? 0 : entry.LastRevenue;
                Events.Add(state, EventKind.TrustLost, lab.Id, -1, lost[i], model.Sectors[i]);
                if (lost[i] > 0 && revenue > best)
                {
                    best = revenue;
                    worstSector = model.Sectors[i];
                    stake = lost[i];
                }
            }
            if (stake <= 0 || lab.Bankrupt || c.Trust.MaxOpenComplaints < 1) return;
            if (lab.Complaints.Count >= c.Trust.MaxOpenComplaints) return;

            var complaint = new Complaint();
            complaint.Id = state.TakeId();
            complaint.SectorId = worstSector;
            complaint.ModelId = model.Id;
            complaint.Stake = stake;
            complaint.ExpiresTick = state.Tick + c.Trust.ComplaintWeeks;
            complaint.Ref = model.Name;
            lab.Complaints.Add(complaint);
            Events.Add(state, EventKind.CustomerComplaint, lab.Id, complaint.Id, stake, worstSector + "|" + model.Name);
        }

        /// <summary>Each week: trust grows where the lab sells and drifts back where it does not, and unanswered complaints lapse.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            TrustConfig t = c.Trust;
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                string id = state.Sectors[s].Id;
                if (state.Sectors[s].Entry(lab.Id) != null) Move(c, lab, id, t.GainPerWeek);
            }
            for (int i = lab.SectorTrust.Count - 1; i >= 0; i--)
            {
                SectorTrust rec = lab.SectorTrust[i];
                if (Sells(state, lab, rec.SectorId)) continue;
                if (rec.Trust > t.BaselineTrust) rec.Trust = (int)MathI.Max(t.BaselineTrust, rec.Trust - t.IdleDecayPerWeek);
                else if (rec.Trust < t.BaselineTrust) rec.Trust = (int)MathI.Min(t.BaselineTrust, rec.Trust + t.IdleDecayPerWeek);
                if (rec.Trust == t.BaselineTrust) lab.SectorTrust.RemoveAt(i);
            }
            for (int i = lab.Complaints.Count - 1; i >= 0; i--)
                if (state.Tick >= lab.Complaints[i].ExpiresTick)
                {
                    lab.Complaints.RemoveAt(i);
                    SafetyCulture.OnComplaintLapsed(c, lab);
                }
        }
    }
}
