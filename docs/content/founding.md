# Founding choices

Three choices shape the start of a game (bible ch. 5). All three are optional: pick none and the game is exactly what it was. The founder and the style are saved on the lab (`founderId`, `styleId`, save schema 22); their lasting effects are read from the config each time, so nothing else is saved.

## Founders

A founder changes the start (cash, hype, grant, one powerful enemy) and carries a lasting edge and a lasting cost.

| Founder | Start | Lasting edge | Lasting cost |
| --- | --- | --- | --- |
| `professor` | Reputation +150, grant 1.5x | Researchers earn 10% more points | Appeal in every sector -10% |
| `dropout` | Cash 1.3x, reputation +250 (hype) | Appeal in every sector +5% | Researchers earn 5% fewer points |
| `defector` | Cash 1.2x, reputation +100; `openaye` starts at a feeling of -600 (a feud) | Researchers earn 6% more points | One powerful enemy |
| `heir` | Cash 3x, reputation -50 | None | Every hire costs 15% more |

## Styles

A style lifts appeal in its focus sectors and lowers it in every other sector, so the lab is a sniper, a shotgun or a wildcard.

| Style | Focus sectors | Focus | Everywhere else |
| --- | --- | --- | --- |
| `sniper` | `expert-systems`, `doc-reading`, `code-completion`, `coding-agents`, `enterprise-knowledge`, `agent-workforce` | +25% | -8% |
| `shotgun` | `voice-assistants`, `image-recognition`, `writing-tools`, `consumer-assistant`, `image-design`, `video-film`, `music-audio`, `companionship` | +8% | -2% |
| `wildcard` | `fraud-detection`, `cybersecurity`, `science-medicine`, `machine-science` | +35% | -10% |

Founder and style appeal add up with the role and room perks as one boost and never fall below -50%.

## Home base

`NewGameOptions.HomeBlocId` picks the bloc the lab is incorporated in (any bloc of the world map); empty keeps the default.

A sold lab goes to its new owners without its founder or style; a lab founded again keeps both.
