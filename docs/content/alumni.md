# Alumni network

People who leave the player's lab stay in a pool for a while and can be asked back (`alumni.*` in the data; Q362). A saved list on the lab (`alumni`: old id, role, trait, week left, how they left; schema 76); older saves have an empty pool.

## Who goes in

| How they left | Recorded as | Where |
|---|---|---|
| Poached by a rival | on good terms (1) | `TalentSystem.Leave` |
| Laid off (a layoff round or `Fire`) | laid off (0) | `Layoffs.Run`, `Fire` |
| Quit by blog post | quit (-1) | `Traits.RunQuits` |

The pool holds `maxSize` (12); when it is full the oldest leaver drops out, and a leaver expires `expireWeeks` (104) after leaving. Nothing else is recorded for rivals.

## `RehireAlumnus`

`RehireAlumnus(labId, personId)`: the person's old id from the lab's list.

| What | Rule |
|---|---|
| Fee | The role's hiring fee x `feeGoodPermille` (50%), `feeLaidOffPermille` (70%) or `feeQuitPermille` (90%), by how they left |
| Seat | A free seat in the role, as for a hire |
| Trait | The same as before: a known quantity, good or bad (a quitter returns with the quitter's morale line) |
| Morale | Where people settle plus `moraleGood` (+80), `moraleLaidOff` (-40) or `moraleQuit` (-100) |
| Colleagues | Every colleague's morale moves once by `peerGood` (+10), `peerLaidOff` (-10) or `peerQuit` (-20) |

The person gets a fresh id and leaves the pool. Refused, with nothing changed, for a rival, a game with `enabled` 0, a person not in the pool, no free seat and a lab short of the fee (`InvalidArgument`, `UnknownId`, `NoSeats`, `NotEnoughCash`).

## Notes

- Event: `AlumniRehired` (SubjectId the old id, Amount the fee, Pick 1 good terms, 0 laid off, -1 quit, Ref the role).
- The validator keeps a worse parting from ever being cheaper or happier than a better one.
- No random numbers; a lab nobody leaves has an empty pool and is exactly as before (the golden moved only through the new saved list and the schema number, verified by taking them out).
