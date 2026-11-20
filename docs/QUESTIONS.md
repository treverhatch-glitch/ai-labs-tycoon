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
- Recommendation and choice: **(a)**; not built yet (phase E). The `ChipSales` ledger line already exists.
- Why: rivals must use the same commands with readable reasons; a listing is one rule and logs cleanly; deals can be added on top with the funding/Bubble systems.

### Q28. How many chip designs at once? DECIDED: one in design at a time

- Options: (a) one in design at a time (any number Ready); (b) unlimited in parallel.
- Recommendation and choice: **(a)**; not built yet (phase E).
- Why: MGT2's console designer is one project at a time; keeps architects a real constraint.

### Q29. Inference chips: serving only? DECIDED: serving only

- Options: (a) inference chips only serve; (b) they also train at a penalty.
- Recommendation and choice: **(a)**; not built yet (phase E).
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
- Recommendation and choice: **(b)**, drift toward 0 by 5 a month, clamped to -1000..1000. A player who keeps winning ends every rival in a Feud (below -500), so rivals lobby the player's home bloc +15 attention a month (a `Lobbied` log line each time). Chips and shady causes arrive with their phases.
- Why: it is what the design table says; it makes dominance cost something. If the 40-year runs show attention smothering a strong player, `feelings.feudAttentionPerMonth` is the knob.

## Open

None.
