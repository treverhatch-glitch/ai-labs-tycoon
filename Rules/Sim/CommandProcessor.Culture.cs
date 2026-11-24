using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetReleaseGate(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Permille < 0 || cmd.Permille > c.Culture.MaxGate)
                return Fail(CommandError.InvalidArgument, "A release gate goes from 0 to " + c.Culture.MaxGate + ".");
            if (SafetyCulture.GateLocked(c, state, lab, cmd.Permille))
            {
                int until = lab.GateSetTick + c.Culture.GateLockWeeks;
                return Fail(CommandError.GateLocked, "You set the gate yourself, and it holds for another " + (until - state.Tick) + " weeks.");
            }
            SafetyCulture.SetGate(c, state, lab, cmd.Permille);
            return CommandResult.Done(-1);
        }
    }
}
