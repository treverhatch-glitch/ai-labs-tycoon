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
| [HQ-STAGE-STACK-ROLLUP.md](HQ-STAGE-STACK-ROLLUP.md) | HQ bands 38–42 rollup (batch 69) |
| [STAFF-GPU-PANEL-INDEX.md](STAFF-GPU-PANEL-INDEX.md) | Staff/GPU HQ band (batches 26, 39) |
| [TRAINING-PEEK-INDEX.md](TRAINING-PEEK-INDEX.md) | Training peek + screen (batches 40, 49) |
| [HQ-MASCOT-STRIP-INDEX.md](HQ-MASCOT-STRIP-INDEX.md) | Mascot strip band (batches 41, 33) |
| [COMPUTE-SPLIT-INDEX.md](COMPUTE-SPLIT-INDEX.md) | Compute split (batches 32, 40) |
| [HQ-STAGE-STACK-COMPLETION.md](HQ-STAGE-STACK-COMPLETION.md) | HQ band indexes complete (batch 74) |
| [INDEX-LANE-MANIFEST.md](INDEX-LANE-MANIFEST.md) | Post-60 index lane manifest (batch 75) |
| [ALT-76-ROLLUP-REFRESH.md](ALT-76-ROLLUP-REFRESH.md) | ALT-76 through batch 75 (batch 76) |
| [GO-PACKET-INDEX-LANE-REFRESH.md](GO-PACKET-INDEX-LANE-REFRESH.md) | GO packets for index 66+ (batch 77) |
| [HQ-LOT-VIEWPORT-INDEX.md](HQ-LOT-VIEWPORT-INDEX.md) | HQ lot viewport (batch 78) |
| [INDEX-LANE-MANIFEST-TAIL.md](INDEX-LANE-MANIFEST-TAIL.md) | Manifest tail 75–78 (batch 79) |
| [HUD-ERA-TAIL-INDEX.md](HUD-ERA-TAIL-INDEX.md) | Era HUD tail (batch 80) |
| [RIVAL-WORLD-TAIL-INDEX.md](RIVAL-WORLD-TAIL-INDEX.md) | Rival/world tail (batch 81) |
| [EXPLAINABLE-HUD-TAIL-INDEX.md](EXPLAINABLE-HUD-TAIL-INDEX.md) | Explainable HUD tail (batch 82) |
| [ALT-76-STATUS-SYNC.md](ALT-76-STATUS-SYNC.md) | ALT STATUS sync (batch 83) |
| [GO-PACKET-TAIL-REFRESH.md](GO-PACKET-TAIL-REFRESH.md) | GO packet tail (batch 84) |
| [PRESENTATION-INDEX-HUB-TAIL.md](PRESENTATION-INDEX-HUB-TAIL.md) | Hub tail (batch 85) |
| [ALT-76-ROLLUP-REFRESH-85.md](ALT-76-ROLLUP-REFRESH-85.md) | ALT refresh 85 (batch 86) |
| [SCREEN-LAYOUT-TAIL-INDEX.md](SCREEN-LAYOUT-TAIL-INDEX.md) | Screen layout tail (batch 87) |
| [ALT-76-STATUS-SYNC-86.md](ALT-76-STATUS-SYNC-86.md) | STATUS sync 86 (batch 88) |
| [CAPTURE-HUD-TAIL-INDEX.md](CAPTURE-HUD-TAIL-INDEX.md) | Capture HUD tail (batch 89) |
| [GO-PACKET-TAIL-REFRESH-88.md](GO-PACKET-TAIL-REFRESH-88.md) | GO packet through 88 (batch 90) |
| [ALT-76-STATUS-SYNC-89.md](ALT-76-STATUS-SYNC-89.md) | STATUS sync 89 (batch 91) |
| [ALT-76-ROLLUP-REFRESH-89.md](ALT-76-ROLLUP-REFRESH-89.md) | ALT rollup 89 (batch 92) |
| [GO-PACKET-TAIL-REFRESH-90.md](GO-PACKET-TAIL-REFRESH-90.md) | GO packet 90 (batch 93 local) |
| [PRESENTATION-TAIL-ROLLUP.md](PRESENTATION-TAIL-ROLLUP.md) | Tail rollup (batch 94 local) |
| [STAFF-PRESENTATION-TAIL-INDEX.md](STAFF-PRESENTATION-TAIL-INDEX.md) | Staff tail (batch 95) |
| [TRAINING-PEEK-TAIL-INDEX.md](TRAINING-PEEK-TAIL-INDEX.md) | Training tail (batch 96 local) |
| [HQ-MASCOT-STRIP-TAIL-INDEX.md](HQ-MASCOT-STRIP-TAIL-INDEX.md) | Mascot strip tail (batch 97 local) |
| [COMPUTE-SPLIT-TAIL-INDEX.md](COMPUTE-SPLIT-TAIL-INDEX.md) | Compute split tail (batch 98) |
| [HQ-STAGE-STACK-COMPLETION-TAIL-INDEX.md](HQ-STAGE-STACK-COMPLETION-TAIL-INDEX.md) | HQ completion tail (batch 99 local) |
| [GO-PACKET-TAIL-REFRESH-98.md](GO-PACKET-TAIL-REFRESH-98.md) | GO packet 98 (batch 100 local) |

Squash: [GROUPED-PUSH-92-95.md](GROUPED-PUSH-92-95.md) (`9b89640`), [GROUPED-PUSH-96-NEXT.md](GROUPED-PUSH-96-NEXT.md) (`81f640a`); next [GROUPED-PUSH-99-NEXT.md](GROUPED-PUSH-99-NEXT.md).

Tokens: `CampusUnityIntegrationHubPresentationSpec` (batch 64); `CampusWorldMapHostIndexPresentationSpec` (batch 66); `CampusTabRailIntegrationIndexPresentationSpec` (batch 67); `CampusRoomBuildIndexPresentationSpec` (batch 68); `CampusHqStageStackRollupPresentationSpec` (batch 69); `CampusStaffGpuPanelIndexPresentationSpec` (batch 70); `CampusTrainingPeekIndexPresentationSpec` (batch 71); `CampusHqMascotStripIndexPresentationSpec` (batch 72); `CampusComputeSplitIndexPresentationSpec` (batch 73); `CampusHqStageStackCompletionPresentationSpec` (batch 74); `CampusIndexLaneManifestPresentationSpec` (batch 75); `CampusAlt76RollupRefreshPresentationSpec` (batch 76); `CampusGoPacketIndexLaneRefreshPresentationSpec` (batch 77); `CampusHqLotViewportIndexPresentationSpec` (batch 78); `CampusIndexLaneManifestTailPresentationSpec` (batch 79); `CampusHudEraTailIndexPresentationSpec` (batch 80); `CampusRivalWorldTailIndexPresentationSpec` (batch 81); `CampusExplainableHudTailIndexPresentationSpec` (batch 82); `CampusAlt76StatusSyncPresentationSpec` (batch 83); `CampusGoPacketTailRefreshPresentationSpec` (batch 84); `CampusPresentationIndexHubTailPresentationSpec` (batch 85); `CampusAlt76RollupRefreshB85PresentationSpec` (batch 86); `CampusScreenLayoutTailIndexPresentationSpec` (batch 87); `CampusAlt76StatusSyncThrough86PresentationSpec` (batch 88); `CampusCaptureHudTailIndexPresentationSpec` (batch 89); `CampusGoPacketTailRefreshThrough88PresentationSpec` (batch 90); `CampusAlt76StatusSyncThrough89PresentationSpec` (batch 91); `CampusAlt76RollupRefreshB89PresentationSpec` (batch 92); `CampusGoPacketTailRefreshThrough90PresentationSpec` (batch 93); `CampusPresentationTailRollupPresentationSpec` (batch 94); `CampusStaffPresentationTailIndexPresentationSpec` (batch 95); `CampusTrainingPeekTailIndexPresentationSpec` (batch 96 local); `CampusHqMascotStripTailIndexPresentationSpec` (batch 97 local); `CampusComputeSplitTailIndexPresentationSpec` (batch 98); `CampusHqStageStackCompletionTailIndexPresentationSpec` (batch 99 local); `CampusGoPacketTailRefreshThrough98PresentationSpec` (batch 100 local). Gate unchanged: `./tools/art/prepush.sh`.

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