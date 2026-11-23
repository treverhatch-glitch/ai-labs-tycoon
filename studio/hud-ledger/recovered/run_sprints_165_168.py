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

F165_166 = [
    ("ThisWeekRentGrantSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRentGrantSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("rent", "LastWeekLedgerGpuRentLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRentGrantSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("rent", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekGrantsNetSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantsNetSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeGrantsNetSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPatentSiteSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekPatentSiteSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimePatentSiteSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
    ]),
    ("ThisWeekGreyLandSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("land", "ThisWeekLedgerLandLine(lab)"),
    ]),
    ("LastWeekGreyLandSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
    ]),
    ("LifetimeGreyLandSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("land", "LifetimeLedgerLandLine(lab)"),
    ]),
]

F167_168 = [
    ("ThisWeekConstructionPatentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LastWeekConstructionPatentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LifetimeConstructionPatentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
    ]),
    ("ThisWeekGpuNetSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGpuNetSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeGpuNetSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekPatentPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "ThisWeekLedgerPatentLicenseLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekPatentPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimePatentPowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("patent", "LifetimeLedgerPatentLicenseLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekFinesGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekFinesGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LifetimeFinesGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
    ]),
]

SPRINT165 = [
    ("ThisWeekRentGrantSpendIncomeSummaryLine", "thisWeekRentGrantSpendIncome", "this week rent grant spend income", "this_week_rent_grant_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_270;\n            lab.Ledger.ThisWeek.Grants = 5_580;", "1,270", "5,580"),
    ("LastWeekRentGrantSpendIncomeSummaryLine", "lastWeekRentGrantSpendIncome", "last week rent grant spend income", "last_week_rent_grant_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_250;\n            lab.Ledger.LastWeek.Grants = 6_450;", "1,250", "6,450"),
    ("LifetimeRentGrantSpendIncomeSummaryLine", "lifetimeRentGrantSpendIncome", "lifetime rent grant spend income", "lifetime_rent_grant_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -180_000;\n            lab.Ledger.Lifetime.Grants = 219_000;", "180,000", "219,000"),
    ("ThisWeekGrantsNetSpendIncomeSummaryLine", "thisWeekGrantsNetSpendIncome", "this week grants net spend income", "this_week_grants_net_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_590;\n            lab.Ledger.ThisWeek.Net = 3_660;", "5,590", "3,660"),
    ("LastWeekGrantsNetSpendIncomeSummaryLine", "lastWeekGrantsNetSpendIncome", "last week grants net spend income", "last_week_grants_net_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_460;\n            lab.Ledger.LastWeek.Net = 3_670;", "6,460", "3,670"),
    ("LifetimeGrantsNetSpendIncomeSummaryLine", "lifetimeGrantsNetSpendIncome", "lifetime grants net spend income", "lifetime_grants_net_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 220_000;\n            lab.Ledger.Lifetime.Net = 127_000;", "220,000", "127,000"),
]

SPRINT166 = [
    ("ThisWeekPatentSiteSpendIncomeSummaryLine", "thisWeekPatentSiteSpendIncome", "this week patent site spend income", "this_week_patent_site_spend_income_summary_line",
     "lab.Ledger.ThisWeek.PatentLicense = 4_250;\n            lab.Ledger.ThisWeek.Construction = -2_320;", "4,250", "2,320"),
    ("LastWeekPatentSiteSpendIncomeSummaryLine", "lastWeekPatentSiteSpendIncome", "last week patent site spend income", "last_week_patent_site_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = 4_240;\n            lab.Ledger.LastWeek.Land = -2_850;", "4,240", "2,850"),
    ("LifetimePatentSiteSpendIncomeSummaryLine", "lifetimePatentSiteSpendIncome", "lifetime patent site spend income", "lifetime_patent_site_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 221_000;\n            lab.Ledger.Lifetime.Construction = -92_000;", "221,000", "92,000"),
    ("ThisWeekGreyLandSpendIncomeSummaryLine", "thisWeekGreyLandSpendIncome", "this week grey land spend income", "this_week_grey_land_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_280;\n            lab.Ledger.ThisWeek.Land = -2_860;", "2,280", "2,860"),
    ("LastWeekGreyLandSpendIncomeSummaryLine", "lastWeekGreyLandSpendIncome", "last week grey land spend income", "last_week_grey_land_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_800;\n            lab.Ledger.LastWeek.Land = -2_840;", "2,800", "2,840"),
    ("LifetimeGreyLandSpendIncomeSummaryLine", "lifetimeGreyLandSpendIncome", "lifetime grey land spend income", "lifetime_grey_land_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 55_400;\n            lab.Ledger.Lifetime.Land = -65_000;", "55,400", "65,000"),
]

SPRINT167 = [
    ("ThisWeekConstructionPatentSpendIncomeSummaryLine", "thisWeekConstructionPatentSpendIncome", "this week construction patent spend income", "this_week_construction_patent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_330;\n            lab.Ledger.ThisWeek.PatentLicense = 4_260;", "2,330", "4,260"),
    ("LastWeekConstructionPatentSpendIncomeSummaryLine", "lastWeekConstructionPatentSpendIncome", "last week construction patent spend income", "last_week_construction_patent_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_310;\n            lab.Ledger.LastWeek.PatentLicense = 4_250;", "2,310", "4,250"),
    ("LifetimeConstructionPatentSpendIncomeSummaryLine", "lifetimeConstructionPatentSpendIncome", "lifetime construction patent spend income", "lifetime_construction_patent_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -92_500;\n            lab.Ledger.Lifetime.Grants = 222_000;", "92,500", "222,000"),
    ("ThisWeekGpuNetSpendIncomeSummaryLine", "thisWeekGpuNetSpendIncome", "this week gpu net spend income", "this_week_gpu_net_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_280;\n            lab.Ledger.ThisWeek.Net = 3_680;", "1,280", "3,680"),
    ("LastWeekGpuNetSpendIncomeSummaryLine", "lastWeekGpuNetSpendIncome", "last week gpu net spend income", "last_week_gpu_net_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_260;\n            lab.Ledger.LastWeek.Net = 3_690;", "1,260", "3,690"),
    ("LifetimeGpuNetSpendIncomeSummaryLine", "lifetimeGpuNetSpendIncome", "lifetime gpu net spend income", "lifetime_gpu_net_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -180_500;\n            lab.Ledger.Lifetime.Net = 127_500;", "180,500", "127,500"),
]

SPRINT168 = [
    ("ThisWeekPatentPowerSpendIncomeSummaryLine", "thisWeekPatentPowerSpendIncome", "this week patent power spend income", "this_week_patent_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.PatentLicense = 4_270;\n            lab.Ledger.ThisWeek.Power = -3_100;", "4,270", "3,100"),
    ("LastWeekPatentPowerSpendIncomeSummaryLine", "lastWeekPatentPowerSpendIncome", "last week patent power spend income", "last_week_patent_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.PatentLicense = 4_260;\n            lab.Ledger.LastWeek.Power = -2_550;", "4,260", "2,550"),
    ("LifetimePatentPowerSpendIncomeSummaryLine", "lifetimePatentPowerSpendIncome", "lifetime patent power spend income", "lifetime_patent_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 223_000;\n            lab.Ledger.Lifetime.Power = -181_000;", "223,000", "181,000"),
    ("ThisWeekFinesGreySpendIncomeSummaryLine", "thisWeekFinesGreySpendIncome", "this week fines grey spend income", "this_week_fines_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -1_210;\n            lab.Ledger.ThisWeek.GreyRevenue = 2_290;", "1,210", "2,290"),
    ("LastWeekFinesGreySpendIncomeSummaryLine", "lastWeekFinesGreySpendIncome", "last week fines grey spend income", "last_week_fines_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -1_220;\n            lab.Ledger.LastWeek.GreyRevenue = 2_810;", "1,220", "2,810"),
    ("LifetimeFinesGreySpendIncomeSummaryLine", "lifetimeFinesGreySpendIncome", "lifetime fines grey spend income", "lifetime_fines_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -29_800;\n            lab.Ledger.Lifetime.GreyRevenue = 55_500;", "29,800", "55,500"),
]

PA165 = '            string lifetimeGrantsFinesSpendIncomePart = string.IsNullOrEmpty(lifetimeGrantsFinesSpendIncome) ? "" : " · " + lifetimeGrantsFinesSpendIncome;'
PA166 = '            string lifetimeGrantsNetSpendIncomePart = string.IsNullOrEmpty(lifetimeGrantsNetSpendIncome) ? "" : " · " + lifetimeGrantsNetSpendIncome;'
PA167 = '            string lifetimeGreyLandSpendIncomePart = string.IsNullOrEmpty(lifetimeGreyLandSpendIncome) ? "" : " · " + lifetimeGreyLandSpendIncome;'
PA168 = '            string lifetimeConstructionPatentSpendIncomePart = string.IsNullOrEmpty(lifetimeConstructionPatentSpendIncome) ? "" : " · " + lifetimeConstructionPatentSpendIncome;'

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



insert_formatters(F165_166)
git_commit("feat(hud): sprint165-166 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT165, PA165, "lifetimeGrantsFinesSpendIncome")
run_sprint(SPRINT166, PA166, "lifetimeGrantsNetSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F167_168)
git_commit("feat(hud): sprint167-168 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT167, PA167, "lifetimeGreyLandSpendIncome")
run_sprint(SPRINT168, PA168, "lifetimeConstructionPatentSpendIncome")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
