using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RunCampaign(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs marketing campaigns.");
            if (!MarketingCampaign.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no marketing campaigns in this game.");
            SectorConfig sector = c.Sector(cmd.Id);
            if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
            if (cmd.Count < 1 || cmd.Count > c.Marketing.MaxLevel) return Fail(CommandError.InvalidArgument, "A campaign budget is from level 1 to " + c.Marketing.MaxLevel + ".");
            if (MarketingCampaign.Running(lab) || MarketingCampaign.Hungover(lab))
                return Fail(CommandError.InvalidArgument, "The last campaign is still running or being forgiven.");
            if (state.Tick < lab.CampaignLockedUntil)
                return Fail(CommandError.CampaignLocked, "The team needs a rest from marketing until week " + lab.CampaignLockedUntil + ".");
            SectorState sectorState = state.Sector(sector.Id);
            if (sectorState == null || sectorState.Entry(lab.Id) == null)
                return Fail(CommandError.NoModelForJob, "The lab sells nothing in " + sector.Name + " to advertise.");
            if (MarketingCampaign.Sales(c, lab) < c.Marketing.MinSales)
                return Fail(CommandError.NotEnoughStaff, "A campaign needs " + c.Marketing.MinSales + " sales people.");
            long cost = MarketingCampaign.Cost(c, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            MarketingCampaign.Start(c, state, lab, sector.Id, cmd.Count, cost);
            return CommandResult.Done(-1);
        }
    }
}
