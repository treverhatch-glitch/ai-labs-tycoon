# Listing

Going public and living by the quarter (`listing.*` in the data; Q386). Eleven saved lab fields (`listed`, `listPrice`, `listGuidance`, `listTarget`, `listQuarter`, `listBase`, `listNext`, `listKeep`, `listStreak`, `listFollowOn`, `listActivist`; schema 90). Only the player's lab, and not a nonprofit.

| Step | What happens |
|---|---|
| `ListCompany` | needs 200 reputation and 50,000 a week in revenue; sells `floatPermille` (200) at 100 weeks of smoothed revenue, less a 6% fee; founders diluted; reporting costs 3,000 a week |
| `SetGuidance` 1/2/3 | first 3 weeks of a quarter only, once; target is the expected quarter times 90%, 100% or 115% |
| earnings (every 12 weeks) | beat by 3%: price up 8% times the level; met: up 2%; missed: down 15% times the level; no guidance: down 4% |
| `FollowOn` | sells 10% more at the price, every 24 weeks at most |
| `BuyBackShares` | spend thousands; the price rises, the founders own more again |
| activist | price under 450 at a report: a step (20 reputation each, 3 at most); a good quarter sends it back |
| `TakePrivate` | pays 130% of the public's shares; the founders' share comes back |

Events: `CompanyListed`, `EarningsReported` (Pick 0 missed, 1 met, 2 beat; Amount the price), `FollowOnSold`, `SharesBoughtBack`, `ActivistArrived`, `CompanyDelisted`.
