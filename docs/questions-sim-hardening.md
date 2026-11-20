# Sim hardening decisions

Forks taken while adding save/load and headless soak tests on `cursor/sim-hardening`. Merge into `docs/QUESTIONS.md` after rivals/unity batches land.

### Q1. Mid-game save snapshot for round-trip tests? DECIDED: autopilot only

- Options: (a) hand-built minimal state; (b) autopilot ~80 weeks with all rivals; (c) load schema-3 fixture only.
- Choice: **(b)** `Autopilot(96)` on a starter lab with rivals, so labs, world map, training, gauntlet history, and ledger history are warm without rack-dependent commands.
- Why: exercises every persisted list without coupling to rivals-thread save fixtures.

### Q2. Monthly invariants in the 10-year soak? DECIDED: ledger cash, bounds, determinism

- Options: (a) digest only at year end; (b) assert cash ledger identity and sane ranges every month start; (c) full SaveGame.Problems every week.
- Choice: **(b)**; `SaveGame.Problems` only when a lab has rooms (mid/late game).
- Why: catches drift early without making the soak depend on validator completeness for empty labs.

### Q3. 2009 quick-start soak seed? DECIDED: separate seed 9009

- Options: share seed with 1995 run; independent seed.
- Choice: **independent** (`9009`) with `StartEraId = punchline`.
- Why: era bootstrap and starting techs differ; a second seed proves determinism per start kit.

### Q4. Spectate save round-trip stepping? DECIDED: sell-lab spectate, Weeks not player Autopilot

- Options: sell-lab spectate; bankrupt spectate; skip stepping after spectate.
- Choice: **sell-lab spectate** then `Weeks()` on loaded sim (no `RunPlan` on player id); bankrupt spectate remains covered in `EndingsTests`.
- Why: after review fixes at `c448c0f`, long steps while spectating no longer throw in `GauntletSystem.RunYear`.

### Q5. Heavy shady soak setup? DECIDED: trained model plus active liberate-funds op

- Options: heat/stash only; injected model state; train then corrupt and start op.
- Choice: **train mini, set corruption, start `liberate-funds`** before the ten-year soak.
- Why: guarantees shady RNG and op queue are warm without hand-building illegal model fields.

### Q6. Command fuzz and fifty-year endless soak? DECIDED: 12 seeds, BALANCE_SOAK_FULL gate

- Fuzzer: 12 seeds × 160 commands covering every `CommandKind`, ~25% biased toward valid args; refusals must match pre-command digest; spectate keeps fuzzing rival labs.
- Endless: default **6 years × 2 seeds** every run; **`BALANCE_SOAK_FULL=1`** runs **50 years × 3 seeds** with auto `bailout` / `found-again` on pending player endings, monthly invariants, and final save/load.

### Q7. Bodies and Upward expansion slice? DECIDED: room + sector appeal kits

- NEXT later item: expansion eras get `expansions.bodies` / `expansions.upward` kits (room type + sector ids + appeal bonus permille) applied in `DisplayQueries.AppealParts` once `EraSystem.Open` is true.
- Why: self-contained from rivals (no shady/endings); data and rooms already shipped; avoids new commands or save fields.