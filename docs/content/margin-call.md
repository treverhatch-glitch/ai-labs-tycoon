# Margin call (draft, batch 16)

Closes the bible funding-ladder tail in [loans.md](loans.md): when collateral resale falls below the loan covenant, the lender can demand GPUs or trigger default early.

Planned schema **144** (Q468): weekly check on player labs with open chip-backed loans; uses existing `FleetResaleValue` and `Loan` rows — no site-map collateral.