# Corporate tax

The lab pays a share of its profit at the year end, and can pay less by building a structure or claiming the research credit, at the price of a tax audit (`tax.*` in the data; Q379). Eleven saved lab fields (`taxStructure`, `taxBuilding`, `taxDue`, `taxAccrued`, `taxSaved`, `taxResearch`, `taxCredit`, `taxClaimed`, `taxExposure`, `taxAuditDue`, `taxAuditOwed`; schema 85); older saves pay no tax until the first week plays. Only the player's lab.

## The bill

Each week the lab accrues `ratePermille` (100) of its profit above `allowancePerWeek`. Profit is the week's net without fines (not deductible) and capital flows (a loan is not income). A loss week takes back some of what the year has accrued, never below nothing. At each year end the bill, less any claimed credit, is paid and a `TaxPaid` event is logged (Pick 1 when the lab kept a structure).

## Structures

| Tier | Cost | Weeks | Share off the tax | Exposure a week |
|---|---|---|---|---|
| 1 holding company | 30,000 | 8 | 25% | 1 |
| 2 offshore | 150,000 | 20 | 60% | 5 |

A structure also costs `feePerTier` (300) a week for each tier. `BuildTaxStructure(0)` unwinds it at once. One structure is built at a time, and nothing is built or unwound while an audit is open.

## The research credit

`ClaimTaxCredit` takes `creditPermille` (12%) of the year's training spend off the bill and adds `claimExposure` (25). The spend counts again from nothing after a claim and at each year end.

## The audit

At a year end with exposure at `auditThreshold` (100) and something saved or claimed since the last audit, an audit opens over that amount. An offshore structure also costs `exposedReputation` (20, damped). Answer within `auditWeeks` (8) with `AnswerTaxAudit`:

| Answer | Cost | What changes |
|---|---|---|
| 0 cooperate | what was at stake plus 10% | the structure is unwound, exposure resets |
| 1 settle | 60% of it | the structure stays, exposure halves |
| 2 contest | nothing now | a tax claim of twice the amount goes on the docket (strength 500 plus 150 a tier) |

Unanswered, the audit is lost by default: the cooperate price plus 30%, the structure goes and exposure resets.

Events: `TaxStructureBuilt`, `TaxPaid`, `TaxCreditClaimed`, `TaxAuditOpened`, `TaxAuditClosed` (Pick 0 cooperate, 1 settle, 2 contest, 3 left unanswered).
