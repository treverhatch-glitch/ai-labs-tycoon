using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    partial class CommandProcessor
    {
        static CommandResult SetSectorPrice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (string.IsNullOrEmpty(cmd.Id)) return Fail(CommandError.InvalidArgument, "Name the sector to reprice.");
            SectorConfig sector = c.Sector(cmd.Id);
            if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
            if (!LabQueries.SectorOpen(c, sector, state.Tick))
                return Fail(CommandError.SectorClosed, sector.Name + " opens in " + sector.OpensYear + ".");
            int price = cmd.Permille;
            if (price < c.Market.SectorPriceMinPermille || price > c.Market.SectorPriceMaxPermille)
                return Fail(CommandError.InvalidArgument, "Price must be from " + c.Market.SectorPriceMinPermille + " to " + c.Market.SectorPriceMaxPermille + " permille.");
            SectorState sectorState = state.Sector(sector.Id);
            SectorEntry entry = sectorState.Entry(lab.Id);
            if (entry == null) return Fail(CommandError.InvalidArgument, lab.Name + " has no model on sale in " + sector.Name + ".");
            if (SectorPricePledgeSystem.PledgeActive(entry, state.Tick))
                return Fail(CommandError.InvalidArgument, sector.Name + " has a price pledge in force.");
            entry.PricePermille = price;
            return CommandResult.Done(-1);
        }
    }
}