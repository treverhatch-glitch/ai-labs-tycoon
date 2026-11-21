# Loan covenants

The lender watches the chips behind the player's loans (`covenants.*` in the data; Q382). Four saved lab fields (`covBreach`, `covWaiverUntil`, `covWaivers`, `covCalls`; schema 88). Only the player's lab.

## The test

Cover is what the chips would fetch over what is owed, in permille. Below `minCoverPermille` (1,500) is a breach. Each week in breach `surchargePermille` (5) of what is owed is added to the first loan. After `cureWeeks` (8) weeks in breach the lender calls the loans.

## The ways out

| Way | Cost |
|---|---|
| `CureCovenant` | pays the loans down, oldest first, by just what restores the cover |
| `WaiveCovenant` | `waiverFeePermille` (15) of what is owed, and the lender looks away for `waiverWeeks` (26); `maxWaivers` (3) in all |
| nothing | after the cure weeks the loans are called: paid from cash if the lab has it, otherwise the chips go as in a default |
| the cover comes right | by more chips or less debt: logged as cured, nothing paid |

Events: `CovenantBreached`, `CovenantCured` (Pick 1 when the lab paid down), `CovenantWaived`, `LoansCalled` (Pick 1 when paid from cash).
