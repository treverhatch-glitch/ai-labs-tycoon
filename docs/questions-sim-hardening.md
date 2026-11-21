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

### Q40. Sandbox wing catches incidents? DROPPED — era room perks (`b45d2c8`)

- Overlaps PR #6 `roomPerks` + `rolePerks` (`IncidentCut`, 90% combined cap). Reverted on `cursor/rules-side-items` (`45657b7`). Log: agent store `internal/q40-vs-era-room-perks.md`.

### Q41. Home freeze stalls active training? DECIDED: progress pauses (no schema)

- Options: (a) freeze only blocks commands (Q37); (b) `TrainingSystem.RunWeek` returns before progress when home is `Freeze`; (c) auto-cancel runs.
- Choice: **(b)** — in-progress runs neither gain capability nor alignment compute; ledger/compute stats still roll; research and release commands unchanged.
- Why: NEXT freeze consequences beyond purchases; world-map only; avoids room/perk/press/talent/gauntlet/rival threads.

### Q42. Chip shortage cuts Envydia supply? DECIDED: inverse strength on monthly pool

- Options: (a) prices only (Q39); (b) `VendorSystem.RunMonth` supply scaled by `2000 − strength` floored at `vendorSupplyFloorPermille`; (c) block queue.
- Choice: **(b)** — same active window as Q39; allocations/queue shrink; keynote month still applies first.
- Why: bible “card shortage” hits gamers buying GPUs, not just list prices; no new schema.

### Q43. Home freeze blocks research? DECIDED: no spend, no accrual

- Options: (a) block `ResearchTech` only; (b) also skip `RunResearch` accrual; (c) drain points.
- Choice: **(b)** — mirror Q41; foreign blocs unaffected; Letter/Hearing/Audit still allow research.
- Why: freeze consequences beyond purchases/commands; stays off era room/perk threads.

### Q44. Home freeze blocks data-source changes? DECIDED: command gate only

- Options: (a) freeze default + block `SetTrainingDataSource`; (b) rewind active runs; (c) block open weights.
- Choice: **(a)** — lab default stays; in-flight runs keep `dataSourceId`; open weights and sector price untouched.
- Why: pairs Q11/Q38; freeze as permit sanction, not new risk track.

### Q45. Training failed runs? DECIDED: weekly collapse dice + scrap command

- Options: (a) deterministic fail at low alignment only; (b) weekly permille risk from data source, engineers and alignment share; (c) auto-refund prep cash.
- Choice: **(b)** — `TrainingSystem.RunWeek` rolls after `failAfterProgressPermille`; `ModelStatus.Failed`, `TrainingFailed` event, `ScrapFailedRun` command removes the wreck; schema **17**.
- Why: DESIGN training row (failed runs); not recipes yet; avoids era talent/press and rules-forward bubble/shady depth.

### Q46. Training recipes? DECIDED: named presets via `StartTrainingRecipe`

- Options: (a) player macros only (UI); (b) config `training.recipes` with line, data source, alignment share, compute permille; (c) rival-only presets.
- Choice: **(b)** — `CommandKind.StartTrainingRecipe`; six presets in `default-config.json`; stamps `ModelState.recipeId`; schema **18**; plan labs keep `StartTraining`.
- Why: DESIGN training row (recipes); pairs Q11 data sources; no new risk track beyond existing finish/fail dice.

### Q47. Scrape and rival-distill data sources? DECIDED: era-gated mixes + distill target lab

- Options: (a) scrape only; (b) distill without rival pick; (c) `scrape` (1999) and `distill-rival` (2004) with `SetDistillTarget` and `DistillTargetRequired` gate.
- Choice: **(c)** — `TrainingQueries.SourceUsable` + `distillMinLeadPermille`; rival recipe `rival-distill`; events `DistillTargetSet`; save fields `distillTargetLabId`, `distillSourceLabId`.
- Why: completes data-source ladder before rules-forward talent; crosses open-weights blocks on risky mixes (Q38) without shady jobs.

### Q48. Plan labs vs training collapse? DECIDED: same dice, separate stream

- Options: (a) rivals immune (skip `MaybeFail`); (b) shared `trainingCollapse` stream; (c) rivals roll on `rng.rivals`.
- Choice: **(c)** — `TrainingSystem.CollapseRng` uses `TrainingCollapse` for the player and `Rivals` for `LabController.Plan`; failed plan runs still auto-clear each week so the planner retries.
- Why: rivals felt immune after Q45; separate stream keeps player golden/determinism isolated from rival collapse rolls.

### Q49. Rivals vs scrape/distill? DECIDED: `playerOnly` on shady mixes

- Options: (a) rivals ignore lab default; (b) `dataSources.playerOnly` on scrape and distill-rival; refuse commands and force `internal` for plan `StartTraining`; (c) recipes only.
- Choice: **(b)** — `PlayerOnlyDataSource` on `SetTrainingDataSource`; `PlanDataSourceId`; recipes and distill target refused on plan labs.
- Why: bible treats scrape/distill as player/shady path; rivals never commit crimes.

### Q50. Protest world headlines? DECIDED: attention bump via `ProtestSystem`

- Options: (a) headlines only; (b) `protestAttentionPermille` on dated events (home bloc or named `protestBlocId`); (c) permit revolt state machine.
- Choice: **(b)** — `ProtestSystem.OnWorldEvent` after `WorldNews`; `teen-chatbot` and `pause-letter` ship in config; `AttentionRose` when attention moves.
- Why: DESIGN world-map protest gap without new save schema or permit-revolt depth.

### Q51. Research forks, dead ends, and patents? DECIDED: `ResearchForkConfig` + `FilePatent`

- Options: (a) flavor-only mutually exclusive tech descriptions; (b) config `research.forks` block sibling techs after pick; (c) patents as passive rep bump only.
- Choice: **(b)+(c)** — `blockedTechs` on `LabState` when a fork tech is researched; shipped fork `classical-ml` (`bayes-nets` / `svm`); `go-fish-heuristic` dead-end flavor tech; `CommandKind.FilePatent` spends cash, blocks rivals on that tech for `research.patents.durationWeeks`; schema **19**.
- Why: DESIGN research row (branching bets, dead ends, patent races) without rival-AI patent logic yet; pairs Q48–Q49 plan/player fairness.

### Q52. Inter-lab compute rental? DECIDED: `RentPeerCompute` on HQ fleet

- Options: (a) flavor-only cloud deals; (b) extend vendor reserved contracts; (c) weekly leases between labs with landlord revenue.
- Choice: **(c)** — `peerComputeLeases` on `LabState`; renter gains compute, landlord HQ GPUs taken off their stack; `compute.peerLeaseRentPermille`; schema **20**.
- Why: bible 4.8 landlord hook without chip-listing complexity; mirrors vendor reserved rent pattern.

### Q53. Sector incident headline text? DECIDED: `safety.sectorIncidentHeadlines` (text hook only)

- Options: (a) era headlines only; (b) dedicated headline list keyed by sector; (c) share `safety.sectorFlavours` with era (Q188–Q190).
- Choice: **(b)** — `SafetyQueries.SectorIncidentHeadlines`; minimal `SafetySystem`/`EventText` hook; **no** schema, **no** economics. Era thread owns `safety.sectorFlavours` economics on row 21 (`cursor/sector-flavours-af2f`).
- Why: avoid config-key collision while shipping sector-specific incident copy early.

### Q54. Device workshop room? DECIDED: `market.deviceWorkshop` appeal bump

- Options: (a) decor only; (b) passive compute; (c) devices-sector appeal once era live, room built, and a model is released in `devices`.
- Choice: **(c)** — new `device-workshop` room type; `DeviceWorkshopSystem.AppealBonusPermille` in `DisplayQueries`; no save schema.
- Why: DESIGN “device workshop” row gets a minimal consumer-hardware hook without a full product line sim.

### Q55. Cloud chip landlord royalty? DECIDED: `chips.cloudLandlordSharePermille`

- Options: (a) listing price only; (b) weekly royalty to design lab on another lab's cloud chips; (c) full transfer of cloud rent.
- Choice: **(b)** — `CloudChipLandlordSystem.RunWeek` after finance; share of `CloudBatchWeeklyRent`; skips self-host and bankrupt labs.
- Why: systems-design 4.8 “cloud landlords” gap without new holdings or schema.

### Q56. GPU rent breakdown query? DECIDED: `LabQueries.GpuRentParts`

- Options: (a) UI guesswork; (b) four-line breakdown (vendor, reserved, cloud, peer); (c) new ledger lines.
- Choice: **(b)** — `GpuRentBreakdown` mirrors `WeeklyGpuRent`; pairs Q52 peer and Q55 cloud.
- Why: hardening bar for compute spend transparency; no gameplay change.

### Q57. Synthetic data lab room? DECIDED: `training.syntheticDataLab`

- Options: (a) decor only; (b) cuts synthetic finish corruption and weekly collapse risk; (c) new data source.
- Choice: **(b)** — Harness-era `synthetic-data-lab`; `SyntheticDataLabSystem`; no new schema.
- Why: bible synthetic-data row; pairs device workshop room pattern (Q54).

### Q58. Weekly running-cost breakdown? DECIDED: `LabQueries.RunningCostParts`

- Options: (a) HUD only; (b) salaries / rooms / GPU rent / upkeep / power lines; (c) per-ledger export.
- Choice: **(b)** — sums to `WeeklyRunningCost`; peer rent stays outside (billed separately).
- Why: DESIGN “explain every number” tail.

### Q59. Peer landlord revenue query? DECIDED: `PeerComputeSystem.WeeklyLandlordRevenue`

- Options: (a) renter bill only (Q56 peer line); (b) landlord revenue matching `RunWeek` payouts; (c) new ledger.
- Choice: **(b)** — read-only mirror of `Finance.Earn` from peer leases; no schema.
- Why: landlord side of Q52 for UI and planner hints.

### Q60. Training collapse risk breakdown? DECIDED: `TrainingQueries.WeeklyFailParts`

- Options: (a) single permille only; (b) base / data / engineer / alignment parts with clamped effective; (c) separate rival stream.
- Choice: **(b)** — `TrainingSystem.WeeklyFailPermille` delegates; respects Q57 synthetic lab cuts on data line.
- Why: pairs Q45–Q49 training failure hardening.

### Q61. Protest permit revolt (lite)? DECIDED: `world.attention.protestRevolt*`

- Options: (a) full permit state machine; (b) amplify protest events when attention ≥ threshold; (c) skip.
- Choice: **(b)** — bonus permille on `ProtestSystem` bump; no schema (Q50 extension).
- Why: world-map protest depth without audit/freeze state machine (rules-forward owns audit).

### Q62. Chiplet packaging die? DECIDED: `chiplet-mcm` + foundry room multiplier

- Options: (a) cosmetic die only; (b) extra foundry slots, higher derive stats, own-silicon training bonus; (c) new chip target.
- Choice: **(b)** — Harness tech `chiplet-packaging`; `ChipQueries.FoundrySlots`; chiplet bonus stacked on in-house training share.
- Why: bible “chiplets” row; pairs reticle dies without new save fields.

### Q63. Capability domains query? DECIDED: `market.capabilityDomains`

- Options: (a) UI-only grouping; (b) `CapabilityDomainQueries` best + footprint; (c) new leaderboard schema.
- Choice: **(b)** — language/vision sector lists; read-only aggregates on released models.
- Why: explainable domain scores for planner/UI without gauntlet changes.

### Q68. Domain leader events? DECIDED: `EventKind.DomainLeaderChanged`

- Options: (a) silent refresh; (b) event log when leader lab changes; (c) world news.
- Choice: **(b)** — `Events.Add` from `CapabilityDomainSystem.RunWeek`; `EventText` line.
- Why: makes Q65 visible to player without Unity UI.

### Q69. Vision domain breadth? DECIDED: add `fraud-detection` sector

- Config extension only; reuses Q65 leader/appeal path.

### Q77. Model mood cuts market quality? DECIDED: overworked + corrupted permilles in `DisplayQueries`

- Why: Unity mascots were presentation-only; starving GPUs should hurt quality scores the market uses (DESIGN mood fiction).
- Choice: **`safety.overworkedQualityPermille`** and **`safety.corruptedMoodQualityPermille`**; `QualityParts` takes optional `LabState`; `MoodAdjustment` breakdown part sums with total.
- Thirteen `ModelMoodQualityTests`. Golden unchanged.

### Q79. Museum reputation for full lines? DECIDED: `MuseumConfig.ReputationPerFullLine`

- Why: Endless-play museum slice had salvage-only hooks; a full wing per line should buff market reputation (NEXT museum bullet).
- Choice: weekly **`reputationPerFullLine`** when retired count on a line equals **`perLine`** cap (after pruning); `MuseumSystem.RetiredOnLine` query; clamped reputation.
- Thirteen `MuseumReputationTests`. Golden unchanged.

### Q78. Domain challenger appeal? DECIDED: near runner-up hook beside leader boost

- Why: Capability domains had a winner bonus only; runner-up labs near the leader deserve a smaller market pull.
- Choice: per-domain **`challengerAppealPermille`** + **`challengerNearLeaderPermille`**; `CapabilityDomainSystem.ChallengerAppealPermille`; `AppealBreakdown.DomainChallengerBoost`.
- Thirteen `DomainChallengerAppealTests`. Golden unchanged.

### Q76. Aggregated power bill lines? DECIDED: `WorldQueries.WeeklyPowerBillTotals`

- Options: (a) sum in UI from per-holding rows; (b) lab-wide Grid/Private/Cooling/Colo rollup; (c) new ledger lines.
- Choice: **(b)** — `PowerBillTotals`; `WeeklyPowerBill` delegates to rollup total.
- Why: cloud-holding rent accrual slice from unity-candidates; pairs Q58 running-cost power line.

### Q73. Combined landlord income? DECIDED: `LabQueries.LandlordIncomeParts`

- Options: (a) sum in UI; (b) peer + cloud royalty lines; (c) new ledger.
- Choice: **(b)** — `WeeklyLandlordIncome`; matches Q59 + Q71 payouts.
- Why: single landlord HUD row for chip designer who also leases GPUs.

### Q74. Protest bump query? DECIDED: `ProtestSystem.ProtestAttentionBump`

- Options: (a) duplicate Q61 math; (b) shared function for UI preview and `OnWorldEvent`; (c) skip.
- Choice: **(b)** — extract revolt bonus from Q61; event amount equals query.
- Why: planner/world UI can show protest impact before firing headline.

### Q75. Training share breakdown? DECIDED: `LabQueries.TrainingShareParts`

- Options: (a) total only; (b) bought vs own + in-house + chiplet bonus permille; (c) training UI only.
- Choice: **(b)** — sums to `TrainingShareOf`; chiplet line reuses Q62 detection.
- Why: explain training compute allocation alongside Q60 fail parts.

### Q71. Cloud landlord revenue query? DECIDED: `CloudChipLandlordSystem.RevenueParts`

- Options: (a) ledger guesswork; (b) gross cloud rent + royalty matching `RunWeek`; (c) new schema.
- Choice: **(b)** — `WeeklyLandlordRevenue` and `RevenueBreakdown`; refactor `RunWeek` to same math.
- Why: landlord-side mirror of Q56 renter `GpuRentParts.Cloud` and Q55 payout.

### Q72. Planner stale GPU query? DECIDED: `Fleet.StaleGpuBatch`

- Options: (a) duplicate planner logic in UI; (b) shared query used by `LabPlanner.Refresh`; (c) skip.
- Choice: **(b)** — extract stale batch pick; planner refresh unchanged behaviour.
- Why: bible sell-oldest-gen tuning hook for planner UI without duplicating tier-gap rules.

### Q70. Third capability domain? DECIDED: `enterprise` domain

- Options: (a) third domain grouping expert/search sectors; (b) defer to UI; (c) gauntlet hook.
- Choice: **(a)** — `enterprise` with `expert-systems` and `spam-search`; same leader/appeal rules as Q65.
- Why: bible “capability domains” beyond language/vision without new schema.

### Q67. Language domain breadth? DECIDED: add `coding-agents` sector

- Options: (a) third domain; (b) extend language `sectorIds`; (c) UI only.
- Choice: **(b)** — config only; leaders and appeal follow Q65 automatically.
- Why: bible coding-agents row fits language without new schema.

### Q66. Optical chip cooling? DECIDED: `optical` cooling option

- Options: (a) tech-only effect; (b) `optical` cooling option gated on `optical-interconnect`; (c) new node.
- Choice: **(b)** — higher compute, lower kW on derive; no schema.
- Why: pairs Q64 exotic silicon path and harnesses `optical-interconnect` tech.

### Q65. Capability domain leaders? DECIDED: `CapabilityDomainSystem` + schema 21

- Options: (a) queries only; (b) weekly leader snapshots and appeal in domain sectors; (c) gauntlet brackets.
- Choice: **(b)** — `GameState.domainLeaders`; `RunWeek` before market; `DisplayQueries.DomainLeaderBoost`.
- Why: bible “capability domains” as a live system; golden updated for appeal path.

### Q64. Exotic foundry node gate? DECIDED: `ChipNodeConfig.requiresTech`

- Options: (a) new n2 node; (b) n3 gated on `chip-codesign`; (c) skip exotic nodes.
- Choice: **(b)** — `ChipQueries.NodeUnlocked`; planner and `StartChipDesign` enforce; listings unchanged.
- Why: bible “exotic nodes” hook without new processes or schema.