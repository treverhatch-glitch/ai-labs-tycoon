using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Model end-of-life notice. See <see cref="SunsetConfig"/>. The successor is the one the existing successor design already records:
    /// a model whose <see cref="ModelState.DesignerId"/> is the old model. Only the player's lab gives notice.
    /// </summary>
    public static class Sunset
    {
        public static bool Enabled(GameConfig c) { return c.Sunset.Enabled > 0; }

        public static bool Pending(ModelState model) { return model.SunsetTick > 0; }

        /// <summary>The released model designed with this one's help that is at least as capable: the strongest, then the oldest. Null when there is none.</summary>
        public static ModelState SuccessorOf(LabState lab, ModelState model)
        {
            ModelState best = null;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Id == model.Id || m.DesignerId != model.Id || m.Status != ModelStatus.Released || m.Capability < model.Capability) continue;
                if (best == null || m.Capability > best.Capability) best = m;
            }
            return best;
        }

        /// <summary>What the model earned last week in the sectors it is on sale in.</summary>
        public static long WeeklyRevenue(GameState state, LabState lab, ModelState model)
        {
            long total = 0;
            for (int i = 0; i < model.Sectors.Count; i++)
            {
                SectorEntry entry = Pullout.EntryOf(state, lab, model, model.Sectors[i]);
                if (entry != null) total += MathI.Max(0, entry.LastRevenue);
            }
            return total;
        }

        public static long WeeklySupport(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return MathI.MulPermille(WeeklyRevenue(state, lab, model), c.Sunset.SupportPermille);
        }

        /// <summary>The trust a sector loses when a notice of this many weeks ends: more for a short notice, less when the successor is the old model's own design.</summary>
        public static int TrustDrop(GameConfig c, int weeks, bool assisted)
        {
            SunsetConfig k = c.Sunset;
            long w = MathI.Clamp(weeks, k.MinWeeks, k.MaxWeeks);
            long drop = k.TrustDropLong + (long)(k.TrustDropShort - k.TrustDropLong) * (k.MaxWeeks - w) / (k.MaxWeeks - k.MinWeeks);
            if (assisted) drop = MathI.MulPermille(drop, MathI.Permille - k.AssistedCutPermille);
            return (int)drop;
        }

        public static void Announce(GameConfig c, GameState state, LabState lab, ModelState model, int weeks)
        {
            model.SunsetStart = state.Tick;
            model.SunsetTick = state.Tick + weeks;
            GameEvent e = Events.Add(state, EventKind.SunsetAnnounced, lab.Id, model.Id, weeks, model.Name);
            e.Pick = SuccessorOf(lab, model) != null ? 1 : 0;
        }

        static void Clear(ModelState model)
        {
            model.SunsetStart = 0;
            model.SunsetTick = 0;
        }

        public static void CallOff(GameConfig c, GameState state, LabState lab, ModelState model, bool chosen)
        {
            int loss = chosen ? c.Sunset.CancelReputation : c.Sunset.BrokenReputation;
            Clear(model);
            loss = PressSystem.Damp(c, lab, loss);
            lab.Reputation = (int)MathI.Clamp((long)lab.Reputation - loss, 0, MathI.Permille);
            GameEvent e = Events.Add(state, EventKind.SunsetCalledOff, lab.Id, model.Id, loss, model.Name);
            e.Pick = chosen ? 1 : 0;
        }

        static void Retire(GameConfig c, GameState state, LabState lab, ModelState model, ModelState successor)
        {
            int weeks = model.SunsetTick - model.SunsetStart;
            int drop = TrustDrop(c, weeks, SuccessorDesign.IsAssisted(successor));
            int moved = 0;
            bool dropped = false;
            var sectors = new System.Collections.Generic.List<string>(model.Sectors);
            for (int i = 0; i < sectors.Count; i++)
            {
                SectorEntry entry = Pullout.EntryOf(state, lab, model, sectors[i]);
                if (entry == null) continue;
                SectorConfig sector = c.Sector(sectors[i]);
                if (sector != null && successor.Capability >= sector.MinCapability)
                {
                    entry.ModelId = successor.Id;
                    if (!successor.Sectors.Contains(sectors[i])) successor.Sectors.Add(sectors[i]);
                    TrustSystem.Shift(c, state, lab, sectors[i], -drop, false);
                    moved++;
                }
                else
                {
                    state.Sector(sectors[i]).Entries.Remove(entry);
                    TrustSystem.Shift(c, state, lab, sectors[i], -(int)MathI.Min(MathI.Permille, MathI.MulPermille(drop, c.Sunset.DroppedPermille)), false);
                    dropped = true;
                }
            }
            model.Sectors.Clear();
            Clear(model);
            model.Status = ModelStatus.Retired;
            GameEvent e = Events.Add(state, EventKind.SunsetDone, lab.Id, successor.Id, moved, model.Name);
            e.Pick = dropped ? 0 : 1;
            Events.Add(state, EventKind.ModelRetired, lab.Id, model.Id, 0, model.Name);
        }

        /// <summary>One week: the lab pays to keep each model with a notice going, and a notice that is up is carried out.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (!Pending(model)) continue;
                if (model.Status != ModelStatus.Released || model.Sectors.Count == 0) { Clear(model); continue; }
                long support = WeeklySupport(c, state, lab, model);
                if (support > 0) Finance.Spend(lab, LedgerLine.Salaries, support);
                if (state.Tick + 1 < model.SunsetTick) continue;
                ModelState successor = SuccessorOf(lab, model);
                if (successor == null) CallOff(c, state, lab, model, false);
                else Retire(c, state, lab, model, successor);
            }
        }
    }
}
