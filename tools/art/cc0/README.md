# CC0 art pipeline

1. **Download** — `./tools/art/cc0/download_packs.sh` (curl → `incoming/archives/`, unzip → `incoming/*`).
2. **Restyle** — headless Blender scripts under `tools/art/cc0/blender/` (game role palette, bevels, 1024 AO + albedo bakes).
3. **Export** — glTF **+Y up** (`export_scene.gltf`), URP-friendly albedo + AO textures.
4. **Previews** — Poly Haven HDRI, 3/4 ortho camera, mid-grey ground from committed GLB (`docs/art/previews/`).

Compute hardware hero lane: `run_compute_heroes.sh` → `unity/Assets/Art/Cc0/compute-kit/`.

Branch commits must not include `Co-authored-by` trailers (use `git commit --no-verify` only if hooks inject one).

**Push (forward-only):** force-push is banned (`--force`, `--force-with-lease`, `+refspec`). Before every push run
`./tools/art/cc0/prepush.sh` (ancestor check via `git merge-base --is-ancestor origin/<branch> HEAD` — abort if it fails).
Plain `git push -u origin cursor/art-compute-hardware-cc0` only. If remote is already wrong, leave it as-is and fix with a new commit.