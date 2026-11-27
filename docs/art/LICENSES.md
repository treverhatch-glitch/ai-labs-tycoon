# CC0 art licences (AI Labs Tycoon)

Only **CC0 1.0** sources listed here may feed `tools/art/cc0/` and Blender hero exports.

## Staff engineer hero (`staff-engineer-work`, PR #89)

| Pack | URL | Licence | Used for |
|------|-----|---------|----------|
| Kenney Starter Kit 3D Platformer | https://kenney.nl/assets/starter-kit-3d-platformer | CC0 (assets); Godot repo MIT for code | `staff-engineer-work` base mesh (`character.glb`), `Textures/colormap.png` |
| Poly Haven `studio_small_09` (1k) | https://polyhaven.com/a/studio_small_09 | CC0 | Staff preview world HDRI (`third_party/cc0/polyhaven-studio_small_09_1k.exr`) |

### Planned staff lanes (not imported in hero commit)

| Pack | URL | Licence | Notes |
|------|-----|---------|-------|
| Quaternius rigged humanoids | https://quaternius.com | CC0 | Future staff lanes |
| Kenney Animated Characters | https://kenney.nl/assets/animated-characters-1 | CC0 | Alternate staff bases |
| ambientCG materials | https://ambientcg.com | CC0 | Prop/surface kits |

Source files for the engineer hero live under `third_party/cc0/`. Do not add non-CC0 packs without updating this table and Hatch review.

## Compute hardware hero (`cursor/art-compute-hardware-cc0`, PR #94)

Hero assets are **restyled derivatives** of the packs below.

| Pack | Author | License | Page | Used for (compute hardware hero) |
| --- | --- | --- | --- | --- |
| Furniture Kit | Kenney | [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/furniture-kit | Monitor set (`computerScreen`, keyboard, mouse) |
| Factory Kit 3.0 | Kenney | [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/factory-kit | Server rack shell (legacy path; heroes procedural) |
| Modular Sci-Fi MegaKit | Quaternius | [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://quaternius.com/packs/modularscifimegakit.html | PC tower reference |
| studio_small_09 (1k HDRI) | Poly Haven | [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://polyhaven.com/a/studio_small_09 | Preview lighting |
| Metal048A | ambientCG | [CC0-1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Metal048A | Rack metal roughness reference (bake palette) |

Download manifests and archive checksums: `tools/art/cc0/packs.manifest.json`.

Regenerate: `./tools/art/cc0/run_compute_heroes.sh`

## Garage interior hero (`cursor/art-garage-interior-cc0`)

Recorded **2026-11-26** (Bangkok).

| Pack | URL | Downloaded (ICT) | Licence |
|------|-----|----------------|---------|
| **Kenney Building Kit** | https://kenney.nl/assets/building-kit · https://kenney-assets.itch.io/building-kit | 2026-11-26 | CC0 1.0 Universal (Kenney) |
| **Kenney Furniture Kit** | https://kenney.nl/assets/furniture-kit · RetroDECK mirror (GLB subset) | 2026-11-27 | CC0 1.0 Universal (Kenney) |
| **ambientCG Concrete015** | https://ambientcg.com/view?id=Concrete015 | 2026-11-26 | CC0 1.0 (ambientCG) |
| **Poly Haven Studio Small 08** | https://polyhaven.com/a/studio_small_08 | 2026-11-26 | CC0 1.0 (Poly Haven) |
| **Quaternius Ultimate Platformer Pack** | https://quaternius.itch.io/ultimate-platformer-pack | *(itch bot gate; not required for garage hero)* | CC0 1.0 (Quaternius) |

Regenerate: `./tools/art/run_blender_cc0_garage_hero.sh`

## Prop desk / server rack heroes (main lane)

Vendored subsets may also live under `docs/art/cc0/`.

| Pack | Author | Licence | URL | Vendored path |
| --- | --- | --- | --- | --- |
| Furniture Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/furniture-kit | `docs/art/cc0/kenney-furniture-kit/` |
| Studio Small 09 (HDRI) | Poly Haven | [CC0 1.0](https://polyhaven.com/license) | https://polyhaven.com/a/studio_small_09 | `docs/art/cc0/polyhaven/hdr/studio_small_09_1k.hdr` |
| Wood092 (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Wood092 | `docs/art/cc0/ambientcg/Wood092_1K-JPG/` |
| Metal048A (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Metal048A | `docs/art/cc0/ambientcg/Metal048A_1K-JPG/` |
| Factory Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/factory-kit | `docs/art/cc0/kenney-factory-kit/` |

**Hero `prop-desk` (PR #92):** Kenney desk cluster; Poly Haven HDRI + mid-grey ground.

**Hero `prop-server-rack` (PR #99):** procedural rack + Kenney `screen-panel-flat.glb`; preview backdrop RGB 150.