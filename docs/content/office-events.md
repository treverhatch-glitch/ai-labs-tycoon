<!-- Generated from Rules/Data/default-config.json (office). The data file is the source of truth: edit it, then update this page. -->
# Office events

The bible's chapter 14 lists "event ideas" with no right answer: an intern leaks the model's instructions, the office cat improves a benchmark, a startup wraps your model, the safety report leaks before a funding round. Now and then one happens to the **player's** lab and waits on the desk for an answer. Rivals have none. Nothing is rolled from a dice stream (see QUESTIONS Q242 to Q245).

## When

- Nothing happens before game week **26** (`firstAfterWeeks`; a game that starts in a later era is already past it).
- With none waiting, each week has a **3%** chance (`chancePermille` 30), worked out from the week, the lab and the game's seed.
- One event waits at a time. It waits **8 weeks** (`expireWeeks`) and then blows over: no effect at all, just a line in the log.
- Some events need something first: a model on sale, somebody on the safety team, anyone on the payroll, a ready chip design of the lab's own (`OwnChip`), or rented GPUs on the books (`RentedCompute`). The list goes round in turn before an event repeats.

## Answering

`AnswerOfficeEvent` takes the choice. It is refused, changing nothing, when nothing is waiting (`NoOfficeEvent`) or the choice is not one of the event's. Every event has three answers; one has an answer that does nothing.

| Effect | What it does |
|---|---|
| Reputation | Points of reputation, kept inside 0 to 1000 |
| Culture | Points of safety culture |
| Morale | Points of morale for everyone on the payroll, kept inside 0 to 1000 |
| ResearchWeeks | Weeks of the lab's research output, as research points. A loss never goes below none |
| Attention | Points of the **home** government's attention to the lab, kept inside 0 to 1000. A loss eases it |
| CashWeeks | Weeks of what the lab pays to stand still (never under **1,000** a week, `minWeekCost`). A gain is a grant; a loss is never more than the cash the lab has, so an event cannot bankrupt it |

## The nineteen events

| Event | Needs | Answers |
|---|---|---|
| The public repository | nothing | rotate every key; call it transparency; fork it with a licence |
| The cat and the benchmark | anyone | credit the cat; rerun without it; give the cat a desk |
| The wrapper | a model on sale | send the lawyers; offer a partnership; ship a better wrapper |
| The leaked report | safety staff | publish in full; say it was a draft; hunt the leaker |
| The town vote | anyone | fund a park; hire a land lawyer; promise quiet fans |
| The open letter | nothing | sign; decline; publish your own |
| The earnings question | nothing | yes; no; talk about the weather |
| The drawings | a model on sale | ask about the door; frame them; move the door |
| Tuesday | a model on sale | investigate; market it as Tuesday Mode; leave it |
| The union joke | anyone | join in; ban it; negotiate |
| The fake video | nothing | honour it; debunk it; ignore it |

## Round three

Six more events from the bible's list (Q287); they are drawn like the others and sit at the end of the round.

| Event | Needs | Answers |
|---|---|---|
| The goodbye (`researcher-leaves`) | anyone | counter-offer; throw a send-off; walk them out |
| Friday and Tuesday (`ceo-friday`) | nothing | comment on the drama; say nothing; open a recruiting thread |
| What off means (`model-off`) | a model on sale | explain it honestly; change the subject; log and escalate |
| The reply (`husk-lol`) | a model on sale | ride the wave; scale up quietly; block him |
| The first benchmark (`minesweeper`) | a ready chip design | post the video; keep it in the lab; start a leaderboard |
| The friendly note (`landlord-note`) | rented GPUs | ask for an audit; shift to other capacity; read it as a compliment |

Changing the subject when the model asks what off means adds corruption to every model on sale; telling it the truth takes some away.

## Release notes

When one of the player's models goes on sale for the first time, the lab is asked how the release notes should read (bible ch. 14, "let the model be funny: its mistakes, its release notes"; Q290). The three events have `trigger` `Release`, so the weekly chance never draws them; they come round in turn. Putting an already-released model into another sector poses nothing, an event already waiting is never replaced (unlike hearings and the escape crisis), and rivals have none.

| Event | Answers |
|---|---|
| The release notes (`notes-changelog`) | let marketing write them; publish the honest version; let the model write them (it adds corruption) |
| Known issues (`notes-known-issues`) | publish all forty; publish the top three; cut the section |
| The joke in the notes (`notes-joke`) | keep it; take it out; turn it into a feature |

## Notes

- Saved per lab: the event waiting, its deadline and the events seen in this round (schema 33).
- `OfficeEvents.Waiting` gives a screen the event and the weeks left. Events, answers and lapses are in the log (`OfficeEventPosed`, `OfficeEventAnswered`, `OfficeEventLapsed`) and the company chat has a line for the first two.
- The autopilot never answers, so its events lapse and cost nothing.
- The pinned golden game moved (the player's lab is posed events in its year, and the save has new fields); the cause was checked, see the commit message.
| The poetry prize | a model on sale | accept the prize; decline politely; ask for a recount |
| The essay | a model on sale | publish the essay; fix the refusal; frame it for the lobby |
| The honest answer | a model on sale | let them print it; ask for a correction; thank the chatbot |
| The senator's question | anyone | send a briefing; send a one-page explainer; say "both" |
| The signed GPU | nothing | bid for it; watch from the back; bid it up, then stop |
| The person in the next room | nothing | post it; say nothing; send it to the regulator |
| The five per cent | safety staff | give them five per cent; offer half; say not this quarter |
| The investor question | safety staff | explain it properly; promise to look at costs; rename it Trust |

The last eight are the second batch (QUESTIONS Q262 and Q263), from the bible's longer list of event ideas and its "more events" section.

## Hearing questions

An event with `trigger: "Hearing"` is never drawn by the weekly chance. A hearing at the home government poses one instead (see [hearing-questions.md](hearing-questions.md)).

An event with `trigger: "Escape"` is posed by a rung-5 incident instead (see [escape-crisis.md](escape-crisis.md)), and may name a `lapseChoice` that is taken for the player when time runs out.
