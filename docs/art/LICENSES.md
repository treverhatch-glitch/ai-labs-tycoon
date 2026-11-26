# Art licenses (CC0 kit lane)

Packet 3–4 env and hero props use **CC0-only** third-party sources. No asset-store packs, no scraped marks.

## Allowed sources

| Source | License | Use in repo |
| --- | --- | --- |
| [Kenney.nl](https://kenney.nl/assets) | CC0 1.0 | Kit meshes (download into `tools/art/cc0/incoming/kenney/`, cite asset name in manifest) |
| [Quaternius](https://quaternius.com/) | CC0 1.0 | Low-poly kit pieces (same `incoming/quaternius/`) |
| [Poly Haven](https://polyhaven.com/) | CC0 1.0 | HDRIs (`tools/art/cc0/hdri/`), PBR materials |
| [ambientCG](https://ambientcg.com/) | CC0 1.0 | Seamless PBR textures (`unity/Assets/Art/EarlyGame/textures/`) |

Store **attribution + URL + download date** in `unity/Assets/Art/EarlyGame/garage-manifest.json` per asset (`license` + `cc0Source` when applicable).

## Pipeline (stylised indie tycoon)

1. **Kit or restyle** — Start from CC0 GLB/OBJ or restyle with Blender headless: role palette (`blender_material_palette.py`), bevels, 1024 AO/albedo/normal/roughness bake.
2. **Export** — glTF binary **+Y up**, URP Lit in Unity (metallic-roughness).
3. **Previews** — Poly Haven HDRI when `tools/art/cc0/hdri/*.hdr` present; else mid-grey studio (`blender_preview_rig.py`). Camera 3/4, contact shadow on ground.
4. **Hero shots** — Composed scenes (e.g. FOR SALE sign + `garage-exterior-wall`) saved under `docs/art/previews/*-hero.png`.

## In-repo procedural fallback

When a CC0 kit piece is not yet imported, scripts may **restyle** authored meshes (bevel, bake, palette) but must not ship raw primitive blockouts as final art. Track replacement in `docs/QUESTIONS.md` until the CC0 file lands.

## HUD / swatches

2D HUD icons, panels, and `tex-garage-wall-beige` swatch stay on the existing in-repo CC0-1.0 bake lane unless a coordinator explicitly folds them into a kit migration.