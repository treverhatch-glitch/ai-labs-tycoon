using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult FabricGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab builds a cluster fabric.");
            if (!Fabric.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody builds cluster fabrics in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OrderFabric(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FabricGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Fabric.Building(lab)) return Fail(CommandError.InvalidArgument, "A fabric tier is already being built.");
            int tier = lab.FabTier + 1;
            if (tier > 3) return Fail(CommandError.InvalidArgument, "The lab already has the best fabric.");
            if (!Fabric.Open(c, tier, state.Tick)) return Fail(CommandError.EraLocked, "Tier " + tier + " is not available yet.");
            int gpus = Fabric.OwnedGpus(lab);
            if (gpus < c.Fabric.MinGpus) return Fail(CommandError.NotEnoughGpus, "A fabric needs at least " + c.Fabric.MinGpus + " owned GPUs and the lab has " + gpus + ".");
            long deposit = Fabric.Deposit(c, Fabric.Cost(c, state, lab, tier, gpus));
            if (lab.Cash < deposit) return NeedCash(lab, deposit);
            Fabric.Order(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult CancelFabric(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FabricGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Fabric.Building(lab)) return Fail(CommandError.InvalidArgument, "No fabric tier is being built.");
            Fabric.Cancel(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
