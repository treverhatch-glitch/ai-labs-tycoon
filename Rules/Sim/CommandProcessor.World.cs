using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>The commands of the world map: land, colo space, halls, power deals and moving hardware between places.</summary>
    public static partial class CommandProcessor
    {
        static CommandResult FrozenIn(GameConfig c, LabState lab, string blocId)
        {
            BlocConfig bloc = c.World.Bloc(blocId);
            return Fail(CommandError.PermitFrozen, bloc.Agency + " has frozen " + lab.Name + "'s permits in " + bloc.Name + ".");
        }

        static CommandResult BuyLand(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            SiteConfig site = c.World.Site(cmd.Id);
            if (site == null) return Fail(CommandError.UnknownId, "There is no site \"" + cmd.Id + "\".");
            if (!WorldQueries.SiteOpen(c, site, state.Tick)) return Fail(CommandError.SiteClosed, site.Name + " opens in " + site.OpensYear + ".");
            if (WorldQueries.PlotsTaken(state, site.Id) >= site.Plots) return Fail(CommandError.NoFreePlot, "Every plot at " + site.Name + " has an owner.");
            if (WorldQueries.HoldingAt(lab, site.Id, HoldingKind.Owned) != null) return Fail(CommandError.NoFreePlot, lab.Name + " already owns a plot at " + site.Name + ".");
            if (WorldQueries.IsFrozen(lab, site.BlocId)) return FrozenIn(c, lab, site.BlocId);
            if (lab.Cash < site.LandPrice) return NeedCash(lab, site.LandPrice);

            var holding = NewHolding(state, site.Id, HoldingKind.Owned);
            lab.Holdings.Add(holding);
            Finance.Spend(lab, LedgerLine.Land, site.LandPrice);
            Events.Add(state, EventKind.LandBought, lab.Id, holding.Id, site.LandPrice, site.Id);
            return CommandResult.Done(holding.Id);
        }

        static Holding NewHolding(GameState state, string siteId, HoldingKind kind)
        {
            var holding = new Holding();
            holding.Id = state.TakeId();
            holding.SiteId = siteId;
            holding.Kind = kind;
            holding.HallDoneTick = -1;
            holding.HallStartTick = -1;
            holding.DealId = "";
            holding.DealDoneTick = -1;
            return holding;
        }

        static CommandResult SellLand(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            Holding holding = lab.Holding(cmd.TargetId);
            if (holding == null || holding.Kind != HoldingKind.Owned) return Fail(CommandError.UnknownId, "The lab owns no land with the id " + cmd.TargetId + ".");
            if (LabQueries.OwnedUnits(lab, holding.Id) > 0) return Fail(CommandError.HoldingNotEmpty, "There is still hardware on that land. Sell it or move it first.");
            if (holding.HallDoneTick >= 0 || holding.DealDoneTick >= 0) return Fail(CommandError.BuildRunning, "Something is still being built there.");

            SiteConfig site = c.World.Site(holding.SiteId);
            long refund = MathI.MulPermille(site.LandPrice, c.World.Halls.LandRefundPermille);
            lab.Holdings.Remove(holding);
            Finance.Earn(lab, LedgerLine.Land, refund);
            Events.Add(state, EventKind.LandSold, lab.Id, holding.Id, refund, site.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult LeaseColo(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            SiteConfig site = c.World.Site(cmd.Id);
            if (site == null) return Fail(CommandError.UnknownId, "There is no site \"" + cmd.Id + "\".");
            if (!WorldQueries.SiteOpen(c, site, state.Tick)) return Fail(CommandError.SiteClosed, site.Name + " opens in " + site.OpensYear + ".");
            if (cmd.Count < 0 || cmd.Count > c.World.Colo.MaxKwPerLabPerSite)
                return Fail(CommandError.InvalidArgument, "A lab can lease from 0 to " + c.World.Colo.MaxKwPerLabPerSite + " kW at one site.");

            Holding holding = WorldQueries.HoldingAt(lab, site.Id, HoldingKind.Colo);
            int current = holding == null ? 0 : holding.HallKw;
            if (holding == null && cmd.Count == 0) return Fail(CommandError.InvalidArgument, "The lab leases nothing at " + site.Name + ".");

            if (cmd.Count > current)
            {
                if (WorldQueries.IsFrozen(lab, site.BlocId)) return FrozenIn(c, lab, site.BlocId);
                long used = WorldQueries.SiteGridUsed(state, site.Id) - current;
                if (used + cmd.Count > site.GridKw)
                    return Fail(CommandError.GridFull, "The grid at " + site.Name + " has " + (site.GridKw - used < 0 ? 0 : site.GridKw - used) + " kW left for the lab.");
            }
            if (holding != null)
            {
                int installed = WorldQueries.InstalledKw(c, lab, holding.Id);
                if (installed > cmd.Count) return Fail(CommandError.NotEnoughPower, "The lab's hardware there draws " + installed + " kW, more than " + cmd.Count + " kW.");
            }

            if (holding == null)
            {
                holding = NewHolding(state, site.Id, HoldingKind.Colo);
                lab.Holdings.Add(holding);
            }
            if (cmd.Count == 0)
            {
                lab.Holdings.Remove(holding);
                Events.Add(state, EventKind.ColoEnded, lab.Id, holding.Id, 0, site.Id);
                return CommandResult.Done(-1);
            }
            holding.HallKw = cmd.Count;
            Events.Add(state, EventKind.ColoLeased, lab.Id, holding.Id, cmd.Count, site.Id);
            return CommandResult.Done(holding.Id);
        }

        static CommandResult BuildHall(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            Holding holding = lab.Holding(cmd.TargetId);
            if (holding == null || holding.Kind != HoldingKind.Owned) return Fail(CommandError.UnknownId, "The lab owns no land with the id " + cmd.TargetId + ".");
            HallConfig hall = c.World.Halls;
            if (cmd.Count < hall.MinKw) return Fail(CommandError.TooSmall, "A hall extension must be at least " + hall.MinKw + " kW.");
            if (holding.HallDoneTick >= 0) return Fail(CommandError.BuildRunning, "A hall is already going up on that land.");
            SiteConfig site = c.World.Site(holding.SiteId);
            if (WorldQueries.IsFrozen(lab, site.BlocId)) return FrozenIn(c, lab, site.BlocId);

            // The grid is checked as if the extension were already standing: private power covers its own share first.
            long others = WorldQueries.SiteGridUsed(state, site.Id) - WorldQueries.GridDrawKw(holding);
            int draw = holding.HallKw + cmd.Count - holding.PrivateKw;
            if (others + (draw > 0 ? draw : 0) > site.GridKw)
                return Fail(CommandError.GridFull, "The grid at " + site.Name + " cannot give " + cmd.Count + " kW more.");
            long cost = cmd.Count * hall.CostPerKw;
            if (lab.Cash < cost) return NeedCash(lab, cost);

            int weeks = WorldQueries.BuildWeeks(c, lab, cmd.Count);
            bool voted = state.Rng.World.Chance(WorldQueries.TownVoteChance(c, lab, site));
            if (voted) weeks += hall.TownVoteDelayWeeks;

            holding.HallBuildingKw = cmd.Count;
            holding.HallDoneTick = state.Tick + weeks;
            holding.HallStartTick = state.Tick;
            Finance.Spend(lab, LedgerLine.SiteBuild, cost);
            Events.Add(state, EventKind.HallStarted, lab.Id, holding.Id, cmd.Count, site.Id);
            if (voted) Events.Add(state, EventKind.TownVoteDelayed, lab.Id, holding.Id, hall.TownVoteDelayWeeks, site.Id);
            return CommandResult.Done(holding.Id);
        }

        static CommandResult AddPowerDeal(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            Holding holding = lab.Holding(cmd.TargetId);
            if (holding == null || holding.Kind != HoldingKind.Owned) return Fail(CommandError.UnknownId, "The lab owns no land with the id " + cmd.TargetId + ".");
            PowerDealConfig deal = c.World.PowerDeal(cmd.Id);
            if (deal == null) return Fail(CommandError.UnknownId, "There is no power deal \"" + cmd.Id + "\".");
            if (!WorldQueries.PowerDealOpen(c, deal, state.Tick)) return Fail(CommandError.EraLocked, deal.Name + " is not on offer until " + deal.OpensYear + ".");
            if (!string.IsNullOrEmpty(holding.DealId)) return Fail(CommandError.BuildRunning, "That land already has a power deal.");
            if (lab.Cash < deal.Cost) return NeedCash(lab, deal.Cost);

            holding.DealId = deal.Id;
            holding.DealDoneTick = state.Tick + deal.Weeks;
            Finance.Spend(lab, LedgerLine.SiteBuild, deal.Cost);
            Events.Add(state, EventKind.PowerDealStarted, lab.Id, holding.Id, deal.Cost, deal.Id);
            return CommandResult.Done(holding.Id);
        }

        static CommandResult MoveHardware(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            HardwareBatch batch = lab.Batch(cmd.TargetId);
            if (batch == null) return Fail(CommandError.UnknownId, "The lab has no hardware batch " + cmd.TargetId + ".");
            if (cmd.Count < 1 || cmd.Count > batch.Count) return Fail(CommandError.InvalidArgument, "Move from 1 to " + batch.Count + " units.");
            int to = cmd.X;
            if (to == batch.HoldingId) return Fail(CommandError.InvalidArgument, "The hardware is already there.");
            GpuGenerationConfig gen = c.Compute.Generation(batch.GpuGenId);

            CommandResult place = CheckPlaceForHardware(c, state, lab, to, gen, cmd.Count);
            if (!place.Ok) return place;
            long cost = c.Compute.MoveCostPerUnit * cmd.Count;
            if (lab.Cash < cost) return NeedCash(lab, cost);

            Fleet.AddGpus(state, lab, to, batch.GpuGenId, cmd.Count);
            Fleet.Take(lab, batch, cmd.Count);
            Finance.Spend(lab, LedgerLine.Hardware, cost);
            Events.Add(state, EventKind.HardwareMoved, lab.Id, to, cmd.Count, gen.Id);
            return CommandResult.Done(-1);
        }
    }
}
