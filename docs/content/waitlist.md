# Waitlist

A public waitlist for a model that is not out yet (`waitlist.*` in the data; Q401). Five saved lab fields (`wlModel`, `wlHype`, `wlLeft`, `wlWins`, `wlFlops`; schema 101). Only the player's lab; it costs nothing to open.

| Command | What happens |
|---|---|
| `OpenWaitlist` (a model that is training or trained, 4 to 40 weeks) | the list opens; one at a time |
| `CloseWaitlist` | closes it: reputation lost, 25 per 1000 hype built (damped) |

Hype grows 25 a week (to 1000). When the model is released in time, the hype is cashed in: 40 reputation per 1000 hype. When the promised weeks run out or the model is retired first, it is spent as a loss: 80 per 1000 (damped). A short promise is safe and earns little; a long one builds more to win or lose.

Events: `WaitlistOpened`, `WaitlistEnded` (Pick 1 shipped in time, 0 lapsed, 2 closed).
