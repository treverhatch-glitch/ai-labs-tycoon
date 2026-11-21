using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Weekly domain leaderboards and market appeal when a lab leads a capability domain.</summary>
    public static class CapabilityDomainSystem
    {
        public static CapabilityDomainLeaderState Leader(GameState state, string domainId)
        {
            if (state.DomainLeaders == null) return null;
            for (int i = 0; i < state.DomainLeaders.Count; i++)
            {
                CapabilityDomainLeaderState row = state.DomainLeaders[i];
                if (row.DomainId == domainId) return row;
            }
            return null;
        }

        public static bool LeadsDomain(GameState state, LabState lab, string domainId)
        {
            CapabilityDomainLeaderState row = Leader(state, domainId);
            return row != null && row.LeaderLabId == lab.Id && row.Footprint > 0;
        }

        public static string DomainForSector(GameConfig c, string sectorId)
        {
            for (int i = 0; i < c.Market.CapabilityDomains.Count; i++)
            {
                CapabilityDomainConfig domain = c.Market.CapabilityDomains[i];
                for (int k = 0; k < domain.SectorIds.Count; k++)
                    if (domain.SectorIds[k] == sectorId) return domain.Id;
            }
            return null;
        }

        /// <summary>Appeal permille when this lab leads the sector's capability domain this week.</summary>
        public static int LeaderAppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (lab.Bankrupt) return 0;
            string domainId = DomainForSector(c, sectorId);
            if (domainId == null) return 0;
            if (!LeadsDomain(state, lab, domainId)) return 0;
            CapabilityDomainConfig domain = c.Market.Domain(domainId);
            return domain == null || domain.LeaderAppealPermille < 1 ? 0 : domain.LeaderAppealPermille;
        }

        /// <summary>Appeal permille for the near runner-up lab in the sector's domain (not the current leader).</summary>
        public static int ChallengerAppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (lab.Bankrupt) return 0;
            string domainId = DomainForSector(c, sectorId);
            if (domainId == null || LeadsDomain(state, lab, domainId)) return 0;
            CapabilityDomainConfig domain = c.Market.Domain(domainId);
            if (domain == null || domain.ChallengerAppealPermille < 1) return 0;
            CapabilityDomainLeaderState leader = Leader(state, domainId);
            if (leader == null || leader.Footprint < 1) return 0;
            long foot = CapabilityDomainQueries.DomainFootprint(c, state, lab, domainId);
            if (foot < MathI.MulPermille(leader.Footprint, domain.ChallengerNearLeaderPermille)) return 0;
            int runnerUpId = -1;
            long bestFoot = 0;
            int bestCap = 0;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState rival = state.Labs[i];
                if (rival.Bankrupt || rival.Id == leader.LeaderLabId) continue;
                long rivalFoot = CapabilityDomainQueries.DomainFootprint(c, state, rival, domainId);
                if (rivalFoot < 1) continue;
                int cap = CapabilityDomainQueries.BestCapability(c, rival, domainId);
                bool better = rivalFoot > bestFoot
                    || (rivalFoot == bestFoot && cap > bestCap)
                    || (rivalFoot == bestFoot && cap == bestCap && (runnerUpId < 0 || rival.Id < runnerUpId));
                if (!better) continue;
                bestFoot = rivalFoot;
                bestCap = cap;
                runnerUpId = rival.Id;
            }
            return runnerUpId == lab.Id ? domain.ChallengerAppealPermille : 0;
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            var previous = new CapabilityDomainLeaderState[state.DomainLeaders.Count];
            for (int i = 0; i < state.DomainLeaders.Count; i++) previous[i] = state.DomainLeaders[i];
            state.DomainLeaders.Clear();
            for (int d = 0; d < c.Market.CapabilityDomains.Count; d++)
            {
                CapabilityDomainConfig domain = c.Market.CapabilityDomains[d];
                if (domain.SectorIds == null || domain.SectorIds.Count == 0) continue;
                int leaderId = -1;
                long bestFoot = 0;
                int bestCap = 0;
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState lab = state.Labs[i];
                    if (lab.Bankrupt) continue;
                    long foot = CapabilityDomainQueries.DomainFootprint(c, state, lab, domain.Id);
                    if (foot < 1) continue;
                    int cap = CapabilityDomainQueries.BestCapability(c, lab, domain.Id);
                    bool better = foot > bestFoot
                        || (foot == bestFoot && cap > bestCap)
                        || (foot == bestFoot && cap == bestCap && (leaderId < 0 || lab.Id < leaderId));
                    if (!better) continue;
                    bestFoot = foot;
                    bestCap = cap;
                    leaderId = lab.Id;
                }
                if (leaderId < 0 || bestFoot < 1) continue;
                int priorLeader = -1;
                for (int p = 0; p < previous.Length; p++)
                    if (previous[p].DomainId == domain.Id) { priorLeader = previous[p].LeaderLabId; break; }
                state.DomainLeaders.Add(new CapabilityDomainLeaderState
                {
                    DomainId = domain.Id,
                    LeaderLabId = leaderId,
                    Footprint = bestFoot,
                    BestCapability = bestCap,
                });
                if (priorLeader != leaderId)
                    Events.Add(state, EventKind.DomainLeaderChanged, leaderId, priorLeader, bestFoot, domain.Id);
            }
            GrantWeeklyStipends(c, state);
        }

        static void GrantWeeklyStipends(GameConfig c, GameState state)
        {
            for (int i = 0; i < state.DomainLeaders.Count; i++)
            {
                CapabilityDomainLeaderState row = state.DomainLeaders[i];
                CapabilityDomainConfig domain = c.Market.Domain(row.DomainId);
                if (domain == null) continue;
                LabState leader = state.Lab(row.LeaderLabId);
                if (leader == null || leader.Bankrupt) continue;
                if (domain.LeaderReputationPerWeek > 0)
                    leader.Reputation = (int)MathI.Clamp(leader.Reputation + domain.LeaderReputationPerWeek, 0, MathI.Permille);
                if (domain.LeaderResearchPerWeek > 0)
                    leader.ResearchPoints += domain.LeaderResearchPerWeek;
                if (domain.ChallengerReputationPerWeek < 1) continue;
                int runnerUp = RunnerUpLabId(c, state, row);
                if (runnerUp < 0) continue;
                LabState challenger = state.Lab(runnerUp);
                if (challenger == null || challenger.Bankrupt) continue;
                challenger.Reputation = (int)MathI.Clamp(challenger.Reputation + domain.ChallengerReputationPerWeek, 0, MathI.Permille);
            }
        }

        static int RunnerUpLabId(GameConfig c, GameState state, CapabilityDomainLeaderState leaderRow)
        {
            CapabilityDomainConfig domain = c.Market.Domain(leaderRow.DomainId);
            if (domain == null) return -1;
            int runnerUpId = -1;
            long bestFoot = 0;
            int bestCap = 0;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState rival = state.Labs[i];
                if (rival.Bankrupt || rival.Id == leaderRow.LeaderLabId) continue;
                long rivalFoot = CapabilityDomainQueries.DomainFootprint(c, state, rival, leaderRow.DomainId);
                if (rivalFoot < 1) continue;
                if (rivalFoot < MathI.MulPermille(leaderRow.Footprint, domain.ChallengerNearLeaderPermille)) continue;
                int cap = CapabilityDomainQueries.BestCapability(c, rival, leaderRow.DomainId);
                bool better = rivalFoot > bestFoot
                    || (rivalFoot == bestFoot && cap > bestCap)
                    || (rivalFoot == bestFoot && cap == bestCap && (runnerUpId < 0 || rival.Id < runnerUpId));
                if (!better) continue;
                bestFoot = rivalFoot;
                bestCap = cap;
                runnerUpId = rival.Id;
            }
            return runnerUpId;
        }
    }
}