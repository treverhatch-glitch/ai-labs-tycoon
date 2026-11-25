# Cluster fabric

The interconnect and cooling that make many GPUs train as one cluster (`fabric.*` in the data; Q420). Six saved lab fields (`fabTier`, `fabBuild`, `fabLeft`, `fabGpus`, `fabBuilt`, `fabCancelled`; schema 113). Only the player's lab. A lab that never orders one has none of this.

`OrderFabric` builds the next tier (1, 2, 3, in order). `CancelFabric` drops the order and the deposit with it.

| Tier | Opens in era | Weeks to build | Cost for each owned GPU (share of the GPU price) | Training speed |
|---|---|---|---|---|
| 1 | 3 | 8 | 8% | +6% |
| 2 | 4 | 14 | 15% | +13% |
| 3 | 6 | 22 | 25% | +21% |

- At least 8 owned GPUs (rented ones do not count). The cost is worked out on the GPUs owned at the order, so buying more afterwards does not raise the bill and does not get the new ones a share of it.
- 40% is paid when ordered; the rest at delivery. If the lab cannot pay at delivery, the tier is not delivered until it can.
- In the last 3 weeks of a build, and while a finished build waits for its balance, the cluster is down for the cutover: training speed falls by 30%. A tier replaces the one before it; it does not add to it.
- The outage is the cost of waiting for the cheaper deal: it falls on whatever the lab is training in those weeks, so the weeks to start a build are the quiet ones.

Events: `FabricOrdered`, `FabricDelivered`, `FabricCancelled`.

Together: squads in which every squad has a senior halve the cutover loss (`squadCutPermille` 500; Q425).
