# Room build panel (presentation)

NEXT Unity item 3 tail: `RoomBuildPanel` layout on HQ stage (`UI-HUD-LAYOUTS.md`).

| Zone | Fraction | Binding |
| --- | --- | --- |
| Lot viewport | 70% width | `HqGridView` + `CampusPropPlacementHints` |
| Build panel | right 28% × top 40% | `RoomBuildPanel` / `RoomBuildController` ghost |

Tokens: `CampusRoomBuildPresentationSpec` (`icon-hq`, 70/28/40 fractions).

Props per purpose: [PROP-PLACEMENT-HINTS.md](PROP-PLACEMENT-HINTS.md). No build rules in graphics batches.