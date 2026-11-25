# Employer brand

The lab's name as a place to work (`brand.*` in the data; Q402). Five saved lab fields (`brandOn`, `brandScore`, `brandReview`, `brandLow`, `brandReviews`; schema 102). Only the player's lab. Nothing is scored until the lab sets up a careers page.

| Command | What happens |
|---|---|
| `OpenCareersPage` | costs 50,000; the score starts at 500 |
| `AnswerReview` (1, 2 or 3) | 1 own up and fix it: 5,000 cash per person and +200 score; 2 publicly rebut it: reputation lost (12, damped); 3 leave it: -60 score |

The lab's own events of the week before move the score: a promotion, equity grant, mentoring start, sabbatical or internship adds 8; a layoff round, resignation or strike takes away 30. The score cuts every hire fee by up to 20% at 1000 and adds up to 20% at 0 (through the same fee breakdown as the founding choices).

A tech bet that was kept or a waitlist that shipped in time adds the same 8; a missed bet or a lapsed waitlist takes away 30 (Q406).

Under a score of 250 one bad review appears (once, until the score is back over the bar) and waits 4 weeks. If nobody answers it counts as left alone (-60, `Pick 0`).

Events: `CareersOpened`, `BadReview`, `ReviewAnswered` (Pick 1 fixed, 2 rebutted, 3 left, 0 nobody answered).
