using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetAutonomy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab gives its agents freedom.");
            if (!Agency.Enabled(c)) return Fail(CommandError.InvalidArgument, "Agents get no freedom in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "No such model.");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            int level = cmd.Count;
            if (level < 0 || level > 3) return Fail(CommandError.InvalidArgument, "The levels are 0 none, 1 supervised, 2 delegated and 3 autonomous.");
            if (level == model.AgentLevel) return CommandResult.Done(-1);
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.Agency.MinEra) return Fail(CommandError.EraLocked, "Agents are not ready in this era.");
            if (level > model.AgentLevel)
            {
                if (model.AgentSince > 0 && state.Tick + 1 < model.AgentSince + c.Agency.Cooldown) return Fail(CommandError.InvalidArgument, "The level was changed too recently.");
                int need = Agency.RoomsNeeded(c, level);
                if (need > 0 && Agency.Wings(lab) < need) return Fail(CommandError.MissingPrerequisite, "Level " + level + " needs " + need + " containment wing(s).");
                if (level == 3 && model.Alignment < c.Agency.MinAlign3) return Fail(CommandError.MissingPrerequisite, model.Name + " is not aligned enough (" + c.Agency.MinAlign3 + ") to run on its own.");
                long cost = (level - model.AgentLevel) * (long)c.Agency.SetupCost;
                if (lab.Cash < cost) return NeedCash(lab, cost);
            }
            Agency.Set(c, state, lab, model, level);
            return CommandResult.Done(-1);
        }
    }
}
