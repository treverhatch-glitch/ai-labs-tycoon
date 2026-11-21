# Endowed chairs

A big one-off payment buys a permanent research post at a university. It adds research points every week, puts a floor under the lab's academic prestige and costs a stipend every week for good. Each chair costs more than the one before (`chairs.*` in the data; Q341). Schema 64 adds one number to each lab.

## `EndowChair`

| What | Rule |
|---|---|
| Price | `costBase` (150000) times the chair it would be: 150000, 300000, 450000 |
| Research | `researchPerWeek` (14) points a chair, added to the lab's weekly research output |
| Prestige | A floor of `prestigeFloor` (120) a chair. Papers still add prestige on top, and prestige above the floor still decays down to it |
| Stipend | `stipendPerWeek` (1200) a chair, part of the weekly salary bill |
| Limit | `maxChairs` (3) |

Refused, with nothing changed, for a lab that is not the player's, a game with `maxChairs` 0, a full set (`InvalidArgument`), fewer than `minResearchers` (3) researchers (`NotEnoughStaff`) and not enough cash (`NotEnoughCash`).

## Notes

- Everything that reads the prestige level (hiring discount, morale, poaching, counter-offers, grants) sees the floor, because the floor is applied where the level is read.
- The validator keeps the floor times the most chairs under the top of the prestige scale, so papers still matter, and the price at least 1.
- Event `ChairEndowed` (SubjectId the chairs held, Amount the price).
- Schema 64: `chairs` on every lab (older saves: 0). Only the player's lab endows them; a save with a rival chair is flagged.
