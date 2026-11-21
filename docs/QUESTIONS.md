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

## Role perks (NEXT.md known limit: roles beyond the first four had no rule)

### Q128. How does a role get a perk? DECIDED: a `rolePerks` table, derived from the staff list, nothing saved

- Options: (a) a new field on every role; (b) a separate table of perks (id, role, kind, share per person, cap, sectors, blurb) read from the staff a lab has today.
- Recommendation and choice: **(b)**. Four kinds: hire discount, incident cut, alignment work, sector appeal. Each person adds the share, held under the cap; perks of one kind add up.
- Why: a role can carry zero, one or two perks without touching the role rows; no save change, so no schema step; letting the role go ends the perk the same week. The validator needs a known role, a share from 1 to 1000, a cap from the share up to 900 (so a fee or a chance never reaches zero), a blurb and known sectors.

### Q129. What does the recruiter do? DECIDED: wins back 3% of every hiring fee per recruiter, up to 25%

- Options: (a) lowers salaries; (b) takes a share off the hiring fee, after the era's talent-war multiplier.
- Recommendation and choice: **(b)**. Nine recruiters offset a quarter of the Chat Boom's tripled fee. The hire command, the planners' budgets and a breakdown query (`HireFeeParts`: list price, era shift, discount) all use the one figure.
- Why: it answers the talent war of item 13 with a choice (pay for recruiters or pay the fee), and costs a recruiter a salary every week whether or not the lab hires.

### Q130. What do red-teamers do? DECIDED: 2.5% of incidents caught per red-teamer, up to 30%, by a second roll

- Options: (a) cut the weekly incident chance in permille; (b) when an incident was about to happen, roll once more and let it pass with the lab's catch chance.
- Recommendation and choice: **(b)**. The second roll is made only when an incident was coming and the lab has red-teamers, so a lab without them draws exactly the dice it did before and plays the same (tested by digest).
- Why: incident odds are a few permille a week, so multiplying them by 0.75 would round to nothing or to a halving. The breakdown (`IncidentOdds`) shows the odds before and the share caught.

### Q131. What do alignment fellows do? DECIDED: +4% on the work of each unit of alignment compute per fellow, up to 40%

- Options: (a) nothing beyond being safety staff; (b) a perk on top of the safety-staff share and the containment wing.
- Recommendation and choice: **(b)**. A fellow already counts as safety staff (a fellow is `Safety`), so the perk is the extra.
- Why: the fellow costs the most of the safety roles and should do the most; the wing and the staff still stack.

### Q132. What do product leads and the specialists do? DECIDED: a boost to the appeal of the lab's models, after the sales boost and before the Gauntlet crown

- Options: (a) lift sales for all; (b) a product lead lifts appeal in every sector (3% each, up to 24%), and each late specialist (agent wrangler, harness operator, roboticist, mission controller) lifts it in its sectors (5% each, up to 30%).
- Recommendation and choice: **(b)**. `AppealBreakdown.RoleBoost` joins the other parts, so the parts still add up.
- Why: later roles should mean something in the markets of their era, and appeal is already what wins share (tested against rivals in a crowded sector).

### Q133. Which roles get no perk? DECIDED: the data roles for now, and the roles that already act through their function

- Options: (a) a perk for all 14; (b) none for `labeller`, `data-engineer`, `data-gardener` (they wait for data sources, which live on `cursor/rules-side-items`), nor for `infra`, `policy` and `chip-architect`, which already do their work by function.
- Recommendation and choice: **(b)**.
- Why: a data perk written now would collide with that branch; adding a kind later is one enum value and a data row.

### Q134. What does a room do beyond seating or holding? DECIDED: six perks, each a share per tile under a cap, worked out from the rooms

- Options: (a) nothing (rooms are only for seats and GPU slots); (b) a `roomPerks` table: a room type, a kind, a share per tile and a cap, read from the tiles the lab has built; (c) per-room upgrade commands that are saved.
- Recommendation and choice: **(b)**. Same shape as role perks (Q128): nothing is saved, so no schema change, and a demolished room stops helping that week.
- Why: it makes the size of a room matter and gives the late rooms a reason beyond the seat. Kinds: `ResearchSpeed`, `TrainingSpeed`, `IncidentCut`, `SectorAppeal`.

### Q135. Which rooms get a perk, and which kinds? DECIDED: the research rooms, the agent ops room, the sandbox wing and the robotics arena

- Options: (a) every room; (b) only rooms whose purpose already names an effect nothing else covers.
- Recommendation and choice: **(b)**. Research floor 0.3% per tile (cap 15%), harness control room 0.4% (20%) and launch control 0.5% (25%) speed research; the agent ops room speeds training 0.4% (20%); the sandbox wing catches 0.4% of incidents (20%); the robotics arena lifts appeal in the four robot and device sectors 0.3% (25%).
- Why: the hardware test lab already speeds chip ramps (`chips.testLabRampPermille`) and the recruiting office, product studio and containment wing already feed role perks; the data rooms and compute halls wait for data sources and for the Bodies and Upward expansions, which live on `cursor/rules-side-items`. The compute halls differ by GPU slots only, which is already their whole job.

### Q136. How do room perks combine with role perks and with each other? DECIDED: they add within a kind, and the incident catch is capped at 90% together

- Options: (a) multiply; (b) add within a kind; cap the incident catch of rooms and staff together.
- Recommendation and choice: **(b)**. Research speed multiplies the points after the era's research share and the researchers' traits; training speed is added into `SpeedPermille` beside techs and engineers; the wing's catch is added to the red-teamers' before the 900 cap; the arena's appeal joins the specialists' as `RoleBoost`.
- Why: additive terms are what the existing speed and boost lines already do, and the validator keeps every cap at or below 900 so a perk never makes a chance certain.

### Q137. Do founders and styles live in config, and what do they save? DECIDED: two config tables and two ids per lab, save schema 22

- Options: (a) hard-code four founders; (b) a `founders` and a `styles` table in the config, the choice saved as two ids on the lab, everything else read from the config each time; (c) save the founder's effects as numbers on the lab.
- Recommendation and choice: **(b)**. `LabState.FounderId` and `StyleId` (empty for none, and for every rival), `MigrateTo22` adds them as empty, `NewGameOptions.FounderId/StyleId/HomeBlocId` choose them, `simcli --founder/--style/--home` too.
- Why: same shape as role and room perks (nothing derived is saved). Schema 22 chains after 21; if another thread takes 22 first, renumber on merge. The golden digest is re-recorded because the two new fields enter every lab's save.

### Q138. What does each founder do? DECIDED: a start change plus one lasting edge and one lasting cost, from the bible's four

- Options: (a) start changes only; (b) start changes plus lasting terms; (c) also seat starting staff ("two loyal students").
- Recommendation and choice: **(b)**. Professor: reputation +150, grant 1.5x, research +10%, appeal -10%. Dropout: cash 1.3x, hype +250, appeal +5%, research -5% (the board's growing distrust has no hook yet, so it is a research cost). Defector: cash 1.2x, reputation +100, research +6%, and `openaye` starts at -600 toward the player (a feud). Heir: cash 3x, reputation -50, every hire 15% dearer.
- Why: (c) is left out because staff need seats and the start has no rooms; a lasting term is the honest version of "loyal". Every term is signed and bounded by the validator so no founder is a free win.

### Q139. What do the styles do? DECIDED: appeal up in a focus set of sectors and down everywhere else

- Options: (a) per-domain capability ceilings (the bible's speciality points); (b) a sector-appeal term, since appeal is what wins share and is already explained in parts.
- Recommendation and choice: **(b)**. Sniper (coding, enterprise, documents): +25% in six sectors, -8% elsewhere. Shotgun (chat, image, video, voice): +8% in eight, -2% elsewhere. Wildcard (security and science): +35% in four, -10% elsewhere. Founder and style add into `RoleBoost` beside role and room perks, floored at -50%.
- Why: capability ceilings per domain would need new model state; appeal gives the same sniper-or-shotgun choice with no more state. Hardware Baron, Open-Weights Rebel and Garage Hacker wait for chips, open weights (rules-side-items) and shady offers.

### Q140. What does choosing the home base do, and what happens when the lab is sold? DECIDED: it sets the bloc, and a sold lab loses the choices while a re-founded lab keeps them

- Options: (a) no home-base option; (b) `NewGameOptions.HomeBlocId` picks any world-map bloc; (c) bloc-specific founding bonuses.
- Recommendation and choice: **(b)** for the option; (c) is not needed because blocs already carry their own rules on the map. A lab sold to new owners (found-again or spectate) loses its founder and style so the successor is no longer "you"; `FoundAgain` hands both to the new player lab.
- Why: the founder is the person, not the company.

### Q141. What happens when a rival poaches someone? DECIDED: it makes an offer that waits four weeks, and nobody answering means the person leaves

- Options: (a) keep the instant raid; (b) an offer that waits for an answer, with a cap of open offers; (c) a pop-up that pauses the game.
- Recommendation and choice: **(b)**. `talent.offerWeeks` 4 and `talent.maxOpenOffers` 2; the offer is saved on the lab (`poachOffers`, save schema 23). Setting `offerWeeks` to 0 gives back (a). The package is the era's hire fee times 6 (twice that for a star).
- Why: the bible says poaching is constant, so the player needs something to do about it; a wait gives a choice without ever stopping the clock, and an unanswered offer costs exactly what it always cost, so old saves and the golden run play the same.

### Q142. What can the player counter with, and what does it cost? DECIDED: four counters, money sure but dear, a mission free but unlikely

- Options: (a) one "match the offer" button; (b) four counters with a cost and a chance each; (c) a negotiation that goes back and forth.
- Recommendation and choice: **(b)**: `money` (100% of the package, always works), `compute` (40%, base 80%), `title` (10%, base 45%), `mission` (free, base 35%). Morale above the baseline and reputation above 500 add to the chance (floor 5%). The cost is paid whether or not it works; a counter that works lifts morale.
- Why: the choice is between cash, compute you may not have and the lab's good name; the chance is taken apart in a `ChanceParts` so the UI can explain it. A back and forth would need a conversation state.

### Q143. How does the player poach back? DECIDED: a headhunter's fee that is always paid, a package on success, and a feud that deepens

- Options: (a) no poaching back; (b) the same maths in reverse; (c) a fixed fee and a flat chance.
- Recommendation and choice: **(b)**: 15% of the package as a fee (always spent), the package on success. Chance is 40% plus unhappiness times 0.6 plus reputation above 500 times 0.3. The rival loses 15 points of feeling on a success and 3 on a failure; only working rival labs can be raided and a free seat is needed.
- Why: it uses the same package and morale as the defence, and raiding a rival that is already feuding is cheap, raiding a friend costs the friendship.

### Q144. Do the planners and the autopilot answer offers? DECIDED: no

- Options: (a) planners answer with the cheapest counter that is likely to work; (b) they never answer.
- Recommendation and choice: **(b)**. A lab on autopilot loses its poached people a few weeks late.
- Why: answering is the player's decision, and the planner code is shared with other threads. The suggestions system can add a prompt later without touching the rules.

### Q145. What do press rooms do, and are they new rooms? DECIDED: two new stateless rooms, the press room and the auditorium, that dampen scandal and sharpen launches

- Options: (a) a press role that staffs a comms desk; (b) rooms with perks like the later rooms (Q134 to Q136); (c) a flat "PR budget" setting.
- Recommendation and choice: **(b)**. The bible lists the press room (launches, crisis comms, keynotes) and "room → auditorium", but the config had neither. `press-room` opens in The Punchline and `auditorium` in The Chat Boom, both without seats (new room purpose `Press`). Two room-perk kinds: `ScandalDamping` (a share off every reputation loss from an incident or hearing) and `LaunchBuzz` (a share of 120 reputation points when a model first goes on sale). Together they are capped (60% and 100%).
- Why: it is the same derived-perk pattern as the other rooms, so nothing about rooms is saved and a sold room stops helping at once. A press role would need a seat, a planner and hiring balance for one effect.

### Q146. What do journalists do, and what happens if the player ignores them? DECIDED: they ask about beats, and an unanswered question just lapses

- Options: (a) an ignored question runs a bad story (a reputation cost); (b) an ignored question lapses with no effect, so only answering changes anything.
- Recommendation and choice: **(b)**. Four beats (a launch, an incident, a hearing, the Gauntlet crown) bring one of three journalists to the player's desk for 3 weeks, up to 3 questions at a time. The stake is the reputation at play (40 for a launch, what the beat cost or paid otherwise). Open questions are saved on the lab (`pressInquiries`, save schema **25**; 24 is left to another thread).
- Why: with (b) a lab on autopilot plays as before (the golden run moves only by the new field and the new "journalist asked" events), and the new system is all opportunity. (a) would tax every existing game and rebalance the soak.

### Q147. How does the player answer? DECIDED: three answers priced in sales fees, each a chance of winning a share of the stake or losing a share of it

- Options: (a) one "issue a statement" button; (b) three answers with a cost, a chance and a downside; (c) a free-text negotiation.
- Recommendation and choice: **(b)**. `honest` is free, always lands and wins back 25%. `spin` costs 6 sales fees, starts at 45%, and a press room's damping raises its chance one for one; it wins 70% and loses 60% on failure. `exclusive` costs 15 fees, starts at 75% and wins 60% for a 15% downside. The journalist adds to the chance (the trade writer +10%, the tabloid -10%) and scales a gaffe (the tabloid 150%). The chance is split into parts so a screen can explain it. Money is booked under the Hiring ledger line (another line would be another schema) and the dice are the existing `Rivals` stream (a new stream would move the saved state of every game).
- Why: honest is the safe floor, spin is a bet the press room turns into a sure thing, and the exclusive is the dear safe option for a lab without one: three useful choices.

### Q148. Can the player turn the press on a rival? DECIDED: yes, a leak, for a rival with a recent incident, with a backfire

- Options: (a) no; (b) `LeakStory` on a rival that had an incident in the last 26 weeks, for a fee, with a chance and a traced-back cost.
- Recommendation and choice: **(b)**, because the bible names it ("leak its problems to the press"). It costs 20 sales fees win or lose, starts at 45% (damping and reputation help), and if it runs the rival loses 70 reputation (less its own press rooms) and 12 feeling toward the player; if it is traced the player loses 40 reputation (less the player's damping) and the rival 4 feeling.
- Why: it is the press version of poaching back (Q143): a cheap shot at a rival that already stumbled, with a price for being caught.

### Q149. Do the planners, the autopilot and the rivals use the press? DECIDED: no

- Options: (a) planners answer journalists and rivals leak; (b) neither.
- Recommendation and choice: **(b)**. No planner or rival code was touched. Rivals do not build press rooms, answer or leak, so a rival's launch earns no buzz.
- Why: the planner code is shared with other threads, and a rival leaking at the player needs a rivalry design of its own (rivals own Q150 and up). The save format would not change.

## The Gauntlet, deeper (NEXT item 17)

### Q160. How does the player "teach to the test"? DECIDED: one lab-wide tuning setting that costs money and lifts every score

- Options: (a) a per-entry choice; (b) one setting for the lab (0 to 300 permille) with a prep bill and a score lift; (c) a research tech.
- Recommendation and choice: **(b)**. `SetGauntletTuning` stores it on the lab (saved, schema **26**). At the Gauntlet the lab pays 10 sales-role hire fees per 100 permille (booked under Hiring) and every score, bracket and crown alike, is lifted by half the tuning (300 gives 15%). A lab that cannot pay, or is bankrupt, enters untuned.
- Why: the entries are already the models the lab put on sale, so a per-entry choice would mean a choice among models the player already made. One slider is the whole decision: how much to risk the lab's name for a win. Paying at the Gauntlet (not at the command) means the player can change their mind all year.

### Q161. What is the downside? DECIDED: a tuned win can be exposed, more likely with feuding rivals

- Options: (a) none; (b) a flat chance; (c) a chance that grows with the tuning and with the rivals who dislike the player, rolled only if the tuning actually won something.
- Recommendation and choice: **(c)**. The chance is twice the tuning in permille (100 is 20%) plus 10% for each working rival in a feud, up to 95%. Exposure strips the lift from every entry (the Gauntlet is ranked again, so the win can go to a rival), costs 80 reputation (less scandal damping) and refunds nothing. The roll uses the existing `Ranking` stream and is made only when a tuned lab won something, so a game with no tuning draws no extra luck.
- Why: the bible asks that rivals be able to expose overfitting. A lab that wins nothing has nothing to expose, so the risk only bites when the gamble paid. Tying it to feuds gives the rivalry system a new job without touching it.

### Q162. What is the live demo? DECIDED: a deterministic reliability check on the crown entry

- Options: (a) a dice roll; (b) a pure function of the model's weekly incident chance; (c) leave it to the Unity scene.
- Recommendation and choice: **(b)**. A model at 0.8% a week or less is a triumph (+20 reputation), 2% or more is a meme (-30, less damping), in between nothing happens. It applies to every working lab. `demoMemeFromPermille` 0 switches it off.
- Why: the bible says reliability decides the outcome. A dice roll would draw luck for every lab every year and move every seeded game; a pure function moves only reputation, and a playable scene can show the same result later.

### Q163. What are the joke awards for? DECIDED: two awards, each with a real effect

- Options: (a) cosmetic; (b) a small effect each.
- Recommendation and choice: **(b)**. "Most Confident While Wrong" (the biggest gap between claimed and true alignment, at least 100) costs 30 reputation. "Best Newcomer" (the best-ranked lab, not the crown winner, within 3 years of its first release) gives 15 reputation and 80 morale to the staff. The winners are saved on the year's record.
- Why: a gap between claimed and true alignment is what sandbagging makes, so the first award is a second, public way to be caught. The newcomer award gives a young lab a reason to enter and the morale half gives morale something to do.

### Q164. Do rivals tune, and what is the season table? DECIDED: rivals never tune, and the table is a query

- Options: (a) planners tune; (b) the player only. For the table: (a) saved; (b) a query over the kept history.
- Recommendation and choice: **(b)** and **(b)**. No planner was touched. `GauntletSystem.SeasonTable` scores 5 for a crown and 2 for a bracket win over the kept years, ties by crowns and then by the older lab.
- Why: planner code is shared with other threads, and a rival that tunes and is exposed would be a rivalry design of its own. A saved table would only repeat what the history already says.

## Sector trust and the customer desk (NEXT item 18)

### Q165. What is sector trust, and what does it do? DECIDED: a 0 to 1000 meter per lab per sector that moves appeal

- Options: (a) one trust number per lab (that is reputation already); (b) a meter per lab per sector that scales appeal; (c) a meter that only unlocks things.
- Recommendation and choice: **(b)**. Trust sits at 500 where it changes nothing, and appeal is multiplied by 1 + (trust - 500) x 0.3, so full trust is +15% and none is -15%. It shows as its own `TrustBoost` part of the appeal breakdown, so the parts still add up. It is saved as a list of records on the lab (`sectorTrust`, save schema **27**); a sector with no record is at the baseline, so a lab that sells nowhere has nothing saved.
- Why: reputation already says how the world sees the lab; the bible's point is that each sector's customers judge separately ("trust is slow to win and fast to lose" in the sector where you stumbled). The slope of 0.3 keeps trust worth about as much as a good year of reputation, not a new way to win by itself. A steeper slope or a bigger drop pushed rivals that ship carelessly into bankruptcy in the economy-bounds seeds, because a lab that has an incident every few months sits near zero trust for good.

### Q166. How slow is slow, and how fast is fast? DECIDED: +2 a week and +40 for a bracket win, against -60 for an incident and -100 for being exposed

- Options: (a) symmetric moves; (b) gains well under losses.
- Recommendation and choice: **(b)**. A lab gains 2 a week in each sector where it has a model on sale (a year is about 96), takes 40 for a bracket win, and loses 60 in every sector an incident's model is on sale in and 100 in every bracket whose tuned win was exposed. A sector the lab has left drifts back to 500 by 1 a week and its record goes. Bankrupt labs are frozen. These are all config numbers.
- Why: one incident should undo thirty weeks of patient work, and a tuned lab that is caught should lose more than the 40 the lie bought. Making old trust drift away stops a lab that left a sector from keeping a stale bonus for ever.

### Q167. What does "carries to neighbouring sectors" mean in data? DECIDED: sectors belong to a family, and 30% of any move lands in the rest of the family

- Options: (a) a neighbour list per sector; (b) a family tag per sector; (c) no carry.
- Recommendation and choice: **(b)**. Each of the 33 sectors has a `family` (games, finance, web, language, creative, code, state, physical, science). A trust move (a win or a loss) lands 30% in each other sector of the same family where the lab has a model on sale, with no second-hand carry (carried moves do not carry again). A sector the lab is not in is not touched.
- Why: a neighbour list for 33 sectors is 33 judgement calls and a validator for symmetry; a tag is one judgement per sector, easy to read in the data and to change. Carrying only into sectors the lab sells in keeps the records few and the effect where it can matter.

### Q168. What is the customer desk? DECIDED: an incident writes one complaint to the player, answered with a refund, a patch or an apology, and an ignored one lapses

- Options: (a) an ignored complaint costs more trust; (b) it lapses with no effect, so only answering changes anything.
- Recommendation and choice: **(b)**, as with the press (Q146). An incident that costs the player trust writes one complaint, from the sector where the model earns most; its stake is the trust lost there (60 with the default numbers). The desk holds 3, a complaint waits 4 weeks. `refund` costs 3 sales fees and a week of the model's takings in that sector, always works and wins back 70% of the stake. `patch` costs 4 fees, works 60% of the time (a good reputation helps), wins back 60%, and loses another 30% if it fails. `apology` is free and wins back 15%. Money is booked under Hiring and the dice are the existing `Rivals` stream (no new stream, no moved saved state). Complaints are saved on the lab.
- Why: the incident has already cost the trust, so the desk is only a way to buy some of it back, and a lab on autopilot plays on as before. The three answers sit at the three useful price points: certain and dear, cheap and risky, free and small.

### Q169. Do rivals and planners use trust? DECIDED: rivals gain and lose trust and it moves their appeal; nobody but the player has a desk

- Options: (a) trust is for the player only; (b) every lab has it, only the player answers complaints.
- Recommendation and choice: **(b)**. Every lab's trust moves with its sales, incidents and bracket wins, so a rival that ships carelessly loses customers like the player would. No planner or rival code was touched: no rival answers complaints.
- Why: trust that only applied to the player would make the player's appeal the only one that ever moved. The golden run moves because every lab's appeal now follows its trust. The economy-bounds and lifetime tests pass unchanged with the default numbers; they are sensitive to any change that touches rival revenue (the same seeds fail at other trust numbers, and a scan of 40 seeds shows no more rival bankruptcies than with trust switched off).

## The workplace (NEXT item 19)

### Q180. What does a comfort room do? DECIDED: it lifts the level morale settles at, and morale then drifts there at the usual point a week

- Options: (a) rooms add morale once, when built; (b) rooms lift the resting level, so morale keeps climbing to it and falls back when the room goes.
- Recommendation and choice: **(b)**. Three new Comfort rooms (`coffee-corner`, `kitchen`, `nap-pods`) seat nobody. Their `MoraleLift` perks give points a tile (3, 4 and 5) under a cap each (45, 100, 100) and under a game-wide cap of 150 (`maxMoraleLift`), so a full set settles at 650. A new hire starts at the resting level.
- Why: it reuses the one drift rule that already exists, so debt, incidents and the Gauntlet's newcomer bonus still mean what they did, and nothing new is saved. The cost is that a lift of 36 takes 36 weeks to arrive, which suits a kitchen: people warm to a place slowly.

### Q181. How does the lab keep people from leaving? DECIDED: a share off the chance of a rival's weekly offer, from the kitchen and the recruiting office

- Options: (a) defence makes a counter-offer cheaper; (b) it cuts the chance an offer arrives.
- Recommendation and choice: **(b)**. A `PoachDefence` perk takes 0.8% a tile off the weekly chance (kitchen, to 20%) and 1% a tile (recruiting office, to 30%); the total never passes 60% (`maxPoachDefencePermille`, validated to 900 at most so an offer is never ruled out). The roll is the same single `Rivals` roll as before against a smaller number, so no stream moved.
- Why: the talent market (Q141 onward) already decides what happens once an offer is out, and morale already feeds that through the counter-offer chance. This is the only place left where a room can act before the offer exists.

### Q182. Does morale move research, up and down? DECIDED: up in the shipped data, and the downward slope is there but set to zero

- Options: (a) research moves both ways with morale; (b) it only rises above the baseline.
- Recommendation and choice: **(b)**, with the loss slope as a data field (`moraleResearchLossSlopePermille`, 0 shipped). Above 500, research rises 40% of the distance a thousand points (650 gives 106%, 1000 gives 120%); below it, research is normal. Set the loss slope to 400 and morale 0 gives 80%.
- Why: with both directions on, the economy-bounds and lifetime tests failed for rival OpenAye at several seeds and at every nonzero slope I tried (100 to 500). The cause is that a rival's economy splits in two: it snowballs into the billions or it stalls and goes bankrupt around year 18, and a 1.6% research dip from one incident drop in year 1 is enough to choose which. A 40-seed 40-year scan of the full autopilot lifetime showed 1 rival bankruptcy with no morale effect (seed 19) and 3 with it, at 200 and at 400. That fragility was there before this item (seed 19); the loss slope would only make the games the guards watch more likely to tip. It is for a later balance pass, where it can be tuned together with a rival that is harder to push over. Meanwhile, low morale already costs a lab through poaching and counter-offers.

### Q183. What about the recruiting office's other bible effect, hiring speed? DECIDED: not built

- Options: (a) build a hire delay and let recruiters shorten it; (b) leave hiring instant.
- Recommendation and choice: **(b)**. Hiring is an instant command everywhere, in the planner and in the saved examples; a delay would need a queue of pending hires, a new save field and planner changes.
- Why: kept for a later item once a screen exists that shows pending hires. The recruiting office keeps its offer defence.

## Staff traits (NEXT item 20)

### Q184. Who gets traits? DECIDED: only the player's hires, taken from the hire's id, not from a dice stream

- Options: (a) every lab's hires roll them; (b) only the player's.
- Recommendation and choice: **(b)**. A hire of the player's lab gets a trait from a fixed share table (330 in a thousand in all) using the new person's id plus `traitRollSalt`; the star roll still comes first and still uses the world dice. Plan-run labs hire `steady` or `star` as before.
- Why: rivals' economies are knife-edge (Q182), and a rival whose researchers argue or mentor would move the guard tests at random. The id roll leaves every saved dice stream where it was, and the salt is a data field so a different one gives the same shares to different people. The shipped salt (15) was picked by scanning 40 salts against the 11 lifetime games the economy tests watch (14 of the 40 kept every lab solvent) and then the eight-year balance soak (all but one of those 14 passed it), which says more about those games than about traits (Q182). It decides nothing about balance. A test holds that rolling traits leaves the world dice alone.

### Q185. What does each trait do? DECIDED: one lab-wide number each, summed under caps, plus the two camps

- Options: (a) a rich set of effects per trait; (b) one number each from a short list: own research, training speed, alignment work, incident catch, mentoring, offer chance, resting morale.
- Recommendation and choice: **(b)**. The doomer and accelerationist pull training speed and alignment work opposite ways at 3 to 4% each; the 10X and the desk-sleeper move their own research by 10%; a mentor lifts every researcher 2%; someone who reads the logs catches 4% more incidents; an interviewer adds 10% to the chance of an offer. Sums are capped (`maxTraitSwingPermille` 300, `maxMentorPermille` 200, `maxOfferBoostPermille` 500; the incident catch stays within 0 to 900 with the rest).
- Why: each effect plugs into a number that already existed (training speed, alignment work, catch, research, offer chance, resting morale), so nothing new is saved and every part shows up in a breakdown.

### Q186. What is "too many in one room"? DECIDED: a lab with both camps pays 4% of research for every pair, up to 30%

- Options: (a) assign people to rooms and count per room; (b) count per lab.
- Recommendation and choice: **(b)**. People are role pools with seats, not placed in rooms, so the bible's "in one room" becomes "in one lab". The pair count is the smaller of the two camps; one camp alone never argues.
- Why: a camp of one with a lone opponent costs 4%, which is a small price; a lab built of both camps pays the cap. The player can fire the lowest-morale person of a role but not pick by trait: sorting people by trait is a staff screen job.

### Q187. Who is courted? DECIDED: stars as before, and now whoever is secretly interviewing

- Options: (a) stars only; (b) stars and interviewers.
- Recommendation and choice: **(b)**, lowest morale first as before. The interviewing trait also raises the chance an offer arrives. The talent market's counters work on whoever is courted.
- Why: otherwise the trait would do nothing in a lab with no star, and a poacher going for someone whose calendar says dentist is the joke.

## Sector incident flavours (NEXT item 21)

### Q188. How does a flavour sit beside the era's risk classes? DECIDED: it is the sector's character and multiplies with the class

- Options: (a) fold the flavours into the risk classes (a class per sector); (b) a separate per-sector table that applies to every incident there.
- Recommendation and choice: **(b)**. A risk class is a kind of trouble chosen at random from the era and the sector. A flavour is what any trouble costs in that sector. When both apply the fine and the reputation multiply.
- Why: classes already pick by era; a sector's character should not depend on which class the dice gave, and the support bot's refunds belong to the support sector in every era.

### Q189. What does an incident cost beyond reputation? DECIDED: refunds in weeks of takings, a longer or deeper glitch in that sector, and a sharper trust fall

- Options: (a) a new "refunds" ledger line; (b) refunds booked with fines; (c) only multipliers.
- Recommendation and choice: **(b)** plus the glitch and the trust multiplier. Refunds are `makeGoodWeeks` of the model's takings in the sector, on top of the fine, on the fines line; the incident line in the log shows the whole bill. The era's fine rule multiplies the fine, not the refunds. The glitch weeks and depth are read per sector from the model's last incident tick, so nothing is saved.
- Why: a new ledger line would change the saved totals and the ledger screens for a figure the lab never plans around; the fines line is already "what incidents cost". Everything else is derived, so there is no schema change.

### Q190. Do rivals pay flavours? DECIDED: not yet. A data switch, `safety.sectorFlavoursForRivals`, shipped false

- Options: (a) every lab pays; (b) the player only for now; (c) rivals pay half.
- Recommendation and choice: **(b)**, with the switch. Costs (fine, reputation, trust, refunds) hit the player's labs. The glitch is a quality curve in the sector, so it is the same for every lab. With every lab paying, the content-flow guard (seed 3) saw OpenAye's lifetime game go bankrupt, the same knife-edge as Q182. With the switch off the guards pass. Not salt-fishing this time: the number of rival incidents is the cause, so the honest fix is to wait for the rivals thread.
- Why: rivals' fines in flavoured sectors are bigger by design, and nothing about the bible says rivals must pay the same. Flip the switch once the economy is robust.

## Safety culture and release gates (NEXT item 22)

### Q191. What is culture, and what does it do? DECIDED: a 0 to 1000 number per lab that adds to or takes from the incident catch
- Options: (a) it changes the incident chance; (b) it changes the share of incidents caught before they ship; (c) it changes staff morale.
- Recommendation and choice: **(b)**, 10% of catch at the extremes (`catchSlopePermille` 200, neutral 500; at 300 the eight-year balance soak sat one game over its crown limit, so the gentler slope shipped). It adds to the red-teamers', rooms' and traits' share under the same 90% ceiling, so "staff who report problems" literally means more problems caught early, and the incident-odds breakdown already explains it.
- Why: the catch line is where the bible's staff-report-or-hide idea already lives; the incident chance is tied to alignment and era load and is used by every lab.

### Q192. What moves it? DECIDED: listening staff, answered and lapsed complaints, rushed and gated releases, and a steady drift down
- Options: (a) only staff; (b) staff plus the events the bible names ("rushing a release", "firing or ignoring the safety team").
- Recommendation and choice: **(b)**. A point a week per safety, red-team or alignment person up to +6, against a drift of -2: three people hold it level and nobody lets it slide, which is "ignoring the safety team". A first release under 500 alignment costs 40 (once per model, so adding sectors cannot farm it); a first release through the lab's own gate at or above that line earns 10; loosening the gate costs 60; answering a complaint earns 10 and one lapsing costs 10. Only the player's labs keep a culture that moves.
- Why: each rule is a hook on something that already exists (the weekly step, the release, the complaint desk). Rivals stay at the start level because their economies are knife-edge (Q182 and Q190), and the plan labs never set gates.

### Q193. What does it mean to "have to obey" a gate? DECIDED: a release below it is refused, and loosening it waits and costs culture
- Options: (a) breaking it costs culture but is allowed; (b) a refusal; (c) a refusal that can be lifted after a wait.
- Recommendation and choice: **(c)**. The gate is the least displayed alignment a model may show to go on sale (up to 900), checked like the law's bar before any claim at the door is applied, so the claim cannot buy a way past it. Raising is free and restarts the lock; loosening or removing it is refused for 26 weeks after the last change, then costs 60 culture.
- Why: a rule you can break at will is not a rule; a permanent one is a trap. The displayed meter is what the player can see, so the gate never leaks the true alignment. This keeps the sandbagging gap meaningful (a lab that lies to itself still passes).

### Q194. Is a save change needed? DECIDED: yes, schema 29: three lab numbers, no config in the save
- Options: (a) store the culture level; (b) store a shift from `culture.start`.
- Recommendation and choice: **(b)**: `cultureShift`, `releaseGate`, `gateSetTick`. An older save migrates with all three at 0, which is the starting culture and no gate, and needs no config. Schema 28 is stack2's ledger migration; mine chains after it and skips it the way 25 skipped 24.
- Why: a shift means a new lab, or a lab founded again after a sale, starts at the data's `culture.start` without touching world setup. The golden digest moved only because of the new saved numbers: with every culture effect off it equals the digest of the same game with the three fields zeroed.

## Version naming rot (NEXT item 23)

### Q195. What rots, and how? DECIDED: a per-lab mess number; every training run adds to it and a few words stack onto new names
- Options: (a) names grow only; (b) a mess meter with its own clean-up; (c) rename old models.
- Recommendation and choice: **(b)**. Each new run adds 70 points of mess (up to 1000). A new model is named with one stacked word per 100 points (Pro, Max, Turbo, Ultra, Plus, v2, Final, (new), Final-FINAL, Reborn), so the third version is the first to be "Pro"; the list holds at its last word. The name is fixed when the run starts and saved with the model, so old models keep the names they were given.
- Why: the bible calls it a joke that is also a system: names get worse as you ship, and it should cost something to fix. A meter lets the screen show the rot and lets a clean-up be a plain command. Only the player's labs rot; rivals keep plain names (the same Q182 reason as Q190 and Q194).

### Q196. What does it cost, and what does a clean-up cost? DECIDED: up to 8% of appeal at full mess; a clean-up takes off 400 and costs four sales hires' fees
- Options: (a) 10% of appeal at full mess; (b) 8%; (c) 6%.
- Recommendation and choice: **(b)**, 80 permille at 1000 mess, linear (`confusionPermille`). It is its own named part of the appeal breakdown, after trust and before the Gauntlet crown, so the parts still add up. A clean-up (`CleanUpNaming`) takes off 400 points and costs `cleanFeeHireFees` 4 times a sales rep's hiring fee in the current era ("a little marketing effort", scaling with the era like the press room's). It is refused when the names are already tidy or the cash is short, and changes nothing then.
- Why: 100 permille took OpenAye bankrupt in the forty-year economy guard at seeds 20 and 27 (the same knife-edge as Q182: the player's autopilot loses some share and the rival's game flips). 80 and 60 both passed those guards; 80 was kept because it is the closest to the bible's "measurably confused". Not salt-fishing: one number scanned, logged here. `StoryTests` (reckless vs careful) sets confusion to 0 so it still compares only care.

### Q197. Is a save change needed? DECIDED: yes, schema 30: one lab number, nothing else
- Options: (a) store each model's suffix count; (b) store one mess number per lab.
- Recommendation and choice: **(b)**: `namingMess`. An older save migrates with it at 0 (tidy names), so nothing in an old game changes until the next training run. Names already in the save stay as they are.
- Why: the suffixes are in the saved names; the number is the only thing the next name and the appeal part need. The golden digest moved, and the cause was checked: with no mess added, the saved number and schema number set back to schema 29's, the digest of the one pinned game equals the old golden `e77d4325`. So the move is the new field, the new schema number, and the naming rot itself (three or more runs in that game year, so names and appeal changed).

## The company chat and the glow (NEXT item 24)

### Q198. How does the chat know what to say, and who says it? DECIDED: read off the event log and the payroll, nothing saved
- Options: (a) store a chat log in the save; (b) derive it from events and the people on the payroll each time it is asked for.
- Recommendation and choice: **(b)**. `chat.lines` in the data (35 lines) say which event kind each reacts to, which roles may say it, an optional trait, and (for yes-or-no events) which pick. The first line that fits the event and has someone to say it wins, so trait lines are listed before the plain ones; the speaker is picked from the people who fit by a hash of the event, so the same event always gets the same speaker while that person is still on the payroll. A kind with no line stays out of the feed. A poach is about the lab it was done to (`about: "subject"`).
- Why: events are already data and the bible asks for staff to react in character; saving prose would bloat the save and freeze the jokes. The cost is that a line's speaker can change when people leave, which is acceptable for a chat that is a view. Reading the chat changes nothing (checked against the digest).

### Q199. What does the glow show, and does it tell the truth? DECIDED: the true meters, against a dashboard that sees only the displayed ones
- Options: (a) show the displayed alignment; (b) show the true risk of the worst model; (c) (b) plus a second reading for the dashboard.
- Recommendation and choice: **(c)**. Seven stages in `glow.stages` follow the ladder: calm, humming (weekly incident chance of 1% or more), flickering (rung 2), empty corridor (rung 3, sandbagging), doors locked (rung 4), power dips (rung 5), quiet (rung 6). The worst released or trained model sets the stage. The dashboard reading is the same calculation on the displayed alignment with no corruption, so when a model sandbags or is corrupted the report says `ChartIsMisleading` and the room is further up the ladder than the chart. Brightness is the combined capability of the latest model of each line, up to `fullAtCapability` 10000.
- Why: the bible's "truth through the walls": a careful player learns to trust the room over the chart. Derived only, no save change, no effect on any rule, so the economy guards are untouched.

## Rooms that age (NEXT item 25)

### Q230. What makes a room old? DECIDED: how many eras have begun since it was built, not how many weeks
- Options: (a) weeks since built; (b) eras since built (`eraAge` on each room, +1 at every era change); (c) the room type's own era.
- Recommendation and choice: **(b)**. At each era change after the first week, every room of a player lab gets one older; a refurbish sets it back to 0. Five stages in `ageing.stages` read it: fresh (0), settled (1), dated (2), beige (3), museum piece (5).
- Why: the bible says a room "goes out of date", and what dates a room is the world moving on, not the calendar. Counting eras makes a quick game age rooms faster in weeks and keeps it independent of the scenario's tick length. Only the player's rooms age (the Q182 knife-edge: rival upkeep is part of the economy the guards pin).

### Q231. What does an old room cost, and what does it cost to fix? DECIDED: extra upkeep and a morale penalty from the dated stage on; a refurbish is a quarter of the build cost
- Options: (a) upkeep only; (b) morale only; (c) both, with a cap on morale.
- Recommendation and choice: **(c)**. Dated: +15% upkeep, 20 morale off the resting level; beige: +30%, 40; museum piece: +50%, 60. The morale penalty is the average over the rooms that seat people, weighted by tiles, so one old broom cupboard does not sink the lab, and it is capped by `maxMoralePenalty` 60. `RefurbishRoom` costs `refurbishCostPermille` 250 of the room's build cost per tile times its tiles (paid on the construction ledger line), refused when the room is already fresh or the cash is short.
- Why: the first two stages are free on purpose so a player has two eras of grace and the cost arrives as a warning (a `RoomDated` event, and a chat line) rather than a surprise. The economy guards (EconomyBounds seeds 1, 2, 4, 5, 20, 24, 27, 31, ContentFlow, the soak) stayed green with these values at the first scan, so the numbers were not tuned against them. Old workplace tests that run for hundreds of weeks set `ageing.maxMoralePenalty` to 0 so they still test comfort rooms alone.

### Q232. Is a save change needed? DECIDED: yes, schema 31: one number per room
- Options: (a) store the tick each room was built and derive the age; (b) store the age.
- Recommendation and choice: **(b)**: `eraAge` per room. An older save migrates with 0 on every room (fresh), so nothing in an old game changes until the next era change. (The first draft of NEXT said `builtTick`; the age is what a refurbish resets, so the age is what is saved.)
- Why: a derived age would need the era history, and a refurbish would need a second field anyway. The golden digest moved and the cause was checked: with the ageing stages set to 0 upkeep and 0 morale, the saved number removed and the schema set back to 30, the digest of the pinned game equals the old golden `6b8ed8f9`. So the move is the new field, the schema number and the ageing itself (the pinned game crosses eras, so upkeep and morale changed).

## Control goes stale (NEXT item 26)

### Q233. What makes control stale, and what puts it right? DECIDED: eras since the lab's newest safety tech; any new safety tech resets it for all of them
- Options: (a) each tech decays by its own age; (b) every safety tech decays by the age of the lab's newest one; (c) a saved "last refreshed" date.
- Recommendation and choice: **(b)**. Stale eras = the era now minus the era of the newest tech the lab owns that has an alignment effect. The first `control.graceEras` (1) are free; each era after costs `lossPerEraPermille` (100, a tenth) of the safety techs' summed bonus, never more than `maxLossPermille` (500). So the whole safety stack counts for 90% two eras after the newest piece of safety research, and half at six. Researching any new safety tech makes the newest one this era's, which puts every older one back to full strength.
- Why: the bible says techniques "go stale as models change" and that new research is the cure. Per-tech decay (a) would punish a lab for owning a fine old tech next to a fresh one; the lab-wide reading rewards keeping up. Nothing needs saving: it is worked out from the techs and the date, so there is no schema change. Only player labs go stale (the Q182 knife-edge: a rival's safety is part of its economy).

### Q234. What does a stale control actually change, and how is the player told? DECIDED: the alignment work in training runs; two events and two chat lines
- Options: (a) lower the displayed alignment of finished models; (b) lower the alignment work a run gets; (c) raise the incident chance.
- Recommendation and choice: **(b)**. `TrainingSystem.AlignmentWorkPermille` now takes the date and uses the stale bonus instead of the full one (the other parts, staff, containment, role perks and traits, are unchanged). Models already finished keep what they got. When a new era leaves a lab further behind, a `ControlStale` event says so (once per era until the cap, then no more); a safety tech that wins ground back writes `ControlRefreshed` with how much. Both have plain-English log text and a chat line from the safety people. `ControlStale.Of` gives a screen the era, the newest tech, the stale eras, the loss and the bonus before and after.
- Why: new runs are where the techniques are used, so that is where staleness bites, and it asks for the thing the bible asks for (new safety research) rather than a new button. Not touching finished models avoids rewriting history and keeps the displayed meter stable.

### Q235. What does firing the safety team cost? DECIDED: 15 culture a head for the roles that listen
- Options: (a) nothing beyond the lost weekly gain from Q191; (b) a one-off culture hit per person let go; (c) a hit for every role.
- Recommendation and choice: **(b)** for the roles in `culture.listeningRoles` (safety, red-teamer, alignment-fellow): `control.firedListenerPoints` 15 each, so letting a team of three go costs 45 on top of the weekly gain it stops. Letting anyone else go costs nothing. Culture stays in 0 to 1000, and a refused firing changes nothing.
- Why: the bible lists "firing or ignoring the safety team" as something that erodes control. Ignoring was already the weekly drift of Q191; firing is a decision, so it gets a visible price. Economy and golden: no player-only revenue or cost changed. The golden run did not move (the pinned game never has the player researching safety and then crossing eras), and EconomyBounds, ContentFlow and the soak stayed green at the first values, so nothing was tuned.

## Research leads (NEXT item 27)

### Q236. What counts as a lucky run, and what does it leave? DECIDED: luck of 6% or more leaves a lead on the player's desk, up to three at a time, cold after a year
- Options: (a) a lead for every run; (b) only runs above a luck bar; (c) a dice roll on top of the luck.
- Recommendation and choice: **(b)**. Training already rolls luck of up to plus or minus 8% (`training.luckPermille` 80). A run at or above `leads.minLuckPermille` (60) leaves a `ResearchLead` for that model (`LeadFound`), about one run in eight. The desk holds `maxOpen` 3; a new lead is not found while it is full. An unfollowed lead goes cold after `expireWeeks` 52 (`LeadWentCold`). Only the player's labs find leads.
- Why: the bible says a model sometimes "is better than it has any right to be and nobody knows why"; the luck number is already that. No second dice roll is needed, so no new random stream and no change to anyone's rolls. Rivals have none (Q182).

### Q237. What does following a lead cost, and what comes of it? DECIDED: six weeks of the lab's research output, eight weeks of waiting, then one of five outcomes chosen from the lead itself
- Options: (a) a fixed price; (b) a price in weeks of the lab's own research; (c) a price in cash.
- Recommendation and choice: **(b)**. `FollowLead` costs `followCostWeeks` 6 times the lab's research per week (never under `minCost` 100), so it scales with the era and the team. One lead at a time. After `followWeeks` 8 it ends in one of `leads.outcomes`, picked by weight from a hash of the lead (so the same lead always ends the same way, and no dice are used): a real insight (weight 4, 20 weeks of research back), a training trick (3, +3% training speed for good, all tricks capped at +15%), a data quirk (2, 8 weeks back), the cat (1, nothing) or Tuesday (1, nothing). Following a lead that is not there is refused (`LeadGone`), as is a second at once, or too few research points; a refusal changes nothing.
- Why: following a lead is a gamble on research you pay for in the same currency, which fits the bible's "mystery becomes a research lead"; the two nothing outcomes are the joke the bible asks for. The expected return (about 9 weeks of research on a 6 week price, plus the speed trick) is mildly positive, never decisive. The autopilot never follows leads, so the economy guards see only the lead list, not the rewards.

### Q238. Is a save change needed, and what did the id counter do? DECIDED: yes, schema 32: a lead list and one number per lab; a lead's id is its model's id
- Options: (a) take a fresh id for each lead; (b) reuse the model's id.
- Recommendation and choice: **(b)**. `researchLeads` and `leadSpeedPermille` per lab; an older save migrates with an empty list and 0, so nothing in an old game changes. The golden digest moved and the cause was checked: with the two new fields removed, the schema set back to 31 and the lead-finding hook switched off, the digest of the pinned game equals the old golden `df0a8d9e`. So the move is the new fields, the schema number and the leads found in that game year.
- Why (b): the first build took a fresh id from the shared counter, which shifted the ids of everything made later and nudged the eight-year soak's "careful" preset from 45.8% to 62.5% of crowns for one lab, over the 60% bar. It is the Q182 knife-edge again (a harmless-looking change moves a trajectory, not a rule), and the cure is not to touch shared counters: a lead is at most one per model, so the model's id is already unique. Logged here, nothing tuned.

## Fine-tune and rollback (NEXT item 28)

### Q239. How does a player mend a glitching model? DECIDED: two commands, a fine-tune that pays to end it and a rollback that swaps the old version back in
- Options: (a) nothing, the glitch just runs out; (b) one "fix" command; (c) the bible's two: fine-tune or rollback.
- Recommendation and choice: **(c)**. The bible's state table says a glitching model is "recent incident or bad data; fix with fine-tune or rollback". `FineTuneModel` and `RollBackModel` both need a model on sale that is glitching (an incident, and fewer weeks since it than the longest glitch of the sectors it sells in); otherwise they are refused and change nothing.
- Why: the glitch already costs quality (30% in the shipped data) for weeks, so a way to end it early is a real decision, and two ways with different prices is the choice the bible describes.

### Q240. What do they cost? DECIDED: a fine-tune is a week of the model's sales and buys 40 alignment; a rollback is free in cash and costs 20 reputation
- Options: (a) fine-tune at two weeks of sales; (b) one week; (c) a flat price.
- Recommendation and choice: **(b)** (`repair.fineTuneRevenueWeeks` 1, never under `fineTuneMinCost` 1000), paid on the training ledger line. It ends the glitch now and adds `fineTuneAlignment` 40 to the real and shown meters (never past 1000), so it also helps the next release gate and the glow. A rollback withdraws the model from all its sectors (it goes back to Trained, so it can be put on sale again after a look) and puts back the newest retired earlier version of the same line in those sectors, losing `rollbackReputationLoss` 20. It is refused when there is no earlier version, or when the older one would not pass the lab's own gate or the law's bar today.
- Why: at 30% quality loss, a week of sales pays for itself within about three weeks of a six-week glitch, and longer glitches (a self-driving model is grounded for sixteen weeks) pay far more, so a fine-tune is a good buy early and a poor one late; a rollback costs reputation instead of cash and gives up the newer model's capability. An incident's date is reset by a fine-tune, so rivals trying to leak that incident see old news (a rival's leak window counts from the same date).

### Q241. Is a save change needed? DECIDED: no
- Options: (a) store a "repaired until" date; (b) move the incident date back and use the model and sector fields that already exist.
- Recommendation and choice: **(b)**. A fine-tune sets the last incident date back by the glitch length, which turns the glitch off through the existing rule; a rollback only changes a model's status and sector list and the sector entry's model. Nothing is added to the save and the golden digest did not move (the autopilot never mends models).
- Why: the existing glitch rule already reads those fields; a second date would have to be kept in step with them. The player-only effect keeps the economy guards untouched, and all five hold at 1455 tests.

### Q242. How do office events arrive? DECIDED: a weekly chance worked out from the date, one at a time, on the player's lab only
- Options: (a) roll the game's dice each week; (b) a fixed calendar of events; (c) a chance worked out from the week, the lab and the seed, one event waiting at a time.
- Recommendation and choice: **(c)**. `office.chancePermille` 30 a week once game week `firstAfterWeeks` 26 has passed and nothing is waiting. The pick is also worked out from the date and seed, among events whose needs the lab meets and that it has not had this round; when all have been had the round starts again.
- Why: a dice roll would shift every later draw in a game and move the economy guards (Q182); a worked-out chance is as steady as a dice stream to a player and touches nothing else. The seed keeps two games different. Only the player's lab has them, as with leads and ageing.

### Q243. What do the answers do, and what if nobody answers? DECIDED: small effects in five kinds; an unanswered event lapses with no effect
- Options: (a) effects in dollars; (b) effects in weeks of the lab's own output and costs; (c) also let a lapse cost something.
- Recommendation and choice: **(b) and not (c)**. Reputation points, culture points, morale for all staff, weeks of research output and weeks of weekly running costs. A cash loss is never more than the cash the lab has. An event waits `expireWeeks` 8 weeks, then blows over with a line in the log and no effect.
- Why: weeks scale with the lab, so the same event is a nuisance to a big lab and a decision to a small one. Because a lapse costs nothing, the autopilot (which never answers) and a player who ignores the desk both keep the numbers the balance tests were tuned on; answering is the only way to gain or lose.

### Q244. Which events, and what do they need? DECIDED: eleven from the bible's list, four needing a model, safety staff or anyone
- Options: (a) only the six the bible names; (b) the bible's list in full; (c) invent more.
- Recommendation and choice: **(b)**: the public repository, the cat, the wrapper, the leaked report, the town vote, the open letter, the earnings question, the mascot's drawings, Tuesday, the union joke and the fake video (the bible's "event ideas" list, ch. 14). The wrapper, drawings and Tuesday need a model on sale; the leaked report needs somebody in safety; the cat, town vote and union joke need anyone on the payroll. Every event has three answers, one of which can be to do nothing much.
- Why: events that mention a model or a safety team make no sense without one. Each answer trades two things (reputation against culture, cash against research) so there is no best row.

### Q245. Is a save change needed, and does the golden move? DECIDED: yes, schema 33; the golden moved with a verified cause
- Options: (a) store nothing and derive the event from the date; (b) save the event waiting, its deadline and the round's seen list.
- Recommendation and choice: **(b)**. `officeEventId`, `officeEventExpires` and `officeSeen` per lab (`MigrateTo33` gives older saves none waiting). The pinned golden game moved; with the new data removed, the new sync lines removed, the schema number put back and the hook off, the previous golden `e4f7f263...` came back exactly, so the move is only the new fields, the schema number and the events posed in that game's year.
- Why: the answer must survive a save, and an event picked by date would be lost on an older save. Nothing derived is stored; `OfficeEvents.Waiting` is a view. All guards hold at 1481 tests.

### Q246. How does the player choose a training objective? DECIDED: a command while the model trains, with a neutral default
- Options: (a) a new argument on `StartTraining`; (b) a separate command for a model still training; (c) a research unlock.
- Recommendation and choice: **(b)**. `SetTrainingObjective(model, objective)` works only while the model is training and can be changed freely; the last choice wins. The default objective, `correct`, changes nothing and is stored as empty.
- Why: `StartTraining` is used by rivals, planners and most tests, and a new argument would touch them all. The recipe screen can send both commands in one tap, and the choice can still be changed during the run, like the bible's long wait.

### Q247. What do the four objectives do? DECIDED: engaging sells and erodes control, obedient adds capability and erodes control, careful the reverse
- Options: (a) only change appeal; (b) change alignment, capability, appeal and culture by data-table points; (c) a hidden "disposition" meter.
- Recommendation and choice: **(b)**. Be engaging: -80 alignment, +15% appeal, -10 culture. Be obedient: -60 alignment, +6% capability, +3% appeal, -10 culture. Be careful: +100 alignment, -7% capability, -3% appeal, +5 culture. Alignment moves the real and shown meters together.
- Why: the bible says engagement sells and "breeds flattery" and that engagement or obedience erodes control, so each pays something real and costs control; careful is the honest slow option. A hidden disposition meter would be a second alignment meter to keep in step with the first; the existing alignment gap already carries "partly hidden". Moving the two meters together keeps these models off the sandbagging rung, which reads the gap between them.

### Q248. Is a save change needed, and does the golden move? DECIDED: yes, schema 34; the golden moved with a verified cause
- Options: (a) store nothing (the effects land at the end of the run, so recompute from the objective); (b) store the objective on the model.
- Recommendation and choice: **(b)**. `objective` per model (`MigrateTo34` sets empty). The appeal effect reads it at any time after the run, so it cannot be derived from anything else. Rivals' and the autopilot's models stay on the default, so the economy guards, which all use them, are unchanged.
- Why: the choice has to survive a save. The pinned golden moved because every model now carries the field; with the sync line, the schema number and the data removed the previous golden came back exactly (see the commit message).

### Q249. What does letting a model help design its successor give and cost? DECIDED: a cheaper, stronger run; a successor with less alignment and a culture hit
- Options: (a) a flat speed-up; (b) a speed-up and a capability gain, paid for in alignment and culture; (c) a risk of a takeoff incident.
- Recommendation and choice: **(b)**. The run needs 15% less of the compute it still has to do and finishes 5% stronger; the successor loses 70 real and shown alignment points and safety culture loses 15 when it finishes. Setting it up cannot be undone.
- Why: the bible lists it as something that erodes control, so it must be a real temptation: the gain is large enough to want and the cost is the control meter. A takeoff incident would need the rung 6 rules, which already trigger from real capability and the alignment gap; this keeps those rules as they are and pays the cost up front in alignment and culture.

### Q250. Does it compound? DECIDED: yes, each link of the chain loses 40 more alignment, up to three links
- Options: (a) the same cost every time; (b) rising with the chain; (c) forbid chains.
- Recommendation and choice: **(b)**. A helper that was itself designed with help counts one link; the loss is 70 + 40 per link, with at most 3 links counted. The helper needs at least half of the run's forecast capability, so a weak model cannot be used for a strong run.
- Why: the bible's sixth rung is a model that "improves itself faster than anyone can". A rising cost makes a chain a real decision rather than a switch to flip every version, and the cap keeps a long-running game's numbers bounded.

### Q251. Is a save change needed, and does the golden move? DECIDED: yes, schema 35; the golden moved with a verified cause
- Options: (a) store only on the successor (the helper id); (b) store a flag on the helper too.
- Recommendation and choice: **(a)**. `designerId` per model, -1 when people did it alone (`MigrateTo35`). The chain is found by following the ids, and a helper that was pruned from the museum just ends the chain. The cost lands at the finish of the run, so nothing else is stored.
- Why: one field is enough, and nothing that can be derived is saved. Rivals and the autopilot never use it, so the economy guards are untouched. The pinned golden moved because every model carries the field; with the sync line, the schema number and the data removed the previous golden returned exactly (see the commit message).

### Q252. How does the player pick how long to test? DECIDED: a normal release is the tested one; a separate command skips weeks
- Options: (a) a testing-weeks field on every release; (b) automatic testing time that grows while a model waits; (c) a separate `RushRelease` for a first launch, with the weeks skipped.
- Recommendation and choice: **(c)**. `RushRelease(model, weeks 1 to 6, sectors)` does what `ReleaseModel` does for a model that has not been on sale and refuses all the same cases. A normal `ReleaseModel` is unchanged and carries no rush.
- Why: (a) and (b) would change every release, including those of the autopilot and rivals, and would move the economy guards that depend on them (Q182). Treating the normal release as the tested one keeps all of that exact, and the bible's "rushing" is exactly the skipped case.

### Q253. What does a rush give and cost? DECIDED: +4% appeal for 13 weeks, +0.3 points of incident chance for 26 weeks, and -3 culture, each per week skipped
- Options: (a) only an incident cost; (b) the launch boost and the incident cost; (c) also a trust hit.
- Recommendation and choice: **(b)** plus culture. For each week skipped: `launchAppealPermillePerWeek` 40 for `launchWeeks` 13; `incidentPermillePerWeek` 3 for `incidentWeeks` 26; `culturePointsPerWeek` 3 once. A six-week rush is +24% appeal, +1.8 points of weekly incident chance and -18 culture.
- Why: the bible names two effects: winning the launch window and a likelier incident. A boost that ends after a quarter and a risk that lasts half a year make the rush a gamble, not a free lunch, and the culture cost matches chapter 10's "rushing a release to beat a rival". The existing culture rule only looks at alignment on release; this one is about time.

### Q254. Is a save change needed, and does the golden move? DECIDED: yes, schema 36; the golden moved with a verified cause
- Options: (a) save the release week and the weeks skipped; (b) save only the weeks skipped, and use the model's existing release date.
- Recommendation and choice: **(b)**. `rushWeeks` per model (`MigrateTo36` sets 0). The window is read from `releasedTick`, which already exists.
- Why: one new field is enough. Rivals and the autopilot never rush, so the economy guards, which use them, are unchanged. The golden moved because every model carries the field; with the sync line, the schema number and the data removed the previous golden returned exactly (see the commit message).

### Q255. What is on the ticker? DECIDED: a headline line per public kind of event, about every lab
- Options: (a) reuse the plain-English event text; (b) a headline line per event kind in the data, in capitals, in the bible's voice; (c) only the player's events.
- Recommendation and choice: **(b)**, for all labs. Fourteen lines cover eras, world news, the Gauntlet, incidents, releases, bankruptcy, acquisitions, funding, rivals joining, the keynote, board drama, endings and chips. The player's private events (hiring, building, poaching) have no line. A line may pick on an event's `pick`, which is how a lab's first chip gets the bible's Minesweeper headline.
- Why: `EventText` is for logs and test output; a ticker needs the joke voice and the capitals, and the bible's own headlines are examples of it. The company chat already covers the lab's own life, so the ticker is about the world, and it is data so a writer can add lines without code.

### Q256. What if little happened? DECIDED: pad with filler picked from the date
- Options: (a) show fewer headlines; (b) a pool of filler headlines, padded to a minimum; (c) generate filler from rivals' state.
- Recommendation and choice: **(b)**. At least 5 headlines are shown: real ones first, then filler lines whose year has come, never repeating on one ticker, chosen by a hash of the tick, so the same week always gives the same ticker. Eight filler lines, four from the bible's list.
- Why: an empty ticker in the quiet early years reads as broken, and the filler is half the joke. Hashing the date, not rolling dice, keeps the view read-only and reproducible, in the same way as the company chat.

### Q257. Is a save change needed? DECIDED: no
- Options: (a) store the ticker; (b) read it off the event log.
- Recommendation and choice: **(b)**. Nothing is saved; the golden did not move.
- Why: everything on the ticker already exists in the event log (which is bounded) or the date. Storing it would duplicate the log and need a migration for no gain.

### Q258. What can the player lobby for, and what does it cost? DECIDED: a quiet word or a subsidy push, priced in weeks of running the lab, once per government per half-year
- Options: (a) one lobbying slider that does everything; (b) two named goals, "quiet" and "subsidy", each with its own price and result; (c) let the player pick which law to nudge.
- Recommendation and choice: **(b)**. A quiet word costs 6 weeks of running the lab and takes 150 off a government's attention. A subsidy push costs 4 weeks and, if it wins, pays 8. A government can be lobbied again after 26 weeks; backfires (12% before policy staff, 2% at best) add 200 attention and take 40 reputation. Policy staff cut the backfire chance and lift the subsidy chance (40% up to 80%).
- Why: attention and grants are the two government levers that already exist, so the player gets something to do about a hearing that is coming without a new law system. Pricing in weeks of running costs keeps it relevant from the first year to the last. Letting the player pick the law (c) needs the law system to take a nudge; it is listed as open. The chance is a hash of the week, not a shared die, so the push cannot move anyone else's dice.

### Q259. Is a save change needed, and does the golden move? DECIDED: yes, schema 37; the golden moved with a verified cause
- Options: (a) a lab-wide cooldown; (b) a cooldown per government, stored on its standing.
- Recommendation and choice: **(b)**. `lobbyUntilTick` on every `BlocStanding` (`MigrateTo37` sets 0).
- Why: the bible lets labs work every government, and one cooldown for all would make lobbying a once-a-year button. The golden moved because every standing carries the field; with the sync line, the schema number and the data removed, the previous golden returned exactly.

### Q260. What does a pivot cost? DECIDED: 8 weeks of running the lab, 15% of the staff (the unhappiest first), and -12% appeal for a year
- Options: (a) money only; (b) money, people and a year of confused customers; (c) also drop the lab's reputation.
- Recommendation and choice: **(b)**. `costWeeks` 8, `staffLossPermille` 150 (rounded down, at least one), `confusionAppealPermille` 120 for `confusionWeeks` 52. The leavers are the lowest morale, ties to the earlier hire, so the result is the same every time.
- Why: the bible names exactly two costs, "loses some staff" and "confuses customers for a year", and money is the tycoon's price for anything. The confusion is a line of its own in the appeal breakdown, so the player can see what the pivot is costing. Reputation is left alone: the staff and customers are already paying. Choosing who leaves by morale avoids a die and gives a reason to keep the staff happy before a pivot.

### Q261. How often, and what is saved? DECIDED: once per era, `pivotEraId` and `pivotUntilTick` on the lab (schema 38); the golden moved with a verified cause
- Options: (a) once per game; (b) once per era, stored as the era's id; (c) a cooldown in weeks.
- Recommendation and choice: **(b)**. The lab stores the era id of its last pivot and the tick its customers stop being confused (`MigrateTo38` sets empty and 0).
- Why: the bible says once per era, and the era id is the thing the data already names. A cooldown in weeks would let two pivots straddle an era boundary and none in a long era. The golden moved because every lab carries the two fields; with the sync lines, the schema number and the data removed the previous golden returned exactly.

### Q262. Which ideas from the bible come next, and what is new in the data? DECIDED: eight more events and one new effect, the home government's attention
- Options: (a) only use the existing five effects; (b) add an `Attention` effect so the senator and the regulator can be answered; (c) add a rule for each event.
- Recommendation and choice: **(b)**. Poetry prize, the refusal essay and the honest chatbot (each needs a model on sale), the senator's "cloud or phone", the signed GPU, the rival's demo (a person in another room), and from the "more events" list the safety team's request for five per cent of the compute and the investor who asks why safety exists (each needs a safety team). `OfficeEffectKind.Attention` moves the player's home government's attention by points, inside 0 to 1000.
- Why: the bible says an event with a consequence becomes a system, and a government's attention is the system a senator or a regulator should move. Everything else uses the effects that exist, so no event needs code. Two of them need a safety team so a lab without one never hears about it; the list still goes round once all eligible events have been seen.

### Q263. Does this need a save change, and does the golden move? DECIDED: no, and it did not move
- Options: (a) none; (b) a new field for the effect.
- Recommendation and choice: **(a)**. Events are data, and the effect uses the standing the lab already saves. No schema change; the golden is the same.
- Why: the autopilot never answers an office event, so a longer list changes nothing it does, and no new field is stored.

### Q264. What is an era goal, and how many are there? DECIDED: a metric and a target in the data, three per era, 100 points each and 50 for all three
- Options: (a) hard-coded goals in code; (b) data: a metric from a fixed list, a target and points; (c) free-form conditions.
- Recommendation and choice: **(b)**. Eight metrics (releases, sectors served, crowns, reputation, staff, techs, lifetime revenue, best alignment), all read off the lab's state, so a goal needs no new saved number. Every era asks for lifetime revenue and two others; the targets are cumulative and climb with the era's starting money.
- Why: the bible says the campaign "sets goals for each era", which is data a designer will keep tuning. A fixed list of metrics keeps the rules simple and testable, and cumulative targets mean a missed goal in one era can still be caught up in the next.

### Q265. When is the score taken, and for whom? DECIDED: when the next era begins, for the player only, once per era
- Options: (a) read the score live and never save it; (b) save a mark when the era ends; (c) mark continuously.
- Recommendation and choice: **(b)**, plus a live query for the run's total (`RunScore`) that adds the current era as it stands. A game that starts in a later era has no marks for earlier ones (the player's founding tick is compared to the era's start), and a bankrupt lab is not marked.
- Why: a live score would let a player cash in a goal and then drop below the target; a mark taken at the era's end is what "goals for each era" means. Only the player is marked because rivals have no campaign, and a late start should not be punished for eras it never played.

### Q266. Is a save change needed, and does the golden move? DECIDED: yes, schema 39; the golden moved with a verified cause
- Options: (a) save the marks; (b) rebuild them from the event log.
- Recommendation and choice: **(a)**. `eraScores` on every lab (`MigrateTo39` sets an empty list). The event log is bounded, so it cannot rebuild a whole run's score.
- Why: the golden run crosses eras, so it now carries a mark and an event for each. With the sync line, the era hook, the schema number and the data removed, the previous golden returned exactly. Nothing in the sim reads a mark, so no decision, price or dice roll moved.

### Q267. What can the player choose, and what do the choices do? DECIDED: an approach (known or your own variant) and a checkpoint habit (now and then or often), both while the run trains
- Options: (a) every run can fail by default; (b) failure only for what the player opts into; (c) an architecture tree of its own.
- Recommendation and choice: **(b)**. A variant adds 8% to the finished capability and fails 1.5% of training weeks; checkpointing often drops what a failure costs from half the run to a tenth, for 4% of a week of running costs each training week. The defaults change nothing.
- Why: the bible ties higher ceilings to "fail more often", which is a risk the player picks. Making every run fail (a) would change what the autopilot and rivals do and move the economy guards (Q182); (b) leaves them exactly as they were. A tree (c) needs the research side of architecture, which is its own job.

### Q268. What does a failure do? DECIDED: it throws away the habit's share of progress and safety work, from a hash of the week
- Options: (a) the run is lost; (b) a share of the progress is lost; (c) the run is delayed a fixed number of weeks.
- Recommendation and choice: **(b)**, and the same share of the safety work, so the run does not end up with safety that outruns its progress. The chance is a hash of the seed, the week, the lab and the model, so no shared die is used, and a failure with nothing to lose is skipped.
- Why: the bible's example, "a data bug found in week nine", is about lost weeks, and checkpoints are what decide how many. Losing the whole run (a) makes the experimental choice a trap; a flat delay (c) would not be changed by checkpoints.

### Q269. Is a save change needed, and does the golden move? DECIDED: yes, schema 40; the golden moved with a verified cause
- Options: (a) two strings per model; (b) pack them into one.
- Recommendation and choice: **(a)**. `approach` and `checkpoints` on every model (`MigrateTo40` sets both empty).
- Why: two ids, two fields, in the same shape as the objective. The golden moved because every model carries them; with the sync lines, the hooks, the schema number and the data removed the previous golden returned exactly.

### Q270. How do engineers cut failed runs? DECIDED: each engineer takes 6% off a variant run's weekly failure chance, up to 60%
- Options: (a) a flat chance cut per engineer; (b) a share of the chance per engineer, with a cap; (c) engineers set the checkpoint loss instead.
- Recommendation and choice: **(b)**, `engineerSavingPermillePerStaff` 60 and `engineerSavingCapPermille` 600 in the `runs` data. The chance a variant run fails in a week is its approach's chance (1.5%) less that share, rounded down to whole permille.
- Why: the bible lists "fewer failed runs" among what engineers do. Training speed already scales with engineers up to a cap, so the same shape is used. A share of the chance cannot push it below zero, and the cap means no crew makes a variant run safe. Known architectures never fail, so nothing changes unless the player chose a variant.

### Q271. Does this need a save change or move the golden? DECIDED: no, the chance is worked out each week from the staff on the books
- Options: (a) store a per-model saving; (b) work it out when the week runs.
- Recommendation and choice: **(b)**. Nothing new is saved and the golden did not move, since the golden run trains only known architectures.
- Why: derived numbers are not saved (Q182 practice); an engineer fired mid-run should change the chance the next week.

### Q272. How is a hearing question asked? DECIDED: as an office event with a hearing trigger, posed when the home government reaches the Hearing level
- Options: (a) a new hearing system with its own state and command; (b) the existing office-event machinery with a trigger field.
- Recommendation and choice: **(b)**. `OfficeEventConfig.Trigger` is `Random` (the weekly chance) or `Hearing` (never drawn by the chance). The answer uses `AnswerOfficeEvent`, so the Unity event panel needs nothing new.
- Why: a hearing question is a choice with effects, which is what an office event already is. It needs no save field and no command, and the log, text and chat already know how to show it.

### Q273. What do the answers move, and what happens to the automatic hit? DECIDED: heat, reputation, culture, morale and money, on top of the unchanged hit
- Options: (a) the answer replaces the flat reputation loss; (b) the answer comes on top.
- Recommendation and choice: **(b)**. Four questions with three answers each, every answer a trade (a promise costs culture, an honest forecast costs morale, a dodge costs reputation and heat). A hearing at home only; a waiting office event lapses; the questions rotate before repeating.
- Why: replacing the hit would change what every unanswered or autopilot hearing costs and move the economy guards (Q182). The bible's "share price" has no lever outside the bubble work, so reputation stands for trust and the effect is left open.

### Q274. Is a save change needed, and does the golden move? DECIDED: no
- Options: (a) store the hearing's bloc on the lab; (b) pose only at home.
- Recommendation and choice: **(b)**. Only a hearing at the home government asks a question, so the heat effect (home attention) is always the right bloc and nothing is saved. The golden run does not reach a hearing as a player.
- Why: a hearing abroad needs its own bloc stored, which would be a schema change for a rare case; it is listed as open.

### Q275. What do posts through it and only works on what is interesting do? DECIDED: posts make launches and scandals louder; the other does more on variant runs and less otherwise
- Options: (a) posts leak secrets; (b) posts add to launch buzz and to what scandals cost; (c) only the loud half. For the third trait: (d) a flat research change; (e) a research change that depends on what the lab is working on.
- Recommendation and choice: **(b)** with +12% each way per poster, capped at 30%, and **(e)** with the work being interesting while a variant run (item 38) trains: +20% to their own research then, -20% otherwise.
- Why: the bible lists the traits without numbers. Live posting is naturally both: free reach when the news is good and a megaphone when it is bad. "Only works on what is interesting" needs a notion of interesting that the player controls, and a variant run is the experiment the game already has. Both stay small so the economy guards (Q182) hold.

### Q276. What does "will quit by blog post" do? DECIDED: they leave the first week their morale is under 300 and the post costs 20 reputation
- Options: (a) a random chance of quitting each week; (b) a morale line.
- Recommendation and choice: **(b)**. No dice are used, so no saved stream moves. The player can see it coming and prevent it with morale (comfort rooms, rest, not running into debt). The reputation loss goes through the press room's damping (and a poster in the lab makes it louder).
- Why: the bible says "leave in protest" and "quits by blog post" as jokes from mechanics, and a mechanic the player can read is funnier than a random one. A line below the resting level of 500 means the trait is only a risk in a lab that is already in trouble.

### Q277. Does this move the golden, and how was that checked? DECIDED: yes, because the new traits change who the golden run's hires turn out to be; with their hire shares at zero the previous golden returned exactly
- Options: (a) give the new traits no hire share (never hired); (b) hire them like the rest.
- Recommendation and choice: **(b)**, 4%, 3% and 4%. No save change: the trait id was always saved. The golden moved to `b1bf2703`.
- Why: a trait nobody can hire is no trait. They come after the existing bands in the roll, so nobody who had a trait lost it. The effects are small and only reach labs that get the people, and none of the economy guards moved.

### Q278. How much can be borrowed, and against what? DECIDED: half of what the lab's owned GPUs would fetch today, less what it still owes
- Options: (a) a flat limit by era; (b) a share of the lab's cash flow; (c) a share of the chips' resale value.
- Recommendation and choice: **(c)**, 50% (`loanToValuePermille` 500), counting interest in what is owed. Rented and reserved compute and the lab's own chip designs are not collateral, since the first two are not the lab's and the last has no market price.
- Why: the bible says "against your chips". Resale value already falls a generation at a time, so the borrowing room shrinks as the chips age, which is the lesson ("whatever the chip is worth by then") without a new number. Half leaves the lender a margin.

### Q279. How is it paid back, and what if it is not? DECIDED: weekly over a year with flat interest; three missed payments in a row and the lender takes the chips
- Options: (a) interest every week on a falling balance; (b) flat interest once, equal weekly payments; (c) a margin call when the chips lose value.
- Recommendation and choice: **(b)**, 8% over 52 weeks, a 10% penalty on a missed payment and a default after three. The lender takes the most valuable GPUs first and writes off what they do not cover. No margin call.
- Why: the bible says repayment is "on schedule, whatever the chip is worth by then", which rules out (c). Flat interest keeps the sums whole and easy to read. Taking the best chips first is what hurts, and the write-off keeps a lab from owing more than it ever had.

### Q280. Where does the money go on the books, and is a save change needed? DECIDED: the capital line both ways; yes, schema 41, and the golden moved with a verified cause
- Options: (a) a new ledger line for loans; (b) the existing capital line.
- Recommendation and choice: **(b)**, with payments as negative capital. One list of loans per lab (`MigrateTo41` gives every lab an empty one). The golden moved only because every lab now carries the list: with the sync line and the schema number reverted, the previous golden returned exactly.
- Why: a new ledger line touches every total and every display (Q182 practice); a loan is capital in and out, so the capital line says what happened.

### Q281. How is the escape attempt made a timed crisis? DECIDED: an office event with an Escape trigger, posed by a rung-5 incident, that takes a default answer if it lapses
- Options: (a) a new crisis state and command; (b) the existing office-event machinery with a trigger and a lapse choice.
- Recommendation and choice: **(b)**. `OfficeTrigger.Escape`, and `lapseChoice` on the event: when nobody answers in time that choice is taken for the player. Every other event has none and still just blows over.
- Why: the crisis is a choice with effects on a clock, which is what an office event already is, and it needs no save field and no new command. "Timed" only means something if running out of time costs something, hence the default.

### Q282. What are the answers? DECIDED: pull the plug (costly, clean), gamble on the sandbox (cheaper, half clean, noticed), watch (free, worse)
- Options: (a) a real dice gamble on containment; (b) three sure answers that trade money against corruption.
- Recommendation and choice: **(b)**, with a new `Corruption` effect on every model on sale. The default for a lapse is "watch", which raises corruption by 200 and takes 10 safety culture.
- Why: a dice gamble would need its own chance rules and a way to show them. Three sure answers are readable and each costs something real: ten weeks of running costs for the clean break, a night of rebuilding and bad press for the middle, and a more likely Loose ending for doing nothing. The gamble is listed as open.

### Q283. Is a save change needed, and does it reach the economy? DECIDED: no save change; rung 5 only follows from corruption the shady path builds
- Options: (a) store the model that tried to escape; (b) apply the effects to every model on sale.
- Recommendation and choice: **(b)**. Nothing new is saved and the golden did not move.
- Why: rung 5 needs corruption of 450 or more, which only comes from shady contracts, so labs that never choose them see none of this. For labs that do, the answers are the choice the bible asks for.

### Q284. What is a scenario? DECIDED: a named bundle of the choices a new game already has, filled in from the data
- Options: (a) scenarios that also set up a starting situation (a lawsuit under way, a hall half built); (b) a named bundle of era, founder, style, home bloc, names and rivals.
- Recommendation and choice: **(b)**, seven of them: four founders in their natural year, the 1995 prologue, a quiet alignment-minded lab in the Nordics and a two-rival race. `Scenarios.Options` and `Scenarios.Start` turn one into a game. A game from a scenario has the same digest as one made by hand with the same options.
- Why: the bible asks for "a named archetype in a specific year". Everything such a start needs is already a new-game option, so the data-only version is faithful, testable and changes no rules. Starting situations need a save field or setup code per scenario and are listed as open.

### Q285. Is anything saved? DECIDED: no
- Options: (a) remember which scenario a game began as; (b) nothing.
- Recommendation and choice: **(b)**. The game is exactly what the options make; no schema change and the golden did not move.
- Why: nothing in the rules would read it. A screen that wants "you are playing the Heir's Gambit" can keep that outside the rules.

### Q286. Do rivals pay sector flavours now? DECIDED: yes, `safety.sectorFlavoursForRivals` ships true

- Options: (a) leave it false until the rivals thread has merged; (b) turn it on in this chain once a forty-year soak shows no extra bankruptcies; (c) rivals pay half.
- Recommendation and choice: **(b)**. Checked on a throwaway merge of this chain with the rival-robustness work (PR #3, `RunwayRetrenchment` and Q200-Q201), never pushed: forty seeds (1-40) of forty years with the autopilot player, counting plan labs that ended bankrupt. Switch off: 0 of 40. Switch on: 0 of 40. A second range, seeds 101-180, ended with 12 rival bankruptcies with the switch off and 5 with it on, so the switch does not add bankruptcies; the Q201 bar of zero holds on its own seeds 1-40 only, which is for that thread to know. On this chain alone (the older retrenchment rule), seeds 1-40 had 1 bankruptcy with the switch off (seed 12) and 0 with it on. The full suite passes with the switch on, the golden digest did not move, and the content-flow and economy-bounds guards are green.
- Why: Q190 turned it off because rival incidents bankrupted OpenAye's lifetime game; with retrenchment before debt that no longer happens. The switch stays in the data, so it can be turned off again if a future change makes the economy knife-edge.

### Q287. Which events come next, and what do they need? DECIDED: six more from the bible's list, with two new needs, a ready chip design and rented compute

- Options: (a) only events that need nothing, a model or staff; (b) add needs for the chip and rented-compute events so they only appear when they make sense; (c) a new trigger kind.
- Recommendation and choice: **(b)**. `OfficeRequires.OwnChip` (a ready design in `chipDesigns`) and `OfficeRequires.RentedCompute` (rented GPUs on the books). The six events: a researcher leaves for a competing lab, a rival's chief executive is fired on Friday and back on Tuesday, the model asks what "off" means (uses the corruption effect from Q282), a loud reply that takes the servers down, the first chip's Minesweeper benchmark, and the landlord who trains its own models sends a friendly note about your usage.
- Why: events with no right answer are the bible's "event ideas". A chip event for a lab with no chips, or a landlord event for a lab that rents nothing, would read as noise. Nothing is saved, no schema change. The golden digest did not move.

### Q288. What does the foundry snub do, and to whom? DECIDED: a 20-week slip on the player's own new orders when the vendor's standing is under 350, one chance in four

- Options: (a) a year's slip as in the bible's table; (b) a shorter slip with a roll; (c) a lost order.
- Recommendation and choice: **(b)**. `chips.snubStandingBelow` 350, `snubChancePermille` 250, `snubWeeks` 20 (a bit under half the 48-week year). It reads the vendor's standing, which falls when a lab makes its own training chips (Envydia's reaction), so the risk is the price of independence. The roll is a hash of the seed, the week, the lab and the order, so it draws nothing from the dice stream and nothing is saved; `ChipSlotSnubbed` goes in the log. Player labs only, and only the lab's own orders, not chips bought from another lab's list.
- Why: a year would end a run of chips outright and rivals' chip buying is plan-driven economy that must stay knife-edge safe (Q182). A lost order would need a refund path. A slip uses the delivery tick the order already has. No schema change; the golden digest did not move.

### Q289. Does serving too much hurt reliability? DECIDED: yes, an overloaded player lab adds up to 6 points to every model's weekly incident chance

- Options: (a) quality penalty only; (b) extra incident chance scaled by the share of demand left unserved; (c) a new meter.
- Recommendation and choice: **(b)**. `safety.overloadIncidentPermille` 60 times the unserved share of last week's demand, added at the weekly roll (`SafetySystem.WeeklyChancePermille`), shown as `FromOverload` in the incident breakdown. Player labs only (`safety.overloadForRivals` false): the rivals' incident counts feed the knife-edge guards (Q182, Q190), so rivals keep rolling exactly as before.
- Why: the bible's "overworked" state says reliability drops. The mood already exists and a quality hit would need a new term in the appeal breakdown; the incident chance already has a breakdown and is where "reliable" lives. Nothing is saved, no schema change.

### Q290. Do the release notes need an answer? DECIDED: yes, a small event on a model's first release, never replacing one already waiting

- Options: (a) flavour text in the log only; (b) a choice event on the first release; (c) a choice on the release command itself.
- Recommendation and choice: **(b)**. A new office trigger, `Release` (like `Hearing` and `Escape`), three events (the changelog, known issues, a joke), posed by `OfficeEvents.OnRelease` when a model first goes on sale. Unlike hearings and the escape crisis it does not replace an event that is waiting, so a release never wipes out something the player has not answered. Player labs only. Small effects (reputation, attention, culture, morale; letting the model write them adds 30 corruption).
- Why: the bible's tone rule says a joke without a consequence is flavour text and one with a consequence is a system. A new field on the release command would change `Command` and every caller for a few points of reputation; the event machinery already has the answer, expiry and lapse. No schema change. The pinned golden game moved: the golden player releases a model, so the log now has a posed (and lapsed) release note; moving the three events to another trigger gave the old digest back, so that is the whole cause.

### Q291. What is the third share of compute, and what does it buy? DECIDED: a player-set share of general compute for research experiments, at +1.2 permille of research per permille set aside, up to 30%

- Options: (a) research always needs compute at a fixed rate; (b) a share the player sets, taken off general compute before training and serving divide it; (c) a separate pool of experiment GPUs.
- Recommendation and choice: **(b)**. `lab.researchSharePermille` (schema 42), `SetResearchShare`, limit `research.maxComputeSharePermille` 300, bonus `research.computeShareBonusPermille` 1200. The share comes off `LabQueries.Split` general compute and own-chip compute, so training and customers see the same smaller pool everywhere the split is used. The bonus multiplies the researchers' output (`TrainingSystem.ResearchPerWeek`), so a lab without researchers gets nothing.
- Why: the bible says "split compute three ways" and only two ways existed. A fixed rate would change every lab's economy at once and the rivals' balance (Q182); a share starts at zero, is player-only and is refused for rivals, so defaults change nothing. A separate GPU pool would need new holdings and orders. Schema 42 is the one new saved field; the golden digest moved only because of it (checked by reverting the field and the schema number).

### Q292. Can an office event belong to a stretch of years? DECIDED: yes, `fromYear` / `toYear` on the event (0 for open), six events use them
- An office event may now name the first and last calendar year it can happen in. `OfficeEvents.Eligible` takes an optional tick and drops events outside their years; the weekly draw passes the current tick. Called without a tick (tests, tools) the years are ignored.
- Six events use it: the modem handshake (to 2003), demo day (2009 to 2016), the muffin (2012 to 2021), the sentence (2017 to 2023), the phantom flights (2024 on) and the weekend request (2029 on). The older 33 events carry `fromYear: 0, toYear: 0` and happen at any date.
- The validator rejects negative years or a first year after the last.
- The golden digest did not move: the draw is only for the player and the golden run sees none of the new events. Rivals have no office, so the economy is untouched.
- Alternatives: era ids instead of years (rejected: scenarios can move era dates); a separate `requires` kind (rejected: a requirement is about the lab, not the calendar).

### Q293. Can filler headlines belong to an era? DECIDED: yes, `toYear` joins `fromYear`, and 18 new lines use it
- A filler line may now name a last year (`toYear`, 0 for no end), the same way an office event names its years (Q292). `NewsTicker.PickFiller` skips a line after its last year; the validator rejects a line that ends before it starts.
- 18 new lines cover the dial-up days (modems, a homepage, a calendar, a chess match), the dot-com bust, the phone and cat-photo years, autocomplete and the poem prize, homework, the chatbot therapist, agent inboxes, harnesses, household robots and the orbital launch delay. The older eight keep `toYear: 0`.
- Every calendar year from 1995 has at least five lines open (a test checks 1995 to 2045), so the ticker is never padded short.
- The ticker is never saved and rolls no shared dice: the golden digest did not move and nothing about the economy changed.

### Q294. Can chat lines belong to an era? DECIDED: yes, `fromYear` / `toYear` on a chat line, seven lines use them
- A chat line may name the first and last calendar year it is said in. `CompanyChat.Fits` compares the event's tick, so an old event keeps the voice of its time when the feed is read later.
- Seven lines use it: a hire in the dial-up days (to 2003), the chat boom (2022 to 2028) and the upward era (2033 on); a release by sales to 2011 and from 2024; a finished model to 2016 and from 2026. They sit after the trait lines and before the plain lines, so personality still beats era, and era beats the plain line.
- The validator rejects negative years or a first year after the last. The chat is read off the event log and never saved: the golden digest did not move and the economy is untouched.
- Two chat tests that pinned the plain "hired" line now ask for it in 2012, a year no era line covers.

### Q295. Which chip and loan events make the news? DECIDED: tapeouts, respins, yield crashes, foundry snubs, chip sales, loans, missed payments and defaults
- Eight new headline lines for events that had none: `ChipTapedOut`, `ChipRespin`, `ChipYieldCrash`, `ChipSlotSnubbed` (item 47), `ChipsBought`, `LoanTaken`, `LoanMissed` and `LoanDefaulted`. A repaid loan stays quiet (good news is not news).
- Every lab's events reach the ticker, so a rival's tapeout or default makes the paper too. The ticker is never saved and rolls no shared dice, so the golden digest did not move and the economy is untouched.
- Alternatives: also headline `ModelSold`, `StaffQuit` and `GauntletAward` (rejected for now: they would crowd out the release and incident lines the player cares about).

### Q296. How do the people react to chips and loans? DECIDED: ten chat lines for eight event kinds
- Ten new lines: `ChipTapedOut` and `ChipRespin` each have a line for chip architects and engineers and a plain line for anyone else; `ChipYieldCrash`, `ChipsDelivered` and `ChipSlotSnubbed` have a plain line that says the chip or week count; `LoanTaken`, `LoanMissed` and `LoanRepaid` have a plain line each.
- Chat is read off the event log and never saved: the golden digest did not move and the economy is untouched. One chat test that counted the table now expects 72 lines.
- Alternatives: lines for `LoanDefaulted` (rejected: the `InDebt` and bankruptcy lines already speak for it and a default is the end of a conversation, not the start).

### Q297. Which press and talent events make the news? DECIDED: leaks (both ways), poached staff, exposed Gauntlet scores and launch buzz
- Five new headline lines: `StoryLeaked` (pick 1 for a leak that ran, pick 0 for one that backfired), `StaffPoached`, `GauntletExposed` and `LaunchBuzz`. A poach by no particular lab reads "A LAB HIRES AWAY...".
- Left out on purpose: `TrustLost` (one event per sector, many with no loss) and `ChinaDealOffered` (an offer, not an event the paper would run).
- The ticker is never saved and rolls no shared dice: the golden digest did not move and nothing in the economy changed.

### Q298. Can the player ask the team to work late? DECIDED: yes, crunch time: +25% research and +15% training speed for -8 morale a week and a rising incident risk, up to 26 weeks
- New state `lab.crunchWeeks` (schema 43, `MigrateTo43`), command `SetCrunch(on)`, `Crunch` system, event `CrunchEnded`, command error `TeamTooTired`. Player only; nothing changes until it is turned on, so no rival, soak or economy guard moves.
- It costs morale (8 a week against a 1-point drift), adds incident chance (2 permille per week up to 40) and ends when the team is worn out (average morale under 250), after 26 weeks, when the player says, or when no one is left.
- It cannot start below an average morale of 450. A full 26-week crunch ends near 320, so the next one waits; the long-run gain is a few percent, not 25.
- Morale below the baseline still does not slow research (Q180), so the real cost is the incident risk, the quits and the wait.
- The golden digest moved to 691a1ad0 because of the new field and the schema number only; reverting both brought the previous digest (3ccf0148) back.
- Alternatives: a research penalty from low morale (rejected: Q180 turned it off on purpose); a cash cost for overtime (rejected: morale and incidents are the cost the bible names for rushing).

### Q299. Can the player insure against incident fines? DECIDED: yes, three levels of 25% cover each, a premium on revenue, claims that load it, and a drop-out at three
- New state on the lab (schema 44, `MigrateTo44`): `insuranceLevel`, `insuranceClaims`, `insuranceLockedUntil`; a new ledger line `insurance` (appended to `LedgerLine`, migrated into every ledger totals object as zero); command `SetInsurance(level)`; `Insurance` system; events `InsuranceClaimPaid` and `InsuranceDropped`; command errors `Uninsurable` and `InsuranceLocked`. All appended.
- Premium per level is the larger of $2,000 and 1.2% of last week's revenue, plus 30% for each remembered claim. The cover is 25% of the fine per level, so level 3 covers 75% and the validator keeps the top level at or under 100%.
- Rung 5 and above (an escape) is never covered, make-good payments and reputation loss are not covered, a claim is forgotten every 52 weeks, and the third remembered claim ends the policy. Changing it locks the policy for 13 weeks.
- Player only and opt-in: no rival buys, the autopilot does not buy, so economy bounds and the balance soak are unchanged. `EconomyBoundsTests` and the soak were not rerun for this item beyond the full suite, since defaults change nothing.
- The golden digest moved to e1ea850c because of the three lab fields, the ledger field and the schema number only; reverting them brought the previous digest (691a1ad0) back.
- Alternatives: a flat premium (rejected: a rich lab would never feel it); covering reputation loss (rejected: insurance pays money, not trust); a one-off deductible instead of a share (rejected: a share scales with the fine).

### Q300. Can the player take the team on an offsite? DECIDED: yes, a week of salaries for +80 morale (never more than 100 above resting) and +15 culture, with no cooldown state
- New command `HoldOffsite`, `Offsite` system, event `OffsiteHeld`, `staffPeople.offsite*` data. No new saved state (so no schema and the golden digest did not move): the limit is the ceiling, resting morale plus 100, so a second offsite gives only what room is left and a third is refused.
- It costs one week of salaries on the hiring ledger line, lifts everyone with room to rise, and adds 15 safety culture points (the player's lab only). Refused when there is no one to take, no cash, nothing to gain, no offsites in the data, or the lab is not the player's or is bankrupt.
- It is how a tired team gets over crunch time's start line (average morale 450) and how a person who would quit by blog post is kept; both are tested.
- The worry was a permanent research bonus from chained offsites. With the ceiling at resting plus 100 the extra research is about 4% and decays at the drift, so it is capped and not a loop.
- Alternatives: a cooldown (rejected: it needs saved state and the ceiling already limits it); a flat morale set to the maximum (rejected: that would be the exploit).

## Open

None.
