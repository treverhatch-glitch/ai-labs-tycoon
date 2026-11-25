# Research focus

The player can pick one direction for the lab's research. Techs in it cost less research, techs in the other directions cost more. Rivals and the autopilot never pick one. Nothing changes until the player does (`focus.*` in the data; Q313).

## `SetResearchFocus`

It takes a focus id, or an empty id to drop the focus. It is refused for a lab that is not the player's, a bankrupt lab, a game with no options, an unknown id, the focus the lab already has, a change inside the lock, and a lab that cannot pay `switchCost` ($50,000, on the training ledger line). Every change, including dropping the focus, costs the same and starts a lock of `lockWeeks` (26) weeks.

## Directions

| Id | Name | Tech effects it covers |
|---|---|---|
| `frontier` | Frontier push | `CapabilityPermille`, `UnlockLine` |
| `alignment` | Alignment first | `AlignmentPermille` |
| `efficiency` | Efficiency | `TrainingSpeedPermille`, `CoolingPermille`, `SoftwarePermille`, `UnlockChipTarget` |

A tech is in a direction when any of its effects is one the direction covers, so a tech can be in two (RLHF is both frontier and alignment). A tech with no effect in any direction (some chip techs) never changes price.

## The price

`TechCost` first applies the discount for a tech a publishing rival makes cheaper, then the focus: `discountPermille` (200) off a tech in the chosen direction, `penaltyPermille` (100) on top for a tech in any other direction. A tech never costs less than one point. Rivals pay what the tech says.

## Notes

- Saved per lab: `researchFocus` (id, empty for none) and `focusLockedUntil`, schema 52.
- A focus id the data no longer has is ignored when pricing and reported as a save problem.
- Event `FocusChanged` (Pick 1 for a new focus, 0 for dropping it).
