using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetResearchFocus(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab picks a research focus.");
            if (!ResearchFocus.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no research focus in this game.");
            string id = cmd.Id ?? "";
            if (id.Length > 0 && c.Focus.Option(id) == null) return Fail(CommandError.UnknownId, "There is no research focus \"" + id + "\".");
            if (id == (lab.ResearchFocus ?? "")) return Fail(CommandError.InvalidArgument, "The lab already has that focus.");
            if (state.Tick < lab.FocusLockedUntil)
                return Fail(CommandError.FocusLocked, "The lab changed its focus recently and cannot change it before week " + lab.FocusLockedUntil + ".");
            if (lab.Cash < c.Focus.SwitchCost) return NeedCash(lab, c.Focus.SwitchCost);
            ResearchFocus.Set(c, state, lab, id);
            return CommandResult.Done(-1);
        }
    }
}
