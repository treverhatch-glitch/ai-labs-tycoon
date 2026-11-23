#!/usr/bin/env python3
"""Insert sprint 19-20 formatter methods before BankruptLine."""
from pathlib import Path

HUD = Path("/agent/repos/ai-labs-tycoon/unity/Assets/Scripts/Hq/HqDisplayQueriesHud.cs")

FORMATTERS = [
    ("LastWeekHardwareHiringSummaryLine", [
        ("hardware", "LastWeekLedgerHardwareLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("salaries", "LastWeekLedgerSalariesLine(lab)"),
    ]),
    ("ThisWeekRevenueSalariesSummaryLine", [
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("salaries", "ThisWeekLedgerSalariesLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeNetLaunderSummaryLine", [
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("laundered", "LifetimeLedgerLaunderedLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("LastWeekSiteTrainingSummaryLine", [
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekGreyChipSummaryLine", [
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
    ]),
    ("LastWeekPowerNetSummaryLine", [
        ("power", "LastWeekLedgerPowerLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("ThisWeekConstructionGreySummaryLine", [
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekGpuLandSummaryLine", [
        ("gpuRent", "LastWeekLedgerGpuRentLine(lab)"),
        ("land", "LastWeekLedgerLandLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimePowerChipSummaryLine", [
        ("power", "LifetimeLedgerPowerLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekGpuGreySummaryLine", [
        ("gpuRent", "ThisWeekLedgerGpuRentLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekPatentNetSummaryLine", [
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekTrainingGrantSummaryLine", [
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
    ]),
]


def body(name, parts):
    decl = "\n".join(f"            string {v} = {call};" for v, call in parts)
    names = [p[0] for p in parts]
    empty = " && ".join(f"string.IsNullOrEmpty({n})" for n in names)
    first = names[0]
    joins = []
    for n in names[1:]:
        joins.append(
            f"""            if (!string.IsNullOrEmpty({n}))
                line = string.IsNullOrEmpty(line) ? {n} : line + " · " + {n};"""
        )
    join_block = "\n".join(joins)
    return f"""
        public static string {name}(LabState lab)
        {{
{decl}
            if ({empty})
                return "";
            string line = "";
            if (!string.IsNullOrEmpty({first})) line = {first};
{join_block}
            return line;
        }}
"""


def main():
    t = HUD.read_text()
    if "LastWeekHardwareHiringSummaryLine" in t:
        print("formatters already present")
        return
    block = "".join(body(n, p) for n, p in FORMATTERS)
    t = t.replace(
        "        public static string BankruptLine(LabState lab)",
        block + "\n        public static string BankruptLine(LabState lab)",
        1,
    )
    HUD.write_text(t)
    print("inserted 12 formatters")


if __name__ == "__main__":
    main()