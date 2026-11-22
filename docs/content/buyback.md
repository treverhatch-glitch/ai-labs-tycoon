# Buying a round back

Paying the investors of a funding round to leave, by a board vote (bible ch. 5, "Control of your own company"; `buyback.*` in the data; Q430). Four saved lab fields beyond the list (`boughtRounds`, `boardVoteWeek`, `boardWon`, `boardLost`; schema 118). Only the player's lab. A lab that never asks keeps every round and its board never votes.

`BuyBackRound` (the round id) is refused for a round the lab never raised, one already bought, when the lab cannot pay the price, or while the board is shut. Otherwise the board votes.

- **Votes follow the equity.** The founders carry what governance gives them (a plain lab its share; dual-class four times that; a non-profit parent all of it). Each round the lab still holds carries its dilution. Each independent director carries 60.
- **How each side votes.** The founders vote yes. The round being bought votes no. The other investors vote yes unless the price is more than 40% of the lab's cash. Directors vote yes only if the lab keeps 26 weeks of running costs after paying.
- It passes with more than half of all votes.
- **Passed:** the price is twice the cash the round brought, plus 1 permille for each permille of bubble pressure. The founders get that round's dilution back and the board loses its seats, so every later vote has fewer investor votes and a bigger founder share. The market gives 2 reputation points. A bought round cannot be raised again.
- **Refused:** nothing is paid, and the board will not hear another buyback for 8 weeks.

Events: `BoardVoted` (Pick 1 passed, 0 refused; Amount the yes votes in permille), `RoundBoughtBack`.
