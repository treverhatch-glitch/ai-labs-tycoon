using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult DatasetsGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a data pipeline.");
            if (!Datasets.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps a data pipeline in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult AcquireData(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DatasetsGate(c, state, lab);
            if (!gate.Ok) return gate;
            int kind = cmd.TargetId;
            if (kind < 1 || kind > 3) return Fail(CommandError.InvalidArgument, "Data is bought as scraped (1), licensed (2) or synthetic (3).");
            if (cmd.Count < 1 || cmd.Count > c.Datasets.MaxBuy) return Fail(CommandError.InvalidArgument, "A purchase is from 1 to " + c.Datasets.MaxBuy + " units.");
            if (Datasets.Stock(lab) + cmd.Count > c.Datasets.StockCap) return Fail(CommandError.InvalidArgument, "The lab cannot hold more than " + c.Datasets.StockCap + " units.");
            if (kind == Datasets.Synthetic && !Datasets.HasReleased(lab)) return Fail(CommandError.MissingPrerequisite, "Synthetic data needs a model on sale to write it.");
            long cost = (long)Datasets.Cost(c, kind) * cmd.Count;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Datasets.Acquire(c, state, lab, kind, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CleanData(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DatasetsGate(c, state, lab);
            if (!gate.Ok) return gate;
            int kind = cmd.TargetId;
            if (kind < 1 || kind > 4) return Fail(CommandError.InvalidArgument, "Unknown source of data.");
            if (lab.DataJobLeft > 0) return Fail(CommandError.InvalidArgument, "A cleaning job is already running.");
            if (cmd.Count < 1 || cmd.Count > c.Datasets.CleanMax) return Fail(CommandError.InvalidArgument, "A job is from 1 to " + c.Datasets.CleanMax + " units.");
            if (Datasets.Raw(lab, kind) < cmd.Count) return Fail(CommandError.InvalidArgument, "The lab has only " + Datasets.Raw(lab, kind) + " raw units of that source.");
            int needed = (cmd.Count + c.Datasets.PerEngineer - 1) / c.Datasets.PerEngineer;
            int staff = LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering);
            if (staff < needed) return Fail(CommandError.NotEnoughStaff, "The job needs " + needed + " engineers and the lab has " + staff + ".");
            long cost = (long)c.Datasets.CleanCost * cmd.Count;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Datasets.StartClean(c, lab, kind, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult LicenseScraped(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DatasetsGate(c, state, lab);
            if (!gate.Ok) return gate;
            int held = lab.DataRaw1 + lab.DataClean1;
            if (cmd.Count < 1 || cmd.Count > held) return Fail(CommandError.InvalidArgument, "The lab holds " + held + " scraped units that are not in a job.");
            long cost = (long)c.Datasets.BackfillCost * cmd.Count;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Datasets.Backfill(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
