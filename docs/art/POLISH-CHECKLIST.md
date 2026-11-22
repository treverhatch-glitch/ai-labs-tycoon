# Polish checklist (post batch 2)

Use when wiring assets on Unity thread (`bc-93b5443a`) or graphics batch 3.

## Scene

- [ ] `CampusPresentationSpec.ApplyGameplayCamera` replaces `HqGridView.CenterCamera`
- [ ] Floor smoothness = `CampusUiTokens.FloorSmoothness`; rooms = `DefaultSmoothness`
- [ ] Era transition cross-fade on lot plate (0.8s)

## Props

- [ ] Manifest has 20 OBJ ids; missing import logs error in editor once
- [ ] Compute rooms: rack + 2–4 `prop-gpu-blade` instances
- [ ] Campus lots: trees + bike rack on border strip

## HUD

- [ ] Top bar uses `CampusUiTokens` (batch 2 wired `HqHudView` + `HqUiFactory`)
- [ ] Import `ui-icons/*.svg` as sprites; assign to future tab rail
- [ ] Advance-week button meets 44×44 pt hit target at 1080p

## CI

- [ ] `HqViewCaptureTests` still writes `unity/artifacts/hq-view.png` > 4 KB
- [ ] New Edit Mode tests green: `CampusPresentationSpecTests`, `CampusUiTokensTests`

## Out of scope until Hatch PC

- Custom OFL font
- Rival illustrated silhouettes (use badge + palette only)