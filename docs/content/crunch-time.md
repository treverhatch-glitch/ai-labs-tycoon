# Crunch time

The player can ask the team to work late. Research and training go faster, and the team pays for it in morale, incident risk and, if it goes on, a worn-out ending. Rivals never crunch. Nothing happens until the player asks (`staffPeople.crunch*` in the data; Q298).

## The command

`SetCrunch(lab, on)` starts or ends it. It is refused when:

- the lab is not the player's;
- the lab is bankrupt;
- crunch is already on (to start) or not on (to stop);
- `crunchMaxWeeks` is 0 (the data has no crunch time);
- there is no one on the payroll;
- the team's average morale is under `crunchMinMorale` (450): "too tired".

## What it does, while it lasts

| Effect | Amount (shipped) |
|---|---|
| Research the whole team makes | +25% (`crunchResearchPermille` 250), added to the compute-share bonus |
| Training speed | +150 permille on top of techs, engineers and rooms (`crunchTrainingPermille`) |
| Morale, every person, every week | -8 (`crunchMoraleDrop`), then the 1-point weekly drift pulls back |
| Weekly incident chance of a released model | +2 permille for each week in crunch, up to +40 (`crunchIncidentPerWeek`, `crunchIncidentCap`); shown as `FromCrunch` in the incident breakdown |

Quitting follows morale as it always did: a person with the "quits by blog post" trait goes when their morale falls under 300.

## How it ends

`CrunchEnded` is logged with the weeks worked and a reason:

| Reason | When |
|---|---|
| `stopped` | The player ended it. |
| `tired` | The team's average morale fell under `crunchBreakMorale` (250). |
| `spent` | It reached `crunchMaxWeeks` (26). |
| `empty` | No one was left on the payroll. |

Morale comes back at the drift (1 point a week), so a full 26-week crunch leaves the team under the 450 needed to start another, and the next one has to wait. The saved field is `crunchWeeks` (schema 44): 1 in the first week and one more each week; 0 when not in crunch.

See QUESTIONS Q298 and [workplace.md](workplace.md) for morale.
