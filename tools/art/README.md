# Art tools

| Script | Output |
| --- | --- |
| `generate_campus_kit.py` | `unity/Assets/Art/Generated/campus-kit/*.obj` + `manifest.json` |
| `generate_ui_icons.py` | `unity/Assets/Art/Generated/ui-icons/*.svg` + `ui-icons-manifest.json` |
| `list_catalog_ids.py` | stdout prop ids for `CATALOG-HANDOFF.md` |
| `print_art_summary.py` | prepush OK lines for GO packets |

| `validate_unity_meta_guids.py` | Exit 1 if any `unity/Assets/**/*.meta` guid ≠ 32 hex chars |
| `check_catalog_manifest.py` | `CampusPropCatalog` ids ↔ `manifest.json` |
| `prepush.sh` | All generators + meta/catalog/icon/bloc/era checks |

All geometry is original, generated in Python, and safe to commit in a public repo.