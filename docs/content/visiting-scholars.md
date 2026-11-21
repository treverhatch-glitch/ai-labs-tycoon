# Visiting scholars

A group of outside researchers hosted for a few months (`scholars.*` in the data; Q369). Three saved fields on the lab (`scholarCount`, `scholarStart`, `scholarDue`; schema 80); older saves have none.

## `HostScholars` and `SendScholarsHome`

`HostScholars(labId, count)`: from 1 to `maxScholars` (3) scholars, one group at a time, for `weeks` (12). They sit at researchers' desks, so there must be that many free researcher seats now, and while they stay a hire cannot take those seats. The whole stipend (`stipendPermille`, 15% of a researcher's hiring fee each) must be in the bank and is paid evenly over the weeks. Refused for a rival, a game with `enabled` 0, a bad size, a group already in the lab, too few free seats and a lab short of the stipend (`InvalidArgument`, `NoSeats`, `NotEnoughCash`).

Each week the group adds research points at `researchPermille` (60%) of a researcher's rate each. When they leave on schedule each adds `prestigeEach` (20) prestige (never past the maximum). `SendScholarsHome(labId)` ends the stay early with no prestige.

## Notes

- Events: `ScholarsArrived` (Amount scholars, Pick weeks) and `ScholarsLeft` (SubjectId scholars, Amount prestige added, Pick 1 on schedule and 0 sent home).
- No random numbers. They are not staff: they never join, never quit and cannot be laid off.
