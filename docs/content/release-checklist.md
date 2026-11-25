# Release checklist

What must be true before a model's first sale (bible ch. 9; `releaseChecklist.*` in the data; Q451). Two saved fields per model (`chkDone`, `chkMissed`; schema 129). Only the player's lab.

`SignChecklistItem` (model, item 1 red team, 2 docs, 4 eval, 8 tool policy) needs the evidence for that row. `ReleaseModel` is blocked until all four are signed when the checklist era is active. Missed items are remembered at first release and cost trust every week after (`missTrust`).

Events: `ChecklistSigned`.