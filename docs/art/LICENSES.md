# CC0 art licences (AI Labs Tycoon)

Only **CC0 1.0** sources listed here may feed `tools/art/cc0/` and Blender hero exports.

## Staff heroes (`cursor/art-staff-founder-researcher`, PR #95)

Hero staff restyle Kenney packs in Blender (role colours, bevel, 1024 bakes, glTF +Y, URP). Previews use Poly Haven HDRI on a mid-grey ground plate.

| Pack | URL | Use on this branch |
| --- | --- | --- |
| Kenney Animated Characters 3 (CC0-1.0) | https://kenney.nl/assets/animated-characters-3 | **`staff-founder-idle`** blocky baked hero (`characterMedium.fbx`, `License.txt` in incoming); pinned kit GLB sha **`b09c59ac`** |
| Kenney stock rig (same kit) | https://kenney.nl/assets/animated-characters-3 | **`staff-researcher-idle`** — founder stock mesh + founder slot map (`kenney_researcher_from_founder.py`); coat skirt + hair cap |
| Quaternius Ultimate Modular Character (CC0) | https://quaternius.com/packs/ultimatemodularcharacters.html | Incoming reference (`character.glb`); modular experiments only |
| Quaternius Animated Man (mirrored GLB) | https://quaternius.com | Incoming reference (`animated_human.glb`) |
| Poly Haven `studio_small_08` | https://polyhaven.com/a/studio_small_08 | EEVEE preview HDRI |

Incoming paths and SHA-256 pins: `tools/art/cc0/manifest.json`.

Baked game outputs under `unity/Assets/Art/EarlyGame/staff-kit/` remain **CC0-1.0** (derivative bake). Blender tooling is GPL-3.0; pixels are in-repo CC0.

Regenerate: `./tools/art/run_blender_cc0_staff_heroes.sh`

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