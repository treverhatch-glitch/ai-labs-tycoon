<!-- Generated from Rules/Data/default-config.json (news). The data file is the source of truth: edit it, then update this page. -->
# The news ticker

The bible's chapter 14 gives ticker headlines in capitals ("OPENAYE RAISES $40B TO BUY CHIPS FROM COMPANY THAT INVESTED $40B IN OPENAYE") and chapter 17 lists a "company chat and news ticker". The company chat is what the lab's own people say about the lab; the ticker is what the world says about everyone. `NewsTicker.Headlines` is a read-only view of the event log and the date. Nothing is saved and no rule changes (see QUESTIONS Q255 to Q257).

## What makes a headline

14 headline lines in the data, one per kind of public event, each with placeholders (`{lab}`, `{subject}`, `{model}`, `{money}`, `{amount}`, `{ref}`, `{sector}`, `{era}`, `{world}`, `{generation}`, `{rival}`, `{ending}`) and an optional `pick` for events that come in two kinds. The first line that fits wins. The result is upper-cased.

| Event | Headline (shown in capitals) |
|---|---|
| A new era | A new era begins: {era} |
| World news | {world} |
| Gauntlet crown | {ref} takes the Gauntlet crown for {lab}. {lab} thanks everyone, especially the GPUs |
| Incident | {lab}'s {model} had an incident. {lab} calls it a learning opportunity |
| A release | {lab} releases {model}. Early reviews: 'it is a model' |
| Bankruptcy | {lab} goes bust. Its GPUs go to a better home |
| An acquisition | {lab} buys {subject} for {money}. Regulators are 'looking into it' |
| A funding round | {lab} raises {money}. Valuation based on vibes |
| A rival joins | {lab} enters the race with {money} and a plan |
| The keynote | {generation} unveiled. It was sold out before the presenter walked on stage |
| Board drama | {lab}'s board fires its boss on Friday. The boss is back on Tuesday |
| An ending | {lab} reaches an ending: {ending} |
| A lab's first chip | {lab}'s first in-house chip works. It runs Minesweeper at 4,000 frames per second. Training support: next quarter |
| Any later chip | {lab} has a new chip design that works. The foundry sends flowers |

`{money}` is the event's amount the way a headline says it: $12, $340K, $1.5M, $40B, $2.2T (whole numbers, rounded down). Every lab's events are on the ticker, not just the player's. Anything a placeholder cannot find falls back to the raw text or "A LAB".

## The ticker

- `Headlines(config, state, max)` gives the newest headlines first, up to `max` (8 unless the screen asks for another number, `defaultHeadlines`).
- When fewer than **5** (`minHeadlines`) real headlines exist it is padded with filler: eight lines such as "LOCAL DATA CENTRE NOW USES MORE POWER THAN LOCAL COUNTRY" (from 2018) and "AI WILL TAKE ALL JOBS, SAYS CEO HIRING 4,000 PEOPLE". A filler line runs from its `fromYear`, is never repeated on one ticker and is picked from the date, so the same week gives the same ticker.
- `NewsTicker.About` gives the headline for one event, for a screen that wants to flash a headline the moment it happens.

## Notes

- Nothing is saved, so there is no schema change and the pinned golden game did not move.
- The validator checks every headline's event kind, text, pick and placeholders, and that filler has no placeholders.
