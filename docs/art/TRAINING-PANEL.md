# Training panel (presentation)

| Zone | Fraction | Binding |
| --- | --- | --- |
| Run queue | 0.60 | active runs, `TrainingRunPanel` |
| Compute split | 0.40 | train/serve/R&D sliders |
| Mascot row | full width bottom | `ModelMascotRow` |

**Icons:** `icon-training.svg`, `icon-compute.svg` on split section.

**Props:** `prop-data-stack` near queue; `prop-server-rack` when compute-heavy run selected.

**Danger:** tint cone `prop-safety-cone` with `CampusUiTokens.UiDanger` when incident rung ≥ 2.