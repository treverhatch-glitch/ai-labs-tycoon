# Free tier

A free tier of the lab's best released model (`freeTier.*` in the data; Q403). Five saved lab fields (`ftLevel`, `ftReach`, `ftWeeks`, `ftSpent`, `ftEarned`; schema 103). Only the player's lab; nothing to pay to open it.

| Command | What happens |
|---|---|
| `OpenFreeTier` (level 1, 2 or 3) | needs a released model; one tier at a time; reach starts at 0 |
| `CloseFreeTier` | shuts it: reputation lost, 30 per 1000 reach (damped); reach goes back to 0 |

| Level | Most extra revenue | Reach a week | Serving cost a week |
|---|---|---|---|
| 1 | 8% | 40 | 15,000 |
| 2 | 15% | 80 | 45,000 |
| 3 | 21% | 120 | 110,000 |

The extra revenue is the ceiling scaled by the reach (of 1000), added to the week's market revenue after any platform deal. The serving cost is the level's cost raised by the reach (double at full reach), booked as upkeep. When the lab has no released model left the tier closes with no penalty.

Events: `FreeTierOpened`, `FreeTierClosed` (Pick 1 shut by the lab, 0 the model went).
