# The ethics board

The player can seat an ethics board: outsiders who refuse rushed launches, soften the reputation loss of an incident, help at hearings and ease regulator attention. It costs setup and a weekly fee, needs safety staff behind it, and is public: stepping it down, or losing the staff that keep it, is a resignation that costs reputation and keeps any board away for a year. Rivals and the autopilot never seat one. Nothing happens until the player does (`ethicsBoard.*` in the data; Q320).

## `SetEthicsBoard(level)`

Level 0 to `maxLevel` (2). Raising pays `setupCostPerLevel` ($40000) for each level added, needs `safetyStaffPerLevel` (1) safety people for each level (`NotEnoughStaff`), and is refused while the lock is on (`EthicsLocked`). Lowering is free in cash but is a resignation: `resignationReputation` (20) comes off the lab's reputation and the lock runs for `lockWeeks` (52). It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` 0, the level it already has, and a lab that cannot pay.

## What a board does, for each level seated

| What | Per level |
|---|---|
| An incident's reputation loss | -`scandalCutPermillePerLevel` (15%) |
| A hearing's reputation points | +`hearingPointsPerLevel` (3) |
| The monthly regulator attention gain | -`attentionPermillePerLevel` (5%), added to the compliance office and the audit certificate |
| A rushed launch (`RushRelease`) | refused from level 1 (`BoardVeto`) |
| A first release | from `vetoLevel` (2) refused when the model's *true* alignment is under `vetoAlignment` (400), whatever it displays (`BoardVeto`). A model already on sale can still be sold into more sectors |

## What it costs

- `weeklyCostPerLevel` ($2500) a week for each level.
- The safety staff. If they fall below what the level needs the board resigns on the next week, down to what the staff cover, with the same reputation loss and lock as stepping down.

## Notes

- Saved per lab: `ethicsLevel` and `ethicsLockedUntil`, schema 54. A level outside the data or a negative lock is reported.
- Events `EthicsBoardChanged` (Pick 1 up, 0 down) and `EthicsBoardResigned`.
