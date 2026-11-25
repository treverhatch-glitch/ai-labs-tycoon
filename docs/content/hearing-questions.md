# Hearing questions

The bible's chapter 12 says a hearing is "a short dialogue scene: your answers move heat, trust and share price". A hearing at the lab's home government used to be a flat reputation hit. It now also puts a question to the player (see QUESTIONS Q272 to Q274).

## How it works

- The question is an **office event** (see [office-events.md](office-events.md)) with `trigger: "Hearing"` instead of `"Random"`. The weekly chance never draws it. When the lab's home government reaches the Hearing level, `OfficeEvents.OnHearing` poses one, answered with the existing `AnswerOfficeEvent` command.
- The automatic reputation hit (and the press beat) is unchanged. The answer is on top of it.
- A waiting office event lapses and is logged (`OfficeEventLapsed`) so the hearing has the floor. An unanswered question lapses after `office.expireWeeks` and changes nothing.
- The questions rotate by a hash of the seed, the week and the lab: all four are asked before one repeats.
- Only the player's lab, and only a hearing at its home government. A hearing abroad, a rival's hearing, and data with no hearing questions behave as they did.

## The four questions

| Question | Answers (effects) |
|---|---|
| The weapon question | Say what you tested (+15 reputation, +10 culture, -30 heat) / Promise a safeguard (+10 reputation, -50 heat, -10 culture) / Call it a hypothetical (-20 reputation, +60 heat) |
| The jobs question | Give the real forecast (+10 reputation, -10 morale, -20 heat) / Say it will make more jobs (+15 reputation, +20 heat, -10 culture) / Offer a retraining fund (-2 weeks of cash, -60 heat, +10 reputation) |
| The data question | Hand over the list (+20 reputation, -30 heat, +10 culture) / Cite trade secrets (+50 heat, -10 reputation, +10 morale) / Offer a closed briefing (-20 heat, +5 reputation, -1 week of cash) |
| The pause question | Yes, if the law says so (+20 reputation, -50 heat, -2 weeks of research) / No, and explain (-10 reputation, +40 heat, +15 morale) / Consult the board (+10 heat, +5 culture, -5 reputation) |

Heat is the home government's attention to the lab (the `Attention` effect). Trust is the lab's reputation.

## Notes

- No new save fields: the question is the lab's pending office event. Schema unchanged; the golden did not move.
- Open: a share-price effect (shares and valuation belong to the bubble work), questions that depend on what the lab has done (a recent incident, a leak), hearings abroad using that bloc's attention.
