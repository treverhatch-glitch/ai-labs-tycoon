# The whistleblower

Show more alignment than a model has and unhappy safety staff will eventually say so. The player gets a few weeks to answer; an unanswered concern leaks to the press (`whistle.*` in the data; Q342). Nothing here draws a random number. Schema 65 adds four numbers to each lab.

## What starts it

Every week, for the player's lab only, the system looks for a released model whose shown alignment stands at least `gapPermille` (150) above the real one, and for a person of the `safety` function at or below `moraleBelow` (450, under the resting 500, so crunch, layoffs and incidents count). When both hold, a count goes up by one; when either does not, it goes down by one. At `weeksToConcern` (6) a concern is raised about the worst model, with `answerWeeks` (4) to answer. Honest launches never start it, and neither does a content lab.

## `AnswerWhistle`

| Answer | What happens |
|---|---|
| 0, come clean | The shown alignment drops to the real one (so the model may no longer clear a release gate or era bar), `cleanReputation` (25) reputation is lost (press damping applies) and every safety person gains `cleanMorale` (120) |
| 1, pay for silence | `silenceFee` (250000) times the silences already bought plus one, every safety person loses `silenceMorale` (80), the gap stays and the count restarts. It will be raised again |
| Say nothing | At the deadline it leaks: `leakReputation` (60) plus `leakPerSilence` (30) for each silence bought is lost (press damping applies), the model's customers in each sector lose trust as if exposed, the shown alignment drops to the real one, the unhappiest safety person resigns, the rest lose `leakMorale` (150), and the silences reset |

A concern about a model that has since been retired or has had its gap closed (a safety audit, an incident, a different release) lapses with no event.

Refused with nothing changed for a rival, a game with `enabled` 0, no concern open, an answer that is not 0 or 1 (`InvalidArgument`) and not enough cash for the silence (`NotEnoughCash`).

## Notes

- Events: `WhistleRaised` (SubjectId model, Amount the gap), `WhistleAnswered` (Amount the cost, Pick 0 clean or 1 silence), `WhistleLeaked` (Amount the reputation lost, Pick 1 when a person resigned).
- Schema 65: `whistleWeeks`, `whistleModel`, `whistleDue`, `silenced` on every lab (older saves: 0). The save check flags a concern about a model that does not exist, negative numbers, a model with no deadline and any of them on a rival.
- The leak is never worth less than coming clean (the validator keeps `leakReputation` at or above `cleanReputation`), so staying quiet is only worth it for what the cost buys.
