using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Long-term support pledges on released models. See <see cref="PledgesConfig"/>. Only the player's lab.</summary>
    public static class Pledges
    {
        public static bool Enabled(GameConfig c) { return c.Pledges.Enabled > 0; }

        public static bool Held(ModelState model) { return model.LtsLeft > 0; }

        public static int Count(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].LtsLeft > 0) n++;
            return n;
        }

        /// <summary>The revenue lift a pledge of this many weeks left gives, in permille: so much for each full 26 weeks left, up to a cap.</summary>
        public static int LiftPermille(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.LtsLeft <= 0) return 0;
            return (int)MathI.Min(c.Pledges.LiftCapPermille, (long)(model.LtsLeft / 26) * c.Pledges.LiftPermilleStep);
        }

        /// <summary>The extra a lab earns this week from the models it has pledged to support.</summary>
        public static long Lift(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return 0;
            long total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                int lift = LiftPermille(c, m);
                if (lift > 0) total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), lift);
            }
            return total;
        }

        /// <summary>The sector of the lab that a pledged model holds, or null when the model named would not replace a pledged one.</summary>
        public static ModelState Blocking(GameConfig c, GameState state, LabState lab, ModelState incoming, string sectorId)
        {
            if (!Enabled(c)) return null;
            SectorState sector = state.Sector(sectorId);
            SectorEntry entry = sector == null ? null : sector.Entry(lab.Id);
            if (entry == null || entry.ModelId == incoming.Id) return null;
            ModelState old = lab.Model(entry.ModelId);
            return old != null && Held(old) ? old : null;
        }

        public static void Pledge(GameConfig c, GameState state, LabState lab, ModelState model, int weeks)
        {
            model.LtsLeft = weeks;
            Events.Add(state, EventKind.SupportPledged, lab.Id, model.Id, weeks, model.Name);
        }

        public static long BreachFee(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return c.Pledges.BreachWeeks * MarketSystem.ModelRevenue(state, lab.Id, model.Id);
        }

        public static void Break(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long fee = BreachFee(c, state, lab, model);
            if (fee > 0) Finance.Spend(lab, LedgerLine.Fines, fee);
            PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Pledges.BreachReputation));
            for (int i = 0; i < model.Sectors.Count; i++)
                TrustSystem.Shift(c, state, lab, model.Sectors[i], -c.Pledges.BreachTrust, false);
            model.LtsLeft = 0;
            lab.LtsBroken++;
            Events.Add(state, EventKind.SupportPledgeEnded, lab.Id, model.Id, fee, model.Name).Pick = 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LtsLeft <= 0) continue;
                if (m.Status != ModelStatus.Released || m.Sectors.Count == 0)
                {
                    Break(c, state, lab, m);
                    continue;
                }
                long support = MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), c.Pledges.SupportPermille);
                if (support > 0) Finance.Spend(lab, LedgerLine.Salaries, support);
                m.LtsLeft--;
                if (m.LtsLeft > 0) continue;
                lab.LtsKept++;
                for (int s = 0; s < m.Sectors.Count; s++)
                    TrustSystem.Shift(c, state, lab, m.Sectors[s], c.Pledges.KeptTrust, false);
                Events.Add(state, EventKind.SupportPledgeEnded, lab.Id, m.Id, 0, m.Name).Pick = 1;
            }
        }
    }
}
