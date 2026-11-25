<!-- Generated from Rules/Data/default-config.json (repair). The data file is the source of truth: edit it, then update this page. -->
# Fine-tune and rollback

The bible's chapter 6 state table says a glitching model is "recent incident or bad data; fix with fine-tune or rollback". A model on sale glitches for a while after an incident (6 weeks in the shipped data, longer in some sectors) and sells at a lower quality. Both commands need a model on sale that is glitching, and are refused, changing nothing, when it is not. Nothing is saved for either (see QUESTIONS Q239 to Q241).

## Fine-tune

`FineTuneModel` costs **1 week** (`fineTuneRevenueWeeks`) of what the model sells for, never under **1,000**, paid on the training ledger line. It ends the glitch now and adds **40** to the model's real and shown alignment (never past 1000). It is refused when the cash is short.

## Rollback

`RollBackModel` withdraws the model from every sector it is sold in and puts the newest earlier version of the same line (a retired model with a lower version number) back on sale in those sectors, keeping the customers' share. It costs **20** reputation and no cash.

- The withdrawn model goes back to Trained, so it can be put on sale again with `ReleaseModel` once the lab has looked at it.
- It is refused when there is no earlier version, when the older version would not pass the lab's own release gate, or when it would not pass the law's safety bar today.

## Notes

- A fine-tune moves the model's last incident date back by the glitch length, which turns the glitch off through the existing rule. A rival trying to leak that incident sees old news.
- `ModelRepair.GlitchWeeksLeft`, `FineTuneCost` and `RollbackTarget` give a screen what to show.
- The autopilot and rivals never mend models.
- No save change. The pinned golden game did not move.
