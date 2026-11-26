# CC0 incoming assets

Fetch lane assets:

```bash
./tools/art/cc0/fetch_cc0_assets.sh
```

- `incoming/kenney_city-kit-suburban_2.0.zip` — Kenney City Kit (Suburban); unzip to `incoming/kenney-city-kit-suburban/`
- `incoming/SHA256SUMS.txt` — checksums for zip + HDRI
- `hdri/*.hdr` — Poly Haven (gitignored when >5 MB; see `docs/art/LICENSES.md`)

Blender imports Kenney GLBs from `incoming/kenney-city-kit-suburban/Models/GLB format/` via `blender_common.import_cc0_kenney_glb`.