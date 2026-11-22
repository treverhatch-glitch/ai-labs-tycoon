# Licensing regimes

Regulation that arrives with the eras and that the lab chooses whether to join (`licence.*` in the data; Q410). Eight saved lab fields (`licRegime`, `licPhase`, `licLeft`, `licStrikes`, `licCooldown`, `licGranted`, `licRefused`, `licSuspended`; schema 108). Only the player's lab. Nothing happens until the lab files.

| Regime | Opens in era | Fee | Term | Best model alignment needed | Fine cut |
|---|---|---|---|---|---|
| 1 registry | it-sees (2012) | 60,000 | 104 weeks | 300 | 15% |
| 2 licence | chat-boom (2022) | 200,000 | 104 weeks | 500 | 30% |
| 3 audit regime | harnesses (2026) | 600,000 | 104 weeks | 700 | 50% |

| Command | What happens |
|---|---|
| `ApplyLicence` (regime) | the fee is paid now; a six week review; one application or licence at a time; refused if the era has not opened that regime or the lab is in a cool-down |
| `RenewLicence` | only in the last 12 weeks of a term; the fee again; needs the alignment bar; clears the strikes |
| `SurrenderLicence` | ends the licence or the application; nothing refunded |

When the review ends the lab's best released model is checked against the regime's bar. Passing grants the licence for its term; failing refuses it, keeps the fee, and starts a 26 week cool-down. A held licence takes its share off every incident fine, but each incident while licensed is a strike: at 2 strikes the licence is suspended, which costs twice the fee, 40 reputation (damped) and a 26 week cool-down. A licence that is not renewed lapses at the end of its term.

The fee and the suspension penalty go on the fines line of the books. Events: `LicenceApplied`, `LicenceDecided` (Pick 1 granted, 0 refused), `LicenceStrike`, `LicenceEnded` (Pick 0 lapsed, 1 surrendered, 2 suspended).

A best model whose training run was rushed (run queue level 3) counts as 60 less aligned when a review or renewal checks the bar (`licence.rushPenalty`; Q414). Breaking a support pledge, or losing one because the model left sale, while the licence is held is a strike (`licence.breachStrike`; Q415).
