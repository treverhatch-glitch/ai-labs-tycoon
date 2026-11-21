<!-- Generated from Rules/Data/default-config.json (naming). The data file is the source of truth: edit it, then update this page. -->
# Version naming

The bible's chapter 6 calls it a "joke that is also a system": the game suggests model names that grow more confusing with every release, suffixes stack up over time, clean naming costs a little marketing effort, and customers are measurably confused if you let it rot. Only the player's labs let it rot; rivals keep plain names. It is saved as one number per lab (save schema 30).

## The mess

A number from 0 (tidy) to 1000. Every training run the player starts adds **70**. It never falls on its own.

## The names

A new model is named `Family Line Version`, plus one stacked word for every 100 points of mess the lab had when the run started.

| Mess when the run starts | Words | Example |
| --- | --- | --- |
| 0 to 99 | none | `Fable Mini 2` |
| 100 to 199 | 1 | `Fable Mini 3 Pro` |
| 200 to 299 | 2 | `Fable Mini 4 Pro Max` |
| 300 to 399 | 3 | `Fable Mini 5 Pro Max Turbo` |
| 400 to 499 | 4 | `Fable Mini 6 Pro Max Turbo Ultra` |
| 500 to 1000 | 5 to 10 | then `Plus`, `v2`, `Final`, `(new)`, `Final-FINAL`, `Reborn`; the list holds at its last word |

The name is fixed when the run starts and is saved with the model. Old models keep the names they were given, and a clean-up changes only the next one.

## What customers do

Customers who cannot tell the lab's models apart give up part of the appeal: 8% at full mess, in a straight line (80 permille of appeal at 1000). It shows as its own part of the appeal breakdown (`NamingConfusion`), after trust and before the Gauntlet crown, so a screen can say why a model sells less than its quality says.

## The clean-up

`CleanUpNaming` takes **400** points off the mess and costs **4** times a sales rep's hiring fee in the current era (the marketing team's time, paid on the same ledger line as other hiring and press costs). It is refused, and changes nothing, when the names are already tidy or the cash is short.

## Notes

- Rivals never gain mess and keep plain names. This is the same knife-edge reason as Q182, Q190 and Q194 (see QUESTIONS Q195 to Q197).
- Save schema 30 adds `namingMess` to every lab. An older save loads with it at 0.
