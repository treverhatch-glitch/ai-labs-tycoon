# The scaling policy

A promise to stop when the lab's models get too capable without the safeguards to match (`scaling.*` in the data; Q375). Four saved lab fields (`policyLevel`, `policyLockedUntil`, `policyHold`, `policyBreaks`; schema 84); older saves have no policy. Only the player's lab.

## Levels and tripwires

`AdoptPolicy` sets the level, 0 to `maxLevel` (3). Raising it costs `adoptFee` (10,000) for each level up. Lowering it costs `walkBackReputation` (25, damped by the press) and is refused until `lockWeeks` (26) after the last change. Level k has a tripwire at capability `firstTripwire` (400) plus `tripwireStep` (400) for each level above the first.

## The hold

A trained model (not yet on sale) whose capability reaches a tripwire of the policy, at or below the lab's level, is held when the lab lacks that level's safeguards: a security tier (Q372) of k and `safetyPerTier` (2) safety people for each of its k levels. While a model is held:

| What | Rule |
|---|---|
| `ReleaseModel` | Refused for the held model |
| `StartTraining` | Refused for every new run |
| The week | `TripwireHit` when a hold begins (SubjectId is the model, Amount the level) and `TripwireCleared` when it ends (Pick 1 when the safeguards stand) |

The lab can put the safeguards in, lower the policy, or `BreakPolicy`: the level drops below the lowest tripwire the model has reached, the lab loses `overrideReputation` (40, damped), the safety staff lose `overrideMorale` (300) and `policyBreaks` counts it. A break also locks the lowered level for `lockWeeks`.

## What a policy gives

Each level takes `dampPermille` (60) off the reputation a scandal costs (the press's damping) and `claimStrength` (40) off the strength of a negligence claim on the litigation docket (Q371).

Events: `TripwireHit`, `TripwireCleared`, `PolicyChanged` (Ref is "raise", "lower" or "break").
