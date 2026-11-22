# Users' data

The flywheel from the lab's own users (bible ch. 6, "Your own users"; `userData.*` in the data; Q428). Five saved lab fields (`userPolicy`, `userSince`, `userBanked`, `userTotal`, `userComplaints`; schema 116). Only the player's lab. A lab that never sets a policy collects nothing.

`SetUserData` (0 none, 1 opt-in, 2 on by default). A change locks the policy for 13 weeks. Each week, last week's revenue divided by 25,000 is the data the lab could collect, and it collects 30% of that under opt-in or all of it by default. It arrives as raw units of source 4 (see the data pipeline) and stops when the lab is at its stock cap. Users' data is cleaned like any other (70% survives) and is worth 90% of a unit to a model.

- A default policy draws a privacy complaint every 26 weeks if the lab has collected 40 or more units since the last one: a fine of 1,500 for each of those units, and 15 trust lost in every sector the lab sells in. Opt-in never draws one.
- Moving off a default policy to opt-in or none deletes 70% of the users' data held, raw and clean. Moving up to default keeps what is there.

Events: `UserDataChanged`, `UserDataComplaint`, `UserDataDeleted`.
