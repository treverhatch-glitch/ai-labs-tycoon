# Rooms, projects and economy (GD-6)

**Status:** Builder-ready draft (28 Nov 2026 ICT). **Scope:** HQ lot, draw-room, task pickers, models (four stats), room linking, specialist families, build order and starting tuning. Two Point Hospital look; existing lots, eras and roles stay unless this doc says otherwise.

**Related content (generated tables):** [rooms.md](../content/rooms.md), [room-perks.md](../content/room-perks.md), [workplace.md](../content/workplace.md).

**Out of scope here (future content docs):** staff loop (§6), gigs (§7), partners (§8), money (§9), rivals (§10), dated events (§11).

---

## 0. Three rules the whole sim hangs on

### 0.1 One room, one job, one target

Every staffed room holds **exactly one task**. A task points at **one target**: a model, a base architecture, a gig, a research item, a shipped product, or another room. Staff belong to a **room**, never to a project. Each work tick a seated worker asks its room "what is our task?" and pushes points into that task's target.

Everything else falls out of this:

- Two rooms cooperate by holding **different tasks that point at the same model**.
- Running two things at once means two rooms.
- Per-model profit and loss comes free: a worker's salary is booked to whatever their room's task targets.
- There is no per-room queue. Tasks have **Auto** and **Repeat** toggles instead (§4.5).

### 0.2 Speed and quality are different knobs

- **Speed = head count × morale × policy.** Every work action burns **one training step** of the target, whoever does it. An intern and a legend finish a step equally fast.
- **Quality = skill × desk tier.** The size of the stat points an action adds depends on the worker's matching skill and their desk tier.
- Tooltips: *"More people: faster. Better people: smarter. Nobody: neither."*

### 0.3 Content is data

Eras, techs, room types, desk lines, packages, product types, sectors, ideal-focus tables, experience tracks, rivals, partners, gig templates, perks, events, headlines and joke lines live in `Rules/Data/default-config.json` (or split JSON it includes). Code reads ids and numbers; writers change text without code. `ContentDocsTests` and real-name tests must cover every new section.

---

## 1. The start: an empty Mum's Garage

### 1.1 New-game setup

Existing pickers stay (start era, default 1995; founder; style; home bloc). Add:

- **Signature sector** (one sector open in the start era): +3 to launch score in that sector; users never get bored of it (§3.7).
- **Founder card:** real staff with one boosted profession at 90, ~40 spare points elsewhere, **Founder** perk (unpaid, never quits, morale 100). Professor / Dropout / Defector / Heir set profession and edges.

### 1.2 What is on the lot

- `garage` (14×10) loads with **no rooms and no desks** — floor, door and walls only.
- **Clutter pile** (3 placeables). Sell once for cash and cleared tiles:

| Clutter | Cash | Joke |
| --- | --- | --- |
| Founder's "vintage" gaming tower | $9,000 | "Runs everything from 1997. Including warm." |
| Box of dot-com promo mousepads | $1,500 | "Synergy, 400 units." |
| Mum's exercise bike | $0 (refuse: +5 founder morale for a year) | "Mum says it's 'in use.' It is a coat rack." |

- **Start cash (1995, normal):** $60,000 — enough for one 4×4 General Dev Room with 3 basic desks, Research Corner, Coffee Corner and Bathroom, plus ~8 weeks of two cheap salaries. Later start eras keep era kit cash and pre-built techs and **still start empty**.

### 1.3 The draw-room tool

- Pick a room type, then **drag a rectangle or paint tiles** (connected shape; 3×3 bounding minimum staffed, 2×2 comfort). Live **tile cost**, **seat estimate**, **overcrowding line**.
- One door tile on any walkable edge. Corridors paint-only, not a hard gate.
- **Resize, move, delete** (50% build refund), **copy blueprint**.
- **Desks placed by the player** — one desk line per room family (§5.1). Worker must sit at a matching desk. **Five desk tiers**, era-unlocked.

### 1.4 The four starter rooms

All other build menu types are greyed with unlock cost and research.

| Room | Id | Family | Job | Joke |
| --- | --- | --- | --- | --- |
| General Dev Room ("Everything Room") | `general-dev-room` | Dev | Train models, base architectures, updates, dev gigs | "EVERYTHING (open Tuesdays)" |
| Research Corner | `research-lab` (day one) | Research | Techs, sectors, packages, rooms, desk tiers | "AGI by Q3" — new pen yearly |
| Coffee Corner | `coffee-corner` | Comfort | Morale recovery | "The only room where decisions are made." |
| Bathroom | `bathroom` | Comfort | Needs; slow morale drain if missing | "Out of order. The order is 'out.'" |

Bathroom v1: needs target only (no queues/cleaning). General room **Dev desk** line: train bench, label table, eval desk, polish station are **cosmetic variants of one desk family**, not separate pipeline slices.

### 1.5 First hour (tutorial)

1. Sell clutter. Draw General Dev Room. Place 3 desks. Seat founder.
2. Draw Research Corner, Coffee Corner, Bathroom.
3. Hire 2 cheap applicants (§6).
4. Research trending sector + 2 capability features.
5. Client gig in Dev Room (3 starters always on board, §7).
6. First own model in New Model wizard (§3).
7. Save ~$180k, research Safety & Evals, build in reserved space.

---

## 2. Rooms and their task pickers

Click a staffed room: **task picker** below. Exactly one active task. HUD: idle rooms ("3 rooms idle: they are 'thinking'").

| Room family | Task groups |
| --- | --- |
| Dev | **Train model** (new, version-up, spin-off, distil, port) · **Updates** (patch, fine-tune add-on, Pro add-on) · **Build base architecture** · **Client gig (dev)** · **Commissioned model** · **Pair up** (§4.1) |
| Research | Research item · Auto-research (category) · Pair up |
| Data | Data package on a model · Polish a model · Data gig · Pair up |
| Safety & Evals | Alignment package · Hallucination fixing · **Eval report** (shipped) · Polish · Safety gig · Pair up |
| Polish (Product Studio) | Polish package · Polish · Persona/voice gig · Pair up |
| Device Lab | Device package · Polish · Device gig · Gadget orders |
| Marketing | Campaign (model or shipped) · Stunt · Market research · **Headhunting** · Pair up |
| Support Desk | Answer tickets · Community campaign · Merch orders · Pair up |
| Training Room | Course (skill × tier) · Auto-train |
| Chip Design Lab | Existing chip tasks ([chips.md](../content/chips.md)) |
| Compute (unstaffed) | Fleet capacity only (existing rules) |

Same-family rooms can **swap tasks** in one click ("move this model to the bigger room").

---

## 3. Models: wizard, four stats and score

### 3.1 New Model wizard (Dev → Train model)

Six screens with running **training steps**, **upfront fee**, **estimated score band**.

1. **Basics:** name; **size class** (Toy → Frontier); product type; **sector + sub-sector**; **audience**; **lead researcher** (§3.4).
2. **Architecture + surfaces:** own/licensed base (§10.3); 4 feature slots; up to 3 surfaces (install base, complexity, tech cap).
3. **Capability features:** researched; steps, cost, stat points; sector fit colours.
4. **Focus sliders:** 6×30 points + 3 direction sliders (open/closed, cautious/edgy, consumer/enterprise); hidden ideals (§3.5).
5. **Priority split** across four stats (default 40/20/20/20).
6. **Budget:** compute / data / polish (Low/Normal/High); pay upfront to start.

### 3.2 The four stats

| Stat | Main source | Skill sizing |
| --- | --- | --- |
| **Capability** | Dev actions, architecture, Device packages | research / engineering |
| **Data** | Dev actions, **Data packages** | data |
| **Alignment** | Dev actions, **Alignment packages** | red-teaming |
| **Polish** | Dev actions, **Polish packages** | product |

Release gates and incidents read Capability and Alignment here. Stats show 0–100 vs frontier bar (§3.6).

### 3.3 Build phases

Feature-by-feature steps; one Dev action = one step + stat points (priority split × skill × desk). ~1/20 **breakthrough**; rare dev events (~1/800). At zero steps: complete screen (keep training, launch, open weights, shelve, scrap, hand over). Polish phase: ongoing stat gain, ~1/8 actions clear a hallucination.

### 3.4 Hallucinations

Chance per Dev action (surfaces, skill, perks). **30% silent** until Safety/polish/launch backlog. Launch penalty on open bugs; post-launch **tickets**; Support clears; user loss past backlog thresholds (§14). Patches on shipped models. Lead researcher: research skill bonus to Capability in **every room on that model**.

### 3.5 Hidden ideals and Eval Reports

Product type × sector hidden ideals. **Eval Report** on shipped model reveals sliders when close enough; else "too high/low". Market research (§7) and user letters add hints.

### 3.6 Score

Stat score ÷ **frontier bar** (monthly, ~×2.2/era). Deductions: missing packages (§5.3), hallucinations, focus error, experience, funding. Named lines on score card. 97+ clamped; 98–100 rare roll.

### 3.7 Experience stars

0–5 stars per modality, audience, surface, architecture; roll on launch (§14). Signature sector exempt from boredom repeat penalty.

### 3.8 Sales and saturation

Score → demand curve; hype/trend multipliers; **saturation cap by score band** (hype cannot lift cap). Decay; users → followers and tickets.

---

## 4. Linking rooms (Plumbing)

Visible cables: data blue, alignment green, polish pink, pair-up white, service yellow. Hover: "Wired: Labelling Room → ChatAye Mini 2".

### 4.1 Pair-up

Same family: X pairs to hub Y; X runs Y's task with X desks/skills; salaries book to Y's target; X idles when Y idles; links point at hub; pairs **persist across hub task changes**.

### 4.2 Package attach

Data / Safety / Polish / Device rooms pick a **model in training**, tick **up to 6 packages** (levels 1–6 unlock). Fixed share of model steps (§14). Paid up front; refund on cancel. **Alive only while a Dev room (or pair) trains that model** — otherwise cancel + refund. Same-family split packages; cross-family parallel.

### 4.3 Polish fallback

After last package tick: auto **Polish** on same model (~12% package efficiency); Safety first tries **Hallucination fixing**.

### 4.4 Follow-this-team

Specialist **follows** a Dev room (dashed cable); on new train task, auto-attach with default package ticks (override per model). Follows pair hubs.

### 4.5 Auto and Repeat gigs

Idle staffed rooms take client gigs (§7). **Auto** / **Repeat**. Priority: own task → package → polish → gig → idle.

### 4.6 Service links

Campaigns, tickets, Eval Reports, patches, gadget orders → **shipped** product (or training model for campaigns).

### 4.7 Summary

| Situation | Player move |
| --- | --- |
| One model, one team | Dev room trains |
| One model, bigger team | Second Dev pairs up |
| One model, many disciplines | Specialists attach packages |
| Split one discipline | Two rooms split package levels |
| Two models | Two Dev rooms; specialists split |
| Between projects | Polish or gigs on Auto |

Spaghetti icon joke at 4+ cables; no mechanical penalty.

---

## 5. Specialist rooms, unlock order, expected packages

### 5.1 Room families and desk lines

Desk lines per family, 5 tiers, **+8% quality per tier** above 1.

| Family | Rooms (tier order) | Stat / job | Unlock (approx) |
| --- | --- | --- | --- |
| Dev | `general-dev-room` → `engineering-bay` → `agent-ops-room` | Steps + all stats | day one; Bay 2009 |
| Research | `research-lab` → `research-floor` → `harness-control-room` → `launch-control` | Techs | day one |
| Safety & Evals | `safety-lab` → `sandbox-wing` → `containment-wing` | Alignment, Eval Reports | $180k, 1995+ |
| Data | `labelling-room` → `data-team-room` → `synthetic-data-lab` | Data | $260k, 1997+ |
| Polish | `product-studio` (research from 2003) | Polish | $320k, 2003+ |
| Marketing | `sales-office` → `press-room` → `auditorium` | Hype, headhunting | $90k, 1995+ |
| Support | `support-desk` (research from 1999) | Tickets, merch | $70k, 1999+ |
| Training | `training-room` (new) | Skills | $120k, 2001+ |
| Device | `hw-test-lab` / `device-workshop` / `robotics-arena` | Device packages | 2012+ / 2026+ / 2029+ |
| Chips | `chip-design-lab` | Silicon | `chip-team` |
| Compute | `server-closet` → `gpu-hall` → … | Capacity | existing |
| Comfort | `coffee-corner`, `bathroom`, `kitchen`, `nap-pods` | Morale, needs | existing |

Era is a floor; higher-tier rooms share tasks with bigger desks and more package levels. **No automatic supersession** — specialists contribute via **player-chosen packages**, pair-up, or follow-this-team.

### 5.2 Package flavour (data)

| Family | Examples |
| --- | --- |
| Data | web scrape, cleaning, dedupe, human labels, preference data, synthetic |
| Alignment | refusal tuning, red-team, eval suite, constitution, interpretability, containment drill |
| Polish | persona, instruction tuning, tool use, voice, memory, warm personality |
| Device | on-device distil, sensors, motor control, safety interlock, battery diet |

### 5.3 Era-expected packages

From family's **expected year**, score deducts for each **missing package level the era expects**:

| Family | Expected from | Levels then | Deduction / missing |
| --- | --- | --- | --- |
| Alignment | 2012 (It Sees) | 1–2, +1/era | −1.5 Alignment |
| Data | 2009 (The Punchline) | 1–2, +1/era | −1.5 Data |
| Polish | 2017 (It Writes) | 1–2, +1/era | −2.5 Polish |
| Device | 2029 (Bodies) | 1–3 | −1.5 Capability (device products) |

Model card warns before launch ("Expected this era: 3 data packages · you have 1").

---

## 6–11. Deferred systems

| § | Topic | Future content doc |
| --- | --- | --- |
| 6 | Staff, talent market, skills, morale, needs, crowding, crunch, Training room | TBD `docs/content/staff-loop.md` |
| 7 | Client gigs, board, deadlines, Prestige | TBD `docs/content/gigs.md` |
| 8 | Partners, exclusives, self-serve, commissions | TBD `docs/content/partners.md` |
| 9 | Credit line, AI Index, bailout, bankruptcy | TBD (funding docs) |
| 10 | Rivals, architecture licensing | [rivals.md](../content/rivals.md) + extensions |
| 11 | Dated event timeline, tabloid reveals | TBD `docs/content/timeline-events.md` |

---

## 12. Build order (12 steps)

Each step: own PR, sim tests; visual steps need 3/4 preview on `#969696`. Step *n* starts when *n−1* is merged. Step 1 detail: [rooms-rework-step1-plan.md](rooms-rework-step1-plan.md).

| Step | Deliverable |
| --- | --- |
| **1** | Empty garage + clutter; draw-room tool; 4 buildable types; place Dev desks; seat founder |
| **2** | Task picker; idle HUD; gig board (3 starters, Auto/Repeat) |
| **3** | New Model wizard; four stats; hallucinations; score + frontier bar |
| **4** | Pair-up links (hub, salaries, persistence) |
| **5** | Safety & Evals: package attach, refunds, Eval Reports |
| **6** | Data + Polish families; era-expected deductions; package split |
| **7** | Follow-this-team + default package ticks |
| **8** | Staff loop (§6) |
| **9** | Post-launch tickets, stars, saturation, patches |
| **10** | Partners, exclusives, commissions (§8) |
| **11** | Money + event timeline (§9, §11) |
| **12** | Late game: acqui-hires, licensing, bundles, sell line, gadgets |

---

## 13. Starting tuning numbers (§14)

| Thing | Value |
| --- | --- |
| Start cash 1995 | $60,000; clutter $9,000 + $1,500 |
| General Dev Room | $350/tile build, $6/tile/week upkeep, min 3×3 |
| Research Corner / Coffee Corner / Bathroom | $300 / $250 / $200 per tile |
| Desk tiers | $1.2k, $3k, $7k, $15k, $32k; +8% quality per tier; tiers 2–5 open 2001, 2009, 2017, 2024 |
| Base speed | 1 training step per worker per 4 game hours at morale 100 |
| Morale factor | (morale + 15) / 115 |
| Hallucination chance | 4% base; −0.03% per engineering point; +1% per surface complexity level |
| Silent share | 30% |
| Breakthrough | 5% per action (7% on architecture specialty) |
| Polish-phase fix | 1 in 8 actions removes a hallucination |
| Package sizes | 4, 5, 6, 7, 8, 9% of model steps (levels 1–6) |
| Polish fallback efficiency | 12% |
| Toy / Mini / Mid / Large / Flagship / Frontier steps | 400 / 900 / 2,000 / 4,500 / 10,000 / 22,000 (× surface complexity) |
| Frontier bar growth | ×2.2 per era, smoothed monthly |
| Expected-package deduction | Data −1.5, Alignment −1.5, Polish −2.5, Device −1.5 |
| Talent pool | cap 24, 2–5 arrivals/week, 10-week stay |
| Off-profession cap | 45 (55 with perk) |
| Legend rate | 1 in 60 applicants |
| Crunch slider | 70–100%, default 90; +15% speed, 2× morale drain |
| Quit | at 0 morale; small roll under 60 while crunching below 85% |
| Gig board | 16 offers; 3 starters; penalty 15–25%; +1 Prestige per gig |
| Commission deadline | 16 + 14 weeks per size class; bonus +25% to +60% |
| Partner acceptance | 40 + 8 per star − 4 per heart |
| Exclusive deal | at 4 hearts; +35% share; 2–4 years; credits expire after 52 weeks |
| Ticket backlog loss | 1%/week above 40%, 3%/week above 70% |
| Saturation caps | score <30: 1% · 30–49: 4% · 50–64: 10% · 65–74: 18% · 75–84: 32% · 85–92: 48% · 93+: 60% of sector |
| Star odds | 8% + 1% per point above 50; 0 below 40; capped at 2 stars under 60 |
| Bankruptcy timer | warning 4 w, ultimatum 10 w, bailout 16 w |
| Funding Winter | credit −40%, interest +4, rounds −50% |
| Self-serving research | $400k, 2009+ |
| Architecture royalty on commissions | 8% for 2 years |

---

## Decisions

| Id | Decision |
| --- | --- |
| Q9 | Whiteboard / device fiction → typed rooms and packages (§5); press rooms unchanged (Q145). |
| **GD-6b** | Empty garage start; **one room, one job, one target**; pair-up, package attach, follow-this-team; **no automatic supersession** (28 Nov 2026). |

Approved **28 Nov 2026** (GD-6 revised).