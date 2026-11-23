# World map panel (mock spec)

Presentation overlay using `world-bloc-tints.json` and campus kit props.

## Layout (see `UI-HUD-LAYOUTS.md` world section)

| Layer | Asset / token |
| --- | --- |
| Bloc cells | `ui-bloc-tile.obj` scaled 2×, tinted from `world-bloc-tints.json` |
| Site pin | `ui-world-map-pin.obj` |
| Selected site preview | `exterior-sign-pole`, optional `building-elevator-core` for megasites |
| HUD icon | `icon-world.svg` |

Index: [WORLD-MAP-HOST-INDEX.md](WORLD-MAP-HOST-INDEX.md) (batch 66). Host shell: [WORLD-MAP-HOST-PRESENTATION.md](WORLD-MAP-HOST-PRESENTATION.md).

## Interaction (Unity thread)

- Click pin → right panel shows `DisplayQueries` power breakdown (rules); frozen blocs use [WORLD-MAP-FREEZE-PRESENTATION.md](WORLD-MAP-FREEZE-PRESENTATION.md).
- No satellite imagery; country names from rules config only (Q16, Q454).

## Rival accents on map

When showing a rival site holding, tint plaque with `RivalPresentationPalette.AccentFor(labId)` and `ui-rival-badge.obj`.