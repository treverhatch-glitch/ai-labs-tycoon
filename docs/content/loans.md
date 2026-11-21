<!-- Generated from Rules/Data/default-config.json (loans). The data file is the source of truth: edit it, then update this page. -->
# Loans against chips

The bible's funding ladder (chapter 9) lists "Debt: loans against your chips. Repayment on schedule, whatever the chip is worth by then." The player can now borrow against the GPUs the lab owns (see QUESTIONS Q278 to Q280). Only the player borrows; rivals and the autopilot never do.

## Borrowing

`Command.TakeLoan(labId, permilleOfRoom)` borrows that share of the room the lab has left.

- **Collateral** is what the lab's owned GPUs would fetch today (`LabQueries.FleetResaleValue`: resale falls a generation at a time). Rented and reserved compute and chips of the lab's own design do not count.
- **Room** is 50% of the collateral (`loanToValuePermille` 500) less everything still owed on open loans.
- The loan is the share of the room asked for, at least `minPrincipal` ($5,000). At most `maxOpen` (3) loans are open at once.
- **Owed** is the principal plus **8%** interest added once (`interestPermille` 80). It is paid in equal weekly payments over **52 weeks** (`termWeeks`), rounded up.
- Refused, changing nothing: an amount outside 1 to 1000 permille or a rival's lab (`InvalidArgument`), too many loans open or chips that back less than the smallest loan (`LoanLimit`).

`Command.RepayLoan(labId, loanId)` pays what is owed in one go (`NotEnoughCash` if the lab cannot, `UnknownId` for no such loan). The interest is flat, so paying early saves nothing; it frees room.

## Paying, missing and defaulting

Payments come out after the week's bills (`Loans.RunWeek`, called from `Finance.RunWeek`). If the lab does not have the payment in cash, that is a miss:

- the payment is not made, **10%** of it is added to what is owed (`missPenaltyPermille`), and the first miss of a run is logged (`LoanMissed`);
- a payment made clears the run of misses;
- **3** misses in a row (`defaultAfterMisses`) is a default. The lender takes the most valuable GPUs first, as many as it needs to cover what is owed at today's resale price, and writes off the rest (`LoanDefaulted`: SubjectId the GPUs taken, Amount what was written off). The loan closes.

There is no margin call. When the chips lose value the loan stays as it was, the room can drop to nothing, and the lab keeps paying: "whatever the chip is worth by then". A lab that goes bankrupt loses its loans with everything else.

## Books

The money in and the payments out are on the capital line of the ledger (`CapitalIn`), so the proceeds are positive and the payments negative there, and cash still equals the founding cash plus the ledger.

## Notes

- Saved per lab: `loans` (id, principal, balance, payment, misses, start tick), schema 41. `SaveGame.Problems` flags a loan that owes nothing.
- Open: loans against chips the lab designed, a margin call, a lender with a face, loans for rivals, a screen (Unity), what the debt does to a sale price or an exit.
