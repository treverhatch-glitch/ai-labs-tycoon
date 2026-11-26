# Blender migration (presentation lane)

**Scope:** All art **outside** `unity/Assets/Art/EarlyGame/` (not started yet). Early-game garage slice stays on the Unity/PC thread.

**Pipeline**

| Step | Location |
| --- | --- |
| Source OBJ (procedural CC0) | `unity/Assets/Art/Generated/campus-kit/*.obj` |
| Headless script per asset | `tools/art/blender/<asset>.py` |
| Shared bake (1024 PBR base, UV) | `tools/art/blender/_common.py` |
| Runner | `tools/art/run_blender_bake.py` |
| Baked outputs | `unity/Assets/Art/BlenderBaked/<asset-id>/` (`.glb`, `.fbx`, `textures/*_basecolor.png`, `manifest.json`) |
| Previews | `docs/art/previews/<asset-id>.png` |

**Command**

```bash
python3 tools/art/generate_campus_kit.py   # refresh OBJ sources
python3 tools/art/run_blender_bake.py prop_desk.py ui_rival_badge.py   # or omit args for pilot set
```

Requires Blender 4.x on PATH (`blender --background --python …`). License: repo-owned procedural geometry only; no third-party meshes.

**Prepush gate:** after each bake, run `python3 tools/art/ensure_unity_asset_metas.py` (or full `./tools/art/prepush.sh`). `check_assets_have_meta.py` fails if any `unity/Assets/**` import lacks a `.meta` — this caused the stalled **597–608+pilot** packet (CI/coordinator **run 133** prepush gate) until metas were generated.

## Status

| Asset | Script | glTF | FBX | 1024 tex | Preview |
| --- | --- | --- | --- | --- | --- |
| `ui-rival-badge` | `ui_rival_badge.py` | ✓ | ✓ | ✓ | ✓ |
| `prop-desk` | `prop_desk.py` | ✓ | ✓ | ✓ | ✓ |
| `prop-whiteboard` | `prop_whiteboard.py` | ✓ | ✓ | ✓ | ✓ |
| `prop-server-rack` | `prop_server_rack.py` | ✓ | ✓ | ✓ | 

## Queue (campus-kit manifest, non–EarlyGame)

Remaining `Generated/campus-kit` entries: rival/UI pins, props, exterior modules — one `tools/art/blender/<name>.py` each, landed in grouped presentation pushes (not doc-only squashes).

## Not in scope

- `EarlyGame/**` (Hatch: separate thread)
- Paid asset services / scraped third-party art