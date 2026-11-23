# World map freeze overlay (presentation)

When a bloc is frozen (rules), art shows:

| Element | Asset |
| --- | --- |
| Freeze badge | `icon-danger` + desaturated `ui-bloc-tile.obj` |
| Pin | `ui-world-map-pin.obj` at 70% alpha |

Index: [SEASONAL-OVERLAY-INDEX.md](SEASONAL-OVERLAY-INDEX.md) (batch 54). Tokens: `CampusWorldMapFreezePresentationSpec` (pin alpha 0.7, bloc desaturate 0.45, `icon-danger`).

Tint source remains `world-bloc-tints.json`; do not encode freeze state in JSON (Unity reads rules). See `WORLD-MAP-PANEL.md` interaction section.