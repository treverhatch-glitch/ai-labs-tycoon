# Unity integration handoff (`bc-93b5443a`)

Graphics batch 1 (branch `cursor/graphics-pipeline-5032`) does **not** change play-mode behaviour. Unity thread wires assets into existing views.

**Studio land:** [STUDIO-GRAPHICS-HANDOFF.md](STUDIO-GRAPHICS-HANDOFF.md) (batch 34).

## Index lane (batches 50–63)

| Doc | Purpose |
| --- | --- |
| [PRESENTATION-INDEX-HUB.md](PRESENTATION-INDEX-HUB.md) | Navigation for index/rollup specs |
| [HANDOFF-ROLLUP.md](HANDOFF-ROLLUP.md) | Expected kit counts (30 props, etc.) |
| [GO-PACKET-TEMPLATE.md](GO-PACKET-TEMPLATE.md) | Coordinator push spacing |
| [WORLD-MAP-HOST-INDEX.md](WORLD-MAP-HOST-INDEX.md) | Host shell + panel mock (batches 36, 4) |
| [TAB-RAIL-INTEGRATION-INDEX.md](TAB-RAIL-INTEGRATION-INDEX.md) | Tab rail layers (batches 7, 28, 44) |
| [ROOM-BUILD-INDEX.md](ROOM-BUILD-INDEX.md) | Room build + explainable HUD (batches 38, 20) |

Tokens: `CampusUnityIntegrationHubPresentationSpec` (batch 64); `CampusWorldMapHostIndexPresentationSpec` (batch 66); `CampusTabRailIntegrationIndexPresentationSpec` (batch 67); `CampusRoomBuildIndexPresentationSpec` (batch 68). Gate unchanged: `./tools/art/prepush.sh`.

## Batch 11 (local, unpushed atop `c17c67d`)

- `CampusAccessibilitySpec`, `ACCESSIBILITY-PRESENTATION.md`, Q460

## Batch 10 on branch (`c17c67d`)

## Batch 9 on branch (`9763f7d`)

- `era-skin-tokens.json`, `CampusEraSkinTokens`, `ERA-SKIN-TOKENS.md`

## Batch 8 on branch (`f185b20`)

- `CampusMascotPresentationSpec` → `ModelMascotPresentation.BobAmplitude`
- `MASCOT-PRESENTATION.md`, `GLOW-PRESENTATION.md`, Q457

## Batch 7 on branch (`5a99b2d`)

- `CampusTabIconBindings`, `CampusWorldMapTokens`, `TAB-RAIL-INTEGRATION.md`
- `world-bloc-tints.json` under `Assets/Art/Generated/` (prepush copy)
- 30 OBJ kit (+ trophy pedestal, scoreboard slab)

## Batch 6 on branch (`a4a7d54`)

- `CampusSilhouetteCatalog` — SVG paths for `rival-silhouettes-manifest.json`
- `RIVALS-PANEL.md`, `GAUNTLET-STAGE.md`, `PROP-PLACEMENT-HINTS.md`
- `CampusHudLayoutSpec` rivals split + gauntlet fade constants

## Batch 5 on branch (`fe3c00d`)

- `CampusHudLayoutSpec`, `CampusPropLoader`, panel tokens + research/training specs
- 28 OBJ props, 4 rival silhouettes, extended `RivalPresentationPalette`

## Batch 4

- `CampusPropCatalog` / `CampusIconCatalog` — import paths for all generated assets
- `RivalPresentationPalette.AccentFor(labId)` on rival badges
- `WORLD-MAP-PANEL.md` for bloc tiles + pins
- Pre-push: `./tools/art/prepush.sh`

## Batch 2 (local, unpushed)

- 8 more campus OBJ (research bench, data stack, lectern, bloc tile, etc.) — regenerate `generate_campus_kit.py`
- 8 procedural SVG HUD icons — `generate_ui_icons.py`
- `SCREEN-KITS.md`, `MATERIAL-TOKENS.json`, `CampusUiTokens.cs`

## What landed in batch 1

| Path | Purpose |
| --- | --- |
| `docs/art/*` | Pipeline, style, layouts, register |
| `tools/art/generate_campus_kit.py` | Regenerates OBJ kit |
| `unity/Assets/Art/Generated/**` | Committed OBJ + `manifest.json` |
| `unity/Assets/Scripts/Hq/CampusPresentationSpec.cs` | Camera + layout constants |

## Existing code to extend (do not fork)

| Type | File | Hook |
| --- | --- | --- |
| Grid | `HqGridView.cs` | After `BuildRooms`, instance props from manifest by `RoomPurpose` |
| Room cube | `RoomBlockView.cs` | Swap primitive for `building-module-1x1` scaled to footprint |
| Palette | `HqVisualPalette.cs` | Keep era/purpose colours; materials read spec tokens |
| HUD | `HqHudView.cs` | Align to `UI-HUD-LAYOUTS.md` when restyling |
| CI capture | `HqViewCaptureTests.cs` | Apply `CampusPresentationSpec.ApplyCaptureCamera(cam, lot)` |

## Suggested wiring steps (Unity thread)

1. Import `Assets/Art/Generated/campus-kit/*.obj` (see import table in `ART-PIPELINE.md`).
2. Create `Assets/Art/Materials/CampusFallback.mat` (URP Lit, smoothness 0.2).
3. Add `CampusPropCatalog.cs` (presentation-only) that reads `manifest.json` via `TextAsset` or hard-coded ids matching manifest.
4. In `HqGridView.BuildRooms`, for each room:
   - Scale module: `(tilesW * TileSize, RoomHeight, tilesH * TileSize)`
   - Place `prop-server-rack` when `purpose == Compute`
   - Place `prop-desk` × min(seats, 4) for seated rooms
5. Optional: parent `exterior-quad-tree` along lot border when `lot.Id` is `campus` or larger.

## Camera migration

`HqGridView.CenterCamera` today fits the lot orthographically. Replace with:

```csharp
CampusPresentationSpec.ApplyGameplayCamera(Camera.main, lot.Width, lot.Height);
```

Constants live in `CampusPresentationSpec.cs` so graphics and Unity agree without duplicating magic numbers in docs.

## Tests to keep green

- `HqVisualPaletteTests`
- `HqViewCaptureTests` (artifact `unity/artifacts/hq-view.png`)
- `EraRoomEvolutionTests`, `GlowSpreadMapTests`

## What graphics will *not* do

- No changes to `Rules/` or SimTests in this batch.
- No Unity `.meta` spam for OBJ (editor regenerates on import).
- No push until coordinator GO (branch stays local).