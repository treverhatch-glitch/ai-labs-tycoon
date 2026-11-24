using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The trouble that belongs to an era. A risk class is a kind of incident that opens with an era and bites models on sale in
    /// particular sectors, so what a lab sells decides what can go wrong. Which class an incident was is written on its event
    /// (<see cref="GameEvent.RiskClass"/>); nothing else is stored.
    /// </summary>
    public static class RiskClasses
    {
        /// <summary>True when the class has opened by this tick. A class stays on the books once open.</summary>
        public static bool IsOpen(GameConfig c, RiskClassConfig risk, int tick)
        {
            EraConfig opens = c.Era(risk.Era);
            return opens != null && c.Eras.IndexOf(EraSystem.AtTick(c, tick)) >= c.Eras.IndexOf(opens);
        }

        /// <summary>The classes open at a tick, oldest first.</summary>
        public static List<RiskClassConfig> Open(GameConfig c, int tick)
        {
            var open = new List<RiskClassConfig>();
            for (int i = 0; i < c.Safety.RiskClasses.Count; i++)
                if (IsOpen(c, c.Safety.RiskClasses[i], tick)) open.Add(c.Safety.RiskClasses[i]);
            return open;
        }

        /// <summary>The classes that open in this very era: what the era banner announces.</summary>
        public static List<RiskClassConfig> OpeningIn(GameConfig c, EraConfig era)
        {
            var list = new List<RiskClassConfig>();
            for (int i = 0; i < c.Safety.RiskClasses.Count; i++)
                if (c.Safety.RiskClasses[i].Era == era.Id) list.Add(c.Safety.RiskClasses[i]);
            return list;
        }

        /// <summary>True when the lab has this model on sale in at least one of the sectors the class bites (any sector, when it names none).</summary>
        public static bool Exposed(GameState state, LabState lab, ModelState model, RiskClassConfig risk)
        {
            if (risk.Sectors.Count == 0) return OnSaleAnywhere(state, lab, model);
            for (int i = 0; i < risk.Sectors.Count; i++)
            {
                SectorState sector = state.Sector(risk.Sectors[i]);
                SectorEntry entry = sector == null ? null : sector.Entry(lab.Id);
                if (entry != null && entry.ModelId == model.Id) return true;
            }
            return false;
        }

        static bool OnSaleAnywhere(GameState state, LabState lab, ModelState model)
        {
            for (int i = 0; i < state.Sectors.Count; i++)
            {
                SectorEntry entry = state.Sectors[i].Entry(lab.Id);
                if (entry != null && entry.ModelId == model.Id) return true;
            }
            return false;
        }

        /// <summary>The open classes that bite this model today.</summary>
        public static List<RiskClassConfig> Biting(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            var biting = new List<RiskClassConfig>();
            for (int i = 0; i < c.Safety.RiskClasses.Count; i++)
            {
                RiskClassConfig risk = c.Safety.RiskClasses[i];
                if (IsOpen(c, risk, state.Tick) && Exposed(state, lab, model, risk)) biting.Add(risk);
            }
            return biting;
        }

        /// <summary>
        /// Draws what kind of trouble an incident is: one of the classes that bite, or null for the era's plain mishap. Draws nothing from
        /// the random stream when no class bites, so a game that never reaches one plays as it always did.
        /// </summary>
        public static RiskClassConfig Draw(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            List<RiskClassConfig> biting = Biting(c, state, lab, model);
            if (biting.Count == 0) return null;
            long total = c.Safety.GenericRiskWeight;
            for (int i = 0; i < biting.Count; i++) total += biting[i].Weight;
            long roll = state.Rng.Safety.Next((int)total);
            if (roll < c.Safety.GenericRiskWeight) return null;
            roll -= c.Safety.GenericRiskWeight;
            for (int i = 0; i < biting.Count; i++)
            {
                if (roll < biting[i].Weight) return biting[i];
                roll -= biting[i].Weight;
            }
            return biting[biting.Count - 1];
        }
    }
}
