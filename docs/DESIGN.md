# AI Labs Tycoon: design

Mad Games Tycoon 2, but you run an AI lab. The product is an artificial mind, the factory is a data centre, and
shipping too well can end the world. Funny, goofy, cute and alive.

This file is the short version every agent and person reads first. It has three parts:

1. [What wins when sources disagree](#1-what-wins-when-sources-disagree)
2. [The game](#2-the-game): the bible's essentials with the owner's decisions applied
3. [The rules core](#3-the-rules-core): how the C# rules are built and what the first slice models

The roadmap is [NEXT.md](NEXT.md). What needs the owner's PC is [SETUP-PC.md](SETUP-PC.md).

## 1. What wins when sources disagree

| Order | Source | Where |
| --- | --- | --- |
| 1 | The owner's decisions | [reference/ai_labs_decisions.md](reference/ai_labs_decisions.md), plus the later decisions listed below |
| 2 | Game Bible v0.2 | [reference/AI-Labs-Tycoon-Game-Bible-v0.2.pdf](reference/AI-Labs-Tycoon-Game-Bible-v0.2.pdf) |
| 3 | Game Bible v0.1 | not in the repo; v0.2 contains it and marks what it added |

### The owner's decisions

- **Engine.** Game rules are plain C# with no Unity types (netstandard2.1), tested with `dotnet test`, no editor
  needed. Unity is for scenes and visuals only. Not browser-first.
- **Building.** Mad Games Tycoon 2 style top-down cutaway rooms, each with a purpose. Buy more land and buildings to
  expand. Two scales: the HQ where you build rooms, and a world map of data-centre sites. Rooms evolve with the eras,
  and the AI visibly glows through the building.
- **Several models per lab**, in lines (like small, mid and flagship tiers), each with its own visible growth, state
  and safety meter.
- **Rivals are parodies** with near-miss names: OpenAye (Jam Saltmen, "ChatGBT"), AntHopticsAI ("Clause"), xAy (Elton
  Husk, "Grokk"), Envydia with its leather-jacket boss Jensun Hwang, and more. Original character designs, no real
  logos.
- **Start year.** A start-era picker. **The default is 1995, "Dial-up Days" (confirmed).** 2009 is a quick start;
  a 1956 prologue is a later unlock.
- **The first products are game AI (confirmed).** In 1995 a lab sells enemy and sidekick AI to video game studios,
  chess and puzzle bots, and simple expert systems. The lab climbs from there through the eras.
- **Endless.** Endings do not end the game. The player can keep playing.
- **Hardware.** The "console" of this game is designing your own GPUs and AI accelerators, which feed back into
  training. Consumer devices (glasses, robots) are side lines; brain-implant chips are very late game.
- **In version 1:** a real shady path (hacking, stealing, copying rivals), circular financing and a bubble that can
  crash the economy, government and China deals, the yearly model tournament, sectors to sell into, shares and
  monopoly.

## 2. The game

### Pillars

| Pillar | In play |
| --- | --- |
| Capability versus control | Every point of model power must be matched by safety work, or risk compounds quietly. This decides the endings. |
| Compute is the map | Territory is GPUs, power and data centres. Renting is flexible and expensive; owning is cheap per hour and slow. Rivals bid for the same supply. |
| Sniper or shotgun | A narrow lab that is the best at one thing and a broad lab that is adequate at everything are both viable and play differently. |
| Rivals with their own plans | Rivals decide from their own cash, culture and the live market, and the player can read their reasons. They should beat a careless player. |
| Funny because it is true | Jokes come from the systems behaving like the real industry. |

Promises to the player: no solved formula (each era changes what works); every number is explainable (tap it and
see its parts; the one exception is a deceptive model corrupting its own readings); low clicks, big decisions; the
building tells the truth first; play as long as you like.

### The loop

Compute, data and talent become a model. The model becomes customers and hype. Those become money. Money buys more
compute, data and talent. Control has to grow as fast as capability.

- **Every week:** split compute between training, serving customers and research; watch runs and incidents.
- **Every few months:** design the next training run and how much safety work it gets; choose what to ship and
  where; hire, fire, buy chips.
- **Every year:** the Gauntlet, the televised model tournament; funding; research direction.
- **Every era:** the rules shift. A new technique, a new market, a new law, a new class of risk.

### Eras

| Era | From | What a model can do | What opens |
| --- | --- | --- | --- |
| 0 Dial-up Days (default start) | 1995 | Rules engines, game bots, spam filters. AI is a dirty word. | Game AI, chess bots, expert systems, then search and spam contracts; the dot-com bubble and crash |
| 1 The Punchline | 2009 | Recognises digits. Sometimes. | Grants, neural nets on gaming GPUs, first GPU racks and cloud rental |
| 2 It Sees | 2012 | Images, speech, board games | First real funding, the talent war, the chip team (inference chip Gen 1) |
| 3 It Writes | 2017 | Fluent text, code completion | Transformers, pretraining at scale, API customers, safety evals, chip Gen 2 |
| 4 The Chat Boom | 2022 | Conversation, images, early video | Consumer apps, chip shortages, the first serious rules |
| 5 Agents | 2024 | Multi-step work with tools | Enterprise seats, coding as a huge market, containment wing |
| 6 Harnesses | 2026 | Teams of agents running for days | Selling work instead of software; AI doing AI research; chip Gen 4 |
| 7 Bodies (expansion) | 2029 | Robots | Hardware lines, manufacturing |
| 8 Upward (expansion) | 2033 | Science at machine speed | Orbital compute, the superintelligence endings |

Version 1 covers eras 0 to 6; Bodies and Upward ship in the data flagged `expansion`. The era table is data
(`eras` in the config; the full content is in [content/](content/eras.md)). An era is a floor, not a window: a tech or
room opens when its era begins and stays open. Each era scales what a training run costs and how fast a researcher
works (x1 up to x60). Hardware generations now do the work the bible gives to "ten times per era": the cost
multipliers were retuned against the compute each generation adds ([QUESTIONS.md](QUESTIONS.md), Q1 and Q35). Every era can
be chosen at the start (`StartEras.Offers` lists them for a picker): 1995 by default, and from 2009 on a quick start with
that era's money, lot, researched techs and the rivals who have joined by then. There is no start screen yet.

Each era also turns a few rules (`EraRules`, the `Era rules` table in [content/eras.md](content/eras.md)): the talent
war (hiring fees from 2012), chip shortages (prices peak in the Chat Boom), customer appetite, incident chance and
fines, and from 2022 a law: a model must *show* a minimum alignment before it can go on sale. Existing customers are
not lost when a law arrives; only new releases are checked. None of it is saved: the year picks the era.

From 2012 each era also opens *risk classes* (bias, a driving crash, misuse, a data leak, an agent that went off-script, runaway agents,
physical harm, loss of control). An incident on a model sold in a class's sectors is that class, with a bigger fine and reputation
hit, so what a lab sells decides what can go wrong. The era banner names the new ones. Only the class of each incident is saved.

Several roles do more than fill a desk (`rolePerks`, [content/role-perks.md](content/role-perks.md)): recruiters win back part of
every hiring fee, red-teamers catch some incidents before they happen, alignment fellows make alignment compute go further, and
product leads and the late specialists lift how much customers like a lab's models in their sectors. Each is the number of people
in the role times a share, under a cap, worked out from the staff each time: nothing is saved.

Some rooms do more than seat people too (`roomPerks`, [content/room-perks.md](content/room-perks.md)): research floors, harness
control rooms and launch control speed research, an agent ops room speeds training, a sandbox wing catches incidents beside the
red-teamers, and a robotics arena lifts the appeal of robots and devices. Every tile adds a small share under a cap, read from the
rooms the lab has.

Who you are is a choice made when the game starts ([content/founding.md](content/founding.md)): a founder (the Professor, the
Dropout, the Defector, the Heir) changes the start and carries a lasting edge and cost, a style (Sniper, Shotgun, Wildcard) lifts
appeal in its sectors and lowers it elsewhere, and the home base is any bloc. The founder and style are two ids on the lab; their
effects are read from the config each time.

Poaching is a market ([content/talent-market.md](content/talent-market.md)): a rival's raid is an offer that waits four weeks, the
player counters with money, compute, a title or the mission (each a cost and a chance), or lets the person go, and can raid a
rival's people in return. Open offers are the only saved part.

The press ([content/press.md](content/press.md)) turns the lab's news into choices: press rooms dampen what a scandal costs and sharpen a
launch, a journalist asks about each beat (a launch, an incident, a hearing, the crown), the player answers (tell the truth, spin it,
offer an exclusive) or lets the question lapse, and a lab can leak a rival's recent incident. Open questions are the only saved part.

The Gauntlet has more to it now ([content/gauntlet.md](content/gauntlet.md)): the player can tune the lab's entries for the test (a lift to every score for a prep bill, at the risk of being exposed), each crown entry takes a live demo whose result follows its reliability, two joke awards carry real effects, and a season table ranks labs over the kept years.

Trust ([content/trust.md](content/trust.md)) is the slow side of reputation: each lab has a meter per sector that grows a little every week it sells there, jumps with a bracket win, collapses with an incident or an exposed tuning, carries to the other sectors of the same family, and moves appeal. An incident also writes a complaint to the player's desk, which can be answered with a refund, a patch or an apology.

The workplace ([content/workplace.md](content/workplace.md)) is what the comfort rooms do for the people in them: a coffee corner, a kitchen and nap pods lift the level morale settles at, the kitchen and the recruiting office cut the chance a rival makes an offer, and happy researchers do more research. Nothing about it is saved; it is read from the rooms each week.

### Models

A lab runs a **family** of model lines at once. Each line is a creature that grows with every version.

| Line | Role |
| --- | --- |
| Mini | Small, cheap, fast. Low ceiling. The workhorse of revenue. |
| Mid | The all-rounder. Most customers, most sectors. |
| Flagship | Wins the Gauntlet and sets the lab's reputation. Huge training bill, most risk. |
| Specialists (later) | Image, video, code, security. Can outscore the flagship in their sector. |

Every model has its own safety meter and one of three visible states: **Healthy** (curious, waves at staff),
**Overworked** (serving load too high), **Glitching** (a recent incident). A training run is a handful of choices
(size, data, compute budget, objective, alignment budget), a long wait, and a result with some luck in it.

### Compute, data and people

- **Compute:** rent by the hour (instant, dearest), reserve for years (cheaper, committed), buy chips and lease
  space (queue, cheap per hour, breaks even against renting after about 18 months of full use), build data centres
  (slow, cheapest), and finally your own chips. Chips come in generations; each new one makes the old fleet worth
  less. One supplier, Envydia, sells the chips everyone needs and decides who gets them.
- **Data:** scrape (free now, lawsuits later), license, pay experts, synthetic, your own users, distilling a rival.
- **People:** researchers (techniques), engineers (training speed, serving cost), safety researchers (control),
  infrastructure, product and sales. Stars matter and get poached.

### Markets and the Gauntlet

Models compete sector by sector. Each sector has buyers, a size, growth, weights for what wins, and its own way of
going wrong. Once a year every lab enters the Gauntlet: one bracket per sector plus an overall crown.

### Money and the bubble

Grants, angels, venture rounds, strategic investors (the chip baron pays you to buy her chips), sovereign funds,
debt, going public. Every round dilutes the founder; lose control and the board can fire you. Circular financing is
the cheapest money in the game and feeds a shared Bubble meter; when it pops, labs with real revenue buy the
wreckage. The lesson carried over from the owner's earlier game: money must keep mattering in the late game.

### Safety and the endings

Capability earns money. Control keeps the game going. The gap between them is the dice. The incident ladder runs
from an embarrassing screenshot (1) through jailbreak scandals (2), sandbagging (3), unauthorised action (4) and an
escape attempt (5) to takeoff (6). Low rungs are frequent, survivable and often funny. Endings are big moments, and
the game continues after them.

### Rivals

Parody labs, each built on an archetype with its own plan, strengths and blind spots.

| Lab | Boss | Archetype | Model |
| --- | --- | --- | --- |
| OpenAye | Jam Saltmen | First-to-market showman. Ships everything, burns cash, safety scandals. | ChatGBT |
| AntHopticsAI | Durian Amodayo | Careful sniper. Trusted, publishes safety research, slow to ship. | Clause |
| xAy | Elton Husk | Rocket billionaire. Limitless cash, builds a data centre in a weekend. | Grokk |
| Goggle DeepMined | Sunder Pitchai | The sleeping giant (joins 2010). Researcher-heavy, slow to ship, first to its own chips. | Gemeny |
| DeepSneak | The Quiet Committee | The lean lab (joins 2023, home `china`). Engineer-heavy, short runs, lives under export rules. | Sneak |
| Metta | Marka Zuckerg | The social graph (joins 2007). Rents, ships consumer-facing models fast. | Facehook |
| Macrosoft | Taya Nadeva | The landlord (joins 2012). Owns halls, buys listings, enterprise sectors. | Copilot |
| Palantear | K. Palant | The gov-tech bench (joins 2017). High release gate, publishes safety research. | Gotham |
| Envydia | Jensun Hwang | The chip baron in the leather jacket. A vendor, not a lab: it rations the GPUs by standing. | sells the GPUs |

More arrivals may come with later expansions (see the bible, chapter 11). Rival detail is in
[content/rivals.md](content/rivals.md).

**Rivals explain themselves.** Each plan-run lab keeps a bounded log (60 lines) of what it did and the numbers behind it
(`DecisionText` builds the sentence from config templates); decisions *not* to act (skipping a run, holding a model
back) are logged once a quarter per line. Rivals have a feeling toward the player (-1000 to 1000) that falls when the
player wins and drifts back; below -500 they lobby the player's home government a little each month. Each has a few
signature behaviours as plan data (a release gate, published safety research, board drama, building halls fast).
Envydia hands out GPUs by standing, charges by standing, holds a keynote each March a generation opens (that month's
supply halves) and writes its own log. Rivals that own GPUs swap batches two generations old for the current one.

**Parody guardrails.** Near-miss names only, never a real name. Original character designs, no real logos. A parody
boss never commits a crime and no text puts invented words in a real person's mouth. A test keeps real names out of
the config text. This is a design rule, not legal advice.

### Tone

Funny, goofy, cute and alive. The lab, the mascots and the glow show what is happening before the dashboards do.

## 3. The rules core

### Layout

| Path | What |
| --- | --- |
| `Rules/` | The rules: plain C# (netstandard2.1, C# 9), no Unity, no packages. Also a Unity package (`package.json`, `AiLabs.Rules.asmdef` with `noEngineReferences`), so Unity compiles the same sources. |
| `Rules/Data/default-config.json` | Every tunable number and every name: eras, lots, roles, rooms, GPUs, techs, model lines, sectors, world events, rivals. |
| `docs/content/` | The content tables (eras, techs, sectors, rooms, events, the world map) for planning. A test keeps them in step with the data. |
| `tools/SimTests/` | The tests (net8, xunit). `dotnet test tools/SimTests`. |
| `tools/SimCli/` | A headless console game for balance work: `dotnet run --project tools/SimCli -- run --years 8`. |
| `unity/` | The Unity project. Scenes and visuals only. |
| `studio/` | The studio harness wiring ([studio/README.md](../studio/README.md)). |

### Rules for the rules

Tests in `tools/SimTests/ArchitectureTests.cs` hold each of these.

1. **No engine.** `Rules/` references nothing but the standard library.
2. **Deterministic.** The same seed and the same orders give the same state on every machine, to the last bit. So:
   whole-number maths only (money in dollars, rates in permille, shares in parts per million), no `float`,
   `double` or `decimal`; no clock, threads, disk or `System.Random`; no mutable statics; every sort has a
   tie-breaker.
3. **Seeded dice, one stream per system.** `Rng` is SplitMix64. Training, safety, the Gauntlet, rivals, the world
   (heatwaves, town votes), chips and the shady path each have their own stream, saved with the game, so adding a
   roll in one system never shifts another. A new stream's state comes from the seed, so an old save gets it on load.
4. **All state is one saved object.** `GameState` holds everything. Each saved class lists its fields once in
   `Sync(Syncer)`, and that one list is the save, the load and the digest. A test fails when a field is missing
   from its list.
5. **The only way to change the game is a `Command`.** New kinds and refusal codes are only ever appended. `Simulation.Apply(command)` checks it completely before
   changing anything, so a refused order changes nothing. Accepted orders are journaled, so a game replays from its
   orders alone.
6. **Rivals play by the player's rules.** A rival is a plan (`PlanConfig`) that issues the same commands as the
   player through the same checks. The player's autopilot is the same planner.
7. **Numbers live in data.** Code holds mechanics; `default-config.json` holds numbers and names. The file is
   validated when it loads, and every problem is reported at once.
8. **Every number can be explained.** A model's capability is stored with its parts (base, tech, carry-over, luck).
   Cash is always the founding cash plus every ledger line.

### One week

A tick is one week: 4 weeks a month, 12 months a year, 48 ticks a year, starting in 1995.

`Simulation.Step()` runs, in order: the era banner and any dated world event for this week → heatwave rolls (first
week of a month, June to August) → rival plans (first week of a month) → `SiteSystem` (heatwaves end, halls and power
deals finish) → research and training for each lab → the market (shares move, revenue is paid) → safety (reputation
heals, incidents roll) → the bills (rent, salaries, rooms, GPU upkeep, the power bill) → month close (`GovernmentSystem`
moves every bloc's attention for every lab, then the books close) → at year end the Gauntlet and market growth.

### What the first slice models

| Area | In the slice | The bible's fuller version (see [NEXT.md](NEXT.md)) |
| --- | --- | --- |
| Time | Weekly ticks from 1995 (or 2009), endless; nine eras as data (1995 to 2033) | Rules that shift per era, not just costs |
| HQ | Rooms on a grid lot; six lots from the garage to a hyperscale campus; 28 room types that open era by era | A few rooms with no mechanical role yet (whiteboard, device workshop) |
| People | Role pools (17 roles across six functions: research, engineering, safety, sales, infrastructure, policy), each needing a desk in its own room | Named staff with traits, morale and poaching |
| Compute | Seven Envydia GPU generations (VooDude 1995 to Blackwall 2026). Rent the newest (instant, weekly rent) or buy it (rack space or a hall, upkeep, resale that falls a generation at a time); owning pays back in about 18 months. The fleet is rows by generation and place | Reserved contracts, Envydia's queue |
| World map | 9 blocs and 11 sites (`docs/content/world.md`): a plot, a hall (kW, build weeks, infra staff, town vote), colo, power deals, a shared grid, power priced by site and cooling, heatwaves, latency, export rules by tier, and per-bloc government attention (Letter, Hearing, Audit, Freeze). Audit can open a shady investigation | Chip-shortage events, protests |
| Chips | The chip design screen (`docs/content/chips.md`): ten techs, a design lab and a hardware test lab, one design at a time picked from a target (inference or training), a foundry node (28 to 3 nm), a die, a number format, memory and cooling; the derived compute, kilowatts, points and wafer cost are whole numbers (Q59). Architects finish the points, tape-out costs money and may need a respin, yield ramps after the first run, and every node has a shared monthly foundry room. Chips are delivered to halls only, feed compute through the maker's software level, and training chips run 15% faster in the lab that made them. Labs can list chips for others; Envydia takes standing for the first training chip, each delivery and the first listing, raises its price for a lab on its own training chips, ambushes at the keynote, and courts the others when one lab runs 30% on its own chips | Cloud deals for renting labs, chip shortages, chiplets and exotic nodes |
| Research | Research points from researchers; 81 techs across nine eras, each opening with its era, with prerequisites | Branching trees, dead ends, patents |
| Models | Mini, Mid and Flagship lines; versions; capability with visible parts; a safety meter per model; Healthy, Overworked, Glitching | Capability domains, qualities, disposition, specialists |
| Training | A run needs compute over weeks; the share spent on alignment fills the safety meter and slows the run | Recipes, data mix, failed runs |
| Sectors | 34 sectors from Game AI Licensing and Chess & Puzzle Bots (1995) to Space Compute (2034); a pool grows only after it opens and slows every year | Trust, sector-specific incidents, specialist lines |
| Market | Customers move gradually toward the better, safer, better-sold model; serving customers needs compute | Pricing, open weights, trust |
| Safety | Incidents by the safety meter (corruption raises odds): reputation loss, a fine, Glitching and Corrupted mood | The six-rung ladder, the alignment gap |
| Endings | Bankrupt bailout, sell the lab (found-again or spectate), monopoly, handcuffs; `PlayerLabId`; frontier research and museum prune | More ending stubs (ousted, paperclipped, …) |
| Shady | Eight contract cards, stash, launder, Heat, monthly investigations, model corruption; player only | Escape rungs tied to corruption |
| Money | Cash, a weekly ledger, monthly books, a start grant, bankruptcy after 12 weeks in debt | The funding ladder, control, shares, the bubble |
| Rivals | OpenAye, AntHopticsAI and xAy from the start, Goggle DeepMined (2010) and DeepSneak (2023) as arrivals; decision logs with reason codes, feelings toward the player, signature behaviours as plan data; Envydia as a vendor with monthly allocation by standing, a keynote and its own log | The rest of the cast, cloud deals for renting labs |
| Ranking | The Gauntlet every year: a bracket per sector, the crown, the safest lab; tuning and exposure, a reliability demo, joke awards, a season table | A playable demo scene, hearings |
| Save | Save, load, digest, replay from orders, strict about unknown or missing fields, schema 5 with migrations from 1, 2, 3 and 4 (fixtures in `tools/SimTests/Fixtures`) | The same |

### The bible's first-playable checks

| Check (bible v0.2, chapter 15) | State |
| --- | --- |
| Determinism: same seed, same inputs, same state hash after 8 years | Tested |
| Save in the middle of a training run and load: same result as not saving | Tested |
| Rules purity: no Unity types in the rules assembly | Tested |
| Speed: 8 game years headless in under 5 seconds | Tested |
| Gap to incidents: the incident rate rises as the safety meter falls | Tested |
| Compute maths: rent against buy breaks even at about 18 months | Tested for rent and buy; reserve comes later |
| Explainable numbers: parts sum to the value | Tested for capability and cash; the rest comes with each system |
| Bot balance over 200 games and 3 presets | `BalanceSoak` + `BalanceTests` (CI default 24/preset; `BALANCE_SOAK_FULL` for 200) |
| Money keeps mattering; bubble; overfit Gauntlet entries | Overfitting is built (tuning and exposure); the rest comes with those systems |
