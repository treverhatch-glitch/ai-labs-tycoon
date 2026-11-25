# Compliance office

The player can keep a legal and policy desk that makes governments less interested in the lab. Rivals and the autopilot never run one. Nothing happens until the player sets a level (`compliance.*` in the data; Q307).

## `SetCompliance`

It takes a level from 0 to `maxLevel` (3). It is refused for a lab that is not the player's, a level out of range, the level the lab already has, a bankrupt lab, and a level the policy staff do not cover (`staffPerLevel`, 1 policy person for each level).

| Level | Retainer a week | Attention gained each month | Audit fine | Hearing |
|---|---|---|---|---|
| 0 | 0 | 100% | 100% | as it is |
| 1 | $6,000 | 92% | 85% | +5 reputation |
| 2 | $12,000 | 84% | 70% | +10 reputation |
| 3 | $18,000 | 76% | 55% | +15 reputation |

(`weeklyCostPerLevel` 6000, `attentionPermillePerLevel` 80, `auditFinePermillePerLevel` 150, `hearingPointsPerLevel` 5.) The retainer is on the salaries ledger line. The attention cut applies after what policy staff already take off (see `AttentionGain.ComplianceSaving`), and all the cuts together stay inside the limits the validator enforces: at most 90% off the attention and the whole of a fine.

## Staff

Every week the office checks the policy staff. If they cover fewer levels than the lab runs, the level drops to what they cover and `ComplianceLapsed` is logged once (the level it now runs at, and the level it had). The retainer follows the lower level. Hiring again does not bring the office back by itself; the player sets the level again.

## Notes

- Saved per lab: `complianceLevel`, schema 48.
- Attention gains are the only thing the monthly close reads, so letters, hearings, audits and freezes come later with a lower gain. A frozen bloc is not lifted by the office.
