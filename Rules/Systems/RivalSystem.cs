using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>What the labs run by plans feel about the player and each other: feelings, feuds, board fits and new labs entering the race.</summary>
    public static class RivalSystem
    {
        /// <summary>The first week of every month, before the plans run.</summary>
        public static void RunMonth(GameConfig c, GameState state)
        {
            RunArrivals(c, state);
            FeelingsConfig f = c.Rivalry.Feelings;

            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Controller != LabController.Plan || lab.Bankrupt) continue;

                if (lab.FeelingToPlayer > 0) lab.FeelingToPlayer = (int)MathI.Max(0, lab.FeelingToPlayer - f.DriftPerMonth);
                else if (lab.FeelingToPlayer < 0) lab.FeelingToPlayer = (int)MathI.Min(0, lab.FeelingToPlayer + f.DriftPerMonth);

                Lobby(c, state, lab);
                BoardDrama(c, state, lab);
            }
        }

        /// <summary>A feeling toward the player moves by this much (a rival only: the player's lab has no feelings about itself).</summary>
        public static void Feel(GameConfig c, LabState lab, int delta)
        {
            if (lab.Controller != LabController.Plan || delta == 0) return;
            lab.FeelingToPlayer = MathI.Clamp(lab.FeelingToPlayer + delta, -MathI.Permille, MathI.Permille);
        }

        public static bool IsFeud(GameConfig c, LabState lab) { return lab.FeelingToPlayer < c.Rivalry.Feelings.FeudBelow; }

        public static bool IsFriendly(GameConfig c, LabState lab) { return lab.FeelingToPlayer > c.Rivalry.Feelings.FriendlyAbove; }

        /// <summary>A rival in a feud with the player spends a little of its month on getting the player's home government to look at them.</summary>
        static void Lobby(GameConfig c, GameState state, LabState lab)
        {
            if (!IsFeud(c, lab)) return;
            LabState player = state.Player;
            BlocStanding standing = player.Bloc(player.HomeBlocId);
            if (standing == null || player.Bankrupt) return;
            standing.Attention = MathI.Clamp(standing.Attention + c.Rivalry.Feelings.FeudAttentionPerMonth, 0, MathI.Permille);
            DecisionLog.Add(c, lab, state.Tick, DecisionAction.Lobbied, player.HomeBlocId, c.Rivalry.Feelings.FeudAttentionPerMonth,
                Reason.Of(ReasonCode.FeelingTowardPlayer, lab.FeelingToPlayer));
        }

        /// <summary>The chance of a fit is a month's share of the yearly chance. The plan does nothing for the next few weeks.</summary>
        static void BoardDrama(GameConfig c, GameState state, LabState lab)
        {
            int yearly = c.Rival(lab.RivalId).Plan.DramaPermillePerYear;
            if (yearly <= 0) return;
            if (!state.Rng.Rivals.Chance(yearly / c.Calendar.MonthsPerYear)) return;
            lab.PlanPausedUntilTick = state.Tick + c.Rivalry.BoardDramaWeeks;
            Events.Add(state, EventKind.BoardDrama, lab.Id, -1, c.Rivalry.BoardDramaWeeks, "");
            DecisionLog.Add(c, lab, state.Tick, DecisionAction.Paused, "", c.Rivalry.BoardDramaWeeks, Reason.Of(ReasonCode.BoardDrama, c.Rivalry.BoardDramaWeeks));
        }

        /// <summary>The best model any other lab has on sale, or 0.</summary>
        public static int BestOtherCapability(GameState state, LabState lab)
        {
            int best = 0;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                if (state.Labs[i].Id == lab.Id || state.Labs[i].Bankrupt) continue;
                ModelState m = LabQueries.BestReleased(state.Labs[i]);
                if (m != null && m.Capability > best) best = m.Capability;
            }
            return best;
        }

        /// <summary>Rivals whose year has come join the race at the start of a month.</summary>
        static void RunArrivals(GameConfig c, GameState state)
        {
            int year = GameDate.Year(c.Calendar, state.Tick);
            for (int i = 0; i < state.PendingRivals.Count; i++)
            {
                RivalConfig rival = c.Rival(state.PendingRivals[i]);
                if (rival == null || rival.JoinsYear > year) continue;
                state.PendingRivals.RemoveAt(i);
                i--;
                LabState lab = WorldSetup.AddRival(c, state, rival, EraSystem.AtTick(c, state.Tick), true);
                Events.Add(state, EventKind.RivalJoined, lab.Id, -1, lab.Cash, rival.Id);
            }
        }

        /// <summary>The techs a lab that joins late already has: everything of the eras before the current one, and what those need.</summary>
        public static List<string> CatchUpTechs(GameConfig c, EraConfig era)
        {
            int eraIndex = c.Eras.IndexOf(era);
            var techs = new List<string>();
            for (int i = 0; i < c.Research.Techs.Count; i++)
                if (c.Eras.IndexOf(c.Era(c.Research.Techs[i].Era)) < eraIndex) techs.Add(c.Research.Techs[i].Id);
            bool grew = true;
            while (grew)
            {
                grew = false;
                for (int i = 0; i < techs.Count; i++)
                {
                    TechConfig tech = c.Tech(techs[i]);
                    for (int k = 0; k < tech.Requires.Count; k++)
                        if (!techs.Contains(tech.Requires[k])) { techs.Add(tech.Requires[k]); grew = true; }
                }
            }
            return techs;
        }
    }
}
