# Training panel (presentation)

| Zone | Fraction | Binding |
| --- | --- | --- |
| Run queue | 0.60 | active runs, `TrainingRunPanel` |
| Compute split | 0.40 | train/serve/R&D sliders |
| Mascot row | full width bottom | `ModelMascotRow` |

Full screen: [TRAINING-SCREEN-PRESENTATION.md](TRAINING-SCREEN-PRESENTATION.md) (batch 49).

**Icons:** `icon-training.svg`, split chrome per [COMPUTE-SPLIT-PRESENTATION.md](COMPUTE-SPLIT-PRESENTATION.md).

**Props:** `prop-data-stack` near queue; `prop-server-rack` when compute-heavy run selected.

**Danger:** per [TRAINING-INCIDENT-PRESENTATION.md](TRAINING-INCIDENT-PRESENTATION.md) and `CampusTrainingIncidentPresentationSpec`.