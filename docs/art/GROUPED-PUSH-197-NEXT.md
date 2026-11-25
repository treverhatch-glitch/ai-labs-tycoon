# Grouped 197–199 · GO (≥~09:27 ICT) — landed **`a44983d`**

**Parent (must match `origin/cursor/graphics-pipeline-5032` before squash):** `ca28446`.

Q646 rollup **157** · Q647 manifest **157** · Q648 GO **193**.

## Mandatory non-FF push workflow (no force)

Canonical: Project store `internal/push-policy-standing.md` + `internal/standing-rules-graphics.md`.

1. **`git fetch origin cursor/graphics-pipeline-5032`**
2. Confirm intended squash parent = **`origin/cursor/graphics-pipeline-5032`** tip (rebuild if remote moved).
3. Decide **Q646–Q648** in `docs/QUESTIONS.md`; stage **only** 197–199 (+ doc updates).
4. **`./tools/art/prepush.sh`** — meta, coauth grep, handoff checks.
5. **`git commit-tree`** with parent = **fetched remote tip**, noreply author/committer only.
6. **`git reset --hard`** to new squash; **`git log -1 --format=%B`**; prepush again.
7. Plain **`git push -u origin cursor/graphics-pipeline-5032`** — **never** `--force` / `--force-with-lease`.
8. If rejected: repeat from (1) with **`commit-tree` onto new remote tip** or stop and report — **do not force**.
9. **`git ls-remote`**; report `*Tests.cs` via **`git ls-tree -r HEAD`** (not dirty-worktree `find`).

**HOLD** until **≥~09:27 ICT** (+30m after `ca28446`).