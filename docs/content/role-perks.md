<!-- Generated from Rules/Data/default-config.json (`rolePerks`). The data file is the source of truth: edit it, then update this page.
     A test (RolePerksTests) fails when a perk id or its role id is missing here. -->
# Role perks

A role beyond the first four does something more than fill a desk. Each perk is one row in `rolePerks`: every person in the role adds `perStaffPermille` to the perk, up to `capPermille`. Perks of the same kind add up. Nothing is saved: the effect is worked out from the staff a lab has today, so letting the role go ends it the same week.

| Perk | Role | Kind | Each person | Cap | Where |
| --- | --- | --- | --- | --- | --- |
| `recruiter-fee-discount` | `recruiter` Talent Recruiter | HireDiscount | 30 | 250 | - |
| `red-team-catch` | `red-teamer` Red-Teamer | IncidentCut | 25 | 300 | - |
| `alignment-fellow-work` | `alignment-fellow` Alignment Fellow | AlignmentWork | 40 | 400 | - |
| `product-lead-appeal` | `product-lead` Product Lead | SectorAppeal | 30 | 240 | every sector |
| `agent-wrangler-appeal` | `agent-wrangler` Agent Wrangler | SectorAppeal | 50 | 300 | `coding-agents`, `agent-workforce`, `enterprise-knowledge` |
| `harness-operator-appeal` | `harness-operator` Harness Operator | SectorAppeal | 50 | 300 | `agent-workforce`, `ai-research-services` |
| `roboticist-appeal` | `roboticist` Roboticist | SectorAppeal | 50 | 300 | `robotics-industry`, `household-robots`, `agri-logistics`, `devices` |
| `mission-controller-appeal` | `mission-controller` Mission Controller | SectorAppeal | 50 | 300 | `machine-science`, `space-compute` |

What the kinds do:

- `HireDiscount`: takes a share off every hiring fee, after the era's talent-war fee (`RolePerks.HireFee`; `HireFeeParts` lists list price, era shift and the discount).
- `IncidentCut`: a released model's weekly incident is caught before it happens with this chance. It is a second roll made only when an incident was about to happen, so a lab with no red-teamers plays exactly as before (`RolePerks.IncidentOdds`).
- `AlignmentWork`: adds to the work each unit of alignment compute does (`TrainingSystem.AlignmentWorkPermille`), on top of the safety-staff and containment-wing bonuses.
- `SectorAppeal`: multiplies the appeal of the lab's models in the listed sectors (every sector when none is listed), after the sales boost and before the Gauntlet crown (`AppealBreakdown.RoleBoost`).

The data roles (`labeller`, `data-engineer`, `data-gardener`), `infra`, `policy` and `chip-architect` have no perk here: the infra, policy and chip roles already act through their function, and the data roles wait for data sources.
