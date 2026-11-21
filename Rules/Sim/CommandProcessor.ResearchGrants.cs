using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ApplyForGrant(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab applies for research grants.");
            if (!ResearchGrants.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no research grants in this game.");
            if (cmd.Count < 1 || cmd.Count > c.ResearchGrants.MaxTier) return Fail(CommandError.InvalidArgument, "A grant is a tier from 1 to " + c.ResearchGrants.MaxTier + ".");
            if (lab.ResearchGrantStage != ResearchGrants.None) return Fail(CommandError.InvalidArgument, "The lab already has a grant under review or owed.");
            if (ResearchGrants.Locked(state, lab)) return Fail(CommandError.GrantLocked, "The lab applied or settled a grant recently and cannot apply again until week " + lab.ResearchGrantLockedUntil + ".");
            if (Prestige.Writers(c, lab) < c.ResearchGrants.MinResearchers) return Fail(CommandError.NotEnoughStaff, "A grant application needs at least " + c.ResearchGrants.MinResearchers + " researchers to write it.");
            long cost = ResearchGrants.ApplyCost(c, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            ResearchGrants.Apply(c, state, lab, cmd.Count, cost);
            return CommandResult.Done(-1);
        }
    }
}
