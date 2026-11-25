using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ContractGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab is offered contracts.");
            if (!GovContracts.Enabled(c)) return Fail(CommandError.InvalidArgument, "No government offers contracts in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult AcceptContract(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ContractGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!GovContracts.Offered(lab)) return Fail(CommandError.InvalidArgument, "There is no contract on offer.");
            string problem = GovContracts.AcceptProblem(c, lab);
            if (problem != null) return Fail(CommandError.MissingPrerequisite, problem);
            GovContracts.Accept(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult DeclineContract(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ContractGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!GovContracts.Offered(lab)) return Fail(CommandError.InvalidArgument, "There is no contract on offer.");
            GovContracts.Decline(c, state, lab, true);
            return CommandResult.Done(-1);
        }

        static CommandResult AbandonContract(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ContractGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!GovContracts.InForce(lab)) return Fail(CommandError.InvalidArgument, "There is no contract in force.");
            GovContracts.Abandon(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
