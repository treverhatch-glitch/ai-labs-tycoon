#!/usr/bin/env python3
import re, subprocess
from pathlib import Path

ROOT = Path("/agent/repos/ai-labs-tycoon")
UNITY = ROOT / "unity/Assets"
HUD_PATH = UNITY / "Scripts/Hq/HqDisplayQueriesHud.cs"
HUD, TRAIN = UNITY / "Scripts/Hq/HqHudView.cs", UNITY / "Scripts/Hq/TrainingRunPanel.cs"
STAFF, MASCOT = UNITY / "Scripts/Hq/StaffGpuPanel.cs", UNITY / "Scripts/Hq/HqMascotStripHud.cs"
BUILD = UNITY / "Scripts/Hq/RoomBuildPanel.cs"
TESTS = UNITY / "Editor/Tests/HqDisplayQueriesHudTests.cs"

F141_142 = [
    ("ThisWeekNetGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekNetGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeNetGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGreyGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGreyGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGreyGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekConstructionCapitalSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LastWeekConstructionCapitalSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
    ]),
    ("LifetimeConstructionCapitalSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRoomTrainingSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekRoomTrainingSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeRoomTrainingSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
]

F143_144 = [
    ("ThisWeekGpuRevenueSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekGpuRevenueSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGpuRevenueSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekChipGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekChipGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeChipGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLaunderGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekLaunderGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeLaunderGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekPatentFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekPatentFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimePatentFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT141 = [
    ("ThisWeekNetGrantSpendIncomeSummaryLine", "thisWeekNetGrantSpendIncome", "this week net grant spend income", "this_week_net_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_520;\n            lab.Ledger.ThisWeek.Grants = 5_360;", "3,520", "5,360"),
    ("LastWeekNetGrantSpendIncomeSummaryLine", "lastWeekNetGrantSpendIncome", "last week net grant spend income", "last_week_net_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_530;\n            lab.Ledger.LastWeek.Grants = 6_260;", "3,530", "6,260"),
    ("LifetimeNetGrantSpendIncomeSummaryLine", "lifetimeNetGrantSpendIncome", "lifetime net grant spend income", "lifetime_net_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 123_000;\n            lab.Ledger.Lifetime.Grants = 195_000;", "123,000", "195,000"),
    ("ThisWeekGreyGrantSpendIncomeSummaryLine", "thisWeekGreyGrantSpendIncome", "this week grey grant spend income", "this_week_grey_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_200;\n            lab.Ledger.ThisWeek.Grants = 5_370;", "2,200", "5,370"),
    ("LastWeekGreyGrantSpendIncomeSummaryLine", "lastWeekGreyGrantSpendIncome", "last week grey grant spend income", "last_week_grey_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_720;\n            lab.Ledger.LastWeek.Grants = 6_270;", "2,720", "6,270"),
    ("LifetimeGreyGrantSpendIncomeSummaryLine", "lifetimeGreyGrantSpendIncome", "lifetime grey grant spend income", "lifetime_grey_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_800;\n            lab.Ledger.Lifetime.Grants = 196_000;", "53,800", "196,000"),
]

SPRINT142 = [
    ("ThisWeekConstructionCapitalSpendIncomeSummaryLine", "thisWeekConstructionCapitalSpendIncome", "this week construction capital spend income", "this_week_construction_capital_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_240;\n            lab.Ledger.ThisWeek.Net = 3_540;", "2,240", "3,540"),
    ("LastWeekConstructionCapitalSpendIncomeSummaryLine", "lastWeekConstructionCapitalSpendIncome", "last week construction capital spend income", "last_week_construction_capital_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_220;\n            lab.Ledger.LastWeek.Revenue = 18_440;", "2,220", "18,440"),
    ("LifetimeConstructionCapitalSpendIncomeSummaryLine", "lifetimeConstructionCapitalSpendIncome", "lifetime construction capital spend income", "lifetime_construction_capital_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -87_500;\n            lab.Ledger.Lifetime.Grants = 197_000;", "87,500", "197,000"),
    ("ThisWeekRoomTrainingSpendIncomeSummaryLine", "thisWeekRoomTrainingSpendIncome", "this week room training spend income", "this_week_room_training_spend_income_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -1_120;\n            lab.Ledger.ThisWeek.Training = -920;", "1,120", "920"),
    ("LastWeekRoomTrainingSpendIncomeSummaryLine", "lastWeekRoomTrainingSpendIncome", "last week room training spend income", "last_week_room_training_spend_income_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -1_100;\n            lab.Ledger.LastWeek.Training = -900;", "1,100", "900"),
    ("LifetimeRoomTrainingSpendIncomeSummaryLine", "lifetimeRoomTrainingSpendIncome", "lifetime room training spend income", "lifetime_room_training_spend_income_summary_line",
     "lab.Ledger.Lifetime.RoomUpkeep = -41_500;\n            lab.Ledger.Lifetime.Training = -54_000;", "41,500", "54,000"),
]

SPRINT143 = [
    ("ThisWeekGpuRevenueSpendIncomeSummaryLine", "thisWeekGpuRevenueSpendIncome", "this week gpu revenue spend income", "this_week_gpu_revenue_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_230;\n            lab.Ledger.ThisWeek.Revenue = 18_870;", "1,230", "18,870"),
    ("LastWeekGpuRevenueSpendIncomeSummaryLine", "lastWeekGpuRevenueSpendIncome", "last week gpu revenue spend income", "last_week_gpu_revenue_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_210;\n            lab.Ledger.LastWeek.Revenue = 18_450;", "1,210", "18,450"),
    ("LifetimeGpuRevenueSpendIncomeSummaryLine", "lifetimeGpuRevenueSpendIncome", "lifetime gpu revenue spend income", "lifetime_gpu_revenue_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -173_000;\n            lab.Ledger.Lifetime.Revenue = 454_000;", "173,000", "454,000"),
    ("ThisWeekChipGrantSpendIncomeSummaryLine", "thisWeekChipGrantSpendIncome", "this week chip grant spend income", "this_week_chip_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_250;\n            lab.Ledger.ThisWeek.Grants = 5_380;", "8,250", "5,380"),
    ("LastWeekChipGrantSpendIncomeSummaryLine", "lastWeekChipGrantSpendIncome", "last week chip grant spend income", "last_week_chip_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_940;\n            lab.Ledger.LastWeek.Grants = 6_280;", "7,940", "6,280"),
    ("LifetimeChipGrantSpendIncomeSummaryLine", "lifetimeChipGrantSpendIncome", "lifetime chip grant spend income", "lifetime_chip_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 117_000;\n            lab.Ledger.Lifetime.Grants = 198_000;", "117,000", "198,000"),
]

SPRINT144 = [
    ("ThisWeekLaunderGreySpendIncomeSummaryLine", "thisWeekLaunderGreySpendIncome", "this week launder grey spend income", "this_week_launder_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 3_530;\n            lab.Ledger.ThisWeek.GreyRevenue = 2_210;", "3,530", "2,210"),
    ("LastWeekLaunderGreySpendIncomeSummaryLine", "lastWeekLaunderGreySpendIncome", "last week launder grey spend income", "last_week_launder_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.Laundered = 3_150;\n            lab.Ledger.LastWeek.GreyRevenue = 2_730;", "3,150", "2,730"),
    ("LifetimeLaunderGreySpendIncomeSummaryLine", "lifetimeLaunderGreySpendIncome", "lifetime launder grey spend income", "lifetime_launder_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.Laundered = 33_400;\n            lab.Ledger.Lifetime.GreyRevenue = 53_900;", "33,400", "53,900"),
    ("ThisWeekPatentFinesSpendIncomeSummaryLine", "thisWeekPatentFinesSpendIncome", "this week patent fines spend income", "this_week_patent_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_050;\n            lab.Ledger.ThisWeek.Grants = 5_390;", "1,050", "5,390"),
    ("LastWeekPatentFinesSpendIncomeSummaryLine", "lastWeekPatentFinesSpendIncome", "last week patent fines spend income", "last_week_patent_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = 4_230;\n            lab.Ledger.LastWeek.Fines = -1_060;", "4,230", "1,060"),
    ("LifetimePatentFinesSpendIncomeSummaryLine", "lifetimePatentFinesSpendIncome", "lifetime patent fines spend income", "lifetime_patent_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -28_900;\n            lab.Ledger.Lifetime.Grants = 199_000;", "28,900", "199,000"),
]

PA141 = '            string lifetimeHiringPatentSpendIncomePart = string.IsNullOrEmpty(lifetimeHiringPatentSpendIncome) ? "" : " · " + lifetimeHiringPatentSpendIncome;'
PA142 = '            string lifetimeGreyGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeGreyGrantSpendIncome) ? "" : " · " + lifetimeGreyGrantSpendIncome;'
PA143 = '            string lifetimeRoomTrainingSpendIncomePart = string.IsNullOrEmpty(lifetimeRoomTrainingSpendIncome) ? "" : " · " + lifetimeRoomTrainingSpendIncome;'
PA144 = '            string lifetimeChipGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeChipGrantSpendIncome) ? "" : " · " + lifetimeChipGrantSpendIncome;'

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
    t = HUD_PATH.read_text()
    block = ""
    for item in batch:
        name = item[0]
        if name in t:
            continue
        block += formatter_body(name, item[1])
    if not block:
        return
    HUD_PATH.write_text(
        t.replace(
            "        public static string BankruptLine(LabState lab)",
            block + "\n        public static string BankruptLine(LabState lab)",
            1,
        )
    )


def git_commit(msg, paths):
    subprocess.check_call(["git", "add", *map(str, paths)], cwd=ROOT)
    if subprocess.run(["git", "diff", "--cached", "--quiet"], cwd=ROOT).returncode == 0:
        return
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
    if not m:
        return False
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
    if not m:
        return False
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



insert_formatters(F141_142)
git_commit("feat(hud): sprint141-142 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT141, PA141, "lifetimeHiringPatentSpendIncome")
run_sprint(SPRINT142, PA142, "lifetimeGreyGrantSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F143_144)
git_commit("feat(hud): sprint143-144 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT143, PA143, "lifetimeRoomTrainingSpendIncome")
run_sprint(SPRINT144, PA144, "lifetimeChipGrantSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
