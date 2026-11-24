# Sector account teams

The player assigns salespeople to a sector where a model of theirs is on sale; the team builds familiarity and lifts that sector's appeal (`accounts.*` in the data; Q360). A saved list on the lab (`accountTeams`: sector, people, weeks; schema 75); older saves have no teams.

## `AssignAccountTeam`

`AssignAccountTeam(labId, sectorId, people)`: `people` from 0 (disband) to `maxPeople` (3).

| What | Rule |
|---|---|
| Who | Up to the lab's salespeople less `keepFree` (1), over all teams. Assigned people stop counting in the lab-wide sales force (the sales boost on every sector's appeal) |
| Where | A sector the lab has a model on sale in. One team a sector |
| Effect | `people` x `perPersonPermille` (4%) x familiarity, added to the sector's appeal as `AccountEffect` (after the campaign effect, so it multiplies everything else) |
| Familiarity | `startPermille` (30%) on the first day, in a straight line to 100% at `maxWeeks` (52) |
| Resizing | What the team built is spread over the larger of the two sizes (2 people with 20 weeks, cut to 1, keeps 10). A person moved to another sector starts that team at 0 |
| Weekly | Teams in a sector the lab has left are disbanded (Pick 4). If the lab has fewer salespeople than are assigned, the team with the least familiarity loses one person at a time (Pick 3, or disbanded, Pick 2) |

Refused, with nothing changed, for a rival, a game with `enabled` 0, an unknown sector, a size out of range, no change of size, a sector with no model of the lab on sale and more people than the lab can spare (`InvalidArgument`, `UnknownId`, `NotEnoughStaff`).

## One cap with Unity's enterprise seats

Unity's enterprise seat deals (Q208-Q214, PR #2, not in this branch) also lift a sector for a major customer. The two must not double-count, so the gain is shared:

- `AccountTeams.Effective(teamBonus, otherBonus)` is `min(teamBonus, max(0, comboCapPermille - otherBonus))`: with `comboCapPermille` 150, a team worth 80 keeps all of it beside no deal, 50 beside a deal worth 100 and none beside a deal worth 150 or more. The other deal is never reduced.
- `AccountTeams.ExternalBonusPermille(c, state, lab, sectorId)` is the one place that reads the other deal. On this branch it returns 0. On merging Unity's enterprise seats it must return the active deal's sector bonus in permille (the claims file has the merge note).

## Notes

- Event: `AccountTeamChanged` (Ref the sector name, SubjectId the weeks of familiarity left, Amount the people, Pick 1 new, 0 resized, 2 disbanded, 3 thinned, 4 sector lost).
- The validator keeps a full team under 100% and a new team always with something left to learn.
- No random numbers; a lab with no teams is exactly as before (the golden moved only through the new saved list and the schema number, verified by taking them out).
