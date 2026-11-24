# Next grouped push (LOCAL queue)

**Remote parent:** `0fa548326a90cba7d59d4f97c1e52cce3e7bacc9` (113–115).

| Batch | Topic | Token |
| --- | --- | --- |
| 116 | Manifest tail refresh 112 | `CampusIndexLaneManifestTailRefreshThrough112PresentationSpec` |
| 117 | Tail rollup refresh 112 | `CampusPresentationTailRollupRefreshThrough112PresentationSpec` |
| 118 | GO packet through 115 | `CampusGoPacketTailRefreshThrough115PresentationSpec` |

**HOLD:** no squash before **≥19:57 ICT** / ~12:27 UTC (`0fa5483` @ 18:56 ICT + 30m).

**Squash:** `commit-tree`; prepush coauth grep; plain push on coordinator GO only.