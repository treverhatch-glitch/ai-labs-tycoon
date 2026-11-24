# The offsite

The player can take the whole team away for a day. Morale rises for everyone who has room to rise, the lab's safety culture gets a small lift, and it costs about a week of salaries. It holds no state of its own, so there is no cooldown: the limit is how high morale can be taken (`staffPeople.offsite*` in the data; Q300).

## The command

`HoldOffsite(lab)` is refused when the lab is not the player's, is bankrupt, has no one on the payroll, cannot pay the cost, the data has no offsites (`offsiteMoraleLift` 0), or no one would gain anything.

## What it does

| | Shipped |
|---|---|
| Cost | one week of salaries (`offsiteSalaryPermille` 1000), on the hiring line of the ledger |
| Morale | +80 for each person (`offsiteMoraleLift`), kept under their ceiling |
| Ceiling | the person's resting morale (trait and comfort rooms included) plus 100 (`offsiteMaxAboveResting`), never over 1000 |
| Safety culture | +15 points (`offsiteCultureLift`), never over 1000 |

A person already at their ceiling stays put and is not counted. A second offsite straight after the first only gives the 20 points of room that are left, and a third is refused. Lifted morale drifts back down by one point a week like any other morale above resting.

## What it touches

- **Crunch time** ([crunch-time.md](crunch-time.md)): crunch cannot start under an average morale of 450, so an offsite or two is how a worn-out team gets going again. An offsite during crunch lifts morale and the crunch carries on.
- **Quits**: a person who quits by blog post goes under morale 300, so an offsite can keep them.
- **Research**: morale above resting raises research ([workplace.md](workplace.md)), so a lift is worth a few percent for as long as it lasts.

The event is `OffsiteHeld` (subject: how many people it lifted, amount: the cost). See QUESTIONS Q300.
