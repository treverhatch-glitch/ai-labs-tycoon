# Interpretability programme

The bible's chapter 10 lists "interpretability: research into what the model is actually doing inside" among what builds control. The player can run a standing programme for it. Rivals and the autopilot never do. Nothing happens until the player sets a level (`interp` in the data; Q302).

## `SetInterpretability`

It takes a level from 0 (off) to the top, 3. Level 1 and level 2 need Interpretability v1; level 3 needs Interpretability v2 (`levelTechs`, one tech per level; the list's length is the top level and an empty list means no programme). It is refused for a lab that is not the player's, a level out of range, the level the lab already has, and a missing tech. Turning it off is free.

## What a level does

| | Per level (shipped) |
|---|---|
| Weekly bill | $5,000 on the training ledger line (`weeklyCostPerLevel`), paid even into debt |
| Research | -6% of the lab's research output (`researchCostPermillePerLevel`) |
| Gap | 10% of the gap between shown and real alignment is closed every week on every model on sale (`gapClosePermillePerLevel`), never less than one point while a gap remains |
| Stale techs | 15% of what stale safety techs have lost is given back (`staleReliefPermillePerLevel`) |

## Notes

- When the last point of a model's gap closes, `GapFound` is logged once for it. Models that are only trained are left alone.
- The shown meter comes down, so a model that was sandbagging can fall under the lab's own release gate. Once its gap is under `safety.sandbagGapPermille` an incident is no longer the sandbagging rung.
- A red team closes half of a gap at once ([red-team.md](red-team.md)); the programme carries on from there.
- Saved per lab: `interpLevel`, schema 46. See [control-stale.md](control-stale.md) for what goes stale.
