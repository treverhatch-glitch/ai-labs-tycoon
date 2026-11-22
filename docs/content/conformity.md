# Conformity assessment

A staged pipeline that certifies one released model (`conformity.*` in the data; Q418). Seven saved fields: `certStage`, `certLeft`, `certScope` and `certTries` on each model, and `cfGranted`, `cfFindings` and `cfSuspended` on each lab (schema 111). Only the player's lab; at most 2 models in the pipeline.

| Command | What happens |
|---|---|
| `StartConformity` (model, scope 1 narrow or 2 broad) | pays the dossier in research points (300, plus 500 for broad); stage 1 lasts 4 weeks |
| `SubmitEvidence` (model) | stage 2: needs a live red-team result, and for a broad scope a live external audit too; not allowed during remediation |
| `WithdrawConformity` (model) | drops the assessment or gives up the certificate |

Stage 3 is the review, 8 weeks. At its end the assessor checks the model: true alignment at least 450 (narrow) or 650 (broad), and the shown alignment no more than 200 above the true one. Pass and the model is certified for 104 weeks and each of its sectors gains 20 trust. Findings send it back to stage 2 with a 6 week remediation delay (and a count of tries): the lab has to fix the model (fine-tune it, close the gap with an audit or a post-mortem) and have a live red-team result again before it can submit.

A certified model's weekly incident chance is cut by 20% (narrow) or 35% (broad). Halfway through (week 52) the assessor checks again with the same bar: a model whose true alignment has dropped, say from corruption, is suspended and each of its sectors loses 30 trust. A model that leaves sale ends its assessment.

The red-team and audit windows are timed, so the order matters: a red-team result lasts 26 weeks and an audit certificate 104, and the evidence has to be live on the day it is submitted.

Events: `ConformityStarted` (Pick the scope), `ConformityDecided` (Pick 1 certified, 0 findings), `ConformityEnded` (Pick 0 withdrawn, 1 expired, 2 suspended).

Together: an incident on a model with a mark leaves a finding on the incident register when the lab has joined it (Q423).
