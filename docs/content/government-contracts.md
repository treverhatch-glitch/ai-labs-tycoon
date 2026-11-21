# Government contracts

Governments hire the player's lab in turn (`govContracts.*` in the data; Q373). Nine saved lab fields (`contractKind`, `contractStart`, `contractDue`, `contractPaid`, `contractMissed`, `offerKind`, `offerDue`, `offerSeq`, `contractNext`; schema 82); older saves have none. Only the player's lab.

## Offers

Nothing is offered while the lab has no model on sale. Otherwise `offerEveryWeeks` (30) after the lab's last contract ended or was turned down (or after its first model sold), the next kind in the list is offered and stays open `offerWeeks` (8); `DeclineContract` or letting it lapse starts the wait again and moves on to the next kind.

| Kind | Tier | Engineers | Weeks | Pay | Safety morale | On accepting | On finishing |
|---|---|---|---|---|---|---|---|
| Public services | 0 | 2 | 24 | 90,000 | 0 | 0 | +15 reputation |
| Intelligence analysis | 2 | 3 | 26 | 240,000 | -150 | -10 | +10 |
| Defence systems | 3 | 4 | 30 | 600,000 | -300 | -30 | +5 |

## Accepting

`AcceptContract` is refused unless the lab holds the kind's security tier (Q372), has no contract in force and has engineers to spare: those who are not on a sabbatical, not on the support desk (Q366) and not already on a contract, less `keepFree` (1). The last engineers in the lab's list (who are not on leave) are then away for the whole term: they do no other work and cannot go on sabbatical, and the desk can take fewer.

## Pay and ending

The pay comes in `installments` (3) at even steps over the term. Each is paid (a grant) if enough engineers are still free on its date, and counts as missed if not. `missLimit` (2) misses end the contract; so does a leaked breach (Q372) for any kind that needs a tier. An ended contract costs `penaltyPermille` (300) of its pay and half of `abandonReputation`; `AbandonContract` costs the same penalty and `abandonReputation` (20), damped by the press. A contract that runs its term gives its finishing reputation. Either way the next offer waits `offerEveryWeeks`.

Events: `ContractOffered`, `ContractAccepted`, `ContractDeclined`, `ContractInstalment`, `ContractEnded` (Pick 1 completed, 0 ended for misses, -1 abandoned, 2 lost to a leak).
