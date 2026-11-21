# The public roadmap

The lab can tell the world its next model will be better than its best by a stated margin and by a stated date (`roadmap.*` in the data; Q351). Four saved lab fields (`roadmapLevel`, `roadmapStart`, `roadmapDue`, `roadmapGoal`, schema 69); older saves have none open.

## `AnnounceRoadmap`

| What | Rule |
|---|---|
| Level | 1 to `maxLevel` (3). Each level asks `stretchPerLevel` (200 permille, so 20%) more capability than the lab's best finished model, rounded up and at least 1 more |
| Deadline | `minWeeks` (13) to `maxWeeks` (52) weeks, given as `TargetId` on the command |
| Hype | `hypePerLevel` (8) reputation a level, at once |
| Delivered | A model released after the announcement with the promised capability settles it the same week: `deliverPerLevel` (20) reputation a level |
| Missed | The deadline week passes with no such model: `missPerLevel` (45) reputation a level lost. A model released later gets nothing |

Models still in training do not count towards the best, so a run already under way can be the one that delivers. A next version of a line is about 30% better than the last, so level 1 is what a routine successor does; a bigger promise needs a bigger step (a better line).

Refused, with nothing changed, for a rival, a game with `maxLevel` 0, a level or deadline out of range, a roadmap already open and a lab with no finished model (`InvalidArgument`).

## Notes

- Events: `RoadmapAnnounced` (SubjectId the level, Amount the capability promised, Pick the weeks) and `RoadmapSettled` (SubjectId the level, Amount the promise, Pick 1 when delivered).
- The validator keeps `missPerLevel` above `hypePerLevel`, so announcing and missing is always a net loss.
- No random numbers; a lab that never announces is exactly as before.
