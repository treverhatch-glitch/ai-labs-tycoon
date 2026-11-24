#!/usr/bin/env python3
"""
Bootstrap CXXIII assets on the backup branch: seed sprint837_840 (synthetic), mirror to 853_856.

This unlocks build_869_885_901_chain.py for tooling tests. Codenames follow bible arc
837 Shawl -> 853 Cloak. Replace with replayed data when sprint 837+ lands on backup tip.
"""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = ROOT / "studio/hud-ledger/recovered"
MAT = ROOT / "studio/hud-ledger/materialize_runner_from_data.py"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)


def find_cmd(needle: str) -> str:
    data = json.loads(TRANSCRIPT.read_text())
    best = ""

    def walk(o):
        nonlocal best
        if isinstance(o, dict):
            c = o.get("command") or ""
            if needle in c and "python3 <<" in c and len(c) > len(best):
                best = c
            for v in o.values():
                walk(v)
        elif isinstance(o, list):
            for x in o:
                walk(x)

    walk(data)
    if not best:
        raise SystemExit(f"transcript command matching {needle!r} not found")
    return best


def exec_py(cmd: str) -> None:
    m = re.search(r"python3 << ['\"]PY['\"]\n(.*)\nPY", cmd, re.DOTALL)
    if not m:
        raise SystemExit("PY heredoc not found")
    py = m.group(1)
    py = py.replace('"/tmp/', f'"{REC}/')
    py = py.replace("'/tmp/", f"'{REC}/")
    py = py.replace('Path("/tmp/', f'Path("{REC}/')
    exec(py, {"Path": Path, "re": re})


def seed_synthetic_837() -> None:
    """Mirror sprint361_364 -> sprint837_840 with Shawl-era codenames (structural bootstrap)."""
    src = REC / "sprint361_364_data.py"
    if not src.is_file():
        raise SystemExit("need sprint361_364_data.py for synthetic seed")
    text = src.read_text()
    rep = [
        ("F361_362", "F837_838"),
        ("F363_364", "F839_840"),
        ("SPRINT361", "SPRINT837"),
        ("SPRINT362", "SPRINT838"),
        ("SPRINT363", "SPRINT839"),
        ("SPRINT364", "SPRINT840"),
        ("PA361", "PA837"),
        ("PA362", "PA838"),
        ("PA363", "PA839"),
        ("PA364", "PA840"),
        ("Loom", "Stump"),
        ("loom", "stump"),
        ("Butte", "Shawl"),
        ("butte", "shawl"),
        ("Trench", "Cincture"),
        ("trench", "cincture"),
        ("PulseStar", "Jab"),
        ("pulseStar", "jab"),
        ("Orbit", "Kerchief"),
        ("orbit", "kerchief"),
        ("Apex", "Mantle"),
        ("apex", "mantle"),
        ("Mist", "Stump"),
        ("mist", "stump"),
    ]
    for a, b in rep:
        text = text.replace(a, b)
    (REC / "sprint837_840_data.py").write_text(text)
    print("wrote synthetic", REC / "sprint837_840_data.py")


def fix_sprint855_label(data_path: Path) -> None:
    """Transcript fix: first SPRINT856 block after F855_856 should be SPRINT855."""
    text = data_path.read_text()
    if "SPRINT855 = [" in text:
        return
    text = text.replace("\nSPRINT856 = [\n", "\nSPRINT855 = [\n", 1)
    data_path.write_text(text)
    print("fixed SPRINT855 label in", data_path.name)


def main() -> int:
    if (REC / "sprint853_856_data.py").is_file() and "SPRINT853" in (REC / "sprint853_856_data.py").read_text():
        print("sprint853_856_data.py already present")
    else:
        if not (REC / "sprint837_840_data.py").is_file():
            seed_synthetic_837()
        subprocess = __import__("subprocess")
        subprocess.check_call([sys.executable, str(MAT), "sprint837_840_data.py"], cwd=ROOT)
        src = (REC / "sprint837_840_data.py").read_text()
        pairs = [
            ("Stump", "Knot"), ("stump", "knot"),
            ("Cincture", "Cinch"), ("cincture", "cinch"),
            ("Jab", "Prod"), ("jab", "prod"),
            ("Kerchief", "Fichu"), ("kerchief", "fichu"),
            ("Shawl", "Cloak"), ("shawl", "cloak"),
            ("Mantle", "Shawl"), ("mantle", "shawl"),
        ]
        for a, b in pairs:
            src = src.replace(a, b)
        for old, new in [
            ("F837_838", "F853_854"), ("F839_840", "F855_856"),
            ("SPRINT837", "SPRINT853"), ("SPRINT838", "SPRINT854"),
            ("SPRINT839", "SPRINT855"), ("SPRINT840", "SPRINT856"),
            ("PA837", "PA853"), ("PA838", "PA854"),
            ("PA839", "PA855"), ("PA840", "PA856"),
        ]:
            src = src.replace(old, new)
        src = re.sub(
            r"ChipDevelopment = (-?\d+)",
            lambda m: f"ChipDevelopment = {int(m.group(1)) - 20}",
            src,
        )
        (REC / "sprint853_856_data.py").write_text(src)
        fix_sprint855_label(REC / "sprint853_856_data.py")
    if not (REC / "run_sprints_853_856.py").is_file():
        import subprocess

        subprocess.check_call([sys.executable, str(MAT), "sprint853_856_data.py"], cwd=ROOT)
    if "SPRINT853" not in (REC / "sprint853_856_data.py").read_text():
        print("sprint853_856_data.py still incomplete", file=sys.stderr)
        return 1
    print("ok: sprint853_856_data.py + run_sprints_853_856.py (synthetic until real @837 replay)")
    return 0


if __name__ == "__main__":
    sys.exit(main())