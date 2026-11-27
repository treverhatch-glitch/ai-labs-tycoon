# CC0 incoming art

## Shared pipeline

1. `download_cc0_packs.sh` / `download_packs.sh` — curl Kenney / ambientCG / Poly Haven (+ itch Quaternius when not bot-blocked).
2. Headless Blender scripts under `tools/art/blender/*cc0*` or `tools/art/cc0/blender/`.
3. Licence log: `docs/art/LICENSES.md`.
4. Previews — Poly Haven HDRI, 3/4 ortho camera, mid-grey ground from committed GLB (`docs/art/previews/`).

**Do not commit** `__pycache__` or `.pyc` under `tools/art/`.

## Compute hardware hero lane

`run_compute_heroes.sh` → `unity/Assets/Art/Cc0/compute-kit/` (rack, tower, monitor).

Branch commits must not include `Co-authored-by` trailers (use `git commit --no-verify` only if hooks inject one).

**Push (forward-only):** force-push is banned (`--force`, `--force-with-lease`, `+refspec`). Before every push run
`./tools/art/cc0/prepush.sh` (ancestor check — abort if it fails). Plain `git push -u origin cursor/art-compute-hardware-cc0` only.

## Garage interior hero

`check_cc0_garage_hero.py` via `run_blender_cc0_garage_hero.sh`.