using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StaffHumanQueue(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab staffs the human queue.");
            if (!HumanQueue.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no human queue in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.HumanQueue.MinEra) return Fail(CommandError.EraLocked, "The human queue is not used in this era.");
            if (lab.HumanQueue <= 0) return Fail(CommandError.InvalidArgument, "The queue is empty.");
            if (lab.HumanBusy > 0) return Fail(CommandError.InvalidArgument, "The desk is already busy.");
            if (LabQueries.StaffCount(lab, "support") < c.HumanQueue.StaffNeed) return Fail(CommandError.NotEnoughStaff, "Staffing the desk needs " + c.HumanQueue.StaffNeed + " support people.");
            if (cmd.Count < 1 || cmd.Count > lab.HumanQueue) return Fail(CommandError.InvalidArgument, "Clear from 1 to " + lab.HumanQueue + " items.");
            HumanQueue.Staff(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult ClearHumanQueue(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab clears the human queue.");
            if (!HumanQueue.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no human queue in this game.");
            if (lab.HumanQueue <= 0) return Fail(CommandError.InvalidArgument, "The queue is empty.");
            if (cmd.Count < 1 || cmd.Count > lab.HumanQueue) return Fail(CommandError.InvalidArgument, "Clear from 1 to " + lab.HumanQueue + " items.");
            long cost = cmd.Count * (long)c.HumanQueue.ClearCost;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            HumanQueue.PayClear(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}