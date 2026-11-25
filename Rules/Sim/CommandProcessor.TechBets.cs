using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult TechBetGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab makes public promises.");
            if (!TechBets.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody bets on research in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult BetOnTech(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = TechBetGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (TechBets.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a public bet.");
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            if (LabQueries.HasTech(lab, tech.Id)) return Fail(CommandError.AlreadyResearched, tech.Name + " is already researched.");
            if (!EraSystem.TechOpen(c, tech, state.Tick)) return Fail(CommandError.EraLocked, tech.Name + " is not invented yet.");
            for (int i = 0; i < tech.Requires.Count; i++)
                if (!LabQueries.HasTech(lab, tech.Requires[i])) return Fail(CommandError.MissingPrerequisite, tech.Name + " needs " + c.Tech(tech.Requires[i]).Name + " first.");
            if (tech.Cost < c.TechBets.MinCost) return Fail(CommandError.InvalidArgument, tech.Name + " is too small to promise the world.");
            TechBets.Make(c, state, lab, c.Research.Techs.FindIndex(t => t.Id == tech.Id));
            return CommandResult.Done(-1);
        }

        static CommandResult WithdrawTechBet(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = TechBetGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!TechBets.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no public bet.");
            TechBets.Withdraw(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
