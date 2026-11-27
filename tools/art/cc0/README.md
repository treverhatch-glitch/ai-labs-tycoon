# CC0 incoming art

HDRIs and CC0 packs land in `incoming/` via fetch scripts. **One canonical binary per asset** where possible — see `manifest.json` (`canonicalPath`). Unity references HDRI by path only (no duplicate EXR under `Assets/`).

| Script | Fetches |
| --- | --- |
| `fetch_polyhaven_hdri.sh` | Poly Haven EXR |
| `fetch_kenney_cc0.sh` | Kenney furniture, building, suburban kits (ZIP) |
| `download_cc0_packs.sh` | Kenney / ambientCG / Poly Haven bundle (garage interior lane) |

Workflow:

1. Run fetch scripts; Kenney ZIPs may need browser referer — re-extract in Blender via `kenney_hero_lib.py`.
2. Headless Blender scripts under `tools/art/blender/*cc0*` or `garage-scene-hero-compose.py`.
3. Licence log: `docs/art/LICENSES.md`.
4. Hero gates: `check_garage_urp_hero_preview.py`, `check_cc0_garage_hero.py` (interior lane).

Staging extracts: `tools/art/cc0/staging/` (local only). **Do not commit** `__pycache__` or `.pyc` under `tools/art/`.