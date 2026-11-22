# Board motions

How a board with investors holds the founders to their word (bible ch. 5, the board and its confidence; `boardAgenda.*` in the data; Q441). Thirteen saved fields on the lab (`meetingsOn`, `boardTension`, the motion on the table, the commitment, the next meeting and four counters; schema 124). Only the player's lab. It differs from the revenue targets of the board: those ask for money, these ask for conduct. Nothing happens until the lab holds meetings.

`HoldBoardMeetings` needs an investor seat on the board. Every 13 weeks the board tables a motion, the kinds in turn: **calm** (no new incident in any model), **runway** (cash 10% above what it is now) and **safety** (one more safety person).

- `AnswerMotion` accepts it (a commitment judged 26 weeks later, and 60 of the tension goes at once) or declines it (+120 tension). Three weeks without an answer count as ignored (+180).
- A kept commitment takes 150 off the tension, a broken one adds 250. Independent directors take 15% off every increase each, at most 80%.
- From a tension of 700 the board can force a vote on the founders: the founders then need 450 plus 30% of the tension (out of 1000) of the control to keep the company. Dual-class shares or a non-profit parent keep them safe; a lab that sold off its shares for funding is not. A lost vote is the ousted ending and the meetings stop.
- A calm board (tension at 200 or below) that has seen one commitment kept adds 10% to the cash of a funding round.
- The meetings stop only when no motion or commitment is open.

Events: `MotionTabled`, `MotionAnswered` (Pick 1 accepted, 0 declined, 2 ignored), `MotionJudged`, `BoardForcedVote`.
