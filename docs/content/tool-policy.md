# Tool policy for agents

Which tools a model's agents may call (bible ch. 6; `toolPolicy.*` in the data; Q449). One saved field per model (`toolMask`; schema 127). Only the player's lab. Needs agents on a released model.

`SetToolPolicy` (model, mask 0–7: web, code, data) costs cash. Each allowed bit lifts agent revenue and adds weekly incident and blow-up risk; at delegated autonomy or higher it also accrues human-queue items each week (`humanQueue.*`).

Events: `ToolPolicySet`.