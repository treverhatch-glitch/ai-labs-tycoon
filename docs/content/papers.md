# Published papers

The player can put the lab's research into print. Rivals and the autopilot never publish. Nothing happens until the player does (`prestige.*` in the data; Q308).

## `PublishPaper`

It costs research points and nothing else: `paperResearchWeeks` (3) weeks of the lab's current research output, never under `minPoints` (60). It is refused for a lab that is not the player's, a game with no papers (`max` or `paperPrestige` at 0), a lab already at `max` (1000), fewer than `minResearchers` (2) researchers on the payroll, too few research points, and a bankrupt lab.

A paper adds `paperPrestige` (200, never past `max`) and `paperReputation` (8) points, and logs `PaperPublished`. Prestige fades by `decayPerWeek` (4) every week, so one paper lasts about 50 weeks.

## What prestige does

Every effect grows in step with prestige and is at the data's value at the most.

| Prestige | Morale settles | Hiring fees | Counter-offers | Rival offers |
|---|---|---|---|---|
| 0 | 0 | 100% | 0 | 100% |
| 500 | +30 | 90% | +50 permille | 110% |
| 1000 | +60 | 80% | +100 permille | 120% |

(`moraleAtMax` 60, `hireDiscountAtMax` 200, `counterAtMax` 100, `poachAtMax` 200.)

- Morale goes into the resting level, like pay and rooms, and people drift to it.
- The fee discount is added to the recruiters' discount and shows in `RolePerks.HireFeeParts` as the perk discount.
- The counter chance bonus is added to a counter-offer's base chance, before the usual floor and ceiling.
- Rival offers are likelier by the same share that a room or a pay level would make them less likely; the sum stays between -500 and 900 permille.

## Notes

- Saved per lab: `prestige`, schema 49.
- The paper cost reads the lab's research output, so a bigger team pays more points for each paper.
