# Buying a round back

Paying the investors of a funding round to leave (bible ch. 5, "Control of your own company"; `buyback.*` in the data; Q430). One saved lab field (`boughtRounds`; schema 118). Only the player's lab. A lab that never asks keeps every round.

`BuyBackRound` (the round id) is refused for a round the lab never raised, one already bought, or when the lab cannot pay.

- The price is twice the cash the round brought, plus 1 permille of that for every permille of bubble pressure: buying out investors in a bubble costs more.
- The founders get that round's dilution back (the share is divided by what the investors kept, up to 100%), and the board loses the seats the round gave.
- The market gives the lab 2 reputation points for it.
- A bought round stays on the lab's list, so it cannot be raised again. The cash goes out on the capital line.

Event: `RoundBoughtBack`.
