# The treasury reserve

Spare cash can be parked in a reserve (`treasury.*` in the data; Q349). It leaves the lab's cash, earns a small yield every week, and comes back for a fee. If the weekly bill leaves the lab in debt, the reserve pays first, at a steeper fee. One saved lab field (`reserve`, schema 67); older saves start with nothing set aside.

## `SetAsideReserve` and `DrawReserve`

| What | Rule |
|---|---|
| Least move | `minMove` (10000) dollars, either way |
| Set aside | Cash out, reserve in. The cash is on the capital line as money out, so the books balance |
| Yield | `weeklyYieldPpm` (800 parts per million of the reserve, about 4% a year), paid every week while the reserve is above 0 |
| Planned draw | `drawFeePermille` (30) of the sum drawn is lost, the rest comes back as cash |
| Debt cover | After the weekly bill, if cash is below 0 the reserve pays what clears the debt (and no more than it holds), with `coverFeePermille` (100) lost. The debt clock resets, so a covered lab is not in debt |

The reserve counts in what the lab would sell for, so it is not lost at an exit. Count on the command is dollars, so one move is at most about 2 billion.

Refused, with nothing changed, for a rival, a game with `enabled` 0, a sum under the least move and a draw over what the reserve holds (`InvalidArgument`) and a set-aside over the cash (`NotEnoughCash`).

## Notes

- Events: `ReserveMoved` (Amount the sum moved before any fee; Pick 1 set aside, 0 drawn on purpose, 2 covered a debt).
- It is a safety net: the yield is smaller than the fees, so a lab that saves for no reason loses a little.
- No random numbers. A lab that sets nothing aside is exactly as before.
