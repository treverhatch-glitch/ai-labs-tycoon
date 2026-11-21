# Model end-of-life notice

The player gives a released model a retirement date; on the day its customers move to the successor the model helped design (`sunset.*` in the data; Q359). Two saved model fields (`sunsetStart`, `sunsetTick`, schema 73); older saves have no notices.

## The successor is the existing one

There is no new successor logic. The successor of a model is read from successor design (Q-successor, `DesignWithModel`): a released model of the same lab whose `designerId` is the old model and whose capability is at least as high (the strongest, then the oldest). Nothing about it is saved on the old model. A model nobody helped design cannot be given a notice.

## `AnnounceSunset` and `CancelSunset`

`AnnounceSunset(labId, modelId, weeks)`: the model must be released, have no notice, a notice of `minWeeks` (4) to `maxWeeks` (26) and a successor on sale. `CancelSunset(labId, modelId)` calls it off for `cancelReputation` (20, damped by the press room).

| What | Rule |
|---|---|
| During the notice | Every week the lab pays `supportPermille` (10%) of what the model earned last week (Salaries line), to keep it running beside its successor |
| On the day | Each sector where the old model is on sale moves to the successor: the entry (and its share) simply points at the successor. Trust in each sector falls by a drop that is `trustDropShort` (150) for a 4-week notice and shrinks in a straight line to `trustDropLong` (20) at 26 weeks, and `assistedCutPermille` (50%) smaller because the successor was designed with the old model's help |
| A sector the successor cannot serve | Its capability is under the sector's bar: the entry is dropped, with `droppedPermille` (x2) the trust loss |
| After | The old model is retired (`ModelRetired`) |
| Successor gone on the day | The notice is withdrawn: `brokenReputation` (40) reputation, `SunsetCalledOff` with Pick 0 |
| Model gone before the day | A model replaced in all its sectors, pulled out or retired by hand loses its notice quietly the next week |

Refused, with nothing changed, for a rival, a game with `enabled` 0, an unknown model, a model that is not on sale, a notice already running, a notice out of range, no successor and a cancel with no notice (`InvalidArgument`, `UnknownId`, `WrongModelStatus`).

## Notes

- Events: `SunsetAnnounced` (SubjectId the model, Amount the weeks, Pick 1 when it has a designed successor), `SunsetDone` (SubjectId the successor, Amount the sectors moved, Pick 1 when none was dropped), `SunsetCalledOff` (Amount the reputation lost, Pick 1 called off, 0 successor gone).
- The validator keeps a short notice at least as costly as a long one and dropping a sector at least as costly as moving it.
- No random numbers; a lab that gives no notice is exactly as before (the golden moved only through the new saved fields, verified by taking them out).
