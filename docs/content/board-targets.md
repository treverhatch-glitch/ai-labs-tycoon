# Board targets

The player can promise the board a revenue number for the next quarter and is judged on what the lab actually makes. A met target earns reputation and a missed one costs more, so the decision is how hard to push (`revenueTargets.*` in the data; Q338). Nothing is judged until a target is set. Schema 64 adds four numbers to each lab.

## `SetRevenueTarget(level)`

`level` is 1 to `maxLevel` (3). The goal is the last week's revenue times `weeks` (13), stretched by `stretchPerLevel` (150 permille) for each level, so level 1 asks for 15 percent more than the current pace, level 3 for 45 percent. The lab needs at least `minWeeklyRevenue` (1000) last week, so there is something to measure against. One target at a time: a second is refused until the first is judged. Refused, with nothing changed, for a lab that is not the player's, a game with `maxLevel` 0, a level out of range, a target already running and too little revenue (all `InvalidArgument`).

## Judging

Each week the revenue made is added to the total so far. On the last of the 13 weeks the lab is judged:

| Result | Reputation |
|---|---|
| Met (total at or over the goal) | `+rewardPerLevel` (10) times the level |
| Missed | `-penaltyPerLevel` (15) times the level |

Reputation stays between 0 and 1000, and the target clears itself so a new one can be set at once. The validator keeps the penalty at or above the reward, so a missed target always costs at least what a met one earns.

## Notes

- Events `TargetSet` (SubjectId the level, Amount the goal) and `TargetSettled` (SubjectId the level, Amount the revenue made, Pick 1 when met).
- Schema 64: `revenueTargetLevel`, `revenueTargetEnd`, `revenueTargetGoal`, `revenueTargetSoFar` on every lab (older saves: 0). Only the player's lab sets one; a save with a rival target or a half-set target is flagged.
- No random draws; the judge only reads the revenue the market already worked out.
