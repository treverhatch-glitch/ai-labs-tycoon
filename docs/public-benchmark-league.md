# Public benchmark league (Q228+)

Player-only weekly scoreboards: submit a **released** model in a benchmark’s sector, survive **vetting**, stay **live** with upkeep reputation, and compete for a **global crown** (cash + reputation). Branching choices: withdraw during vetting, **pledge defense** (bonus while leading, harsher demotion), or **retarget** to a stronger model (restarts vetting, fee).

Not the yearly Gauntlet (automatic brackets), not enterprise/data-license contract pipelines, not distill consumables.

## Commands

- `SubmitPublicBenchmark` — pay `entryFee`, start vetting.
- `WithdrawPublicBenchmark` — abandon an open entry.
- `PledgeBenchmarkDefense` — pay `pledgeFee` while live; extra leader rep, higher demotion penalty.
- `RetargetPublicBenchmark` — swap model, pay `retargetFee`, vetting resets.

## Weekly `BenchmarkLeagueSystem.RunWeek`

1. Per lab: advance vetting → live, live upkeep rep, demote if model no longer qualifies.
2. Global: highest live score per benchmark updates `GameState.benchmarkLeaders`, pays leader stipend, emits leader events.

## Cross-system

- `DisplayQueries.AppealParts` — `liveAppealPermille` / `leaderAppealPermille` when the lab has live (or leading) entries in that sector.

## Era overlap

Independent of Gauntlet crowns, capability domain leaders, and release review. Rivals may submit entries in tests; player commands are gated by `Enabled` (player lab only).