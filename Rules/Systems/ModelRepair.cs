using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Fine-tune and rollback for glitching models. See <see cref="RepairConfig"/>. Nothing is saved beyond what the commands change in the model and the sectors.</summary>
    public static class ModelRepair
    {
        /// <summary>How many weeks an incident leaves this model glitching: the longest of the sectors it is on sale in.</summary>
        public static int GlitchWeeks(GameConfig c, ModelState model)
        {
            int weeks = 0;
            for (int i = 0; i < model.Sectors.Count; i++) weeks = (int)MathI.Max(weeks, SectorFlavours.GlitchWeeks(c, model.Sectors[i]));
            return weeks > 0 ? weeks : c.Safety.GlitchWeeks;
        }

        /// <summary>Weeks of glitching still to run for a model on sale, or 0 when it is not glitching.</summary>
        public static int GlitchWeeksLeft(GameConfig c, ModelState model, int tick)
        {
            if (model.Status != ModelStatus.Released || model.Incidents <= 0) return 0;
            return (int)MathI.Max(0, GlitchWeeks(c, model) - (tick - model.LastIncidentTick));
        }

        public static long FineTuneCost(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return MathI.Max(c.Repair.FineTuneMinCost, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.Repair.FineTuneRevenueWeeks);
        }

        /// <summary>The older version a rollback would put back on sale: the newest retired version of the same line below this one, or null.</summary>
        public static ModelState RollbackTarget(LabState lab, ModelState model)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != model.LineId || m.Status != ModelStatus.Retired || m.Version >= model.Version || m.Capability <= 0) continue;
                if (best == null || m.Version > best.Version) best = m;
            }
            return best;
        }

        /// <summary>Ends the glitch now and adds a little real alignment.</summary>
        public static void FineTune(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.LastIncidentTick = state.Tick - GlitchWeeks(c, model);
            model.Alignment = (int)MathI.Min(MathI.Permille, (long)model.Alignment + c.Repair.FineTuneAlignment);
            model.DisplayedAlignment = (int)MathI.Min(MathI.Permille, (long)model.DisplayedAlignment + c.Repair.FineTuneAlignment);
        }

        /// <summary>
        /// Withdraws the model from every sector it is sold in and puts the older version back in its place. The withdrawn model goes back to
        /// Trained, so it can be sold again once it has been looked at.
        /// </summary>
        public static void RollBack(GameConfig c, GameState state, LabState lab, ModelState model, ModelState older)
        {
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorEntry entry = state.Sectors[s].Entry(lab.Id);
                if (entry == null || entry.ModelId != model.Id) continue;
                entry.ModelId = older.Id;
                if (!older.Sectors.Contains(state.Sectors[s].Id)) older.Sectors.Add(state.Sectors[s].Id);
            }
            model.Sectors.Clear();
            model.Status = ModelStatus.Trained;
            older.Status = ModelStatus.Released;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Repair.RollbackReputationLoss);
        }
    }
}
