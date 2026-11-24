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








F93_94 = [
    ("ThisWeekPatentIncomeSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekPatentIncomeSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimePatentIncomeSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekUpkeepIncomeSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekUpkeepIncomeSummaryLine", [
        ("upkeep", "LastWeekLedgerGpuUpkeepLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeUpkeepIncomeSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHiringIncomeSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekHiringIncomeSummaryLine", [
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeHiringIncomeSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekNetSpendSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekNetSpendSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeNetSpendSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
]

F95_96 = [
    ("ThisWeekGreySpendSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekGreySpendSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeGreySpendSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
    ]),
    ("ThisWeekFinesSpendSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekFinesSpendSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeFinesSpendSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRentSpendSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRentSpendSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRentSpendSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekGrantSpendSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantSpendSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGrantSpendSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

SPRINT93 = [
    ("ThisWeekPatentIncomeSummaryLine", "thisWeekPatentIncome", "this week patent income", "this_week_patent_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -670;\n            lab.Ledger.ThisWeek.Grants = 4_920;", "670", "4,920"),
    ("LastWeekPatentIncomeSummaryLine", "lastWeekPatentIncome", "last week patent income", "last_week_patent_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_940;\n            lab.Ledger.LastWeek.Fines = -670;", "5,940", "670"),
    ("LifetimePatentIncomeSummaryLine", "lifetimePatentIncome", "lifetime patent income", "lifetime_patent_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_400;\n            lab.Ledger.Lifetime.Grants = 158_000;", "26,400", "158,000"),
    ("ThisWeekUpkeepIncomeSummaryLine", "thisWeekUpkeepIncome", "this week upkeep income", "this_week_upkeep_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_160;\n            lab.Ledger.ThisWeek.Net = 3_180;", "2,160", "3,180"),
    ("LastWeekUpkeepIncomeSummaryLine", "lastWeekUpkeepIncome", "last week upkeep income", "last_week_upkeep_income_summary_line",
     "lab.Ledger.LastWeek.GpuUpkeep = -2_020;\n            lab.Ledger.LastWeek.Revenue = 18_060;", "2,020", "18,060"),
    ("LifetimeUpkeepIncomeSummaryLine", "lifetimeUpkeepIncome", "lifetime upkeep income", "lifetime_upkeep_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 428_000;\n            lab.Ledger.Lifetime.Net = 101_000;", "428,000", "101,000"),
]

SPRINT94 = [
    ("ThisWeekHiringIncomeSummaryLine", "thisWeekHiringIncome", "this week hiring income", "this_week_hiring_income_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_860;\n            lab.Ledger.ThisWeek.Grants = 4_940;", "7,860", "4,940"),
    ("LastWeekHiringIncomeSummaryLine", "lastWeekHiringIncome", "last week hiring income", "last_week_hiring_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -3_150;\n            lab.Ledger.LastWeek.Grants = 5_960;", "3,150", "5,960"),
    ("LifetimeHiringIncomeSummaryLine", "lifetimeHiringIncome", "lifetime hiring income", "lifetime_hiring_income_summary_line",
     "lab.Ledger.Lifetime.Training = -157_000;\n            lab.Ledger.Lifetime.Grants = 159_000;", "157,000", "159,000"),
    ("ThisWeekNetSpendSummaryLine", "thisWeekNetSpend", "this week net spend", "this_week_net_spend_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_190;\n            lab.Ledger.ThisWeek.Power = -2_850;", "3,190", "2,850"),
    ("LastWeekNetSpendSummaryLine", "lastWeekNetSpend", "last week net spend", "last_week_net_spend_summary_line",
     "lab.Ledger.LastWeek.Net = 2_860;\n            lab.Ledger.LastWeek.Fines = -680;", "2,860", "680"),
    ("LifetimeNetSpendSummaryLine", "lifetimeNetSpend", "lifetime net spend", "lifetime_net_spend_summary_line",
     "lab.Ledger.Lifetime.Net = 102_000;\n            lab.Ledger.Lifetime.Fines = -26_500;", "102,000", "26,500"),
]

SPRINT95 = [
    ("ThisWeekGreySpendSummaryLine", "thisWeekGreySpend", "this week grey spend", "this_week_grey_spend_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_060;\n            lab.Ledger.ThisWeek.Fines = -680;", "2,060", "680"),
    ("LastWeekGreySpendSummaryLine", "lastWeekGreySpend", "last week grey spend", "last_week_grey_spend_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_580;\n            lab.Ledger.LastWeek.Laundered = 3_400;", "2,580", "3,400"),
    ("LifetimeGreySpendSummaryLine", "lifetimeGreySpend", "lifetime grey spend", "lifetime_grey_spend_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_800;\n            lab.Ledger.Lifetime.Laundered = 32_500;", "52,800", "32,500"),
    ("ThisWeekFinesSpendSummaryLine", "thisWeekFinesSpend", "this week fines spend", "this_week_fines_spend_summary_line",
     "lab.Ledger.ThisWeek.Fines = -690;\n            lab.Ledger.ThisWeek.Net = 3_200;", "690", "3,200"),
    ("LastWeekFinesSpendSummaryLine", "lastWeekFinesSpend", "last week fines spend", "last_week_fines_spend_summary_line",
     "lab.Ledger.LastWeek.Fines = -690;\n            lab.Ledger.LastWeek.Net = 2_870;", "690", "2,870"),
    ("LifetimeFinesSpendSummaryLine", "lifetimeFinesSpend", "lifetime fines spend", "lifetime_fines_spend_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_600;\n            lab.Ledger.Lifetime.Grants = 160_000;", "26,600", "160,000"),
]

SPRINT96 = [
    ("ThisWeekRentSpendSummaryLine", "thisWeekRentSpend", "this week rent spend", "this_week_rent_spend_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_170;\n            lab.Ledger.ThisWeek.Revenue = 18_650;", "2,170", "18,650"),
    ("LastWeekRentSpendSummaryLine", "lastWeekRentSpend", "last week rent spend", "last_week_rent_spend_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_410;\n            lab.Ledger.LastWeek.Grants = 5_980;", "2,410", "5,980"),
    ("LifetimeRentSpendSummaryLine", "lifetimeRentSpend", "lifetime rent spend", "lifetime_rent_spend_summary_line",
     "lab.Ledger.Lifetime.Power = -162_000;\n            lab.Ledger.Lifetime.Revenue = 429_000;", "162,000", "429,000"),
    ("ThisWeekGrantSpendSummaryLine", "thisWeekGrantSpend", "this week grant spend", "this_week_grant_spend_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_960;\n            lab.Ledger.ThisWeek.Net = 3_210;", "4,960", "3,210"),
    ("LastWeekGrantSpendSummaryLine", "lastWeekGrantSpend", "last week grant spend", "last_week_grant_spend_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_000;\n            lab.Ledger.LastWeek.Revenue = 18_080;", "6,000", "18,080"),
    ("LifetimeGrantSpendSummaryLine", "lifetimeGrantSpend", "lifetime grant spend", "lifetime_grant_spend_summary_line",
     "lab.Ledger.Lifetime.Grants = 161_000;\n            lab.Ledger.Lifetime.Net = 103_000;", "161,000", "103,000"),
]

PA93 = '            string lifetimeRoomIncomePart = string.IsNullOrEmpty(lifetimeRoomIncome) ? "" : " · " + lifetimeRoomIncome;'
PA94 = '            string lifetimeUpkeepIncomePart = string.IsNullOrEmpty(lifetimeUpkeepIncome) ? "" : " · " + lifetimeUpkeepIncome;'
PA95 = '            string lifetimeNetSpendPart = string.IsNullOrEmpty(lifetimeNetSpend) ? "" : " · " + lifetimeNetSpend;'
PA96 = '            string lifetimeFinesSpendPart = string.IsNullOrEmpty(lifetimeFinesSpend) ? "" : " · " + lifetimeFinesSpend;'


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


insert_formatters(F93_94)
git_commit("feat(hud): sprint93-94 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT93, PA93, "lifetimeRoomIncomePart")
run_sprint(SPRINT94, PA94, "lifetimeUpkeepIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F95_96)
git_commit("feat(hud): sprint95-96 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT95, PA95, "lifetimeNetSpendPart")
run_sprint(SPRINT96, PA96, "lifetimeFinesSpendPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
