# Training incident rings (presentation)

Rules supply incident rung; art maps to props only (`docs/DESIGN.md` training dangers).

| Rung | Presentation |
| --- | --- |
| ≥ 2 | Tint `prop-safety-cone` with `CampusUiTokens.UiDanger` |
| ≥ 3 | Add `icon-danger` badge beside cone |
| ≥ 4 | Pulse cone scale 1.0→1.05 (UI anim, Unity thread) |

Index: [TRAINING-INCIDENT-INDEX.md](TRAINING-INCIDENT-INDEX.md) (batch 55). Constants: `CampusTrainingIncidentPresentationSpec` (rungs 2/3/4, cone + `icon-danger`, pulse 1.05).

No rung values in graphics JSON; bind at runtime from rules queries (`SafetySystem` rungs 1–3 in rules; 4+ later per systems design).