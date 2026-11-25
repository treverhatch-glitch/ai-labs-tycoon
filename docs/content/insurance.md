# Incident insurance

The player's lab can insure against incident fines. Each level covers a share of the fine for a weekly premium; claims push the premium up, and too many end the policy. Rivals never buy insurance. Nothing happens until the player buys (`finance.insurance*` in the data; Q299).

## The command

`SetInsurance(lab, level)` takes out, changes or cancels cover (level 0 to `insuranceMaxLevel`, 3). It is refused when the lab is not the player's, is bankrupt, asks for the level it has or one outside the range, changed the policy less than `insuranceLockWeeks` (13) ago, or asks for more cover while the insurer remembers `insuranceDropClaims` (3) claims or more.

## What a level buys

| | Shipped |
|---|---|
| Cover per level | 25% of the fine (`insuranceCoverPermille` 250); level 3 covers 75% |
| Weekly premium per level | the larger of $2,000 (`insuranceMinPremium`) and 1.2% of last week's revenue (`insurancePremiumPermille` 12) |
| Loading | +30% of the premium for each claim the insurer remembers (`insuranceClaimLoadingPermille`) |
| Not covered | incidents at rung 5 and above (`insuranceExcludedRung`), the make-good payments of sector flavours, and the reputation loss |

The premium has its own ledger line (`insurance`, schema 45) so cash stays explained. A claim is logged as `InsuranceClaimPaid`.

## Claims

- Every covered incident is a claim. A claim is forgotten one at a time, every `insuranceForgetWeeks` (52).
- At `insuranceDropClaims` the insurer pays that claim, ends the policy (`InsuranceDropped`) and will not sell another until a claim has been forgotten.
- The validator rejects data in which the top level would cover more than the whole fine.

The saved fields are `insuranceLevel`, `insuranceClaims` and `insuranceLockedUntil` on the lab. See QUESTIONS Q299, [safety-culture.md](safety-culture.md) and [overload.md](overload.md) for what else moves incident risk.
