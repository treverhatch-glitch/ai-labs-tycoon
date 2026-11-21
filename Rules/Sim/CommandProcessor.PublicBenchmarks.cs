using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SubmitPublicBenchmark(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!BenchmarkLeagueSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Public benchmarks are not open yet.");
            PublicBenchmarksConfig cfg = c.PublicBenchmarks;
            PublicBenchmarkConfig bench = cfg.Benchmark(cmd.Id);
            if (bench == null) return Fail(CommandError.UnknownId, "There is no benchmark \"" + cmd.Id + "\".");
            if (LabQueries.StaffByFunction(c, lab, RoleFunction.Research) < cfg.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "Hire a researcher before entering benchmarks.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (!BenchmarkLeagueSystem.ModelQualifies(c, model, bench))
                return Fail(CommandError.BelowSectorBar, model.Name + " does not meet the bars for " + bench.Name + ".");
            if (BenchmarkLeagueSystem.OpenEntryForBenchmark(lab, bench.Id) != null)
                return Fail(CommandError.InvalidArgument, "This lab already has an open entry in " + bench.Name + ".");
            if (BenchmarkLeagueSystem.EntryFor(lab, bench.Id, model.Id) != null)
                return Fail(CommandError.InvalidArgument, "That model is already entered in " + bench.Name + ".");
            if (bench.EntryFee > 0 && lab.Cash < bench.EntryFee) return NeedCash(lab, bench.EntryFee);

            int vetting = bench.VettingWeeks < 1 ? 1 : bench.VettingWeeks;
            var entry = new PublicBenchmarkEntry
            {
                BenchmarkId = bench.Id,
                ModelId = model.Id,
                Phase = PublicBenchmarkPhase.Vetting,
                SubmittedTick = state.Tick,
                VettingEndsTick = state.Tick + vetting,
            };
            if (bench.EntryFee > 0) Finance.Spend(lab, LedgerLine.Training, bench.EntryFee);
            lab.PublicBenchmarkEntries.Add(entry);
            Events.Add(state, EventKind.PublicBenchmarkSubmitted, lab.Id, model.Id, vetting, bench.Id);
            return CommandResult.Done(model.Id);
        }

        static CommandResult WithdrawPublicBenchmark(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!BenchmarkLeagueSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Public benchmarks are not open yet.");
            PublicBenchmarkEntry entry = BenchmarkLeagueSystem.EntryFor(lab, cmd.Id, cmd.TargetId);
            if (entry == null)
                return Fail(CommandError.InvalidArgument, "There is no open entry for that model in this benchmark.");
            if (entry.Phase != PublicBenchmarkPhase.Vetting && entry.Phase != PublicBenchmarkPhase.Live)
                return Fail(CommandError.InvalidArgument, "That entry is already closed.");
            entry.Phase = PublicBenchmarkPhase.Withdrawn;
            Events.Add(state, EventKind.PublicBenchmarkWithdrawn, lab.Id, entry.ModelId, 0, entry.BenchmarkId);
            return CommandResult.Done(entry.ModelId);
        }

        static CommandResult PledgeBenchmarkDefense(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!BenchmarkLeagueSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Public benchmarks are not open yet.");
            PublicBenchmarkEntry entry = BenchmarkLeagueSystem.EntryFor(lab, cmd.Id, cmd.TargetId);
            if (entry == null || entry.Phase != PublicBenchmarkPhase.Live)
                return Fail(CommandError.InvalidArgument, "Only a live benchmark entry can pledge defense.");
            if (entry.DefendPledged)
                return Fail(CommandError.InvalidArgument, "That entry is already pledged.");
            PublicBenchmarkConfig bench = c.PublicBenchmarks.Benchmark(cmd.Id);
            if (bench.PledgeFee > 0 && lab.Cash < bench.PledgeFee) return NeedCash(lab, bench.PledgeFee);
            if (bench.PledgeFee > 0) Finance.Spend(lab, LedgerLine.Training, bench.PledgeFee);
            entry.DefendPledged = true;
            Events.Add(state, EventKind.PublicBenchmarkPledged, lab.Id, entry.ModelId, 0, bench.Id);
            return CommandResult.Done(entry.ModelId);
        }

        static CommandResult RetargetPublicBenchmark(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!BenchmarkLeagueSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Public benchmarks are not open yet.");
            PublicBenchmarkConfig bench = c.PublicBenchmarks.Benchmark(cmd.Id);
            if (bench == null) return Fail(CommandError.UnknownId, "There is no benchmark \"" + cmd.Id + "\".");
            PublicBenchmarkEntry entry = BenchmarkLeagueSystem.EntryFor(lab, bench.Id, cmd.TargetId);
            if (entry == null || (entry.Phase != PublicBenchmarkPhase.Vetting && entry.Phase != PublicBenchmarkPhase.Live))
                return Fail(CommandError.InvalidArgument, "There is no open entry to retarget.");
            ModelState replacement = lab.Model(cmd.Count);
            if (replacement == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.Count + ".");
            if (!BenchmarkLeagueSystem.ModelQualifies(c, replacement, bench))
                return Fail(CommandError.BelowSectorBar, replacement.Name + " does not meet the bars for " + bench.Name + ".");
            if (BenchmarkLeagueSystem.EntryFor(lab, bench.Id, replacement.Id) != null)
                return Fail(CommandError.InvalidArgument, "That replacement model is already in this benchmark.");
            if (bench.RetargetFee > 0 && lab.Cash < bench.RetargetFee) return NeedCash(lab, bench.RetargetFee);
            if (bench.RetargetFee > 0) Finance.Spend(lab, LedgerLine.Training, bench.RetargetFee);

            int vetting = bench.VettingWeeks < 1 ? 1 : bench.VettingWeeks;
            entry.ModelId = replacement.Id;
            entry.Phase = PublicBenchmarkPhase.Vetting;
            entry.SubmittedTick = state.Tick;
            entry.VettingEndsTick = state.Tick + vetting;
            entry.DefendPledged = false;
            entry.LastScore = 0;
            Events.Add(state, EventKind.PublicBenchmarkRetargeted, lab.Id, replacement.Id, cmd.TargetId, bench.Id);
            return CommandResult.Done(replacement.Id);
        }
    }
}