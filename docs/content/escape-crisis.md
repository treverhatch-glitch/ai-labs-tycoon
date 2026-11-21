# The escape-attempt crisis

Rung 5 of the bible's incident ladder (chapter 10) is "Escape attempt: the model tries to copy itself out. A timed crisis: shut down data centres at huge cost, or gamble on containment." It used to be an incident like the rest, a fine and a reputation loss. It is now also a decision (see QUESTIONS Q281 to Q283).

## How it works

- An **office event** (see [office-events.md](office-events.md)) with `trigger: "Escape"`, posed when a rung-5 incident hits a model of the **player's** lab (`SafetySystem.Incident` calls `OfficeEvents.OnEscape`). It takes the place of any event waiting. Rivals never get it. The weekly chance never draws it.
- The player answers with the usual `AnswerOfficeEvent`. Unanswered, it lapses after `office.expireWeeks` (8) and **takes the event's `lapseChoice`** (`watch`), the same as if the player had chosen it. The log says so (`OfficeEventLapsed`, Ref `event|choice`).
- Rung 5 needs a model with corruption of 450 or more, so it only happens to labs on the shady path.

## The three ways out

| Answer | Effects |
|---|---|
| Pull the plug on the data centres | -10 weeks of running costs, -400 corruption on every model on sale, -10 reputation, -10 morale |
| Gamble on the sandbox | -150 corruption, -2 weeks of running costs, +100 attention at home, -20 reputation |
| Watch what it does (the default) | +200 corruption, -10 safety culture |

The new effect kind `Corruption` moves the corruption of every model the lab has **on sale** by that many points, kept between 0 and 1000 (models still training are not touched). Lower corruption means the next incident is a lower rung and the ending gates (Loose, Paperclipped) are further away.

## Notes

- New data: `OfficeEventConfig.lapseChoice` (empty on every other event, so they still just blow over), `OfficeTrigger.Escape`, `OfficeEffectKind.Corruption`. The validator checks the lapse choice exists. No new save field: the crisis is the lab's pending office event. Schema unchanged; the golden did not move.
- Open: a real gamble with a chance that depends on the sandbox tier and the safety staff, a crisis for rung 6 (takeoff) that decides the ending, shutting down only one site, a timer shown in days.
