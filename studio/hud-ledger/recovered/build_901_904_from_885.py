# Excerpt from bc-93b5443a transcript (turn-474) — requires /tmp/sprint885_888_data.py and /tmp/run_sprints_885_888.py on disk.
from pathlib import Path
import re

src = Path("/tmp/sprint885_888_data.py").read_text()
pairs = [
    ("Clew", "Kink"), ("clew", "kink"),
    ("Cable", "Harness"), ("cable", "harness"),
    ("Tabard", "Vestment"), ("tabard", "vestment"),
    ("Surplice", "Cassock"), ("surplice", "cassock"),
    ("Jerkin", "Doublet"), ("jerkin", "doublet"),
]
for a, b in pairs:
    src = src.replace(a, b)
for old, new in [
    ("F885_886", "F901_902"), ("F887_888", "F903_904"),
    ("SPRINT885", "SPRINT901"), ("SPRINT886", "SPRINT902"),
    ("SPRINT887", "SPRINT903"), ("SPRINT888", "SPRINT904"),
    ("PA885", "PA901"), ("PA886", "PA902"), ("PA887", "PA903"), ("PA888", "PA904"),
]:
    src = src.replace(old, new)
src = re.sub(r"ChipDevelopment = (-?\d+)", lambda m: f"ChipDevelopment = {int(m.group(1)) - 20}", src)
Path("/tmp/sprint901_904_data.py").write_text(src)

runner = Path("/tmp/run_sprints_885_888.py").read_text()
for old, new in [
    ("sprint885_888_data", "sprint901_904_data"),
    ("F885_886", "F901_902"), ("F887_888", "F903_904"),
    ("SPRINT885", "SPRINT901"), ("SPRINT886", "SPRINT902"),
    ("SPRINT887", "SPRINT903"), ("SPRINT888", "SPRINT904"),
    ("PA885", "PA901"), ("PA886", "PA902"), ("PA887", "PA903"), ("PA888", "PA904"),
    ("sprint885-886", "sprint901-902"), ("sprint887-888", "sprint903-904"),
    ("run_sprints_885_888", "run_sprints_901_904"),
]:
    runner = runner.replace(old, new)
runner = runner.replace("lifetimeDevClewSpendIncomePart", "lifetimeDevKinkSpendIncomePart")
runner = re.sub(
    r'run_sprint\(SPRINT902, PA902, "[^"]+"\)',
    'run_sprint(SPRINT902, PA902, "lifetimeDevHarnessSpendIncomePart")',
    runner,
)
runner = re.sub(
    r'run_sprint\(SPRINT903, PA903, "[^"]+"\)',
    'run_sprint(SPRINT903, PA903, "lifetimeDevVestmentSpendIncomePart")',
    runner,
)
runner = re.sub(
    r'run_sprint\(SPRINT904, PA904, "[^"]+"\)',
    'run_sprint(SPRINT904, PA904, "lifetimeDevCassockSpendIncomePart")',
    runner,
)
Path("/tmp/run_sprints_901_904.py").write_text(runner.rstrip() + "\n")
print("ok — run: python3 /tmp/run_sprints_901_904.py")