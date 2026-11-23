# HUD ledger rebuild (backup branch)

Unity composite HUD / `SpendIncome` work is rebuilt on **`cursor/unity-hud-backup`** (no PR, avoids Unity CI). Merge to **`cursor/rules-side-items` / PR #2** only after `UNITY_LICENSE` compile CI is green.

## Base

- Remote PR #2 tip: `1ee6a756da3d95176b09ff1720e0e1c8a63de159` (`cursor/rules-side-items`).
- VM loss @904 (~33k local commits) is **not** in the store; see `internal/hud-ledger-vm-loss-901-904-bc-2a01d9a5.md`.

## This tree

| Path | Role |
|------|------|
| `recovered/` | 100+ sprint runners extracted from bc-93b5443a transcript (was `/tmp` only). |
| `extract_transcript_runners.py` | Re-extract from a fetched `transcript.json` if a slice is missing. |
| `build_901_904_from_885.py` | Mirror builder for sprints 901–904 (needs `run_sprints_885_888.py` on disk). |
| `run_batch_01.sh` | Batch 01: persisted toolchain only (batch 01 push). |
| `fix_staff_mascot_tail_chain.py` | Rebuild `*Part` concat tails in StaffGpuPanel / Mascot strip. |
| `run_batch03.sh` | Batch 03: tail fix, sprint replay 4–12, golden test. |
| `run_batch04.sh` | Batch 04: sprint replay **13–20** (+ `gen_formatters_19_20.py`). |
| `bootstrap_and_run_batch02.py` | Genesis HUD from transcript merge + sprint replay driver. |
| `run_batch02_resume.sh` | Resume sprints 3–12 after genesis (safe empty commits). |
| **Batch 02 tip** | `3a05178` — genesis + sprint 3–6 partial; golden OK. |

## Run hygiene

- `git commit` inside runners during replay (matches historical VM stack); squashing optional before promoting to PR #2.
- Push: composer-2.5, noreply author, no Co-authored-by, coordinator GO, big grouped batches only.
- Golden: `dotnet test tools/SimTests/SimTests.csproj --filter FullyQualifiedName~golden`.

## Queued macro (store)

After rebuild reaches sprint **900** (`lifetimeDevKinkSpendIncomePart`), run **905–908** (site mirror 889–892). See `internal/bible-systems-design-questions-backlog.md`.