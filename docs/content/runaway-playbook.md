# Runaway playbook

Lab-wide response when agent blow-ups stack (bible ch. 10; `runawayPlaybook.*`; Q453). Lab fields `playLevel`, `playArmed`, `playLeft`, `playArmUntil` (schema 131). Only the player's lab.

`SetPlaybook` sets the posture 0–3. `ArmPlaybook` needs safety staff and a cooldown, and keeps the playbook armed for several weeks. While armed, blow-up chance and trust loss from blow-ups are softened. Events: `PlaybookSet`, `PlaybookArmed`.