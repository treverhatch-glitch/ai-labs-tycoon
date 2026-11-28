# HQ capture regression

Edit Mode test `HqViewCaptureTests` writes `unity/artifacts/hq-view.png`.

| Check | Threshold |
| --- | --- |
| File exists | after capture step |
| Size | > 4096 bytes (non-blank) |
| Camera | `CampusPresentationSpec.ApplyCaptureCamera` |

HUD regions in capture: [CAPTURE-HUD-SNAPSHOT.md](CAPTURE-HUD-SNAPSHOT.md).

Graphics batches must not change capture dimensions without updating this doc and the test.