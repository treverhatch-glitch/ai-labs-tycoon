# Platform deals

Distribution deals with a phone maker, a browser or an office suite (`platforms.*` in the data; Q394). Eight saved lab fields (`platPlat`, `platLeft`, `platCut`, `platRenewals`, `platReliance`, `platHoldup`, `platPaid`, `platKept`; schema 96). Only the player's lab; one deal at a time.

| Command | What happens |
|---|---|
| `SignPlatform` (1 phone from era 3, 2 browser era 4, 3 office suite era 5) | needs a released model of capability 200 and reputation 300; costs nothing; 96 weeks |
| `RenewPlatform` | only in the last 12 weeks; term resets; the cut goes up 5 points |
| `WalkFromPlatform` | ends the deal now and costs reputation by reliance |

Every week the market pays the lab its revenue and the deal adds 20% (phone), 15% (browser) or 25% (office suite) of it on top; the platform keeps 35%, 30% or 40% of that extra (the cut). Reliance (the share of the lab's users who come through the platform) grows 0.8 points a week. On entering the last 12 weeks the platform raises its cut by 15 points if reliance is at least half. At the end of the term, or on a walk, the deal ends and the lab loses reputation: 12 points at full reliance, fewer at less (damped by the press room). The cut never goes over 80%.

Events: `PlatformSigned`, `PlatformHoldup`, `PlatformRenewed`, `PlatformEnded` (Pick 0 walked, 1 ran out).
