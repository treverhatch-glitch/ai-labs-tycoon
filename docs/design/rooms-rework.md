# Rooms rework (GD-6)

Mechanics-only design for bringing Mum's Garage and later lots closer to a **MGT2-style** management feel: many desks per work room, placeable furniture, and (later) per-model room assignments. Visual direction stays Two Point Hospital; existing lot sizes, era gates, roles, and product pipeline stay.

Related content pages: [rooms.md](../content/rooms.md), [room-perks.md](../content/room-perks.md), [workplace.md](../content/workplace.md).

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

- **Density:** Tiles-per-seat allows several people per room, but 3×3 minimums read as one-person boxes in play. There is no furniture layer: seats come from area math, not placed desks.
- **Comfort as rooms only:** Coffee corner, kitchen, and nap pods are whole room types, not small items beside desks.
- **Lab-wide perks only:** Room bonuses mostly help the whole lab, not a named assignment on one flagship model (in classic tycoon games, QA and art pour into a single title in flight).
- **Decor without jobs:** Whiteboard corners, press staging, and device benches were held back until a room changed gameplay ([QUESTIONS.md](../QUESTIONS.md) Q9 — now superseded by placeables below).

---

## 8. Adaptation proposal

Mechanics only. Keep Two Point Hospital look and existing lot, era, and role tables unless a step below says otherwise.

### 8.1 Empty building, drawn rooms, many desks

| Rule | Detail |
| --- | --- |
| Starter footprint | Default suggested first work rooms are **4×5 or 5×5** (research bay + engineering bay), not the legal 3×3 minimum alone. |
| Seats from desks | Each **placed desk** grants one seat; staff without a desk in a work room cannot contribute work that week. |
| Tiles-per-seat | Stays in data as a **soft cap**; build UI warns on overcrowding before confirm. Overcrowding sets a morale stub (full amenity morale in step 5). |
| Room ops | **Resize, move, delete** rooms; **copy room / blueprint** from day one (v1: duplicate footprint + type + desk layout stub). |
| Corridors | Optional paint-only corridors for readability; **not** a hard door gate. |

### 8.2 Furniture: desks plus short amenity list

Placeables sit on room tiles (never stacking a desk). A per-room **quality meter** sums contributions up to a cap; comfort room types keep their lab-wide morale perks until step 5 reconciles tiers.

| Placeable | Room rating | Primary effect | Notes |
| --- | --- | --- | --- |
| Desk | Required for work | Grants one seat when assigned | Blocks under other placeables; removed with room delete rules in step 1. |
| Water cooler | Low | Small morale bump; short social break target | Walk target from desks; no whole-room type needed. |
| Coffee machine / cart | Low–mid | Morale; stacks lightly with coffee-corner room perk | Replaces "buy another coffee corner" fantasy for small labs. |
| Potted plant | Low | Room rating only | Cheap filler; no upkeep spike. |
| Lights (desk / overhead) | Low | Room rating | Dim rooms cap rating until lit. |
| AC / heating unit | Mid | Room rating; trims overcrowding morale penalty | One per room band; not per tile sim. |
| Server / GPU rack (decor) | Mid | Room rating in compute-adjacent rooms | Distinct from real `server-closet` / `gpu-hall` tiles that hold fleet slots. |
| Whiteboard nook | Mid | Unlocks planning micro-perk on research / safety rooms | Satisfies Q9 whiteboard fiction as placeable, not empty room type. |
| Nap pod / snack station | Mid | Morale locally | Does not replace `nap-pods` room perk at campus scale. |

**Out of v1:** toilet micromanagement, auto-furnish that hides cost, placing over an occupied desk, mandatory hygiene sim.

Auto-place may run as **preview only** (show suggested layout before the player confirms spend).

### 8.3 Rooms as classic dev-studio equivalents + per-model support queue

Work room **types** stay the shipped ids. The rework adds **assignments**: when a model is in train, eval, launch prep, or support, named staff slots in the right room types pour progress into **that model** instead of only lab-wide perks.

#### MGT2-style room mapping

| Classic studio room | AI Labs room type(s) | Era (first unlock) | Lab-wide perk today | Per-model queue slot (when built) |
| --- | --- | --- | --- | --- |
| Development | `engineering-bay`, `engineering` work in `agent-ops-room` | Dial-up / Agents | — | Engineering hours on active training run |
| Research | `research-lab`, `research-floor`, `harness-control-room`, `launch-control` | Dial-up → Upward | Research-speed perks on floors | Research points budgeted to assigned model |
| QA | `safety-lab`, `sandbox-wing`, `containment-wing` | Dial-up / It Writes / Agents | Incident cut (sandbox) | Eval report + red-team tranches |
| Graphics | `labelling-room`, `data-team-room`, `synthetic-data-lab` | Dial-up → Harnesses | — | Data feature quality on assigned model |
| Sound / post | Post-training step (no dedicated room yet) | It Writes+ pipeline | — | Style / alignment pass on assigned model |
| Mocap / hardware | `hw-test-lab`, `robotics-arena`, `device-workshop` placeables | It Sees / Bodies / Harnesses | Sector appeal (robotics) | Device test weeks + robotics appeal |
| Marketing | `sales-office`, `product-studio`, `press-room`, `auditorium` | Dial-up → Chat Boom | Buzz + scandal damping | Launch hype on assigned model |
| Support | `support-desk` | Agents | — | Post-launch support load on assigned model |
| Server / hosting | `server-closet`, `gpu-hall`, `cluster-hall`, `liquid-cooling-pod`, `ai-factory-hall`, `orbital-uplink` | Dial-up → Upward | GPU slots | Compute queued to runs on assigned model |
| HR / comfort | `coffee-corner`, `kitchen`, `nap-pods`, `recruiting-office` | Dial-up → Chat Boom | Morale lift, poach defence | No per-model slot (people benefit globally) |
| Policy / infra | `policy-office`, `ops-centre` | Dial-up | — | World-map and compliance hooks unchanged |
| Chips | `chip-design-lab` | It Sees | — | Chip design projects (one at a time per Q chip rule) |

Press room and auditorium **remain** stateless press rooms (Q145); whiteboard and device workshop fiction moves to **placeables** and typed work rooms above, not orphan room ids.

#### Per-model pipeline synergy (one flagship in flight)

| Phase | Rooms assigned on the model card | Pours into |
| --- | --- | --- |
| Train | Engineering bay, GPU / cluster hall (compute) | Training progress, compute debt |
| Data features | Labelling, data team, synthetic data | Capability / sector features |
| Safety test / report | Safety lab, sandbox, containment | Eval report, incident risk cut |
| Style / post-training pass | Engineering + post-training step | Alignment and polish meters |
| Hype / launch | Sales, product studio, press room, auditorium | Launch buzz, scandal damping on this release |
| Support | Support desk | Ticket load and churn on this model |

Lab-wide perks still stack underneath; per-model slots determine **which** staff time is attributed to the active card.

### 8.4 Staff placement

- Assign **role → room → desk**; unassigned or deskless staff idle or do lab chores only (no research/engineering credit).
- Visible walks to water, coffee, kitchen; pathing smoke is a step-1 gate.
- When per-model UI lands (step 3), the same person can be pinned to a **room skill** on one model (e.g. safety tester on Eval Report).
- Overcrowding beyond soft tiles-per-seat applies morale pressure even if desks exist.

### 8.5 Refuse from the classic template

| Refused mechanic | Why |
| --- | --- |
| Hard corridor doors blocking adjacency | Optional corridors only; no key-and-door puzzling. |
| Auto-furnish that overfills and hides cost | Player confirms spend; preview is optional. |
| Decor arms race without cap | Room rating capped; comfort rooms keep global perks. |
| Mandatory toilet sim v1 | Hygiene folded into morale / rating later if needed. |
| Hidden improvement values | All placeables show cost and rating contribution. |

### 8.6 Build order

| Step | Deliverable | Depends on |
| --- | --- | --- |
| **1** | Visible multi-desk garage rooms, coffee corner, resize / move / delete / copy blueprint stub; seat = desk; overcrowding warning | This design doc |
| **2** | Placeable desks + amenity set above; capped room rating meter | Step 1 |
| **3** | Per-model assignments + Eval Report attribution | Step 2 |
| **4** | Post-training, marketing, support on the same model assignment UI | Step 3 |
| **5** | Dirt, janitors, blueprint library polish, decor tiers reconciled with comfort rooms | Step 4 |

Implementation task breakdown for step 1 lives in the project plan keyed off §8.6 step 1 (ALT-1034); later steps wait on merge gates after each step ships.

---

## Decisions

| Id | Decision |
| --- | --- |
| Q9 | Whiteboard, device-workshop, and similar **non-seat** fiction are **placeable items** and typed work-room behavior, not skipped empty room types. See §8.2–8.3. |

Approved **28 Nov 2026** (GD-6).