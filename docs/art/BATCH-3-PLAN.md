# Graphics batch 3 (planned)

**Target:** materials + tab rail mock + validation guards. Local until GO ≥30m after Graphics-P2 (~11:24 UTC 2026-11-22).

## Code

- [x] `HqGridView.CenterCamera` → `CampusPresentationSpec.ApplyGameplayCamera`
- [ ] `HqViewCaptureTests` camera aligned to presentation spec
- [ ] `CampusTabRailMock.cs` — procedural left rail using `ui-tab-rail-plaque` ids (presentation only)
- [ ] `CampusFallback.mat` spec documented; material file on Unity thread

## Assets

- [ ] 4–6 more OBJ: lounge sofa, fence segment, solar shade, water cooler, elevator core
- [ ] Bloc tint table in `world-bloc-tints.json` (data-only colours)

## Tooling

- [x] `tools/art/validate_unity_meta_guids.py` — CI/local guard for 32-char guids
- [ ] Hook into graphics batch pre-push checklist

## Docs

- Update `POLISH-CHECKLIST` when capture test changes