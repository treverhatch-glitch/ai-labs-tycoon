# Sabbaticals

A person sent away on full pay (`sabbatical.*` in the data; Q367). Two saved fields on each person (`leaveDue`, `leaveMorale`; schema 79); older saves have nobody away.

## `SendOnSabbatical` and `RecallFromSabbatical`

`SendOnSabbatical(labId, personId)` sends a person away for `weeks` (8). At most `maxAway` (2) people are away at once, and nobody in a mentoring pair can go. While away the person does no work (the same way a skunkworks team does), is not counted in their function and cannot quit. Refused for a rival, a game with `enabled` 0, an unknown or already-away person, a mentoring pair and a lab with the maximum away (`InvalidArgument`, `UnknownId`).

How they come back depends on their morale the day they went:

| Morale then | Comes back | Morale |
|---|---|---|
| Below `burnoutBelow` (350) | refreshed | `refreshedMorale` (850) |
| Above `restlessAbove` (650) | restless | `restlessMorale` (400) |
| In between | rested | where people settle plus `restedBonus` (+100) |

`RecallFromSabbatical(labId, personId)` calls them back early with the morale they left with.

## Notes

- Events: `SabbaticalStarted` (SubjectId the person, Amount weeks, Ref role) and `SabbaticalEnded` (Amount morale now, Pick 2 refreshed, 1 rested, 0 restless, -1 recalled).
- No random numbers. The point is who to send: it helps the worn-down and hurts the happy.

Someone already away on a skunkworks team, an account team or the support desk cannot be sent on sabbatical (Q370).
