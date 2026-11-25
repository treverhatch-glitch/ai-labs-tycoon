# Spot capacity

Cheap rented GPUs that the cloud can take back (bible ch. 6, compute; `spot.*` in the data; Q431). Seven saved lab fields (`spotPermille`, `spotSince`, `spotSaved`, `spotPreempts`, `spotLost`, `spotOut`, `spotWaves`; schema 119). Only the player's lab. A lab that never sets a share pays full rent and is never reclaimed.

`SetSpotShare` (0 to 600 permille) chooses what share of the rented GPUs run as spot. A different share has to wait 4 weeks.

- The rent of that share is 40% cheaper, so a 50% share cuts the GPU rent by a fifth. The saving is added up each week.
- **Reclaim waves.** Each week the cloud may call a wave (12% at the full 600 share, less what the lab's engineers save, as for failed runs). It needs a run in training and cannot start while one is on.
- **When it starts**, every training run that is not rushed in the run queue is interrupted and loses what a failure would for its checkpoint habit (10% often, 50% now and then; 50% with none set).
- **While it lasts** (2 weeks), the spot share of every run that is not rushed does no work: a 50% share halves those runs' progress.
- **Rushed runs** (queue level 3) sit on reserved capacity: they are neither interrupted nor slowed. Rushing costs progress and only so many runs can be rushed, so the queue decides what the lab protects.

Event: `SpotPreempted` (one for each interrupted run).
