# Red teams

The bible's chapter 10 lists "red teams: people paid to break it first" among what builds control. The player can pay for one attack on a single model. Rivals and the autopilot never do. Nothing happens until the player orders it (`redTeam` in the data; Q301).

## `RedTeamModel`

It takes a model of the lab that is trained or on sale. It is refused, changing nothing, for a lab that is not the player's, a model that does not exist, a model still training or retired, a model whose red-team weeks are still running, a lab with fewer than `minStaff` (1) red-teamers, a game whose `weeks` is 0, and a lab that cannot pay.

| What | Shipped |
|---|---|
| Bill | the larger of $15,000 (`minCost`) and 2 weeks (`revenueWeeks`) of what the model sells for, on the training ledger line |
| Gap | 50% of the gap between the shown and real alignment is closed (`gapClosePermille`) |
| Fixes | +30 real alignment, and the shown meter moves with it, never past 1000 (`alignmentGain`) |
| Culture | +5 safety culture (`culturePoints`) |
| Safer for | 26 weeks (`weeks`); the model's own weekly incident chance loses 30% (`incidentCutPermille`) |

## Notes

- The cut comes off the model's own chance (after a rushed launch). Overload and crunch add their share on top and are not cut. The incident breakdown has its own line, `FromRedTeam`, zero or negative, so the parts still add up to the total.
- A model that was sandbagging comes back with a more honest meter. That can put it under the lab's own release gate, which is the point of the trade.
- Saved per model: `redTeamUntil` (0 when never red-teamed), schema 45. `EventKind.RedTeamRan` is logged.
- The red-teamer role and its room already exist; `redTeam.roleId` can be emptied to ask for no staff.
