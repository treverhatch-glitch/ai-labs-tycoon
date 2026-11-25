using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Scrapping a training run. See <see cref="AbandonConfig"/>. Only the player's lab does it.</summary>
    public static class Abandon
    {
        public static bool Enabled(GameConfig c) { return c.Abandon.Enabled > 0; }

        /// <summary>How far along a run is, in permille of the compute it needs.</summary>
        public static int ProgressPermille(ModelState model)
        {
            if (model.RequiredCompute <= 0) return 0;
            return (int)MathI.Clamp(model.ProgressCompute * MathI.Permille / model.RequiredCompute, 0, MathI.Permille);
        }

        /// <summary>The research points the team keeps from the compute already spent.</summary>
        public static long Salvage(GameConfig c, ModelState model)
        {
            return MathI.MulPermille(MathI.Max(0, model.ProgressCompute), c.Abandon.SalvagePermille);
        }

        public static int ReputationLoss(GameConfig c, ModelState model)
        {
            return (int)(c.Abandon.ReputationAtFull * (long)ProgressPermille(model) / MathI.Permille);
        }

        public static int MoraleLoss(GameConfig c, ModelState model)
        {
            return (int)(c.Abandon.MoraleAtFull * (long)ProgressPermille(model) / MathI.Permille);
        }

        public static void Scrap(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long salvage = Salvage(c, model);
            int progress = ProgressPermille(model);
            int rep = ReputationLoss(c, model);
            int morale = MoraleLoss(c, model);
            lab.ResearchPoints += salvage;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - rep);
            if (morale > 0)
                for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - morale);
            lab.Models.Remove(model);
            GameEvent e = Events.Add(state, EventKind.RunScrapped, lab.Id, model.Id, salvage, model.Name);
            e.Pick = progress >= 500 ? 1 : 0;
        }
    }
}
