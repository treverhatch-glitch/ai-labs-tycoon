# Overload

The bible's chapter 6 says an overworked model is "sweaty, slow, dark circles": serving load too high, reliability drops. The creature already shows
the mood (`ModelMood.Overworked`, set when a lab cannot serve all its demand). This makes the mood cost something: an overloaded lab's models have
more incidents until the lab catches up.

## The rule

Each week a released model rolls for an incident. The roll's chance is the model's own chance (alignment, corruption, era load, a rushed launch) plus
the overload:

`overload = safety.overloadIncidentPermille (60) x the share of last week's demand left unserved`

A lab that serves none of its demand adds 6 percentage points to every model's weekly chance; one that serves 90% adds 0.6. A lab that serves it
all, or has no customers, adds nothing. The total never passes certainty.

## Who

Only the player's labs, while `safety.overloadForRivals` is false (the shipped value): the rivals' economies are knife-edge (Q182) and their
incident counts feed the same guards as Q190. Setting 0 in `overloadIncidentPermille` turns the rule off for everyone.

## What shows it

`DisplayQueries.IncidentParts(c, state, lab, model)` has a `FromOverload` line beside config, alignment, corruption and rush, so the odds screen
explains the extra chance. The model-only query is unchanged and shows none.

## Notes

- Nothing is saved: the unserved share is last week's market result. No schema change.
- The roll still draws from the same dice stream once per released model per week, so a lab that is not overloaded plays exactly as before.
