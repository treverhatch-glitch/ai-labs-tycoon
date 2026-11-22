# Training queue

A priority queue across the lab's training runs (`runQueue.*` in the data; Q408). One saved model field (`queueLevel`; schema 106). Only the player's lab.

| Command | What happens |
|---|---|
| `SetRunPriority` (a model in training, level 1 background, 2 normal, 3 rush) | sets the run's level; costs the run 2% of its progress (a reload from checkpoint); at most 1 run rushed at a time |

With no level set on any run, every run in training gets an equal share of the training compute, as it always did (the oldest run takes what does not divide evenly). Once any run has a level, the compute is shared by weights: background 1, normal 2 (also a run with no level), rush 4. Nothing is lost: the shares always add up to the week's training compute, so a rush is paid for by the other runs, not by cash.

Events: `RunPriorityChanged` (SubjectId the model, Amount the progress lost, Pick the new level).

A rushed run weakens the licence bar for its model (Q414).
