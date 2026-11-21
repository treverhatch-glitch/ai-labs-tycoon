# Launch watch

A desk that watches rival launches (`launchWatch.*` in the data; Q404). Six saved lab fields (`lwOn`, `lwRival`, `lwLead`, `lwWait`, `lwHits`, `lwAnswers`; schema 104). Only the player's lab. It only reads what rivals did (their released models and release events): it never changes what they do.

| Command | What happens |
|---|---|
| `OpenWatchDesk` | costs 100,000, then 12,000 a week |
| `CloseWatchDesk` | stops the bill; a launch still waiting is taken as held |
| `AnswerLaunch` (1, 2 or 3) | 1 counter-programme: 4,000 cash per person, the loss lands at 25%; 2 hold: the whole loss; 3 own the safety story: needs the lab's best released model at alignment 600 or more, and wins 10 reputation with no loss |

With the desk open, each rival release of the week before is read. If the rival model is stronger than the lab's best released model (none counts as 0) the launch waits for an answer, with the rival's lead in permille; one waits at a time and a second launch is not noticed. A launch left for 3 weeks lands as held (`Pick 0`). The loss is 20 reputation (damped) at an even lead, from half that at a tiny lead to one and a half times at a huge one. A rival launch the lab already beats earns 2 reputation.

Events: `WatchDeskOpened`, `RivalLaunchSeen` (Pick 1 it beats the lab, 0 the lab beats it), `LaunchAnswered` (Pick 1 counter-programmed, 2 held, 3 safety story, 0 nobody answered).
