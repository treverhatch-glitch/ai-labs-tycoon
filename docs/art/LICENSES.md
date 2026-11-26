# Art source licences (CC0-only pipeline)

Only assets from the packs listed here may be imported into the Blender restyle pipeline. Each row is the canonical pack page; vendored subsets live under `docs/art/cc0/`.

| Pack | Author | Licence | URL | Vendored path |
| --- | --- | --- | --- | --- |
| Furniture Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/furniture-kit | `docs/art/cc0/kenney-furniture-kit/` |
| Studio Small 09 (HDRI) | Poly Haven | [CC0 1.0](https://polyhaven.com/license) | https://polyhaven.com/a/studio_small_09 | `docs/art/cc0/polyhaven/hdr/studio_small_09_1k.hdr` |
| Wood092 (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Wood092 | `docs/art/cc0/ambientcg/Wood092_1K-JPG/` |
| Metal048A (1K) | ambientCG | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://ambientcg.com/view?id=Metal048A | `docs/art/cc0/ambientcg/Metal048A_1K-JPG/` (Color, NormalGL, Roughness only) |
| Factory Kit | [Kenney](https://www.kenney.nl) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/factory-kit | `docs/art/cc0/kenney-factory-kit/` (local fetch via `tools/art/cc0/download_kenney_factory.sh`) |

**Hero `prop-desk` (PR #92):** assembled from Kenney `desk.glb`, `computerScreen.glb`, `computerKeyboard.glb`, `computerMouse.glb`; restyled palette, 1024 AO + albedo bake; preview uses Poly Haven HDRI + mid-grey ground plane.

**Hero `prop-server-rack` (PR #99):** procedural dark charcoal cabinet (bevelled bpy meshes, Metal048A tint); Kenney accent **`screen-panel-flat.glb`** only for door glass strip; preview backdrop exactly RGB 150.