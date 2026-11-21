# Compute sublet

The player can let some of the lab's general compute to outside customers. Rivals and the autopilot never do. Nothing happens until the player sets a share (`sublet.*` in the data; Q309).

## `SetSublet`

It takes a share in permille from 0 to `maxPermille` (500). It is refused for a lab that is not the player's, a bankrupt lab, a share out of range, the share the lab already has, a game with `maxPermille` at 0, and a change less than `noticeWeeks` (4) after the last one (`SubletLocked`).

## What it does

- The share comes out of the lab's general compute (rented, owned and reserved GPUs, training chips) after the research share has taken its part. Inference chips are never let.
- Every week the let compute earns `revenuePermille` (500) of the rent of the GPUs it equals, on the `Sublet` ledger line (income). `SubletChanged` is logged with the share and the weekly figure.
- The let compute is gone from training and from serving. A training run gets less, so `TrainingSystem.EstimateWeeks` goes up, and `LastWeek.Compute` and `LastWeek.ServingCompute` go down, so a lab with customers can be overloaded.
- `ComputeSplit.Sublet` is the let compute. It is not part of `Total`.

| Share | General compute left | Earns a week (GPUs let at rent R) |
|---|---|---|
| 0 | 100% | 0 |
| 250 | 75% | a quarter of the GPUs, at half the rent |
| 500 | 50% | half the GPUs, at half the rent |

A cloud GPU the lab rents costs its full rent and earns at most half of it back by being let, so renting to sublet always loses; only owned hardware and idle reserved compute gain from it.

## Notes

- Saved per lab: `subletPermille` and `subletLockedUntil`, and the `sublet` field of every ledger total, schema 50.
- The validator keeps `maxPermille` at 900 or under, so the lab keeps at least a tenth of its compute, and `revenuePermille` at 1000 or under.
