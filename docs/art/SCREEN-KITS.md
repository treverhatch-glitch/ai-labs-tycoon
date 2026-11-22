# Screen kits (batch 2)

Prefab-less prop **groups** for Unity to instance when each screen opens. All paths are under `unity/Assets/Art/Generated/`.

## HQ office kit (`campus-kit/`)

| Prop id | When |
| --- | --- |
| `building-module-1x1` | Every room block |
| `prop-desk`, `prop-chair` | Seated rooms |
| `prop-server-rack`, `prop-gpu-blade` | Compute rooms |
| `prop-whiteboard` | Research / policy |
| `prop-monitor-wall` | `ops-centre`, `gpu-hall` |
| `prop-potted-plant`, `exterior-quad-tree`, `exterior-bike-rack` | `campus`+ lots |
| `prop-coffee-cart` | `campus`, `megacampus` borders |

## Research kit

| Prop id | Placement |
| --- | --- |
| `prop-research-bench` | Centre of `research-lab` / `research-floor` |
| `prop-whiteboard` | Back wall |
| `prop-potted-plant` | Corner accent |

**UI (future):** import `ui-icons/icon-research.svg` on tree panel header.

## Training kit

| Prop id | Placement |
| --- | --- |
| `prop-data-stack` | `data-team-room`, labelling rooms |
| `prop-server-rack` | Near compute split UI metaphor |
| `prop-safety-cone` | When incident rung ≥ 2 (tint `ui-danger`) |

**UI:** `icon-training.svg` on `TrainingRunPanel` title.

## Rivals kit

| Prop id | Placement |
| --- | --- |
| `ui-rival-badge` | One per lab row (palette from rival id hash) |
| `prop-gauntlet-lectern` | Gauntlet overlay only |
| `prop-trophy-pedestal`, `prop-scoreboard-slab` | Gauntlet overlay flanks |

**UI:** `icon-rivals.svg` via `CampusTabIconBindings.IconPathForTab("rivals")`.

## World map kit

| Prop id | Placement |
| --- | --- |
| `ui-bloc-tile` | Bloc background cells (scale 2–4×, tint `world-bloc-tints.json`) |
| `ui-world-map-pin` | Site markers |
| `exterior-sign-pole` | Selected site preview |

**UI:** `icon-world.svg` on map tab; power breakdown uses `icon-compute.svg`.

## Global HUD icons (`ui-icons/`)

| Icon | HUD use |
| --- | --- |
| `icon-cash` | Cash row (+ tap for breakdown) |
| `icon-calendar` | Date / quarter |
| `icon-hq` | Future left-rail HQ tab |
| `icon-research`, `icon-training`, `icon-rivals`, `icon-world` | Future tab rail |
| `ui-tab-rail-plaque` | 3D mock of rail button behind SVG (optional) |

Import SVG as `Sprite (2D and UI)` with pixels per unit 64.

## Material polish (Unity thread)

See `MATERIAL-TOKENS.json` and `CampusUiTokens.cs` for shared colours. Target smoothness 0.15–0.25 on rooms, 0.05 on floor plate.