#!/usr/bin/env python3
import re
import subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"

CYCLES = [
    {
        "method": "LastWeekPatentSiteSummaryLine",
        "var": "lastWeekPatentSite",
        "slug": "last week patent site",
        "test_name": "last_week_patent_site_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -10_000;
            lab.Ledger.LastWeek.Land = -3_500;
            string line = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
            Assert.That(line, Does.Contain("10,000").Or.Contain("3,500"));""",
    },
    {
        "method": "ThisWeekTrainingChipSummaryLine",
        "var": "thisWeekTrainingChip",
        "slug": "this week training chip",
        "test_name": "this_week_training_chip_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Training = -8_000;
            lab.Ledger.ThisWeek.ChipSales = 6_200;
            string line = HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(lab);
            Assert.That(line, Does.Contain("8,000").Or.Contain("6,200"));""",
    },
    {
        "method": "LifetimeNetTrainingSummaryLine",
        "var": "lifetimeNetTraining",
        "slug": "lifetime net training",
        "test_name": "lifetime_net_training_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Net = 90_000;
            lab.Ledger.Lifetime.Training = -400_000;
            string line = HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("90,000").Or.Contain("400,000"));""",
    },
    {
        "method": "LastWeekFinesTrainingSummaryLine",
        "var": "lastWeekFinesTraining",
        "slug": "last week fines training",
        "test_name": "last_week_fines_training_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Fines = -1_100;
            lab.Ledger.LastWeek.Training = -15_000;
            string line = HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("1,100").Or.Contain("15,000"));""",
    },
    {
        "method": "ThisWeekGreyNetSummaryLine",
        "var": "thisWeekGreyNet",
        "slug": "this week grey net",
        "test_name": "this_week_grey_net_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.GreyRevenue = 2_400;
            lab.Ledger.ThisWeek.Net = 1_800;
            string line = HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(lab);
            Assert.That(line, Does.Contain("2,400").Or.Contain("1,800"));""",
    },
    {
        "method": "LastWeekRoomHiringSummaryLine",
        "var": "lastWeekRoomHiring",
        "slug": "last week room hiring",
        "test_name": "last_week_room_hiring_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.RoomUpkeep = -2_200;
            lab.Ledger.LastWeek.Hiring = -5_500;
            string line = HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(lab);
            Assert.That(line, Does.Contain("2,200").Or.Contain("5,500"));""",
    },
]

HUD = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"
PART_ANCHOR = '            string thisWeekPowerChipPart = string.IsNullOrEmpty(thisWeekPowerChip) ? "" : " · " + thisWeekPowerChip;'


def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode == 0: return
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
    text = text.replace(PART_ANCHOR, PART_ANCHOR + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ thisWeekPowerChipPart(?: \+ \w+Part)*)", text)
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
    text = text.replace(PART_ANCHOR, PART_ANCHOR + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(r"(\+ thisWeekPowerChipPart(?: \+ \w+Part)*);", text)
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
    for i, cycle in enumerate(CYCLES, 1):
        run_cycle(cycle)
        if i in {2, 4}:
            subprocess.check_call(
                ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
                cwd=ROOT,
            )


if __name__ == "__main__":
    main()