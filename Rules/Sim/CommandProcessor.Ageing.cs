using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RefurbishRoom(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoomState room = lab.Room(cmd.TargetId);
            if (room == null) return Fail(CommandError.UnknownId, "There is no room " + cmd.TargetId + ".");
            if (room.EraAge <= 0) return Fail(CommandError.InvalidArgument, "That room is as up to date as the era.");
            long cost = RoomAge.RefurbishCost(c, room);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Construction, cost);
            room.EraAge = 0;
            Events.Add(state, EventKind.RoomRefurbished, lab.Id, room.Id, cost, room.TypeId);
            return CommandResult.Done(room.Id);
        }
    }
}
