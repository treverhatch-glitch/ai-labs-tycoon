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

F153_154 = [
    ("ThisWeekTrainingGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekTrainingGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeTrainingGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRoomGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRoomGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRoomGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGpuGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGpuGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGpuGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRevenueFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekRevenueFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeRevenueFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

F155_156 = [
    ("ThisWeekConstructionGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekConstructionGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeConstructionGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHiringGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "ThisWeekLedgerHiringLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHiringGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("hiring", "LifetimeLedgerHiringLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSalariesRoomSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LastWeekSalariesRoomSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
    ]),
    ("LifetimeSalariesRoomSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
    ]),
    ("ThisWeekGrantTrainingSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekGrantTrainingSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeGrantTrainingSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("training", "LifetimeLedgerTrainingSpendLine(lab)"),
    ]),
]

SPRINT153 = [
    ("ThisWeekTrainingGrantSpendIncomeSummaryLine", "thisWeekTrainingGrantSpendIncome", "this week training grant spend income", "this_week_training_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Training = -940;\n            lab.Ledger.ThisWeek.Grants = 5_480;", "940", "5,480"),
    ("LastWeekTrainingGrantSpendIncomeSummaryLine", "lastWeekTrainingGrantSpendIncome", "last week training grant spend income", "last_week_training_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Training = -930;\n            lab.Ledger.LastWeek.Grants = 6_350;", "930", "6,350"),
    ("LifetimeTrainingGrantSpendIncomeSummaryLine", "lifetimeTrainingGrantSpendIncome", "lifetime training grant spend income", "lifetime_training_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Training = -54_500;\n            lab.Ledger.Lifetime.Grants = 208_000;", "54,500", "208,000"),
    ("ThisWeekRoomGrantSpendIncomeSummaryLine", "thisWeekRoomGrantSpendIncome", "this week room grant spend income", "this_week_room_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -1_130;\n            lab.Ledger.ThisWeek.Grants = 5_490;", "1,130", "5,490"),
    ("LastWeekRoomGrantSpendIncomeSummaryLine", "lastWeekRoomGrantSpendIncome", "last week room grant spend income", "last_week_room_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -1_110;\n            lab.Ledger.LastWeek.Grants = 6_360;", "1,110", "6,360"),
    ("LifetimeRoomGrantSpendIncomeSummaryLine", "lifetimeRoomGrantSpendIncome", "lifetime room grant spend income", "lifetime_room_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.RoomUpkeep = -42_000;\n            lab.Ledger.Lifetime.Grants = 209_000;", "42,000", "209,000"),
]

SPRINT154 = [
    ("ThisWeekGpuGrantSpendIncomeSummaryLine", "thisWeekGpuGrantSpendIncome", "this week gpu grant spend income", "this_week_gpu_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_240;\n            lab.Ledger.ThisWeek.Grants = 5_500;", "1,240", "5,500"),
    ("LastWeekGpuGrantSpendIncomeSummaryLine", "lastWeekGpuGrantSpendIncome", "last week gpu grant spend income", "last_week_gpu_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_220;\n            lab.Ledger.LastWeek.Grants = 6_370;", "1,220", "6,370"),
    ("LifetimeGpuGrantSpendIncomeSummaryLine", "lifetimeGpuGrantSpendIncome", "lifetime gpu grant spend income", "lifetime_gpu_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -176_000;\n            lab.Ledger.Lifetime.Grants = 210_000;", "176,000", "210,000"),
    ("ThisWeekRevenueFinesSpendIncomeSummaryLine", "thisWeekRevenueFinesSpendIncome", "this week revenue fines spend income", "this_week_revenue_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_890;\n            lab.Ledger.ThisWeek.Fines = -1_130;", "18,890", "1,130"),
    ("LastWeekRevenueFinesSpendIncomeSummaryLine", "lastWeekRevenueFinesSpendIncome", "last week revenue fines spend income", "last_week_revenue_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_480;\n            lab.Ledger.LastWeek.Fines = -1_140;", "18,480", "1,140"),
    ("LifetimeRevenueFinesSpendIncomeSummaryLine", "lifetimeRevenueFinesSpendIncome", "lifetime revenue fines spend income", "lifetime_revenue_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 456_000;\n            lab.Ledger.Lifetime.Fines = -29_400;", "456,000", "29,400"),
]

SPRINT155 = [
    ("ThisWeekConstructionGrantSpendIncomeSummaryLine", "thisWeekConstructionGrantSpendIncome", "this week construction grant spend income", "this_week_construction_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_280;\n            lab.Ledger.ThisWeek.Grants = 5_510;", "2,280", "5,510"),
    ("LastWeekConstructionGrantSpendIncomeSummaryLine", "lastWeekConstructionGrantSpendIncome", "last week construction grant spend income", "last_week_construction_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_260;\n            lab.Ledger.LastWeek.Grants = 6_380;", "2,260", "6,380"),
    ("LifetimeConstructionGrantSpendIncomeSummaryLine", "lifetimeConstructionGrantSpendIncome", "lifetime construction grant spend income", "lifetime_construction_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -90_000;\n            lab.Ledger.Lifetime.Grants = 211_000;", "90,000", "211,000"),
    ("ThisWeekHiringGrantSpendIncomeSummaryLine", "thisWeekHiringGrantSpendIncome", "this week hiring grant spend income", "this_week_hiring_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -1_640;\n            lab.Ledger.ThisWeek.Grants = 5_520;", "1,640", "5,520"),
    ("LastWeekHiringGrantSpendIncomeSummaryLine", "lastWeekHiringGrantSpendIncome", "last week hiring grant spend income", "last_week_hiring_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -1_620;\n            lab.Ledger.LastWeek.Grants = 6_390;", "1,620", "6,390"),
    ("LifetimeHiringGrantSpendIncomeSummaryLine", "lifetimeHiringGrantSpendIncome", "lifetime hiring grant spend income", "lifetime_hiring_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hiring = -61_000;\n            lab.Ledger.Lifetime.Grants = 212_000;", "61,000", "212,000"),
]

SPRINT156 = [
    ("ThisWeekSalariesRoomSpendIncomeSummaryLine", "thisWeekSalariesRoomSpendIncome", "this week salaries room spend income", "this_week_salaries_room_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -3_520;\n            lab.Ledger.ThisWeek.RoomUpkeep = -1_140;", "3,520", "1,140"),
    ("LastWeekSalariesRoomSpendIncomeSummaryLine", "lastWeekSalariesRoomSpendIncome", "last week salaries room spend income", "last_week_salaries_room_spend_income_summary_line",
     "lab.Ledger.LastWeek.Salaries = -3_500;\n            lab.Ledger.LastWeek.RoomUpkeep = -1_120;", "3,500", "1,120"),
    ("LifetimeSalariesRoomSpendIncomeSummaryLine", "lifetimeSalariesRoomSpendIncome", "lifetime salaries room spend income", "lifetime_salaries_room_spend_income_summary_line",
     "lab.Ledger.Lifetime.Salaries = -142_000;\n            lab.Ledger.Lifetime.RoomUpkeep = -42_500;", "142,000", "42,500"),
    ("ThisWeekGrantTrainingSpendIncomeSummaryLine", "thisWeekGrantTrainingSpendIncome", "this week grant training spend income", "this_week_grant_training_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_530;\n            lab.Ledger.ThisWeek.Training = -950;", "5,530", "950"),
    ("LastWeekGrantTrainingSpendIncomeSummaryLine", "lastWeekGrantTrainingSpendIncome", "last week grant training spend income", "last_week_grant_training_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_400;\n            lab.Ledger.LastWeek.Training = -940;", "6,400", "940"),
    ("LifetimeGrantTrainingSpendIncomeSummaryLine", "lifetimeGrantTrainingSpendIncome", "lifetime grant training spend income", "lifetime_grant_training_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 213_000;\n            lab.Ledger.Lifetime.Training = -55_000;", "213,000", "55,000"),
]

PA153 = '            string lifetimeSiteGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeSiteGrantSpendIncome) ? "" : " · " + lifetimeSiteGrantSpendIncome;'
PA154 = '            string lifetimeTrainingGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeTrainingGrantSpendIncome) ? "" : " · " + lifetimeTrainingGrantSpendIncome;'
PA155 = '            string lifetimeRoomGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeRoomGrantSpendIncome) ? "" : " · " + lifetimeRoomGrantSpendIncome;'
PA156 = '            string lifetimeGpuGrantSpendIncomePart = string.IsNullOrEmpty(lifetimeGpuGrantSpendIncome) ? "" : " · " + lifetimeGpuGrantSpendIncome;'

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



insert_formatters(F153_154)
git_commit("feat(hud): sprint153-154 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT153, PA153, "lifetimeSiteGrantSpendIncome")
run_sprint(SPRINT154, PA154, "lifetimeTrainingGrantSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F155_156)
git_commit("feat(hud): sprint155-156 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT155, PA155, "lifetimeRoomGrantSpendIncome")
run_sprint(SPRINT156, PA156, "lifetimeGpuGrantSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
