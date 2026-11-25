# Public post-mortems

After a model has an incident the player can write it up in public. The write-up wins back part of the reputation the incident cost. Only the player does this; rivals never do (`postMortem.*` in the data; Q328).

## `PublishPostMortem(model, depth)`

Within `windowWeeks` (6) of the model's last incident, once for each incident. Depth 1 is a summary. Depth 2 is a full write-up.

| | Summary | Full |
|---|---|---|
| Cash (on the fines line) | $20000 | $50000 |
| Reputation won back | 300 permille of what the incident cost | 600 permille of what the incident cost |
| Displayed alignment | unchanged | set to the true alignment (the public learns it) |

A write-up needs at least `minSafetyStaff` (1) safety people. It is refused, with nothing changed, for a lab that is not the player's, a game with no window, a depth other than 1 or 2, a model with no incident, an incident that already has its write-up, an incident older than the window, no safety staff (`NotEnoughStaff`) and too little cash (`NotEnoughCash`).

## The trade

A full write-up closes the gap between the alignment the public sees and the real one. A model that was borrowing appeal from that gap (the sandbag boost in the quality breakdown) loses it. So the player picks between a cheap summary, a dearer honest one that wins back more, and saying nothing.

## Caps

- Reputation won back is a share of what the incident actually cost the lab, recorded when the incident happened after every cut (press damping, ethics board, on-call), so a write-up never wins back more than the loss and a cut incident has less to win back. The validator rejects shares above 1000 permille and a full share below the summary share.
- Reputation stays between 0 and 1000.
- A new incident clears the old write-up and starts a new window.

## Notes

- Saved per model: `incidentRepLoss`, `postMortemDone` (0 none, 1 summary, 2 full), schema 58. A record that does not fit is reported.
- Event `PostMortemPublished` (Amount the reputation won back, Pick 0 summary, 1 full).
