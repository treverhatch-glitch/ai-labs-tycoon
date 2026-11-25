# Chief officers

The player can name one of the lab's own people chief scientist or chief safety officer. A chief costs a fee and a weekly premium, and gives the lab one thing; it also makes the lab a better target for rivals, and the team takes it hard when a chief goes. Rivals and the autopilot never name chiefs. Nothing happens until the player does (`chiefs.*` in the data; Q319).

## `AppointChief(title, personId)`

`title` is `scientist` (the person must be in a Research role) or `safety` (a Safety role). It costs `fee` ($30000) and is refused for a lab that is not the player's, a bankrupt lab, a game with `enabled` false, an unknown title or person, a person in the wrong kind of job, a payroll under `minStaff` (5), a person who already holds the title, and a lab that cannot pay. Naming someone else to a seat that is taken pays the fee again and takes `shockMorale` (100) off the old chief. A person id of -1 stands the sitting chief down for nothing, and costs them the same morale.

## What a chief does

| Title | Effect | Cost |
|---|---|---|
| Chief scientist | +`researchPermille` (12%) on the lab's research points | `weeklyPremium` ($1200) a week |
| Chief safety officer | -`incidentCutPermille` (15%) on the weekly chance of an incident (it shows as `FromChief` in the incident breakdown) | `weeklyPremium` ($1200) a week |
| Either | -`poachPenaltyPermille` (4%) off the lab's poach defence for each chief | |

## When a chief goes

A chief counts only while the person is still on the payroll and still in a role of the right function. Fired, poached, quit or retrained to another field, they stop counting at once. On the next week the seat is emptied, every remaining person loses `shockMorale`, and `ChiefLeft` is logged. Nobody is paid a premium for an empty seat.

## Notes

- Saved per lab: `chiefScientistId` and `chiefSafetyId` (-1 for nobody), schema 54. A save with an id below -1 is reported. An id that points at nobody is allowed: it is how a chief who left looks until the week runs.
- Events `ChiefChanged` (Pick 1 named, 0 stood down; Ref the title) and `ChiefLeft`.
