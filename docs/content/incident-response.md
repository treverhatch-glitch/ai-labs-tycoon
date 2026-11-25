# Incident response (the on-call rota)

The player can put safety people and engineers on an on-call rota. When one of the lab's models has an incident, the rota softens what it costs: a smaller fine, less reputation lost and a smaller blow to the team's morale. The price is a weekly fee and a team that never quite switches off. Rivals and the autopilot never do it. Nothing happens until the player does (`onCall.*` in the data; Q323).

## `SetOnCall(level)`

Level 0 to `maxLevel` (3). Raising needs `staffPerLevel` (2) safety people and engineers, counted together, on the payroll for each level (`NotEnoughStaff`). There is no setup fee. Any change locks the rota for `lockWeeks` (8) (`OnCallLocked`), up or down. It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` 0, a level outside the data, and the level it already has.

## What it does, for each level that counts

| What | Per level |
|---|---|
| The fine for an incident | -`fineCutPermillePerLevel` (10%), applied before insurance pays its share |
| The reputation lost in an incident | -`reputationCutPermillePerLevel` (10%), after the press and any ethics board have had their say |
| The morale every person loses when an incident lands | -`moraleDropCutPermillePerLevel` (25%) |
| Where the team's morale settles | -`moralePenaltyPerLevel` (3 points) |
| The weekly bill | +`weeklyCostPerLevel` ($1500), on salaries |

The cuts are capped (900 permille for the fine and the reputation, 1000 for the morale drop). The rota never changes how often an incident happens, only what it costs when one does.

## The people behind it

The level that counts is the set level held down by the people on the payroll. If they leave, the level stays set but the effect, the penalty and the bill fall to what the people left can staff, and come back when they are hired again.

## Notes

- Saved per lab: `onCallLevel`, `onCallLockedUntil`, schema 56 (`MigrateTo56`). A level outside the data is reported.
- Event `OnCallSet` (Pick 1 up, 0 down).
- Stacks with an ethics board, a chief safety officer and insurance: each takes its share in turn.
