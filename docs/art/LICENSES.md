# Art licenses (CC0)

Only **CC0 1.0** sources listed here may feed `tools/art/cc0/` and Blender hero exports. In-repo procedural art is also documented in `EARLY-GAME-LICENSES.md`.

## Garage URP hero lane (`cursor/art-urp-garage-scene`)

| Asset | Path(s) | License | Source |
| --- | --- | --- | --- |
| Brown Photo Studio 02 (1k EXR) | `tools/art/cc0/incoming/brown_photostudio_02_1k.exr` (canonical) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Poly Haven — brown_photostudio_02](https://polyhaven.com/a/brown_photostudio_02) |
| Furniture Kit v1.0 | `tools/art/cc0/incoming/kenney/kenney_furniture-kit.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — furniture-kit](https://kenney.nl/assets/furniture-kit) |
| Building Kit | `tools/art/cc0/incoming/kenney/kenney_building-kit.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — building-kit](https://kenney.nl/assets/building-kit) |
| City Kit (Suburban) v2.0 | `tools/art/cc0/incoming/kenney/kenney_city-kit-suburban_20.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — city-kit-suburban](https://kenney.nl/assets/city-kit-suburban) |
| Concrete047A / Bricks085 (1K) | `tools/art/cc0/incoming/ambientcg/` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [ambientCG](https://ambientcg.com/) |

Unity references the HDRI path via `EarlyGameGarageUrPresentationSpec.HdriCanonicalPath` (manifest only — no second EXR binary in `Assets/`).

Manifest: `tools/art/cc0/manifest.json`. Fetch: `./tools/art/cc0/fetch_polyhaven_hdri.sh`, `./tools/art/cc0/fetch_kenney_cc0.sh`, `./tools/art/cc0/fetch_ambientcg_shell.sh`.

## Garage interior hero (`cursor/art-garage-interior-cc0`)

Recorded **2026-11-26** (Bangkok).

| Pack | URL | Licence |
|------|-----|---------|
| **Kenney Building Kit** | https://kenney.nl/assets/building-kit | CC0 1.0 Universal (Kenney) |
| **Kenney Furniture Kit** | https://kenney.nl/assets/furniture-kit | CC0 1.0 Universal (Kenney) |
| **ambientCG Concrete015** | https://ambientcg.com/view?id=Concrete015 | CC0 1.0 (ambientCG) |
| **Poly Haven Studio Small 08** | https://polyhaven.com/a/studio_small_08 | CC0 1.0 (Poly Haven) |

Regenerate: `./tools/art/run_blender_cc0_garage_hero.sh`

## Prop desk / server rack (main graphics lane)

| Pack | Author | Licence | URL | Vendored path |
| --- | --- | --- | --- | --- |
| Furniture Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/furniture-kit | `docs/art/cc0/kenney-furniture-kit/` |
| Studio Small 09 (HDRI) | Poly Haven | [CC0 1.0](https://polyhaven.com/license) | https://polyhaven.com/a/studio_small_09 | `docs/art/cc0/polyhaven/hdr/studio_small_09_1k.hdr` |
| Wood092 (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Wood092 | `docs/art/cc0/ambientcg/Wood092_1K-JPG/` |
| Metal048A (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Metal048A | `docs/art/cc0/ambientcg/Metal048A_1K-JPG/` |
| Factory Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/factory-kit | `docs/art/cc0/kenney-factory-kit/` |

**Hero `prop-desk` (PR #92)** and **`prop-server-rack` (PR #99):** see main lane docs above.