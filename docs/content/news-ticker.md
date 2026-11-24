<!-- Generated from Rules/Data/default-config.json (news). The data file is the source of truth: edit it, then update this page. -->
# The news ticker

The bible's chapter 14 gives ticker headlines in capitals ("OPENAYE RAISES $40B TO BUY CHIPS FROM COMPANY THAT INVESTED $40B IN OPENAYE") and chapter 17 lists a "company chat and news ticker". The company chat is what the lab's own people say about the lab; the ticker is what the world says about everyone. `NewsTicker.Headlines` is a read-only view of the event log and the date. Nothing is saved and no rule changes (see QUESTIONS Q255 to Q257).

## What makes a headline

47 headline lines in the data, one per kind of public event, each with placeholders (`{lab}`, `{subject}`, `{model}`, `{money}`, `{amount}`, `{ref}`, `{sector}`, `{era}`, `{world}`, `{generation}`, `{rival}`, `{ending}`) and an optional `pick` for events that come in two kinds. The first line that fits wins. The result is upper-cased.

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
| A tapeout | {lab} taps out {ref} for {money}. The foundry's cheque cleared faster than the design |
| A respin | {lab} respins {ref} for {money}. The first one is now 'a learning chip' |
| A yield crash | {lab}'s {ref} yield crashes: {amount} chips arrive. The foundry blames the weather |
| A foundry snub | The foundry bumps {lab} down the queue by {amount} weeks. It says it is nothing personal |
| A chip sale | {lab} buys {amount} {ref} chips from {subject}. The invoice has a lot of zeros |
| A loan | {lab} borrows {money}. The bank calls the lab 'confident' |
| A missed payment | {lab} misses a loan payment. A spokesperson says the cheque is in the post |
| A default | {lab} defaults on a loan, {money} owed. The bank 'remains optimistic' |
| A leak that runs (pick 1) | A story about {subject}'s {ref} reaches the press. {subject} suspects {lab}. {lab} suspects a coincidence |
| A leak that backfires (pick 0) | {lab} leaks a story about {subject}'s {ref} and it turns out to be about {lab}. Nobody is surprised |
| Poached staff | {lab} hires away {subject}'s {ref}. {subject} wishes them well, through a lawyer |
| Exposed Gauntlet scores | {lab}'s Gauntlet scores are exposed. {lab} calls it 'a different methodology' |
| A launch buzz | {model} launches to a gust of buzz. {lab} says it planned that |
| Crunch ends | {lab}'s team ends crunch time after {amount} weeks. The sofas are 'still warm' |
| An insurance claim | {lab}'s insurer pays out {money} on {ref}. The premium 'will be reviewed' |
| A dropped policy | {lab}'s insurer drops it after {amount} claims. 'It is not you, it is the incidents' |
| An offsite | {lab} takes the team on an offsite for {money}. Nobody will say what happened at the escape room |
| A red team | {lab} pays {money} for a red team to attack {ref}. The red team 'had a lovely time' |
| A finding | {lab}'s interpretability team finishes reading {ref}. It is 'mostly about lunch' |
| A pay change | {lab} changes its pay policy. The weekly salary bill is now {money} |
| A lapsed compliance office | {lab}'s compliance office loses its policy staff and drops from level {amount}. A spokesperson is 'fully compliant with the idea of it' |
| A paper | {lab} publishes a paper. Reviewers 'have notes' |
| A sublet change | {lab} changes how much compute it lets to outsiders. It now brings in {money} a week |
| An audit with a certificate | {lab} gets an outside safety certificate for {ref}. The auditors 'could not find the rest of it' |
| An audit that embarrassed the lab | Outside auditors publish the real safety score of {ref}. It is not the one on {lab}'s website |
| An audit that only published the score | Auditors publish the real score of {ref}. {lab} calls it 'a data point' |
| A certificate pulled | {lab} loses the safety certificate of {ref} after an incident. The sticker is being 'repurposed' |
| A new research focus | {lab} announces a new research focus. The old one is 'still in the building' |
| A dropped research focus | {lab} drops its research focus and will 'follow the science, wherever it is' |
| A hack week starting | {lab} puts its roadmap on hold for a hack week. Someone has built a toaster that files bug reports |
| A hack week ending | {lab}'s hack week ends. The toaster is 'a priority for v2' |
| Staff equity | {lab} gives its staff equity. Employees are 'cautiously optimistic about a number they cannot sell' |
| A retrained person | {lab} retrains a person for a new role. HR calls it 'a journey' |
| A chief named | {lab} names a chief. The org chart now has a box with a person in it |
| A chief stepping back | {lab} lets a chief step back. The box on the org chart is empty again |
| A chief walking out | {lab}'s chief walks out. The org chart is 'being reviewed' |
| An ethics board seated | {lab} seats an ethics board. It has already asked for a bigger room |
| An ethics board stepped down | {lab} steps its ethics board down. The press notices before the board does |
| An ethics board resigning | {lab}'s ethics board resigns. The minutes are 'still being drafted' |
| A campaign starting | {lab} launches a marketing campaign. The billboard is mostly logo |
| A campaign ending | {lab}'s marketing campaign wraps up. Customers hold it up against the billboard |
| Inference optimised | {lab} optimises its inference. The GPUs have been asked to do more with less |
| An on-call rota | {lab} puts its team on call. The pagers were ordered in bulk |
| Policy consultants hired | {lab} hires policy consultants. Their rate card has a section called 'access' |
| Policy consultants leaving | {lab}'s policy consultants head home. The invoices stay |
| A post-mortem summary | {lab} publishes a short note on what went wrong with {ref}. It uses the word 'learnings' |
| A full post-mortem | {lab} publishes a full post-mortem of {ref}, including the bit about its real alignment |
| A service-level pledge | {lab} pledges a service level to its customers. The uptime page is now a legal document |
| A pledge lowered | {lab} lowers its service-level pledge. The footnote grows |
| A pledge broken by load | {lab} misses its service level and refunds customers. Demand 'exceeded expectations' |
| A pledge broken by an incident | {lab} breaks its service-level pledge with an incident. The refund is a rounding error to nobody |
| A grant awarded | {lab} wins a research grant. The panel liked the abstract and loved the budget |
| A grant declined | {lab}'s grant application is declined. The panel found it 'ambitious, in places' |
| A grant delivered | {lab} delivers the papers its grant asked for. The panel is 'cautiously delighted' |
| A grant clawed back | {lab} misses its grant deadline and pays part back. The papers are 'nearly done' |

`{money}` is the event's amount the way a headline says it: $12, $340K, $1.5M, $40B, $2.2T (whole numbers, rounded down). Every lab's events are on the ticker, not just the player's. Anything a placeholder cannot find falls back to the raw text or "A LAB".

## The ticker

- `Headlines(config, state, max)` gives the newest headlines first, up to `max` (8 unless the screen asks for another number, `defaultHeadlines`).
- When fewer than **5** (`minHeadlines`) real headlines exist it is padded with filler: 26 lines (eight from the bible and 18 for the eras, Q293) such as "LOCAL DATA CENTRE NOW USES MORE POWER THAN LOCAL COUNTRY" (from 2018) and "AI WILL TAKE ALL JOBS, SAYS CEO HIRING 4,000 PEOPLE". A filler line runs from its `fromYear` to its `toYear` (0 for no end; the modem queue ends in 2003, the orbital launch delay never does), and every year from 1995 has at least five open, is never repeated on one ticker and is picked from the date, so the same week gives the same ticker.
- `NewsTicker.About` gives the headline for one event, for a screen that wants to flash a headline the moment it happens.

## Notes

- Nothing is saved, so there is no schema change and the pinned golden game did not move.
- The validator checks every headline's event kind, text, pick and placeholders, and that filler has no placeholders.
