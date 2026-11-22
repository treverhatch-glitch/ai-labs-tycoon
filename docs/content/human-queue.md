# Human escalation queue

When agent actions need a person in the loop (bible ch. 6; `humanQueue.*` in the data). Three saved fields on the lab (`humanQueue`, `humanBusy`, `humanCleared`; schema 128). Only the player's lab. Items accrue from agent tool policy; past capacity they cut agent lift and raise incidents.

`StaffHumanQueue` drains items with support people and keeps the desk busy for a time. `ClearHumanQueue` pays cash to clear items now. Each week support staff drain what they can without a command.

Events: `HumanQueueDeep`, `HumanQueueCleared` (Pick 1 staffed drain, 0 paid).