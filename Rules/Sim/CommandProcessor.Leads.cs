using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult FollowLead(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ResearchLead lead = ResearchLeads.Lead(lab, cmd.TargetId);
            if (lead == null) return Fail(CommandError.LeadGone, "That lead has gone cold.");
            if (lead.Following) return Fail(CommandError.InvalidArgument, "The lab is already looking into that.");
            if (ResearchLeads.AnyFollowed(lab)) return Fail(CommandError.InvalidArgument, "The lab can only look into one lead at a time.");
            long cost = ResearchLeads.FollowCost(c, lab, state.Tick);
            if (lab.ResearchPoints < cost)
                return Fail(CommandError.NotEnoughResearch, "Looking into " + lead.Ref + " takes " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            ResearchLeads.Follow(c, state, lab, lead);
            return CommandResult.Done(-1);
        }
    }
}
