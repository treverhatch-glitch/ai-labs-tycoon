# Unity integration — graphics batch 1

Coordinates the **graphics** branch (`cursor/graphics-pipeline-5032`, base `cursor/rules-side-items`) with the **HQ / rules-side-items** thread. Do not land on `main` until the coordinator gives **GO** on push.

## What graphics delivers

| Area | Path / type |
|------|-------------|
| Generated campus kit | `tools/art/generate_campus_kit.py`, 12× OBJ, `manifest.json` |
| Presentation spec | `CampusPresentationSpec.cs` + EditMode tests |
| Art docs | `docs/art/` pipeline, style guide, asset register, HUD layouts |

See also the agent store handoff: `internal/graphics-batch1-unity-handoff.md` (graphics subagent).

## HQ thread (interim, until merge)

`HqStarterOfficeDressing` is **procedural placeholder** dressing (desks, PCs, staff capsules) called from `HqGridView.BuildRooms`. It keeps starter-office captures and playable milestone green before OBJ props exist on this branch.

`HqGridView.AlignMainCamera` centers the lot for captures and play mode. Graphics replaces this with `CampusPresentationSpec.ApplyGameplayCamera` after merge.

## Merge checklist (HQ thread owns after graphics PR is ready)

1. **Import assets** — Copy OBJ + manifest into `unity/Assets/Art/CampusKit/` (or path named in graphics PR). Let Unity generate `.meta`; commit LFS/binary per repo convention.
2. **Fallback material** — Add `CampusFallback.mat` (URP Lit) per graphics spec; reference from presentation spawner.
3. **Props in `HqGridView`** — After each room block is created, spawn kit instances from manifest (room type / density), parented under the room transform. Remove or gate `HqStarterOfficeDressing.DressRoom` (e.g. single flag or delete once OBJ path is default).
4. **Camera** — In `Rebuild()`, call `CampusPresentationSpec.ApplyGameplayCamera(lot, Camera.main)` instead of `AlignMainCamera` for runtime; keep `AlignMainCamera` only in capture tests until those tests switch to the spec API.
5. **CI captures** — `HqViewCaptureTests`, `HqStarterOfficeCaptureTests` should still write `unity/artifacts/hq-view.png` and `hq-starter-office.png` (workflow upload paths unchanged).
6. **Conflicts** — If both branches touch `HqGridView.cs`, prefer graphics prop wiring + HQ save/load/glow/shell; resolve in one stacked PR or rebase HQ onto graphics.

## CI (HQ thread, pre-merge)

- Unity **6000.3.0f1**, activation via **`UNITY_LICENSE` only** (ULF file; no email/password in workflow).
- Sim golden digest unchanged: `a9804ff2be84a33784bef21466bb3fde2e952913280e96daa67e229e91b89323`.

## Do not

- Push or merge without coordinator **GO**.
- Edit Hatch PC–local editor workflows.
- Duplicate `generate_campus_kit.py` output inside `unity/` on the HQ branch before graphics lands.