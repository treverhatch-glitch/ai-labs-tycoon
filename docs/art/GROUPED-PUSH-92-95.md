# Grouped push batches 92–95 (LOCAL ready)

**Remote parent:** `db6f9e3e32d38f724710c6cfa2661b130a0e1264` (batch 91).

| Batch | Topic | Key token |
| --- | --- | --- |
| 92 | ALT rollup refresh 89 | `CampusAlt76RollupRefreshB89PresentationSpec` |
| 93 | GO packet through 90 | `CampusGoPacketTailRefreshThrough90PresentationSpec` |
| 94 | Presentation tail rollup | `CampusPresentationTailRollupPresentationSpec` |
| 95 | Staff presentation tail | `CampusStaffPresentationTailIndexPresentationSpec` |

**Squash:** one `commit-tree` child of `db6f9e3`; author `342607508+treverhatch-glitch@users.noreply.github.com`; **no** `Co-authored-by`; **plain** push only (no force).

**Gate:** `./tools/art/prepush.sh` (meta + coauth grep).

**Coordinator:** Hatch asked **no CI trigger on push** — include skip directive in squash message if branch workflow requires it.

Batch **96+** stays LOCAL until next GO.