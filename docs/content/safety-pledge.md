# The public safety pledge

The lab can promise customers that every model it launches will show at least some alignment (`pledge.*` in the data; Q350). Two saved lab fields (`pledgeLevel`, `pledgeLockedUntil`, schema 68); older saves start with no pledge.

## `SetPledge`

| What | Rule |
|---|---|
| Levels | 0 (none) to `maxLevel` (3). Level 1 asks 450 alignment (`baseFloor`), each level above adds `floorStep` (150), so 600 and 750 |
| Making or raising it | Free. Locks the level for `lockWeeks` (26) |
| Walking it back | Lowering or dropping it costs `walkBackReputation` (15). Also locks the level |
| While it holds | Each sector the lab sells in gives `trustPerLevel` (1) extra trust a week for every level |
| Breaking it | A first release that shows less than the floor: the level goes to 0, `breachReputation` (40) lost, `breachTrust` (120) lost in every sector the lab sells in, and the level is locked for `lockWeeks` |

The floor is checked on the alignment a launch shows (the claim on `ReleaseModel`), not the one the model has. A lab that claims enough keeps its pledge, and the whistleblower (`whistle.*`) is the check on that.

Refused, with nothing changed, for a rival, a game with `maxLevel` 0, a level out of range and a level already held (`InvalidArgument`), and a level inside the lock (`PledgeLocked`).

## Notes

- Events: `PledgeSet` (SubjectId the new level, Amount the reputation lost, Pick 1 when raised) and `PledgeBroken` (SubjectId the model, Amount the floor it missed, Pick the level broken).
- Only first releases are checked; an upgrade of a model already on sale is not.
- No random numbers; a lab that never pledges is exactly as before.
