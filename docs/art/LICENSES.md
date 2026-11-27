# Third-party CC0 art sources (Hero lane)

Hero staff on `cursor/art-staff-founder-researcher` restyle these packs in Blender (role colours, bevel, 1024 bakes, glTF +Y, URP). Previews use Poly Haven HDRI on a mid-grey ground plate.

| Pack | URL | Use on this branch |
| --- | --- | --- |
| Kenney Animated Characters 3 (CC0-1.0) | https://kenney.nl/assets/animated-characters-3 | `staff-founder-idle` — kit **Animated Characters 3**, mesh `Model/characterMedium.fbx`, `License.txt` in incoming |
| Quaternius Ultimate Modular Character (CC0) | https://quaternius.com/packs/ultimatemodularcharacters.html | Incoming reference (`character.glb`); modular v9/v10 experiments only |
| Quaternius Animated Man (mirrored GLB) | https://quaternius.com | Incoming reference (`animated_human.glb`) |
| Kenney stock kit GLB (pinned) | https://kenney.nl/assets/animated-characters-3 | **`b09c59ac`** — founder stock gate; **`staff-researcher-idle` v12+** uses same rig bake + in-repo lab-coat recolour (`kenney_researcher_stock_recolor.py`) |
| Poly Haven `studio_small_08` | https://polyhaven.com/a/studio_small_08 | EEVEE preview HDRI |

Incoming paths and SHA-256 pins: `tools/art/cc0/manifest.json`.

Baked game outputs under `unity/Assets/Art/EarlyGame/staff-kit/` remain **CC0-1.0** (derivative bake). Blender tooling is GPL-3.0; pixels are in-repo CC0.