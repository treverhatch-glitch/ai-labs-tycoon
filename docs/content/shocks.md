# World shocks

The big dated events of each era landing on the player's lab (`shocks.*` in the data; Q396). Six saved lab fields (`shockKind`, `shockRef`, `shockLeft`, `shockBraced`, `shockRode`, `shockCountered`; schema 98). Only the player's lab.

`Shocks.KindOf` sorts the world events by id: a squeeze (talent war, Friday firing, card shortage, sold-out chip, heatwave, everyone home), a backlash (teen chatbot, first crash, too dangerous, pause letter, invented cases, deleted tests, polite flash crash, fridge recall, bank wobble), a boom (glass rectangle, phone assistant, quiz show, contest winner, attention paper, code autocomplete, hundred million, cheap challenger, work as a service, co-author). Other news stays news.

When one lands (`EraSystem.RunWeek`, after the news event) the lab has 8 weeks to answer with `AnswerShock`:

| Answer | Squeeze (cash 15,000 a head, morale 80) | Backlash (reputation 40, damped) | Boom (reputation 30) |
|---|---|---|---|
| 1 Brace (20,000 a head) | takes 25% of the hit | takes 25% | gains double |
| 2 Ride | takes all of it | takes all of it | gains 30 |
| 3 Counter (best released model capability 500) | works: no hit, +30 reputation; fails: takes double | the same | works: gains triple; fails: loses double (damped) |

A lab held up as an example by its creed (Q395) takes 70% of a backlash's reputation hit and one whose creed is a joke 140%, before the answer scales it. An answer not given is a ride (`Pick 5`); a second shock landing first rides the first. Events: `ShockStruck`, `ShockAnswered` (Pick 1 braced, 2 rode, 3 countered and it worked, 4 countered and it failed, 5 nobody answered).
