# Hack week

The player can let the researchers drop the roadmap for a couple of weeks. The team is happier and finds more, but training slows and it cannot be repeated for half a year. Rivals and the autopilot never hold one. Nothing happens until the player does (`hack.*` in the data; Q314).

## `HoldHackWeek`

It is refused for a lab that is not the player's, a bankrupt lab, a game with `weeks` set to 0, one that is already on, crunch time (and crunch time cannot start while it is on), fewer than `minResearchers` (2) researchers, a cooldown still running (`HackLocked`), an average morale under `minMorale` (300, `TeamTooTired`) and a lab that cannot pay `costPerStaff` ($800) for every person on the payroll (on the hiring ledger line, like an offsite).

## While it lasts

| What | Change |
|---|---|
| Morale | +`moraleLift` (80) for everyone when it starts, up to 1000 |
| Research points | +`researchPermille` (40%) of what the team makes, added to the other research bonuses |
| Training speed | -`trainingCutPermille` (40%) |

It lasts `weeks` (2) and ends by itself. It is logged when it starts (`HackWeek`, Pick 1) and when it ends (Pick 0). The next one can be held `cooldownWeeks` (26) after the start, which the validator keeps at or above `weeks`.

## Notes

- Saved per lab: `hackWeeks` (weeks still to run) and `hackLockedUntil`, schema 52.
- The week's research and training use the effect first and the count runs down afterwards, so a two week hack week gives two weeks of it.
- A bankrupt lab is skipped by the weekly loop, so its count stays where it was.
