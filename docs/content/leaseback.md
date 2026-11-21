# Sale-leaseback

Selling owned head-office GPUs and leasing them straight back (`leaseback.*` in the data; Q389). Seven saved lab fields (`leaseUnits`, `leaseValue`, `leaseRent`, `leaseBuyback`, `leaseLeft`, `leaseMissed`, `leaseExtended`; schema 93). Only the player's lab; one lease at a time.

| Command | What happens |
|---|---|
| `SaleLeaseback` (count) | 90% of the units' resale value now; rent 0.3% a week of that value (GPU rent line); 96 weeks |
| `BuyBackLease` | pays 110% of the value at the sale; units free |
| `ExtendLease` | once: 48 weeks more at 125% of the rent |
| `ReturnLease` | hands the oldest units back now |

At the end of the term the units go back. Leased units cannot be sold (`SellGpus` counts only the rest), and their value at the sale comes off the loan collateral (`Loans.Collateral`). Three weeks of missed rent in a row: repossession, 25 reputation lost (damped). Units lost another way (failures, theft) shrink the lease.

Events: `GpusLeasedBack`, `LeaseBoughtBack`, `LeaseEnded` (Pick 0 term, 1 early, 2 repossessed), `LeaseRentMissed`, `LeaseExtended`.
