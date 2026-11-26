# Distill slot ledger (Q221+)

After **2006** (`distillSlots.opensYear`), the player buys **prepaid slot packs** tied to a **rival lab** before `SetDistillTarget` / `distill-rival` training. Not a pursue/sign contract: one `PurchaseDistillSlots` command, finite slots, expiry clock.

## Player trade-offs

- **Rival pick:** slots are wasted on the wrong lab if you pivot targets.
- **Pack:** few cheap slots with short `validWeeks` vs bulk discount and longer expiry.
- **Timing:** unused slots **expire** at `expiresTick` (lost money).

Each distill **run start** consumes **one slot** from the oldest non-expired lot for that rival.

## Era SuccessorDesign (Q227 merge)

Rules-side slots do **not** implement successor compute save or alignment loss. On merge with era PR #11, `DesignWithModel` / `SuccessorDesign.OnTrained` run on their own hooks; distill prep discount and finish capability permille stay in training only.