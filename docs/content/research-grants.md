# Research grants

The bible's "grants and prizes" source: small sums for a paper and a thank-you. The player applies for a grant of a tier, waits for a review, takes the money if the panel is satisfied, and then owes papers. Only the player's lab applies (`researchGrants.*` in the data; Q333). The starting weekly grant of the era is a different thing and is untouched.

## `ApplyForGrant(tier)`

Tier 1 to `maxTier` (3). It costs `applyCostPerTier` ($5000) a tier, on the training line, and needs `minResearchers` (2) researchers on the payroll. Refused, with nothing changed, for a lab that is not the player's, a game with no tiers or no papers, a tier out of range, a grant already under review or owed, a lab inside its wait (`GrantLocked`), too few researchers (`NotEnoughStaff`) and too little cash (`NotEnoughCash`).

## The review (`reviewWeeks`, 6)

When the weeks are up the panel scores the lab: its academic prestige (0 to 1000, from published papers) plus `reputationWeightPermille` (300) of its reputation. The tier's bar is `barPerTier` (250) a tier. The verdict uses the score at that week, not at the application, so papers published during the review help. There is no dice: `ResearchGrants.WouldPass` shows what the panel would say now.

| Verdict | What happens |
|---|---|
| Score below the bar | Declined. The fee is gone. The lab waits `cooldownWeeks` (20) before applying again |
| Score at or over the bar | Awarded at once: `awardPerTier` ($60000) a tier on the grants line. The lab owes one paper for each tier within `deliverWeeks` (30) |

## Owing papers

Each paper published after the award (`PublishPaper`) counts off one. Papers published during the review do not count. When the last one is in, the grant is settled: reputation `completedReputation` (+15) and the wait starts. If the weeks run out first the lab pays back `clawbackPermille` (500) of the award on the fines line, loses `failedReputation` (30) reputation, and the wait starts.

## Caps

- One grant at a time, a wait after each.
- The validator needs the award to be at least the application fee (no grant that cannot pay for itself), the top tier's bar at most 2000, and the clawback at most the whole award.
- Reputation stays between 0 and 1000.

## Notes

- Saved per lab: `researchGrantStage` (0 none, 1 review, 2 owed), `researchGrantTier`, `researchGrantTick`, `researchGrantOwed`, `researchGrantLockedUntil` (schema 60). A combination that does not fit is reported.
- Events: `GrantApplied`, `GrantDecided` (Pick 1 awarded, 0 declined; Amount the award), `GrantSettled` (Pick 1 delivered, 0 not; Amount the cash paid back).
