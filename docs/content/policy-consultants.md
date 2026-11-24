# Policy consultants

When a hearing or a push on the government is coming and the lab has no policy people, the player can hire a firm of consultants for a few weeks. Each level counts as extra policy staff for hearings, the monthly attention gain and lobbying. The compliance office never counts them. Rivals and the autopilot never hire them. Nothing happens until the player does (`consultants.*` in the data; Q326).

## `HireConsultants(level)`

Level 1 to `maxLevel` (2). It costs `feePerLevel` ($30000) a level when the firm comes in. It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` or `weeks` 0, a level outside the range, a firm that is already in, a firm that left less than `cooldownWeeks` (26) after the last one came in (`ConsultantsLocked`), and a lab that cannot pay (`NotEnoughCash`). A refusal changes nothing.

## What the firm does (for `weeks`, 12)

Each level counts as `staffPerLevel` (3) policy people, added to the people on the payroll for:

| What | Effect |
|---|---|
| The monthly attention gain | The policy saving (40 permille a person, capped at 600 permille in the data) uses the combined number, so the cap still holds |
| A hearing | The lab is "prepared" when the combined number reaches `preparedPolicyStaff` |
| Lobbying | A lower backfire chance (down to its floor) and a higher subsidy chance (up to its cap) |

A firm costs `weeklyPerLevel` ($2000) a level on salaries for each of its weeks. When the weeks are up it leaves (event `ConsultantsHired`, Pick 0) and the cooldown keeps running from the day it came in.

## What it does not do

The compliance office counts only the people on the payroll, so consultants cannot raise the office's staff cover, its audit fine relief, hearing points or attention relief. A lab with the top compliance level and a firm in gets the same audit relief as without the firm (Q326).

## Notes

- Saved per lab: `consultantLevel`, `consultantWeeks`, `consultantLockedUntil`, schema 57. A firm that does not fit the data is reported.
- Event `ConsultantsHired` (Pick 1 when they come in, 0 when they leave).
- The data validator requires `cooldownWeeks` to be at least `weeks`.
