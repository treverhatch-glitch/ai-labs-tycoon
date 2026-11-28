# Enterprise seat deals (Q208+)

From **2018**, the **player** can run a **pipeline** on selected enterprise sectors: pursue a buyer, sign an SLA tier, earn a revenue bonus while compute keeps up, and lose the seat on repeated misses.

Not rivals, not era compliance/ethics/on-call.

## State machine

| Phase | Meaning |
| --- | --- |
| `Prospect` | Paid pursuit window; must sign before `prospectEndsTick` |
| `Active` | Term running; weekly SLA vs `lab.LastWeek.ServedPermille` |
| `AtRisk` | One or more strikes, below limit |
| `Churned` | Strike cap or walked away |
| `Completed` | Term ended cleanly; may `RenewEnterpriseSeat` |

## Commands

- `PursueEnterpriseSeat` — needs a model on sale in sector, sales staff, pursuit cash
- `SignEnterpriseSeat` — binds model, pays sign cash, sets tier SLA/bonus/term
- `WalkEnterpriseSeat` — abandon prospect
- `TerminateEnterpriseSeat` — break penalty, early churn
- `RenewEnterpriseSeat` — after `Completed`

## Weekly (`EnterpriseDealSystem.RunWeek`)

After `MarketSystem`, compares served permille to tier SLA. Bonus = sector `LastRevenue` × tier bonus permille. Strikes feed churn reputation penalty from config.

## Cross-system

- **Market:** sector entry revenue + global served permille
- **Staff:** sales headcount gate on pursue
- **Release review:** same strict sectors may still need review before first sale (use `ExpansionTestSupport` in tests)
- **Service-level pledge (era Q329, PR #20):** voluntary lab-wide premium/refund on `LastWeek.ServedPermille` (and incidents). **Not a near-duplicate** — enterprise seats are per-sector contracts with pursuit, term, bonus, and churn. Naming alone is not enough: the same low-served week can breach both.

### Q214 overlap rule (rules thread)

When the player has an active pledge level (`slaLevel > 0`, config `sla.maxLevel > 0`) and the week fails the pledge **served-rate** bar (`servedPermille < sla.breachBelowPermille` with demand), the enterprise deal still **skips its weekly bonus** but does **not** add a deal strike, does **not** apply deal churn reputation, and does **not** emit `EnterpriseSeatSlip`. The pledge keeps refund, reputation, and `slaStrikes`. Incident-only pledge breaks do not suppress deal strikes. On merge, run `Sla.RunWeek` before `EnterpriseDealSystem.RunWeek` (era order).

Enterprise **bonus** is a separate `LedgerLine.Revenue` earn from `sector.LastRevenue × bonusPermille`; it does **not** write back into `LastRevenue`, so stack2 **model sales** quotes (`WeeklyIncome` from entries) do not double-count.

Forks: Q208–Q214 in `docs/questions-sim-hardening.md`.