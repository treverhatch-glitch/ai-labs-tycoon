# Lab values

The lab's stated values and whether it lives up to them (`creed.*` in the data; Q395). Eight saved lab fields (`creedA`, `creedB`, `creedIntegrity`, `creedBand`, `creedLock`, `creedTown`, `creedKept`, `creedBroken`; schema 97). Only the player's lab.

| Command | What happens |
|---|---|
| `SetCreed` (two of 1 safety, 2 speed, 3 openness, 4 people, 5 profit) | integrity starts at 500; locked for 48 weeks; changing them later starts integrity at 300 and the record stays |
| `HoldTownHall` | costs 100,000, adds 80 integrity; 12 weeks before another; not while the creed is a joke |
| `OwnUp` | costs 30 reputation, brings integrity up to at least 500; only below 500; shares the 12-week wait |

Each week the lab's own acts of the week before are held up against the two values: an act in keeping adds 15, an act against takes 60. The table is in `Creed.Score`: safety is kept by red teams, post-mortems, bounties and audits and broken by a rushed model or a whistle-blower leak; speed is kept by a rushed model, hack weeks, skunkworks and fast builds and broken by red teams and post-mortems; openness is kept by papers, post-mortems and scholars and broken by taking a contract or selling a model; people is kept by promotions, sabbaticals, mentoring, internships, equity and recognising a union and broken by layoffs and resisting one; profit is kept by a follow-on, an advance, a platform deal, a contract and a funding round and broken by a foundation or a chair. One act can keep one value and break the other. Devices, platform deals and replication feed it too: a rushed model or a recalled device breaks safety, designing a device keeps speed, putting a paper's code out keeps openness and keeping it back breaks it, and a platform deal or its renewal keeps profit.

At 800 or more the lab is held up as an example (once: +20 reputation, +60 morale to every person); at 300 or less the creed is a joke (once: 40 reputation lost, damped, 100 morale lost). A band is left only 100 points back inside, so it can be crossed again.

A lab held up as an example takes 70% of any backlash shock (Q396) and a lab whose creed is a joke takes 140% (`shieldPermille`, `exposePermille`).

Events: `CreedSet`, `TownHallHeld`, `CreedOwnedUp`, `CreedHollow`, `CreedProud`.
