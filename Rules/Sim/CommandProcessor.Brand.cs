using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult BrandGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has an employer brand.");
            if (!Brand.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody scores employers in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OpenCareersPage(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = BrandGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Brand.On(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a careers page.");
            if (lab.Cash < c.Brand.OpenCost) return NeedCash(lab, c.Brand.OpenCost);
            Brand.Open(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult AnswerReview(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = BrandGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Brand.Waiting(lab)) return Fail(CommandError.InvalidArgument, "No review is waiting for an answer.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The answer is 1 own up and fix it, 2 rebut it or 3 leave it.");
            if (cmd.Count == 1 && lab.Cash < Brand.RepairCost(c, lab)) return NeedCash(lab, Brand.RepairCost(c, lab));
            Brand.Answer(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
