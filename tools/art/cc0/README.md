# CC0 incoming art

## Packet 3–4 env / sign lane

Fetch lane assets:

```bash
./tools/art/cc0/fetch_cc0_assets.sh
```

- `incoming/kenney_city-kit-suburban_2.0.zip` — Kenney City Kit (Suburban); unzip to `incoming/kenney-city-kit-suburban/`
- `incoming/SHA256SUMS.txt` — checksums for zip + HDRI
- `hdri/*.hdr` — Poly Haven (gitignored when >5 MB; see `docs/art/LICENSES.md`)

Blender imports Kenney GLBs from `incoming/kenney-city-kit-suburban/Models/GLB format/` via `blender_common.import_cc0_kenney_glb`.

## Garage interior hero lane

1. `download_cc0_packs.sh` — curl Kenney / ambientCG / Poly Haven (+ itch Quaternius when not bot-blocked).
2. Headless Blender scripts under `tools/art/blender/*cc0*` or `garage-interior-room-kit.py`.
3. Licence log: `docs/art/LICENSES.md`.
4. Hero gate: `check_cc0_garage_hero.py` via `run_blender_cc0_garage_hero.sh`.

**Do not commit** `__pycache__` or `.pyc` under `tools/art/`.