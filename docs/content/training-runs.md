<!-- Generated from Rules/Data/default-config.json (runs). The data file is the source of truth: edit it, then update this page. -->
# Experimental runs and checkpoints

The bible's chapter 6 recipe table says the architecture can be "whatever you have researched this era, plus your own experimental variants. New designs have higher ceilings and fail more often", and "runs can fail: divergence, a data bug found in week nine, a power cut. You can checkpoint more often at a cost". The player now sets both while a model trains (see QUESTIONS Q267 to Q269).

## `SetRunApproach` and `SetCheckpoints`

Both take the model and an id, and both are refused, changing nothing, for an unknown model or id (`UnknownId`) or a model that is no longer training (`WrongModelStatus`). Choosing what the model already has changes and logs nothing. Both are logged as `EventKind.RunSetUp`.

| Approach | Capability when the run finishes | Weekly failure chance |
|---|---|---|
| Known architecture (default) | none | none |
| Your own variant | **+8%** (`capabilityPermille` 80) | **1.5%** each training week (`failurePermillePerWeek` 15) |

| Checkpoint habit | What a failure throws away | Weekly cost |
|---|---|---|
| Now and then (default) | **50%** of the run's progress (`lossPermille` 500) | free |
| Every few days | **10%** (`lossPermille` 100) | **4%** of a week of running the lab, each training week (`weeklyCostPermille` 40), never more than the cash in hand, on the training line |

## A failure

Each training week the run's chance is a hash of the seed, the week, the lab and the model, so it draws nothing from the shared dice. A failure throws away the habit's share of both the run's progress and its safety work and logs `EventKind.TrainingFailed` (Amount: the compute lost). A run with nothing yet to lose does not fail. The capability bonus is added the way the objective's is, into the luck part, so the parts still add up to the whole.

## Engineers

Each engineer takes **6%** off the run's weekly failure chance (`runs.engineerSavingPermillePerStaff` 60), up to **60%** (`engineerSavingCapPermille` 600). A variant run's 1.5% becomes 1.41% with one engineer and 0.6% at the cap. Nothing is saved: the saving is worked out each week from the staff on the books, so firing engineers mid-run raises the chance the next week (see QUESTIONS Q270 and Q271).

## Notes

- Saved per model: `approach` and `checkpoints` (empty for the defaults), schema 40.
- A run nobody set up does exactly what it did before. Rivals and the autopilot never choose, so the economy guards are untouched.
- Open: the recipe-screen rows (Unity); failures with different causes (a data bug found late, a power cut); rivals trying variants.
