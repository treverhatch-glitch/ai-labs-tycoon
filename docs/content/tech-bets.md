# Tech bets

A public promise to research one tech within a number of weeks (`techBets.*` in the data; Q400). Four saved lab fields (`tbRef`, `tbLeft`, `tbWins`, `tbLosses`; schema 100). Only the player's lab; it costs nothing to make.

| Command | What happens |
|---|---|
| `BetOnTech` (a tech id) | the tech must be real, not researched, open in the era, with its needs researched and a cost of at least 100 research points; one bet at a time; 48 weeks |
| `WithdrawTechBet` | takes the promise back: 10 reputation lost (damped) |

Researching the tech in time keeps the promise: +25 reputation and +60 prestige. When the weeks run out first, 30 reputation is lost (damped). Taking it back costs less than missing it.

Events: `TechBetMade` (Ref the tech), `TechBetEnded` (Pick 1 kept, 0 the weeks ran out, 2 taken back).
