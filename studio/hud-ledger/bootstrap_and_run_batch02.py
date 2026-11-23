#!/usr/bin/env python3
"""Genesis HUD from transcript method merge + replay sprint 3–12 on cursor/unity-hud-backup."""
import json
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD = UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs"
HUD_VIEW = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
OPS = UNITY / "Scripts/Hq/HqOperationsHud.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)
MERGED_METHODS = Path(__file__).parent / "snapshot_sprint2/HqDisplayQueriesHud.methods.cs"
RECOVERED = Path(__file__).parent / "recovered"

BASE_HUD = """using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Unity.Hq
{
    /// <summary>One-line HUD formatters backed by <see cref="DisplayQueries"/>.</summary>
    public static class HqDisplayQueriesHud
    {
        public static string WeeklyPowerLine(Simulation sim)
        {
            if (sim == null) return "";
            var bills = DisplayQueries.WeeklyPowerBillParts(sim.Config, sim.State, sim.Player);
            if (bills == null || bills.Count == 0) return "";
            long total = 0;
            for (int i = 0; i < bills.Count; i++)
                total += bills[i].Total;
            if (total <= 0) return "";
            return "Power $" + total.ToString("N0") + "/wk";
        }

        public static string BankruptLine(LabState lab)
        {
            if (lab == null || !lab.Bankrupt) return "";
            return "Bankrupt";
        }

        public static string LatestVendorCourtLine(GameState state, int tick)
        {
            return "";
        }

        public static string WeeklyComputeLine(GameConfig config, GameState state, LabState lab)
        {
            return "";
        }

"""


def git_commit(msg: str, paths):
    subprocess.check_call(["git", "add", *[str(p) for p in paths]], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def formatter_body(name, parts):
    decl = "\n".join(f"            string {v} = {call};" for v, call in parts)
    names = [p[0] for p in parts]
    empty = " && ".join(f"string.IsNullOrEmpty({n})" for n in names)
    first = names[0]
    joins = [
        f"""            if (!string.IsNullOrEmpty({n}))
                line = string.IsNullOrEmpty(line) ? {n} : line + " · " + {n};"""
        for n in names[1:]
    ]
    return f"""
        public static string {name}(LabState lab)
        {{
{decl}
            if ({empty})
                return "";
            string line = "";
            if (!string.IsNullOrEmpty({first})) line = {first};
""" + "\n".join(joins) + """
            return line;
        }}
"""


def insert_formatters(batch):
    t = HUD.read_text()
    block = ""
    for name, parts in batch:
        if name in t:
            continue
        block += formatter_body(name, parts)
    if not block:
        return False
    anchor = "        public static string BankruptLine(LabState lab)"
    HUD.write_text(t.replace(anchor, block + "\n" + anchor, 1))
    return True


def parse_sprint_wire_formatters(py_path: Path):
    """Parse CYCLES from sprintN_wire.py into formatter tuples."""
    src = py_path.read_text()
    m = re.search(r"CYCLES\s*=\s*\[(.*?)\n\]", src, re.S)
    if not m:
        return []
    block = m.group(1)
    out = []
    for cycle in re.finditer(
        r'"method":\s*"([^"]+)".*?"test_body":\s*"""(.*?)"""',
        block,
        re.S,
    ):
        method = cycle.group(1)
        body = cycle.group(2)
        fields = re.findall(r"lab\.Ledger\.(?:ThisWeek|LastWeek|Lifetime)\.(\w+)\s*=", body)
        if len(fields) < 2:
            continue
        parts = []
        for i, field in enumerate(fields[:3]):
            var = field[0].lower() + field[1:]
            period = "ThisWeek" if "ThisWeek" in body else "LastWeek" if "LastWeek" in body else "Lifetime"
            if f"lab.Ledger.{period}.{field}" in body:
                call = f"{period}Ledger{field}Line(lab)"
                # fix naming: Training -> TrainingSpend for ThisWeek/LastWeek
                if field == "Training":
                    call = f"{period}LedgerTrainingSpendLine(lab)"
                parts.append((var, call))
        if len(parts) >= 2:
            out.append((method, parts))
    return out


def build_hud_from_merge():
    raw = MERGED_METHODS.read_text().strip()
    lines = []
    for line in raw.splitlines():
        if line.startswith("public static string "):
            line = "        " + line
        lines.append(line)
    methods = "\n".join(lines)
    closing = """
    }
}
"""
    HUD.write_text(BASE_HUD + "\n" + methods + "\n" + closing)
    git_commit("feat(hud): genesis ledger HUD formatters from transcript merge", [HUD])


def patch_hq_hud_view():
    t = HUD_VIEW.read_text()
    if "LatestVendorCourtLine" in t:
        return
    old = """            if (_statusLabel != null)
            {
                string status = HqFinancialHud.StatusLine(lab);
                _statusLabel.text = status;
                _statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(status));
            }"""
    new = """            if (_statusLabel != null)
            {
                string status = HqFinancialHud.StatusLine(lab);
                string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekLand : status + " · " + lastWeekLand;
                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);
                if (!string.IsNullOrEmpty(vendorCourt))
                    status = string.IsNullOrEmpty(status) ? vendorCourt : status + " · " + vendorCourt;
                _statusLabel.text = status;
                _statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(status));
            }"""
    if old not in t:
        raise SystemExit("HqHudView status block missing")
    HUD_VIEW.write_text(t.replace(old, new, 1))
    git_commit("feat(hud): genesis hud view ledger anchors", [HUD_VIEW])


def patch_training_panel():
    t = TRAIN.read_text()
    if "weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine" in t:
        return
    anchor = """            _status.text = string.IsNullOrEmpty(_host.StatusMessage)
                ? "Start a run after researching the line."
                : _host.StatusMessage;"""
    insert = """            string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLand))
                        _status.text += " · " + lastWeekLand;
                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);
                    if (!string.IsNullOrEmpty(weeklyCompute))
                        _status.text += " · " + weeklyCompute;
"""
    if anchor not in t:
        raise SystemExit("TrainingRunPanel anchor missing")
    t = t.replace(anchor, insert + "\n            " + anchor, 1)
    TRAIN.write_text(t)
    git_commit("feat(hud): genesis training panel ledger tail", [TRAIN])


def write_ops_stub():
    OPS.write_text(
        """using AiLabs.Rules.Sim;

namespace AiLabs.Unity.Hq
{
  public static class HqOperationsHud
  {
    public static string RunningCostLine(Simulation sim) => "";
    public static string ModelStripLine(LabState lab) => lab?.Models?.Count.ToString() ?? "0";
  }
}
"""
    )
    git_commit("feat(hud): HqOperationsHud stub", [OPS])


def patch_staff_panel():
    t = STAFF.read_text()
    if "lastWeekLandPart" in t:
        return
    anchor = '_gpuSummary.text = "GPUs: " + rented + " rented, " + owned + " owned (" + gen.Name + ")";'
    block = """            string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;
            _gpuSummary.text = "GPUs: " + rented + " rented, " + owned + " owned (" + gen.Name + ")" + lastWeekLandPart;"""
    if anchor not in t:
        raise SystemExit("StaffGpuPanel missing gpu summary")
    t = t.replace(anchor, block, 1)
    STAFF.write_text(t)
    git_commit("feat(hud): genesis staff gpu ledger tail", [STAFF])


def patch_mascot():
    if not MASCOT.exists():
        MASCOT.write_text(
            """using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    public sealed class HqMascotStripHud : MonoBehaviour
    {
        SimulationHost _host;
        Text _line;
        public static HqMascotStripHud Create(Transform parent, SimulationHost host)
        {
            var go = new GameObject("MascotStrip");
            go.transform.SetParent(parent, false);
            var h = go.AddComponent<HqMascotStripHud>();
            h._host = host;
            h._line = go.AddComponent<Text>();
            host.StateChanged += h.Refresh;
            return h;
        }
        void OnDestroy() { if (_host != null) _host.StateChanged -= Refresh; }
        void Refresh()
        {
            if (_host?.Simulation == null) return;
            var sim = _host.Simulation;
            string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(sim.Player);
            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;
            _line.text = "Mascots" + lastWeekLandPart;
        }
    }
}
"""
        )
    git_commit("feat(hud): genesis mascot strip", [MASCOT])


def patch_build_panel():
    t = BUILD.read_text()
    if "HqOperationsHud.RunningCostLine" in t:
        return
    if "_buildSummary" not in t:
        t = t.replace(
            "        Text _status;",
            "        Text _status;\n        Text _buildSummary;",
            1,
        )
    anchor = '_status.text = "Drag on the lot to place a room."'
    ins = """            _buildSummary.text = "Build"
                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);
            """
    if anchor in t:
        t = t.replace(anchor, ins.strip() + "\n            " + anchor, 1)
    BUILD.write_text(t)
    git_commit("feat(hud): genesis room build summary tail", [BUILD])


def ensure_tests():
    if TESTS.exists():
        return
    TESTS.write_text(
        """using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqDisplayQueriesHudTests
    {
        [Test]
        public void last_week_land_hardware_summary_line()
        {
            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Land = -10_000;
            lab.Ledger.LastWeek.Hardware = -2_000;
            string line = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
            Assert.That(line, Does.Contain("10,000").Or.Contain("2,000"));
        }
    }
}
"""
    )
    git_commit("test(hud): genesis display queries harness", [TESTS])


def golden():
  subprocess.check_call(
      ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
      cwd=ROOT,
  )


def run_sprint_wire(n: int):
    py = RECOVERED / f"sprint{n}_wire.py"
    if not py.exists():
        print("skip missing", py)
        return
    fmts = parse_sprint_wire_formatters(py)
    if insert_formatters(fmts):
        git_commit(f"feat(hud): sprint{n} six ledger summary formatters", [HUD])
    subprocess.check_call([sys.executable, str(py)], cwd=ROOT)


def main():
    if not MERGED_METHODS.is_file():
        raise SystemExit("run transcript merge first")
    build_hud_from_merge()
    patch_hq_hud_view()
    write_ops_stub()
    patch_training_panel()
    patch_staff_panel()
    patch_mascot()
    patch_build_panel()
    ensure_tests()
    for n in range(3, 9):
        run_sprint_wire(n)
    for script in ["run_sprints_9_10.py", "run_sprints_11_12.py"]:
        p = RECOVERED / script
        if p.exists():
            subprocess.check_call([sys.executable, str(p)], cwd=ROOT)
    try:
        golden()
    except FileNotFoundError:
        print("dotnet missing; golden skipped", file=sys.stderr)


if __name__ == "__main__":
    main()