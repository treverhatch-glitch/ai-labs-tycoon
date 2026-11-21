<!-- Generated from Rules/Data/default-config.json (successor). The data file is the source of truth: edit it, then update this page. -->
# A model helps design its successor

The bible's chapter 10 lists "letting the model help design its successor" among the things that erode control. A lab can put one of its finished models to work on a run that is still training (see QUESTIONS Q249 to Q251). It cannot be undone.

## What it does

`DesignWithModel` takes the run and the helper.

| When | Effect (shipped data) |
|---|---|
| At once | The run needs **15%** less of the compute it still has to do (`computeSavedPermille`), so it finishes sooner |
| When the run finishes | The successor's capability is **+5%** (`capabilityPermille`) |
| When the run finishes | The successor's real and shown alignment is **-70** points (`alignmentPoints`), plus **-40** for every earlier link in the chain (`chainPoints`, up to `maxChain` 3) |
| When the run finishes | Safety culture **-15** points (`culturePoints`) |

A link is a helper that was itself designed with help. The first successor of a model designed by people loses 70, the next 110, then 150 and at most 190.

## The rules

- The helper must have finished training (on sale, trained or retired) and have at least **half** (`helperMinPermille` 500) of the capability the run is forecast to reach.
- Refused, changing nothing: an unknown model, a run that has finished training, a helper still training, a run already designed with help, a helper too weak (`BelowSectorBar`).
- If the helper has since been pruned from the museum, the successor still pays the base cost.
- Rivals and the autopilot never do it.

## Notes

- Saved per model: `designerId` (the helper, or -1), schema 35.
- `EventKind.SuccessorDesigned` is logged and the company chat has a line for it. `SuccessorDesign.AlignmentLoss` and `HelperBar` give a screen what to show before the player commits.
