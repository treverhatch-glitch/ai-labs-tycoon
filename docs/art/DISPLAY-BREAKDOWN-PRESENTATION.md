# Explainable numbers (presentation)

From `docs/DESIGN.md`: capability and cash values show **parts that sum to the total**. Graphics does not implement `DisplayQueries`; Unity UI should:

| Drawer | Visual |
| --- | --- |
| Quality / appeal | stacked bar, one colour per part label from rules |
| Weekly revenue | monospace list; largest part bold |
| Incident odds | icon-danger when above era median (tint only) |

Index: [EXPLAINABLE-HUD-INDEX.md](EXPLAINABLE-HUD-INDEX.md) (batch 56). Room build context: [ROOM-BUILD-INDEX.md](ROOM-BUILD-INDEX.md) (batch 68). Use `CampusUiTokens` for drawer chrome; layout constants in `CampusBreakdownPresentationSpec`.

Rivals feeling (-1000…1000 per `docs/DESIGN.md`): normalize to bar width in rivals panel (`RIVALS-PANEL.md`), tint from `RivalPresentationPalette` — graphics does not read `LabState` in batch 20.