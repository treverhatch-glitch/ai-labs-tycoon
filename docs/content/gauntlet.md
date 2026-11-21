# The Gauntlet, deeper

Once a year every lab enters The Gauntlet (see [sectors.md](sectors.md)). This page covers what the player can do about it: **tune** the entries for the test, the risk of being **exposed**, the **live demo**, the **joke awards**, and the **season table**. The only new saved state is the tuning setting on the lab and three small fields on each year's record (save schema 26); the rest is worked out each year.

## Tuning for the test

The player sets one number for the whole lab, `SetGauntletTuning`, from 0 to **300** permille. 0 enters every model as it is. The setting stays until the player changes it.

At the Gauntlet the lab pays a **prep bill**: 10 hiring fees of a sales rep of the era for each 100 permille of tuning (so 300 costs 30 fees). The bill is booked under the Hiring ledger line. A lab that cannot find the money, or that is bankrupt, enters untuned and keeps its cash.

A tuned lab's scores, in every bracket and in the crown ranking, are lifted by **half** of the tuning: a tuning of 300 lifts each score by 15%. A lift that stands shows as `Tuned` on the entry.

## Being exposed

A tuned lab that comes first in any bracket, or takes the crown, rolls to be **exposed**. Labs that win nothing by it are never exposed. The chance is:

- the tuning times 2 (a tuning of 100 is a 20% risk), plus
- 10% for every working rival that is in a feud with the player (player only), up to
- a ceiling of **95%**.

An exposed lab loses the lift on every entry (they are scored and ranked again as if untuned, so the crown and the brackets may go to someone else), loses **80 reputation** (less its press rooms' scandal damping, see [press.md](press.md)), and the prep bill is not refunded.

## The live demo

After the crown is decided, each working lab's crown entry takes the stage. Nothing is random. The model's weekly incident chance (the same figure the safety system uses) decides it:

| Weekly incident chance | Result | Reputation |
| --- | --- | --- |
| 0.8% or less | Triumph | +20 |
| Over 0.8% and under 2% | Nothing | |
| 2% or more | A meme | -30 (less scandal damping) |

Set `demoMemeFromPermille` to 0 to switch the demo round off. Rivals take the stage too; a bankrupt or absorbed lab does not.

## Joke awards

Two awards carry real effects. Both go to working labs only.

| Award | Goes to | Effect |
| --- | --- | --- |
| Most Confident While Wrong | The lab whose crown entry claims the most alignment it does not have, with a gap of at least 100 (a tie goes to the older lab) | -30 reputation (less scandal damping) |
| Best Newcomer | The best-ranked lab, not the crown winner, whose first release was no more than 3 years ago | +15 reputation and +80 morale for every person on its staff (capped at 1000) |

The winners are written on the year's record (`confidentLabId`, `newcomerLabId`, -1 for nobody).

## The season table

`GauntletSystem.SeasonTable` ranks every lab that has stood in the kept years: 5 points for each crown, 2 for each bracket win. Equal points go to the lab with more crowns, then the older lab. It is a query; nothing is saved for it.

## What rivals do

Rivals never tune: their setting is 0 and no planner sets it. They do take the stage and can win the joke awards.

## Not built

Sector trust (a slow-to-win, fast-to-lose meter that carries to neighbouring sectors) is the next item, and a playable demo scene is a Unity matter.
