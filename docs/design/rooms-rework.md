# Rooms rework (GD-6)

Mechanics-only design for a **MGT2-style** HQ loop: one **general dev room** at game start handles every model stage at a **basic** level; **specialist rooms** unlock by research and era, then **take over their slice on new models** with **better** output. Workstations are **unique per room type** (label station, eval rig, server console—not one generic desk). Later steps add placeable amenities, per-model picks from each room's **era-growing contribution list**, and room rating. Visual direction stays Two Point Hospital; lot sizes and the world map stay.

Related content pages: [rooms.md](../content/rooms.md), [room-perks.md](../content/room-perks.md), [workplace.md](../content/workplace.md).

**Hatch addendum (28 Nov 2026, ICT):** general room first; specialists supersede **slices**, not the whole pipeline at once; every room pours its own part into every new model.

---

## 7. What AI Labs Tycoon does today

From the shipped room, perk, workplace, and ageing docs plus the default config:

### Already close, and worth keeping

- HQ is a top-down cutaway office on a grid lot, with purpose-built rooms and a separate world map for data-centre sites.
- Lots scale from Mum's Garage (14×10, $0) through Strip-Mall, Office Park, Campus, Megacampus, and Hyperscale; the build panel drags a room type onto empty tiles.
- Room types are era-gated. Work rooms have minimum size, tiles-per-seat (usually 3 or 4; robotics arena 6), build cost per tile, and weekly upkeep. Comfort rooms seat nobody.
- Later rooms add lab-wide perks scaled per tile up to a cap ([room-perks.md](../content/room-perks.md)).
- Staff have roles, salaries, traits, and morale. Comfort rooms lift morale's resting level; kitchen and recruiting blunt poaching ([workplace.md](../content/workplace.md)).
- The product pipeline runs research → train → post-training → eval → red-team → marketing → support → press; chip design has its own rooms.

### The actual gap

- **No starter general room:** New games expect separate era room types immediately; there is no single dev space that runs training, data, testing, and polish at a **basic** tier until specialists exist.
- **Specialists are all-or-nothing on paper:** Era gates unlock room **types**, but the rules do not yet express "**this slice on new models** is handled here, better than the general room."
- **Density:** Tiles-per-seat allows several people per room, but 3×3 minimums read as one-person boxes. Seats come from area math, not **placed, room-typed workstations**.
- **Generic seat fiction:** One desk id for every room; no labelling station vs eval rig vs product desk distinction.
- **Lab-wide perks only:** Room bonuses mostly help the whole lab, not per-model **contribution picks** (classic tycoon: sound studio → sound on the title in flight).
- **Comfort as rooms only:** Coffee corner, kitchen, and nap pods are whole room types, not small items beside workstations.
- **Decor without jobs:** Whiteboard and device benches were held back until a room changed gameplay ([QUESTIONS.md](../QUESTIONS.md) Q9 — superseded by placeables in §8.2).

---

## 8. Adaptation proposal

Mechanics only. Keep Two Point Hospital look and existing lot and role tables; add **`general-dev-room`** (working id) and workstation ids per specialist room type.

### 8.1 Starter lot: one general dev room, many typed workstations

| Rule | Detail |
| --- | --- |
| New game default | Mum's Garage spawns **one general-purpose dev room** (suggested **5×5 or 6×5**), plus a **coffee corner**—not separate research lab + engineering bay as the suggested starter. |
| General room job | For each **new model**, the general room contributes **every pipeline slice** at **basic** quality: training, data features, safety/testing, polish/post-training—until a specialist exists for that slice. |
| Specialist takeover | When the player builds a specialist room (via era and/or tech), that room **takes over only its slice on models started after the room exists**. The general room **stops** doing that slice on those models (still does remaining slices at basic). In-flight models keep the mix they started with. |
| Better specialist output | Same staff time in a specialist room yields **strictly better** progress on its slice than the general room's basic tier (config permille per room type). |
| Seats from workstations | Each **placed workstation** matching the room type grants one seat; staff without a workstation cannot contribute work that week. |
| Workstation uniqueness | **No generic desk.** Each room type places its own workstation id (examples in §8.2). The general room places a **small set of basic workstation archetypes** (train bench, label table, eval desk, polish station) that visually read as "jammed startup." |
| Tiles-per-seat | **Soft cap**; build UI warns on overcrowding. Overcrowding sets a morale stub until step 5. |
| Room ops | **Resize, move, delete**; **copy room / blueprint** from day one (v1: footprint + type + workstation layout stub). |
| Corridors | Optional paint-only; **not** a hard door gate. |

### 8.2 Workstations and amenities

Workstations are placeables on room tiles (never stacking). Amenities add room rating (step 2+); comfort room types keep lab-wide morale perks until step 5.

#### Room-typed workstations (examples)

| Room type | Workstation id (example) | Seat role |
| --- | --- | --- |
| `general-dev-room` | `workstation-train-bench`, `workstation-label-table`, `workstation-eval-desk`, `workstation-polish-station` | Any early-role staff at basic slice rates |
| `research-lab` / `research-floor` | `workstation-research-rig` | `researcher` |
| `engineering-bay` | `workstation-engineering-desk` | `engineer` |
| `labelling-room` | `workstation-labelling-station` | `labeller` |
| `data-team-room` / `synthetic-data-lab` | `workstation-data-console` | `data-engineer` / `data-gardener` |
| `safety-lab` | `workstation-eval-rig` | `safety` |
| `sandbox-wing` / `containment-wing` | `workstation-red-team-pod` | `red-teamer` / `alignment-fellow` |
| `product-studio` | `workstation-product-desk` | `product-lead` |
| `support-desk` | `workstation-support-console` | `support` |
| `server-closet` / `gpu-hall` | `workstation-server-console` (seat optional) + GPU tiles | `infra` where seated |
| … | Same pattern for other seated room types | Matching `rooms.md` roles |

#### Shared amenities (step 2+)

| Placeable | Room rating | Primary effect | Notes |
| --- | --- | --- | --- |
| Water cooler | Low | Morale; walk target | Beside workstations. |
| Coffee machine / cart | Low–mid | Morale | Stacks lightly with coffee-corner room perk. |
| Potted plant, lights, AC | Low–mid | Room rating cap | |
| Whiteboard nook | Mid | Planning micro-perk | Q9 whiteboard as placeable. |
| Server / GPU rack (decor) | Mid | Rating in compute rooms | Not fleet GPU slots. |

**Out of v1:** toilet micromanagement, auto-furnish that hides cost, placing over an occupied workstation.

### 8.3 Core loop: every room → one slice; general room → all slices (basic)

**Principle (MGT2-style):** **Every room contributes its own part to every new model**—sound studio → sound, graphics room → graphics. In AI Labs terms, each room type exposes an **era-growing list of contribution kinds** (training throughput, label quality, eval depth, launch buzz, etc.). On the **model card**, the player **picks** which contributions from which built rooms apply to that model (defaults sensible; min one source per required slice).

| Stage slice | General dev room (always if no specialist) | Specialist room(s) when built | Supersedes on new models? |
| --- | --- | --- | --- |
| Training / engineering | Basic train bench | `engineering-bay`, `agent-ops-room` | Yes — engineering slice |
| Research points | Basic polish station slot | `research-lab`, `research-floor`, `harness-control-room`, `launch-control` | Yes — research slice |
| Data / features | Basic label table | `labelling-room`, `data-team-room`, `synthetic-data-lab` | Yes — data slice |
| Safety / eval / red-team | Basic eval desk | `safety-lab`, `sandbox-wing`, `containment-wing` | Yes — safety slice |
| Post-training / polish | Basic polish station | Product / engineering rooms tied to post-training step | Yes — polish slice |
| Marketing / hype | Basic (weak) | `sales-office`, `product-studio`, `press-room`, `auditorium` | Yes — marketing slice |
| Support load | Basic (weak) | `support-desk` | Yes — support slice |
| Compute | Server closet tiles (unchanged) | `server-closet` → `gpu-hall` → … | Compute rooms add fleet; general room does not host GPUs |

**Era growth:** Each specialist room's contribution list **gains entries** when its era tech unlocks (e.g. safety lab adds "structured eval report" in It Writes). The UI shows only contributions the lab's era allows.

**Per-model picks (step 3+):** Player toggles which enabled contributions from each built room feed the active model; general room contributions auto-disable for slices already superseded.

#### MGT2-style mapping (specialists vs general)

| Classic studio room | AI Labs specialist type(s) | Era (first unlock) | General room basic stand-in | Specialist better at |
| --- | --- | --- | --- | --- |
| Development | `engineering-bay`, `agent-ops-room` | Dial-up / Agents | Train bench | Training throughput |
| Research | `research-lab`, `research-floor`, … | Dial-up → Upward | Shared research rig slot | Research points on model |
| QA | `safety-lab`, `sandbox-wing`, `containment-wing` | Dial-up / It Writes / Agents | Eval desk | Eval depth, incident cut |
| Graphics | `labelling-room`, `data-team-room`, `synthetic-data-lab` | Dial-up → Harnesses | Label table | Data feature quality |
| Sound / post | Post-training + product rooms | It Writes+ | Polish station | Alignment / polish meters |
| Mocap / hardware | `hw-test-lab`, `robotics-arena`, device placeables | It Sees+ | (none—optional weak basic) | Device tests, robotics appeal |
| Marketing | `sales-office`, `press-room`, `auditorium`, … | Dial-up → Chat Boom | Weak hype stub | Buzz, scandal damping |
| Support | `support-desk` | Agents | Weak support stub | Ticket load |
| Server | Compute halls (existing ids) | Dial-up → Upward | — (fleet rooms separate) | GPU slots |
| HR / comfort | `coffee-corner`, `kitchen`, … | Dial-up+ | — | Morale (global) |
| **General dev** | **`general-dev-room`** | **Day one** | **All basic slices** | **Fallback only where no specialist** |

Press room and auditorium stay stateless press rooms (Q145). Whiteboard / device fiction → placeables and typed rooms (Q9).

### 8.4 Staff placement

- Assign **role → room → workstation**; deskless staff idle (no slice credit).
- Walks to coffee corner / amenities; pathing smoke in step 1 ([rooms-rework-step1-plan.md](rooms-rework-step1-plan.md)).
- Per-model contribution picks (step 3) pin **room skill** to the model card without moving staff every week.
- Overcrowding beyond soft tiles-per-seat hurts morale.

### 8.5 Refuse from the classic template

| Refused mechanic | Why |
| --- | --- |
| Hard corridor doors | Optional corridors only. |
| Auto-furnish overfill | Player confirms spend. |
| Decor arms race without cap | Rating capped. |
| Mandatory toilet sim v1 | Deferred. |
| Hidden improvement values | All items show cost and effect. |
| Specialist instantly replaces entire pipeline | **Slice supersession only on new models.** |

### 8.6 Build order

| Step | Deliverable | Depends on |
| --- | --- | --- |
| **1** | **General dev room** + coffee corner; **multiple basic workstation types** in the general room; seats from placed workstations; soft cap; resize / move / delete / copy; pathing smoke. See [rooms-rework-step1-plan.md](rooms-rework-step1-plan.md). | This design doc |
| **2** | Specialist room workstations + amenity set; capped room rating | Step 1 |
| **3** | Era-growing contribution lists; per-model picks; specialist **slice takeover** on new models | Step 2 |
| **4** | Post-training, marketing, support contributions on same model UI | Step 3 |
| **5** | Dirt, janitors, blueprint library, decor tiers vs comfort rooms | Step 4 |

---

## Decisions

| Id | Decision |
| --- | --- |
| Q9 | Whiteboard, device-workshop → **placeables** + typed work rooms. See §8.2. |
| GD-6a | **General dev room** at new game; specialists **supersede slices on new models only**, with better output. |

Approved **28 Nov 2026** (GD-6); **amended same day** per Hatch ICT direction.