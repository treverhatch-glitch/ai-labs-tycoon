# Art licenses (CC0 kit lane)

Packet 3–4 env and hero props use **CC0-only** third-party sources. No asset-store packs, no scraped marks.

## Imported sources (this lane)

### Kenney — City Kit (Suburban) 2.0

| Field | Value |
| --- | --- |
| Author | Kenney ([kenney.nl](https://kenney.nl)) |
| License | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) |
| Download URL | https://opengameart.org/sites/default/files/kenney_city-kit-suburban_2.0.zip |
| Downloaded | 2025-11-26 |
| In repo | `tools/art/cc0/incoming/kenney_city-kit-suburban_2.0.zip`, `tools/art/cc0/incoming/KENNEY-LICENSE.txt` |
| SHA-256 (zip) | `5869c35cf30b1c87bdb2d197b6d325eebadd2ef08ea27f04797e8e08d77a9a39` |
| Pieces used | `fence-1x2.glb`, `driveway-short.glb`, `building-type-b.glb` (sign + hero facade) |

Full license text (from pack):

> License: (Creative Commons Zero, CC0)  
> https://creativecommons.org/publicdomain/zero/1.0/  
> You can use this content for personal, educational, and commercial purposes.

### Poly Haven — studio_small_09 (2k HDRI)

| Field | Value |
| --- | --- |
| License | [CC0](https://polyhaven.com/license) |
| Download URL | https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/2k/studio_small_09_2k.hdr |
| Downloaded | 2025-11-26 |
| Fetch | `tools/art/cc0/fetch_cc0_assets.sh` → `tools/art/cc0/hdri/` (not committed; >5 MB) |
| SHA-256 | `36724313c0fc66dab126ff90f081b85a0b1ef47be65eda1417bd20b033f0573f` |

Poly Haven assets are released under CC0; see [polyhaven.com/license](https://polyhaven.com/license).

## Allowed sources (general)

| Source | License | Use in repo |
| --- | --- | --- |
| [Kenney.nl](https://kenney.nl/assets) | CC0 1.0 | `tools/art/cc0/incoming/` |
| [Quaternius](https://quaternius.com/) | CC0 1.0 | `incoming/quaternius/` |
| [Poly Haven](https://polyhaven.com/) | CC0 1.0 | HDRIs via fetch script |
| [ambientCG](https://ambientcg.com/) | CC0 1.0 | Seamless PBR textures |

Store **attribution + URL + download date** in `unity/Assets/Art/EarlyGame/garage-manifest.json` per asset (`license` + `cc0Source` when applicable).

## Pipeline (stylised indie tycoon)

1. **Kit or restyle** — CC0 GLB pieces + Blender headless bake (1024 AO/albedo/normal/roughness).
2. **Export** — glTF binary **+Y up**, URP Lit in Unity.
3. **Previews** — Poly Haven HDRI when `tools/art/cc0/hdri/*.hdr` present; else mid-grey studio.
4. **Hero shots** — FOR SALE sign + `garage-exterior-wall` + `garage-roll-door` under `docs/art/previews/*-hero.png`.

## HUD / swatches

2D HUD icons, panels, and `tex-garage-wall-beige` swatch stay on the existing in-repo bake lane unless a coordinator explicitly migrates them.