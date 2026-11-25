# The litigation docket

Claims against the player's lab (`litigation.*` in the data; Q371). A saved list of claims and a counter on the lab (`cases`, `caseSeq`; schema 81, shared with security); older saves have none. Rivals are never sued.

## What files a claim

| Cause | Claim | Strength |
|---|---|---|
| An incident | `incidentPermille` (2000) of what it cost the lab | 1100 less the safety culture, plus 25 for each earlier incident of the model |
| A layoff round | `dismissalPermille` (2000) of the severance paid | 1000 less average morale, plus 100 for a deep round |
| A breach of security (Q372) | `privacyClaim` (60,000) | 450, plus 250 if it leaked, plus 100 for each ransom paid, less 60 for each tier held |
| A change of charter (Q374) | set by the charter system | set by the charter system |

Every claim is at least `minClaim` (20,000). Up to `maxOpen` (4) claims are open; a further one is not filed. The strength is fixed on the day.

## What the player can do

| Command | Rule |
|---|---|
| `SettleCase` | Before the hearing. Costs `settlePermille` (400) of the claim and ends it |
| `RetainFirm` | Before the hearing, once. Costs `firmFeePermille` (150) of the claim; the firm takes `firmStrength` (350) off the strength instead of the in-house counsel's `inHouseStrength` (100) |
| `AppealCase` | After a lost hearing, once. Costs `appealFeePermille` (100) of the claim, takes `appealStrength` (150) more off and gives a second ruling after `appealWeeks` (8) |

## The hearing

`hearingWeeks` (12) after filing the claim wins when its strength less the counsel's reaches `judgeAt` (500). A claim that does not win is dropped and gives back `winReputation` (5). One that wins becomes a judgment of `judgmentPermille` (1000) of the claim, paid `appealWeeks` later with `lossReputation` (15) lost (damped by the press) unless the lab appeals. An appeal that still wins pays the judgment on top of its fee; one that does not overturns it.

Events: `CaseFiled`, `CaseJudged`, `CaseClosed` (Pick 0 settled, 1 won, 2 lost and paid, 3 overturned).
