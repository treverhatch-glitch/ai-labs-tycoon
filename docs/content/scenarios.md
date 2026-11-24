<!-- Generated from Rules/Data/default-config.json (scenarios). The data file is the source of truth: edit it, then update this page. -->
# Scenarios

The bible's chapter 15 lists "Scenarios: start as a named archetype in a specific year" under what waits for later. The pieces were already there (the start-era picker, founders, styles, home blocs and rival choices), so a scenario is a named bundle of them (see QUESTIONS Q284 and Q285).

`Scenarios.List(config)` gives the list for a picker. `Scenarios.Options(config, id, seed)` returns the `NewGameOptions`, and `Scenarios.Start(config, id, seed)` starts the game. An unknown scenario throws, like an unknown era does. Nothing is saved: a scenario is only a way to fill in the options, so a scenario game is exactly the game the same options would make by hand.

| Scenario | Year | Founder | Style | Home | Rivals |
|---|---|---|---|---|---|
| The Professor's Grant | 2009 | Professor | Sniper | US | all |
| The Dropout's Deck | 2012 | Dropout | Wildcard | US | all |
| The Defector's Lawsuit | 2017 | Defector | Shotgun | US | all (OpenAye is the enemy) |
| The Heir's Gambit | 2022 | Heir | Shotgun | Gulf | all |
| The Dial-Up Prologue | 1995 | Professor | Sniper | UK | all |
| The Quiet Lab | 2024 | Professor | Sniper | Nordics | all |
| The Two-Horse Race | 2009 | Dropout | Shotgun | US | OpenAye and DeepSneak only |

Each also has a lab name, a leader, a family name and a blurb for the picker. The validator checks that the era is startable and that the founder, style, bloc and rivals exist.

## Notes

- Open: a scenario with its own goals (the campaign goals of item 37 already work in every era), scenarios with a starting problem (a lawsuit already in progress), the picker screen (Unity), the 1956 prologue.
