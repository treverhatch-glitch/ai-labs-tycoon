# CC0 art licences (AI Labs Tycoon)

Only **CC0 1.0** sources listed here may feed `tools/art/cc0/` and Blender hero exports.

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

**Hero `prop-desk` (PR #92):** assembled from Kenney `desk.glb`, `computerScreen.glb`, `computerKeyboard.glb`, `computerMouse.glb`; restyled palette, 1024 AO + albedo bake; preview uses Poly Haven HDRI + mid-grey ground plane.