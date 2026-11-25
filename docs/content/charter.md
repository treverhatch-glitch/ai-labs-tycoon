# The charter

What kind of company the player's lab is (`charter.*` in the data; Q374). Four saved lab fields (`charter`, `charterTo`, `charterDue`, `capPaid`; schema 83); older saves are for-profit with nothing under review. Only the player's lab.

| Form | Funding | Research grants | Every week |
|---|---|---|---|
| For-profit (0) | A round raises its full cash | 100% of the grant | Nothing |
| Capped-profit (1) | `cappedFundingPermille` (700) of a round | `cappedGrantPermille` (1200) | `distributionPermille` (150) of last week's profit goes to investors until `capAmount` (400,000) has gone |
| Nonprofit (2) | Refused | `nonprofitGrantPermille` (1800) | `donationPerPrestige` (20) for each point of prestige comes in as a grant |

## Changing form

`ConvertCharter` takes the form (0, 1 or 2). It is refused for the form the lab has, while a change is already under review, and without the `convertFee` (40,000), which is paid now. The change is made `reviewWeeks` (26) later. `CancelCharterChange` drops it; the fee stays spent.

| Change | What else happens |
|---|---|
| Nonprofit to for-profit or capped-profit | Everyone loses `profitMorale` (200) morale and a challenge is filed on the docket (Q371): claim `challengeClaim` (250,000), strength `challengeStrength` (650) |
| Anything to nonprofit | Everyone gains `missionMorale` (100) morale |
| Anything to capped-profit | The investors' total starts again at nothing |

Events: `CharterReviewStarted`, `CharterReviewDropped`, `CharterChanged` (Pick is the new form, Amount the old).
