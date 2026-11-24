<!-- Generated from Rules/Data/default-config.json (lobbying). The data file is the source of truth: edit it, then update this page. -->
# Lobbying

The bible's chapter 12 lets the player "spend on influence to slow or shape rules, win subsidies", and warns it can backfire as a scandal; the room list gives the policy and legal room "lobbying, compliance, hearings". Until now only rivals in a feud lobbied (they raised the player's attention at home). The player can now pay for a push on any government (see QUESTIONS Q258 and Q259).

## `Lobby`

`Command.Lobby(labId, blocId, goal)`, goal `"quiet"` or `"subsidy"`. It costs weeks of running the lab (the lab's weekly running cost, never under `minWeekCost` 1,000), paid at once on the hiring line, and works like this:

| Goal | Costs | If it lands |
|---|---|---|
| `quiet` | **6** weeks (`quietCostWeeks`) | The government's attention to the lab falls by **150** (`quietAttention`), never below 0 |
| `subsidy` | **4** weeks (`subsidyCostWeeks`) | The lab is paid **8** weeks of running costs as a grant (`subsidyReturnWeeks`) |

- A **backfire** is rolled first: **12%** (`backfirePermille`), less **2.5 points per policy person**, never under **2%** (`backfireFloorPermille`). A backfire ruins either goal: the government's attention goes **up 200** (`backfireAttention`) and reputation falls **40** (`backfireReputation`, softened by the press room's damping).
- A quiet word that does not backfire always lands. A subsidy push lands on **40%** (`subsidyChancePermille`) plus **5 points per policy person**, up to **80%**; otherwise it misses and the money is gone.
- Each government can be lobbied once every **26** weeks (`cooldownWeeks`), per government, home or not. `BlocStanding.LobbyUntilTick` holds it.
- Refused, changing nothing: an unknown government or goal, a quiet word where attention is already 0, a government still cooling (`LobbyCooling`), not enough cash, or any lab but the player's.

The answer is a hash of the seed, the week, the lab and the government, so it uses none of the shared dice and the same week always gives the same answer. `EventKind.Lobbied` is logged (Amount the cost, Ref `bloc|goal|result` with result `landed`, `missed` or `backfired`) and the company chat has a line for it.

## Notes

- Saved: `lobbyUntilTick` per government standing, schema 37.
- Rivals and the autopilot never lobby, so the economy guards are untouched.
- Open: choosing the rules a government writes next (the "shape" half of the bible's line) needs the law system to take a nudge; the lobby screen (Unity).
