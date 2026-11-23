#!/usr/bin/env python3
from pathlib import Path

src = Path("/tmp/run_sprints_81_84.py").read_text()
head = src[: src.index("F81_82")]
tail = src[src.index("\ndef formatter_body") : src.index("\ninsert_formatters(F81_82)")]

DATA = r"""
F85_86 = [
    ("ThisWeekGreyIncomeSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekGreyIncomeSummaryLine", [
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeGreyIncomeSummaryLine", [
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekFinesIncomeSummaryLine", [
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekFinesIncomeSummaryLine", [
        ("fines", "LastWeekLedgerFinesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeFinesIncomeSummaryLine", [
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekRentIncomeSummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekRentIncomeSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRentIncomeSummaryLine", [
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekGrantIncomeSummaryLine", [
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekGrantIncomeSummaryLine", [
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimeGrantIncomeSummaryLine", [
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
]

F87_88 = [
    ("ThisWeekChipIncomeSummaryLine", [
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekChipIncomeSummaryLine", [
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeChipIncomeSummaryLine", [
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
    ]),
    ("LifetimeSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekPowerIncomeSummaryLine", [
        ("power", "ThisWeekLedgerPowerLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekPowerIncomeSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("LifetimePowerIncomeSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekTrainingIncomeSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekTrainingIncomeSummaryLine", [
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeTrainingIncomeSummaryLine", [
        ("training", "LifetimeLedgerTrainingLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT85 = [
    ("ThisWeekGreyIncomeSummaryLine", "thisWeekGreyIncome", "this week grey income", "this_week_grey_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_020;\n            lab.Ledger.ThisWeek.Revenue = 18_510;", "2,020", "18,510"),
    ("LastWeekGreyIncomeSummaryLine", "lastWeekGreyIncome", "last week grey income", "last_week_grey_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_540;\n            lab.Ledger.LastWeek.Grants = 5_740;", "2,540", "5,740"),
    ("LifetimeGreyIncomeSummaryLine", "lifetimeGreyIncome", "lifetime grey income", "lifetime_grey_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 52_500;\n            lab.Ledger.Lifetime.Grants = 148_000;", "52,500", "148,000"),
    ("ThisWeekFinesIncomeSummaryLine", "thisWeekFinesIncome", "this week fines income", "this_week_fines_income_summary_line",
     "lab.Ledger.ThisWeek.Fines = -650;\n            lab.Ledger.ThisWeek.Net = 3_140;", "650", "3,140"),
    ("LastWeekFinesIncomeSummaryLine", "lastWeekFinesIncome", "last week fines income", "last_week_fines_income_summary_line",
     "lab.Ledger.LastWeek.Fines = -660;\n            lab.Ledger.LastWeek.Net = 2_820;", "660", "2,820"),
    ("LifetimeFinesIncomeSummaryLine", "lifetimeFinesIncome", "lifetime fines income", "lifetime_fines_income_summary_line",
     "lab.Ledger.Lifetime.Fines = -26_200;\n            lab.Ledger.Lifetime.Grants = 149_000;", "26,200", "149,000"),
]

SPRINT86 = [
    ("ThisWeekRentIncomeSummaryLine", "thisWeekRentIncome", "this week rent income", "this_week_rent_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -2_150;\n            lab.Ledger.ThisWeek.Revenue = 18_530;", "2,150", "18,530"),
    ("LastWeekRentIncomeSummaryLine", "lastWeekRentIncome", "last week rent income", "last_week_rent_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -2_400;\n            lab.Ledger.LastWeek.Grants = 5_760;", "2,400", "5,760"),
    ("LifetimeRentIncomeSummaryLine", "lifetimeRentIncome", "lifetime rent income", "lifetime_rent_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 422_000;\n            lab.Ledger.Lifetime.Net = 97_000;", "422,000", "97,000"),
    ("ThisWeekGrantIncomeSummaryLine", "thisWeekGrantIncome", "this week grant income", "this_week_grant_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 4_780;\n            lab.Ledger.ThisWeek.Net = 3_150;", "4,780", "3,150"),
    ("LastWeekGrantIncomeSummaryLine", "lastWeekGrantIncome", "last week grant income", "last_week_grant_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 5_780;\n            lab.Ledger.LastWeek.Revenue = 18_000;", "5,780", "18,000"),
    ("LifetimeGrantIncomeSummaryLine", "lifetimeGrantIncome", "lifetime grant income", "lifetime_grant_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 150_000;\n            lab.Ledger.Lifetime.Net = 98_000;", "150,000", "98,000"),
]

SPRINT87 = [
    ("ThisWeekChipIncomeSummaryLine", "thisWeekChipIncome", "this week chip income", "this_week_chip_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 5_530;\n            lab.Ledger.ThisWeek.Revenue = 18_550;", "5,530", "18,550"),
    ("LastWeekChipIncomeSummaryLine", "lastWeekChipIncome", "last week chip income", "last_week_chip_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_320;\n            lab.Ledger.LastWeek.Grants = 5_800;", "7,320", "5,800"),
    ("LifetimeChipIncomeSummaryLine", "lifetimeChipIncome", "lifetime chip income", "lifetime_chip_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 250_000;\n            lab.Ledger.Lifetime.Grants = 151_000;", "250,000", "151,000"),
    ("ThisWeekSpendIncomeSummaryLine", "thisWeekSpendIncome", "this week spend income", "this_week_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_570;\n            lab.Ledger.ThisWeek.Grants = 4_800;", "18,570", "4,800"),
    ("LastWeekSpendIncomeSummaryLine", "lastWeekSpendIncome", "last week spend income", "last_week_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 2_830;\n            lab.Ledger.LastWeek.Grants = 5_820;", "2,830", "5,820"),
    ("LifetimeSpendIncomeSummaryLine", "lifetimeSpendIncome", "lifetime spend income", "lifetime_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 423_000;\n            lab.Ledger.Lifetime.Grants = 152_000;", "423,000", "152,000"),
]

SPRINT88 = [
    ("ThisWeekPowerIncomeSummaryLine", "thisWeekPowerIncome", "this week power income", "this_week_power_income_summary_line",
     "lab.Ledger.ThisWeek.Power = -2_830;\n            lab.Ledger.ThisWeek.Net = 3_160;", "2,830", "3,160"),
    ("LastWeekPowerIncomeSummaryLine", "lastWeekPowerIncome", "last week power income", "last_week_power_income_summary_line",
     "lab.Ledger.LastWeek.Power = -3_660;\n            lab.Ledger.LastWeek.Revenue = 18_020;", "3,660", "18,020"),
    ("LifetimePowerIncomeSummaryLine", "lifetimePowerIncome", "lifetime power income", "lifetime_power_income_summary_line",
     "lab.Ledger.Lifetime.Power = -160_000;\n            lab.Ledger.Lifetime.Grants = 153_000;", "160,000", "153,000"),
    ("ThisWeekTrainingIncomeSummaryLine", "thisWeekTrainingIncome", "this week training income", "this_week_training_income_summary_line",
     "lab.Ledger.ThisWeek.Training = -9_500;\n            lab.Ledger.ThisWeek.Grants = 4_820;", "9,500", "4,820"),
    ("LastWeekTrainingIncomeSummaryLine", "lastWeekTrainingIncome", "last week training income", "last_week_training_income_summary_line",
     "lab.Ledger.LastWeek.Training = -9_150;\n            lab.Ledger.LastWeek.Grants = 5_840;", "9,150", "5,840"),
    ("LifetimeTrainingIncomeSummaryLine", "lifetimeTrainingIncome", "lifetime training income", "lifetime_training_income_summary_line",
     "lab.Ledger.Lifetime.Training = -154_000;\n            lab.Ledger.Lifetime.Revenue = 424_000;", "154,000", "424,000"),
]

PA85 = '            string lifetimeNetIncomePart = string.IsNullOrEmpty(lifetimeNetIncome) ? "" : " · " + lifetimeNetIncome;'
PA86 = '            string lifetimeFinesIncomePart = string.IsNullOrEmpty(lifetimeFinesIncome) ? "" : " · " + lifetimeFinesIncome;'
PA87 = '            string lifetimeGrantIncomePart = string.IsNullOrEmpty(lifetimeGrantIncome) ? "" : " · " + lifetimeGrantIncome;'
PA88 = '            string lifetimeSpendIncomePart = string.IsNullOrEmpty(lifetimeSpendIncome) ? "" : " · " + lifetimeSpendIncome;'

"""

MAIN = """
insert_formatters(F85_86)
git_commit("feat(hud): sprint85-86 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT85, PA85, "lifetimeNetIncomePart")
run_sprint(SPRINT86, PA86, "lifetimeFinesIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)

insert_formatters(F87_88)
git_commit("feat(hud): sprint87-88 twelve ledger summary formatters", [HUD_PATH])
run_sprint(SPRINT87, PA87, "lifetimeGrantIncomePart")
run_sprint(SPRINT88, PA88, "lifetimeSpendIncomePart")
subprocess.check_call(
    ["dotnet", "test", "tools/SimTests/SimTests.csproj", "--filter", "FullyQualifiedName~golden", "-v", "q"],
    cwd=ROOT,
)
"""

out = head + DATA + tail + MAIN
Path("/tmp/run_sprints_85_88.py").write_text(out)
import py_compile

py_compile.compile("/tmp/run_sprints_85_88.py", doraise=True)
print("wrote", len(out))