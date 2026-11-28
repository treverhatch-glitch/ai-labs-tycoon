# Handoff rollup (presentation)

Single Edit Mode anchor for Unity import counts ([HANDOFF-INDEX.md](HANDOFF-INDEX.md), batch 18). Gate: `./tools/art/prepush.sh`.

| Kind | Expected count | Check |
| --- | --- | --- |
| Props | 30 | `check_catalog_manifest.py` |
| HUD icons | 10 | `check_ui_icons_manifest.py` |
| Rival silhouettes | 4 | `check_silhouette_manifest.py` |
| Bloc tints | 6 | `check_bloc_tints.py` |
| Era skins | 4 | `check_era_skin_tokens.py` |

Tokens: `CampusHandoffRollupPresentationSpec` (5 kinds + expected counts). Batch 60.