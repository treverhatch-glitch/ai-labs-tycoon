# Customer support desk

Engineers on a support desk against a weekly stream of tickets (`support.*` in the data; Q366). Two saved lab fields (`supportPeople`, `supportBacklog`; schema 78); older saves have no desk and no backlog.

## `StaffSupportDesk`

`StaffSupportDesk(labId, people)` sets the engineers on the desk (0 closes it): at most `maxPeople` (4), and `keepFree` (1) engineers must stay off it. They are the first engineers in the order the lab lists them, and they leave engineering and training work (the same way a skunkworks team or an account team does). Refused for a rival, a game with `enabled` 0, a negative number, more than the lab can spare and the number it already has (`InvalidArgument`, `NotEnoughStaff`).

## The week

- Tickets in: the weekly revenue of the lab's models on sale divided by `revenuePerTicket` ($4000).
- Tickets cleared: `baseClear` (6, the sales team) plus `perPerson` (4) for each engineer on the desk.
- The backlog moves by the difference, never below 0 or above `maxBacklog` (100).
- Past `backlogLimit` (15) the lab loses `trustPerTicket` (2) permille of trust for each ticket over, up to `maxWeeklyTrust` (20), in every sector it sells in, every week until the desk catches up.

## Notes

- Events: `SupportDeskSet` (Amount the people on it) and `SupportBacklogChanged` (Amount the backlog, Pick 1 when it passed the limit and 0 when it came back under).
- No random numbers; a lab with nothing on sale has no tickets.

People on a sabbatical are not drawn on: the desk takes the first engineers who are not on leave, and a desk member cannot be sent on sabbatical (Q370).
