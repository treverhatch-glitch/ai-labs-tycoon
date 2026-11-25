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
| `prepush.sh` | All generators + meta/catalog/icon/bloc/era checks |

All geometry is original, generated in Python, and safe to commit in a public repo.