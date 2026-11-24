# Staff equity

The founder can hand the team a slice of the company, one level at a time. It buys cheaper salaries, steadier morale and a team that is harder to poach, and it costs the founder's stake and part of the price if the lab is sold. Rivals and the autopilot never grant it. Nothing happens until the player does (`equity.*` in the data; Q315).

## `GrantStaffEquity`

It gives the team the next level (to `maxLevel`, 3). It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` 0, a team that already has every level, fewer than `minStaff` (3) people on the payroll, and a grant that would take the founder's stake to the funding data's control line (`funding.controlPermille`) or under it (`WouldLoseControl`). A grant cannot be taken back.

Each level moves `dilutionPermille` (4%) of the founder's current stake to the people, never less than 1 permille. Funding rounds dilute the people's stake by the same share as the founder's (never below 1 permille), so the two stakes always add up to at most the whole company.

## What the team gets, for each level granted

| What | Per level |
|---|---|
| Every salary | -`salaryCutPermille` (6%), after the pay policy |
| Where morale settles | +`moraleStep` (20), so an offsite can lift people higher too |
| A rival's chance to poach | -`poachStepPermille` (5%) |

## What it costs

- The founder's stake, which funding rounds and the board's control line already watch. A later round can take the board's control sooner because of the grant.
- When the lab is sold, the people's stake comes out of the price (`StaffShare` in the exit price breakdown, taken off the total). The new lab starts with the net price and the old lab's equity is cleared.

## Notes

- Saved per lab: `equityLevel` and `equityPermille`, schema 53. A save where the level is out of range, only one of the two is zero, or the stakes together pass the whole company is reported.
- Event `EquityGranted` (SubjectId the level, Amount the people's stake in permille).
