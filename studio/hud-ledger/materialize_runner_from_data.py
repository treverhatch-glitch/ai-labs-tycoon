#!/usr/bin/env python3
"""Build run_sprints_LO-HI.py from sprintLO_HI_data.py + run_sprints_57_60.py helper tail."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = Path(__file__).resolve().parent / "recovered"
TEMPLATE = REC / "run_sprints_57_60.py"


def main() -> int:
    if len(sys.argv) != 2:
        print("usage: materialize_runner_from_data.py sprint125_128_data.py", file=sys.stderr)
        return 1
    data_path = REC / sys.argv[1] if not sys.argv[1].startswith("/") else Path(sys.argv[1])
    m = re.match(r"sprint(\d+)_(\d+)_data\.py$", data_path.name)
    if not m:
        print("expected sprintNNN_MMM_data.py", file=sys.stderr)
        return 1
    lo, hi = map(int, m.groups())
    if (hi - lo) != 3:
        print("expected 4-sprint span", file=sys.stderr)
        return 1
    data = data_path.read_text()
    template = TEMPLATE.read_text()
    head_end = template.index("F57_58")
    head = template[:head_end]
    tail_start = template.index("\ndef formatter_body")
    tail = template[tail_start:]

    pas = re.findall(r"^PA(\d+) = ", data, re.MULTILINE)
    if len(pas) != 4:
        print(f"expected 4 PA lines, got {len(pas)}", file=sys.stderr)
        return 1
    s0, s1, s2, s3 = [int(x) for x in pas]

    def tail_token(pa_line: str) -> str:
        mm = re.search(r"string (\w+)Part", pa_line)
        if not mm:
            raise SystemExit(f"bad PA line: {pa_line[:60]}")
        return mm.group(1)

    pa_lines = {
        k: v
        for k, v in re.findall(r"^(PA\d+) = (.+)$", data, re.MULTILINE)
    }
    main = f"""
insert_formatters(F{s0}_{s0 + 1})
git_commit("feat(hud): sprint{s0}-{s0 + 1} twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT{s0}, PA{s0}, "{tail_token(pa_lines[f'PA{s0}'])}")
run_sprint(SPRINT{s0 + 1}, PA{s0 + 1}, "{tail_token(pa_lines[f'PA{s0 + 1}'])}")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F{s2}_{s2 + 1})
git_commit("feat(hud): sprint{s2}-{s2 + 1} twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT{s2}, PA{s2}, "{tail_token(pa_lines[f'PA{s2}'])}")
run_sprint(SPRINT{s2 + 1}, PA{s2 + 1}, "{tail_token(pa_lines[f'PA{s2 + 1}'])}")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""
    # strip duplicate main from tail (everything after last run_sprint in template)
    tail = tail.split("insert_formatters(F57_58)")[0]
    out_name = REC / f"run_sprints_{s0}_{s3}.py"
    body = head + data + "\n" + tail + main
    # harden git_commit in tail if old style
    old = '    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)\n'
    new = (
        '    if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode == 0:\n'
        "        return\n"
        + old
    )
    if old in body and new not in body:
        body = body.replace(
            "def git_commit(msg, paths):\n"
            "    subprocess.check_call([\"git\", \"add\", *map(str, paths)], cwd=ROOT)\n"
            + old,
            "def git_commit(msg, paths):\n"
            "    subprocess.check_call([\"git\", \"add\", *map(str, paths)], cwd=ROOT)\n"
            + new,
        )
    out_name.write_text(body)
    print(f"wrote {out_name.name} ({len(body)} bytes)")
    return 0


if __name__ == "__main__":
    sys.exit(main())