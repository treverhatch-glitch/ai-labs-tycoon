# Capital allowances

How hardware and building spend is deducted from the tax base (`allowances.*` in the data; Q383). Four saved lab fields (`capPolicy`, `capPool`, `capSwitchTick`, `capWritten`; schema 89). Works inside the tax (`docs/content/tax.md`) and needs it on. Only the player's lab.

| Policy | What the tax base sees |
|---|---|
| 0 expense (default) | the spend as it is spent; nothing is held |
| 1 capitalise | the spend added back into a pool; `writeOffPermille` (40) of the pool is deducted each week |
| 2 enhanced | as capitalise, with `enhancedPermille` (300) more in the pool; the extra is counted as tax saved for an audit and adds `enhancedExposure` (8) exposure in each week it claims something |

Going back to expensing writes the whole pool off in one week. `lockWeeks` (48) after a change the policy cannot change again (`SetCapitalPolicy`). Event: `CapitalPolicySet` (Amount the policy, SubjectId the pool written off at once).
