# Retraining

The player can move one person to another role instead of letting them go and hiring someone new. Rivals and the autopilot never do. Nothing happens until the player does (`retrain.*` in the data; Q316).

## `RetrainStaff`

It takes a person id and a role id. It is refused for a lab that is not the player's, a bankrupt lab, a game with `enabled` false, an unknown person or role, the role the person already has, a person whose morale is under `minMorale` (300, `TeamTooTired`), a new role with no free seat (`NoSeats`) and a lab that cannot pay the fee.

| | Fee (share of the new role's hire fee) | Morale lost |
|---|---|---|
| Different function | `feePermille` 50% | `moraleDrop` 120 |
| Same function (for example engineer to labeller) | `sameFunctionFeePermille` 25% | `sameFunctionMoraleDrop` 40 |

The fee is the lab's hire fee for the new role (era fee, recruiter and prestige discounts and the founder's edge all apply) and goes on the hiring ledger line. The person keeps their trait and id; the old seat is freed and the weekly salary becomes the new role's, with the pay policy and equity applied as usual.

## Other effects

- A person moved out of a listening role (`culture.listeningRoles`) into a role that is not one costs the lab culture, as letting one go does (`control.firedListenerPoints`). Moving between listening roles costs none.
- Everything that counts staff (a compliance office's policy people, an audit's safety staff, a hack week's researchers) sees the new role from the next check.
- A second move straight after the first is refused while the person is below `minMorale`; that is the only limit on how often a person can be moved.

## Notes

- No new saved state: the move changes the person's `roleId`, so the save schema did not change.
- Event `StaffRetrained` (Pick 1 inside a function, 0 across functions; Ref `oldRole>newRole`).
