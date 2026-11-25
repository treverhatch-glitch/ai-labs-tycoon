# Revenue-based financing

One cash advance against future revenue (`rbf.*` in the data; Q387). Seven saved lab fields (`rbfAdvance`, `rbfOwed`, `rbfBase`, `rbfShare`, `rbfTier`, `rbfWeeks`, `rbfSlump`; schema 91). Only the player's lab.

| Tier | Share of each week's revenue | Total repaid |
|---|---|---|
| 1 | 5% | 140% of the advance |
| 2 | 10% | 130% |
| 3 | 20% | 120% |

`TakeRevenueAdvance` asks for tier and a share of the largest advance (ten weeks of last week's revenue, at least 10,000 a week), one at a time. A slump week (revenue under half of what it was at the start) adds 2% of the advance to what is owed. `BuyOutAdvance` after 12 weeks pays 95% of what is left. The cash and the sweeps are on the capital line, so they are neither income nor cost in the tax base (`docs/content/tax.md`).

Events: `RevenueAdvanceTaken` (SubjectId the tier), `RevenueAdvanceCleared` (Pick 0 swept out, 1 bought out), `RevenueAdvanceStretched`.
