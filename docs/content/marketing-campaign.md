# Marketing campaigns

The player can pay for a burst of advertising in one sector. While it runs, the lab's models are more appealing there, most of all for a lab nobody has heard of. When it ends customers feel a little let down for a while, and if the model has an incident during the campaign the whole thing collapses. Rivals and the autopilot never run one. Nothing happens until the player does (`marketing.*` in the data; Q322).

## `RunCampaign(sector, level)`

Level 1 to `maxLevel` (3). It costs `costPerLevel` ($40000) for each level, needs `minSales` (1) sales person on the payroll (`NotEnoughStaff`), and the lab must already sell in the sector (`NoModelForJob`). It is refused for a lab that is not the player's, a bankrupt lab, a game with `maxLevel` 0 or `weeks` 0, an unknown sector (`UnknownId`), a level outside 1..max, a campaign already running or being forgiven, and for `cooldownWeeks` (26) after the last one started (`CampaignLocked`). A lab that cannot pay gets `NotEnoughCash`. Nothing is changed by a refusal.

## While it runs (`weeks`, 8)

The appeal of every model the lab sells in the campaign sector is multiplied by `1000 + boost`, where

    boost = level x appealPermillePerLevel x (shareCeilingPermille - share) / shareCeilingPermille

A lab with no share gets the whole boost (12% a level); it falls in a straight line to nothing when the lab's share of the sector reaches the ceiling (60%). The effect is shown as `CampaignEffect` in the appeal breakdown and is included in its total. Other sectors are not touched.

## When it ends

After `weeks` it ends by itself (event `CampaignRun`, Pick 0). A `hangoverWeeks` (8) hangover follows, in which the appeal in that sector is multiplied by `1000 - level x hangoverPermillePerLevel` (5% a level), and then the state clears.

If a model the lab sells in the campaign sector has an incident while the campaign is running, the lab loses `backlashReputation` (15) on top of the incident's own cost, the campaign ends at once (Pick 2) and the hangover starts. An incident in another sector, outside a campaign or during the hangover costs nothing extra.

## Notes

- Saved per lab: `campaignSector`, `campaignLevel`, `campaignWeeks`, `hangoverWeeks`, `campaignLockedUntil`, schema 55 (added by `MigrateTo55`). A campaign that does not fit the data is reported on load.
- Only one campaign at a time. The data validator requires `cooldownWeeks` to be at least `weeks` + `hangoverWeeks`.
- A lab that already has most of the sector gains nothing, so the campaign pays for a lab that is still small in a crowded sector.
