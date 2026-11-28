# Blender migration inventory

Audit of migratable art across `main`, `cursor/early-game-art-bootstrap`, and `cursor/graphics-pipeline-5032`. **Graphics lane** owns the 44 mesh/icon rows below (excluding `EarlyGame/**`).

**Audited:** 2026-11-26 (post #86 on `main` `1f62b217`).

## Lane split

| Lane | Branch | Scope |
| --- | --- | --- |
| **early** | `cursor/early-game-art-bootstrap` (#86) | `unity/Assets/Art/EarlyGame/**` |
| **graphics** | `cursor/graphics-pipeline-5032` | Campus kit, UI icons, rival silhouettes, `BlenderBaked/` |

## Graphics pipeline (this lane)

| Step | Location |
| --- | --- |
| Scripts | `tools/art/blender/*.py` |
| Runner | `tools/art/run_blender_bake.py` |
| Outputs | `unity/Assets/Art/BlenderBaked/<id>/` |
| Previews | `docs/art/previews/<id>.png` |

**Prepush:** `ensure_unity_asset_metas.py` + `check_assets_have_meta.py` before every stage/push.

**LOCAL status:** campus-kit **30** OBJ targets — bakes staged in next slab (not on `origin/graphics` until grouped push).

## Summary

| Metric | Count |
| --- | ---: |
| **Total migratable assets** | **92** |
| Early lane | 48 |
| Graphics lane | 44 |

JSON manifests under `Art/` are excluded. Full path table lives on `main` via merge (#86 inventory); graphics mesh rows are the `Generated/campus-kit`, `ui-icons`, and `rival-silhouettes` entries marked **graphics** in that table.