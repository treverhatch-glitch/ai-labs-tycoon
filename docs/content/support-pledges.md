# Long-term support pledges

A promise to keep a released model supported, which locks the model in place and pays while it lasts (`pledges.*` in the data; Q411). Three saved fields: `ltsLeft` on each model, and `ltsKept` and `ltsBroken` on each lab (schema 109). Only the player's lab.

| Command | What happens |
|---|---|
| `PledgeSupport` (model, 26 to 104 weeks) | the model must be on sale in a sector and have no end-of-life notice; at most 2 pledges at a time |
| `BreakSupportPledge` (model) | pays 8 weeks of the model's revenue, loses 60 trust in each of its sectors and 25 reputation (damped) |

While a pledge stands:

- the model earns a lift on its revenue, 4% for each full 26 weeks still left, up to 20%, so a long pledge pays more and the lift shrinks as the pledge runs down;
- the lab pays 6% of the model's weekly revenue as support (on the salaries line), so a pledge under 26 weeks costs more than it earns and one of two years or more earns more than it costs;
- no newer model can be released into a sector where the pledged model holds the lab's place, and the model cannot be given an end-of-life notice. Releasing the newer model into other sectors is allowed.

A pledge that reaches its end is kept: the lab gains 25 trust in each sector of the model. A pledged model that leaves sale is treated as a breach the next week.

Events: `SupportPledged`, `SupportPledgeEnded` (Pick 1 kept, 0 broken; Amount the fee for a breach).

A sharp pod takes a quarter off the weekly support cost (Q416). Breaking a pledge while licensed is a strike on the licence (Q415).
