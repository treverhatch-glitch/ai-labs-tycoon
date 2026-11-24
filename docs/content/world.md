<!-- Generated from Rules/Data/default-config.json ("world"). The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when a bloc, site, power deal or export rule id from the data is missing here. All names are parody names. -->
# The world map

Where the lab builds. The home HQ is always in the lab's home bloc; every other place is a **site**: a plot of land, a slice of the local grid, and a town that may or may not like data centres. Hardware sits in **holdings** (the HQ, a hall you built, or a colocation lease), and every holding draws power.

How it works, in one paragraph: buy a plot (one per lab per site), build a hall on it (kW of capacity, takes weeks, shortened by Infrastructure staff, may be delayed by a town vote), then buy or move GPUs in. The grid at each site is finite and shared by every lab; a power deal adds private capacity. Power is billed weekly at the site price times the site's cooling factor. Heatwaves in summer throttle sites. Governments watch the biggest and loudest labs: attention rises with the kW you run, your market share and your incidents, and climbs from a Letter to a Hearing, an Audit and finally a Freeze, which blocks every purchase in that bloc.

## Blocs

| Id | Name | Agency | Strictness | Flavour |
| --- | --- | --- | --- | --- |
| `us` | United States | Department of Chips & Commerce | 1000‰ | Deep capital. Hearings on live TV. |
| `eu` | European Union | Office of the Very Big AI Act | 1200‰ | The strictest paperwork on Earth. |
| `uk` | United Kingdom | Frontier Safety Institute | 900‰ | Safety institutes, and a very nice summit. |
| `nordics` | Nordic Countries | Nordic Green Grid Council | 700‰ | Cheap green power. Please recycle. |
| `china` | China | Ministry of Industrial Harmony | 1300‰ | Cheap power. The state makes requests. |
| `gulf` | Gulf States | Sovereign Compute Authority | 400‰ | Sovereign money and very few rules. |
| `singapore` | Singapore | Smart Nation Compute Board | 800‰ | A neutral hub with a very small map. |
| `japan` | Japan | Robotics & Silicon Agency | 800‰ | Precise. Robots come later. |
| `thailand` | Thailand | Eastern Seaboard Investment Office | 600‰ | Cheap land, hot weather, friendly paperwork. |

## Sites

| Id | Name | Bloc | Opens | Plots | Land | Grid | Power/kW/week | Cooling | Heatwave/yr | Town vote | Latency |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `us-oregon` | Columbia River, Oregon | `us` | 1995 | 3 | $120k | 1,500 kW | $14 | 1000‰ | 30‰ | 100‰ | 20‰ |
| `us-virginia` | Data Alley, Virginia | `us` | 1995 | 4 | $200k | 1,200 kW | $22 | 1100‰ | 60‰ | 150‰ | 0‰ |
| `us-texas` | Texas Plains | `us` | 1998 | 4 | $80k | 2,500 kW | $16 | 1300‰ | 150‰ | 50‰ | 40‰ |
| `eu-ireland` | Dublin Docklands, Ireland | `eu` | 1997 | 3 | $180k | 800 kW | $26 | 950‰ | 10‰ | 200‰ | 10‰ |
| `nordics-iceland` | Reykjanes, Iceland | `nordics` | 2000 | 3 | $60k | 3,000 kW | $8 | 850‰ | 0‰ | 100‰ | 150‰ |
| `singapore-jurong` | Jurong, Singapore | `singapore` | 1999 | 2 | $300k | 600 kW | $30 | 1350‰ | 100‰ | 50‰ | 30‰ |
| `thailand-seaboard` | Eastern Seaboard, Thailand | `thailand` | 2003 | 3 | $40k | 1,000 kW | $12 | 1400‰ | 200‰ | 80‰ | 90‰ |
| `china-guizhou` | Guizhou Mountains, China | `china` | 2005 | 4 | $50k | 4,000 kW | $10 | 1000‰ | 40‰ | 20‰ | 60‰ |
| `japan-hokkaido` | Hokkaido, Japan | `japan` | 2008 | 2 | $150k | 2,000 kW | $20 | 900‰ | 10‰ | 120‰ | 50‰ |
| `gulf-desert` | Desert Campus, UAE | `gulf` | 2015 | 6 | $100k | 20,000 kW | $9 | 1450‰ | 300‰ | 10‰ | 120‰ |
| `us-tennessee` | Valley Reactor Site, Tennessee | `us` | 2024 | 2 | $400k | 100,000 kW | $11 | 1050‰ | 40‰ | 250‰ | 30‰ |

Cooling above 1000‰ is what a hot site adds to the power bill; the Liquid Cooling tech shrinks only that excess. Latency slows serving (customers) but never training.

## Power deals

| Id | Name | Opens | Adds | Cost | Weeks | Price/kW/week | Attention |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `grid-upgrade` | Grid upgrade | 1995 | 500 kW | $150k | 12 | $0 | 1000‰ |
| `gas-turbines` | Gas turbines on site | 2015 | 20,000 kW | $6.0M | 26 | $13 | 1200‰ |
| `solar-battery` | Solar with batteries | 2018 | 10,000 kW | $5.0M | 30 | $7 | 700‰ |
| `reactor-restart` | Restart an old reactor | 2024 | 200,000 kW | $400.0M | 104 | $6 | 1300‰ |

## Halls and colocation

- A hall costs $250 per kW to build and $3 per kW per week to keep; at least 50 kW. Build time is 16 weeks plus one per 250 kW, never under 4 weeks, shortened 30‰ per Infrastructure engineer (at most 600‰).
- A town vote can delay a hall by 8 weeks. The chance is the site's town-vote chance plus the lab's attention in that bloc divided by 4, at most 900‰.
- Selling land refunds 600‰ of the price. Land with hardware or a hall under construction cannot be sold.
- Colocation: rent up to 200 kW per lab per site at the power price plus $20 per kW per week, with no build time and no land.

## Heatwaves

From month 6 to month 8, each site a lab occupies rolls once a month. A heatwave lasts 4 weeks and throttles compute at that site by 400‰ (halved with 300‰ of cooling tech).

## Government attention

Each lab has an attention score from 0 to 1000 per bloc. Every month it gains (kW the lab runs there past the notice size of the year, share of the market above the dominance line, incidents) and decays 100‰. Policy staff reduce the gain. Levels: Letter at 400, Hearing at 600, Audit at 800, Freeze at 950. A Freeze lifts below 800; each level drops 100 under its entry point.

| Year | Notice size |
| --- | --- |
| 1995 | 500 kW |
| 2009 | 5,000 kW |
| 2017 | 50,000 kW |
| 2022 | 300,000 kW |
| 2026 | 1,000,000 kW |

## Export rules

Buying or moving hardware above a rule's tier into a destination bloc is refused when a rule applies to the lab's home bloc. Hardware already in place keeps running.

| Id | Name | From | Home blocs | Destination | Allowed up to tier |
| --- | --- | --- | --- | --- | --- |
| `chips-act-1` | Chips Act, first round | 2022 | `us`, `eu`, `uk`, `japan` | `china` | 4 |
| `chips-act-2` | Chips Act, Gulf round | 2023 | `us` | `gulf` | 5 |
| `chips-act-3` | Chips Act, second round | 2025 | `us`, `eu`, `uk`, `japan` | `china` | 3 |
