# Art pipeline

## Principles

1. **Rules vs pixels:** `Rules/` never references meshes, textures, or UI prefabs. Unity reads `GameState` and `DisplayQueries` only.
2. **License safety:** No third-party asset store packs, no scraped logos, no real company marks. Rivals use parody silhouettes and palette swaps only.
3. **In-repo truth:** Every committed mesh is either (a) procedural output from `tools/art/` or (b) hand-authored ASCII/OBJ in `unity/Assets/Art/`.
4. **One regen command:** `python3 tools/art/generate_campus_kit.py` rebuilds the starter campus kit and `manifest.json`.

## Stages

| Stage | Output | Owner thread | Gate |
| --- | --- | --- | --- |
| **P0 Spec** | Style guide, HUD wireframes, asset register | graphics (`bc-6502c7bf`) | Coordinator batch GO |
| **P1 Blockout** | Procedural OBJ kit, `CampusPresentationSpec` camera constants | graphics | Unity imports OK |
| **P2 Greybox** | Room modules instanced from `RoomBlockView` + props per purpose | Unity (`bc-93b5443a`) | Edit Mode capture PNG |
| **P3 Skin** | URP Lit materials, era skins, glow emission on compute rooms | Unity + graphics | CI `unity.yml` green |
| **P4 Polish** | Staff bobbles, mascot rigs, world map pins | graphics batch 2+ | Hatch PC spot check |

## Folder layout (Unity)

```
unity/Assets/Art/
  Generated/           # OBJ + manifest (committed)
  Materials/           # URP materials (Unity thread)
  Textures/            # hand-painted or procedural PNG (future)
  UI/                  # sprites, nine-slices (future)
```

## Import settings (Unity thread)

Apply consistently when importing `Generated/**/*.obj`:

| Setting | Value |
| --- | --- |
| Scale factor | 1 |
| Convert units | off (meters) |
| Generate colliders | off (presentation) |
| Normals | Import / Calculate |
| Materials | None (assign `Art/Materials/CampusFallback.mat`) |

## Pre-push (required)

```bash
python3 tools/art/generate_campus_kit.py
python3 tools/art/generate_ui_icons.py
python3 tools/art/validate_unity_meta_guids.py
```

## Review checklist (each batch)

- [ ] Regenerated OBJ hashes match `manifest.json` vertex/face counts
- [ ] No files larger than 500 KB unless justified
- [ ] `git log` author/committer `342607508+treverhatch-glitch@users.noreply.github.com`, no `Co-authored-by`
- [ ] `docs/QUESTIONS.md` updated for any presentation fork
- [ ] Unity Edit Mode tests still pass (`HqViewCaptureTests`, palette tests)

## Coordination with Unity thread

- **Do not** depend on Hatch's PC Unity editor (cloud agents only).
- Unity thread owns: scenes, materials, wiring props into `RoomBlockView`, UI Toolkit migration if any.
- Graphics thread owns: specs, procedural source, palette tokens, HUD layout docs, `manifest.json`.
- Shared contract: [UNITY-INTEGRATION.md](UNITY-INTEGRATION.md) and `CampusPresentationSpec.cs`.