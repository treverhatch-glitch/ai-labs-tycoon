# CC0 hero art directive (Hatch, 2026-11-26)

Quality bar, sources, and lane policy for early CC0 art. **Standing merge/push rules:** project store [`standing-rules-merge.md`](https://cursor.com) → read `push-policy-standing.md` before any push; **builders never merge**; Hatch squash only after **visual PASS** + Rules CI on exact PR head.

## Quality bar

Stop primitive-box iteration. Target shipped stylised indie tycoon (Two Point Hospital / Kenney showcase).

## Sources (CC0 only)

- Kenney.nl — furniture, city, characters
- Quaternius — characters, props
- Poly Haven — PBR textures, HDRIs
- ambientCG — textures

Record every pack in `docs/art/LICENSES.md` with pack URL. No other licences.

## Pipeline

1. Import kits into Blender (headless).
2. Restyle: game role-colour palette, bevels, bake 1024 AO + albedo (normals where needed).
3. Combine/adapt into game assets; export glTF +Y up; URP materials.
4. Previews: grey floor / mid-grey studio (`blender_preview_rig`), 3/4 view (~30° elev, ~32° yaw on heroes).

Staff heroes (founder lane): **Kenney `characterMedium` stock** recolor — one mesh body + face-plane eyes (+ optional prop mesh). Legacy packet-2 staff remain cube/bake until **packet 3** upgrades them.

## Hero assets (one visual milestone per PR, then pause)

| PR | Worker | Hero / scope |
|----|--------|----------------|
| #92 | Graphics | Desk |
| #89 | Staff | Engineer staff kit (packet 2) |
| #90 | HUD/env | FOR SALE sign + garage front |
| #88 | Garage | Garage prop set |

## Staff hero stack (2026-11-27 — **FROZEN tips, NO PUSH**)

| PR | Branch | Frozen tip | Role |
|----|--------|------------|------|
| **[#95](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/95)** | `cursor/art-staff-founder-researcher` | `e164f581` | **Parent** — dual Quaternius/modular heroes; **Hatch hard FAIL** PNG silhouette gates |
| **[#98](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/98)** | `cursor/founder-upright-silhouette-f171` | `31d8c951` | Founder Kenney stock — **VISUAL PASS** (frozen) |
| **[#100](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/100)** | `cursor/researcher-hero-kenney-f171` | `1b8578f0` | Researcher Kenney stock — v2 local; **frozen** pending Hatch visual |

**Disambiguation:** rules roadmap item **#95 Promotions** (Q337) is **not** PR #95.

### Parent [#95](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/95) hard FAIL — LOCAL fix plan (no push)

Hatch authority: **alpha-mask silhouette** gates (`check_cc0_staff_preview_upright.py`, shared `preview_quality_lib` extent/edge rules) fail on parent previews at `e164f581` while stacked children fixed stock path on separate PRs.

**Before next grouped push on #95:**

1. **Cherry-pick / replay** Kenney stock pipeline from #98/#100 onto parent branch (founder + researcher `run_blender_cc0_staff_*.sh`, shared `staff-founder-kenney-stock-kit.glb` sha `b09c59ac…` unchanged).
2. Re-render **all** hero preview triplets: `{id}.png`, `{id}.mask.png`, `{id}.frame.json`.
3. `./tools/art/run_blender_cc0_staff_heroes.sh` green; `prepush.sh` green on exact head; noreply author; **no** `Previews:` trailer paragraph in commit bodies.
4. Do **not** force-push; new commits on top only.

### Eye gate bar (heroes — port to parent when unfreezing #95)

Shared library: `tools/art/staff_preview_eye_head_gate.py`.

| Rule | Detail |
|------|--------|
| Search region | Head bbox = **top 30%** of mask figure span; face band **8–26%** span (not torso/crotch) |
| Blob count | Two dark blobs in band **or** (idle 3/4 only) one blob + Blender `preview_eyes_visible=2` + GLB world eye gate — **no** PNG-only soft-pass when GLB passes alone |
| Negatives | Historical PNGs with eye outside head must **FAIL** |
| Debug | `staff-*-eye-gate-debug.png` circles on **face** blobs only |
| Exposure | Clip gate: `<5%` masked pixels at `(255,255,255)` on idle + vs-stock |

**LOCAL without push:** keep eye-gate tests aligned on frozen branches; fold into #95 parent only at coordinator **GO**.

### Stacked squash prep (Hatch-only merge)

1. Parent #95 lands Kenney heroes + gates (after hard FAIL cleared).
2. Close/stack #98 and #100 as **already superseded** child work or squash sequence per Hatch — **agents do not merge**.
3. Packet 2 **#89** stays separate (cube kit + wisp); do not mix stock sha or founder blend without coordinator note.

## Staff packet 3 (bible-backed design — LOCAL)

**Goal:** Raise **three** early-role idle heroes from packet-2 cubes to Kenney stock recolors (same rig/gates as founder/researcher). One stacked PR per role after parent #95 is green; **design only until GO**.

See [art/staff-packet3-hero-design.md](art/staff-packet3-hero-design.md).

| Asset id | Rules role | Bible / perks hook |
|----------|------------|-------------------|
| `staff-recruiter-idle` | `recruiter` | ch. 7 talent war; `recruiter-fee-discount` ([role-perks.md](content/role-perks.md)) |
| `staff-safety-idle` | `red-teamer` / safety | ch. 10 listeners; `red-team-catch` |
| `staff-gpu-tech-idle` | `infra` / engineers | ch. 3 compute; GPU rack culture (no perk row — **silhouette** = cables, vest, boots) |

## New lanes (2026-11-26)

| Branch | Worker | Hero |
|--------|--------|------|
| `cursor/art-garage-interior-cc0` | bc-f93fae11 | Garage interior |
| `cursor/art-compute-hardware-cc0` | bc-e570c1d0 | Server rack + PC + monitor |
| `cursor/art-urp-garage-scene` | bc-e91acd7d | URP volume, HDRI, camera |
| `cursor/art-staff-founder-researcher` | bc-70d7a92f | Founder + researcher (#95 stack) |

## Standing rules

- No force-push; no Co-authored-by on **new** agent commits; no tracked `.pyc`.
- Rules CI green on exact head before Hatch squash.
- **Composer-2.5 / Auto only** on art lanes unless coordinator names another model.
- Grouped pushes only on coordinator **GO**; Hatch visual where the lane requires it.
- In art lanes, disable Cursor **co-author on merge commits** where the UI allows; squash-merge trailers on `main` are Hatch-owned — agents do not rewrite history to strip them.