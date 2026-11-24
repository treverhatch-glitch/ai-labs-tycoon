# Layoff rounds

The player can let go of the same share of every function in one command. It is cheaper per head than letting people go one at a time, but the people who stay and the press mind it (`layoffs.*` in the data; Q335). No new state.

## `Layoff(permille)`

`permille` is 1 to `maxPermille` (300). Each function lets go of that share of its people, rounded down, taking the lowest morale first. Refused, with nothing changed, for a lab that is not the player's, a game with `enabled` 0, a share outside the range (`InvalidArgument`) and a share that rounds down to nobody in every function (`NotEnoughStaff`). A function with one person is never emptied by a round.

## What happens

| What | Amount |
|---|---|
| Package | `severancePermille` (500) of the role's severance for each person, so half of what letting them go one by one costs |
| Reputation lost | `reputationAtAll` (40) times the share of the whole staff let go |
| Morale lost by everyone who stays | `moraleAtAll` (200) times the same share |

The share used is the share of the whole staff actually let go, after rounding. The payroll falls by what was let go, and the listening roles' safety culture is moved the same way as when they are let go one by one.

## Notes

- Event `LayoffRound` (Amount the people let go, SubjectId the share in permille, Pick 1 when the share is at least `deepPermille`, 200).
- The validator keeps the package between 100 and 1000 permille (cheaper than one by one but never free) and the cap at 500.
