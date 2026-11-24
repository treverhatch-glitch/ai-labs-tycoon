<!-- Generated from Rules/Data/default-config.json (culture). The data file is the source of truth: edit it, then update this page. -->
# Safety culture and the release gate

The bible's chapter 10 lists what builds control: evaluations, interpretability, sandboxes, red teams, **release gates** ("rules you set for yourself and then have to obey") and **culture** ("whether staff report problems or hide them"). This page is the last two.

## Culture

A number from 0 to 1000 for each lab, starting at 500. Only the player's labs keep one that moves; rivals stay where they started. It is saved as a shift from the starting level (save schema 29).

**What it does.** Above 500 it adds to the share of incidents the lab catches before they ship, below it takes from it: 2% of catch for every 100 points, so 1000 is +10% and 0 is -10%. It adds to the red-teamers', the rooms' and the traits' share, and the whole stays between 0 and 90%.

**What moves it.**

| Event | Points |
| --- | --- |
| Every week, for each person in `safety`, `red-teamer`, `alignment-fellow` | +1 each, at most +6 |
| Every week, whatever else happens | -2 |
| A model goes on sale for the first time showing under 500 alignment (a rushed release) | -40 |
| A model goes on sale for the first time through a gate the lab set, at or above the rush line | +10 |
| The lab loosens or removes its own gate | -60 |
| A customer's complaint is answered | +10 |
| A customer's complaint lapses unanswered | -10 |

So three people on the safety side keep it level (3 - 2 = +1 a week), none lose 100 points a year, and firing the team lets it slide. A rushed release is one event per model: adding sectors to a model already on sale does not count again.

## Bands

| Id | Name | From | |
| --- | --- | --- | --- |
| `cover-up` | Cover-up | 0 | Bad news is stopped before it reaches the door. The red-teamers have given up writing things down. |
| `hurried` | Hurried | 200 | Everyone knows about the problem. Nobody is sure it is their problem. The launch date is on the wall. |
| `steady` | Steady | 400 | Concerns get raised and mostly get read. The usual amount of shipping anyway. |
| `careful` | Careful | 600 | Someone has a standing meeting called 'what could go wrong'. People go to it. |
| `open-book` | Open book | 800 | A junior engineer flags a bug in the CEO's favourite model and gets a thank-you note. |

## The release gate

The player sets a gate with `SetReleaseGate`: the least alignment a model may **show** (its displayed meter, the one the law checks too) and still go on sale, up to 900. 0 means no gate. Then the lab has to obey it:

- A release of a model showing less is refused (`BelowReleaseGate`). Claiming a better meter at the door does not help: the claim is applied after the check, as with the law's bar. The higher of the gate and the law's bar wins.
- A gate can be raised at any time. It cannot be loosened (or removed) until 26 weeks after it last changed (`GateLocked`), and then loosening costs 60 culture. Raising it starts the lock again.
- The autopilot and the rivals never set one.

Nothing derived is saved: the level, the band, the catch and the lock date are worked out from three lab numbers (`cultureShift`, `releaseGate`, `gateSetTick`).
