#!/bin/bash
# Local only: apply batch 15 code+config on cursor/era-systems-o-af2f before commit (avoid restore-golden-tip wiping WIP).
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
python3 <<'PY'
from pathlib import Path
# GameState schema + lab fields
p = Path('Rules/State/GameState.cs')
text = p.read_text()
text = text.replace('public const int CurrentSchema = 138;', 'public const int CurrentSchema = 143;', 1)
old = '''        public List<string> PriceSectors = new List<string>();
        public List<int> PriceTiers = new List<int>();
        /// <summary>The level of the revenue target the lab has committed to, 0 when none.</summary>
        public int RevenueTargetLevel;'''
new = '''        public List<string> PriceSectors = new List<string>();
        public List<int> PriceTiers = new List<int>();
        /// <summary>Week when the campaign clock stops (0 sandbox), schema 139.</summary>
        public int CampaignDeadlineTick;
        /// <summary>Law shape waiting for the next era (schema 140).</summary>
        public int LawShapePending;
        public int LawShapeDelta;
        public string LawShapeEra = "";
        /// <summary>Takeoff crisis cannot repeat before this tick (schema 142).</summary>
        public int TakeoffCooldownUntil;
        /// <summary>Scenario baggage id and picked flag (schema 143).</summary>
        public string BaggageId = "";
        public int BaggagePicked;
        /// <summary>The level of the revenue target the lab has committed to, 0 when none.</summary>
        public int RevenueTargetLevel;'''
if old in text:
    text = text.replace(old, new, 1)
old2 = '''            s.StrList("priceSectors", ref PriceSectors);
            s.IntList("priceTiers", ref PriceTiers);
            s.Int("revenueTargetLevel", ref RevenueTargetLevel);'''
new2 = '''            s.StrList("priceSectors", ref PriceSectors);
            s.IntList("priceTiers", ref PriceTiers);
            s.Int("campaignDeadlineTick", ref CampaignDeadlineTick);
            s.Int("lawShapePending", ref LawShapePending);
            s.Int("lawShapeDelta", ref LawShapeDelta);
            s.Str("lawShapeEra", ref LawShapeEra);
            s.Int("takeoffCooldownUntil", ref TakeoffCooldownUntil);
            s.Str("baggageId", ref BaggageId);
            s.Int("baggagePicked", ref BaggagePicked);
            s.Int("revenueTargetLevel", ref RevenueTargetLevel);'''
if old2 in text:
    text = text.replace(old2, new2, 1)
p.write_text(text)
# Simulation
p = Path('Rules/Sim/Simulation.cs')
t = p.read_text()
if 'public string BaggageId' not in t:
    t = t.replace('        public string HomeBlocId = "";\n    }', '        public string HomeBlocId = "";\n        /// <summary>Scenario baggage id for a new game (schema 143); empty for none.</summary>\n        public string BaggageId = "";\n    }', 1)
if 'TakeoffCrisis.RunWeek' not in t:
    t = t.replace('                    OfficeEvents.RunWeek(c, state, state.Labs[i]);\n                    ShadySystem.RunWeek(c, state, state.Labs[i]);',
                  '                    OfficeEvents.RunWeek(c, state, state.Labs[i]);\n                    TakeoffCrisis.RunWeek(c, state, state.Labs[i]);\n                    CampaignDeadline.RunWeek(c, state, state.Labs[i]);\n                    ShadySystem.RunWeek(c, state, state.Labs[i]);', 1)
p.write_text(t)
# CommandProcessor
p = Path('Rules/Sim/CommandProcessor.cs')
t = p.read_text()
if 'SetCampaignDeadline' not in t:
    t = t.replace('                case CommandKind.SetSectorPrice: return SetSectorPrice(c, state, lab, cmd);\n                case CommandKind.StaffHumanQueue:',
                  '                case CommandKind.SetSectorPrice: return SetSectorPrice(c, state, lab, cmd);\n                case CommandKind.SetCampaignDeadline: return SetCampaignDeadline(c, state, lab, cmd);\n                case CommandKind.ShapeLaw: return ShapeLaw(c, state, lab, cmd);\n                case CommandKind.RespondTakeoff: return RespondTakeoff(c, state, lab, cmd);\n                case CommandKind.PickScenarioBaggage: return PickScenarioBaggage(c, state, lab, cmd);\n                case CommandKind.StaffHumanQueue:', 1)
p.write_text(t)
# default-config
import json
p = Path('Rules/Data/default-config.json')
d = json.loads(p.read_text())
if 'campaignDeadline' not in d:
    d['campaignDeadline'] = {'enabled': 1, 'minEra': 3, 'scoreBonusPermille': 100, 'endingId': 'campaign-stop'}
    d['lawShaping'] = {'enabled': 1, 'minEra': 2, 'strictDelta': 40, 'looseDelta': 30, 'regulatorAttentionDampFrom': 700, 'regulatorDampPermille': 500}
    d['takeoffCrisis'] = {'enabled': 1, 'minEra': 4, 'capabilityMin': 850, 'corruptionMin': 400, 'cooldownWeeks': 48}
    d['scenarioBaggage'] = {'enabled': 1, 'items': [
        {'id': 'early-lawsuit', 'name': 'Lawsuit on day one', 'kind': 'lawsuit', 'caseClaim': 500000, 'caseStrength': 400, 'reputationLoss': 0, 'trustLoss': 0, 'loanPrincipal': 0, 'notePrincipal': 0, 'rbfAdvance': 0},
        {'id': 'starter-debt', 'name': 'Starter debt', 'kind': 'debt', 'loanPrincipal': 200000, 'notePrincipal': 0, 'rbfAdvance': 0, 'caseClaim': 0, 'caseStrength': 0, 'reputationLoss': 0, 'trustLoss': 0},
        {'id': 'reputational-leak', 'name': 'Reputational leak', 'kind': 'leak', 'reputationLoss': 80, 'trustLoss': 40, 'loanPrincipal': 0, 'notePrincipal': 0, 'rbfAdvance': 0, 'caseClaim': 0, 'caseStrength': 0},
    ]}
    end = d['endings']
    if 'exitCovenantCallPermille' not in end: end['exitCovenantCallPermille'] = 200
    if not any(e.get('id') == 'campaign-stop' for e in end['endings']):
        end['endings'].append({'id': 'campaign-stop', 'name': 'Campaign clock', 'kind': 'Neutral', 'blurbKey': 'campaign-stop', 'continueModes': ['keep-going', 'spectate', 'acknowledge']})
    office = d['office']
    if not any(e.get('trigger') == 'Takeoff' for e in office['events']):
        takeoff = {'id': 'takeoff-crisis', 'name': 'This is getting away from us', 'text': 'Your best model is clever enough to plan and corrupted enough to hide it. The board wants words now, not a paper next quarter.', 'requires': 'Nothing', 'trigger': 'Takeoff', 'lapseChoice': 'listen', 'fromYear': 0, 'toYear': 0, 'choices': [
            {'id': 'contain', 'text': 'Double containment spending', 'result': 'You buy time and trust, at a cost.', 'effects': [{'kind': 'CashWeeks', 'amount': -4}, {'kind': 'Corruption', 'amount': -100}, {'kind': 'Culture', 'amount': 15}]},
            {'id': 'listen', 'text': 'Let it run while you watch', 'result': 'It does not slow down.', 'effects': [{'kind': 'Corruption', 'amount': 120}, {'kind': 'Attention', 'amount': 80}]},
            {'id': 'publish', 'text': 'Open a safety blog post', 'result': 'Customers like the honesty; regulators like the paper trail.', 'effects': [{'kind': 'Reputation', 'amount': 15}, {'kind': 'Culture', 'amount': 30}]},
        ]}
        idx = next(i for i, e in enumerate(office['events']) if e.get('id') == 'escape-attempt')
        office['events'].insert(idx + 1, takeoff)
    for h in [
        {'id': 'campaign-deadline-on', 'kind': 'CampaignDeadlineSet', 'pick': -1, 'text': '{lab} sets a campaign clock. The run now has a last date'},
        {'id': 'campaign-deadline-off', 'kind': 'CampaignDeadlineSet', 'pick': 0, 'text': '{lab} turns the campaign clock off. Sandbox again'},
        {'id': 'campaign-deadline-hit', 'kind': 'CampaignDeadlineHit', 'pick': -1, 'text': '{lab} hits its deadline. The scoreboard stops and a chapter ends'},
        {'id': 'law-shaped', 'kind': 'LawShaped', 'pick': -1, 'text': "{lab} spends lobbying to bend the next era's alignment law"},
        {'id': 'scenario-baggage', 'kind': 'ScenarioBaggagePicked', 'pick': -1, 'text': '{lab} opens with baggage from a harder scenario'},
    ]:
        if not any(x.get('id') == h['id'] for x in d['news']['headlines']): d['news']['headlines'].append(h)
    for c in [
        {'id': 'campaign-deadline-chat', 'kind': 'CampaignDeadlineSet', 'roles': [], 'trait': '', 'pick': -1, 'about': 'lab', 'text': 'We set a campaign deadline. Everyone knows when the sprint ends.', 'fromYear': 0, 'toYear': 0},
        {'id': 'campaign-hit-chat', 'kind': 'CampaignDeadlineHit', 'roles': [], 'trait': '', 'pick': -1, 'about': 'lab', 'text': 'The deadline arrived. Score the era and decide whether to continue.', 'fromYear': 0, 'toYear': 0},
        {'id': 'law-shaped-chat', 'kind': 'LawShaped', 'roles': [], 'trait': '', 'pick': -1, 'about': 'lab', 'text': 'We shaped the law for the next era. Lobbyists earned their retainer.', 'fromYear': 0, 'toYear': 0},
        {'id': 'baggage-chat', 'kind': 'ScenarioBaggagePicked', 'roles': [], 'trait': '', 'pick': -1, 'about': 'lab', 'text': 'We chose starting baggage. The lab begins under a cloud.', 'fromYear': 0, 'toYear': 0},
    ]:
        if not any(x.get('id') == c['id'] for x in d['chat']['lines']): d['chat']['lines'].append(c)
    p.write_text(json.dumps(d, indent=2) + '\n')
print('batch15 apply ok')
PY