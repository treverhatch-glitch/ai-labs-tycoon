# Foundation

An endowed fund that gives money away for goodwill (`foundation.*` in the data; Q388). Seven saved lab fields (`foundFund`, `foundGiven`, `foundFocus`, `foundPayout`, `foundGoodwill`, `foundSwitch`, `foundDrip`; schema 92). Only the player's lab.

- `EndowFoundation` (thousands, first gift at least 100,000, with a cause) puts cash into the fund through the capital line. Not a tax deduction.
- The fund earns 0.2% a week. `SetFoundationPayout` sets giving to 0 (default), 1 (0.2%), 2 (0.4%) or 3 (0.8%) of the fund a week. 500 given is a point of goodwill, out of 1,000; goodwill fades 3% a week.
- Cause 1 safety: scandal damping of goodwill/2 in permille, at most 250 (works through `PressSystem.Damp`). Cause 2 education: a reputation point every 6 weeks at goodwill 150 or more. Cause 3 open science: goodwill times 0.3 off the strength of incident claims (`Litigation.OnIncident`).
- `SetFoundationFocus` loses 30% of the goodwill and locks the cause for 24 weeks. `WindUpFoundation` returns 70% of the fund, costs 15 reputation and clears the goodwill. A fund giving itself empty ends (`FoundationRanDry`).

Events: `FoundationEndowed` (SubjectId the cause), `FoundationReported` every 12 weeks (SubjectId the goodwill, Amount given in all), `FoundationRefocused`, `FoundationWoundUp`, `FoundationRanDry`.
