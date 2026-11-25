using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PlatformGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can sign a distribution deal.");
            if (!Platforms.Enabled(c)) return Fail(CommandError.InvalidArgument, "No platform deals in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult SignPlatform(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PlatformGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Platforms.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a deal with a platform.");
            if (cmd.TargetId < 1 || cmd.TargetId > 3) return Fail(CommandError.InvalidArgument, "The platform is 1 phone maker, 2 browser or 3 office suite.");
            if (EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick)) < Platforms.Era(c, cmd.TargetId)) return Fail(CommandError.MissingPrerequisite, "That platform is not looking for a model yet.");
            ModelState best = LabQueries.BestReleased(lab);
            if (best == null || best.Capability < c.Platforms.MinCapability) return Fail(CommandError.MissingPrerequisite, "The platform wants a released model of capability " + c.Platforms.MinCapability + ".");
            if (lab.Reputation < c.Platforms.MinReputation) return Fail(CommandError.MissingPrerequisite, "The platform wants a reputation of " + c.Platforms.MinReputation + ".");
            Platforms.Sign(c, state, lab, cmd.TargetId);
            return CommandResult.Done(-1);
        }

        static CommandResult RenewPlatform(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PlatformGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Platforms.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no deal with a platform.");
            if (!Platforms.InWindow(c, lab)) return Fail(CommandError.InvalidArgument, "The platform will only talk about a renewal in the last " + c.Platforms.WindowWeeks + " weeks.");
            Platforms.Renew(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult WalkFromPlatform(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PlatformGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Platforms.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no deal with a platform.");
            Platforms.End(c, state, lab, 0);
            return CommandResult.Done(-1);
        }
    }
}
