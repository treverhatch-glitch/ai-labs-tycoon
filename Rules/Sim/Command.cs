using System.Collections.Generic;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Sim
{
    public enum CommandKind
    {
        BuildRoom, DemolishRoom, BuyLot,
        Hire, Fire,
        SetRentedGpus, BuyGpus, SellGpus, SetTrainingShare,
        ResearchTech, StartTraining, ReleaseModel,
        BuyLand, SellLand, LeaseColo, BuildHall, AddPowerDeal, MoveHardware
    }

    /// <summary>
    /// One order from a lab's boss. The player's screens and the rivals' plans both make these, and
    /// they are the only way anything outside the rules changes the game. A command is plain data,
    /// so a recorded list of them replays a whole game.
    /// </summary>
    public sealed class Command : ISync
    {
        public CommandKind Kind;
        public int LabId;
        /// <summary>A config id: the room type, role, tech, model line, site or power deal the order is about.</summary>
        public string Id = "";
        /// <summary>A room, model, holding or hardware batch already in the game, or -1.</summary>
        public int TargetId = -1;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Count;
        public int Permille;
        /// <summary>Sector ids, for a release.</summary>
        public List<string> Ids = new List<string>();

        public void Sync(Syncer s)
        {
            s.Enum("kind", ref Kind);
            s.Int("labId", ref LabId);
            s.Str("id", ref Id);
            s.Int("targetId", ref TargetId);
            s.Int("x", ref X);
            s.Int("y", ref Y);
            s.Int("width", ref Width);
            s.Int("height", ref Height);
            s.Int("count", ref Count);
            s.Int("permille", ref Permille);
            s.StrList("ids", ref Ids);
        }

        static Command Make(CommandKind kind, int labId)
        {
            var c = new Command();
            c.Kind = kind;
            c.LabId = labId;
            return c;
        }

        public static Command BuildRoom(int labId, string roomTypeId, int x, int y, int width, int height)
        {
            Command c = Make(CommandKind.BuildRoom, labId);
            c.Id = roomTypeId;
            c.X = x;
            c.Y = y;
            c.Width = width;
            c.Height = height;
            return c;
        }

        public static Command DemolishRoom(int labId, int roomId)
        {
            Command c = Make(CommandKind.DemolishRoom, labId);
            c.TargetId = roomId;
            return c;
        }

        /// <summary>Buys the next bigger lot. Rooms keep their place.</summary>
        public static Command BuyLot(int labId) { return Make(CommandKind.BuyLot, labId); }

        public static Command Hire(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.Hire, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        public static Command Fire(int labId, string roleId, int count)
        {
            Command c = Make(CommandKind.Fire, labId);
            c.Id = roleId;
            c.Count = count;
            return c;
        }

        /// <summary>Sets how many GPUs the lab rents, up or down. Rent is paid weekly; there is nothing to pay now.</summary>
        public static Command SetRentedGpus(int labId, int count)
        {
            Command c = Make(CommandKind.SetRentedGpus, labId);
            c.Count = count;
            return c;
        }

        /// <summary>Buys GPUs of the newest generation into the HQ's racks.</summary>
        public static Command BuyGpus(int labId, int count) { return BuyGpus(labId, count, HardwareBatch.Hq); }

        /// <summary>Buys GPUs of the newest generation into a holding at a site, or into the HQ when the holding id is -1.</summary>
        public static Command BuyGpus(int labId, int count, int holdingId)
        {
            Command c = Make(CommandKind.BuyGpus, labId);
            c.Count = count;
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Sells GPUs from the HQ, oldest generation first.</summary>
        public static Command SellGpus(int labId, int count) { return SellGpus(labId, count, HardwareBatch.Hq); }

        public static Command SellGpus(int labId, int count, int holdingId)
        {
            Command c = Make(CommandKind.SellGpus, labId);
            c.Count = count;
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Sets the share of compute that trains models while a run is active. The rest serves customers.</summary>
        public static Command SetTrainingShare(int labId, int permille)
        {
            Command c = Make(CommandKind.SetTrainingShare, labId);
            c.Permille = permille;
            return c;
        }

        public static Command ResearchTech(int labId, string techId)
        {
            Command c = Make(CommandKind.ResearchTech, labId);
            c.Id = techId;
            return c;
        }

        /// <summary>Starts training the next version of a line. The alignment share is the part of the run spent on safety work.</summary>
        public static Command StartTraining(int labId, string lineId, int alignmentSharePermille)
        {
            Command c = Make(CommandKind.StartTraining, labId);
            c.Id = lineId;
            c.Permille = alignmentSharePermille;
            return c;
        }

        /// <summary>Buys a plot of land at a site. The hall comes after.</summary>
        public static Command BuyLand(int labId, string siteId)
        {
            Command c = Make(CommandKind.BuyLand, labId);
            c.Id = siteId;
            return c;
        }

        /// <summary>Sells the land of a holding with no hardware on it. Part of the price comes back; the hall does not.</summary>
        public static Command SellLand(int labId, int holdingId)
        {
            Command c = Make(CommandKind.SellLand, labId);
            c.TargetId = holdingId;
            return c;
        }

        /// <summary>Leases (or changes, or with 0 ends the lease of) space at a site: kilowatts in someone else's hall.</summary>
        public static Command LeaseColo(int labId, string siteId, int kw)
        {
            Command c = Make(CommandKind.LeaseColo, labId);
            c.Id = siteId;
            c.Count = kw;
            return c;
        }

        /// <summary>Starts building a bigger hall on owned land: this many more kilowatts.</summary>
        public static Command BuildHall(int labId, int holdingId, int kw)
        {
            Command c = Make(CommandKind.BuildHall, labId);
            c.TargetId = holdingId;
            c.Count = kw;
            return c;
        }

        /// <summary>Buys private power for a holding.</summary>
        public static Command AddPowerDeal(int labId, int holdingId, string dealId)
        {
            Command c = Make(CommandKind.AddPowerDeal, labId);
            c.TargetId = holdingId;
            c.Id = dealId;
            return c;
        }

        /// <summary>Moves units of a hardware batch to another place: a holding's id, or -1 for the HQ.</summary>
        public static Command MoveHardware(int labId, int batchId, int toHoldingId, int count)
        {
            Command c = Make(CommandKind.MoveHardware, labId);
            c.TargetId = batchId;
            c.X = toHoldingId;
            c.Count = count;
            return c;
        }

        /// <summary>Puts a trained model on sale in the named sectors, replacing the lab's older model there.</summary>
        public static Command ReleaseModel(int labId, int modelId, params string[] sectorIds)
        {
            Command c = Make(CommandKind.ReleaseModel, labId);
            c.TargetId = modelId;
            c.Ids.AddRange(sectorIds);
            return c;
        }
    }

    public enum CommandError
    {
        None,
        UnknownLab, LabBankrupt, UnknownId, InvalidArgument,
        OutOfBounds, Overlap, TooSmall, NoBiggerLot,
        NotEnoughCash, NoSeats, NoGpuSlots, StaffNeedSeats, GpusNeedSlots, NotEnoughStaff, NotEnoughGpus,
        AlreadyResearched, MissingPrerequisite, NotEnoughResearch,
        LineLocked, TooManyRuns, WrongModelStatus, SectorClosed, BelowSectorBar,
        EraLocked,
        SiteClosed, NoFreePlot, GridFull, NotEnoughPower, ExportBlocked, PermitFrozen, BuildRunning, HoldingNotEmpty,
        AllocationExhausted
    }

    public struct CommandResult
    {
        public bool Ok;
        public CommandError Error;
        /// <summary>Why the order was refused, in plain words. Empty when it went through.</summary>
        public string Message;
        /// <summary>The id of the room or model the order made, or -1.</summary>
        public int CreatedId;

        public static CommandResult Done(int createdId)
        {
            var r = new CommandResult();
            r.Ok = true;
            r.Error = CommandError.None;
            r.Message = "";
            r.CreatedId = createdId;
            return r;
        }

        public static CommandResult Fail(CommandError error, string message)
        {
            var r = new CommandResult();
            r.Ok = false;
            r.Error = error;
            r.Message = message;
            r.CreatedId = -1;
            return r;
        }
    }
}
