using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult FoundationGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can have a foundation.");
            if (!Foundation.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody endows a foundation in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult EndowFoundation(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FoundationGate(c, state, lab);
            if (!gate.Ok) return gate;
            long amount = cmd.Count * 1000L;
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Give something.");
            if (!Foundation.Exists(lab))
            {
                if (cmd.TargetId < 1 || cmd.TargetId > 3) return Fail(CommandError.InvalidArgument, "Pick a cause: 1 safety, 2 education or 3 open science.");
                if (amount < c.Foundation.MinGift) return Fail(CommandError.InvalidArgument, "A foundation starts with at least " + c.Foundation.MinGift + ".");
            }
            if (lab.Cash < amount) return NeedCash(lab, amount);
            Foundation.Endow(c, state, lab, amount, cmd.TargetId);
            return CommandResult.Done(-1);
        }

        static CommandResult SetFoundationFocus(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FoundationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Foundation.Exists(lab)) return Fail(CommandError.InvalidArgument, "There is no foundation.");
            if (cmd.TargetId < 1 || cmd.TargetId > 3 || cmd.TargetId == lab.FoundFocus) return Fail(CommandError.InvalidArgument, "Pick another cause: 1 safety, 2 education or 3 open science.");
            if (state.Tick < lab.FoundSwitch + c.Foundation.LockWeeks) return Fail(CommandError.InvalidArgument, "The foundation has only just chosen its cause.");
            Foundation.Refocus(c, state, lab, cmd.TargetId);
            return CommandResult.Done(-1);
        }

        static CommandResult SetFoundationPayout(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FoundationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Foundation.Exists(lab)) return Fail(CommandError.InvalidArgument, "There is no foundation.");
            if (cmd.Count < 0 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "Giving is level 0 to 3.");
            lab.FoundPayout = cmd.Count;
            return CommandResult.Done(-1);
        }

        static CommandResult WindUpFoundation(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FoundationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Foundation.Exists(lab)) return Fail(CommandError.InvalidArgument, "There is no foundation.");
            Foundation.WindUp(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
