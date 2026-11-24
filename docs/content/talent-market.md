# Talent market

Poaching is constant (bible ch. 7). A feuding rival no longer takes a person the moment its dice say so: it makes an **offer**, and the player has a few weeks to answer. The player can play the same game back at the rivals. Offers are saved on the lab (`poachOffers`, save schema 23); nothing else about the market is saved, because every price and chance is worked out from the config each time.

## Offers

- A rival that feels a feud toward the player, and wins its poaching dice, makes an offer to one person: a star first, else the unhappiest person of the role.
- An offer waits 4 weeks (`talent.offerWeeks`). If nobody answers, the person leaves, as they always did. Setting `offerWeeks` to 0 brings back the old instant raid.
- A lab can have 2 offers open at once (`talent.maxOpenOffers`); a rival does not raid a person who already has an offer.
- The offer's **package** is the era's hiring fee times 6, and twice that for a star. It is what the person is worth to anyone who wants them.
- Letting a person go closes any offer about them.

## Counters

The player answers an offer with one counter. Each costs a share of the package and works with a chance. A counter that works keeps the person and lifts their morale; one that fails costs the money and the person leaves anyway. The chance is the base plus the person's morale above the baseline times the morale slope plus the lab's reputation above 500 times the reputation slope, never under 5% and never over 100%.

| Counter | Cost (of the package) | Base chance | Morale slope | Reputation slope | Morale gain |
| --- | --- | --- | --- | --- | --- |
| `money` | 100% | 100% | 0 | 0 | +100 |
| `compute` | 40% | 80% | 0.2 | 0 | +60 |
| `title` | 10% | 45% | 0.4 | 0.2 | +40 |
| `mission` | free | 35% | 0.5 | 0.5 | +30 |

Money always works but is the dearest. The cheap counters work for happy people at a lab with a good name, and badly for the unhappy at a lab with a bad one.

## Poaching back

The player can raid a rival's people for a headhunter's fee (15% of the package, paid whether or not it works) plus the package itself if it does. The chance is 40% plus the target's unhappiness (the morale baseline minus their morale) times 0.6 plus the player's reputation above 500 times 0.3. The rival must be a working rival lab (not the player, not bankrupt, not absorbed) and the player needs a free seat.

- A raid that works costs the rival 15 points of feeling toward the player and moves the person across with their morale reset to the baseline.
- A raid that fails costs the rival 3 points of feeling.
- A raided star takes their research with them: the rival's output falls and the player's rises by the star's edge.

## What the player and the autopilot do

The planners and the autopilot never answer an offer, so a lab on autopilot loses its people as before, only a few weeks later. Answering is a decision for the player.
