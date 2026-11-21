<!-- Generated from Rules/Data/default-config.json (objectives). The data file is the source of truth: edit it, then update this page. -->
# Training objectives

The bible's training recipe (ch. 6) has an **objective** choice: "be correct, be engaging, be obedient, be careful. Training for engagement sells and breeds flattery." Chapter 10 lists "training for engagement or obedience over honesty" among the things that erode control. While a model is still training the player can choose what it is for. The effects land when the run finishes (see QUESTIONS Q246 to Q248).

| Objective | Alignment | Capability | Appeal | Culture |
|---|---|---|---|---|
| Be correct (the default) | none | none | none | none |
| Be engaging | -80 | none | +15% | -10 |
| Be obedient | -60 | +6% | +3% | -10 |
| Be careful | +100 | -7% | -3% | +5 |

- `SetTrainingObjective` takes the model and the objective. It is refused, changing nothing, for an unknown model or objective, or for a model that has finished training. It can be changed any number of times while the model trains; the last choice wins. Choosing the default clears it.
- Alignment points move the real and the shown meter together (kept inside 0 to 1000), so an engaging model is not a hidden gap, just a model that is less safe than it would have been.
- Capability is a share of the model's finished capability, added to its luck part so the capability parts still add up. It never drops below 1.
- Appeal is a separate line in the appeal breakdown (`ObjectiveEffect`), a share of everything before it, in every sector the model sells in.
- Culture moves once, when the run finishes.
- Rivals and the autopilot never choose, so their models are all trained to be correct.

## Notes

- Saved per model: `objective` (empty for the default), schema 34.
- `EventKind.ObjectiveSet` is logged when the choice changes, and the company chat has a line for it.
