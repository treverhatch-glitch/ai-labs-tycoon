# Information security

The lab builds security up a tier at a time against an exposure that fills as its models sell (`security.*` in the data; Q372). Six saved lab fields (`securityTier`, `securityBuilding`, `securityDue`, `exposure`, `breachDue`, `ransoms`; schema 81, shared with the litigation docket); older saves start at tier 0 with nothing open. Only the player's lab.

## Tiers

`BuildSecurity` builds the next tier (up to `maxTier`, 4): `tierCost` (30,000) times the tier's number is paid now, the tier stands after `buildWeeks` (8) and every tier held costs `upkeepPerTier` (500) a week. One tier is built at a time.

## Exposure and breaches

Each week every model on sale adds `exposurePerModel` (3) and every tier takes `reliefPerTier` (3) off, never below nothing. At `threshold` (150) a breach opens and the lab has `answerWeeks` (4) to answer with `AnswerBreach`:

| Answer | What it does |
|---|---|
| 0 pay the ransom | `ransom` (15,000) plus that again for every ransom paid since the last disclosure. Quiet |
| 1 disclose | Reputation (`discloseReputation`, 20, damped by the press), trust in every sector the lab sells in, resets the ransom price and draws a privacy claim (Q371) |
| 2 lock down | The next tier at once for double its price. Refused at the top tier or while a tier is being built |

An answer clears the exposure. An unanswered breach leaks: `leakReputation` (40, damped), the same trust loss and a stronger privacy claim.

Events: `SecurityBuilt`, `BreachOpened`, `BreachAnswered` (Pick 0, 1, 2), `BreachLeaked`.
