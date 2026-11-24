# Technical debt

Cutting corners piles up debt in the player's lab. Debt never wears off. It raises the weekly chance of an incident in proportion to how much there is, and the only way to clear it is to pay for it with cash and engineers (`techDebt.*` in the data; Q336). Schema 62 adds one number to each lab.

## Where it comes from

| Source | Points |
|---|---|
| A rushed launch | `rushPerWeek` (40) for each week of testing skipped |
| A week of crunch time | `crunchPerWeek` (3) |
| Fine-tuning a glitching model | `fineTunePatch` (30) |

Debt stops at 1000. Only the player's lab keeps it; rivals never do, and a save that gives a rival debt is flagged. A lab that cuts none of these corners never has any.

## What it does

`incidentAtFull` (40 permille) is added to the model's weekly incident chance at 1000 points and less for less (20 at 500). It sits in the same sum as crunch and overload, so the incident breakdown shows it as `FromDebt` and the parts still add up to the total. The incident floor applies after it, as with the other parts.

## `PayDownDebt(points)`

1 to `maxPerCommand` (250) points at a time, no more than the lab owes, for `costPerPoint` (2500) cash each, and the lab needs at least `minEngineers` (2) engineers. Refused with `InvalidArgument` (not the player, off, bad amount, no debt or more than owed), `NotEnoughStaff` (too few engineers) or `NotEnoughCash`; nothing changes. Event `DebtPaid` (Amount the points retired, SubjectId the debt left, Pick 1 when none is left).

## Notes

- Schema 62: `techDebt` on every lab (older saves: 0).
- No timers and no new random draws: every effect is a number the player can read.
