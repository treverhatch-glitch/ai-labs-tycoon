# The service-level pledge

The player can promise customers uptime. While the promise holds the lab earns a premium on its revenue. In a week the promise breaks it refunds part of that week's revenue, loses reputation and takes a strike. Only the player's lab pledges (`sla.*` in the data; Q329).

## `SetSla(level)`

Level 0 to `maxLevel` (3). Raising the pledge needs a reputation of at least `minReputation` (300); dropping it needs none. Every change locks the pledge for `lockWeeks` (8, `SlaLocked`). Refused, with nothing changed, for a lab that is not the player's, a game with no levels, a level out of range, the level already set, a locked pledge and too little reputation.

## Every week (after the market and any incident)

A lab with no pledge or no revenue that week is not touched. Otherwise:

| The week | What happens |
|---|---|
| Kept | Premium of `premiumPermillePerLevel` (40) a level on the week's revenue is earned on the revenue line. Strikes go back to 0 |
| Broken | No premium. A refund of `refundPermillePerLevel` (120) a level of the week's revenue is paid on the fines line. Reputation falls by `breachReputationPerLevel` (8) a level. One strike. Event `SlaBroken` |

A week is broken when the lab could serve less than `breachBelowPermille` (900) of its demand, or one of its models had an incident that week. `strikeWeeks` (3) broken weeks in a row drop the pledge one level and lock it again (event `SlaPledged`, Pick 0). Zero never drops it.

## Caps

The validator keeps every level's premium together at most 300 permille, the refund at least the premium and every level together at most 1000, and the reputation loss at most 200 together. So a pledge that is always broken never pays, and no refund is more than the week's revenue. Reputation stays at 0 or more.

## Notes

- Saved per lab: `slaLevel`, `slaLockedUntil`, `slaStrikes`, schema 59. A pledge that does not fit the data is reported.
- Events: `SlaPledged` (Pick 1 up, 0 down or dropped), `SlaBroken` (Amount the refund, Pick 0 serving, 1 incident).
