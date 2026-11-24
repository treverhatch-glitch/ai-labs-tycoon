# Promotions

The player picks a person and makes them senior. A fee and a raise now buy someone who counts for more in their function, in the same chair. A function can only be so senior, and seniors are the last to go when people are let go (`seniority.*` in the data; Q337). Schema 63 adds one flag to each person.

## `PromoteStaff(person)`

| What | Rule |
|---|---|
| Fee | `feePermille` (1500) of the role's hire cost, paid at once |
| Raise | `raisePermille` (400): the senior's weekly salary is the role's salary plus that share |
| Morale | The person gains `moraleBoost` (150) and needs `minMorale` (400) to be promoted at all |
| Limit | At most `maxSharePermille` (250) of a function's people, rounded up, so a small team can have one |

Refused, with nothing changed, for a lab that is not the player's, a game with `enabled` 0, an unknown person (`UnknownId`), a person already senior, one too unhappy, a function already at its limit (all `InvalidArgument`) and not enough cash (`NotEnoughCash`).

## What a senior does

A senior counts as `weightPermille` (1600) of a person where it matters: a senior researcher makes 1.6 times the research, and a senior engineer, safety tester or salesperson counts as 1.6 heads in training speed, alignment work and the sales pull. The head counts themselves (seats, caps that count people) do not change. A lab with no seniors gets exactly the numbers it had before.

Seniors are let go last: letting people go, and layoff rounds, take the lowest morale first but only reach a senior when no one else is left in the function. Moving a senior to a function with a different kind of work (retraining) starts them over as an ordinary person.

## Notes

- Event `StaffPromoted` (SubjectId the person, Amount the fee, Ref the role id).
- The validator keeps the weight between 1000 and 3000, and keeps the raise from falling more than 400 permille below the extra weight, so a promotion is never a free gain.
- Schema 63: `senior` on every person (older saves: 0). Only the player's lab has seniors; a save with a senior rival is flagged.
