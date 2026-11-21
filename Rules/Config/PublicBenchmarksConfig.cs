using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class PublicBenchmarkConfig : ISync, IHasId
    {
        public string IdValue;
        public string Id { get { return IdValue; } }
        public string Name = "";
        public string SectorId = "";
        public int MinCapability;
        public int MinAlignmentPermille;
        public int VettingWeeks;
        public long EntryFee;
        public long RetargetFee;
        public long PledgeFee;
        public long LeaderWeeklyCash;
        public int LeaderWeeklyRep;
        public int LiveWeeklyRep;
        public int DemoteRepPenalty;
        public int DefendDemotePenalty;
        public int PledgeLeaderBonusRep;

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("sectorId", ref SectorId);
            s.Int("minCapability", ref MinCapability);
            s.Int("minAlignmentPermille", ref MinAlignmentPermille);
            s.Int("vettingWeeks", ref VettingWeeks);
            s.Long("entryFee", ref EntryFee);
            s.Long("retargetFee", ref RetargetFee);
            s.Long("pledgeFee", ref PledgeFee);
            s.Long("leaderWeeklyCash", ref LeaderWeeklyCash);
            s.Int("leaderWeeklyRep", ref LeaderWeeklyRep);
            s.Int("liveWeeklyRep", ref LiveWeeklyRep);
            s.Int("demoteRepPenalty", ref DemoteRepPenalty);
            s.Int("defendDemotePenalty", ref DefendDemotePenalty);
            s.Int("pledgeLeaderBonusRep", ref PledgeLeaderBonusRep);
        }
    }

    public sealed class PublicBenchmarksConfig : ISync
    {
        public int OpensYear;
        public int MinResearchers;
        /// <summary>Extra sector appeal permille when this lab holds a live benchmark entry in that sector.</summary>
        public int LiveAppealPermille;
        /// <summary>Extra sector appeal permille when this lab holds the global benchmark crown.</summary>
        public int LeaderAppealPermille;
        public List<PublicBenchmarkConfig> Benchmarks = new List<PublicBenchmarkConfig>();

        public PublicBenchmarkConfig Benchmark(string id)
        {
            for (int i = 0; i < Benchmarks.Count; i++) if (Benchmarks[i].Id == id) return Benchmarks[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("opensYear", ref OpensYear);
            s.Int("minResearchers", ref MinResearchers);
            s.Int("liveAppealPermille", ref LiveAppealPermille);
            s.Int("leaderAppealPermille", ref LeaderAppealPermille);
            s.ObjList("benchmarks", ref Benchmarks);
        }
    }
}