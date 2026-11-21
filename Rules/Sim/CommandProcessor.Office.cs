using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnswerOfficeEvent(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!OfficeEvents.IsPending(lab)) return Fail(CommandError.NoOfficeEvent, "Nothing is waiting for an answer.");
            OfficeEventConfig e = c.Office.Event(lab.OfficeEventId);
            if (e == null) return Fail(CommandError.NoOfficeEvent, "Nothing is waiting for an answer.");
            OfficeChoiceConfig choice = e.Choice(cmd.Id);
            if (choice == null) return Fail(CommandError.UnknownId, "\"" + e.Name + "\" has no answer called \"" + cmd.Id + "\".");
            OfficeEvents.Answer(c, state, lab, e, choice);
            return CommandResult.Done(-1);
        }
    }
}
