# Early art quality pass (post–#86)

**Status:** PR #88 packet 1 **pass 3** — shared alpha-mask preview rig; packets 2–4 held.

### Packet 1 pass 2 (mesh join rules)

- Every garage export is **one Blender mesh island** after join; overlaps + `remove_doubles` (no hidden spine unless `EARLY_ART_ALLOW_SPINE=1`).
- Parts **overlap ~3–8 cm** where they meet; whole asset **grounded** (`min_y ≈ 0`).

### Shared preview rig (all art lanes)

- **Framing:** world bbox over all mesh vertices; camera distance `radius/tan(fov/2)×1.15`; 3/4 azimuth ~45°, elevation ~30°; aim bbox center (`blender_preview_rig.py`).
- **Alpha pass:** post-render `.mask.png` (RGBA). Fail if alpha bbox touches frame edge or `max(height,width)` extent ∉ **0.35–0.75**.
- **Saturation:** mean sat ≥ **0.15** on **mask-alpha object pixels only** (`preview_quality_lib.py`), not full-frame.
- **Garage:** `check_garage_quality_previews.py` + `garage-scene-kit` combined shot; staff/HUD: `check_early_quality_previews.py <ids…>`.
- Prepush: `check_garage_mesh_topology.py` + garage preview script (`run_blender_quality_packet1_garage.sh`).

**Parent merge:** `main` @ `1f62b217` (rules run 140 green on `52a7dd8d`).

**Brief (Hatch):** store doc `docs/early-art-quality-pass-43.md` on project agent `bc-65240cc3`.

**Scope:** 43 previews in `docs/art/previews/` @ refold `52a7dd8d`.

## Prep landed locally (worktree)

| Piece | Path |
| --- | --- |
| Studio preview rig | `tools/art/blender/blender_preview_rig.py` |
| Opt-in renders | `EARLY_ART_PREVIEW_RIG=1` → `blender_common.render_preview` |

## Not started (post-merge PR)

- Per-asset material identity, bpy wear/labels/decals
- Staff silhouettes + joined props (chair/desk)
- Wisp emission + mood faces
- HUD glyph textures
- Full 1024 PBR rebake with distinct palettes