# Release review loop (API era)

From **2017**, the **player lab** must file **review packages** before certain releases go on sale. Rivals are unchanged; this is the missing half of the bible’s “release gate” for the human lab.

## When review is required

A release needs a cleared package when **either**:

1. Any target sector is in `releaseReview.strictSectorIds` (enterprise, government, and similar buyers), or
2. The claimed alignment on release is **above** the model’s true alignment (sandbagging).

Honest releases into consumer sectors before 2017 behave as today.

## State machine

Each package is a `ReleaseReviewCase` on the player lab:

| Phase | Meaning |
| --- | --- |
| `Submitted` | Queued for the next monthly slot |
| `NeedsRevision` | Board sent it back; player must revise or appeal |
| `Approved` | `ReleaseModel` may consume this exact sector list + claimed alignment |
| `Rejected` | Terminal; reputation hit (and government attention if a strict sector was named) |
| `Withdrawn` | Player cancelled |

**Monthly** (`ReleaseReviewSystem.RunMonth` at month start):

- Safety staff add review **slots** (`baseSlotsPerMonth` + staff).
- FIFO queue; expedited packages jump the line.
- Backlog over `maxBacklogBeforePenalty` costs reputation each month.

## Player decisions (commands)

| Command | Role |
| --- | --- |
| `SubmitReleaseReview` | File trained model + sectors + claimed alignment |
| `ReviseReleaseReview` | After send-back, lower the claim and re-queue |
| `AppealReleaseReview` | Pay cash; RNG may approve or hard-reject |
| `WithdrawReleaseReview` | Drop a non-approved package |
| `ExpediteReleaseReview` | Pay cash to prioritise the queue |
| `ReleaseModel` | Refused until an **approved** package matches sectors + claim |

The headless **autopilot** submits packages when its plan would release into gated sectors, then releases after approval.

## Cross-system ties

- **Safety / sandbag:** gap vs `sandbagSendBackGapPermille` drives send-back and aligns with display-vs-true alignment elsewhere.
- **Government:** rejected strict-sector packages add an incident tick toward monthly attention.
- **Market:** no sector entry until `ReleaseModel` succeeds.
- **Staff:** safety headcount sets monthly throughput.

Design forks are logged as **Q200–Q207** in `docs/questions-sim-hardening.md`.

## After merge with era ethics board (PR #19 / item 79, Q320)

Era’s **ethics board** (`EthicsBoard`, `SetEthicsBoard`) is **not** the same system. It is an **opt-in** standing board (player-only) that:

- **Vetoes** `ReleaseModel` at command time when seated at `vetoLevel` and the model’s **true** `alignment` is below `vetoAlignment` (`CommandError.BoardVeto`).
- **Refuses** `RushRelease` when any ethics level is seated (`BlocksRush`).

**Release review** is **mandatory** (when triggered) and **monthly**: strict enterprise/government sectors and **oversold alignment claims**, independent of whether the player seats an ethics board.

These are **complementary, not duplicates**. One cleared review package does **not** satisfy the ethics veto, and ethics clearance does **not** replace filing review.

### `ReleaseModel` gate order (combined tree)

Apply every check; the first failure wins. **Do not** run a second veto penalty for the same attempt.

| Step | System | Who | What |
| --- | --- | --- | --- |
| 1 | `SafetyCulture.GateBlocks` | Era | Displayed alignment vs lab release gate |
| 2 | `EthicsBoard.Vetoes` | Era (player, opt-in) | True alignment vs board veto bar — **refuse only** |
| 3 | `EraRules.MeetsReleaseBar` | Era | Era law minimum alignment |
| 4 | Sector bars (capability, open year, etc.) | Shared | Refuse before any approval is consumed |
| 5 | `ReleaseReviewSystem` | Rules | **Consume** the approved package only when the release will succeed (same for era gates above: refuse without consuming) |
| 6 | Market attach, events | Shared | Unchanged |

`SubmitReleaseReview` / monthly resolve run **outside** `ReleaseModel` (month start). Ethics board weekly pay and lock are unrelated.

### Penalties (no double hit)

| Outcome | Review loop | Ethics board |
| --- | --- | --- |
| Monthly reject / appeal fail | Reputation (and strict-sector `incidentsThisMonth` on reject) | — |
| `ReleaseModel` refused for ethics veto | Approval **stays** `Approved` until consumed or withdrawn | Refuse only; no review reputation replay |
| `RushRelease` with seated board | — | Refuse before `ReleaseModel` |

**Rush:** ethics blocks the rush command first. If rush ever calls through to `ReleaseModel`, step 4 still applies for gated sectors/claims.

**Merge task:** insert steps 1–3 from era **above** step 4 in `CommandProcessor.ReleaseModel`; keep review consume at step 4. Do not copy ethics veto into `ReleaseReviewSystem.ResolveCase`.