# CC0 third-party art (incoming)

HDRIs and CC0 packs land in `incoming/` via `curl` scripts. **One canonical binary per asset** — see `manifest.json` (`canonicalPath`). Unity references HDRI by path only (no duplicate EXR under `Assets/`).

| Script | Fetches |
| --- | --- |
| `fetch_polyhaven_hdri.sh` | Poly Haven EXR |
| `fetch_kenney_cc0.sh` | Kenney furniture, building, suburban kits (ZIP) |

Kenney ZIPs need a browser referer; re-extract in Blender via `kenney_hero_lib.py`. Staging extracts: `tools/art/cc0/staging/` (local only).

Record every pack in `docs/art/LICENSES.md`.