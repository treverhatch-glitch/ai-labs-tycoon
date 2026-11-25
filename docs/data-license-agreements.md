# Data license agreements (Q215+)

From **2000** (`dataLicenses.opensYear` in config), the **player** negotiates rights to train on **licenseable** mixes (`licensed`, `scrape` by default). Rivals never pursue licenses.

## State machine

| Phase | Meaning |
| --- | --- |
| `Prospect` | Paid pursuit; must sign before `prospectEndsTick` |
| `Active` | Term running; weekly training-use obligation |
| `AtRisk` | Missed a week without an active training run on the source |
| `Revoked` | Strike cap, walk, early terminate, or expired prospect |
| `Completed` | Term ended; may `RenewDataLicense` |

## Commands

- `PursueDataLicense` — researchers, pursuit cash
- `SignDataLicense` — sign cash, sets discount and weekly RP bonus
- `WalkDataLicense` — abandon prospect
- `TerminateDataLicense` — break penalty
- `RenewDataLicense` — after `Completed`

## Weekly (`DataLicenseSystem.RunWeek`)

After training, checks `lab.LastWeek.TrainingSourceIds` (filled by `TrainingSystem.RunWeek`). Weeks with an active run on the licensed source reset strikes and grant configured research bonus. Misses add strikes toward revoke reputation penalty.

## Cross-system

- **Training:** `StartTraining` / recipes refuse licenseable sources until an **Active** license exists (only after `opensYear`). Prep cost permille discount from the signed tier.
- **Before opensYear:** scrape/distill behave as before (no license gate).

Forks: Q215–Q220 in `docs/questions-sim-hardening.md`.