# Hardware reliability

The GPUs the lab owns can fail, and the lab picks how to be covered (`reliability.*` in the data; Q381). Six saved lab fields (`warrantyTier`, `spares`, `failCarry`, `rmaUnits`, `rmaDue`, `gpuFailures`; schema 87). Only the player's lab. Rented GPUs and the lab's own chips do not fail.

## The chance

Each week a unit fails at `baseFailPpm` (400) in a million, plus `agePpm` (300) for each year since its generation opened, times `heatwavePermille` (2,000, so double) at a site in a heatwave. The chances of the whole fleet are added to `failCarry`; each whole million is a failure and the rest carries. The oldest generation loses a unit first. No random numbers.

## Cover

| Cover | A failed unit | Costs |
|---|---|---|
| None | lost | nothing |
| Spares (`StockSpares`) | a spare takes its place at once, as the current generation | the current price to stock, `spareUpkeep` (5) a spare a week, at most `maxSpares` (40) |
| Warranty tier 1 or 2 (`SetWarranty`) | sent back; a current-generation unit arrives after `rmaWeeksTier1` (8) or `rmaWeeksTier2` (2) weeks | `feeTier1` (10) or `feeTier2` (30) a unit a week, and `switchFee` (15,000) to raise the tier |

Spares are used first. Units sent back arrive together when the first of them is due, at the headquarters. `ExpediteReplacements` delivers what is waiting at once for `expeditePermille` (150) of its price.

Events: `GpusFailed` (Pick 0 lost, 1 swapped for spares, 2 sent back), `ReplacementsArrived` (Pick 1 when rushed), `WarrantyChanged`, `SparesStocked`.
