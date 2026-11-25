# External safety audit

The player can pay outside auditors to check one model and publish what they find. Rivals and the autopilot never commission one. Nothing happens until the player does (`audit.*` in the data; Q312).

## `CommissionAudit`

It takes a model id. It is refused for a lab that is not the player's, a bankrupt lab, a game with `weeks` set to 0, an unknown model, a model that is not trained or released, a model that already holds a certificate, fewer than `minSafetyStaff` (2) safety people on the payroll, and a lab that cannot pay the bill.

The bill is the larger of `minCost` ($25,000) and `revenueWeeks` (3) times what the model has sold for so far. It goes on the training ledger line.

## What the auditors find

The shown score is always replaced by the real one. Then the gap that had been hidden (shown minus real, never below 0) decides the outcome:

| Gap | Outcome |
|---|---|
| up to `cleanGap` (20) | A certificate for `weeks` (52) and +`cleanReputation` (15) reputation |
| between 21 and 59 | The real score is published; no certificate, no reputation change |
| `shameGap` (60) or more | A scandal: -`shameReputation` (25) reputation, softened by a good press standing the way other reputation losses are |

Every audit also moves the lab's safety culture by `culturePoints` (4).

## While a certificate is held

If any model of the lab holds a certificate (several do not stack), every government gains `attentionReliefPermille` (15%) less attention each month, added to what a compliance office takes off, and a hearing goes better by `hearingPoints` (5) reputation. The relief shows in `AttentionGain.ComplianceSaving`.

An incident on a certified model pulls its certificate at once and logs `AuditRevoked`. The certificate also runs out after its weeks. The model can be audited again afterwards.

## Notes

- Saved per model: `auditedUntil` (the tick the certificate ends, 0 for none), schema 51.
- Events: `AuditDone` (Pick 1 clean, 0 scandal, -1 published only) and `AuditRevoked`.
