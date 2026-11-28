# Early-game art licenses

All assets on the `cursor/early-game-art-bootstrap` lane are **original, procedural, in-repo** content unless noted below.

| Asset tree | Generator | License |
| --- | --- | --- |
| `unity/Assets/Art/EarlyGame/garage-kit/*.glb` + `textures/**` | `tools/art/blender/garage-*.py` (Blender **4.0.2**) | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/staff-kit/*.glb` + `textures/**` | `tools/art/blender/staff-*.py` (Blender **4.0.2**) | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/hud-icons/*.png` (primary) | `tools/art/blender/blender_render_hud_wisp.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/hud-icons/*.svg` (fallback) | `tools/art/generate_early_hud_icons.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/wisp-moods/*-sheet.png` | `tools/art/blender/blender_render_hud_wisp.py` (+ `generate_wisp_mascot.py` SVG fallback) | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/textures/*.png` + `*_normal.png` + `*_roughness.png` | `tools/art/blender/tex-*.py` (1024 bake) + `generate_early_textures.py` fallback | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/hud-icons/*.png` (6 finish icons) | `tools/art/blender/hud-*.py`, `panel-*.py`, `icon-gpu-hire.py`, `icon-week-advance.py` | CC0-1.0 |
| `docs/art/previews/*.png` | Blender EEVEE preview renders | CC0-1.0 |
| `unity/Assets/StreamingAssets/EarlyGame/**` | copies from Blender HUD/wisp + texture generators | CC0-1.0 |
| `tools/art/blender/blender_common.py` | shared Blender bpy helpers | GPL-3.0 (tooling; art output CC0) |

**Tool pin:** Blender 4.0.2 (`blender --version`) — GPL-3.0 editor; baked mesh/texture pixels are CC0-1.0 in-repo.

No third-party meshes, textures, fonts, or reference art were used. Typography in Play mode remains Unity built-in `LegacyRuntime.ttf` (bundled with the editor).

Regenerate before push: `./tools/art/prepush-early-game.sh` (set `SKIP_BLENDER=1` to use legacy Python OBJ generators only).