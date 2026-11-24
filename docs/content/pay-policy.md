# Pay policy

Everyone starts at market pay. The player can pay the team better or worse than that, which moves the weekly salary bill, how happy people settle, and how many of a rival's offers turn their heads. Rivals and the autopilot never change pay. Nothing happens until the player sets a level (`staffPeople.pay*` in the data; Q303).

## `SetPayLevel`

It takes a level from `payMinLevel` (-1) to `payMaxLevel` (2). It is refused for a lab that is not the player's, a level out of range, the level the lab already has, a game with both limits at 0, and a change less than `payLockWeeks` (26) after the last one.

| Level | Salaries | Resting morale | Rival offers turned away |
|---|---|---|---|
| -1 | 85% | -40 | -15 points (so offers are 15% likelier) |
| 0 | 100% | 0 | 0 |
| 1 | 115% | +40 | +15 points |
| 2 | 130% | +80 | +30 points |

(`paySalaryStepPermille` 150, `payMoraleStep` 40, `payPoachStepPermille` 150.)

## A cut

Cutting pay takes `payCutShock` (100) morale from everyone at once for each level it falls, never below 0. A raise gives no jolt; people drift up to the new level at the usual weekly rate. Someone who quits when unhappy can leave after a cut, and a team below `crunchMinMorale` cannot start crunch.

## Notes

- Room defences and pay add together and are kept between -500 and 900 permille, so a rival's offer is never ruled out and is never more than one and a half times likelier.
- The salary query feeds the offsite price and the shady cost estimates, so they follow the policy. Hiring fees and severance do not.
- Saved per lab: `payLevel` and `payLockedUntil`, schema 47. `EventKind.PayChanged` is logged.
