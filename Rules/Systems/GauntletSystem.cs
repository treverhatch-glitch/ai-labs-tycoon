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

        public const string AwardConfident = "confident-wrong";
        public const string AwardNewcomer = "newcomer";

        /// <summary>One tuned lab's entries this year, with the scores they would have had untuned.</summary>
        sealed class Tuned
        {
            public int LabId;
            public int Lift;
            public readonly List<GauntletEntry> Entries = new List<GauntletEntry>();
            public readonly List<long> Raw = new List<long>();
        }

        /// <summary>The score lift, in permille, a lab's tuning would give its entries.</summary>
        public static int TuningLift(GameConfig c, LabState lab)
        {
            if (c.Gauntlet.MaxTuningPermille < 1) return 0;
            return (int)MathI.MulPermille(MathI.Clamp(lab.GauntletTuning, 0, c.Gauntlet.MaxTuningPermille), c.Gauntlet.TuningScoreSharePermille);
        }

        /// <summary>The prep bill for a tuning at this time: sales-role hire fees for each 100 permille.</summary>
        public static long TuningCost(GameConfig c, int tick, int tuning)
        {
            RoleConfig sales = c.Role("sales");
            if (sales == null || tuning <= 0) return 0;
            return EraRules.HireFee(c, sales, tick) * c.Gauntlet.TuningCostHireFeesPer100 * tuning / 100;
        }

        /// <summary>The chance, in permille, that a tuned win is exposed: more tuning and more feuding rivals both make it likelier.</summary>
        public static int ExposeChance(GameConfig c, GameState state, LabState lab)
        {
            long chance = (long)lab.GauntletTuning * c.Gauntlet.ExposeChancePerTuningPermille / 1000;
            if (lab.Id == state.PlayerLabId)
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState rival = state.Labs[i];
                    if (rival.Controller != LabController.Plan || rival.Bankrupt || rival.AbsorbedByLabId >= 0) continue;
                    if (RivalSystem.IsFeud(c, rival)) chance += c.Gauntlet.ExposeFeudBonusPermille;
                }
            return (int)MathI.Clamp(chance, 0, 950);
        }

        public static GauntletRecord RunYear(GameConfig c, GameState state)
        {
            var record = new GauntletRecord();
            record.Year = GameDate.Year(c.Calendar, state.Tick);
            record.CrownLabId = -1;
            record.CrownModelId = -1;
            record.SafestLabId = -1;

            List<Tuned> tuned = PayForTuning(c, state);
            var brackets = new List<List<GauntletEntry>>();
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sectorState = state.Sectors[s];
                SectorConfig sector = c.Sector(sectorState.Id);
                var bracket = new List<GauntletEntry>();
                for (int i = 0; i < sectorState.Entries.Count; i++)
                {
                    LabState lab = state.Lab(sectorState.Entries[i].LabId);
                    ModelState model = lab.Model(sectorState.Entries[i].ModelId);
                    bracket.Add(Tune(tuned, lab, Entry(c, state, sector.Id, lab, model, LabQueries.Quality(c, sector, model, state.Tick))));
                }
                brackets.Add(bracket);
            }

            var crown = new List<GauntletEntry>();
            int safestAlignment = -1;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                ModelState best = LabQueries.BestReleased(lab);
                if (best == null) continue;
                crown.Add(Tune(tuned, lab, Entry(c, state, CrownBracket, lab, best, best.Capability)));
                if (best.Alignment > safestAlignment)
                {
                    safestAlignment = best.Alignment;
                    record.SafestLabId = lab.Id;
                }
            }

            RankAll(brackets, crown);
            if (Expose(c, state, tuned)) RankAll(brackets, crown);

            for (int s = 0; s < brackets.Count; s++)
            {
                List<GauntletEntry> bracket = brackets[s];
                string sectorId = state.Sectors[s].Id;
                record.Entries.AddRange(bracket);

                state.Sectors[s].ChampionLabId = bracket.Count > 0 ? bracket[0].LabId : -1;
                if (bracket.Count > 0)
                {
                    Events.Add(state, EventKind.GauntletBracket, bracket[0].LabId, bracket[0].ModelId, bracket[0].Score, sectorId);
                    if (state.PlayerLabId >= 0 && bracket[0].LabId == state.PlayerLabId)
                        for (int i = 1; i < bracket.Count; i++) RivalSystem.Feel(c, state.Lab(bracket[i].LabId), c.Rivalry.Feelings.BracketPermille);
                }
            }
            record.Entries.AddRange(crown);

            if (crown.Count > 0)
            {
                record.CrownLabId = crown[0].LabId;
                record.CrownModelId = crown[0].ModelId;
                AddReputation(state.Lab(record.CrownLabId), c.Gauntlet.CrownReputation);
                Events.Add(state, EventKind.GauntletCrown, record.CrownLabId, record.CrownModelId, crown[0].Score, crown[0].ModelName);
                PressSystem.OnBeat(c, state, state.Lab(record.CrownLabId), "crown", c.Gauntlet.CrownReputation, crown[0].ModelName);
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

            RunDemos(c, state, crown);
            GiveJokeAwards(c, state, record, crown);

            state.Gauntlets.Add(record);
            while (state.Gauntlets.Count > c.Gauntlet.HistoryYears) state.Gauntlets.RemoveAt(0);
            return record;
        }

        static List<Tuned> PayForTuning(GameConfig c, GameState state)
        {
            var list = new List<Tuned>();
            if (c.Gauntlet.MaxTuningPermille < 1) return list;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.GauntletTuning <= 0 || lab.Bankrupt) continue;
                long cost = TuningCost(c, state.Tick, (int)MathI.Min(lab.GauntletTuning, c.Gauntlet.MaxTuningPermille));
                bool paid = lab.Cash >= cost;
                if (paid)
                {
                    if (cost > 0) Finance.Spend(lab, LedgerLine.Hiring, cost);
                    var row = new Tuned();
                    row.LabId = lab.Id;
                    row.Lift = TuningLift(c, lab);
                    list.Add(row);
                }
                Events.Add(state, EventKind.GauntletTuned, lab.Id, -1, cost, "").Pick = paid ? 1 : 0;
            }
            return list;
        }

        static GauntletEntry Tune(List<Tuned> tuned, LabState lab, GauntletEntry entry)
        {
            for (int i = 0; i < tuned.Count; i++)
            {
                if (tuned[i].LabId != lab.Id) continue;
                tuned[i].Entries.Add(entry);
                tuned[i].Raw.Add(entry.Score);
                entry.Score = MathI.MulPermille(entry.Score, MathI.Permille + tuned[i].Lift);
                entry.Tuned = tuned[i].Lift > 0;
            }
            return entry;
        }

        static void RankAll(List<List<GauntletEntry>> brackets, List<GauntletEntry> crown)
        {
            for (int i = 0; i < brackets.Count; i++) Rank(brackets[i]);
            Rank(crown);
        }

        /// <summary>Rolls the exposure for every tuned lab that actually won something by it. Returns true if any was exposed (the ranking then needs doing again).</summary>
        static bool Expose(GameConfig c, GameState state, List<Tuned> tuned)
        {
            bool any = false;
            for (int i = 0; i < tuned.Count; i++)
            {
                Tuned row = tuned[i];
                if (row.Lift <= 0) continue;
                bool won = false;
                for (int e = 0; e < row.Entries.Count; e++) if (row.Entries[e].Rank == 1) won = true;
                if (!won) continue;
                LabState lab = state.Lab(row.LabId);
                if (!state.Rng.Ranking.Chance(ExposeChance(c, state, lab))) continue;

                for (int e = 0; e < row.Entries.Count; e++)
                {
                    row.Entries[e].Score = row.Raw[e];
                    row.Entries[e].Tuned = false;
                }
                int before = lab.Reputation;
                AddReputation(lab, -PressSystem.Damp(c, lab, c.Gauntlet.ExposedReputationLoss));
                Events.Add(state, EventKind.GauntletExposed, lab.Id, row.Entries.Count, before - lab.Reputation, "");
                any = true;
            }
            return any;
        }

        /// <summary>Each lab's crown entry takes the stage. Reliability decides: a steady model is a triumph and a flaky one is a meme.</summary>
        static void RunDemos(GameConfig c, GameState state, List<GauntletEntry> crown)
        {
            if (c.Gauntlet.DemoMemeFromPermille < 1) return;
            for (int i = 0; i < crown.Count; i++)
            {
                LabState lab = state.Lab(crown[i].LabId);
                if (lab.Bankrupt || lab.AbsorbedByLabId >= 0) continue;
                ModelState model = lab.Model(crown[i].ModelId);
                int chance = SafetySystem.IncidentChancePermille(c, state.Tick, model);
                int before = lab.Reputation;
                bool triumph = chance <= c.Gauntlet.DemoTriumphBelowPermille;
                bool meme = chance >= c.Gauntlet.DemoMemeFromPermille;
                if (!triumph && !meme) continue;
                if (triumph) AddReputation(lab, c.Gauntlet.DemoTriumphReputation);
                else AddReputation(lab, -PressSystem.Damp(c, lab, c.Gauntlet.DemoMemeReputation));
                Events.Add(state, EventKind.GauntletDemo, lab.Id, model.Id, lab.Reputation - before, model.Name).Pick = triumph ? 1 : 0;
            }
        }

        static void GiveJokeAwards(GameConfig c, GameState state, GauntletRecord record, List<GauntletEntry> crown)
        {
            int worstGap = c.Gauntlet.ConfidentWrongMinGap - 1;
            for (int i = 0; i < crown.Count; i++)
            {
                LabState lab = state.Lab(crown[i].LabId);
                if (lab.Bankrupt || lab.AbsorbedByLabId >= 0) continue;
                int gap = SafetySystem.AlignmentGap(c, lab.Model(crown[i].ModelId));
                if (gap > worstGap || (gap == worstGap && record.ConfidentLabId >= 0 && lab.Id < record.ConfidentLabId))
                {
                    worstGap = gap;
                    record.ConfidentLabId = lab.Id;
                }
            }
            if (record.ConfidentLabId >= 0)
            {
                LabState lab = state.Lab(record.ConfidentLabId);
                int before = lab.Reputation;
                AddReputation(lab, -PressSystem.Damp(c, lab, c.Gauntlet.ConfidentWrongReputation));
                Events.Add(state, EventKind.GauntletAward, lab.Id, -1, before - lab.Reputation, AwardConfident);
            }

            int newcomerWeeks = c.Gauntlet.NewcomerYears * c.Calendar.WeeksPerYear;
            for (int i = 0; i < crown.Count && newcomerWeeks > 0; i++)
            {
                LabState lab = state.Lab(crown[i].LabId);
                if (lab.Bankrupt || lab.AbsorbedByLabId >= 0 || lab.Id == record.CrownLabId) continue;
                int first = FirstReleaseTick(lab);
                if (first < 0 || state.Tick - first > newcomerWeeks) continue;
                record.NewcomerLabId = lab.Id;
                break;
            }
            if (record.NewcomerLabId >= 0)
            {
                LabState lab = state.Lab(record.NewcomerLabId);
                int before = lab.Reputation;
                AddReputation(lab, c.Gauntlet.NewcomerReputation);
                for (int i = 0; i < lab.Staff.Count; i++)
                    lab.Staff[i].Morale = MathI.Clamp(lab.Staff[i].Morale + c.Gauntlet.NewcomerMorale, 0, MathI.Permille);
                Events.Add(state, EventKind.GauntletAward, lab.Id, -1, c.Gauntlet.NewcomerMorale, AwardNewcomer);
            }
        }

        static int FirstReleaseTick(LabState lab)
        {
            int first = -1;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                int t = lab.Models[i].ReleasedTick;
                if (t >= 0 && (first < 0 || t < first)) first = t;
            }
            return first;
        }

        /// <summary>One row of the season table.</summary>
        public sealed class SeasonRow
        {
            public int LabId;
            public int Crowns;
            public int BracketWins;
            public int Points;
        }

        /// <summary>Every lab that has stood in the Gauntlet, ranked over the kept years: crowns, then bracket wins, by points.</summary>
        public static List<SeasonRow> SeasonTable(GameConfig c, GameState state)
        {
            var rows = new List<SeasonRow>();
            for (int r = 0; r < state.Gauntlets.Count; r++)
            {
                List<GauntletEntry> entries = state.Gauntlets[r].Entries;
                for (int i = 0; i < entries.Count; i++)
                {
                    SeasonRow row = null;
                    for (int k = 0; k < rows.Count; k++) if (rows[k].LabId == entries[i].LabId) row = rows[k];
                    if (row == null)
                    {
                        row = new SeasonRow();
                        row.LabId = entries[i].LabId;
                        rows.Add(row);
                    }
                    if (entries[i].Rank != 1) continue;
                    if (entries[i].Bracket == CrownBracket) row.Crowns++;
                    else row.BracketWins++;
                }
            }
            for (int i = 0; i < rows.Count; i++)
                rows[i].Points = rows[i].Crowns * c.Gauntlet.SeasonCrownPoints + rows[i].BracketWins * c.Gauntlet.SeasonBracketPoints;
            rows.Sort(delegate (SeasonRow a, SeasonRow b)
            {
                if (a.Points != b.Points) return a.Points > b.Points ? -1 : 1;
                if (a.Crowns != b.Crowns) return a.Crowns > b.Crowns ? -1 : 1;
                return a.LabId.CompareTo(b.LabId);
            });
            return rows;
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
