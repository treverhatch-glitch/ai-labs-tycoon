# Conference submissions

Beside the instant paper (`PublishPaper`), the player can send a paper to a venue and wait for the decision (`conference.*` in the data; Q361). A saved list on the lab (`submissions`: venue, effort, points paid, decision week; schema 75); older saves have none out.

## `SubmitPaper` and `WithdrawPaper`

`SubmitPaper(labId, venue, effort)`: venue 1 workshop, 2 main conference, 3 flagship; effort 1 to 3. `WithdrawPaper(labId, index)` takes the paper at that place in the lab's list (from 0) out of review.

| What | Rule |
|---|---|
| Cost | Research points, up front: the plain paper cost x the venue multiple (1, `mainCost` 2, `flagshipCost` 4) x effort |
| Review | `workshopWeeks` 6, `mainWeeks` 12, `flagshipWeeks` 24; at most `maxPending` (3) papers at once, and the lab needs the researchers a plain paper needs |
| Standing | On decision day: the lab's prestige that day plus `effortPrestige` (100) x effort. Prestige fades 4 a week, so a long review can cost the paper |
| Bars | Workshop 0 (always), main `mainBar` 300, flagship `flagshipBar` 600 |
| Accepted | Prestige `acceptPrestige` (150) x venue number, reputation `acceptReputation` (20) x venue number, and it counts as a paper for any grant owed |
| Rejected | `refundPermille` (50%) of the points come back |
| Withdrawn | `withdrawPermille` (80%) of the points come back, nothing else |

Papers are decided oldest first, in the week their date arrives. Nothing is random.

Refused, with nothing changed, for a rival, a game with `enabled` 0 (or no papers), a venue or effort out of range, a full pipeline, too few researchers, too few research points and a withdraw of a paper that is not there (`InvalidArgument`, `NotEnoughStaff`, `NotEnoughResearch`, `UnknownId`).

## Notes

- Events: `PaperSubmitted` (SubjectId the venue, Amount the points, Pick the effort), `PaperDecided` (Amount the prestige added or the points back, Pick 1 accepted, 0 rejected, 2 withdrawn).
- The validator keeps the bars (up to 2000) and costs rising with the venue, and a withdrawal at least as kind as a rejection but never free.
- No random numbers; a lab that submits nothing is exactly as before (the golden moved only through the new saved list and the schema number, verified by taking them out).
