# Art tools

| Script | Output |
| --- | --- |
| `generate_campus_kit.py` | `unity/Assets/Art/Generated/campus-kit/*.obj` + `manifest.json` |
| `generate_ui_icons.py` | `unity/Assets/Art/Generated/ui-icons/*.svg` + `ui-icons-manifest.json` |

| `validate_unity_meta_guids.py` | Exit 1 if any `unity/Assets/**/*.meta` guid ≠ 32 hex chars |

All geometry is original, generated in Python, and safe to commit in a public repo.