# Early art quality pass (post–#86)

**Status:** Post–#86 PR — packet 1 (15 garage) materials + studio previews; staff/wisp/HUD in later packets.

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