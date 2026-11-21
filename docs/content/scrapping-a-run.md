# Scrapping a training run

The player can stop a run that is still training. The compute already spent is gone and the model disappears, but part of it comes back as research points. The further along the run was, the more the press and the team mind. Only the player's lab does it (`abandon.*` in the data; Q334). No new state.

## `ScrapRun(model)`

Refused, with nothing changed, for a lab that is not the player's, a game with `enabled` 0, an unknown model (`UnknownId`) and a model that is not training any more (`WrongModelStatus`: trained, released or retired models are kept).

## What happens

| What | Amount |
|---|---|
| Research points kept | `salvagePermille` (200) of the compute already spent |
| Reputation lost | `reputationAtFull` (20) times the progress share |
| Morale lost by every person | `moraleAtFull` (60) times the progress share |
| The model | Removed from the lab. The run slot is free at once and the next run of the line gets the same version number |

Progress share is the compute spent over the compute the run needs, from 0 to 1. A run scrapped on its first week costs the press and the team nothing. Reputation and morale never go below 0.

## Caps

The validator keeps the salvage at or under 500 permille, so scrapping a run can never pay better than finishing it, and keeps the losses inside sensible bounds.

## Notes

- Event `RunScrapped` (Amount the research points kept, Ref the model's name, Pick 1 when the run was at least half done, 0 when less).
