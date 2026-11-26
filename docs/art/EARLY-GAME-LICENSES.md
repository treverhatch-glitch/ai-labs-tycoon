# Early-game art licenses

All assets on the `cursor/early-game-art-bootstrap` lane are **original, procedural, in-repo** content unless noted below.

| Asset tree | Generator | License |
| --- | --- | --- |
| `unity/Assets/Art/EarlyGame/garage-kit/*.obj` | `tools/art/generate_garage_kit.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/staff-kit/*.obj` | `tools/art/generate_early_staff.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/hud-icons/*.svg` | `tools/art/generate_early_hud_icons.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/wisp-moods/*.svg` | `tools/art/generate_wisp_mascot.py` | CC0-1.0 |
| `unity/Assets/Art/EarlyGame/textures/*.png` | `tools/art/generate_early_textures.py` | CC0-1.0 |

No third-party meshes, textures, fonts, or reference art were used. Typography in Play mode remains Unity built-in `LegacyRuntime.ttf` (bundled with the editor).

Regenerate before push: `./tools/art/prepush-early-game.sh`