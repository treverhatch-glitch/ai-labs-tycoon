using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A public promise to research a tech in time. See <see cref="TechBetsConfig"/>. Only the player's lab.</summary>
    public static class TechBets
    {
        public static bool Enabled(GameConfig c) { return c.TechBets.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.TbRef > 0; }

        public static void Make(GameConfig c, GameState state, LabState lab, int index)
        {
            lab.TbRef = index + 1;
            lab.TbLeft = c.TechBets.Weeks;
            Events.Add(state, EventKind.TechBetMade, lab.Id, index, 0, c.Research.Techs[index].Id);
        }

        static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            string id = c.Research.Techs[lab.TbRef - 1].Id;
            int rep = 0;
            if (pick == 1)
            {
                lab.TbWins++;
                rep = c.TechBets.WinReputation;
                PressSystem.AddReputation(lab, rep);
                lab.Prestige = (int)MathI.Min(c.Prestige.Max, Prestige.Level(c, lab) + (long)c.TechBets.WinPrestige);
            }
            else
            {
                if (pick == 0) lab.TbLosses++;
                rep = -PressSystem.Damp(c, lab, pick == 0 ? c.TechBets.LossReputation : c.TechBets.WithdrawReputation);
                PressSystem.AddReputation(lab, rep);
            }
            lab.TbRef = 0;
            lab.TbLeft = 0;
            Events.Add(state, EventKind.TechBetEnded, lab.Id, 0, rep, id).Pick = pick;
        }

        public static void Withdraw(GameConfig c, GameState state, LabState lab) { End(c, state, lab, 2); }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab)) return;
            string id = c.Research.Techs[lab.TbRef - 1].Id;
            if (LabQueries.HasTech(lab, id))
            {
                End(c, state, lab, 1);
                return;
            }
            lab.TbLeft--;
            if (lab.TbLeft <= 0) End(c, state, lab, 0);
        }
    }
}
