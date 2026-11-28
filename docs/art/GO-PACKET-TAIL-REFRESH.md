# GO packet tail refresh (presentation)

Extends [GO-PACKET-INDEX-LANE-REFRESH.md](GO-PACKET-INDEX-LANE-REFRESH.md) (batch 77) for tail pushes **78–83+**.

| Field | Source |
| --- | --- |
| Spacing | `CampusGoPacketTemplatePresentationSpec.MinSpacingMinutes` (30m) |
| Extended lane | [INDEX-LANE-MANIFEST-TAIL.md](INDEX-LANE-MANIFEST-TAIL.md) through batch **82** |
| STATUS sync | [ALT-76-STATUS-SYNC.md](ALT-76-STATUS-SYNC.md) (batch 83) |
| Store path | `internal/graphics-batch{N}-go-packet.md` |

Later: [GO-PACKET-TAIL-REFRESH-88.md](GO-PACKET-TAIL-REFRESH-88.md) (batch 90).

Tokens: `CampusGoPacketTailRefreshPresentationSpec`. Batch 84.