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












F109_110 = [
    ("ThisWeekTrainingIncomeSpendSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekTrainingIncomeSpendSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeTrainingIncomeSpendSummaryLine", [
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekPowerIncomeSpendSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekPowerIncomeSpendSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimePowerIncomeSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekRentIncomeSpendSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekRentIncomeSpendSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeRentIncomeSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekConstructionIncomeSpendSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekConstructionIncomeSpendSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeConstructionIncomeSpendSummaryLine", [
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

F111_112 = [
    ("ThisWeekSalariesIncomeSpendSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekSalariesIncomeSpendSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeSalariesIncomeSpendSummaryLine", [
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekHiringIncomeSpendSummaryLine", [
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekHiringIncomeSpendSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeHiringIncomeSpendSummaryLine", [
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekLandIncomeSpendSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekLandIncomeSpendSummaryLine", [
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeLandIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
    ("ThisWeekRoomIncomeSpendSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LastWeekRoomIncomeSpendSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
    ]),
    ("LifetimeRoomIncomeSpendSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
    ]),
]

SPRINT109 = [
    ("ThisWeekTrainingIncomeSpendSummaryLine", "thisWeekTrainingIncomeSpend", "this week training income spend", "this_week_training_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_940;\n            lab.Ledger.ThisWeek.Grants = 5_090;", "2,940", "5,090"),
    ("LastWeekTrainingIncomeSpendSummaryLine", "lastWeekTrainingIncomeSpend", "last week training income spend", "last_week_training_income_spend_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_210;\n            lab.Ledger.LastWeek.Fines = -790;", "18,210", "790"),
    ("LifetimeTrainingIncomeSpendSummaryLine", "lifetimeTrainingIncomeSpend", "lifetime training income spend", "lifetime_training_income_spend_summary_line",
     "lab.Ledger.Lifetime.Training = -158_000;\n            lab.Ledger.Lifetime.Net = 110_500;", "158,000", "110,500"),
    ("ThisWeekPowerIncomeSpendSummaryLine", "thisWeekPowerIncomeSpend", "this week power income spend", "this_week_power_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_300;\n            lab.Ledger.ThisWeek.Revenue = 18_730;", "3,300", "18,730"),
    ("LastWeekPowerIncomeSpendSummaryLine", "lastWeekPowerIncomeSpend", "last week power income spend", "last_week_power_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 3_010;\n            lab.Ledger.LastWeek.Grants = 6_080;", "3,010", "6,080"),
    ("LifetimePowerIncomeSpendSummaryLine", "lifetimePowerIncomeSpend", "lifetime power income spend", "lifetime_power_income_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -164_000;\n            lab.Ledger.Lifetime.Grants = 172_000;", "164,000", "172,000"),
]

SPRINT110 = [
    ("ThisWeekRentIncomeSpendSummaryLine", "thisWeekRentIncomeSpend", "this week rent income spend", "this_week_rent_income_spend_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_180;\n            lab.Ledger.ThisWeek.Fines = -790;", "2,180", "790"),
    ("LastWeekRentIncomeSpendSummaryLine", "lastWeekRentIncomeSpend", "last week rent income spend", "last_week_rent_income_spend_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_420;\n            lab.Ledger.LastWeek.Revenue = 18_220;", "2,420", "18,220"),
    ("LifetimeRentIncomeSpendSummaryLine", "lifetimeRentIncomeSpend", "lifetime rent income spend", "lifetime_rent_income_spend_summary_line",
     "lab.Ledger.Lifetime.Revenue = 438_000;\n            lab.Ledger.Lifetime.Net = 111_000;", "438,000", "111,000"),
    ("ThisWeekConstructionIncomeSpendSummaryLine", "thisWeekConstructionIncomeSpend", "this week construction income spend", "this_week_construction_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Construction = -9_660;\n            lab.Ledger.ThisWeek.Grants = 5_100;", "9,660", "5,100"),
    ("LastWeekConstructionIncomeSpendSummaryLine", "lastWeekConstructionIncomeSpend", "last week construction income spend", "last_week_construction_income_spend_summary_line",
     "lab.Ledger.LastWeek.Construction = -10_560;\n            lab.Ledger.LastWeek.Net = 3_020;", "10,560", "3,020"),
    ("LifetimeConstructionIncomeSpendSummaryLine", "lifetimeConstructionIncomeSpend", "lifetime construction income spend", "lifetime_construction_income_spend_summary_line",
     "lab.Ledger.Lifetime.Construction = -84_400;\n            lab.Ledger.Lifetime.Grants = 173_000;", "84,400", "173,000"),
]

SPRINT111 = [
    ("ThisWeekSalariesIncomeSpendSummaryLine", "thisWeekSalariesIncomeSpend", "this week salaries income spend", "this_week_salaries_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_880;\n            lab.Ledger.ThisWeek.Net = 3_310;", "7,880", "3,310"),
    ("LastWeekSalariesIncomeSpendSummaryLine", "lastWeekSalariesIncomeSpend", "last week salaries income spend", "last_week_salaries_income_spend_summary_line",
     "lab.Ledger.LastWeek.Salaries = -8_100;\n            lab.Ledger.LastWeek.Revenue = 18_230;", "8,100", "18,230"),
    ("LifetimeSalariesIncomeSpendSummaryLine", "lifetimeSalariesIncomeSpend", "lifetime salaries income spend", "lifetime_salaries_income_spend_summary_line",
     "lab.Ledger.Lifetime.Salaries = -419_000;\n            lab.Ledger.Lifetime.Net = 111_500;", "419,000", "111,500"),
    ("ThisWeekHiringIncomeSpendSummaryLine", "thisWeekHiringIncomeSpend", "this week hiring income spend", "this_week_hiring_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -3_170;\n            lab.Ledger.ThisWeek.Grants = 5_110;", "3,170", "5,110"),
    ("LastWeekHiringIncomeSpendSummaryLine", "lastWeekHiringIncomeSpend", "last week hiring income spend", "last_week_hiring_income_spend_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_370;\n            lab.Ledger.LastWeek.Fines = -800;", "3,370", "800"),
    ("LifetimeHiringIncomeSpendSummaryLine", "lifetimeHiringIncomeSpend", "lifetime hiring income spend", "lifetime_hiring_income_spend_summary_line",
     "lab.Ledger.Lifetime.Hiring = -62_400;\n            lab.Ledger.Lifetime.Revenue = 439_000;", "62,400", "439,000"),
]

SPRINT112 = [
    ("ThisWeekLandIncomeSpendSummaryLine", "thisWeekLandIncomeSpend", "this week land income spend", "this_week_land_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -800;\n            lab.Ledger.ThisWeek.Revenue = 18_740;", "800", "18,740"),
    ("LastWeekLandIncomeSpendSummaryLine", "lastWeekLandIncomeSpend", "last week land income spend", "last_week_land_income_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -800;\n            lab.Ledger.LastWeek.Grants = 6_090;", "800", "6,090"),
    ("LifetimeLandIncomeSpendSummaryLine", "lifetimeLandIncomeSpend", "lifetime land income spend", "lifetime_land_income_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -27_500;\n            lab.Ledger.Lifetime.Grants = 174_000;", "27,500", "174,000"),
    ("ThisWeekRoomIncomeSpendSummaryLine", "thisWeekRoomIncomeSpend", "this week room income spend", "this_week_room_income_spend_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_950;\n            lab.Ledger.ThisWeek.Net = 3_320;", "2,950", "3,320"),
    ("LastWeekRoomIncomeSpendSummaryLine", "lastWeekRoomIncomeSpend", "last week room income spend", "last_week_room_income_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 3_030;\n            lab.Ledger.LastWeek.Revenue = 18_240;", "3,030", "18,240"),
    ("LifetimeRoomIncomeSpendSummaryLine", "lifetimeRoomIncomeSpend", "lifetime room income spend", "lifetime_room_income_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 112_000;\n            lab.Ledger.Lifetime.Revenue = 440_000;", "112,000", "440,000"),
]

PA109 = '            string lifetimeDevIncomeSpendPart = string.IsNullOrEmpty(lifetimeDevIncomeSpend) ? "" : " · " + lifetimeDevIncomeSpend;'
PA110 = '            string lifetimePowerIncomeSpendPart = string.IsNullOrEmpty(lifetimePowerIncomeSpend) ? "" : " · " + lifetimePowerIncomeSpend;'
PA111 = '            string lifetimeConstructionIncomeSpendPart = string.IsNullOrEmpty(lifetimeConstructionIncomeSpend) ? "" : " · " + lifetimeConstructionIncomeSpend;'
PA112 = '            string lifetimeHiringIncomeSpendPart = string.IsNullOrEmpty(lifetimeHiringIncomeSpend) ? "" : " · " + lifetimeHiringIncomeSpend;'


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


insert_formatters(F109_110)
git_commit("feat(hud): sprint109-110 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT109, PA109, "lifetimeDevIncomeSpendPart")
run_sprint(SPRINT110, PA110, "lifetimePowerIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F111_112)
git_commit("feat(hud): sprint111-112 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT111, PA111, "lifetimeConstructionIncomeSpendPart")
run_sprint(SPRINT112, PA112, "lifetimeHiringIncomeSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
