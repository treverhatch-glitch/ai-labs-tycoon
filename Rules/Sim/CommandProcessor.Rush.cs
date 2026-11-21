using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        /// <summary>A first release with some testing skipped. Everything <see cref="ReleaseModel"/> refuses, this refuses too, and changes nothing.</summary>
        static CommandResult RushRelease(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained)
                return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a model that has not been on sale can be rushed out.");
            if (cmd.Count < 1 || cmd.Count > c.Rush.MaxWeeks)
                return Fail(CommandError.InvalidArgument, "Skip from 1 to " + c.Rush.MaxWeeks + " weeks of testing.");
            if (EthicsBoard.BlocksRush(c, lab)) return Fail(CommandError.BoardVeto, "The ethics board will not sit through a rushed launch.");
            CommandResult released = ReleaseModel(c, state, lab, cmd);
            if (!released.Ok) return released;
            model.RushWeeks = cmd.Count;
            SafetyCulture.Move(c, lab, -c.Rush.CulturePointsPerWeek * cmd.Count);
            Events.Add(state, EventKind.ModelRushed, lab.Id, model.Id, cmd.Count, model.Name);
            return released;
        }
    }
}
