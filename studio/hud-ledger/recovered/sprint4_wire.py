#!/usr/bin/env python3
"""Wire sprint 4 HUD summary lines (cycles 1-6)."""
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"

CYCLES = [
    {
        "method": "ThisWeekFinesPowerSummaryLine",
        "var": "thisWeekFinesPower",
        "slug": "this week fines power",
        "test_name": "this_week_fines_power_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Fines = -800;
            lab.Ledger.ThisWeek.Power = -2_200;
            string line = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
            Assert.That(line, Does.Contain("800").Or.Contain("2,200"));""",
    },
    {
        "method": "LastWeekSalariesRoomSummaryLine",
        "var": "lastWeekSalariesRoom",
        "slug": "last week salaries room",
        "test_name": "last_week_salaries_room_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Salaries = -11_000;
            lab.Ledger.LastWeek.RoomUpkeep = -900;
            string line = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
            Assert.That(line, Does.Contain("11,000").Or.Contain("900"));""",
    },
    {
        "method": "LifetimeRevenueChipSummaryLine",
        "var": "lifetimeRevenueChip",
        "slug": "lifetime revenue chip",
        "test_name": "lifetime_revenue_chip_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Revenue = 200_000;
            lab.Ledger.Lifetime.ChipSales = 75_000;
            string line = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
            Assert.That(line, Does.Contain("200,000").Or.Contain("75,000"));""",
    },
    {
        "method": "LastWeekGpuUpkeepRentSummaryLine",
        "var": "lastWeekGpuUpkeepRent",
        "slug": "last week gpu upkeep rent",
        "test_name": "last_week_gpu_upkeep_rent_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.GpuUpkeep = -1_200;
            lab.Ledger.LastWeek.GpuRent = -3_400;
            string line = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
            Assert.That(line, Does.Contain("1,200").Or.Contain("3,400"));""",
    },
    {
        "method": "LastWeekRevenueNetSummaryLine",
        "var": "lastWeekRevenueNet",
        "slug": "last week revenue net",
        "test_name": "last_week_revenue_net_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Revenue = 18_000;
            lab.Ledger.LastWeek.Net = 4_500;
            string line = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
            Assert.That(line, Does.Contain("18,000").Or.Contain("4,500"));""",
    },
    {
        "method": "ThisWeekNetTrainingSummaryLine",
        "var": "thisWeekNetTraining",
        "slug": "this week net training",
        "test_name": "this_week_net_training_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Net = 3_300;
            lab.Ledger.ThisWeek.Training = -6_500;
            string line = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
            Assert.That(line, Does.Contain("3,300").Or.Contain("6,500"));""",
    },
]

HUD = UNITY / "Scripts/Hq/HqHudView.cs"
TRAIN = UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF = UNITY / "Scripts/Hq/StaffGpuPanel.cs"
MASCOT = UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"


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
    if before not in text:
        raise SystemExit("HqHudView vendorCourt anchor missing")
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
    if before not in text:
        raise SystemExit("TrainingRunPanel weeklyCompute anchor missing")
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
    anchor = f"            string {cycle['var']} = "
    if anchor in text:
        return False
    part_anchor = "            string lastWeekSiteCapitalPart = string.IsNullOrEmpty(lastWeekSiteCapital) ? \"\" : \" · \" + lastWeekSiteCapital;"
    if part_anchor not in text:
        raise SystemExit("StaffGpuPanel lastWeekSiteCapitalPart anchor missing")
    text = text.replace(part_anchor, part_anchor + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(
        r"(\+ lastWeekSiteCapitalPart(?: \+ \w+Part)*)",
        text,
    )
    if not old_tail:
        raise SystemExit("StaffGpuPanel tail anchor missing")
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
    part_anchor = "            string lastWeekSiteCapitalPart = string.IsNullOrEmpty(lastWeekSiteCapital) ? \"\" : \" · \" + lastWeekSiteCapital;"
    if part_anchor not in text:
        raise SystemExit("HqMascotStripHud lastWeekSiteCapitalPart anchor missing")
    text = text.replace(part_anchor, part_anchor + "\n" + insert.rstrip("\n"), 1)
    old_tail = re.search(
        r"(\+ lastWeekSiteCapitalPart(?: \+ \w+Part)*);",
        text,
    )
    if not old_tail:
        raise SystemExit("HqMascotStripHud tail anchor missing")
    text = text.replace(old_tail.group(1) + ";", old_tail.group(1) + f" + {cycle['var']}Part;", 1)
    MASCOT.write_text(text)
    return True


def wire_build(cycle):
    text = BUILD.read_text()
    if f"HqDisplayQueriesHud.{cycle['method']}(_host.Simulation.Player)" in text:
        return False
    before = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = (
        f'                                 + " · " + HqDisplayQueriesHud.{cycle["method"]}(_host.Simulation.Player)\n'
    )
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
    if insert_at < 0:
        raise SystemExit("tests file anchor missing")
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
    start = int(sys.argv[1]) if len(sys.argv) > 1 else 0
    end = int(sys.argv[2]) if len(sys.argv) > 2 else len(CYCLES)
    for cycle in CYCLES[start:end]:
        run_cycle(cycle)


if __name__ == "__main__":
    main()