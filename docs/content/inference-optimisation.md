# Inference optimisation

The engineers can spend time making serving cheaper. Each level cuts the compute the lab needs to serve its customers, which pays the moment the lab cannot serve them all, and makes the models a little more brittle. Rivals and the autopilot never do it. Nothing happens until the player does (`optimise.*` in the data; Q321).

## `SetOptimisation(level)`

Level 0 to `maxLevel` (3). Raising pays `setupCostPerLevel` ($60000) for each level added and needs `engineersPerLevel` (2) engineers on the payroll for each level (`NotEnoughStaff`). Lowering is free, but the work is lost and has to be paid for again. It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` 0, the level it already has, and a lab that cannot pay.

## What it does, for each level

| What | Per level |
|---|---|
| The compute needed to serve customers | -`demandCutPermillePerLevel` (10%), applied to the lab's whole demand before the lab's serving compute is set against it |
| The weekly incident chance of every released model | +`incidentPermillePerLevel` (8 permille), shown as `FromOptimisation` in the incident breakdown, before a chief safety officer's cut |

A lab that already serves everyone gains nothing in revenue, only the risk. A lab that is short of compute serves a larger share, earns more and has a smaller overload term in its incident chance.

## The engineers behind it

The level that counts is the set level held down by what the engineers cover. If the engineers leave, the level stays set (and keeps its saved value) but the effect falls to what the people left can run, and it comes back when they are hired again. There is no weekly fee.

## Notes

- Saved per lab: `optimiseLevel`, schema 55. A level outside the data is reported.
- Event `OptimisationSet` (Pick 1 up, 0 down).
- The serving demand a lab shows (`LastWeek.ServingDemand`) is the demand after the cut.
