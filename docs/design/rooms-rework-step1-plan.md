# GD-6 build step 1 — empty garage + draw-room

**Scope:** Mechanics + Unity garage scene only. Later steps: [rooms-rework.md](rooms-rework.md) §12. Do not start step 2 until step 1 merges.

**Prerequisites:** This design doc on #106. Do not race #88 visual / #103 CI unless branches stay separate.

**Art (when props land):** CC0 kits, headless Blender, previews on `#969696`, GLB blank-scene import (#88 rig lessons).

---

## Step 1 outcome ([rooms-rework.md](rooms-rework.md) §12 step 1)

- New game on `garage`: **zero rooms**, **3 clutter placeables** (sell once for cash, free tiles).
- **Draw-room tool:** rectangle + paint; 3×3 staffed / 2×2 comfort minimum; live cost, seat estimate, overcrowding line; resize / move / delete (50% refund) / copy.
- Build menu: exactly **4** types — `general-dev-room`, `research-lab`, `coffee-corner`, `bathroom`; all others greyed with unlock text.
- Player draws rooms, places **tier-1 Dev desks** in general room, **seats founder** by drag.
- **No pre-spawned rooms.**

---

## Gated tasks (one PR slice each, with tests)

| ID | Task | Tests / gate | Depends |
| --- | --- | --- | --- |
| **R1** | Empty lot + clutter: $60k start; sell tower → $69k; clutter tiles clear. | New-game snapshot: 0 rooms, clutter ids, cash. | Design doc |
| **R2** | Draw-room tool: paint/rect, min sizes, live cost + seat + overcrowding readouts; door tile rule. | Build refused below minimum; cost matches design doc tuning table. | R1 |
| **R3** | Four-type build menu; greyed entries show unlock hint. | Only four ids buildable at 1995 start. | R2 |
| **R4** | Dev desk place/remove; seat = placed matching desk; unseated worker produces nothing. | N desks ⇒ N seats max; wrong desk type refused. | R2 |
| **R5** | Resize / move / delete (50% refund) / copy blueprint. | Sim per op; save/load keeps geometry + desks + seats. | R4 |
| **R6** | Unity garage sync: clutter, drawn rooms, desks, seats in save payload. | PlayMode or editor: counts match rules tests. | R5 |
| **R7** | Pathing smoke: seat ↔ coffee corner / bathroom door; founder rig pivots (#88). | Visual checklist + minimal walk test. | R6 |

**Merge order:** R1 → R2 → R3 → R4 → (R5 ∥ R6) → R7.

**Out of step 1:** task picker, gigs, models, linking, packages, staff loop.

---

## Acceptance (from design §12 step 1)

- Test: 1995 game asserts 0 rooms and $60k; selling tower → $69k; 4×4 Dev room charges tiles × price; 2×2 Dev refused; worker without matching desk produces nothing; save/load keeps rooms, desks, seated staff.