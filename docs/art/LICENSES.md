# CC0 art licences (AI Labs Tycoon)

Only **CC0 1.0** sources listed here may feed `tools/art/cc0/` and Blender hero exports. No asset-store packs, no scraped marks.

## Packet 3–4 env and sign lane

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

### Allowed sources (packet 3–4)

| Source | License | Use in repo |
| --- | --- | --- |
| [Kenney.nl](https://kenney.nl/assets) | CC0 1.0 | `tools/art/cc0/incoming/` |
| [Quaternius](https://quaternius.com/) | CC0 1.0 | `incoming/quaternius/` |
| [Poly Haven](https://polyhaven.com/) | CC0 1.0 | HDRIs via fetch script |
| [ambientCG](https://ambientcg.com/) | CC0 1.0 | Seamless PBR textures |

Store **attribution + URL + download date** in `unity/Assets/Art/EarlyGame/garage-manifest.json` per asset (`license` + `cc0Source` when applicable).

### Packet 3–4 pipeline (stylised indie tycoon)

1. **Kit or restyle** — CC0 GLB pieces + Blender headless bake (1024 AO/albedo/normal/roughness).
2. **Export** — glTF binary **+Y up**, URP Lit in Unity.
3. **Previews** — Poly Haven HDRI when `tools/art/cc0/hdri/*.hdr` present; else mid-grey studio.
4. **Hero shots** — FOR SALE sign + `garage-exterior-wall` + `garage-roll-door` under `docs/art/previews/*-hero.png`.

2D HUD icons, panels, and `tex-garage-wall-beige` swatch stay on the existing in-repo bake lane unless a coordinator explicitly migrates them.

## Garage interior hero (`cursor/art-garage-interior-cc0`)

Recorded **2026-11-26** (Bangkok).

| Pack | URL | Downloaded (ICT) | Licence |
|------|-----|----------------|---------|
| **Kenney Building Kit** | https://kenney.nl/assets/building-kit · https://kenney-assets.itch.io/building-kit | 2026-11-26 | CC0 1.0 Universal (Kenney) |
| **Kenney Furniture Kit** | https://kenney.nl/assets/furniture-kit · RetroDECK mirror (GLB subset) | 2026-11-27 | CC0 1.0 Universal (Kenney) |
| **ambientCG Concrete015** | https://ambientcg.com/view?id=Concrete015 | 2026-11-26 | CC0 1.0 (ambientCG) |
| **Poly Haven Studio Small 08** | https://polyhaven.com/a/studio_small_08 | 2026-11-26 | CC0 1.0 (Poly Haven) |
| **Quaternius Ultimate Platformer Pack** | https://quaternius.itch.io/ultimate-platformer-pack | *(itch bot gate; not required for garage hero)* | CC0 1.0 (Quaternius) |

### Incoming layout (garage interior)

| Path under `tools/art/cc0/incoming/` | Contents |
|--------------------------------------|----------|
| `kenney_building-kit/` | Building Kit GLBs (hero subset or full zip) |
| `kenney_furniture-kit/` | Furniture Kit GLBs (desk, chair, screen, shelf, lamp, boxes) |
| `ambientcg-Concrete015/` | Concrete albedo/normal/roughness (2K) |
| `polyhaven/studio_small_08_1k.hdr` | HDRI for Blender previews |
| `Concrete015_2K-JPG.zip` | ambientCG archive (retained) |

Regenerate: `./tools/art/run_blender_cc0_garage_hero.sh`

## Prop desk hero (main lane)

Vendored subsets may also live under `docs/art/cc0/`.

| Pack | Author | Licence | URL | Vendored path |
| --- | --- | --- | --- | --- |
| Furniture Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/furniture-kit | `docs/art/cc0/kenney-furniture-kit/` |
| Studio Small 09 (HDRI) | Poly Haven | [CC0 1.0](https://polyhaven.com/license) | https://polyhaven.com/a/studio_small_09 | `docs/art/cc0/polyhaven/hdr/studio_small_09_1k.hdr` |
| Wood092 (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Wood092 | `docs/art/cc0/ambientcg/Wood092_1K-JPG/` |
| Metal048A (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Metal048A | `docs/art/cc0/ambientcg/Metal048A_1K-JPG/` (Color, NormalGL, Roughness only) |
| Factory Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/factory-kit | `docs/art/cc0/kenney-factory-kit/` (local fetch via `tools/art/cc0/download_kenney_factory.sh`) |

**Hero `prop-desk` (PR #92):** assembled from Kenney `desk.glb`, `computerScreen.glb`, `computerKeyboard.glb`, `computerMouse.glb`; restyled palette, 1024 AO + albedo bake; preview uses Poly Haven HDRI + mid-grey ground plane.

**Hero `prop-server-rack` (PR #99):** procedural dark charcoal cabinet (bevelled bpy meshes, Metal048A tint); Kenney accent **`screen-panel-flat.glb`** only for door glass strip; preview backdrop exactly RGB 150.