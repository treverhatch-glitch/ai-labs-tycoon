# Skunkworks

The player can pull a team out of research, safety or engineering to work on one project for a set number of weeks (`skunkworks.*` in the data; Q357). Six saved lab fields (`skunkKind`, `skunkPeople`, `skunkStart`, `skunkDue`, `skunkWork`, `skunkLockedUntil`, schema 71); older saves run no project.

## `StartSkunkworks` and `WrapUpSkunkworks`

`StartSkunkworks(labId, kind, people, weeks)`: `kind` is `research`, `safety` or `engineering`. `WrapUpSkunkworks(labId)` ends it early.

| What | Rule |
|---|---|
| Team | `minPeople` (2) up to `maxSharePermille` (50%) of that function's people, rounded down, and never everyone |
| Length | `minWeeks` (4) to `maxWeeks` (26) |
| Budget | `costPerPersonWeek` ($600) times people times weeks, paid when it starts |
| While it runs | The team is away: it stops counting in the function's output (research points, or the weighted head count behind training speed, alignment work and sales) |
| Work | Each week adds the people still on the team, up to the people the function still has, to a running person-weeks count. Losing people shrinks it |
| Pay | Research: person-weeks times a researcher's weekly points times `researchPayoffPermille` (150%). Safety: `alignmentPerHundred` (100) permille of alignment for every 100 person-weeks, up to `maxAlignment` (120), added to every finished model's true alignment. Engineering: `debtPerPersonWeek` (1) technical debt point cleared per person-week, never more than the lab owes |
| Long project | `longWeeks` (13) or more pays `longBonusPermille` (25%) extra |
| Wrap up | Pays `earlyPermille` (50%) of the work so far, with no long bonus; refused before the team has done any work |
| After | The next project cannot start for `lockWeeks` (13) |

A safety project needs a finished (trained or released) model and an engineering project needs some debt to clear. Only the first people of the function (in the order the lab lists them) count as away, so seniors on the list come out first.

Refused, with nothing changed, for a rival, a game with `enabled` 0, an unknown kind, a team or length out of range, a project already running, the cooldown and a lab short of the budget (`InvalidArgument`, `NotEnoughCash`).

## Notes

- Events: `SkunkworksStarted` (SubjectId the kind number, Amount the people, Pick the weeks) and `SkunkworksDone` (Amount what it paid, Pick 1 when it ran its course, 0 when wrapped up early).
- The validator keeps early wrap-ups below full pay, a minimum cost and a team that never empties a function.
- No random numbers; a lab that never starts a project is exactly as before (the golden moved only through the new saved fields, verified by taking them out).
