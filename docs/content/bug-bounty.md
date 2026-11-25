# The bug bounty

A standing programme that pays outsiders to find faults in the released models before customers do. It takes a fee to raise, a bill every week for every model on sale, and takes a share off the weekly chance of an incident while it runs (`bugBounty.*` in the data; Q343). Changing the level locks it for some weeks. Schema 67 adds two numbers to each lab.

## `SetBugBounty`

| What | Rule |
|---|---|
| Levels | 0 (none) to `maxLevel` (3) |
| Fee | `setupPerLevel` (40000) for each level a change raises it by; lowering it, or closing it, is free |
| Weekly bill | The level times `weeklyPerLevel` (2000) times the models on sale, taken every week (Salaries line) |
| Effect | `cutPerLevel` (100 permille, so 10%) of the weekly incident chance for each level, so at most 30%. It is one more term in the weekly chance, beside a chief safety officer's, and the era's incident floor still holds under all of it |
| Lock | After a change the level holds for `cooldownWeeks` (13) |

Refused, with nothing changed, for a rival, a game with `maxLevel` 0, a level outside the range or the one it already is (`InvalidArgument`), a change inside the lock (`BountyLocked`) and not enough cash for the fee (`NotEnoughCash`).

## Notes

- The bill grows with every model on sale, so a lab with a wide range pays more for the same cut; a lab with nothing on sale pays nothing.
- The lock is what stops it being switched on for the weeks the risk is high and off again.
- The incident breakdown has a `FromBounty` term (zero or negative).
- Event `BountySet` (SubjectId the new level, Amount the fee, Pick 1 when raised).
- Schema 67: `bountyLevel`, `bountyLockedUntil` on every lab (older saves: 0). The save check flags a level out of range, a negative lock and either on a rival.
