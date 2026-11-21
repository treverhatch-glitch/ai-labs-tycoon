using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartSkunkworks(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a skunkworks project.");
            if (!Skunkworks.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no skunkworks projects in this game.");
            RoleFunction? function = Skunkworks.FunctionOf(cmd.Id);
            if (function == null) return Fail(CommandError.InvalidArgument, "A project is a research, safety or engineering one.");
            if (cmd.TargetId < c.Skunkworks.MinWeeks || cmd.TargetId > c.Skunkworks.MaxWeeks)
                return Fail(CommandError.InvalidArgument, "A project runs from " + c.Skunkworks.MinWeeks + " to " + c.Skunkworks.MaxWeeks + " weeks.");
            if (Skunkworks.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a project running.");
            if (Skunkworks.Locked(state, lab)) return Fail(CommandError.InvalidArgument, "The last project ended recently; the next can start in week " + lab.SkunkLockedUntil + ".");
            int most = Skunkworks.MaxPeople(c, lab, function.Value);
            if (cmd.Count < c.Skunkworks.MinPeople || cmd.Count > most)
                return Fail(CommandError.InvalidArgument, "A team takes from " + c.Skunkworks.MinPeople + " to " + most + " people of that function, and never all of them.");
            if (cmd.Id == Skunkworks.Safety && !HasFinishedModel(lab)) return Fail(CommandError.InvalidArgument, "A safety project needs a finished model to improve.");
            if (cmd.Id == Skunkworks.Engineering && (!TechDebt.Enabled(c) || lab.TechDebt < 1)) return Fail(CommandError.InvalidArgument, "An engineering project needs technical debt to clear.");
            long cost = Skunkworks.Cost(c, cmd.Count, cmd.TargetId);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Skunkworks.Start(c, state, lab, cmd.Id, cmd.Count, cmd.TargetId);
            return CommandResult.Done(-1);
        }

        static CommandResult WrapUpSkunkworks(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a skunkworks project.");
            if (!Skunkworks.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no project running.");
            if (lab.SkunkWork < 1) return Fail(CommandError.InvalidArgument, "The team has done no work yet.");
            Skunkworks.Settle(c, state, lab, true);
            return CommandResult.Done(-1);
        }

        static bool HasFinishedModel(LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status == ModelStatus.Trained || lab.Models[i].Status == ModelStatus.Released) return true;
            return false;
        }
    }
}
