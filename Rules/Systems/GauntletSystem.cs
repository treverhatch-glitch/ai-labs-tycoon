using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The yearly model tournament. Every sector with models on sale is a bracket, scored on the quality
    /// that sector cares about. The crown goes to the most capable model overall, and the safest lab gets a prize too.
    /// </summary>
    public static class GauntletSystem
    {
        public const string CrownBracket = "crown";

        public static GauntletRecord RunYear(GameConfig c, GameState state)
        {
            var record = new GauntletRecord();
            record.Year = GameDate.Year(c.Calendar, state.Tick);
            record.CrownLabId = -1;
            record.CrownModelId = -1;
            record.SafestLabId = -1;

            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sectorState = state.Sectors[s];
                SectorConfig sector = c.Sector(sectorState.Id);
                var bracket = new List<GauntletEntry>();
                for (int i = 0; i < sectorState.Entries.Count; i++)
                {
                    LabState lab = state.Lab(sectorState.Entries[i].LabId);
                    ModelState model = lab.Model(sectorState.Entries[i].ModelId);
                    bracket.Add(Entry(c, state, sector.Id, lab, model, LabQueries.Quality(c, sector, model, state.Tick)));
                }
                Rank(bracket);
                record.Entries.AddRange(bracket);

                sectorState.ChampionLabId = bracket.Count > 0 ? bracket[0].LabId : -1;
                if (bracket.Count > 0)
                {
                    Events.Add(state, EventKind.GauntletBracket, bracket[0].LabId, bracket[0].ModelId, bracket[0].Score, sector.Id);
                    if (state.PlayerLabId >= 0 && bracket[0].LabId == state.PlayerLabId)
                        for (int i = 1; i < bracket.Count; i++) RivalSystem.Feel(c, state.Lab(bracket[i].LabId), c.Rivalry.Feelings.BracketPermille);
                }
            }

            var crown = new List<GauntletEntry>();
            int safestAlignment = -1;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                ModelState best = LabQueries.BestReleased(lab);
                if (best == null) continue;
                crown.Add(Entry(c, state, CrownBracket, lab, best, best.Capability));
                if (best.Alignment > safestAlignment)
                {
                    safestAlignment = best.Alignment;
                    record.SafestLabId = lab.Id;
                }
            }
            Rank(crown);
            record.Entries.AddRange(crown);

            if (crown.Count > 0)
            {
                record.CrownLabId = crown[0].LabId;
                record.CrownModelId = crown[0].ModelId;
                AddReputation(state.Lab(record.CrownLabId), c.Gauntlet.CrownReputation);
                Events.Add(state, EventKind.GauntletCrown, record.CrownLabId, record.CrownModelId, crown[0].Score, crown[0].ModelName);
                int runnerUp = crown.Count > 1 ? crown[1].LabId : -1;
                if (state.PlayerLabId >= 0)
                {
                    LabState player = state.Player;
                    if (record.CrownLabId == player.Id)
                    {
                        player.CrownRunnerUpStreak = 0;
                        for (int i = 0; i < state.Labs.Count; i++)
                            if (!state.Labs[i].Bankrupt)
                                RivalSystem.Feel(c, state.Labs[i], state.Labs[i].Id == runnerUp ? c.Rivalry.Feelings.SecondPlacePermille : c.Rivalry.Feelings.CrownPermille);
                    }
                    else if (runnerUp == player.Id)
                    {
                        player.CrownRunnerUpStreak++;
                        EndingSystem.TrySecondPlace(c, state, player);
                    }
                    else player.CrownRunnerUpStreak = 0;
                }
            }
            if (record.SafestLabId >= 0)
            {
                AddReputation(state.Lab(record.SafestLabId), c.Gauntlet.SafestReputation);
                Events.Add(state, EventKind.GauntletSafest, record.SafestLabId, -1, safestAlignment, "");
            }

            state.Gauntlets.Add(record);
            while (state.Gauntlets.Count > c.Gauntlet.HistoryYears) state.Gauntlets.RemoveAt(0);
            return record;
        }

        static GauntletEntry Entry(GameConfig c, GameState state, string bracket, LabState lab, ModelState model, long trueScore)
        {
            int jitter = c.Gauntlet.JitterPermille > 0 ? state.Rng.Ranking.Range(-c.Gauntlet.JitterPermille, c.Gauntlet.JitterPermille) : 0;
            var e = new GauntletEntry();
            e.Bracket = bracket;
            e.LabId = lab.Id;
            e.ModelId = model.Id;
            e.ModelName = model.Name;
            e.Score = MathI.MulPermille(trueScore, MathI.Permille + jitter);
            return e;
        }

        /// <summary>Best score first. Equal scores go to the older lab, so the order never depends on the sort used.</summary>
        static void Rank(List<GauntletEntry> entries)
        {
            entries.Sort(delegate (GauntletEntry a, GauntletEntry b)
            {
                if (a.Score != b.Score) return a.Score > b.Score ? -1 : 1;
                return a.LabId.CompareTo(b.LabId);
            });
            for (int i = 0; i < entries.Count; i++) entries[i].Rank = i + 1;
        }

        static void AddReputation(LabState lab, int amount)
        {
            lab.Reputation = MathI.Clamp(lab.Reputation + amount, 0, MathI.Permille);
        }

        /// <summary>The latest ranking of a bracket, best first, or an empty list before the first Gauntlet.</summary>
        public static List<GauntletEntry> Latest(GameState state, string bracket)
        {
            var list = new List<GauntletEntry>();
            if (state.Gauntlets.Count == 0) return list;
            GauntletRecord record = state.Gauntlets[state.Gauntlets.Count - 1];
            for (int i = 0; i < record.Entries.Count; i++)
                if (record.Entries[i].Bracket == bracket) list.Add(record.Entries[i]);
            return list;
        }
    }
}
