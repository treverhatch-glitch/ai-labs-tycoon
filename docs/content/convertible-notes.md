# Convertible notes

A bridge that turns into shares (`convertibleNotes.*` in the data; Q390). Seven saved lab fields (`noteOwed`, `notePrincipal`, `noteRate`, `noteDiscount`, `noteLeft`, `noteExtended`, `noteTier`; schema 94). Only the player's lab; one note at a time.

| Tier | Weekly interest | Discount | Weeks |
|---|---|---|---|
| 1 | 0.2% | 10% | 96 |
| 2 | 0.4% | 20% | 72 |
| 3 | 0.6% | 30% | 48 |

- `IssueNote` (tier, thousands, at most 5,000): cash now through the capital line; interest compounds into what is owed.
- A funding round converts it at the round's value (cash over dilution) less the discount, and `FundingSystem.ApplyRaise` dilutes the founders by the round and the note together. A listing converts it at the listing's value (`Notes.OnListing`, called from `Listing.List`). At most 40% of the lab.
- `RepayNote` pays what is owed. `ExtendNote` once: 24 weeks more, the rate times 1.5, 5% more discount.
- At maturity it converts at a 40% discount against the greater of the lab's value and twice what is owed.

Events: `NoteIssued`, `NoteConverted` (Pick 0 round, 1 listing, 2 maturity; SubjectId the share in permille), `NoteRepaid`, `NoteExtended`.
