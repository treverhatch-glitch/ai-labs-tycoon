# Tab rail integration (batch 7)

Maps `CampusTabRailMock.TabIds` to HUD SVGs (`CampusIconCatalog`) and layout width `CampusHudLayoutSpec.TabRailWidth`.

| Tab id | Icon id | Panel doc |
| --- | --- | --- |
| `hq` | `icon-hq` | `UI-HUD-LAYOUTS.md` HQ |
| `research` | `icon-research` | `RESEARCH-PANEL.md` |
| `training` | `icon-training` | `TRAINING-PANEL.md` |
| `rivals` | `icon-rivals` | `RIVALS-PANEL.md` |
| `world` | `icon-world` | `WORLD-MAP-PANEL.md` |

**Unity thread:** import SVGs as sprites; assign in tab rail when replacing `CampusTabRailMock` cubes.