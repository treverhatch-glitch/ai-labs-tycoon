# Industry standards

The industry drafts standards on its own clock. The lab can speed a draft up, hold it back, and then comply or pay for not complying (`standards.*` in the data; Q380). Seven saved lab fields (`stdLevel`, `stdDraft`, `stdSupport`, `stdRole`, `stdNext`, `stdAdopted`, `stdLapses`; schema 86); older saves start the clock on the first week played. Only the player's lab.

## The draft

`firstWeeks` (104) after the start, and `everyWeeks` (120) after each ratification, the industry starts drafting the next level (up to `maxLevel`, 3) and logs `StandardDrafted`. A draft of level n gains `industryRate` (2) support a week and is ratified at `supportNeeded` (60) times n (`StandardRatified`, Pick is the lab's part).

## The lab's part

`TakeStandardsRole` sets a part while a draft is open. It resets to observer when the draft is ratified.

| Part | Support a week | Cost |
|---|---|---|
| 0 observer | industry rate only | nothing |
| 1 member | +2 | 500 dues a week |
| 2 sponsor | +6 | 20,000 to become one, 2,500 dues a week |
| 3 obstructor | -3 (never below nothing in all, so the draft stalls) | 2,000 dues a week and 3 reputation every 13 weeks (damped) |

## Complying

A ratified level binds the first release of a model of capability `bindingCapability` (200) or more. `AdoptStandards` adopts every ratified level at `adoptCost` (30,000) for each new one and costs `upkeepPerLevel` (400) a week for each level; `DropStandards` stops the upkeep. A bound release the lab has not complied with is fined `lapseFine` (25,000) and loses `lapseReputation` (15, damped) for each missing level (`StandardLapsed`).

## What adopted levels give

Each takes `claimStrength` (30) off the strength of a negligence claim on the docket (Q371) and stands in for `safetyRelief` (1) safety person in the safeguards of a scaling policy (Q375).

Events: `StandardDrafted`, `StandardRatified`, `StandardAdopted`, `StandardLapsed`.
