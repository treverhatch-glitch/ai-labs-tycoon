using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q228: player-submitted public benchmark league with vetting, live scoring, and weekly crowns.</summary>
    public static class BenchmarkLeagueSystem
    {
        public static bool Enabled(GameConfig c, GameState state, LabState lab)
        {
            if (lab == null || lab.Id != state.PlayerLabId || c.PublicBenchmarks == null) return false;
            return GameDate.Year(c.Calendar, state.Tick) >= c.PublicBenchmarks.OpensYear;
        }

        public static PublicBenchmarkEntry EntryFor(LabState lab, string benchmarkId, int modelId)
        {
            for (int i = 0; i < lab.PublicBenchmarkEntries.Count; i++)
            {
                PublicBenchmarkEntry e = lab.PublicBenchmarkEntries[i];
                if (e.BenchmarkId != benchmarkId || e.ModelId != modelId) continue;
                if (e.Phase == PublicBenchmarkPhase.Withdrawn) continue;
                return e;
            }
            return null;
        }

        public static PublicBenchmarkEntry OpenEntryForBenchmark(LabState lab, string benchmarkId)
        {
            for (int i = 0; i < lab.PublicBenchmarkEntries.Count; i++)
            {
                PublicBenchmarkEntry e = lab.PublicBenchmarkEntries[i];
                if (e.BenchmarkId != benchmarkId) continue;
                if (e.Phase == PublicBenchmarkPhase.Vetting || e.Phase == PublicBenchmarkPhase.Live) return e;
            }
            return null;
        }

        public static bool ModelInSector(ModelState model, string sectorId)
        {
            for (int i = 0; i < model.Sectors.Count; i++) if (model.Sectors[i] == sectorId) return true;
            return false;
        }

        public static bool ModelQualifies(GameConfig c, ModelState model, PublicBenchmarkConfig bench)
        {
            if (model == null || model.Status != ModelStatus.Released) return false;
            if (!ModelInSector(model, bench.SectorId)) return false;
            if (model.Capability < bench.MinCapability) return false;
            if (model.DisplayedAlignment < bench.MinAlignmentPermille) return false;
            return true;
        }

        public static long Score(GameConfig c, ModelState model, PublicBenchmarkConfig bench)
        {
            SectorConfig sector = c.Sector(bench.SectorId);
            long cap = model.Capability;
            long align = model.DisplayedAlignment;
            if (sector != null)
            {
                cap = MathI.MulPermille(cap, sector.CapabilityWeightPermille);
                align = MathI.MulPermille(align, sector.SafetyWeightPermille);
            }
            return cap + align;
        }

        public static BenchmarkLeaderState Leader(GameState state, string benchmarkId)
        {
            for (int i = 0; i < state.BenchmarkLeaders.Count; i++)
                if (state.BenchmarkLeaders[i].BenchmarkId == benchmarkId) return state.BenchmarkLeaders[i];
            return null;
        }

        static void EnsureLeaders(GameConfig c, GameState state)
        {
            for (int i = 0; i < c.PublicBenchmarks.Benchmarks.Count; i++)
            {
                PublicBenchmarkConfig bench = c.PublicBenchmarks.Benchmarks[i];
                if (Leader(state, bench.Id) != null) continue;
                state.BenchmarkLeaders.Add(new BenchmarkLeaderState { BenchmarkId = bench.Id });
            }
        }

        public static int LiveAppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            PublicBenchmarksConfig cfg = c.PublicBenchmarks;
            if (cfg == null || cfg.LiveAppealPermille <= 0 || lab == null) return 0;
            int bonus = 0;
            for (int i = 0; i < lab.PublicBenchmarkEntries.Count; i++)
            {
                PublicBenchmarkEntry e = lab.PublicBenchmarkEntries[i];
                if (e.Phase != PublicBenchmarkPhase.Live) continue;
                PublicBenchmarkConfig bench = cfg.Benchmark(e.BenchmarkId);
                if (bench == null || bench.SectorId != sectorId) continue;
                bonus += cfg.LiveAppealPermille;
                BenchmarkLeaderState top = Leader(state, e.BenchmarkId);
                if (top != null && top.LabId == lab.Id && top.ModelId == e.ModelId)
                    bonus += cfg.LeaderAppealPermille;
            }
            return bonus;
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            PublicBenchmarksConfig cfg = c.PublicBenchmarks;
            if (cfg == null || cfg.Benchmarks.Count == 0) return;
            EnsureLeaders(c, state);

            for (int li = 0; li < state.Labs.Count; li++)
            {
                LabState lab = state.Labs[li];
                if (lab.Bankrupt) continue;
                AdvanceLabWeek(c, state, lab);
            }

            for (int bi = 0; bi < cfg.Benchmarks.Count; bi++)
                RefreshCrown(c, state, cfg.Benchmarks[bi]);
        }

        static void AdvanceLabWeek(GameConfig c, GameState state, LabState lab)
        {
            for (int i = lab.PublicBenchmarkEntries.Count - 1; i >= 0; i--)
            {
                PublicBenchmarkEntry entry = lab.PublicBenchmarkEntries[i];
                PublicBenchmarkConfig bench = c.PublicBenchmarks.Benchmark(entry.BenchmarkId);
                if (bench == null) { lab.PublicBenchmarkEntries.RemoveAt(i); continue; }
                ModelState model = lab.Model(entry.ModelId);
                if (entry.Phase == PublicBenchmarkPhase.Vetting)
                {
                    if (model == null || !ModelQualifies(c, model, bench))
                    {
                        entry.Phase = PublicBenchmarkPhase.Rejected;
                        Events.Add(state, EventKind.PublicBenchmarkRejected, lab.Id, entry.ModelId, 0, bench.Id);
                        continue;
                    }
                    if (state.Tick >= entry.VettingEndsTick)
                    {
                        entry.Phase = PublicBenchmarkPhase.Live;
                        entry.LastScore = Score(c, model, bench);
                        Events.Add(state, EventKind.PublicBenchmarkLive, lab.Id, entry.ModelId, (int)entry.LastScore, bench.Id);
                    }
                    continue;
                }

                if (entry.Phase != PublicBenchmarkPhase.Live) continue;
                if (model == null || !ModelQualifies(c, model, bench))
                {
                    Demote(c, state, lab, entry, bench);
                    continue;
                }
                entry.LastScore = Score(c, model, bench);
                if (bench.LiveWeeklyRep > 0) lab.Reputation += bench.LiveWeeklyRep;
            }
        }

        static void Demote(GameConfig c, GameState state, LabState lab, PublicBenchmarkEntry entry, PublicBenchmarkConfig bench)
        {
            entry.Phase = PublicBenchmarkPhase.Demoted;
            int penalty = entry.DefendPledged ? bench.DefendDemotePenalty : bench.DemoteRepPenalty;
            if (penalty > 0) lab.Reputation -= penalty;
            Events.Add(state, EventKind.PublicBenchmarkDemoted, lab.Id, entry.ModelId, penalty, bench.Id);
        }

        static void RefreshCrown(GameConfig c, GameState state, PublicBenchmarkConfig bench)
        {
            int bestLab = -1;
            int bestModel = -1;
            long bestScore = -1;
            for (int li = 0; li < state.Labs.Count; li++)
            {
                LabState lab = state.Labs[li];
                if (lab.Bankrupt) continue;
                for (int ei = 0; ei < lab.PublicBenchmarkEntries.Count; ei++)
                {
                    PublicBenchmarkEntry entry = lab.PublicBenchmarkEntries[ei];
                    if (entry.BenchmarkId != bench.Id || entry.Phase != PublicBenchmarkPhase.Live) continue;
                    ModelState model = lab.Model(entry.ModelId);
                    if (model == null) continue;
                    long score = Score(c, model, bench);
                    entry.LastScore = score;
                    if (score <= bestScore) continue;
                    bestScore = score;
                    bestLab = lab.Id;
                    bestModel = entry.ModelId;
                }
            }

            BenchmarkLeaderState leader = Leader(state, bench.Id);
            if (leader == null) return;
            int prevLab = leader.LabId;
            int prevModel = leader.ModelId;
            leader.LabId = bestLab;
            leader.ModelId = bestModel;
            leader.Score = bestScore < 0 ? 0 : bestScore;

            if (bestLab < 0) return;
            LabState winner = state.Lab(bestLab);
            if (winner == null) return;
            PublicBenchmarkEntry winEntry = EntryFor(winner, bench.Id, bestModel);
            if (winEntry == null) return;

            if (bench.LeaderWeeklyCash > 0) Finance.Earn(winner, LedgerLine.Grants, bench.LeaderWeeklyCash);
            int rep = bench.LeaderWeeklyRep;
            if (winEntry.DefendPledged) rep += bench.PledgeLeaderBonusRep;
            if (rep > 0) winner.Reputation += rep;

            if (prevLab != bestLab || prevModel != bestModel)
                Events.Add(state, EventKind.PublicBenchmarkLeader, bestLab, bestModel, (int)bestScore, bench.Id);
        }
    }
}