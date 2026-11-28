# GD-6 Step 1: General dev room + typed workstations

**Scope:** Mechanics + Unity garage scene only. Steps 2–5 wait on merge gates per [rooms-rework.md](rooms-rework.md) §8.6.

**Art (when step 1 touches props):** CC0 kits, headless Blender, Two Point Hospital read, previews on `#969696`, GLB import blank scene + assembly checks (#88 lesson). Staff walk pivots/joints must stay correct.

**Prerequisites:** Design doc PR merged (or branch aligned). Do not land step 1 ahead of #88 pass12 or #103 CI fix unless those remain on other branches.

---

## Step 1 outcome

In **Mum's Garage**, the player sees:

- **One `general-dev-room`** (suggested **5×5 or 6×5**)—not default-suggested separate research lab + engineering bay.
- **Several room-typed basic workstations** inside that room: train bench, label table, eval desk, polish station (rules ids in [rooms-rework.md](rooms-rework.md) §8.2); enough placed seats for **several** staff at once.
- A small **coffee corner** comfort room.
- **Seats from placed workstations** (not area formula); staff without a workstation cannot contribute slice work.
- **Tiles-per-seat** as **soft cap** with **overcrowding warning** in build UI.
- **Resize, move, delete** rooms; **copy room / blueprint** (v1: footprint + type + workstation layout stub).
- **Pathing** smoke: workstation ↔ coffee corner.

**Not in step 1:** specialist slice takeover on new models (step 3), amenity rating meter (step 2), per-model contribution picker (step 3).

---

## Gated tasks (one PR sub-slice each, each with tests)

| ID | Task | Tests / gate | Depends |
| --- | --- | --- | --- |
| **R1** | **Rules hooks:** `general-dev-room` id; workstation entities per §8.2; seat count = placed workstation count; soft tiles-per-seat + overcrowding flag; document in `workplace.md` stub. | Schema/config test: general room type exists; seat count follows workstations; overcrowding flag when over soft cap. | [rooms-rework.md](rooms-rework.md) |
| **R2** | **Default garage template:** new game spawns one general dev room (5×5 or 6×5) with **starter workstation bundle** + coffee corner; no dual research/engineering default suggestion. | Snapshot test: room count, types, dimensions, workstation ids on starter save. | R1 |
| **R3** | **Workstation placement (rules):** place/move/remove typed workstation on room tile; assign staff; unassigned or deskless staff cannot contribute. | Sim: N workstations ⇒ N workers max; wrong room type refuses workstation id; remove workstation ⇒ seat lost. | R1 |
| **R4** | **Workstation placement (Unity):** place/move/remove in garage scene; sync to save payload for rules tests. | PlayMode or editor test: workstation count and ids match save. | R3 |
| **R5** | **Overcrowding UX:** build panel soft cap + warning; morale hook stub (flag only). | UI golden string; sim overcrowding flag. | R3 |
| **R6** | **Room resize / move / delete:** delete clears workstations; resize blocks if workstations out of bounds; move preserves assignments when legal. | Sim tests per operation. | R4 |
| **R7** | **Copy room / blueprint:** duplicate general room footprint + workstation layout to empty tiles; show cost. | Sim: copy creates second room; cost deducted; workstations copied in bounds. | R6 |
| **R8** | **Staff pathing smoke:** walk workstation ↔ coffee corner; founder/researcher pivot sanity (#88 constraints). | Visual checklist + minimal PlayMode walk test. | R4 |

**Merge order:** R1 → R2 → R3 → (R4 ∥ R5) → R6 → R7 → R8.

**Out of step 1:** specialist supersession (step 3), amenities (step 2), contribution lists (step 3), dirt/janitors (step 5).

---

## Step 1 implementation lanes (after design PR)

| Lane | Owner | Notes |
| --- | --- | --- |
| Rules / sim | New worker when R1 merges | Small commits; `list-tests` bar. |
| Unity garage | Art-capable worker | Distinct workstation meshes for general-room archetypes; shares #88 rig lessons. |
| Coordinator | Tracker | One active implementation thread until step 1 gate GREEN. |

---

## Open questions for Hatch (non-blocking)

- Exact general room footprint: 5×5 vs 6×5 on first frame?
- Starter bundle: minimum count per workstation archetype in general room?
- Copy-room v1: always include workstation layout, or footprint-only option?