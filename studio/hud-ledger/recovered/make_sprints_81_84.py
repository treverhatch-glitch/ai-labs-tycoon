#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_77_80.py").read_text()
head = src[: src.index("F77_78")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F77_78)")]

DATA = r"""
F81_82 = [
    ("ThisWeekSpendPatentSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekSpendPatentSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeSpendPatentSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekLaunderPatentSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekIncomeNetSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekIncomeNetSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeIncomeNetSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendGreySummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekSpendGreySummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeSpendGreySummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekIncomePowerSummaryLine", [
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekIncomeDevSummaryLine", [
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
]

F83_84 = [
    ("LifetimeIncomePowerSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekHiringGrantSummaryLine", [
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekTrainingGrantSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeHardwareGrantSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekConstructionGrantSummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPowerChipSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekDevIncomeSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekDevIncomeSummaryLine", [
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeDevIncomeSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekNetIncomeSummaryLine", [
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekNetIncomeSummaryLine", [
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeNetIncomeSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT81 = [
    ("ThisWeekSpendPatentSummaryLine", "thisWeekSpendPatent", "this week spend patent", "this_week_spend_patent_summary_line",
     "lab.Ledger.ThisWeek.Fines = -620;\n            lab.Ledger.ThisWeek.Grants = 4_700;", "620", "4,700"),
    ("LastWeekSpendPatentSummaryLine", "lastWeekSpendPatent", "last week spend patent", "last_week_spend_patent_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_640;\n            lab.Ledger.LastWeek.Fines = -650;", "5,640", "650"),
    ("LifetimeSpendPatentSummaryLine", "lifetimeSpendPatent", "lifetime spend patent", "lifetime_spend_patent_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_000;\n            lab.Ledger.Lifetime.Grants = 145_000;", "26,000", "145,000"),
    ("ThisWeekLaunderPatentSummaryLine", "thisWeekLaunderPatent", "this week launder patent", "this_week_launder_patent_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 1_980;\n            lab.Ledger.ThisWeek.Fines = -630;", "1,980", "630"),
    ("ThisWeekIncomeNetSummaryLine", "thisWeekIncomeNet", "this week income net", "this_week_income_net_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_120;\n            lab.Ledger.ThisWeek.Revenue = 18_450;", "3,120", "18,450"),
    ("LastWeekIncomeNetSummaryLine", "lastWeekIncomeNet", "last week income net", "last_week_income_net_summary_line",
     "lab.Ledger.LastWeek.Net = 2_780;\n            lab.Ledger.LastWeek.Grants = 5_660;", "2,780", "5,660"),
]

SPRINT82 = [
    ("LifetimeIncomeNetSummaryLine", "lifetimeIncomeNet", "lifetime income net", "lifetime_income_net_summary_line",
     "lab.Ledger.Lifetime.Revenue = 419_000;\n            lab.Ledger.Lifetime.Net = 95_000;", "419,000", "95,000"),
    ("ThisWeekSpendGreySummaryLine", "thisWeekSpendGrey", "this week spend grey", "this_week_spend_grey_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_000;\n            lab.Ledger.ThisWeek.Fines = -640;", "2,000", "640"),
    ("LastWeekSpendGreySummaryLine", "lastWeekSpendGrey", "last week spend grey", "last_week_spend_grey_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_520;\n            lab.Ledger.LastWeek.Laundered = 3_350;", "2,520", "3,350"),
    ("LifetimeSpendGreySummaryLine", "lifetimeSpendGrey", "lifetime spend grey", "lifetime_spend_grey_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_000;\n            lab.Ledger.Lifetime.Fines = -26_100;", "52,000", "26,100"),
    ("ThisWeekIncomePowerSummaryLine", "thisWeekIncomePower", "this week income power", "this_week_income_power_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_820;\n            lab.Ledger.ThisWeek.Revenue = 18_470;", "2,820", "18,470"),
    ("LastWeekIncomeDevSummaryLine", "lastWeekIncomeDev", "last week income dev", "last_week_income_dev_summary_line",
     "lab.Ledger.LastWeek.Revenue = 17_950;\n            lab.Ledger.LastWeek.Grants = 5_680;", "17,950", "5,680"),
]

SPRINT83 = [
    ("LifetimeIncomePowerSummaryLine", "lifetimeIncomePower", "lifetime income power", "lifetime_income_power_summary_line",
     "lab.Ledger.Lifetime.Revenue = 420_000;\n            lab.Ledger.Lifetime.Power = -158_000;", "420,000", "158,000"),
    ("ThisWeekHiringGrantSummaryLine", "thisWeekHiringGrant", "this week hiring grant", "this_week_hiring_grant_summary_line",
     "lab.Ledger.ThisWeek.Salaries = -7_820;\n            lab.Ledger.ThisWeek.Grants = 4_720;", "7,820", "4,720"),
    ("LastWeekTrainingGrantSummaryLine", "lastWeekTrainingGrant", "last week training grant", "last_week_training_grant_summary_line",
     "lab.Ledger.LastWeek.Training = -9_100;\n            lab.Ledger.LastWeek.Grants = 5_700;", "9,100", "5,700"),
    ("LifetimeHardwareGrantSummaryLine", "lifetimeHardwareGrant", "lifetime hardware grant", "lifetime_hardware_grant_summary_line",
     "lab.Ledger.Lifetime.Power = -159_000;\n            lab.Ledger.Lifetime.Grants = 146_000;", "159,000", "146,000"),
    ("ThisWeekConstructionGrantSummaryLine", "thisWeekConstructionGrant", "this week construction grant", "this_week_construction_grant_summary_line",
     "lab.Ledger.ThisWeek.Construction = -6_150;\n            lab.Ledger.ThisWeek.Grants = 4_740;", "6,150", "4,740"),
    ("LastWeekPowerChipSummaryLine", "lastWeekPowerChip", "last week power chip", "last_week_power_chip_summary_line",
     "lab.Ledger.LastWeek.Power = -3_640;\n            lab.Ledger.LastWeek.ChipSales = 7_280;", "3,640", "7,280"),
]

SPRINT84 = [
    ("ThisWeekDevIncomeSummaryLine", "thisWeekDevIncome", "this week dev income", "this_week_dev_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_510;\n            lab.Ledger.ThisWeek.Revenue = 18_490;", "5,510", "18,490"),
    ("LastWeekDevIncomeSummaryLine", "lastWeekDevIncome", "last week dev income", "last_week_dev_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_300;\n            lab.Ledger.LastWeek.Grants = 5_720;", "7,300", "5,720"),
    ("LifetimeDevIncomeSummaryLine", "lifetimeDevIncome", "lifetime dev income", "lifetime_dev_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 249_000;\n            lab.Ledger.Lifetime.Revenue = 421_000;", "249,000", "421,000"),
    ("ThisWeekNetIncomeSummaryLine", "thisWeekNetIncome", "this week net income", "this_week_net_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_130;\n            lab.Ledger.ThisWeek.Grants = 4_760;", "3,130", "4,760"),
    ("LastWeekNetIncomeSummaryLine", "lastWeekNetIncome", "last week net income", "last_week_net_income_summary_line",
     "lab.Ledger.LastWeek.Net = 2_800;\n            lab.Ledger.LastWeek.Revenue = 17_980;", "2,800", "17,980"),
    ("LifetimeNetIncomeSummaryLine", "lifetimeNetIncome", "lifetime net income", "lifetime_net_income_summary_line",
     "lab.Ledger.Lifetime.Net = 96_000;\n            lab.Ledger.Lifetime.Grants = 147_000;", "96,000", "147,000"),
]

PA81 = '            string lifetimeIncomePatentPart = string.IsNullOrEmpty(lifetimeIncomePatent) ? "" : " · " + lifetimeIncomePatent;'
PA82 = '            string lastWeekIncomeNetPart = string.IsNullOrEmpty(lastWeekIncomeNet) ? "" : " · " + lastWeekIncomeNet;'
PA83 = '            string lastWeekIncomeDevPart = string.IsNullOrEmpty(lastWeekIncomeDev) ? "" : " · " + lastWeekIncomeDev;'
PA84 = '            string lastWeekPowerChipPart = string.IsNullOrEmpty(lastWeekPowerChip) ? "" : " · " + lastWeekPowerChip;'

"""

MAIN = """
insert_formatters(F81_82)
git_commit("feat(hud): sprint81-82 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT81, PA81, "lifetimeIncomePatentPart")
run_sprint(SPRINT82, PA82, "lastWeekIncomeNetPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F83_84)
git_commit("feat(hud): sprint83-84 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT83, PA83, "lastWeekIncomeDevPart")
run_sprint(SPRINT84, PA84, "lastWeekPowerChipPart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_81_84.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_81_84.py", doraise=True)
print("wrote", len(out))