# Art licenses (CC0-only pipeline)

Early-game art on PR **#88** and follow-on lanes uses **only** assets that permit commercial use under **CC0 1.0** (or equivalent public-domain dedication) from approved sources. Everything vendored or baked into `unity/Assets/Art/EarlyGame/` must be traceable in this file.

## Approved sources

| Provider | Site | License | Use in repo |
| --- | --- | --- | --- |
| **Kenney** | [kenney.nl](https://kenney.nl/assets) | CC0 1.0 | GLB/GLTF imports → Blender restyle → game GLB |
| **Quaternius** | [quaternius.com](https://quaternius.com) | CC0 1.0 | Modular kits / props (GLTF) |
| **Poly Haven** | [polyhaven.com](https://polyhaven.com) | CC0 1.0 | HDRI lighting, PBR models & textures |
| **ambientCG** | [ambientcg.com](https://ambientcg.com) | CC0 1.0 | Tiling materials (floor/wall trims) |

No other third-party meshes, textures, fonts, or reference imagery without an explicit coordinator GO and a row added here.

## Pipeline (stylised hero quality)

Target look: **Two Point Hospital / Kenney showcase** — readable silhouettes, role colours, smooth bevels, 1024² baked AO × albedo, **glTF +Y-up**, URP.

1. **Fetch** — `python3 tools/art/cc0/fetch_polyhaven_garage.py` (and Kenney/Quaternius scripts as added).
2. **Import + restyle** — Blender 4.0.2: import CC0 GLTF, garage role materials (`garage_canonical_materials.py`), bevel, 1024 PBR bake.
3. **Export** — `unity/Assets/Art/EarlyGame/garage-kit/{id}.glb` + textures.
4. **Previews** — Poly Haven HDRI studio (`tools/art/cc0/hdri/`), 3/4 camera, **mid-grey ground** (`blender_preview_rig.py`).

**Procedural primitive boxes are frozen** (`garage_kit_builders.py`); garage heroes use `tools/art/run_cc0_garage_hero.sh`.

## Vendored CC0 (garage hero, Poly Haven models)

Incoming GLTF packs live under `third_party/cc0/polyhaven/{PolyHavenId}/`. Each game asset lists its source in `tools/art/cc0/garage_cc0_map.json` and in `unity/Assets/Art/EarlyGame/garage-manifest.json` (`source` + `sourceId` fields).

| Game id | Poly Haven id | Notes |
| --- | --- | --- |
| garage-desk-crt | metal_office_desk | Hero desk; CRT add-on in Blender |
| garage-folding-chair | SchoolChair_01 | |
| garage-coffee-drip | CoffeeCart_01 | |
| garage-crt-tower | vintage_video_camera | Stand-in tower prop |
| garage-cable-spool | garden_hose_wall_mounted_01 | Hose/coiled proxy |
| garage-roll-door | rollershutter_door | |
| garage-server-closet | worn_metal_rack | Closet shell |
| garage-lot-curb | concrete_road_barrier | Curb massing block |
| garage-land-for-sale-sign | WetFloorSign_01 | Sign proxy |
| garage-whiteboard-1995 | standing_chalkboard_01 | |
| garage-window-frame | rollershutter_window_01 | |
| garage-wall-panel | industrial_wall_lamp | Wall accent block |
| garage-exterior-wall | garden_hose_wall_mounted_01 | Wall section proxy |
| garage-floor-tile | WoodenTable_01 | Flattened tile footprint |
| garage-voodude-rack | worn_metal_rack | Rack variant scale |

Attribution is appreciated but not required (CC0). We still record provenance in manifests for Hatch review.

## HDRI

| File | Source | License |
| --- | --- | --- |
| `tools/art/cc0/hdri/studio_small_09_1k.hdr` | Poly Haven [studio_small_09](https://polyhaven.com/a/studio_small_09) | CC0 |

## Garage interior hero (`cursor/art-garage-interior-cc0`, main @ #96)

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

## Prop desk & server rack (main lane)

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

## Tooling

| Path | License |
| --- | --- |
| `tools/art/blender/*.py` | GPL-3.0 (Blender scripting); **baked output CC0** |
| Blender 4.0.2 editor | GPL-3.0 |

## Legacy (pre-CC0 pivot)

Procedural in-repo generators (`garage-*.py` using `garage_kit_builders.py`) remain in the tree for history but must not be used for new heroes after the CC0 pivot commit on #88.

See also: `docs/art/EARLY-GAME-LICENSES.md` (lane-wide index; this file is canonical for third-party CC0).