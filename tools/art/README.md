# Art tools

| Script | Output |
| --- | --- |
| `generate_campus_kit.py` | `unity/Assets/Art/Generated/campus-kit/*.obj` + `manifest.json` |
| `generate_ui_icons.py` | `unity/Assets/Art/Generated/ui-icons/*.svg` + `ui-icons-manifest.json` |
| `list_catalog_ids.py` | stdout prop ids for `CATALOG-HANDOFF.md` |
| `list_icon_ids.py` | stdout HUD icon ids for `ICON-HANDOFF.md` |
| `list_silhouette_ids.py` | stdout rival ids for `SILHOUETTE-HANDOFF.md` |
| `check_silhouette_manifest.py` | `CampusSilhouetteCatalog` ↔ manifest |
| `verify_handoff_lists.py` | list_* stdout counts (30/10/4) |
| `print_art_summary.py` | prepush OK lines for GO packets |

| `validate_unity_meta_guids.py` | Exit 1 if any `unity/Assets/**/*.meta` guid ≠ 32 hex chars |
| `check_catalog_manifest.py` | `CampusPropCatalog` ids ↔ `manifest.json` |
| `prepush.sh` | Generators + checks + **forward-only** gate (`merge-base --is-ancestor` on `ART_PREPUSH_UPSTREAM`) |
| `push-branch.sh` | `prepush.sh` then plain `git push -u` (force flags rejected) |
| `prepush-early-game.sh` | Early-game lane: garage/staff/hud/wisp/texture + `prepush.sh` + garage/hud manifest checks |
| `generate_garage_kit.py` | `unity/Assets/Art/EarlyGame/garage-kit/*.obj` |
| `generate_early_staff.py` | `unity/Assets/Art/EarlyGame/staff-kit/*.obj` |
| `generate_early_hud_icons.py` | `unity/Assets/Art/EarlyGame/hud-icons/*.svg` |
| `generate_wisp_mascot.py` | `unity/Assets/Art/EarlyGame/wisp-moods/*.svg` |
| `generate_early_textures.py` | `unity/Assets/Art/EarlyGame/textures/*.png` + `*_normal.png` |
| `mesh_lib.py` / `garage_kit_builders.py` | UVs + MTL garage hero props |
| `noise_lib.py` | 512 tileable noise for early textures |

All geometry is original, generated in Python, and safe to commit in a public repo.