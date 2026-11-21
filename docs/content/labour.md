# Labour relations

Pressure to organise builds in the player's lab; when it tips the player recognises the union, resists it, or lets the window run out (`labour.*` in the data; Q358). Four saved lab fields (`labourStage`, `labourPressure`, `labourDue`, `labourHard`, schema 72); older saves have no organising.

## Pressure

Each week while the lab has at least `minStaff` (8) people: `lowMoralePerWeek` (30) when the average morale is under `moraleBelow` (450), and `crunchPerWeek` (20) in a week of crunch time; `decayPerWeek` (10) always falls off it. A layoff round adds `layoffPerFull` (600) scaled by the share of the staff it lets go. Pressure is 0 to 1000.

## Stages

| Stage | What happens |
|---|---|
| Quiet (0) | At `drivePressure` (700) the staff organise: stage 1, `LabourDrive`, `windowWeeks` (8) to answer |
| Organising (1) | `RecogniseUnion` or `ResistUnion`. If the window runs out, it counts as resisting with no fees |
| Union (2) | A contract, until the end of the game. Pressure is 0 |
| Resisting (3) | `resistWeeks` (26). Weekly pressure added is multiplied by `resistFactorPermille` (2000). At 1000 it is a strike at once; at the end of the period, pressure under the drive level means the drive collapses (back to quiet at `heldPressure` 250), otherwise a strike |
| Strike (4) | `strikeWeeks` (3): every person loses `strikeMorale` (120) when it starts, and research and training lose `strikeCutPermille` (60%). It ends in a forced, harder contract |

## Contract

| | Recognised | Forced by a strike |
|---|---|---|
| Pay bill | +`payPermille` (8%) | +`hardPayPermille` (12%) |
| Severance (layoffs and `Fire`) | x`severancePermille` (2) | x`hardSeverancePermille` (3) |
| Research | +`researchPermille` (5%) | +`hardResearchPermille` (0) |
| Crunch time | cannot start | cannot start |
| Morale | +`recogniseMorale` (60) once | none |

Resisting costs `resistCost` ($50,000, Fines line) when the player chooses it.

Refused, with nothing changed, for a rival, a game with `enabled` 0, a drive that is not waiting for an answer and (for resisting) a lab short of the fees (`InvalidArgument`, `NotEnoughCash`).

## Notes

- Events: `LabourDrive`, `LabourRecognised` (Pick 1 chosen, 0 forced), `LabourResisted` (Pick 1 chosen, 0 by default), `LabourHeld`, `LabourStrike`.
- The validator keeps the drive level under 1000, a resisted drive harder to calm than a quiet one and the forced contract at least as hard as the chosen one.
- No random numbers; a lab nobody is angry at never organises (the golden moved only through the new saved fields, verified by taking them out).
