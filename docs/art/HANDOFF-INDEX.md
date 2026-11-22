# Graphics handoff index (Unity thread)

| Asset kind | List command | Check |
| --- | --- | --- |
| Props (30 OBJ) | `list_catalog_ids.py` | `check_catalog_manifest.py` |
| HUD icons (10 SVG) | `list_icon_ids.py` | `check_ui_icons_manifest.py` |
| Rivals (4 SVG) | `list_silhouette_ids.py` | `check_silhouette_manifest.py` |
| Map blocs | — | `world-bloc-tints.json` + `check_bloc_tints.py` |
| Era skins | — | `era-skin-tokens.json` + `check_era_skin_tokens.py` |

**Gate:** `./tools/art/prepush.sh`