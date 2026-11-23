# shellcheck shell=bash
# Disable batch-seventeen at tip when golden-reverting an earlier batch (sourced after disable_cfg exists).
disable_batch17_at_tip() {
  for _b17 in reservedComputeGate rivalIntentNews gpuTierUnlock endlessScenario shadyFreezeAudit; do
    disable_cfg "$_b17"
  done
  python3 -c "
import json, os
p='Rules/Data/default-config.json'
with open(p) as f: d=json.load(f)
for e in d.get('endings', {}).get('endings', []):
    if e.get('id') == 'scenario-win':
        e['continueModes'] = [x for x in e.get('continueModes', []) if x != 'keep-going']
t = p + '.tmp'
with open(t, 'w') as f:
    json.dump(d, f, indent=2)
    f.write(chr(10))
os.replace(t, p)
" 2>/dev/null || true
}