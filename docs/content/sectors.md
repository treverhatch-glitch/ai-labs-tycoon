<!-- Generated from Rules/Data/default-config.json. The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when an id from the data is missing here. All names are parody names. -->
# Sectors

Every sector belongs to a family, and sector trust carries between sectors of a family: see [trust.md](trust.md).

What each sector sells to. `pool/week` is the pool in the year it opens; it then grows at `growth` a year, slowed by `decay` each year after. A model needs `min` capability to enter and stops gaining at `sat`.

How each sector fails when a model goes wrong in it is on [sector-flavours.md](sector-flavours.md).

## Dial-up Days (1995)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `game-ai` | Game AI Licensing | 1995 | game studios who need enemies, sidekicks and crowds | $50k | 6% | 90% | 60 | 500 | 80/20 |
| `game-bots` | Chess & Puzzle Bots | 1995 | boxed-software publishers and chess-computer makers | $25k | 4% | 90% | 90 | 450 | 90/10 |
| `expert-systems` | Expert Systems | 1996 | banks, insurers and factories | $77k | 10% | 85% | 200 | 1,500 | 50/50 |
| `spam-search` | Spam Filtering & Search | 1999 | internet providers and web portals | $488.3k | 25% | 85% | 450 | 3,000 | 60/40 |
| `machine-translation` | Machine Translation | 2001 | publishers, travel sites and embassies | $120k | 15% | 85% | 600 | 4,000 | 70/30 |
| `web-ads` | Ad Targeting & Recommendations | 2003 | portals, shops and a very large search company | $300k | 20% | 85% | 800 | 6,000 | 60/40 |

## The Punchline (2009)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `doc-reading` | Document & Digit Reading | 2009 | post offices, banks and tax offices | $600k | 12% | 85% | 1,200 | 6,000 | 70/30 |
| `fraud-detection` | Fraud Detection | 2010 | card networks and banks | $900k | 15% | 85% | 1,800 | 8,000 | 50/50 |

## It Sees (2012)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `image-recognition` | Image Recognition | 2012 | photo sites, retailers and hospitals | $2.0M | 25% | 85% | 4,000 | 16,000 | 75/25 |
| `voice-assistants` | Voice Assistants & Dictation | 2013 | phone makers and carmakers | $3.0M | 20% | 85% | 3,500 | 15,000 | 60/40 |
| `autonomous-driving` | Autonomous Driving | 2015 | carmakers and taxi fleets | $6.0M | 25% | 85% | 8,000 | 30,000 | 40/60 |

## It Writes (2017)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `code-completion` | Code Completion | 2018 | developers and software companies | $8.0M | 30% | 85% | 15,000 | 50,000 | 70/30 |
| `writing-tools` | Writing & Marketing Copy | 2019 | agencies, publishers and marketers | $6.0M | 30% | 85% | 12,000 | 40,000 | 50/50 |

## The Chat Boom (2022)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `consumer-assistant` | Consumer Assistant | 2022 | everyone with a phone | $40.0M | 20% | 85% | 28,000 | 100,000 | 60/40 |
| `image-design` | Image & Design | 2022 | agencies, photographers and marketers | $20.0M | 15% | 85% | 25,000 | 90,000 | 70/30 |
| `customer-support` | Customer Support | 2023 | retailers, telecoms and airlines | $30.0M | 20% | 85% | 30,000 | 110,000 | 45/55 |
| `legal-research` | Law | 2023 | law firms and courts | $10.0M | 15% | 85% | 40,000 | 130,000 | 35/65 |
| `music-audio` | Music & Audio | 2023 | labels, studios and streaming services | $8.0M | 15% | 85% | 28,000 | 90,000 | 70/30 |
| `video-film` | Video & Film | 2023 | studios, advertisers and creators | $25.0M | 20% | 85% | 40,000 | 150,000 | 80/20 |

## Agents (2024)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `coding-agents` | Coding Agents | 2024 | software companies and your own lab | $100.0M | 25% | 85% | 60,000 | 200,000 | 60/40 |
| `enterprise-knowledge` | Enterprise Knowledge Work | 2024 | large companies | $80.0M | 20% | 85% | 55,000 | 190,000 | 40/60 |
| `companionship` | Companionship | 2024 | lonely people | $20.0M | 20% | 85% | 45,000 | 130,000 | 70/30 |
| `government-services` | Government Services | 2025 | ministries and city halls | $30.0M | 15% | 85% | 62,000 | 220,000 | 30/70 |
| `cybersecurity` | Cybersecurity & Defence | 2025 | militaries, banks and governments | $40.0M | 20% | 85% | 66,000 | 230,000 | 30/70 |
| `finance-trading` | Finance & Trading | 2025 | funds and banks | $60.0M | 20% | 85% | 68,000 | 240,000 | 45/55 |
| `science-medicine` | Science & Medicine | 2025 | pharma, labs and hospitals | $50.0M | 20% | 85% | 69,000 | 260,000 | 50/50 |

## Harnesses (2026)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `agent-workforce` | Agent Workforce | 2026 | anyone who would rather buy finished work | $300.0M | 30% | 85% | 105,000 | 420,000 | 55/45 |
| `agri-logistics` | Agriculture & Logistics | 2026 | farms, ports and warehouses | $60.0M | 15% | 85% | 100,000 | 350,000 | 65/35 |
| `devices` | Devices & Wearables | 2027 | phone makers and glasses brands | $100.0M | 20% | 85% | 100,000 | 330,000 | 60/40 |
| `ai-research-services` | AI Research Services | 2027 | other labs, quietly | $150.0M | 25% | 85% | 107,000 | 480,000 | 65/35 |

## Bodies (2029)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `robotics-industry` | Robotics & Industry | 2029 | factories and warehouses | $500.0M | 25% | 85% | 158,000 | 700,000 | 50/50 |
| `household-robots` | Household Robots | 2030 | anyone with a laundry basket | $300.0M | 20% | 85% | 152,000 | 650,000 | 50/50 |

## Upward (2033)

| Id | Name | Opens | Buyers | Pool/week | Growth | Decay | Min | Sat | Capability vs safety |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `machine-science` | Machine-Speed Science | 2033 | universities, pharma and governments | $1.5B | 25% | 85% | 220,000 | 1,000,000 | 45/55 |
| `space-compute` | Space Compute | 2034 | launch companies and defence ministries | $800.0M | 20% | 85% | 222,000 | 1,100,000 | 45/55 |

