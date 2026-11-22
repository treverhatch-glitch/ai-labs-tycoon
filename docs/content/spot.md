# Spot capacity

Cheap rented GPUs that the cloud can take back (bible ch. 6, compute; `spot.*` in the data; Q431). Five saved lab fields (`spotPermille`, `spotSince`, `spotSaved`, `spotPreempts`, `spotLost`; schema 119). Only the player's lab. A lab that never sets a share pays full rent and is never preempted.

`SetSpotShare` (0 to 600 permille) chooses what share of the rented GPUs run as spot. A change can be repeated at once, but a different share has to wait 4 weeks.

- The rent of that share is 40% cheaper, so a 50% share cuts the GPU rent by a fifth. The saving is added up each week (`spotSaved`) and shown with the running costs.
- Every week each training run has a chance of being preempted: 12% at the full 600 share, less what the lab's engineers save (the same saving they give against failed runs). A preempted run loses what a failure would for its checkpoint habit (10% often, 50% now and then); with no habit set, 50%.

Event: `SpotPreempted`.
