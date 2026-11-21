# Devices

A hardware side line (`devices.*` in the data; Q393). Seven saved lab fields (`devLine`, `devStage`, `devWeeks`, `devQuality`, `devTier`, `devSold`, `devBase`; schema 95). Only the player's lab; one line at a time.

| Command | What happens |
|---|---|
| `DesignDevice` (line, test weeks) | line 1 speaker (from era 3), 2 glasses (era 5), 3 home robot (era 6); needs a released model of capability 100; pays the design cost; 12/16/24 weeks plus the testing |
| `SetDevicePrice` (tier) | 1 low (70% of the margin, 130% of the units), 2 middle, 3 high (140% of the margin, 75% of the units) |
| `WithdrawDevice` | ends the line, on sale or in design |

Quality is the base 300, plus up to 400 from the model's capability, plus 70 for each week of testing (up to 3), to a cap of 1000. At the launch a device under 600 flops: it keeps 30% of its demand and costs reputation (damped by the press room); a device at or over the bar is a hit and adds reputation. On sale it earns units times the margin each week on the revenue line, and its demand fades 2% of the start each week to a floor of 10%. A device under 450 that has been on sale 8 weeks is recalled: it costs 40 for each unit sold (scaled up for the bigger lines) on the fines line, and some reputation.

Events: `DeviceDesigned`, `DeviceLaunched` (Pick 1 a hit, 0 a flop), `DeviceRecalled`, `DeviceWithdrawn`.
