# Mentoring

The player can pair a senior with a junior in the same function (`mentoring.*` in the data; Q356). Two saved fields on a person (`mentorId`, `mentorWeeks`, schema 71); older saves have nobody mentored.

## `AssignMentor` and `EndMentoring`

`AssignMentor(labId, mentorId, menteeId)` starts a pair. `EndMentoring(labId, menteeId)` calls it off.

| What | Rule |
|---|---|
| Who | A senior (mentor) and a person who is not senior (junior), both in the same function. One junior a mentor, one mentor a junior, at most `maxPairs` (2) pairs at once |
| Cost while it lasts | The mentor counts as a plain person (1000 permille) instead of the senior weight (1600), and the junior counts as `menteeWeightPermille` (800). Both are read by the research output and by the weighted head counts for engineering, safety and sales |
| Junior's morale | +`moralePerWeek` (4) a week, up to `moraleCeiling` (800) |
| Graduation | After `weeks` (26) weeks, when the function has a free senior seat (the usual share rule), the junior becomes senior for no fee. Their morale gets +`graduateMorale` (100) and the mentor's +`mentorMorale` (60). With no seat free the pair keeps going, and keeps costing output, until a seat opens or the player calls it off |
| Breaking up | Either person leaving, the mentor no longer being senior, the junior being promoted another way, or the two no longer sharing a function ends the pair at the next weekly step. The weeks are lost |

The new senior then draws the senior raise like any other (`seniority.raisePermille`), so a free promotion is not a free person.

Refused, with nothing changed, for a rival, a game with `enabled` 0, unknown ids, the same person twice, a mentor who is not senior, a senior junior, people in different functions, a junior who already has a mentor, a mentor who already has a junior and a full set of pairs (`InvalidArgument` or `UnknownId`).

## Notes

- Events: `MentoringStarted` (SubjectId the junior, Amount the mentor, Ref the junior's role) and `MentoringEnded` (Amount the weeks held, Pick 0 graduated, 1 broke up, 2 called off).
- Seniors still have to fit the share the data allows, so mentoring does not get round the seat limit; it is a cheaper road to a seat, paid for in output over half a year.
- No random numbers; a lab that never pairs anyone is exactly as before (the golden moved only through the two new saved fields, verified by taking them out).
