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
| 35 | `e8ebc83` | Capture HUD snapshot |
| 36 | `6a558e1` | World map host shell |
| 37 | `b4255a8` | Start-era picker |
| 38 | `09cbb6c` | Room build panel |
| 39 | `67e1fc8` | Staff/GPU panel |
| 40 | `24a9ea1` | Training peek |
| 41 | `5798e49` | HQ mascot strip |
| 42 | `3bd78b2` | HQ lot viewport |
| 43 | `8316759` | HQ stage stack |
| 44 | `5d52a64` | Tab rail v2 |
| 45 | `2801aa1` | Safe area (in `f44cd35` push) |
| 46 | `f44cd35` | Advance week |
| 47 | `81fe8d1` | Research screen |
| 48 | `6b1adc3` | Rivals screen |
| 49 | `ad8db69` | Training screen |
| 50 | `54f990b` | Screen layout index |
| 51 | `909fc96` | Top bar token index |
| 52 | `3a303b1` | HUD era label |
| 53 | `e9d0381` | Seasonal overlay index |
| 54 | `8e7a445` | World freeze in overlay index |
| 55 | `e37f322` | Training incident index |
| 56 | `cf40d41` | Explainable HUD index |
| 57 | `8b9e790` | Staff presentation index |
| 58 | `5e6b64c` | Rival feeling index |
| 59 | `11aa6cb` | A11y chrome index |
| 60 | `6e3f97c` | Handoff rollup |
| 61 | `5eb9103` | Presentation index hub |
| 62 | `d75f611` | ALT-76 rollup refresh |
| 63 | `669a933` | GO packet template |
| 64 | `d545341` | Unity integration hub |
| 65 | `ef4f0d8` | Studio handoff hub link |
| 66 | `cc036a8` | World map host index |
| 67 | `0607fca` | Tab rail integration index |
| 68 | `1b8cf85` | Room build panel index |
| 69 | `f4f2e53` | HQ stage stack rollup |
| 70 | `3912cd7` | Staff/GPU panel index |
| 71 | `2d4058b` | Training peek index |
| 72 | `745a159` | HQ mascot strip index |
| 73 | `974aef2` | Compute split index |
| 74 | `2429879` | HQ stack completion |
| 75 | `01b0286` | Index lane manifest |
| 76 | `3cd1fda` | ALT-76 rollup refresh |
| 77 | `7bce909` | GO packet index refresh |
| 78 | `338fc05` | HQ lot viewport index |
| 79 | `fed9015` | Index manifest tail |
| 80 | `fa2920e` | HUD era tail index |
| 81 | `c567747` | Rival/world tail index |
| 82 | `a79600b` | Explainable HUD tail |
| 83 | `08727c3` | ALT-76 STATUS sync |
| 84 | `b78f921` | GO packet tail refresh |
| 85 | `13418d8` | Presentation hub tail |
| 86 | (local) | ALT-76 rollup refresh 85 |

Rollup: [ALT-76-ROLLUP.md](ALT-76-ROLLUP.md) (batch 62). Refresh: [ALT-76-ROLLUP-REFRESH.md](ALT-76-ROLLUP-REFRESH.md) (batch 76).

**Unity thread:** import kit, wire `CampusPropPlacementHints`, tab sprites, materials per `UNITY-INTEGRATION.md`.

**Gate:** `./tools/art/prepush.sh` or `python3 tools/art/print_art_summary.py`.