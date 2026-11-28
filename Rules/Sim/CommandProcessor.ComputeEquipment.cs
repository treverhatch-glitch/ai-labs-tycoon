using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult UpgradeComputeEquipment(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player upgrades lab equipment.");
            if (!ComputeEquipment.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no equipment tier path in this game.");
            ComputeEquipmentTierConfig next = ComputeEquipment.Next(c, lab);
            if (next == null) return Fail(CommandError.InvalidArgument, "The lab is already on the top equipment tier.");
            if (!ComputeEquipment.Unlocked(c, state, next)) return Fail(CommandError.EraLocked, "That equipment tier is not open in this era yet.");
            if (lab.Cash < next.UpgradeCost) return NeedCash(lab, next.UpgradeCost);
            ComputeEquipment.Upgrade(c, state, lab);
            return CommandResult.Done(lab.ComputeEquipTier);
        }
    }
}