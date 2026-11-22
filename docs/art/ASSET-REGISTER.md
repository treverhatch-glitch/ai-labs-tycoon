# Asset register — placeholder → final

Status key: **PH** placeholder (procedural / greybox), **SK** skin pass, **FN** final v1 shippable.

Scale: 1 Unity unit = 1 meter. Tile = `0.45m` (`HqGridLayout.TileSize`).

## HQ office (garage → megacampus)

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `building-module-1x1` | HQ | ✓ | | | Walls for cutaway module; stacks to room rects |
| `prop-desk` | HQ | ✓ | | | Per seat in research/engineering rooms |
| `prop-chair` | HQ | ✓ | | | Paired with desk |
| `prop-whiteboard` | HQ | ✓ | | | Research / planning rooms |
| `prop-server-rack` | HQ | ✓ | | | Server closet, GPU halls |
| `prop-gpu-blade` | HQ | ✓ | | | Rack detail instances |
| `prop-potted-plant` | HQ | ✓ | | | Campus / sales biomes |
| `prop-coffee-cart` | HQ | ✓ | | | Late-campus lots only (`campus`, `megacampus`) |
| `exterior-quad-tree` | HQ | ✓ | | | Lot border for `campus`+ |
| `exterior-sign-pole` | HQ | ✓ | | | Parody lab nameplate |
| `mat-lot-plate` | HQ | PH cube | SK | FN | URP Lit; era floor tint |
| `mat-room-block` | HQ | PH cube | SK | FN | Purpose colour + emission |
| `tex-glow-noise` | HQ | null | SK | FN | Soft scrolling emission mask |
| `vfx-training-pulse` | HQ | null | SK | FN | Optional ParticleSystem on compute |

## Research screen

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `ui-research-tree-panel` | Research | code UI | SK | FN | Tech cards; reuse `ui-paper` |
| `ui-research-lead-chip` | Research | label | SK | FN | Active lead badge |
| `icon-research-lab` | Research | colour dot | SK | FN | Maps `research-lab` room id |
| `prop-microscope-block` | Research | | SK | FN | Optional; can instance whiteboard |

## Training screen

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `ui-training-run-panel` | Training | code (`TrainingRunPanel`) | SK | FN | Run queue + bars |
| `ui-model-mascot-sprites` | Training | mood colour | SK | FN | `ModelMascotView` |
| `icon-data-source` | Training | text | SK | FN | License / scrape / synthetic |
| `vfx-epoch-tick` | Training | null | SK | FN | Subtle bar flash on week step |

## Rivals screen

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `ui-rival-row` | Rivals | text list | SK | FN | Feeling bar + log snippet |
| `ui-rival-badge` | Rivals | ✓ | SK | FN | `ui-rival-badge.obj` + palette |
| `tex-rival-silhouette-*` | Rivals | null | SK | FN | 8 parody labs, abstract shapes only |
| `ui-decision-log-scroll` | Rivals | monospace | SK | FN | 60 lines, oldest dropped |

## World map screen

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `ui-world-map-canvas` | World | PH plane | SK | FN | Bloc colours from `world.md` |
| `ui-world-map-pin` | World | ✓ | SK | FN | Site markers |
| `icon-bloc-us` … | World | colour | SK | FN | Real country labels, fictional agencies |
| `mesh-site-hall-preview` | World | box | SK | FN | Holding hall extrusion on select |
| `ui-power-bill-card` | World | HUD panel | SK | FN | Ties to `DisplayQueries` breakdown |

## Cross-cutting UI

| Id | Screen | PH | SK | FN | Notes |
| --- | --- | --- | --- | --- | --- |
| `ui-hud-topbar` | All | ✓ (`HqHudView`) | SK | FN | Cash, date, era |
| `ui-advance-week` | All | ✓ | SK | FN | Bottom-right CTA |
| `ui-room-build-panel` | HQ | ✓ (`RoomBuildPanel`) | SK | FN | |
| `ui-staff-gpu-panel` | HQ | ✓ | SK | FN | |
| `ui-gauntlet-panel` | HQ | ✓ | SK | FN | Seasonal overlay |

## Generated kit (batch 1 — committed on PR #41)

Paths under `unity/Assets/Art/Generated/campus-kit/`:

`prop-desk`, `prop-chair`, `prop-server-rack`, `prop-potted-plant`, `prop-whiteboard`, `building-module-1x1`, `exterior-quad-tree`, `exterior-sign-pole`, `ui-world-map-pin`, `ui-rival-badge`, `prop-gpu-blade`, `prop-coffee-cart`.

Manifest: `unity/Assets/Art/Generated/manifest.json`.

## Generated kit (batch 2 — local, HOLD push until ≥11:01 UTC)

**+8 OBJ:** `prop-research-bench`, `prop-data-stack`, `prop-gauntlet-lectern`, `ui-bloc-tile`, `prop-safety-cone`, `prop-monitor-wall`, `exterior-bike-rack`, `ui-tab-rail-plaque`.

**+8 SVG:** `unity/Assets/Art/Generated/ui-icons/` + `ui-icons-manifest.json`.

**Code:** `CampusUiTokens`, `CampusPropIds`, HUD factories wired to tokens; `SCREEN-KITS.md`, `POLISH-CHECKLIST.md`.

## Batch 3 (local, awaiting GO atop `3e8987a`)

**+5 OBJ:** `prop-lounge-sofa`, `exterior-fence-segment`, `exterior-solar-shade`, `prop-water-cooler`, `building-elevator-core` (**25** total).

**Code:** campus camera in grid + CI capture; `CampusTabRailMock`; `validate_unity_meta_guids.py`; `world-bloc-tints.json`; `MATERIAL-CampusFallback.md`.

## Batch 5 on branch (`fe3c00d`)

**+3 OBJ** → **28** total; `CampusHudLayoutSpec`, `CampusPropLoader`, `SCREEN-PANEL-TOKENS.json`; research/training panel docs; 4 rival silhouette SVGs; extended rival palette; layout/loader tests.

## Batch 6 (local, awaiting GO ≥12:59 UTC atop `fe3c00d`)

**Code:** `CampusSilhouetteCatalog`, rivals/gauntlet HUD fractions, silhouette tests; **docs:** rivals + gauntlet + prop placement; **Q455** gauntlet overlay decision.