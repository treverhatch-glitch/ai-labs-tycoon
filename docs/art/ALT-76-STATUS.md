# ALT-76 graphics status (PR #41)

| Batch | Remote tip (examples) | Theme |
| --- | --- | --- |
| 5 | `fe3c00d` | HUD layout, props 28, rival silhouettes |
| 6–7 | `a4a7d54`, `5a99b2d` | Rivals/gauntlet, tab rail, world map |
| 8–9 | `f185b20`, `9763f7d` | Mascot/glow, era skins |
| 10–11 | `c17c67d`, `eb67bf8` | Materials, accessibility |
| 12 | `2deaeed` | CI capture contract |
| 13–14 | `f8123f5`, `bdd9427` | Rollup, staff placeholders |
| 15–17 | `1160d80`…`e11a146` | HUD icons, catalog/icon handoff |
| 18 | `879fb9c` | Silhouette sync + handoff index |
| 19–22 | `f0478d4`…`a2999d9` | Handoff verify, breakdown/rival/world-freeze presentation specs |
| 23 | `5af83e0` | Training incident rings, NEXT refill |
| 24 | `68dbbb0` | Gauntlet stage HUD |
| 25 | `a8517af` | Research breakdown drawer |
| 26 | `e2cf84c` | Staff roster rows |
| 27 | `a0dc2a5` | HQ cash readout |
| 28 | `bdc426a` | Tab rail active state |
| 29 | `d8ad54d` | Era skin preview |
| 30 | `33050c2` | Focus ring / hit targets |
| 31 | `58babaa` | HUD date/week row |
| 32 | `46bfc78` | Compute split bars |
| 33 | `843705d` | Mascot row HUD |
| 34 | `e9ccf12` | Studio handoff |
| 35 | (local GO) | Capture HUD snapshot |

**Unity thread:** import kit, wire `CampusPropPlacementHints`, tab sprites, materials per `UNITY-INTEGRATION.md`.

**Gate:** `./tools/art/prepush.sh` or `python3 tools/art/print_art_summary.py`.