# Next grouped push (LOCAL queue)

**Remote parent:** `9b896400064d94337ee449827b5ccf89159fe661` (batches 92–95).

| Batch | Topic | Token (when ready) |
| --- | --- | --- |
| 96 | Training peek tail | `CampusTrainingPeekTailIndexPresentationSpec` |
| 97 | Mascot strip tail | `CampusHqMascotStripTailIndexPresentationSpec` |
| 98 | Compute split tail | `CampusComputeSplitTailIndexPresentationSpec` |

**Spacing:** coordinator GO only after `./tools/art/prepush.sh` clean; **~30m** since last push (`9b89640`) — no back-to-back small pushes.

**Squash:** one `commit-tree` child of tip; noreply author; zero `Co-authored-by`; `prepush.sh` grep before push; plain push only. No empty commits for CI.