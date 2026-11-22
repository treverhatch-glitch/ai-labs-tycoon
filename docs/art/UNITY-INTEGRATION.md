# Unity integration handoff (`bc-93b5443a`)

Graphics batch 1 (branch `cursor/graphics-pipeline-5032`) does **not** change play-mode behaviour. Unity thread wires assets into existing views.

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