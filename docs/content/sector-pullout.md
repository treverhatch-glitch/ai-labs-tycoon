# Pulling out of a sector

A lab can walk away from one sector's customers while its model stays on sale elsewhere (`pullout.*` in the data; Q344). It costs a break fee, trust, reputation and the share the model had built; in return the load that sector put on the lab's compute and the revenue the model's incidents are fined against leave with it. No new saved state.

## `PullOutOfSector`

| What | Rule |
|---|---|
| Fee | `feeWeeks` (4) times what the model last earned in that sector in a week, paid to the customers (Fines line) |
| Trust | `trustDrop` (100) lost in that sector only, not carried to its family |
| Reputation | `reputationLoss` (10) lost, with press damping |
| Share | The sector entry is removed, so the share built up is gone. Going back in with `ReleaseModel` starts again from nothing |
| Last sector | A model with no sector left is retired |

Refused, with nothing changed, for a rival or a game with `enabled` 0 (`InvalidArgument`), a model the lab does not have or a sector that does not exist (`UnknownId`), a model that is not on sale (`WrongModelStatus`), a sector the model is not sold in or more than one sector named (`InvalidArgument`) and not enough cash for the fee (`NotEnoughCash`).

## Notes

- A model that is losing money to a sector (a heavy load for a thin share) is the reason to leave; the cost is a month of what it earned there, not what it might.
- Event `SectorPulledOut` (SubjectId the model, Amount the fee, Pick 1 when it retired the model, Ref the sector name); a retired model also logs `ModelRetired`.
- Nothing is saved that was not before: the entry and trust record already were, so the golden does not move.
