<!-- Generated from Rules/Data/default-config.json (pivot). The data file is the source of truth: edit it, then update this page. -->
# Pivoting the lab

The bible's chapter 5 says "speciality points can be re-spent once per era at a cost: a pivot loses some staff and confuses customers for a year", and chapter 3 asks the player, every era, to "decide whether your lab's identity still fits, or pivot". The player picked a style at the start (Sniper, Shotgun or Wildcard). They can now change it (see QUESTIONS Q260 and Q261).

## `PivotStyle`

`Command.PivotStyle(labId, styleId)`. It changes `StyleId` at once, so the new focus sectors get their bonus and the old ones lose it from that moment. It also:

| What | Effect (shipped data) |
|---|---|
| Money | Costs **8** weeks of running the lab (`costWeeks`; never under `minWeekCost` 1,000), on the hiring line |
| Staff | **15%** of the staff leave (`staffLossPermille`), rounded down, at least one. The unhappiest go first; a tie goes to the earlier hire |
| Customers | Every model's appeal is **-12%** for **52** weeks (`confusionAppealPermille`, `confusionWeeks`). It is its own line in the appeal breakdown (`PivotConfusion`) |

Once per era: `PivotEraId` holds the era of the last pivot and a second one in the same era is refused (`PivotSpent`). Also refused, changing nothing: an unknown style, the style the lab already has, not enough cash, or any lab but the player's. A lab that picked no style at the start may pick one this way.

`EventKind.StylePivoted` is logged (SubjectId the number who left, Amount the cost, Ref `from|to`) and the company chat has a line for it.

## Notes

- Saved per lab: `pivotEraId` and `pivotUntilTick`, schema 38.
- Rivals and the autopilot never pivot, so the economy guards are untouched.
- Open: the pivot screen (Unity); a founder who keeps their old edge or loses it on a pivot; rivals changing identity.
