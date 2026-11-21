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

### Q8. Specialist / open weights / sector pricing slice? DECIDED: (a) sector pricing first

- Options: **(a)** per-`SectorEntry` `pricePermille` + `SetSectorPrice`; **(b)** open weights; **(c)** specialist lines; **(d)** data sources (defer).
- Choice: **(a)** — default 1000 permille, config min/max, scales `LastRevenue` after share split; appeal unchanged (quality still wins customers).
- Why: smallest bible-aligned market slice; schema 12 migration; open weights and specialist lines remain later on the same NEXT row.

### Q9. Open weights slice? DECIDED: model flag + market permilles

- Options: (a) per-sector open listing; (b) `ModelState.openWeights` + `SetOpenWeights`, config `openWeightsRevenuePermille` / `openWeightsAppealBonusPermille`; (c) always-on for mini lines.
- Choice: **(b)** — toggle after train; revenue scales after sector price; quality scales before appeal square.
- Why: matches bible trade-off; schema 13 adds bool; cross-tested with sector pricing.

### Q10. First specialist line? DECIDED: code-specialist on code sectors

- Options: (a) image specialist; (b) `code-specialist` unlocked by `code-models`, bonus permille only in listed `specialistSectorIds`; (c) flagship-only sectors.
- Choice: **(b)** — line data in config; `SpecialistSystem` applies bonus in `QualityParts.FromCapability` only.
- Why: sectors and tech already exist; no new commands beyond training the line.

### Q11. Data sources v1? DECIDED: lab default + prep/capability permilles

- Options: (a) per-run command list; (b) `training.dataSources`, `SetTrainingDataSource`, stamp `dataSourceId` on model; (c) full lawsuit/shady track now.
- Choice: **(b)** — prep at train start, capability at finish; schema 14 migrates `internal` default.
- Why: bible data mix without shady; defers scrape/legal events.

### Q12. Open weights in quality breakdown? DECIDED: `OpenWeightsBoost` part

- Options: (a) leave open-weights appeal outside `QualityParts` (parts no longer sum); (b) add `OpenWeightsBoost` so `QualityParts.Total` matches `DisplayQueries.Quality`; (c) stop showing open weights in quality at all.
- Choice: **(b)** — same permille as revenue; appeal still flows through squared quality.
- Why: matches NEXT item 3 explain-every-number; `MarketBreakdownHardeningTests` cover specialist, sector price, and open weights sums.

### Q13. World map breakdown queries? DECIDED: reuse `WorldQueries` parts via `DisplayQueries`

- Options: (a) duplicate power/attention math in `DisplayQueries`; (b) lab-level rollups `WeeklyPowerBillParts` and `AttentionGainParts` over existing `PowerBill` / `AttentionGain`; (c) defer attention until Unity.
- Choice: **(b)** — systems design 2.8; `WorldBreakdownHardeningTests` assert part sums and save/load.
- Why: zero behaviour change; UI gets one import.

### Q14. Trained capability breakdown? DECIDED: `CapBase`..`CapLuck` + `FromDataSource` delta

- Options: (a) store data-source boost in state; (b) derive `FromDataSource = Capability − (CapBase + CapTech + CapCarryover + CapLuck)` on trained models; (c) skip until model sales UI.
- Choice: **(b)** — matches training finish order; synthetic source can show negative part.
- Why: bible data mix without new fields; pairs with Q11 data sources.

### Q18. Bodies and Upward training mechanics? DECIDED: capability + orbital speed kits

- Options: (a) defer to art pass; (b) `capabilityBonusPermille` on finished runs when the robotics arena is live; (c) `trainingSpeedBonusPermille` from the orbital uplink; (d) new commands.
- Choice: **(b)+(c)** — config on `expansions` kits; `CapExpansion` on models (schema 15); `TrainedCapabilityParts.FromExpansion`; speed in `TrainingSystem.SpeedPermille`.
- Why: NEXT “mechanics beyond appeal”; no rival planner changes.

### Q19. Envydia courting at 30% own-chip share? DECIDED: harden existing `CourtRivals`

- Options: (a) new event kind; (b) document + isolate tests for `ChipSystem.CourtRivals` / `VendorSystem.RunMonth`; (c) change share math.
- Choice: **(b)** — systems design 4.7 already logs `DecisionAction.Courted`; `EnvydiaCourtingHardeningTests` (9 facts).
- Why: gameplay already in rules; bubble/stack threads own triggers.

### Q20. Vendor keynote ambush? DECIDED: harden existing `OnKeynote`

- Options: (a) new UI event copy; (b) isolate lookback, month keynote, planner buy skip, save/load; (c) change ambush weeks.
- Choice: **(b)** — systems design 3.3/4.7; `VendorKeynoteAmbushHardeningTests` (9 facts).
- Why: gameplay already gates rival listing buys; no vendor config change.

### Q21. Envydia standing hits on chip milestones? DECIDED: harden tape-out, listing, delivery

- Options: (a) add breakdown queries; (b) assert one-shot training/listing flags and per-delivery drops; (c) change drop amounts.
- Choice: **(b)** — `VendorChipStandingHardeningTests` (9 facts) over `VendorStanding` flags and `VendorSystem.Change`.
- Why: bible 4.7 reactions already in rules; explainable via standing UI.

### Q22. Chip yield ramp? DECIDED: harden weekly `RampYields`

- Options: (a) defer to Unity chip UI; (b) engineers, test lab, max yield, save/load; (c) change ramp formula.
- Choice: **(b)** — `ChipYieldRampHardeningTests` (9 facts).
- Why: design 4.5 ramp is live gameplay; no new commands.

### Q17. Frontier research cost breakdown? DECIDED: base + growth parts

- Options: (a) show only `FrontierCost` total; (b) `FrontierCostParts` with `Base`, `FromGrowth`, `Capped`; (c) list each growth multiplier step.
- Choice: **(b)** — `Total` matches `EndingSystem.FrontierCost`; growth permille explained as `FromGrowth`.
- Why: endless play UI (systems design 5.4); 9 hardened facts.

### Q16. Owner chip software breakdown? DECIDED: lab + tech + cap clip

- Options: (a) store cap separately in state; (b) `ChipQueries.SoftwareParts` with `FromLab`, `FromTech`, `CapClip`; (c) defer until Unity chip UI.
- Choice: **(b)** — `Total` equals `EffectiveSoftware`; batch compute cross-tested.
- Why: systems design 4.6/4.11 explainable software permille.

### Q15. Second specialist line? DECIDED: `image-specialist` on `conv-nets`

- Options: (a) image specialist on `multimodal`; (b) unlock with `conv-nets`, home sectors `image-recognition` + `image-design`; (c) wait for era-content art.
- Choice: **(b)** — parallel to code-specialist pattern; locked line + sector bonus permille.
- Why: completes NEXT “specialist lines” slice without rival training planner changes (`LabPlanner` still skips specialist lines).

### Q23. Grey compute payout timing? DECIDED: weekly while op active

- Options: (a) lump sum on resolve; (b) weekly `GreyPayout` each op week; (c) defer payout to stash only.
- Choice: **(b)** — `ShadySystem` pays each week; resolve clears op without lump grant.
- Why: makes long grey runs costly/opaque; crosses finance ledger.

### Q24. Keynote ambush vs chip listing? DECIDED: block priced listing only

- Options: (a) freeze all chip commands; (b) refuse `SetChipListing` with price while ambush active; (c) block delist too.
- Choice: **(b)** — `CommandError.KeynoteAmbush` on new price/limit; delist with count 0 still allowed.
- Why: rivals can exit a bad list; matches vendor narrative.

### Q25. Orbital uplink passive compute? DECIDED: permille per tile in split

- Options: (a) training-speed only; (b) `passiveComputePermillePerTile` adds to `LabQueries.Split` general; (c) new holding type.
- Choice: **(b)** — config 60‰ per tile after Upward era; `PassiveOrbitalGeneralCompute`.
- Why: gives expansion rooms rackless compute; crosses fleet split.

### Q26. Museum beyond retire cap? DECIDED: salvage research on prune

- Options: (a) delete silently; (b) grant `researchPermillePerCapability` on pruned retirees; (c) reputation only.
- Choice: **(b)** — `MuseumSystem.RunWeek` grants research + `TechResearched` ref `museum-salvage`; crosses frontier spend.
- Why: endless play loop; no schema bump (config only).

### Q27. Envydia courting antitrust attention? DECIDED: home bloc attention on maker

- Options: (a) rival attention only; (b) `courtingMakerAttentionPermille` on chip-dominant lab home bloc; (c) global world event.
- Choice: **(b)** — with existing `CourtRivals` standing boost; `AttentionRose` event; crosses `GovernmentSystem.CloseMonth`.
- Why: dominant silicon draws scrutiny at home; config-only.

### Q28. Robotics arena passive compute? DECIDED: mirror orbital on bodies kit

- Options: (a) capability only; (b) `passiveComputePermillePerTile` on bodies kit; (c) same as uplink shared helper.
- Choice: **(b)** — 45‰ default; `PassiveExpansionGeneralCompute` sums bodies + upward.
- Why: symmetric expansion payoff; crosses training split.

### Q29. Bodies capability on specialist lines? DECIDED: sector overlap gate

- Options: (a) all lines get bodies bonus; (b) specialists need specialist sector ∈ kit sectors; (c) disable for all specialists.
- Choice: **(b)** — general lines unchanged; `code-specialist` / `image-specialist` off unless overlap (config or tweak).
- Why: robotics arena boosts robot sectors, not every specialist by default.

### Q30. Keynote ambush vs chip orders? DECIDED: block `OrderChips` during ambush

- Options: (a) orders like rentals (exempt); (b) same gate as listing; (c) only block first order after keynote.
- Choice: **(b)** — `KeynoteAmbush` on `OrderChips`; tape-out / fabrication continue.
- Why: launch window pressure on go-to-market; crosses vendor + foundry flow.

### Q31. Keynote ambush vs buying listed chips? DECIDED: block buyer during ambush

- Options: (a) exempt listed buys; (b) refuse `BuyListedChips` on the buyer while ambush active; (c) block seller listings only.
- Choice: **(b)** — after listing validation, same `KeynoteAmbush` error; seller ambush does not block buyer.
- Why: rivals cannot stockpile during buried launch; crosses rival trade + planner skip.

### Q32. Keynote ambush vs new chip designs? DECIDED: block `StartChipDesign`

- Options: (a) block all chip commands; (b) block new designs; tape-out/cancel continue; (c) allow redesigns.
- Choice: **(b)** — in-progress designs still tape out; cancel allowed.
- Why: keynote freezes go-to-market starts, not fab work in flight.

### Q33. Courting maker attention frequency? DECIDED: once per quarter with court log

- Options: (a) every `CourtRivals` call; (b) only when vendor logs `Courted` this quarter; (c) once per month.
- Choice: **(b)** — attention tied to same guard as vendor court log line.
- Why: antitrust spike is a quarterly story beat, not a weekly tick.

### Q34. Own-chip share vs expansion passive compute? DECIDED: passive excluded from denominator

- Options: (a) passive dilutes share like other general compute; (b) subtract passive expansion from general in `OwnChipsShare`; (c) add passive to own chips.
- Choice: **(b)** — uplink/arena passive still in `Split` for training; share math ignores it.
- Why: orbital signal should not mask silicon dominance for Envydia courting; crosses Q25/Q28 + Q27.

### Q35. Data source finish consequences (beyond prep/cap)? DECIDED: corruption, heat, home attention

- Options: (a) finish applies only prep/cap modifiers; (b) per-source finish deltas on corruption, lab heat, and home attention; (c) incident roll on finish.
- Choice: **(b)** — `FinishCorruption` on model (capped), `FinishHeat` on lab, `HomeAttentionPermille` on home bloc when finish fires; `AttentionRose` event when attention moves.
- Why: synthetic/open-weights risk is not just training cost; crosses shady heat + world attention + serving reputation.

### Q36. Gauntlet sector bracket vs expansion appeal? DECIDED: scale bracket score, not crown

- Options: (a) expansion only in market appeal; (b) multiply sector bracket entry score by bodies/upward kit bonus when era open and room present; (c) crown uses bonus too.
- Choice: **(b)** — `GauntletSystem` scales quality-based bracket score via `ExpansionSystem.ScaleAppeal`; crown still raw capability; era gate matches market appeal.
- Why: robotics/orbital kits should swing sector champions without rewriting crown; crosses Q28/Q25 + gauntlet records.

### Q37. Freeze vs training at home? DECIDED: block `StartTraining` when home bloc frozen

- Options: (a) training exempt; (b) refuse `StartTraining` like other permit checks; (c) freeze only blocks hires/land.
- Choice: **(b)** — `CommandProcessor.StartTraining` checks home bloc `PermitFrozen`; foreign-site bloc freeze does not affect home training.
- Why: government freeze should stop new runs, not just GPUs; aligns with rent/found permit gates.

### Q38. Open weights vs data source? DECIDED: synthetic blocks `SetOpenWeights`

- Options: (a) any trained model can open weights; (b) `blocksOpenWeights` on `training.dataSources`, refuse with `DataSourceBlocksOpenWeights`; (c) open weights only on internal.
- Choice: **(b)** — synthetic mix cannot publish weights; internal/licensed may; stamp on model `dataSourceId`, not lab default.
- Why: risky data cannot be “open sourced” credibly; crosses Q9 open weights + Q35 synthetic finish risks.

### Q39. Global chip shortage from world news? DECIDED: timed crunch on prices and foundry

- Options: (a) flavor-only `card-shortage` headline; (b) `chipShortageStrengthPermille` on world events + `GameState` clock; (c) random monthly roll.
- Choice: **(b)** — `ChipShortageSystem` scales GPU rent/buy, reserved deposit, fab orders, listed buys; shrinks effective foundry room; `ChipShortageBegan`/`Ended` events; schema **16**.
- Why: DESIGN.md world-map tail (chip shortages) with visible economy; crosses `WorldNews`, vendor compute, and chips without new player commands.