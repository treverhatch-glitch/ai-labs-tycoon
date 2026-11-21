# Design questions

Forks in the road that came up while building, with the option taken. Per the owner's note, nothing here stopped the
build: each entry picks the option that best fits the bible (v0.2) and
[reference/ai_labs_decisions.md](reference/ai_labs_decisions.md), and every one is easy to reverse (a number or a
flag in `Rules/Data/default-config.json`, unless it says otherwise). Hatch's assistant reviews this log; change an
answer by editing the data and the line below.

Status: **DECIDED** = taken and built, **OPEN** = needs a human before it can be built (none right now).

## Eras and content (batch 1)

### Q1. How fast do training costs grow per era? DECIDED: modest steps now

- Options: (a) the bible's about x10 per era; (b) modest steps until hardware generations exist; (c) flat.
- Recommendation and choice: **(b)** x1, 1.5, 2.5, 5, 10, 20, 40, 80, 120 (`trainingCostPermille` per era).
- Why: with fixed GPUs a x10 jump per era bankrupts every autopilot lab in 2009 to 2016, and hardware generations (NEXT item 6) are what should pay for x10; raise the numbers when they land.

### Q2. Are Bodies (2029) and Upward (2033) in the base game? DECIDED: in the data, flagged `expansion`

- Options: (a) leave them out until the expansions; (b) ship their data now, flagged; (c) ship them unflagged.
- Recommendation and choice: **(b)** both eras have techs, sectors, rooms and events, and `expansion: true` lets a menu hide or gate them.
- Why: the decisions file calls them expansions, and having the data now proves the table scales past era 6 and keeps a 40-year run busy to 2035.

### Q3. Which eras can a new game start in? DECIDED: 1995 and 2009

- Options: (a) 1995 only; (b) 1995 plus 2009 as the bible's quick start; (c) every era; (d) also the 1956 prologue.
- Recommendation and choice: **(b)** `startable: true` on `dial-up-days` and `punchline`; the 2009 kit is cash x15, grant x10, the `strip-mall` lot and all 28 first-era techs researched.
- Why: the bible names 2009 as the quick start and 1995 as the default; later eras need hardware and funding systems that do not exist yet, and the 1956 prologue is a different game.

### Q4. What does a sector's `poolPerWeek` mean? DECIDED: the pool in the year it opens, then slowing growth

- Options: (a) keep "the pool in 1995" and let every pool compound from the start; (b) the pool in its opening year, growing only afterwards, with a decay on the growth rate; (c) hand-written pool tables.
- Recommendation and choice: **(b)**: `growthDecayPermille` multiplies the growth rate for every year since opening (85% for most, 90% for game AI and bots). A game started in 2009 gets the grown pools.
- Why: (a) put a 2034 sector at billions a week before it opened; (b) keeps every later sector a believable size, and the numbers stay in data.

### Q5. Is an era a window or a floor for techs and rooms? DECIDED: a floor

- Options: (a) window (the old tech disappears when the era ends); (b) floor (open at the era's start, open forever).
- Recommendation and choice: **(b)** `EraSystem.TechOpen` and `RoomOpen`; refusal is `CommandError.EraLocked` with the era and year in the message.
- Why: players never lose something they bought into, and old rooms still work; retiring rooms can come with the museum or hardware work.

### Q6. Where do incident headlines live? DECIDED: per era

- Options: (a) one global list in `safety`; (b) a list on each era.
- Recommendation and choice: **(b)**; the era at the incident's tick picks the list (6 to 8 headlines each; the old eight became the first era's).
- Why: a 2030 model should not fail like a 1997 one, and it keeps jokes with the era that owns them. The old `safety.incidentHeadlines` field is gone (a config change, not a save change: saves store only the pick number).

### Q7. How much can world events do? DECIDED: dated, deterministic, flavour or one pool shock

- Options: (a) flavour text only; (b) dated events that scale one sector's pool once; (c) random events and player choices (hearings, bubbles).
- Recommendation and choice: **(b)** 50 dated events from 1995 to 2035; no dice, so adding an event never moves another system's rolls (a test holds this).
- Why: choices and the Bubble meter belong to the funding-ladder and bubble work in NEXT; this gives the news ticker and the crash now without new state.

### Q8. Do the new roles add new mechanics? DECIDED: no, they map onto the four functions

- Options: (a) new mechanics per role; (b) 11 new roles that each fill one of Research, Engineering, Safety and Sales.
- Recommendation and choice: **(b)** labeller, data-engineer, recruiter, red-teamer, product-lead, alignment-fellow, agent-wrangler, data-gardener, harness-operator, roboticist, mission-controller.
- Why: keeps the rules small and the planner simple; real per-role effects come with staff-as-people (NEXT item 9).

### Q9. Rooms with no mechanical role (whiteboard, press room, device workshop)? DECIDED: skipped

- Options: (a) add them as decor; (b) leave them out until the room that does something exists.
- Recommendation and choice: **(b)**; 17 new room types ship, each seating a role or holding GPUs.
- Why: a room a player builds should change something; decor can come with the Unity pass.

### Q10. How should the autopilot and rivals grow across 40 years? DECIDED: wealth in first-era dollars, and stop growing when overstretched

- Options: (a) keep time-based growth only; (b) count wealth in first-era dollars and add a brake; (c) hand-tuned plans per era.
- Recommendation and choice: **(b)**: a lab's wealth-driven growth counts cash scaled down by the era's training cost, and it does not hire or rent beyond its starting plan while running costs exceed 150% of weekly revenue.
- Why: without it cash inflation made xAy buy 250,000 GPUs, and thin years bankrupted rivals; with it six seeds over 40 years finish with nobody bankrupt.

### Q11. Does research speed change with the era? DECIDED: yes, per era

- Options: (a) flat; (b) `researchPermille` per era (x1 to x60).
- Recommendation and choice: **(b)**.
- Why: tech costs rise with the era and a flat researcher could not keep up; this is the "talent and tools scale" idea without a new system.

### Q12. Any new rival characters? DECIDED: no

- Options: (a) add Goggle DeepMined, Macrosoft, Metta and others now; (b) keep the three rivals.
- Recommendation and choice: **(b)**; the rivals' plans got the new roles and sectors (OpenAye, AntHopticsAI and xAy skip a few sectors that do not fit them).
- Why: the cast is its own piece of work (names, plans, entry years) and the decisions file puts it in the world-map and funding items.

### Q13. How big do capabilities get? DECIDED: up to about 10^5 to 3x10^5 by 2034

- Options: (a) cap near 10^4; (b) let the tech bonuses carry it to 10^5 and tie sector bars to what a flagship reaches in each era.
- Recommendation and choice: **(b)**; a test requires every sector's `minCapability` to sit under 80% of what a flagship can reach with that era's techs.
- Why: it gives each era a ladder to climb and makes "a sector nobody can ever enter" a failing test.

### Q14. Does the first era need more than 13 techs? DECIDED: yes, 28

- Options: (a) leave 13 and accept research stopping in 2002; (b) add content that lasts to 2009.
- Recommendation and choice: **(b)**: 15 more techs in the 1995 to 2008 stretch (the web-era toolbox: bayes nets, SVMs, ensembles, crawling, crowd labelling and more).
- Why: it is the exact gap NEXT.md warned about, and a test (research keeps flowing every six years) now guards it.

### Q15. Who gets the start kit in a 2009 game? DECIDED: everyone, with plans offset to the start tick

- Options: (a) only the player; (b) every lab gets the researched techs and rivals' plan clocks start at 2009.
- Recommendation and choice: **(b)**; rival plans do not act on weeks before the game began.
- Why: a rival that had to research the 1995 tree again while the player has it would be an odd start.

## Systems design forks (batch 2)

Q16 to Q34 are the 19 forks decided in the systems design (D1 to D19), copied here so there is one place to override. Where a fork is for a later phase the line says so; the choice is recorded now so later work does not drift.

### Q16. Real countries or fictional blocs on the world map? DECIDED: real countries, blocs kept as a rename pack

- Options: (a) real country names with fictional agencies and politicians; (b) Fable's fictional blocs (Pacific Federation, Continental Union, ...); (c) both, with the blocs as a rename pack.
- Recommendation and choice: **(a)**, with (b) kept as a data rename pack (`world.blocs` names and agencies are data).
- Why: the bible's recommendation, and Hatch's "deal with China" only reads with real names; no real leaders appear.

### Q17. Where does hardware live: placed explicitly, or auto-filled? DECIDED: placed explicitly

- Options: (a) every batch has a location the player chooses; (b) one pooled fleet filling slots automatically.
- Recommendation and choice: **(a)**: every `HardwareBatch` has a `HoldingId` (-1 is the HQ).
- Why: the bible makes site choice (power, cooling, export rules, attention) the world map's decisions; auto-fill would hide them.

### Q18. Hardware as batches with generations, or keep a single GPU count? DECIDED: batches with generations

- Options: (a) batches by generation and location; (b) one count per location, all the same generation.
- Recommendation and choice: **(a)**: Envydia's seven generations (VooDude 1995 to Blackwall B9000 2026) are data in `compute.gpuGenerations`.
- Why: the bible's "each new generation makes your old fleet worth less" and own chips need mixed hardware; this is the one state refactor and is done first.

### Q19. One shady meter or two (Heat and Exposure)? DECIDED: one Heat plus per-bloc Attention

- Options: (a) one lab-wide `Heat` for the shady trail plus per-bloc `Attention` for legal scrutiny; (b) separate Heat, Exposure and Attention; (c) a single meter for everything.
- Recommendation and choice: **(a)**. Attention is built now (government attention per bloc); Heat arrives with the shady path.
- Why: the bible uses both words for the same trail; "low clicks, big decisions" favours fewer meters, and keeping legal scrutiny per bloc keeps site choice meaningful.

### Q20. Shady board: always open, or only as temptations when broke? DECIDED: always open, temptations as nudges

- Options: (a) always open, temptations as nudges when runway is short; (b) temptations only (Fable v0.1).
- Recommendation and choice: **(a)**; not built yet (phase G), recorded so the design does not drift.
- Why: Hatch decided on a real shady path in v1; the bible's v0.2 recommendation is a real, chooseable way to play.

### Q21. Can rivals take shady jobs? DECIDED: never for parody labs

- Options: (a) never for parody labs; (b) yes, with lower appetite; (c) only fictional startups (later).
- Recommendation and choice: **(a)**, with (c) left for the procedural startups later.
- Why: the bible's parody guardrail: parodies never commit crimes; the dark path belongs to the player and fictional characters.

### Q22. Stolen money: off-books stash or straight into cash? DECIDED: off-books stash, laundered at 70%

- Options: (a) separate stash, laundered into cash at 70%; (b) straight into cash on a ledger line.
- Recommendation and choice: **(a)**. The ledger lines `GreyRevenue` and `Laundered` already exist (schema 2) so the stash can land without a migration.
- Why: keeps "cash = founding cash + ledger" exact and gives Heat a second lever (laundering), matching the bible's "untraceable cash outside the books".

### Q23. Do endings pause the rules? DECIDED: no, rules never block

- Options: (a) rules never block; consequences apply at once; the host pauses; a command picks continuation where state is stuck; (b) `Step()` refuses until the player chooses.
- Recommendation and choice: **(a)**; not built yet (phase F).
- Why: Hatch's "endings do not end the game"; headless soaks and replays keep working; the host owns pacing.

### Q24. After selling the lab (Exit), what happens to it? DECIDED: it stays as a plan-run rival

- Options: (a) it stays in the world as a plan-run rival under new owners, and the player can found again with the proceeds; (b) it is removed; (c) the player keeps a share.
- Recommendation and choice: **(a)**; not built yet (phase F).
- Why: the bible's "it is someone else's problem" becomes literal and the world stays continuous (MGT2-style continue); shares wait for the funding ladder.

### Q25. Endless content once the tech tree runs out? DECIDED: repeatable frontier levels

- Options: (a) repeatable frontier levels with growing cost; (b) nothing new, the economy plays on; (c) procedural techs.
- Recommendation and choice: **(a)**; not built yet (phase F).
- Why: simplest thing that keeps capability and money meaningful forever (the Console Empire lesson in the bible); procedural techs can come later on top.

### Q26. Envydia: a lab that trains models, or a vendor? DECIDED: a vendor

- Options: (a) vendor only, with allocation, standing, price and a log; (b) a full lab that also trains.
- Recommendation and choice: **(a)**. This batch builds the generations Envydia sells; its allocation and standing are phase C.
- Why: DESIGN.md and the bible cast Envydia as the chip baron that "sells the GPUs"; landlords that also train (Macrosoft, Amazin') are a later archetype.

### Q27. Selling your chips: open listings or bilateral deals? DECIDED: open listings

- Options: (a) a listing (price, monthly cap) that rival plans buy from by rule; (b) negotiated one-off deals.
- Recommendation and choice: **(a)**; built (phase D): `SetChipListing` and `BuyListedChips`, paid on the `ChipSales` and `Hardware` ledger lines.
- Why: rivals must use the same commands with readable reasons; a listing is one rule and logs cleanly; deals can be added on top with the funding/Bubble systems.

### Q28. How many chip designs at once? DECIDED: one in design at a time

- Options: (a) one in design at a time (any number Ready); (b) unlimited in parallel.
- Recommendation and choice: **(a)**; built (phase D): `StartChipDesign` refuses while another design is Designing or TapingOut.
- Why: MGT2's console designer is one project at a time; keeps architects a real constraint.

### Q29. Inference chips: serving only? DECIDED: serving only

- Options: (a) inference chips only serve; (b) they also train at a penalty.
- Recommendation and choice: **(a)**; built (phase D): `ChipQueries.BatchTrains` is false for inference chips, so they only add to `ComputeSplit.Inference`.
- Why: the bible's Gen 1 is the inference chip for "cheaper serving for your Mini"; training chips are "the prize".

### Q30. New rivals mid-game? DECIDED: data-driven arrivals by year

- Options: (a) data-driven arrivals by year (Goggle DeepMined 2010, DeepSneak 2023); (b) all labs from day one.
- Recommendation and choice: **(a)**; built (phase C): `RivalConfig.joinsYear`, `GameState.PendingRivals`, `RivalSystem.RunArrivals`.
- Why: the bible says more labs arrive with the eras; a 1995 start with Goggle DeepMined would break the era fiction.

### Q31. Local politics and weather in v1? DECIDED: heatwaves and the town vote only

- Options: (a) heatwaves and the town vote (a build delay) only; (b) also protests and noise lawsuits; (c) none.
- Recommendation and choice: **(a)**: both are built (`SiteSystem.RollHeatwaves`, the town-vote roll in `BuildHall`), each with one dice roll on the `World` stream.
- Why: both are bible events with a clear mechanical effect and one roll each; more can be added as data.

### Q32. Can stolen weights skip a locked line? DECIDED: yes, the next version needs the unlock

- Options: (a) yes, the copy can be of a line you have not unlocked, but the next version needs the unlock; (b) only lines you have unlocked.
- Recommendation and choice: **(a)**; not built yet (phase G).
- Why: the bible's payoff is "skip research"; the follow-up lock keeps it a shortcut, not a replacement.

### Q33. Founding country choice (home bloc)? DECIDED: `playerStart.homeBlocId`, no founding screen yet

- Options: (a) this batch uses `playerStart.homeBlocId = "us"` and a new-game option to change it; (b) build the full founding screen (bible chapter 5) now.
- Recommendation and choice: **(a)**: the data field exists and rivals have their own `homeBlocId`; the new-game option comes with the start-era picker work.
- Why: export rules and attention need a home bloc now; the founding bonuses belong with the start-era picker work.

### Q34. Cast names for the arrivals? DECIDED: the bible's parody near-misses

- Options: (a) the bible's suggested parodies (Goggle DeepMined / Sunder Pitchai / Gemeny; DeepSneak with no named boss); (b) fully invented names.
- Recommendation and choice: **(a)**; built (phase C): the real-name test grew to keep the guardrail.
- Why: Hatch chose parody near-miss names.

### Q35. Training costs after hardware generations? DECIDED: era `trainingCostPermille` retuned down

- Options: (a) keep Q1's steps and let labs be rich; (b) retune the era multipliers by the compute per rent-dollar the new generations add.
- Recommendation and choice: **(b)**. A VooDude-to-Blackwall jump makes a rented GPU about 25x cheaper per unit of compute by 2026, so the era multipliers fell to keep a flagship run costing about the same share of a lab's cash.
- Why: Q1 said to raise the numbers when generations landed; this is the equivalent move in the other direction. `Incidents_use_the_headlines_of_the_era_they_happen_in` caught it (labs bankrolled themselves out of the early eras). The golden digest moved with it.

### Q36. Attention notice size: one number or a list? DECIDED: `noticeKwByYear`, a list

- Options: (a) one kW threshold; (b) a year-keyed list.
- Recommendation and choice: **(b)**: 500 kW in 1995, 5,000 in 2009, 50,000 in 2017, 300,000 in 2022, 1,000,000 in 2026.
- Why: a data centre that makes headlines in 1995 is a rounding error in 2026; the list is a config edit and keeps old saves valid.

### Q37. Where do incidents count for attention? DECIDED: `LabState.IncidentsThisMonth`

- Options: (a) read the incident events; (b) a monthly counter the safety system bumps and government month-close resets.
- Recommendation and choice: **(b)**; it feeds the home bloc's gain only.
- Why: events are a bounded log and must never be game state; a counter is one field and trivially replayable.

### Q38. Liquid cooling: tech, room, or both? DECIDED: a tech (`liquid-cooling`, +300 cooling) in Chat Boom

- Options: (a) a tech; (b) a room type; (c) both.
- Recommendation and choice: **(a)**, costing 16,000 RP after Mixed-Precision. It shrinks only the cooling factor above 1000 and halves heatwave throttle; the existing `liquid-cooling-pod` room stays flavour.
- Why: a tech is a lab-wide, one-off choice that reads clearly next to the site table; the room would need per-site placement rules.

### Q39. Infrastructure and Policy: new roles or reuse old ones? DECIDED: new roles `infra` and `policy` with their own rooms

- Options: (a) reuse Engineer and Safety Tester; (b) two new functions, roles and rooms (`ops-centre`, `policy-office`) from 1995.
- Recommendation and choice: **(b)**; `PlanConfig.Staff` can hire them and the autopilot does not need them.
- Why: both have a clear numeric effect (build time and attention), and giving them a function keeps the staff-by-function query uniform.

### Q40. What does a hall's `HallBuildingKw` mean when nothing is building? DECIDED: 0, with `HallDoneTick` -1

- Options: (a) -1 for none; (b) 0 for none, tick -1 as the flag.
- Recommendation and choice: **(b)**.
- Why: `HallBuildingKw` is used in sums (grid draw), so zero is the harmless value; the tick is the "is it building" test.

### Q41. Which refusals do the land commands use? DECIDED: existing codes plus eight appended

- Options: (a) one generic refusal; (b) a code per reason.
- Recommendation and choice: **(b)**: SiteClosed, NoFreePlot (both a full site and a second plot for one lab), GridFull, NotEnoughPower, ExportBlocked, PermitFrozen, BuildRunning (hall, deal) and HoldingNotEmpty.
- Why: the UI can say exactly why; codes are appended, so journals keep their numbers.

### Q42. Does a freeze block the HQ? DECIDED: yes, a frozen home bloc blocks buying GPUs at the HQ

- Options: (a) a freeze only blocks sites in that bloc; (b) it also blocks the HQ when the HQ is in that bloc.
- Recommendation and choice: **(b)**; selling, moving out and training still work.
- Why: the home bloc is where the lab is registered; otherwise the strongest government could freeze everything but your own garage.

### Q43. What happens to sites when a lab goes bankrupt? DECIDED: holdings and the hardware in them are dropped

- Options: (a) leave them; (b) release every holding and its hardware.
- Recommendation and choice: **(b)**: plots become free for other labs and the grid is released.
- Why: a bankrupt lab can no longer pay; leaving plots owned by a dead lab would lock a site forever.

### Q44. When does a plan build data centres? DECIDED: only with `ownGpus`, site appetite above 0, a full HQ and a shortfall

- Options: (a) every plan builds; (b) a plan field `siteAppetitePermille` (with the allowed blocs) and the owned-fleet switch.
- Recommendation and choice: **(b)**: xAy builds (appetite 1000, `us`), AntHopticsAI 300 and OpenAye 400 but both rent, the autopilot never. Hardware goes into the hall with most free power first; otherwise land at the cheapest open site (power price times cooling) in the plan's blocs, then a hall sized by appetite. The HQ keeps only `plan.gpus` for an appetite plan.
- Why: it uses the same commands as the player so refusals stop it the same way; it is easy to turn off (appetite 0, test `A_lab_with_no_site_appetite_never_buys_land`). Replacing obsolete hardware was added in phase C (Q52).

### Q45. Audit consequences? DECIDED: a fine of two weeks of revenue now; investigations wait for the shady path

- Options: (a) build the investigation roll now; (b) fine now, roll in phase F.
- Recommendation and choice: **(b)**: Letter is a headline, Hearing costs 20 reputation (+10 with two policy staff), Audit fines `auditFineRevenueWeeks` of revenue, Freeze blocks buying in the bloc.
- Why: the roll needs the Heat meter that comes with the shady path.

### Q46. Ops Centre and Policy Office: which era? DECIDED: the first (Dial-up Days)

- Options: (a) later eras; (b) available from the start.
- Recommendation and choice: **(b)**.
- Why: an early lab that wants to build in Oregon in 1997 needs infra staff then; the cost is low and nothing requires them.

### Q47. Heatwave events: one per site or one per lab? DECIDED: one per lab at the site

- Options: (a) one world event per site; (b) one event for every lab with a holding there.
- Recommendation and choice: **(b)**, in config site order, with the roll made once per site.
- Why: the event feed is a per-lab log; the roll stays one die per site so sharing a site does not change the dice.

### Q48. Does the player's own lab keep a decision log? DECIDED: no, only plan-run labs do

- Options: (a) the player's autopilot writes to the player's log (systems design 3.1); (b) only labs whose controller is a plan write a log.
- Recommendation and choice: **(b)**. `DecisionLog.Add` returns at once for any lab that is not plan-run, so `LabState.Log` of the player stays empty.
- Why: a replay re-applies only the journaled commands, not the planner that issued them, so a log written by an outside bot could never be rebuilt and the replay digest would drift (`A_recorded_game_replays_to_the_same_state_from_its_orders_alone` caught it). The host can keep its own player-side log.

### Q49. How small is too small to log? DECIDED: under three units, and renting also needs a quarter move

- Options: (a) only the 25% rule for renting; (b) the 25% rule and at least `rivalry.minLoggedUnits` (3) units.
- Recommendation and choice: **(b)**. Hires, GPU buys and swaps below 3 units are not logged; `RentedGpus` needs both.
- Why: a small lab that rents 0, 1, 0, 1 GPUs on alternate months would fill sixty lines with noise (seen in the xAy log) and push real decisions out.

### Q50. How does the release gate boost the next run? DECIDED: derived from the models, not a stored number

- Options: (a) a field `GateBoostPermille` on the lab; (b) count the trained, unreleased models below the gate, newest first (`LabPlanner.RunAlignmentShare`).
- Recommendation and choice: **(b)**: +100 per model in a row that is waiting below the gate, capped by the 900 share limit; a model that passes (or any released model) ends the streak.
- Why: no new state to sync or migrate and it cannot drift from the models it describes.

### Q51. When does xAy's infra team start? DECIDED: 2008, not week 0

- Options: (a) from the first week (systems design 3.4); (b) a plan staff entry with `fromYear` (new `PlanStaffConfig.FromYear`), set to 2008.
- Recommendation and choice: **(b)**. Six infrastructure engineers grown like other staff from 1995 made xAy bankrupt in several seeds (and the 40-year survival test failed on seed 3); by 2008 it has the revenue and the halls to need them. Its site appetite (1000, `us`) and its 4-week hall floor are unchanged.
- Why: "builds fast" is a trait of a lab that already builds halls; a start-up with a payroll of 12 and no revenue is just a lab that fails. The event text still fires whenever a hall opens within `fastBuildWeeks` (8).

### Q52. How do plans replace obsolete hardware? DECIDED: a monthly Refresh for owning plans

- Options: (a) never; (b) once a month the oldest batch that is `rivalry.replaceTierGap` (2) generations behind the current one is sold at resale and the same number is bought again, in the same place.
- Recommendation and choice: **(b)**, only for plans with `ownGpus`, and only when the lab is not overstretched. The count is the smallest of the batch, the vendor's allocation left, the free racks or kilowatts after the sale, and what the cash allows above the reserve. Export rules and permit freezes are checked first so nothing is sold that cannot be bought back. Only the buy is logged (`Replaced`, reason `GenerationsBehind`).
- Why: it closes the Q44 gap (fleets stuck on 1995 hardware in 2012) with the player's own commands. `Rules/Systems/LabPlanner.Refresh`; `Old_hardware_is_swapped_for_the_current_generation_and_only_the_buying_is_logged`.

### Q53. Board drama: how often and what does it do? DECIDED: one twelfth of the yearly chance a month, a 4-week pause, an event

- Options: (a) narration only; (b) a pause of `rivalry.boardDramaWeeks` (4) with an event and a `Paused` log line.
- Recommendation and choice: **(b)**. Each month a plan-run lab rolls `dramaPermillePerYear / 12` on the `Rivals` stream (OpenAye 80, xAy 40, others 0); the plan returns without commands until `PlanPausedUntilTick`. Event `BoardDrama`: "OpenAye's board fired its boss on Friday. The boss was back on Tuesday." (no gendered pronoun, no quotes).
- Why: it is the one signature behaviour that costs a lab something real, so a parody has a price.

### Q54. Vendor allocation edge cases? DECIDED: bankrupt labs get 0 and drop out of the sum; the remainder goes to the top standing

- Options: (a) bankrupt labs keep a share; (b) they get 0.
- Recommendation and choice: **(b)**. Standing starts at 500, drifts 5 a month, rises 1 per 2 GPUs bought; the rounding remainder goes to the highest standing (ties to the lower lab id). A share that moves 10% or more month to month writes a `ShiftedAllocation` line in the vendor's log with the standing, share and buying causes.
- Why: the shares must sum to the supply exactly (`Allocation_adds_up_to_the_supply_and_a_lab_past_its_share_is_refused`); a dead lab holding a share would waste chips.

### Q55. How does a late rival enter? DECIDED: at the first month start of its year, with era catch-up

- Options: (a) at the first week of the year only; (b) at the first month start whose year is its `joinsYear` or later (the first week of January in a normal game, the next month start in an old save).
- Recommendation and choice: **(b)**. It gets its full `startCash`, the era's start lot, every tech of the eras before the current one (and what those need), its blocs, a vendor standing of 500, an event `RivalJoined`, and appends to `Labs` so the player stays first. `successor` has `joinsYear` 9999 and never spawns. A new game with no rivals picked gets no pending rivals; an old save gets `pendingRivals` filled from the config.
- Why: Goggle DeepMined arrives in 2010 with $5M, "a sleeping giant" that is slow to ship; DeepSneak in 2023, home `china`, short runs, under the export rules.

### Q56. Feelings: exact causes? DECIDED: as the design table, bracket loss counts per bracket

- Options: (a) one penalty per Gauntlet; (b) the table: crown -30 for every non-bankrupt rival and -60 for the runner-up, -10 per bracket the player wins that the rival also entered.
- Recommendation and choice: **(b)**, drift toward 0 by 5 a month, clamped to -1000..1000. A player who keeps winning ends every rival in a Feud (below -500), so rivals lobby the player's home bloc +15 attention a month (a `Lobbied` log line each time). Chip trades add `feelings.chipOrderPermille` (Q57+); shady causes arrive with their phase.
- Why: it is what the design table says; it makes dominance cost something. If the 40-year runs show attention smothering a strong player, `feelings.feudAttentionPerMonth` is the knob.

### Q57. Which techs open the chip screen, and how do the spec's years map to eras? DECIDED: ten techs, era-gated

- Options: (a) copy the spec's years and make them the only gate; (b) put the techs in the era that opens nearest to the year and let the era be the floor (Q5).
- Recommendation and choice: **(b)**. `chip-team` (It Sees, unlocks inference), `hbm-memory`, `training-silicon` (It Writes, unlocks training), three `compiler-stack` steps (+100 software each), `stacked-hbm`, `reticle-dies`, `low-precision`, `optical-interconnect` (+100 cooling and training speed). Foundry nodes still have their own `opensYear`, so a lab with the tech in 2012 cannot order 5 nm before 2022 (`EraLocked`).
- Why: the content tables stay one place per era, and the node years are what the chip fiction needs.

### Q58. Do the chip rooms need a tech as well as an era? DECIDED: yes, `RoomTypeConfig.requiresTech`

- Options: (a) era only; (b) an optional tech. 
- Recommendation and choice: **(b)**: `chip-design-lab` and `hw-test-lab` need `chip-team`; `BuildRoom` answers `MissingPrerequisite`. The planner's `RoomFor` skips locked rooms.
- Why: the era opens in 2012 for everyone, but the room is the lab's own chip team; a lab without it has no reason to own the room.

### Q59. How are derived chip numbers rounded? DECIDED: floor at each step, kilowatts ceil once

- Options: (a) float-free but rounded at the end only; (b) `MulPermille` (floor) in a fixed order: target, die, format, memory, cooling; kilowatts as one `ceil(node.kw x die x format x memory x cooling / 1e12)`, at least 1.
- Recommendation and choice: **(b)**. Pinned: inference/n28/small/fp16/ddr/air is 99 compute, 1 kW, 1,260 points, a wafer of 3,600; training/n5/medium/fp8/hbm-stacked/liquid is 4,118, 3, 15,444, 64,350.
- Why: whole numbers must agree between the screen, the order and the tests; one documented order is enough.

### Q60. Respin odds and the yield ramp: which knobs? DECIDED: two new knobs, the rest from the spec

- Options: (a) fixed respin odds by node; (b) the node's odds less `respinPerEngineerPermille` (10) per engineer, capped at `respinEngineerCap` (10).
- Recommendation and choice: **(b)**. The ramp is `(10 + min(engineers, 10))` a week, doubled by a test lab, up to the design's maximum; a respin costs 40% of the tape-out and 12 weeks. A tape-out comes back Ready with the node's initial yield.
- Why: the spec asked for engineers to matter in both; two integer knobs in data keep them tunable.

### Q61. How is the foundry room shared? DECIDED: per node and calendar month, labs book in order

- Options: (a) a global queue; (b) `capacityPerMonth x (1000 - foundryVendorSharePermille) / 1000` chips per node per month, an order takes the first month with room from the current one, up to `bookAheadMonths` (24), and delivers `leadWeeks` after that month starts.
- Recommendation and choice: **(b)**. `GameState.Foundry` holds only booked months; past months are dropped at month start; a lab that goes bankrupt gives its room back. A full book is `FoundryFull`.
- Why: it makes a rival's order matter to the player without a queue to simulate, and it replays the same for the same seed.

### Q62. Where do chips live, and what can be done with them? DECIDED: only in holdings, never moved or sold like GPUs

- Options: (a) chips behave like any hardware batch; (b) chips are batches with `ChipDesignId >= 0`, delivered to a holding, never to the HQ.
- Recommendation and choice: **(b)**. `SellGpus` and the oldest-first refresh count GPU batches only, `MoveHardware` refuses chips, `SellLand` is refused while orders are pending (`HoldingNotEmpty`), and pending orders reserve kilowatts (`WorldQueries.FreeKw`).
- Why: a chip is made for a node and a site at order time; moving it would need export checks twice and a resale rule nobody asked for.

### Q63. How do own chips feed compute? DECIDED: `ComputeSplit` with the software factor, an in-house bonus for the maker

- Options: (a) chips add their paper compute; (b) paper compute times the maker's software permille (300 to 1000); training chips count as general compute, inference chips as serving only; own training chips get +15% when the lab trained them itself.
- Recommendation and choice: **(b)**. `SoftwarePermille` stores the base (grown by architects, capped at 700) and the compiler techs are added at read time (cap 1000). A lab that buys another lab's chips runs the seller's software level and gets no bonus.
- Why: it makes designing worth more than buying and keeps the two ways visible as numbers.

### Q64. How does a rival plan run the chip programme? DECIDED: only labs that own GPUs and build sites

- Options: (a) every plan with `chipProgramYear`; (b) `chipProgramYear`, `ownGpus`, `siteAppetitePermille > 0`, `chip-team` and not overstretched.
- Recommendation and choice: **(b)**. xAy (2020), Goggle (2014) and DeepSneak (2023) run it; OpenAye (2024) and AntHopticsAI (2025) rent and have no halls, so their `buysListedChips` stays inert until a cloud-deal feature exists. The steps are staffing architects, one design at a time on the best open node, tape-out, own orders (half the gap, counted 1:1 as GPU units), buying listings, and listing.
- Why: chips can only be delivered to holdings (Q62), so a lab without any cannot use them; every step is logged with its reasons.

### Q65. Listings: what can a buyer and a seller do? DECIDED: the buyer pays the seller the order price, the seller pays the foundry

- Options: (a) the buyer pays the foundry directly; (b) `BuyListedChips` pays `PricePerChip x count` to the seller (`ChipSales`, buyer `Hardware`), the seller pays the wafers; it is refused when the seller cannot pay them.
- Recommendation and choice: **(b)**, capped by `maxPerMonth` and `listingMaxPerMonth`. A rival buys only at or below 90% of Envydia's price per unit of compute (95% when friendly) and delivers to its own holdings. A trade with the player moves the rival's feeling by `feelings.chipOrderPermille`.
- Why: the seller carries the yield risk, so the price has to cover it.

### Q66. When does Envydia notice? DECIDED: on the first Ready training chip, each delivery, the first listing, and a yearly ambush

- Options: (a) at tape-out start; (b) when the first training tape-out comes back Ready.
- Recommendation and choice: **(b)** -150 once; -50 per delivery of own chips; -100 at the first listing (`VendorStanding.ListingSeen`, schema 5); the price rises 10% for a lab that runs own training chips; the keynote ambushes a design that became Ready within 8 weeks before it (12 weeks of `ChipAmbushUntilTick`); when one lab runs 30% or more of its general compute on own chips, the vendor courts every other lab (+20, one log line a quarter).
- Why: it is the spec's list; the ambush window is "just before the keynote".

### Q67. Is chip compute counted in the planner's GPU target? DECIDED: yes, one unit for one unit

- Options: (a) a chip counts as its compute in GPU units; (b) 1:1.
- Recommendation and choice: **(b)**, with pending orders counted when deciding what to buy but rent covering the physical gap. 
- Why: an own chip is worth several GPUs, so the lab ends up with less hardware by count and more by compute; the 45-year soak shows no lab failing and own-chip shares of 8% to 15%, below the courting line, so courting is exercised by a test, not yet by the cast.

### Q68. Endings and `PlayerLabId`? DECIDED: schema 6, spectate at -1

- Recommendation and choice: **(b)** from systems design section 5: `GameState.PlayerLabId`, bailouts via `ContinueAfterEnding`, `SellLab` with `found-again` or `spectate`; save problems accept spectating without a player lab.
- Why: phases E/F implementation on schema 6 then 7 for shady.

### Q69. Frontier research? DECIDED: research points, 1400‰ growth, cap 60

- Recommendation and choice: **(b)**. `ResearchFrontier` after all open techs; `FrontierLevel` adds capability via `LabQueries.TechBonus`; rivals buy in `LabPlanner` after techs.
- Why: endless content once the tree is done.

### Q70. Model museum? DECIDED: six retired per line, oldest out weekly

- Recommendation and choice: **(b)**. `MuseumSystem.RunWeek` prunes beyond `endings.museum.perLine`.
- Why: long saves stay bounded.

### Q71. Shady stash vs cash? DECIDED: stash off-books, `Laundered` ledger line

- Recommendation and choice: **(b)**. Stash never touches cash except `Launder` at 700‰ keep; Heat from laundering and ops.
- Why: cash invariant and section 6.

### Q72. Rivals and shady? DECIDED: `shadyAppetite` 0, validator enforces

- Recommendation and choice: **(b)**. Only the player's lab (and future player autopilot when appetite > 0) may `StartShadyOp`.
- Why: parody guardrails.

### Q73. Investigations? DECIDED: month-end on player lab, conviction → handcuffs ending

- Recommendation and choice: **(b)**. `ShadySystem.RunMonth`, Audit level adds an extra roll; conviction uses Heat and policy staff.
- Why: section 6.5 wired to section 5 handcuffs.

### Q74. Corruption? DECIDED: incidents, quality trust, mood, carry-over, side jobs

- Recommendation and choice: **(b)**. Weights in `shady` config; `ModelMood.Corrupted` at 600+; training finish carry-over 500‰.
- Why: section 6.6.

### Q75. Golden digest after endings and shady? DECIDED: re-recorded

- Recommendation and choice: **(b)**. New digest `59cd8b60…` — weekly shady resolution, museum prune and frontier planner hooks change the autopilot year.
- Why: firm rule on golden moves.

### Q76. Bot balance soak and late-game cash? DECIDED: `BalanceSoak`, dial-up 1500‰, wealth growth down

- Options: (a) leave the old 40-world story soak; (b) eight-year `BalanceSoak` over three player presets with crown, rival-survival and year-eight budget-pressure gates.
- Recommendation and choice: **(b)**. CI runs 24 games per preset (`BALANCE_SOAK_GAMES_PER_PRESET` or `BALANCE_SOAK_FULL=1` for 200 each). Dial-up `trainingCostPermille` is 1500 (Q35 follow-through after hardware generations); plan `wealthGrowthPermillePerMillion` is 75/70 on rivals and the autopilot; golden digest `af2df07a…`.
- Why: late-game cash after Q35 was swamping year-eight pressure; Envydia courting on rivals still waits on chip share (Q67), so courting stays a unit test, not a soak outcome.

### Q77. How do breakdowns relate to the live formulas? DECIDED: one `DisplayQueries` helper, deltas that sum

- Options: (a) duplicate the math in the UI; (b) a read-only `DisplayQueries` whose parts add to the same totals as `LabQueries.Quality`, `MarketSystem.Appeal`, `SafetySystem.IncidentChancePermille` and `lab.LastWeek.Revenue`.
- Recommendation and choice: **(b)**. Appeal uses step deltas (core, reputation, sales, crown); incident splits config, alignment and corruption portions; no save or command changes (schema 7).
- Why: alignment-gap sandbagging (NEXT item 8) will lie on the screen, so the honest breakdown must stay beside the real rules.

### Q78. Funding rounds and ousting? DECIDED: three data rounds, permille dilution, ending at control

- Options: (a) rival-only backers as today; (b) player `RaiseFunding` for angel, Series A and B with cash, reputation and year gates, `boardSeats`, founder `founderSharePermille` diluted each round, `RecordOusted` when share is at or below `controlPermille` (leader becomes `oustedSuccessor`).
- Recommendation and choice: **(b)**. Rounds must close in config order; saves migrate with full founder share and no rounds (schema 8); the board can oust without spectating (continue modes still empty until phase F).
- Why: matches DESIGN.md dilution and board fiction; golden digest `ca046311…` after schema bump.

### Q79. Alignment on screen vs in the rules? DECIDED: displayed meter, gap, rungs 1–3

- Options: (a) one alignment number; (b) true `alignment`, `displayedAlignment`, incident rungs screenshot/jailbreak/sandbag; release `claimedAlignmentPermille` sandbags; quality breakdown adds `SandbagBoost`; odds still use the true meter.
- Recommendation and choice: **(b)**. Schema 9 copies displayed from true on migrate; rung 3 closes the gap. Golden digest `463eb18f…`.
- Why: NEXT item 8 and DESIGN.md ladder fiction; item 8's sandbagging needed item 3's breakdown first (Q77).

### Q81. Alignment and staff test depth? DECIDED: integration facts for gap, poach, stars

- Options: (a) keep three smoke tests per feature; (b) add sandbag scandal gap-close, star research permille, feud poach, and migration count checks on the existing schema-2 fixture.
- Recommendation and choice: **(b)**. No new golden for test-only commit unless a soak moves.
- Why: user review expected deeper coverage before the branch ships.

### Q83. Test depth for alignment, staff, and compute? DECIDED: paths, edges, migration, cross-system

- Options: (a) ~3 smoke tests per feature; (b) a dozen-ish facts each covering main paths, zero/max/bankrupt edges, prior-schema load, save round-trip, and one cross-system tie (endings, feud feelings, GPU generations).
- Recommendation and choice: **(b)**. `SaveSchemaPretend` rewinds saves for isolated migrations; the three expansion files hold 13 alignment, 12 staff, and 14 compute facts at first land; `64512a4` actually runs **370** tests (the earlier 372 count double-counted two removed parrots).
- Why: owner quality bar before further NEXT items; behaviour assertions only, not config parroting.

### Q84. Opus review must-fixes on alignment/staff? DECIDED: quality gap, billing order, queue, migrate, morale

- Options: (a) ship as reviewed; (b) fix all six must-fixes with regression tests; cheap nice-to-haves where free.
- Recommendation and choice: **(b)**. Market quality uses effective alignment and sandbag boost `max(0, displayed − alignment)` so corruption cannot cancel out; `AlignmentGap` compares displayed to true alignment; contract expiry runs after `Finance.RunWeek`; GPU queue can cancel (count 0), refuses a second holding, partial fulfilment, stalls with `GpuQueueStalled`; schema-10 staff ids come from root `nextId` with the first non-star trait; morale drifts down in debt and after incidents/poaches, poach picks the unhappiest star. Suite **380** after the review pass (+10 facts, weaker parrots removed).
- Why: Opus review on `64512a4`; ledger-over-contract-life and corruption/gap tests would have caught the billing and quality regressions.

### Q82. Reserved compute and the Envydia queue? DECIDED: contracts, deposit, queue at month start

- Options: (a) keep instant rent and spot buys only; (b) multi-year reserved contracts at `reservedRentPermille` with an upfront `reservedDepositWeeks` block, general compute while the contract runs; (c) when allocation is gone, queue units and buy them when the next month opens.
- Recommendation and choice: **(b)+(c)**. `SignReservedCompute` and `QueueGpuPurchase` commands; `FulfillQueues` after `RunAllocation` each month; schema 11 adds `reservedContracts` and vendor `queuedGpus`. Golden digest `61bacf6b…`.
- Why: NEXT item 5 and DESIGN.md reserved line; allocation-by-standing was already in from phase C.

### Q85. Wider cast arrivals? DECIDED: Metta (2007), Macrosoft (2012), Palantear (2017) as data

- Options: (a) keep five playable rivals; (b) add bible parodies as `joinsYear` plans only, same arrival and planner rules as Goggle and DeepSneak.
- Recommendation and choice: **(b)**. Metta rents consumer sectors; Macrosoft owns and buys listings; Palantear publishes safety, no halls. Twelve `WiderCastTests`; lifetime soaks retuned cash and growth so none go bankrupt.
- Why: NEXT item 12 and DESIGN.md cast table; no new commands. Start-era picker remains host work.

### Q86. Cloud chip deals for rent-only rivals? DECIDED: `HardwareBatch.Cloud`, weekly rent surcharge

- Options: (a) leave `buysListedChips` inert for OpenAye and AntHopticsAI; (b) a cloud holding id (−2) with no kW reservation, foundry export checks on the buyer bloc, `cloudRentPermille` weekly bill counted in GPU rent; planner `usesCloudDeals` runs listing buys only.
- Recommendation and choice: **(b)**. `BuyListedChips` and rival planner accept cloud when the lab has no halls; freeze still blocks the buyer home bloc. Twelve facts in `CloudDealTests.cs`.
- Why: DESIGN.md chips row and Q decision for OpenAye (2024) / AntHopticsAI (2025); without halls, listed chips were unreachable.

### Q80. Staff pools or people? DECIDED: `StaffMember`, traits, morale drift, feud poaching

- Options: (a) keep role counts; (b) one save row per person with trait and morale, hire/fire by individual, stars poached when a rival is in a feud.
- Recommendation and choice: **(b)**. Schema 10 expands pools on migrate; stars get research permille; fire drops lowest morale. Golden digest `e358c673…`.
- Why: DESIGN.md "stars matter and get poached"; pool counts alone could not carry trait or morale state.

### Q87. Endings continuation test depth? DECIDED: bailout, exit, monopoly, frontier, museum, ousted

- Options: (a) smoke only; (b) ~12 facts for each continuation path and config stubs, spectate-safe where relevant.
- Recommendation and choice: **(b)**. `EndingsContinuationTests.cs` at 12 facts; rules never block after an ending (Q23).
- Why: NEXT endless-play slice; complements funding ousted flow (Q78).

### Q88. Shady path later depth? DECIDED: cards, grey payout, freeze signup, audits

- Options: (a) defer to expansion; (b) government hack attention, grey-compute kw payout, audit investigations, corruption trust, freeze blocks reserved signup, smuggle export bypass.
- Recommendation and choice: **(b)**. `ShadyPathLaterTests.cs` at 12 facts; grey compute pays via `GreyCompute` special only (no double cash line); `SignReservedCompute` respects home freeze (Q42).
- Why: NEXT shady path; pairs with alignment corruption trust (Q74).

### Q89. Freeze permits and audit investigation floor? DECIDED: block growth commands; `OnAuditOpen` uses audit permille

- Options: (a) only reserved compute; (b) refuse funding, GPU queue, and site growth in frozen blocs; audit uses `auditInvestigationPermille` on escalation.
- Recommendation and choice: **(b)**. `FreezeAuditLaterTests.cs` at 12 facts; audit fines use `LabState.LastWeek.Revenue`.
- Why: systems design §2 Freeze command list and shady audit hook (Q45 tail).

### Q90. Bubble meter? DECIDED: schema 12 `bubblePermille`, config-driven boosts and decay

- Options: (a) defer until shares; (b) monthly meter with funding, listings, and event hooks from config.
- Recommendation and choice: **(b)**. `BubbleMeterTests.cs` at 12 facts; `MigrateTo12` defaults meter to 0.
- Why: systems design bubble hook on `BuyListedChips` and circular financing lead-in.

### Q91. Incident rungs 4–6? DECIDED: corruption mins 250/450/700, six rungs in config

- Options: (a) rung 3 only; (b) corruption thresholds for rungs 4–6 before sandbag/alignment picks; validator allows optional upper rungs.
- Recommendation and choice: **(b)**. `IncidentRungsLaterTests.cs` at 12 facts; honest models stay on rung 1 below 250 corruption; golden digest re-recorded.
- Why: bible safety ladder; pairs with corruption incident weight (Q74).

### Q101. 60-year headless soak? DECIDED: step sixty years in tests

- Options: (a) CI only; (b) one `LongHorizonSoakLaterTests` year step with Problems check.
- Recommendation and choice: **(b)**.
- Why: systems-design 5.6 soak tail without balance-soak runtime cost.

### Q100. Era incident load? DECIDED: era `incidentLoadPermille`

- Options: (a) flat forever; (b) per-era multiplier on weekly incident chance.
- Recommendation and choice: **(b)**; default 1000, agents 1200.
- Why: NEXT item 13 "rules shift" lite; `IncidentParts` and `SafetySystem` take tick.

### Q99. Exit sell-price breakdown? DECIDED: `ExitPriceParts`

- Options: (a) prose only; (b) breakdown query matching `SellPrice`.
- Recommendation and choice: **(b)**. Revenue years, fleet, cash on hand.

### Q98. Second Place ending? DECIDED: crown runner-up streak

- Options: (a) never; (b) `crownRunnerUpStreak` after Gauntlet crown rank; `secondPlaceRunnerUpYears: 3`.
- Recommendation and choice: **(b)**. Schema 17; continue `acknowledge`.

### Q97. The Pop ending? DECIDED: player haircut on bubble pop

- Options: (a) news only; (b) record `the-pop` when weak player takes bubble haircut.
- Recommendation and choice: **(b)**. `BubbleConfig.thePopEndingForPlayer`.

### Q96. Endings continuation tail? DECIDED: continue modes on monopoly + superintelligence

- Options: (a) host-only; (b) config `continueModes` + `ApplyContinue` for spectate on worst endings.
- Recommendation and choice: **(b)**. Item 11 closed; no golden move.

### Q95. Funding ladder follow-ons? DECIDED: down rounds, player board drama, investor events

- Options: (a) defer until UI; (b) hot-bubble down-round cash/dilution on raise; (c) investor seats trigger monthly board lock; (d) after two rounds, investor board visits nudge rep and bubble.
- Recommendation and choice: **(b)+(c)+(d)**. `FundingFollowOnLaterTests.cs` at 12 facts; schema 16 `boardLockedUntilTick`; golden digest moved because `FundingSystem.RunMonth` runs at month start on the autopilot year.
- Why: DESIGN.md bubble and board fiction; rivals already had board drama on plans only.

### Q94. Containment rooms and superintelligence endings? DECIDED: wing bonus + three gates

- Options: (a) containment as cosmetic room only; (b) tiles add capped alignment-work permille in training; (c) endings on year-end when best released model crosses capability bar.
- Recommendation and choice: **(b)+(c)**. `ContainmentSuperintelLaterTests.cs` at 12 facts; schema 15 noop migration (config-only); paperclip / long-summer / loose priority order; loose blocked when containment tiles &gt; 0.
- Why: bible agent-era safety wing and upward ending stubs in default-config.

### Q93. Shady depth and China deals? DECIDED: player-only crimes, export stays legal-only

- Options: (a) rivals can run shady cards; (b) only the player runs shady cards; China partnership via accept/decline; smuggle bypasses export only on the shady path.
- Recommendation and choice: **(b)**. `ShadyChinaLaterTests.cs` at 12 facts; schema 14; real country name China in data; `ClampedSuccess` floor 0 when odds are 0.
- Why: project-context rule 10 (parody rivals never commit crimes); bible state requests in China.

### Q92. Rival shares and regulators vs monopoly? DECIDED: public float, IPO loop, scrutiny

- Options: (a) defer until funding shares UI; (b) plan-run `publicFloatPpm`, monthly IPO when bubble hot, regulator attention on dominance and monopoly.
- Recommendation and choice: **(b)**. Schema 13; `SharesRegulatorsLaterTests.cs` at 12 facts; golden digest re-recorded for share/regulator month hooks.
- Why: systems design circular financing and monopoly antitrust (§5).

### Q103. Bubble crash phase (Hatch)? DECIDED: grace months, sustained crash, residual permille

- Options: (a) instant pop clears meter; (b) grace on weak returns then multi-month crash; (c) world events only.
- Recommendation and choice: **(b)**. `BubbleMeterCrashHatchTests`; schema **19**; `DisplayQueries.BubbleMeterParts`.
- Why: Hatch spec — crash lasts quarters, funding/GPUs/customers tighten, meter never returns to zero.

### Q116. Acquire distressed rival labs (Hatch)? DECIDED: quote, integration, regulator on large deals

- Options: (a) player-only; (b) distressed pricing + rival rollups during crash; (c) full M&A sim.
- Recommendation and choice: **(b)**. `AcquireLab`; `absorbedByLabId` schema **20**; rivals skip planning when absorbed.
- Why: Hatch spec — crash consolidations without counting absorption as bankruptcy.

### Q117. Sell released models (Hatch)? DECIDED: earnings multiple vs scrap basis

- Options: (a) retire only; (b) one-shot cash for income foregone + sector rep hit; (c) sell to rivals as entities.
- Recommendation and choice: **(b)**. `SellModel`; `DisplayQueries.ModelSaleParts`; cannot sell last active model.
- Why: Hatch spec — liquidity vs lost weekly sector revenue.

### Q102. China deal re-offer after decline? DECIDED: month cooldown in save (schema 18)

- Options: (a) re-offer every month; (b) `declineCooldownMonths` and `chinaDealDeclinedUntilTick` per lab.
- Recommendation and choice: **(b)**. Default 24 months; `ShadyChinaLaterTests` regression.
- Why: Opus review — monthly decline was a permanent attention ratchet.

## Era rules (NEXT item 13)

### Q127. What does "the rules shift" mean in data? DECIDED: five era multipliers and one law, beside `incidentLoadPermille`

- Options: (a) a free-form rules list per era; (b) typed fields on `EraConfig`; (c) per-era code branches.
- Recommendation and choice: **(b)** `hireFeePermille`, `chipPricePermille`, `adoptionPermille`, `finePermille`, `minReleaseAlignment`; `EraRules` is the one place every system asks.
- Why: typed fields validate and round-trip like the rest of the config, need no save change, and the first era must stay at baseline so old balance and the 1995 golden run hold.

### Q104. Do salaries rise with the talent war? DECIDED: only the one-off hiring fee

- Options: (a) hire fee and weekly salary; (b) hire fee only.
- Recommendation and choice: **(b)** x1.5 in It Sees up to x3.5 in Agents, easing to x2.5.
- Why: `WeeklySalaries` has no calendar in its signature (14 call sites and the planners' reserve maths); a fee needs only the hiring tick. Salary scaling is a later change to the same table.

### Q105. Which alignment does the release law read? DECIDED: displayed alignment

- Options: (a) true alignment; (b) the alignment a lab can show (`DisplayedAlignment`).
- Recommendation and choice: **(b)**; the floor is 0, 80, 120, 160, 200, 250 from 2022 on and never falls.
- Why: a regulator audits what it is shown, so sandbagging passes on paper and incidents that close the gap are what catch it; this gives the shady path a reason to exist.

### Q106. What does the law do to models already on sale? DECIDED: nothing

- Options: (a) pull them from sale when the law arrives; (b) check only new releases and upgrades (`ReleaseModel`).
- Recommendation and choice: **(b)**, with `CommandError.BelowEraSafetyBar` and a message naming the era and the bar.
- Why: no stored "retired by law" state is needed, saves are unchanged, and a lab that ships a bad model into a new era still feels it at its next release.

### Q107. How do rivals meet the law? DECIDED: the planner holds a model back and logs it

- Options: (a) rivals bump alignment spend to the bar; (b) a model below the bar is `HeldBack` with `ReleaseGate` and `AlignmentBelowGate` reasons, logged at most once a quarter.
- Recommendation and choice: **(b)**.
- Why: a careless rival (OpenAye) is then visibly squeezed by the law while careful ones ship, with no new planning code and the reasons already in the decision log.

### Q108. Where is the chip shortage applied? DECIDED: the vendor's list price

- Options: (a) allocation supply too; (b) price only, in `VendorSystem.PricePerGpu`.
- Recommendation and choice: **(b)** (x1.1 in It Writes, x1.6 in the Chat Boom, easing to x1).
- Why: one line reaches every buying path (command, planner, queue); allocation already has its own standing rules, and changing both at once would double-count the shortage.

### Q109. Customer appetite and fines? DECIDED: scale adoption (capped) and the final fine

- Options: (a) leave the market alone; (b) `adoptionPermille` multiplies a sector's adoption (still capped at 1000), `finePermille` multiplies the incident fine after rung scaling.
- Recommendation and choice: **(b)** (appetite up to x1.4 in the Chat Boom; fines x1.5 in 2022, x2 in 2024, x3 from 2026).
- Why: the Chat Boom should feel like a rush and later years like a regulated industry; both are one multiplier at one call site, and the era banner lists what changed so the shift is explained.

### Q110. New risk classes per era? DECIDED: not in this item

- Options: (a) add era-specific incident kinds now; (b) leave them for the funding ladder and alignment-gap rungs.
- Recommendation and choice: **(b)**; incident load and fines are the era's risk dial for now.
- Why: a risk class needs per-model state (and a save schema step after 17), and incident rungs 1 to 6 already carry the ladder; it should land with the risk-class design, not as a side effect of item 13.

## Economy bounds (the "runaway economy" report)

### Q111. Is the economy running away? DECIDED: no, cash tops out near $450 billion; the report was a column overflow in `simcli`

- Options: (a) cut sector pools or revenue; (b) leave the economy alone, widen the `simcli` table, and pin the bounds with a soak.
- Recommendation and choice: **(b)**. Across 70 seeds the richest lab ends 40 years with $360 to 450 billion (about 4e11, never the 4e21 first read), and no lab earns more in a week than every sector pays together.
- Why: `simcli` printed cash in a 12-wide column, so a lab's cash ran into its weekly revenue and read as one number a billion times bigger. Pools are the design (a world market worth billions a week by 2030), and the existing $1 trillion ceiling in `ContentFlowTests` holds.

### Q112. What did go wrong? DECIDED: a rival that lost its market waited for debt to cut, then went bankrupt

- Options: (a) bail rivals out with funding rounds (ties to the bubble meter, which the rivals thread owns); (b) let the planner cut earlier and not cut what earns money; (c) nudge OpenAye's personality in the data.
- Recommendation and choice: **(b)**. On the stack before this change OpenAye went bankrupt in 3 of 30 seeds (1999 to 2001): its revenue fell under its payroll, it could never afford its next run (runway 3 to 6 weeks against a reserve of 8), and the planner only cut once in debt, one person a month.
- Why: it needs no new state or save change, stays out of bubble and crash code, and is the same rule for every plan, the player's autopilot included.

### Q113. When does a plan retrench? DECIDED: running costs over 125% of revenue and cash under 4 weeks of costs

- Options: (a) any loss; (b) the thresholds above, set in `rivalry.retrenchCostPermille` and `rivalry.retrenchRunwayWeeks`; (c) only once in debt (the old rule).
- Recommendation and choice: **(b)**. In the first year every starting lab loses money (costs 115% of revenue) with about 8 weeks of cash, which must not trip it.
- Why: with (a) a healthy start shrinks; with (c) the lab dies before the cuts bite. Thresholds in config keep it tunable and validated.

### Q114. What does retrenching cut? DECIDED: one person in six, and only GPUs the customers are not using

- Options: (a) one person a month (old rule); (b) one in six each plan, from the biggest team, never the last of a role; give back at most a quarter of the rented GPUs and none while the lab cannot serve its demand.
- Recommendation and choice: **(b)**.
- Why: the first try handed back GPUs the customers needed, revenue fell with them, and seed 4's OpenAye died faster than before. Whether a lab is short comes from last week's served share and compute, so nothing new is saved.

### Q115. What does the soak pin? DECIDED: nobody bankrupt, cash between -$1 billion and $1 trillion, revenue under the total pools

- Options: (a) only the existing seeds 1, 3 and 4 and the $1 trillion ceiling; (b) 8 seeds for 40 years from 1995 (including every seed that went bankrupt on the way: 2, 24, 27, and 4, 20, 1 from intermediate tries) and 2 seeds for 26 years from 2009.
- Recommendation and choice: **(b)** in `EconomyBoundsTests`; the sweep over seeds 1 to 70 was run by hand and is not a test.
- Why: each seed costs about 3 seconds, so the soak stays inside the suite; the floor, ceiling and revenue-under-pools checks catch both a runaway and a collapse.

## Start eras and risk classes (NEXT item 12 rules half, and Q110 revisited)

### Q118. Which eras can a game start in? DECIDED: all nine

- Options: (a) keep 1995 and 2009 only; (b) add a few mid eras; (c) every era, including the two expansions.
- Recommendation and choice: **(c)**. `startable` is true for every era; the two expansion eras are marked in the offer so a picker can say so.
- Why: starting is data (`startCashPermille`, `startGrantPermille`, `startLotId`, `startTechs`), so one more era costs a row, and the soak test plays a start in each of them.

### Q119. What does a later start hand the player? DECIDED: every tech of the eras before, and cash that grows with the era

- Options: (a) the same kit as 2009 scaled; (b) all techs of earlier eras (`RivalSystem.CatchUpTechs`, which also pulls in prerequisites) and cash from x30 (2012) to x15000 (2033) of the 1995 start, a grant only up to 2017, and the biggest lot from 2017.
- Recommendation and choice: **(b)**. It is $6M in 2012, $25M in 2017, $100M in 2022, $250M in 2024, $750M in 2026, $1.5B in 2029 and $3B in 2033.
- Why: a 2022 lab should not research the basics again, and the cash tracks what a training run and a GPU fleet cost then (training cost x10 in 2022, x120 in 2033, plus the era rules). The validator rejects a startable era with no cash or a missing earlier tech, so the data cannot drift.

### Q120. Who is in the world at the start? DECIDED: every rival whose join year has come; later ones are pending

- Options: (a) all rivals at once; (b) those with `joinsYear` at or before the start year, the rest arrive on their year as in a normal game.
- Recommendation and choice: **(b)** (`StartEras.Offer` lists the cast and who is present).
- Why: it is what a game played through to that year would look like, and no new arrival code is needed.

### Q121. How much cash does a rival start with in a later era? DECIDED: founders take the era's share, veterans take a grown figure held near the player's

- Options: (a) everyone gets their own start cash (macrosoft and goggle went bankrupt in every start from 2022 on, because a few million cannot meet era costs); (b) founders (`joinsYear` 0) take their start cash times the era's share; a rival that joined later takes its start cash grown by the training-cost multiplier since it joined, clamped to half to double the player's kit (`WorldSetup.EstablishedCash`); one that arrives later keeps its own start cash.
- Recommendation and choice: **(b)**. Over 9 eras and 3 seeds nobody went bankrupt. One change to an existing start: a later joiner already present in 2009 (Metta) now gets the established figure, which is the reason the 2009 start digest differs.
- Why: it is a rule from existing data, not a table per era, and it keeps the cast able to play.

### Q122. Is there a picker? DECIDED: only the list for one

- Options: (a) build a screen; (b) `StartEras.Offers(config)` returns era, year, tagline, what models can do, cash, grant, lot, tech count and the cast with their cash; `NewGameOptions.StartEraId` starts the game.
- Recommendation and choice: **(b)**. No UI in this item.
- Why: the rules half is done and tested; a screen needs only this list. The offer restates what `NewGame` does, and a test checks every era's offer against the game it starts.

### Q123. How do risk classes work? DECIDED: a class opens with its era, bites by sector, and replaces the plain mishap in an incident

- Options: (a) a class per model line; (b) a class per era and sector list; the class is drawn when an incident happens, weighted against the plain mishap (`safety.genericRiskWeight` 1000).
- Recommendation and choice: **(b)**: bias and autonomous crash (2012), misuse (2017), data leak (2022), agent mishap (2024), runaway agents (2026), physical harm (2029), loss of control (2033), eight in `safety.riskClasses`.
- Why: what a lab sells then decides what can go wrong, which a flat incident rate could not say. This answers Q110 (which put it off until it had its own design). The plain mishap always stays possible; with no class biting, the dice are not touched, so a game before 2012 plays as before.

### Q124. What does a class change? DECIDED: the fine and the reputation lost, then the headline

- Options: (a) only the headline; (b) fine times `finePermille` (after the rung fine, before the era's fine rule) and reputation lost times `reputationPermille`.
- Recommendation and choice: **(b)**, from x1 (bias, with x1.5 reputation) up to x5 (loss of control, x3 reputation).
- Why: a driving crash or a leaked database should cost more than a bad answer, and the later classes more than the earlier. The rung ladder (1 to 6) stays as it is.

### Q125. Where is the class stored? DECIDED: on the incident event, save schema 21

- Options: (a) not stored, drawn again from the sector when read; (b) a text id on `GameEvent` (`riskClass`, empty for the plain mishap), a schema step 20 to 21 that adds an empty one to every old event.
- Recommendation and choice: **(b)**.
- Why: a headline is picked once at the incident and must read the same later, even after the model changes sector. It adds one key per event and nothing else. The golden digest moved for this reason alone (checked: with schema 20 and the field removed, the digest `507debe1` before this change comes back), so the new constant is `71b44460`.

### Q126. What does the validator insist on? DECIDED: ranges, known ids, oldest first, and a `{model}` in every headline

- Options: (a) trust the data; (b) weight 1 to 100000 (so the draw cannot overflow), fine and reputation 250 to 10000 per mille, known sectors and eras, never the first era, listed oldest first, three or more headlines each with `{model}`, `genericRiskWeight` 1 to 100000.
- Recommendation and choice: **(b)**, with missing or unknown JSON keys an error as everywhere else.
- Why: a typo in a class should fail when the config loads, not as a crash in the middle of a game.

## Open

None.
