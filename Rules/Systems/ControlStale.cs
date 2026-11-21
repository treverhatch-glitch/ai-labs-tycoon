using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Control that goes stale. See <see cref="ControlConfig"/>. Only the player's labs go stale. Nothing here is saved: it is read off the lab's techs and the date.</summary>
    public static class ControlStale
    {
        public struct Report
        {
            /// <summary>The era the world is in now, as an index into the eras.</summary>
            public int EraIndex;
            /// <summary>The era of the newest safety tech the lab has, or -1 when it has none.</summary>
            public int NewestSafetyEraIndex;
            /// <summary>Id of that tech, or empty.</summary>
            public string NewestSafetyTechId;
            /// <summary>Eras that have begun since then.</summary>
            public int StaleEras;
            /// <summary>Permille of the safety techs' bonus lost.</summary>
            public int LossPermille;
            /// <summary>The bonus the safety techs would give if fresh, and what they give now.</summary>
            public int FreshBonusPermille;
            public int BonusPermille;
        }

        static bool Goes(LabState lab) { return lab.Controller == LabController.Player; }

        /// <summary>The era of the lab's newest safety tech: its index, or -1 with none. The id is the first such tech of that era in the lab's list.</summary>
        static int NewestSafetyEra(GameConfig c, LabState lab, out string techId)
        {
            int newest = -1;
            techId = "";
            for (int i = 0; i < lab.Techs.Count; i++)
            {
                TechConfig tech = c.Tech(lab.Techs[i]);
                if (tech == null || !LabQueries.IsSafetyTech(tech)) continue;
                int era = EraSystem.IndexOf(c, tech.Era);
                if (era > newest) { newest = era; techId = tech.Id; }
            }
            return newest;
        }

        public static int LossFor(GameConfig c, int staleEras)
        {
            long loss = (long)MathI.Max(0, staleEras - c.Control.GraceEras) * c.Control.LossPerEraPermille;
            return (int)MathI.Min(loss, c.Control.MaxLossPermille);
        }

        public static Report Of(GameConfig c, LabState lab, int tick)
        {
            var r = new Report();
            r.EraIndex = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, tick));
            string id;
            r.NewestSafetyEraIndex = NewestSafetyEra(c, lab, out id);
            r.NewestSafetyTechId = id;
            r.FreshBonusPermille = LabQueries.TechBonus(c, lab, TechEffectKind.AlignmentPermille);
            if (!Goes(lab) || r.NewestSafetyEraIndex < 0)
            {
                r.BonusPermille = r.FreshBonusPermille;
                return r;
            }
            r.StaleEras = (int)MathI.Max(0, r.EraIndex - r.NewestSafetyEraIndex);
            r.LossPermille = (int)MathI.MulPermille(LossFor(c, r.StaleEras), MathI.Permille - Interpretability.StaleReliefPermille(c, lab));
            r.BonusPermille = (int)MathI.MulPermille(r.FreshBonusPermille, MathI.Permille - r.LossPermille);
            return r;
        }

        /// <summary>What the lab's safety techs give now, after they have gone stale.</summary>
        public static int AlignmentBonusPermille(GameConfig c, LabState lab, int tick) { return Of(c, lab, tick).BonusPermille; }

        /// <summary>A new era begins: a player lab whose safety techs have fallen further behind hears about it.</summary>
        public static void OnEraBegan(GameConfig c, GameState state)
        {
            for (int l = 0; l < state.Labs.Count; l++)
            {
                LabState lab = state.Labs[l];
                if (!Goes(lab)) continue;
                Report now = Of(c, lab, state.Tick);
                if (now.NewestSafetyEraIndex < 0) continue;
                if (now.LossPermille > LossFor(c, now.StaleEras - 1))
                    Events.Add(state, EventKind.ControlStale, lab.Id, -1, now.LossPermille, now.NewestSafetyTechId);
            }
        }

        /// <summary>The loss in force before a safety tech is added, so the command can say how much it won back.</summary>
        public static int LossNow(GameConfig c, LabState lab, int tick) { return Of(c, lab, tick).LossPermille; }

        /// <summary>A safety tech has just been researched: if it won back lost ground, say so.</summary>
        public static void OnResearched(GameConfig c, GameState state, LabState lab, int lossBefore, TechConfig tech)
        {
            if (!Goes(lab) || !LabQueries.IsSafetyTech(tech) || lossBefore <= 0) return;
            int lossAfter = LossNow(c, lab, state.Tick);
            if (lossAfter < lossBefore)
                Events.Add(state, EventKind.ControlRefreshed, lab.Id, -1, lossBefore - lossAfter, tech.Id);
        }

        /// <summary>People let go from the roles that listen to the lab's concerns cost it culture.</summary>
        public static void OnFired(GameConfig c, LabState lab, string roleId, int count)
        {
            if (!c.Culture.ListeningRoles.Contains(roleId)) return;
            SafetyCulture.Move(c, lab, -count * c.Control.FiredListenerPoints);
        }
    }
}
