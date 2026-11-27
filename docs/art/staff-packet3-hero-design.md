# Staff packet 3 — Kenney hero upgrades (design)

**Status:** LOCAL design only — **no push**, no manifest/GLB edits until parent **[PR #95](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/95)** silhouette FAIL is cleared and coordinator **GO**.  
**Depends on:** frozen stack [#98](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/98) `31d8c951`, [#100](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/100) `1b8578f0` (eye gate + stock recolor patterns).

## Why packet 3

[Early art packet 2](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/89) shipped **cube/bake** idles for the early roster. Founder and researcher proved the **Kenney stock** lane (recolor slots, face eyes, glasses/props, five preview PNGs, `run_blender_cc0_staff_*.sh`). Packet 3 replaces the **three most gameplay-visible** early specialists so hiring screens read role at a glance (bible ch. 7 “you can tell who does what”).

## Heroes (one PR each when building)

### 1. `staff-recruiter-idle`

| Field | Spec |
|-------|------|
| Rules id | `recruiter` — Talent Recruiter |
| Perk | `recruiter-fee-discount` — hire fee relief in talent-war eras |
| Silhouette | Open blazer or smart jacket over **warm** shirt (amber/coral, **not** founder hoodie pink); clipboard or phone prop on weak hand |
| Face | Kenney eyes; optional subtle smile line; no glasses required |
| Distinct vs founder | Jacket length shorter, no jeans; prop + warmer top |
| Distinct vs researcher | No lab coat; warmer palette, props are HR not science |

### 2. `staff-safety-idle` (red-team / alignment listener)

| Field | Spec |
|-------|------|
| Rules id | `red-teamer` primary art read; also reads as generic **safety** early hire |
| Perk | `red-team-catch` — second roll before incidents |
| Silhouette | Dark zip fleece or utility vest, **muted** trousers, boots; optional chest badge or lanyard (flat mesh, not floating cube) |
| Face | Kenney eyes; slightly heavier brows; **no** lab coat |
| Palette | Charcoal + safety orange **accent** (single stripe or badge — not full hi-vis) |
| Distinct vs gpu-tech | No cables/tool belt; upper body bulkier, darker |

### 3. `staff-gpu-tech-idle`

| Field | Spec |
|-------|------|
| Rules id | `infra` / engineer bench GPU culture (ch. 3 compute split) |
| Perk | none in `rolePerks` — sell the role with **gear** |
| Silhouette | Rolled sleeves, **teal or grey** work shirt, dark pants, **tool belt or coiled cable** prop at hip |
| Face | Kenney eyes; optional safety glasses mesh (shared glasses spawn pattern from researcher) |
| Distinct vs researcher | No lab coat; hands-busy prop; cooler/grey shirt not teal undershirt |

## Shared technical bar (match #98 / #100)

- Stock kit sha256 fixed: `b09c59ac3c3cb0883df8e9200feeb946f0b7e1c37a2d7d67b3eb1e5eb7a6bd36`
- Idle pose: arms down, hands **mid-thigh** (shared `staff-founder-kenney-stock.blend` bake)
- GLB: body + `FounderEye_L` + `FounderEye_R` + **one** prop mesh max (recruiter clipboard, safety badge block, gpu cable)
- Previews (five): idle, vs-stock-check (left kit panel), face-diagnostic 0°, axis-sheet, eye-gate-debug
- Gates: head-bbox eye PNG + GLB world eyes; clip `<5%` white pixels; material diversity; connectivity 4 meshes

## Suggested branch / PR stack (after #95 GO)

| Order | Branch suffix | Asset |
|-------|---------------|-------|
| 3a | `cursor/recruiter-hero-kenney-f171` | recruiter |
| 3b | `cursor/safety-hero-kenney-f171` | safety |
| 3c | `cursor/gpu-tech-hero-kenney-f171` | gpu-tech |

Base each branch on post-fix `cursor/art-staff-founder-researcher` (or `main` after Hatch squash sequence). **Do not** push until grouped **GO** + Rules CI + Hatch visual per hero.

## Out of scope for packet 3

- `staff-sales-idle`, `staff-engineer-work`, `staff-pose-typing` — packet 4+ or pose variants later
- Unity screens / wisp mood — packet 2 + #89 lane
- Rules/data role perks changes — art only