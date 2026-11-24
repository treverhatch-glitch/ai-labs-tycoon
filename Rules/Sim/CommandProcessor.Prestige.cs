using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PublishPaper(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab publishes papers here.");
            if (!Prestige.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no papers in this game.");
            if (Prestige.Level(c, lab) >= c.Prestige.Max) return Fail(CommandError.InvalidArgument, "The lab's name in the field cannot get any bigger right now.");
            if (Prestige.Writers(c, lab) < c.Prestige.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "A paper needs at least " + c.Prestige.MinResearchers + " researchers.");
            long cost = Prestige.PaperCost(c, state, lab);
            if (lab.ResearchPoints < cost)
                return Fail(CommandError.NotEnoughResearch, "A paper costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            Prestige.Publish(c, state, lab, cost);
            return CommandResult.Done(-1);
        }
    }
}
