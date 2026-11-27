# CC0 art pipeline

1. **Download** — `./tools/art/cc0/download_packs.sh` (curl → `incoming/archives/`, unzip → `incoming/*`).
2. **Restyle** — headless Blender scripts under `tools/art/cc0/blender/` (game role palette, bevels, 1024 AO + albedo bakes).
3. **Export** — glTF **+Y up** (`export_scene.gltf`), URP-friendly albedo + AO textures.
4. **Previews** — Poly Haven HDRI, 3/4 ortho camera, mid-grey **RGB 150** ground from committed GLB (`docs/art/previews/`).

**Lanes:** `run_compute_heroes.sh` → `unity/Assets/Art/Cc0/compute-kit/`; garage interior → `tools/art/run_blender_cc0_garage_hero.sh`.

**Era-tiered compute heroes:** asset ids like `compute-pc-tower-era1`; previews `docs/art/previews/<id>.png`.

Branch commits must not include `Co-authored-by` trailers on **lane tip commits you intend to land** (squash merge should carry a clean message).

### Merging `origin/main` without a `Co-authored-by` trailer

Cursor (and some `git commit` hooks) may append `Co-authored-by` on a normal merge commit. Prefer:

```bash
git fetch origin main
FIRST=$(git rev-parse HEAD)
git merge origin/main --no-commit
# resolve conflicts; do not change committed preview/GLB bytes for a VISUAL PASS head
export GIT_AUTHOR_NAME="treverhatch-glitch"
export GIT_COMMITTER_NAME="treverhatch-glitch"
export GIT_AUTHOR_EMAIL="342607508+treverhatch-glitch@users.noreply.github.com"
export GIT_COMMITTER_EMAIL="342607508+treverhatch-glitch@users.noreply.github.com"
TREE=$(git write-tree)
NEW=$(git commit-tree "$TREE" -p "$FIRST" -p "$(git rev-parse origin/main)" \
  -m "merge: origin/main into $(git branch --show-current)")
git reset --hard "$NEW"
```

Optional: `GIT_CONFIG_COUNT=1 GIT_CONFIG_KEY_0=commit.template GIT_CONFIG_VALUE_0= git merge …` to disable a template that injects trailers.

**Hatch policy:** If a merge is **already pushed** with a trailer, do **not** force-push; squash-merge drops it. `prepush.sh` may still FAIL on HEAD — that is OK for branch-only merge SHAs.

**Do not commit** `__pycache__` or `.pyc` under `tools/art/`.

**Push (forward-only):** force-push is banned. Before every push run `./tools/art/cc0/prepush.sh` (ancestor check). Plain `git push -u origin <branch>` only.