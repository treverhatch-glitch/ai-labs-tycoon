using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult EndowChair(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab endows a chair.");
            if (!Chairs.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no endowed chairs in this game.");
            if (lab.Chairs >= c.Chairs.MaxChairs) return Fail(CommandError.InvalidArgument, "The lab already holds " + c.Chairs.MaxChairs + " chairs, the most there are.");
            if (Prestige.Writers(c, lab) < c.Chairs.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "A chair needs a lab to sit in: at least " + c.Chairs.MinResearchers + " researchers.");
            long cost = Chairs.NextCost(c, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Chairs.Endow(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
