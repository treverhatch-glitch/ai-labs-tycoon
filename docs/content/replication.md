# Replication

Other labs trying to reproduce the lab's papers (`replication.*` in the data; Q397). Six saved lab fields (`replPending`, `replWait`, `replLeft`, `replRigor`, `replWins`, `replFails`; schema 99). Only the player's lab.

A paper the lab publishes (`PublishPaper`) or has accepted at a venue (`SubmitPaper`) comes up for a decision the week after (up to 3 waiting). Then:

| Command | What happens |
|---|---|
| `ReleaseCode` (0 to 3 weeks of clean-up) | pays 40 research points a week; the oldest waiting paper goes out; one at a time |
| `WithholdCode` | keeps the oldest back: 40 prestige lost (never below 0) |

A paper left more than 12 weeks is taken to have been kept back (the same cost, `Pick 1`). A paper that goes out is tried by other labs for 12 weeks. How reproducible it is: 350, plus 150 a week of clean-up, plus 30 for each person doing research (up to ten), to a cap of 1000. The bar it has to meet is 500 plus up to 300 at full prestige: a famous lab is checked harder. At or over the bar it replicates: +120 prestige, +15 reputation. Under it, it is a scandal: 200 prestige and 40 reputation lost (damped).

Events: `CodeReleased`, `CodeWithheld` (Pick 0 chosen, 1 waited too long), `PaperReplicated` (Pick 1 replicated, 0 did not).
