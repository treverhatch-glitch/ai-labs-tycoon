<!-- Generated from Rules/Data/default-config.json (ageing). The data file is the source of truth: edit it, then update this page. -->
# Rooms that age

The bible's chapter 14 says old rooms go out of date. Each time a new era begins, every room of a player lab gets one era older. Rivals' rooms never age (the same knife-edge reason as Q182; see QUESTIONS Q230 to Q232). It is saved as one number per room, `eraAge` (save schema 31).

## The stages

| Stage | Eras old | Extra upkeep | Morale off the resting level |
| --- | --- | --- | --- |
| Fresh | 0 | none | none |
| Settled | 1 | none | none |
| Dated | 2 | +15% | 20 |
| Beige | 3 | +30% | 40 |
| Museum piece | 5 | +50% | 60 |

A room is in the highest stage whose "eras old" it has reached. When a room moves up into a stage that costs something, a `RoomDated` event is written ("... is now dated, 2 eras behind. Staff are complaining.") and the company chat comments on it. The first two stages are free, so a room has two eras of grace.

## Morale

The lab's resting morale loses the average penalty of its rooms that seat people, weighted by tiles, and never more than `maxMoralePenalty` (60). A lab with one old closet and a big fresh office feels almost nothing.

## Refurbishing

`RefurbishRoom` puts a room back to fresh. It costs a quarter (`refurbishCostPermille` 250) of the room's build cost per tile times its tiles, on the construction line of the ledger. It is refused, and changes nothing, when the room is already fresh, the room is unknown, or the cash is short.

## Notes

- `RoomAge.Of` gives a report with every room, its stage, what it adds to upkeep and what a refurbish would cost, for a screen to show.
- Save schema 31 adds `eraAge` to every room. An older save loads with 0 on every room.
