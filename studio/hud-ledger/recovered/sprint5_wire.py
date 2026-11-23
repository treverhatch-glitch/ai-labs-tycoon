#!/usr/bin/env python3
"""Wire sprint 5 HUD summary lines (cycles 1-6)."""
import re
import subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"

CYCLES = [
    {
        "method": "LastWeekChipDevLandSummaryLine",
        "var": "lastWeekChipDevLand",
        "slug": "last week chip dev land",
        "test_name": "last_week_chip_dev_land_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.ChipDevelopment = -4_500;
            lab.Ledger.LastWeek.Land = -2_000;
            string line = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
            Assert.That(line, Does.Contain("4,500").Or.Contain("2,000"));""",
    },
    {
        "method": "ThisWeekGrantsFinesSummaryLine",
        "var": "thisWeekGrantsFines",
        "slug": "this week grants fines",
        "test_name": "this_week_grants_fines_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Grants = 5_500;
            lab.Ledger.ThisWeek.Fines = -600;
            string line = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("5,500").Or.Contain("600"));""",
    },
    {
        "method": "LifetimePowerFinesSummaryLine",
        "var": "lifetimePowerFines",
        "slug": "lifetime power fines",
        "test_name": "lifetime_power_fines_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Power = -120_000;
            lab.Ledger.Lifetime.Fines = -8_000;
            string line = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
            Assert.That(line, Does.Contain("120,000").Or.Contain("8,000"));""",
    },
    {
        "method": "LastWeekGreyLaunderSummaryLine",
        "var": "lastWeekGreyLaunder",
        "slug": "last week grey launder",
        "test_name": "last_week_grey_launder_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GreyRevenue = 3_300;
            lab.Ledger.LastWeek.Laundered = 700;
            string line = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("700"));""",
    },
    {
        "method": "ThisWeekConstructionGrantsSummaryLine",
        "var": "thisWeekConstructionGrants",
        "slug": "this week construction grants",
        "test_name": "this_week_construction_grants_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -9_000;
            lab.Ledger.ThisWeek.Grants = 4_000;
            string line = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("4,000"));""",
    },
    {
        "method": "LastWeekTrainingRevenueSummaryLine",
        "var": "lastWeekTrainingRevenue",
        "slug": "last week training revenue",
        "test_name": "last_week_training_revenue_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Training = -12_000;
            lab.Ledger.LastWeek.Revenue = 22_000;
            string line = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("12,000").Or.Contain("22,000"));""",
    },
]

HUD = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
PART_ANCHOR = '            string thisWeekNetTrainingPart = string.IsNullOrEmpty(thisWeekNetTraining) ? "" : " · " + thisWeekNetTraining;'


def git_commit(msg: str, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def wire_hud(cycle):
    text = HUD.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""                string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                if (!string.IsNullOrEmpty({cycle['var']}))
                    status = string.IsNullOrEmpty(status) ? {cycle['var']} : status + " · " + {cycle['var']};
"""
    before = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    text = text.replace(before, insert.rstrip("\n") + "\n" + before, 1)
    HUD.write_text(text)
    return True


def wire_train(cycle):
    text = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""                    string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                    if (!string.IsNullOrEmpty({cycle['var']}))
                        _summary.text += " · " + {cycle['var']};
"""
    before = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    text = text.replace(before, insert.rstrip("\n") + "\n" + before, 1)
    TRAIN.write_text(text)
    return True


def wire_staff(cycle):
    text = STAFF.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(lab)" in text:
        return False
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    if PART_ANCHOR not in text:
        raise SystemExit("StaffGpuPanel anchor missing")
    text = text.replace(PART_ANCHOR, PART_ANCHOR + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ thisWeekNetTrainingPart(?: \+ \w+Part)*)", text)
    if not old_tail:
        raise SystemExit("StaffGpuPanel tail missing")
    text = text.replace(old_tail.group(1), old_tail.group(1) + f" + {cycle['var']}Part", 1)
    STAFF.write_text(text)
    return True


def wire_mascot(cycle):
    text = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(sim.Player)" in text:
        return False
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(sim.Player);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    anchor = '            string thisWeekNetTrainingPart = string.IsNullOrEmpty(thisWeekNetTraining) ? "" : " · " + thisWeekNetTraining;'
    text = text.replace(anchor, anchor + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ thisWeekNetTrainingPart(?: \+ \w+Part)*);", text)
    if not old_tail:
        raise SystemExit("Mascot tail missing")
    text = text.replace(old_tail.group(1) + ";", old_tail.group(1) + f" + {cycle['var']}Part;", 1)
    MASCOT.write_text(text)
    return True


def wire_build(cycle):
    text = BUILD.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(_host.Simulation.Player)" in text:
        return False
    before = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{cycle["method"]}(_host.Simulation.Player)\n'
    text = text.replace(before, line + before, 1)
    BUILD.write_text(text)
    return True


def add_test(cycle):
    text = TESTS.read_text()
    if cycle["test_name"] in text:
        return False
    block = f"""
        [Test]
        public void {cycle['test_name']}()
        {{
{cycle['test_body']}
        }}
"""
    insert_at = text.rfind("\n    }\n}")
    text = text[:insert_at] + block + text[insert_at:]
    TESTS.write_text(text)
    return True


def run_cycle(cycle):
    print(f"=== {cycle['slug']} ===")
    if wire_hud(cycle):
        git_commit(f"feat(hud): wire hud view {cycle['slug']}", [HUD])
    if wire_train(cycle):
        git_commit(f"feat(hud): wire training {cycle['slug']}", [TRAIN])
    if wire_staff(cycle):
        git_commit(f"feat(hud): wire staff gpu {cycle['slug']}", [STAFF])
    if wire_mascot(cycle):
        git_commit(f"feat(hud): wire build mascot {cycle['slug']}", [MASCOT])
    if wire_build(cycle):
        git_commit(f"feat(hud): wire build panel {cycle['slug']}", [BUILD])
    if add_test(cycle):
        git_commit(f"test(hud): {cycle['slug']} summary line", [TESTS])


def main():
    golden_after = {2, 4}
    for i, cycle in enumerate(CYCLES, 1):
        run_cycle(cycle)
        if i in golden_after:
            subprocess.check_call(
                [
                    "dotnet",
                    "test",
                    "tools/SimTests/SimTests.csproj",
                    "--filter",
                    "FullyQualifiedName~golden",
                    "-v",
                    "q",
                ],
                cwd=ROOT,
            )


if __name__ == "__main__":
    main()