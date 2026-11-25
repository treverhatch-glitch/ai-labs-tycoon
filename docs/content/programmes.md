# Research programmes

A multi-stage route to one costly tech, with a branching decision at every stage (`programmes.*` in the data; Q407). Nine saved lab fields (`pgTech`, `pgDone`, `pgPhase`, `pgLeft`, `pgBold`, `pgInsight`, `pgSpent`, `pgWins`, `pgLosses`; schema 105). Only the player's lab; research points only, no cash.

| Command | What happens |
|---|---|
| `StartProgramme` (a tech id) | the tech must be real, unresearched, open in the era, with its needs done and a cost of at least 400; one programme at a time; free |
| `ChooseApproach` (1 careful, 2 bold) | pays the stage's points now and starts it |
| `RecoverStage` (1 retry boldly, 2 fall back to careful, 3 give up) | answers a failed bold stage |
| `AbandonProgramme` | gives up at any point |

Three stages. Careful: 35% of the tech's cost, 6 weeks, always works. Bold: 25% of the cost, 4 weeks, works 55% of the time (rolled when the weeks end). A failed bold stage waits for the player (no time limit) and the stage does not count. A programme that finishes gives the tech (like researching it) and pays back, for each bold stage that worked, 15% of the tech's cost in research points and 20 prestige. Giving up takes 25% of the points spent back and nothing else.

On average a bold stage costs about 45% with retries, against 35% careful, less the 15% paid back: a small edge with a lot of variance, and a lab short of points cannot afford a run of failures.

Events: `ProgrammeStarted`, `ProgrammeStageDone` (Pick 1 careful, 2 bold), `ProgrammeStageFailed`, `ProgrammeEnded` (Pick 1 finished, 0 given up; Amount the points paid back or refunded).

A stage that starts while the lab has a sharp pod runs a week shorter (Q413).
