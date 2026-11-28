<!-- Generated from Rules/Data/default-config.json (rooms, roomPerks, staffPeople). The data file is the source of truth: edit it, then update this page. -->
# The workplace

Morale and poaching from placeable desks, water, and coffee in the [rooms rework design](../design/rooms-rework.md) extend the comfort-room rules below after step 2; steps 1–2 still use whole-room comfort perks only.

Morale was a number nobody could move. Now the rooms a lab builds change it, and it changes research.

## Comfort rooms

Three rooms of purpose Comfort seat nobody. Each only has perks.

| Room | Era | Min size | Build/tile | Upkeep/tile/week | Morale lift per tile (cap) | Offer defence per tile (cap) |
| --- | --- | --- | --- | --- | --- | --- |
| `coffee-corner` | Dial-up Days | 2 x 2 | $250 | $4 | 3 points (45) | - |
| `kitchen` | It Sees | 3 x 3 | $500 | $9 | 4 points (100) | 0.8% (20%) |
| `nap-pods` | The Chat Boom | 3 x 3 | $700 | $12 | 5 points (100) | - |

The Recruiting Office also takes 1% a tile off the chance of a rival's offer, up to 30% ([room-perks.md](room-perks.md)).

## Morale lift

Morale is 0 to 1000 and settles at 500 (`moraleBaseline`), one point a week (`moraleDriftPerWeek`). The comfort rooms' lift is added to that resting level, up to 150 points in all (`maxMoraleLift`): a lab with all three rooms full settles at 650. People already above the resting level drift back down to it, people below drift up, and the debt, incident and poaching drops work as before. A new hire starts at the resting level, not the baseline. Pull a room down and the level falls the same week; morale follows it down a point a week.

## Offers

A feuding rival's weekly chance to make an offer (`poachChancePermille`, 8%) is cut by the Kitchen and the Recruiting Office, by no more than 60% in all (`maxPoachDefencePermille`). The roll is the same single roll as before, just against a smaller number. A defended lab is never safe: 60% off still leaves 3.2% a week.

## Morale moves research

Each researcher's weekly research points are scaled by their own morale. Above the baseline, research rises 40% of the distance per 1000 points (`moraleResearchSlopePermille`): morale 650 gives 106%, morale 1000 gives 120%. Below the baseline it falls by `moraleResearchLossSlopePermille`, which is 0 in the shipped data, so an unhappy lab does the research it always did (the reason is in Q180). Set it to 400 and morale 0 gives 80%. Research never falls under a tenth of normal. A lab at the baseline, which is every lab before it builds a comfort room, does exactly the research it did before. The Star trait and the research-floor perks multiply on top. Morale also feeds the counter-offer chance ([talent-market.md](talent-market.md)), so a happy star is easier to keep.

`Workplace.Report` gives a screen all of this: lift, resting level, offer defence, today's offer chance, the researchers' average morale and what that does to research.

Not built: faster hiring from the recruiting office.
