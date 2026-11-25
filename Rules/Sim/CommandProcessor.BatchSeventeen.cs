using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult GpuPurchaseBlocked(GameConfig c, GameState state, LabState lab, GpuGenerationConfig gen, int pick, string message)
        {
            GpuTierUnlock.Refuse(c, state, lab, gen, pick);
            return Fail(CommandError.EraLocked, message);
        }
    }
}