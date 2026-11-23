#!/usr/bin/env python3
"""Wire sprint 3 HUD summary lines across six cycles."""
import re
import subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"

CYCLES = [
    {
        "method": "ThisWeekConstructionSalariesSummaryLine",
        "var": "thisWeekBuild",
        "slug": "this week construction salaries",
        "test_name": "this_week_construction_salaries_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.Construction = -3_000;
            lab.Ledger.ThisWeek.Salaries = -4_000;
            string line = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
            Assert.That(line, Does.Contain("3,000").Or.Contain("4,000"));""",
    },
    {
        "method": "LastWeekHiringPatentSummaryLine",
        "var": "lastWeekHiringPatent",
        "slug": "last week hiring patent",
        "test_name": "last_week_hiring_patent_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Hiring = -2_500;
            lab.Ledger.LastWeek.CapitalIn = 12_000;
            string line = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("2,500").Or.Contain("12,000"));""",
    },
    {
        "method": "LastWeekConstructionPatentSummaryLine",
        "var": "lastWeekConstructionPatent",
        "slug": "last week construction patent",
        "test_name": "last_week_construction_patent_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.Construction = -6_000;
            lab.Ledger.LastWeek.Hiring = -1_000;
            string line = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
            Assert.That(line, Does.Contain("6,000").Or.Contain("1,000"));""",
    },
    {
        "method": "ThisWeekChipRevenueSummaryLine",
        "var": "thisWeekChipRevenue",
        "slug": "this week chip revenue",
        "test_name": "this_week_chip_revenue_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.ThisWeek.ChipSales = 9_000;
            lab.Ledger.ThisWeek.Revenue = 15_000;
            string line = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
            Assert.That(line, Does.Contain("9,000").Or.Contain("15,000"));""",
    },
    {
        "method": "LifetimeGrantsNetSummaryLine",
        "var": "lifetimeGrantsNet",
        "slug": "lifetime grants net",
        "test_name": "lifetime_grants_net_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.Lifetime.Grants = 50_000;
            lab.Ledger.Lifetime.Net = 20_000;
            string line = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
            Assert.That(line, Does.Contain("50,000").Or.Contain("20,000"));""",
    },
    {
        "method": "LastWeekSiteCapitalSummaryLine",
        "var": "lastWeekSiteCapital",
        "slug": "last week site capital",
        "test_name": "last_week_site_capital_summary_line",
        "test_body": """            var lab = new LabState { Id = 1 };
            lab.Ledger.LastWeek.SiteBuild = -7_000;
            lab.Ledger.LastWeek.CapitalIn = 25_000;
            string line = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
            Assert.That(line, Does.Contain("7,000").Or.Contain("25,000"));""",
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
    anchor = """                string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekLand : status + " · " + lastWeekLand;"""
    needle = f"""                string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                if (!string.IsNullOrEmpty({cycle['var']}))
                    status = string.IsNullOrEmpty(status) ? {cycle['var']} : status + " · " + {cycle['var']};
                string vendorCourt"""
    if cycle["var"] in text and f"{cycle['method']}(lab)" in text:
        return False
    if needle.replace(cycle["var"], "PLACEHOLDER")[:80] not in text and cycle["var"] in text:
        return False
    if f"{cycle['method']}(lab)" in text:
        return False
    insert = f"""                string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                if (!string.IsNullOrEmpty({cycle['var']}))
                    status = string.IsNullOrEmpty(status) ? {cycle['var']} : status + " · " + {cycle['var']};
"""
    if insert.strip() in text:
        return False
    if anchor not in text:
        raise SystemExit("HqHudView anchor missing")
    text = text.replace(
        anchor,
        anchor + "\n" + insert.rstrip("\n"),
        1,
    )
    HUD.write_text(text)
    return True


def wire_train(cycle):
    text = TRAIN.read_text()
    anchor = """                    string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLand))
                        _summary.text += " · " + lastWeekLand;"""
    if f"{cycle['method']}(lab)" in text:
        return False
    insert = f"""                    string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
                    if (!string.IsNullOrEmpty({cycle['var']}))
                        _summary.text += " · " + {cycle['var']};
"""
    text = text.replace(anchor, anchor + "\n" + insert.rstrip("\n"), 1)
    TRAIN.write_text(text)
    return True


def wire_staff(cycle):
    text = STAFF.read_text()
    if f"{cycle['method']}(lab)" in text:
        return False
    anchor = '            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;'
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(lab);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    text = text.replace(anchor, anchor + "\n" + insert.rstrip("\n"), 1)
    text = text.replace(
        " + thisWeekTrainingPart + lastWeekLandPart",
        f" + thisWeekTrainingPart + lastWeekLandPart + {cycle['var']}Part",
    )
    STAFF.write_text(text)
    return True


def wire_mascot(cycle):
    text = MASCOT.read_text()
    if f"{cycle['method']}(sim.Player)" in text:
        return False
    anchor = '            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;'
    insert = f"""            string {cycle['var']} = HqDisplayQueriesHud.{cycle['method']}(sim.Player);
            string {cycle['var']}Part = string.IsNullOrEmpty({cycle['var']}) ? "" : " · " + {cycle['var']};
"""
    text = text.replace(anchor, anchor + "\n" + insert.rstrip("\n"), 1)
    text = text.replace(
        " + thisWeekTrainingPart + lastWeekLandPart;",
        f" + thisWeekTrainingPart + lastWeekLandPart + {cycle['var']}Part;",
    )
    MASCOT.write_text(text)
    return True


def wire_build(cycle):
    text = BUILD.read_text()
    if f"{cycle['method']}(_host.Simulation.Player)" in text:
        return False
    anchor = (
        '                                 + " · " + HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(_host.Simulation.Player)\n'
        '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    )
    insert = (
        f'                                 + " · " + HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(_host.Simulation.Player)\n'
        f'                                 + " · " + HqDisplayQueriesHud.{cycle["method"]}(_host.Simulation.Player)\n'
        f'                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    )
    text = text.replace(anchor, insert, 1)
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
    # Insert before last closing braces of class — after last_week_site or last test
    marker = "        public void Last_week_land_hardware_summary_line()"
    if marker not in text:
        marker = "        public void Last_week_room_chip_summary_line()"
    # append at end of file before final }
    text = text.rstrip()
    if not text.endswith("}"):
        raise SystemExit("tests file unexpected")
    # find last [Test] and append after last test method's closing brace
    insert_at = text.rfind("\n    }\n}")
    if insert_at < 0:
        insert_at = text.rfind("\n}")
    text = text[:insert_at] + block + text[insert_at:]
    TESTS.write_text(text)
    return True


def main():
    for i, cycle in enumerate(CYCLES, 1):
        print(f"=== cycle {i}: {cycle['slug']} ===")
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


if __name__ == "__main__":
    main()