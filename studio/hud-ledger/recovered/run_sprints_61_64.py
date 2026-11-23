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

F61_62 = [
    ("ThisWeekLaunderRoomSummaryLine", [
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeRentLandSummaryLine", [
        ("gpuRent", "LifetimeLedgerGpuRentLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSalariesGrantSummaryLine", [
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekSiteChipSummaryLine", [
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekCapitalConstructionSummaryLine", [
        ("capital", "ThisWeekLedgerCapitalInLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeGrantLandSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRoomGrantSummaryLine", [
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
    ]),
    ("ThisWeekPatentRentSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekFinesRentSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeChipRoomSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("room", "LifetimeLedgerRoomUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekConstructionFinesSummaryLine", [
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeSalariesGrantSummaryLine", [
        ("salaries", "LifetimeLedgerSalariesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

F63_64 = [
    ("LastWeekRentGrantSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekLandChipSummaryLine", [
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekGreySiteSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekHardwareLandSummaryLine", [
        ("hardware", "ThisWeekLedgerHardwareLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekFinesLandSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekCapitalGrantSummaryLine", [
        ("capital", "LastWeekLedgerCapitalInLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekRoomGrantSummaryLine", [
        ("room", "ThisWeekLedgerRoomUpkeepLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("ThisWeekConstructionRentSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekSalariesLandSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekNetRoomSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("room", "LastWeekLedgerRoomUpkeepLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeGrantChipSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekGrantLaunderSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("laundered", "ThisWeekLedgerLaunderedLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
]

SPRINT61 = [
    ("ThisWeekLaunderRoomSummaryLine", "thisWeekLaunderRoom", "this week launder room", "this_week_launder_room_summary_line",
     "lab.Ledger.ThisWeek.Laundered = 2_600;\n            lab.Ledger.ThisWeek.RoomUpkeep = -2_220;", "2,600", "2,220"),
    ("LifetimeRentLandSummaryLine", "lifetimeRentLand", "lifetime rent land", "lifetime_rent_land_summary_line",
     "lab.Ledger.Lifetime.GpuRent = -104_000;\n            lab.Ledger.Lifetime.Land = -88_000;", "104,000", "88,000"),
    ("LastWeekSalariesGrantSummaryLine", "lastWeekSalariesGrant", "last week salaries grant", "last_week_salaries_grant_summary_line",
     "lab.Ledger.LastWeek.Salaries = -7_550;\n            lab.Ledger.LastWeek.Grants = 5_050;", "7,550", "5,050"),
    ("ThisWeekSiteChipSummaryLine", "thisWeekSiteChip", "this week site chip", "this_week_site_chip_summary_line",
     "lab.Ledger.ThisWeek.SiteBuild = -7_150;\n            lab.Ledger.ThisWeek.ChipSales = 5_300;", "7,150", "5,300"),
    ("ThisWeekCapitalConstructionSummaryLine", "thisWeekCapitalConstruction", "this week capital construction", "this_week_capital_construction_summary_line",
     "lab.Ledger.ThisWeek.CapitalIn = 47_500;\n            lab.Ledger.ThisWeek.Construction = -6_050;", "47,500", "6,050"),
    ("LifetimeGrantLandSummaryLine", "lifetimeGrantLand", "lifetime grant land", "lifetime_grant_land_summary_line",
     "lab.Ledger.Lifetime.Grants = 125_000;\n            lab.Ledger.Lifetime.Land = -89_000;", "125,000", "89,000"),
]

SPRINT62 = [
    ("LastWeekRoomGrantSummaryLine", "lastWeekRoomGrant", "last week room grant", "last_week_room_grant_summary_line",
     "lab.Ledger.LastWeek.RoomUpkeep = -2_210;\n            lab.Ledger.LastWeek.Grants = 5_150;", "2,210", "5,150"),
    ("ThisWeekPatentRentSummaryLine", "thisWeekPatentRent", "this week patent rent", "this_week_patent_rent_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_030;\n            lab.Ledger.ThisWeek.Grants = 4_250;", "2,030", "4,250"),
    ("ThisWeekFinesRentSummaryLine", "thisWeekFinesRent", "this week fines rent", "this_week_fines_rent_summary_line",
     "lab.Ledger.ThisWeek.Fines = -490;\n            lab.Ledger.ThisWeek.GpuRent = -1_990;", "490", "1,990"),
    ("LifetimeChipRoomSummaryLine", "lifetimeChipRoom", "lifetime chip room", "lifetime_chip_room_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 235_000;\n            lab.Ledger.Lifetime.RoomUpkeep = -60_000;", "235,000", "60,000"),
    ("LastWeekConstructionFinesSummaryLine", "lastWeekConstructionFines", "last week construction fines", "last_week_construction_fines_summary_line",
     "lab.Ledger.LastWeek.Construction = -6_050;\n            lab.Ledger.LastWeek.Fines = -570;", "6,050", "570"),
    ("LifetimeSalariesGrantSummaryLine", "lifetimeSalariesGrant", "lifetime salaries grant", "lifetime_salaries_grant_summary_line",
     "lab.Ledger.Lifetime.Salaries = -420_000;\n            lab.Ledger.Lifetime.Grants = 126_000;", "420,000", "126,000"),
]

SPRINT63 = [
    ("LastWeekRentGrantSummaryLine", "lastWeekRentGrant", "last week rent grant", "last_week_rent_grant_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_290;\n            lab.Ledger.LastWeek.Grants = 5_200;", "2,290", "5,200"),
    ("ThisWeekLandChipSummaryLine", "thisWeekLandChip", "this week land chip", "this_week_land_chip_summary_line",
     "lab.Ledger.ThisWeek.Land = -11_100;\n            lab.Ledger.ThisWeek.ChipSales = 5_250;", "11,100", "5,250"),
    ("LastWeekGreySiteSummaryLine", "lastWeekGreySite", "last week grey site", "last_week_grey_site_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_400;\n            lab.Ledger.LastWeek.SiteBuild = -7_300;", "2,400", "7,300"),
    ("ThisWeekHardwareLandSummaryLine", "thisWeekHardwareLand", "this week hardware land", "this_week_hardware_land_summary_line",
     "lab.Ledger.ThisWeek.Hardware = -4_280;\n            lab.Ledger.ThisWeek.Land = -10_950;", "4,280", "10,950"),
    ("LastWeekFinesLandSummaryLine", "lastWeekFinesLand", "last week fines land", "last_week_fines_land_summary_line",
     "lab.Ledger.LastWeek.Fines = -580;\n            lab.Ledger.LastWeek.Land = -11_700;", "580", "11,700"),
    ("LastWeekCapitalGrantSummaryLine", "lastWeekCapitalGrant", "last week capital grant", "last_week_capital_grant_summary_line",
     "lab.Ledger.LastWeek.CapitalIn = 42_500;\n            lab.Ledger.LastWeek.Grants = 5_300;", "42,500", "5,300"),
]

SPRINT64 = [
    ("ThisWeekRoomGrantSummaryLine", "thisWeekRoomGrant", "this week room grant", "this_week_room_grant_summary_line",
     "lab.Ledger.ThisWeek.RoomUpkeep = -2_240;\n            lab.Ledger.ThisWeek.Grants = 4_350;", "2,240", "4,350"),
    ("ThisWeekConstructionRentSummaryLine", "thisWeekConstructionRent", "this week construction rent", "this_week_construction_rent_summary_line",
     "lab.Ledger.ThisWeek.Construction = -5_950;\n            lab.Ledger.ThisWeek.GpuRent = -2_040;", "5,950", "2,040"),
    ("ThisWeekSalariesLandSummaryLine", "thisWeekSalariesLand", "this week salaries land", "this_week_salaries_land_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_720;\n            lab.Ledger.ThisWeek.Land = -11_050;", "7,720", "11,050"),
    ("LastWeekNetRoomSummaryLine", "lastWeekNetRoom", "last week net room", "last_week_net_room_summary_line",
     "lab.Ledger.LastWeek.Net = 2_650;\n            lab.Ledger.LastWeek.RoomUpkeep = -2_230;", "2,650", "2,230"),
    ("LifetimeGrantChipSummaryLine", "lifetimeGrantChip", "lifetime grant chip", "lifetime_grant_chip_summary_line",
     "lab.Ledger.Lifetime.Grants = 127_000;\n            lab.Ledger.Lifetime.ChipSales = 238_000;", "127,000", "238,000"),
    ("ThisWeekGrantLaunderSummaryLine", "thisWeekGrantLaunder", "this week grant launder", "this_week_grant_launder_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_400;\n            lab.Ledger.ThisWeek.Laundered = 2_700;", "4,400", "2,700"),
]

PA61 = '            string thisWeekNetRoomPart = string.IsNullOrEmpty(thisWeekNetRoom) ? "" : " · " + thisWeekNetRoom;'
PA62 = '            string lifetimeGrantLandPart = string.IsNullOrEmpty(lifetimeGrantLand) ? "" : " · " + lifetimeGrantLand;'
PA63 = '            string lifetimeSalariesGrantPart = string.IsNullOrEmpty(lifetimeSalariesGrant) ? "" : " · " + lifetimeSalariesGrant;'
PA64 = '            string lastWeekCapitalGrantPart = string.IsNullOrEmpty(lastWeekCapitalGrant) ? "" : " · " + lastWeekCapitalGrant;'


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


insert_formatters(F61_62)
git_commit("feat(hud): sprint61-62 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT61, PA61, "thisWeekNetRoomPart")
run_sprint(SPRINT62, PA62, "lifetimeGrantLandPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F63_64)
git_commit("feat(hud): sprint63-64 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT63, PA63, "lifetimeSalariesGrantPart")
run_sprint(SPRINT64, PA64, "lastWeekCapitalGrantPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)