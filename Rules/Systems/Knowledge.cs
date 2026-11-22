using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Owners and documentation of the models. See <see cref="KnowledgeConfig"/>. Only the player's lab.</summary>
    public static class Knowledge
    {
        public static bool Enabled(GameConfig c) { return c.Knowledge.Enabled > 0; }

        public static StaffMember Person(LabState lab, int staffId)
        {
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].Id == staffId) return lab.Staff[i];
            return null;
        }

        public static int Owned(LabState lab, int staffId)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].OwnerId == staffId) n++;
            return n;
        }

        /// <summary>The gap a departing owner would leave this model now.</summary>
        public static int GapLeft(GameConfig c, ModelState model)
        {
            return (int)MathI.Max(0, MathI.MulPermille(c.Knowledge.GapBase, MathI.Permille - model.DocLevel * c.Knowledge.DocCut));
        }

        public static int IncidentPermille(GameConfig c, ModelState model)
        {
            return Enabled(c) ? (int)MathI.MulPermille(c.Knowledge.GapIncident, model.KnowGap) : 0;
        }

        /// <summary>True when nobody dares change the model: the gap is too big.</summary>
        public static bool Untouchable(GameConfig c, ModelState model)
        {
            return Enabled(c) && model.KnowGap > 0 && model.KnowGap >= c.Knowledge.GapRiskAt;
        }

        public static void Own(GameConfig c, GameState state, LabState lab, ModelState model, int staffId)
        {
            model.OwnerId = staffId;
            if (staffId != 0) Events.Add(state, EventKind.OwnerNamed, lab.Id, model.Id, staffId, model.Name);
        }

        public static void WriteDocs(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (c.Knowledge.DocCost > 0) Finance.Spend(lab, LedgerLine.Training, c.Knowledge.DocCost);
            model.DocLevel++;
            model.DocAge = 1;
            Events.Add(state, EventKind.DocsWritten, lab.Id, model.Id, model.DocLevel, model.Name);
        }

        /// <summary>A bankrupt lab has no staff left, so no model has an owner.</summary>
        public static void OnBankrupt(LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++) lab.Models[i].OwnerId = 0;
        }

        public static void OnLeft(GameConfig c, GameState state, LabState lab, int staffId)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.OwnerId != staffId) continue;
                m.OwnerId = 0;
                m.Handovers++;
                m.KnowGap = (int)MathI.Min(MathI.Permille, m.KnowGap + GapLeft(c, m));
                Events.Add(state, EventKind.OwnerLeft, lab.Id, m.Id, m.KnowGap, m.Name);
            }
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.OwnerId != 0 && m.KnowGap > 0) m.KnowGap = (int)MathI.Max(0, m.KnowGap - c.Knowledge.Ramp);
                if (m.DocLevel == 0) continue;
                m.DocAge++;
                if (m.DocAge > c.Knowledge.DocStale)
                {
                    m.DocLevel--;
                    m.DocAge = m.DocLevel == 0 ? 0 : 1;
                    Events.Add(state, EventKind.DocsStale, lab.Id, m.Id, m.DocLevel, m.Name);
                }
            }
        }
    }
}
