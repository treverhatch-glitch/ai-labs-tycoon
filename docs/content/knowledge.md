# Owners and documentation

Who knows the model (bible ch. 6, key-person risk and documentation; `knowledge.*` in the data; Q440). Five saved fields on each model (`ownerId`, `docLevel`, `docAge`, `knowGap`, `handovers`; schema 123). Only the player's lab. A lab that names nobody has no owners and no gaps.

- `AssignOwner` names a researcher, engineer or safety person as the owner of a model (count 0 takes the owner away). One person owns at most two models.
- `WriteDocs` writes the next level of documentation for a model with an owner, up to three levels, for 12,000 each. It goes stale after 52 weeks and loses a level.
- When the owner leaves (let go, quits, poached or lost in a pivot; everything that goes through the staff record) the model is left with a knowledge gap of 600, less 250 for each level of documentation, and the handover is counted.
- The model's weekly chance of failing in front of customers rises by 9% of the gap. A new owner closes 25 of the gap each week; with no owner it stays.
- From a gap of 300 nobody dares change the model: it cannot be handed to agents (raising its autonomy) or tuned until the gap has closed.

- The gap makes agents blow up more often (Q443) and documentation softens what preference tuning teaches (Q446).

Events: `OwnerNamed`, `OwnerLeft`, `DocsWritten`, `DocsStale`.
