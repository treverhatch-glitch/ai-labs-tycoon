#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

SPRINT19 = [
    ("LastWeekHardwareHiringSummaryLine", "lastWeekHardwareHiring", "last week hardware hiring", "last_week_hardware_hiring_summary_line",
     "lab.Ledger.LastWeek.Hardware = -6_300;\n            lab.Ledger.LastWeek.Hiring = -2_100;", "6,300", "2,100"),
    ("ThisWeekRevenueSalariesSummaryLine", "thisWeekRevenueSalaries", "this week revenue salaries", "this_week_revenue_salaries_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 16_000;\n            lab.Ledger.ThisWeek.Salaries = -7_400;", "16,000", "7,400"),
    ("LifetimeNetLaunderSummaryLine", "lifetimeNetLaunder", "lifetime net launder", "lifetime_net_launder_summary_line",
     "lab.Ledger.Lifetime.Laundered = 21_000;\n            lab.Ledger.Lifetime.Revenue = 520_000;", "21,000", "520,000"),
    ("LastWeekSiteTrainingSummaryLine", "lastWeekSiteTraining", "last week site training", "last_week_site_training_summary_line",
     "lab.Ledger.LastWeek.SiteBuild = -12_500;\n            lab.Ledger.LastWeek.Training = -8_000;", "12,500", "8,000"),
    ("ThisWeekGreyChipSummaryLine", "thisWeekGreyChip", "this week grey chip", "this_week_grey_chip_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_100;\n            lab.Ledger.ThisWeek.ChipSales = 5_200;", "2,100", "5,200"),
    ("LastWeekPowerNetSummaryLine", "lastWeekPowerNet", "last week power net", "last_week_power_net_summary_line",
     "lab.Ledger.LastWeek.Power = -3_900;\n            lab.Ledger.LastWeek.Net = 2_500;", "3,900", "2,500"),
]

SPRINT20 = [
    ("ThisWeekConstructionGreySummaryLine", "thisWeekConstructionGrey", "this week construction grey", "this_week_construction_grey_summary_line",
     "lab.Ledger.ThisWeek.Construction = -4_200;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_700;", "4,200", "1,700"),
    ("LastWeekGpuLandSummaryLine", "lastWeekGpuLand", "last week gpu land", "last_week_gpu_land_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_400;\n            lab.Ledger.LastWeek.Land = -18_000;", "2,400", "18,000"),
    ("LifetimePowerChipSummaryLine", "lifetimePowerChip", "lifetime power chip", "lifetime_power_chip_summary_line",
     "lab.Ledger.Lifetime.Power = -120_000;\n            lab.Ledger.Lifetime.ChipSales = 200_000;", "120,000", "200,000"),
    ("ThisWeekGpuGreySummaryLine", "thisWeekGpuGrey", "this week gpu grey", "this_week_gpu_grey_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_950;\n            lab.Ledger.ThisWeek.GreyRevenue = 1_300;", "1,950", "1,300"),
    ("LastWeekPatentNetSummaryLine", "lastWeekPatentNet", "last week patent net", "last_week_patent_net_summary_line",
     "lab.Ledger.LastWeek.Revenue = 22_000;\n            lab.Ledger.LastWeek.Net = 3_600;", "22,000", "3,600"),
    ("ThisWeekTrainingGrantSummaryLine", "thisWeekTrainingGrant", "this week training grant", "this_week_training_grant_summary_line",
     "lab.Ledger.ThisWeek.Training = -5_800;\n            lab.Ledger.ThisWeek.Grants = 6_500;", "5,800", "6,500"),
]

PA19 = '            string thisWeekChipFinesPart = string.IsNullOrEmpty(thisWeekChipFines) ? "" : " · " + thisWeekChipFines;'
PA20 = '            string lastWeekPowerNetPart = string.IsNullOrEmpty(lastWeekPowerNet) ? "" : " · " + lastWeekPowerNet;'


def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    subprocess.check_call(["git", "commit", "-m", msg], cwd=ROOT)


def cycle_def(t):
    method, var, slug, test_name, ledger, a, b = t
    test_body = f"""            var lab = new LabState {{ Id = 1 }};
            {ledger}
            string line = HqDisplayQueriesHud.{method}(lab);
            Assert.That(line, Does.Contain("{a}").Or.Contain("{b}"));"""
    return dict(method=method, var=var, slug=slug, test_name=test_name, test_body=test_body)


def wire_hud(c):
    t = HUD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                if (!string.IsNullOrEmpty({c['var']}))
                    status = string.IsNullOrEmpty(status) ? {c['var']} : status + " · " + {c['var']};
"""
    b = """                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);"""
    HUD.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_train(c):
    t = TRAIN.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""                    string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
                    if (!string.IsNullOrEmpty({c['var']}))
                        _summary.text += " · " + {c['var']};
"""
    b = """                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);"""
    TRAIN.write_text(t.replace(b, ins.rstrip("\n") + "\n" + b, 1))
    return True


def wire_staff(c, part_anchor, tail_token):
    t = STAFF.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(lab)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(lab);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*)", t)
    t = t.replace(m.group(1), m.group(1) + f" + {c['var']}Part", 1)
    STAFF.write_text(t)
    return True


def wire_mascot(c, part_anchor, tail_token):
    t = MASCOT.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(sim.Player)" in t:
        return False
    ins = f"""            string {c['var']} = HqDisplayQueriesHud.{c['method']}(sim.Player);
            string {c['var']}Part = string.IsNullOrEmpty({c['var']}) ? "" : " · " + {c['var']};
"""
    t = t.replace(part_anchor, part_anchor + "\n" + ins.rstrip("\n"), 1)
    m = re.search(rf"(\+ {tail_token}(?: \+ \w+Part)*);", t)
    t = t.replace(m.group(1) + ";", m.group(1) + f" + {c['var']}Part;", 1)
    MASCOT.write_text(t)
    return True


def wire_build(c):
    t = BUILD.read_text()
    if f"HqDisplayQueriesHud.{c['method']}(_host.Simulation.Player)" in t:
        return False
    b = '                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);'
    line = f'                                 + " · " + HqDisplayQueriesHud.{c["method"]}(_host.Simulation.Player)\n'
    BUILD.write_text(t.replace(b, line + b, 1))
    return True


def add_test(c):
    t = TESTS.read_text()
    if c["test_name"] in t:
        return False
    block = f"\n        [Test]\n        public void {c['test_name']}()\n        {{\n{c['test_body']}\n        }}\n"
    at = t.rfind("\n    }\n}")
    TESTS.write_text(t[:at] + block + t[at:])
    return True


def run_sprint(cycles, part_anchor, tail_token):
    for i, raw in enumerate(cycles, 1):
        c = cycle_def(raw)
        if wire_hud(c):
            git_commit(f"feat(hud): wire hud view {c['slug']}", [HUD])
        if wire_train(c):
            git_commit(f"feat(hud): wire training {c['slug']}", [TRAIN])
        if wire_staff(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire staff gpu {c['slug']}", [STAFF])
        if wire_mascot(c, part_anchor, tail_token):
            git_commit(f"feat(hud): wire build mascot {c['slug']}", [MASCOT])
        if wire_build(c):
            git_commit(f"feat(hud): wire build panel {c['slug']}", [BUILD])
        if add_test(c):
            git_commit(f"test(hud): {c['slug']} summary line", [TESTS])
        if i in {2, 4}:
            subprocess.check_call(
                ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
                cwd=ROOT,
            )


subprocess.check_call(["python3", "/tmp/gen_formatters_19_20.py"])
subprocess.check_call(["git", "add", str(UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs")], cwd=ROOT)
subprocess.check_call(["git", "commit", "-m", "feat(hud): sprint19-20 twelve ledger summary formatters"], cwd=ROOT)
run_sprint(SPRINT19, PA19, "thisWeekChipFinesPart")
run_sprint(SPRINT20, PA20, "lastWeekPowerNetPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)