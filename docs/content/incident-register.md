# Incident register

A voluntary register of incidents (`disclosure.*` in the data; Q421). Eight saved lab fields (`discOn`, `discOpen`, `discLeft`, `discPrep`, `discFindings`, `discFiled`, `discInspect`, `discModel`; schema 114). Only the player's lab. A lab that never joins has no register, and incidents open nothing.

| Command | What happens |
|---|---|
| `JoinRegister` | starts the register; the first inspection is 26 weeks off |
| `LeaveRegister` | leaves; refused while a report is waiting or being prepared |
| `FileReport` 1 | files the oldest waiting report now: +12 trust in the incident model's sectors, but a finding is left on file |
| `FileReport` 2 | a full report: needs 2 safety people, takes 3 weeks to prepare (one at a time); on completion clears a finding and, if the report was still waiting, closes it for +30 trust |

- With the lab on the register, each incident opens a report due in 4 weeks (up to 4 waiting). An incident beyond that is unreported and leaves a finding at once.
- A report that is not closed in its 4 weeks is late: a fine of 40,000 times the findings on file (up to 5), and another finding. A full report that was started too late to close it in time still clears its finding when it is done.
- Every 26 weeks an inspection reads the findings. At 3 or more it fines 60,000 for each finding and halves them; below that it finds nothing.

So a summary is quick and earns trust but stores up trouble; a full report is slow and needs staff but wipes a finding; and a lab that gets a string of incidents at once has to choose which to file how.

Events: `RegisterChanged` (Pick 1 joined), `ReportFiled` (Pick 1 summary, 2 full; Amount the trust), `ReportLate`, `InspectionHeld` (Pick 1 clean, 0 fined).

Together: an incident on a model with a conformity mark leaves one extra finding at once (`certifiedFindings` 1; Q423). In a settled functional lab with a senior safety person, a full report takes a week less (`orgFasterWeeks` 1; Q424).
