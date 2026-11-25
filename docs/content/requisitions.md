# Open requisitions

People ordered for a future date at a locked price (`requisitions.*` in the data; Q368). A saved list on the lab (`requisitions`: role, count, week due, locked fee; schema 80); older saves have none.

## `OpenRequisition` and `CancelRequisition`

`OpenRequisition(labId, roleId, count)`: from 1 to `maxCount` (3) people, up to `maxOpen` (3) orders at once. The price per person is locked on the day: `feePermille` (70%) of that day's hiring fee. A deposit of `depositPermille` (20%) of the locked price is paid now for everyone in the order. The people arrive `leadWeeks` (8) later. Refused for a rival, a game with `enabled` 0, an unknown role, a bad size, too many open orders and a lab short of the deposit (`InvalidArgument`, `UnknownId`, `NotEnoughCash`).

On the day each person needs a free seat in the role (visiting scholars hold researcher seats) and the rest of the locked price. Anyone who cannot be seated or paid for is lost, and their deposit with them; the people who can be are hired as normal (traits and all).

`CancelRequisition(labId, index)` closes an order for good; the deposit is not returned.

## Notes

- Events: `RequisitionOpened` (Amount people, Pick weeks, Ref role) and `RequisitionClosed` (SubjectId ordered, Amount came, Pick 1 all, 0 some or none, -1 cancelled).
- No random numbers on the order itself. The choice is to lock a lower price now and have the seats and the cash ready in two months, against hiring today at the full fee.
