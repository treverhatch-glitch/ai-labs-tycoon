# Restructuring a loan

A loan against chips is paid off weekly, and a lab that misses three payments in a row loses the chips. The lender will stretch what is left over a longer term for a fee (`restructure.*` in the data; Q345). The weekly payment falls, a clean sheet of missed payments is given back, and the lab pays more in all. No new saved state: it edits the loan's own fields.

## `RestructureLoan`

| What | Rule |
|---|---|
| New term | `stretchPermille` (2000) of the loan term in the data, so 104 weeks when a loan is 52 |
| Fee | `feePermille` (100) of the balance, added to what is owed. No cash changes hands |
| Payment | What is then owed, divided over the new term and rounded up |
| Missed payments | Back to 0, so a lab one miss from default has room again |
| Reputation | `reputationLoss` (15) lost, with press damping |

It works only while it would lower the payment. A restructured loan is already spread over the long term, so asking again at once is refused; it works again once enough of the balance has been paid off that the longer term gives a lower payment (about ten weeks at the shipped numbers).

Refused, with nothing changed, for a rival, a game with `enabled` 0 and a loan that would not get a lower payment (`InvalidArgument`) and a loan the lab does not have (`UnknownId`).

## Notes

- It is a trade of cash flow now for money later: 10% more owed, in return for a payment that is a little over half, and the lender's patience.
- Event `LoanRestructured` (SubjectId the loan, Amount the fee, Pick 1 when it cleared missed payments).
- The books balance as before: the fee is owed, not paid, and every payment is the usual capital line.
