# Art licenses (CC0 hero lane)

Third-party **CC0** assets used in AI Labs Tycoon art lanes. In-repo procedural art is documented in `EARLY-GAME-LICENSES.md`.

| Asset | Path(s) | License | Source |
| --- | --- | --- | --- |
| Brown Photo Studio 02 (1k EXR) | `tools/art/cc0/incoming/brown_photostudio_02_1k.exr` (canonical) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Poly Haven — brown_photostudio_02](https://polyhaven.com/a/brown_photostudio_02) |
| Furniture Kit v1.0 | `tools/art/cc0/incoming/kenney/kenney_furniture-kit.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — furniture-kit](https://kenney.nl/assets/furniture-kit) |
| Building Kit | `tools/art/cc0/incoming/kenney/kenney_building-kit.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — building-kit](https://kenney.nl/assets/building-kit) |
| City Kit (Suburban) v2.0 | `tools/art/cc0/incoming/kenney/kenney_city-kit-suburban_20.zip` | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | [Kenney — city-kit-suburban](https://kenney.nl/assets/city-kit-suburban) |

Unity references the HDRI path via `EarlyGameGarageUrPresentationSpec.HdriCanonicalPath` (manifest only — no second EXR binary in `Assets/`).

Manifest: `tools/art/cc0/manifest.json`. Fetch: `./tools/art/cc0/fetch_polyhaven_hdri.sh`, `./tools/art/cc0/fetch_kenney_cc0.sh`.