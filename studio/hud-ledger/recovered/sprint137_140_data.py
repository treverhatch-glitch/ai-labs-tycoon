F137_138 = [
    ("ThisWeekNetTrainingSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "ThisWeekLedgerNetLine(lab)"),
        ("training", "ThisWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LastWeekNetTrainingSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("net", "LastWeekLedgerNetLine(lab)"),
        ("training", "LastWeekLedgerTrainingSpendLine(lab)"),
    ]),
    ("LifetimeNetTrainingSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
        ("training", "LifetimeLedgerTrainingLine(lab)"),
    ]),
    ("ThisWeekGpuPowerSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "ThisWeekLedgerGpuRentLine(lab)"),
        ("power", "ThisWeekLedgerPowerLine(lab)"),
    ]),
    ("LastWeekGpuPowerSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("gpu", "LastWeekLedgerGpuRentLine(lab)"),
        ("power", "LastWeekLedgerPowerLine(lab)"),
    ]),
    ("LifetimeGpuPowerSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("upkeep", "LifetimeLedgerGpuUpkeepLine(lab)"),
        ("power", "LifetimeLedgerPowerLine(lab)"),
    ]),
    ("ThisWeekRevenueGrantsSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "ThisWeekLedgerRevenueLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
    ]),
    ("LastWeekRevenueGrantsSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("revenue", "LastWeekLedgerRevenueLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
    ]),
    ("LifetimeRevenueGrantsSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("net", "LifetimeLedgerNetLine(lab)"),
    ]),
    ("ThisWeekConstructionSiteSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "ThisWeekLedgerConstructionLine(lab)"),
        ("site", "ThisWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LastWeekConstructionSiteSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("build", "LastWeekLedgerConstructionLine(lab)"),
        ("site", "LastWeekLedgerSiteBuildLine(lab)"),
    ]),
    ("LifetimeConstructionSiteSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("build", "LifetimeLedgerConstructionLine(lab)"),
        ("site", "LifetimeLedgerSiteBuildLine(lab)"),
    ]),
]

F139_140 = [
    ("ThisWeekGrantDevSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "ThisWeekLedgerGrantsLine(lab)"),
        ("chipDev", "ThisWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LastWeekGrantDevSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grants", "LastWeekLedgerGrantsLine(lab)"),
        ("chipDev", "LastWeekLedgerChipDevelopmentLine(lab)"),
    ]),
    ("LifetimeGrantDevSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
        ("chipDev", "LifetimeLedgerChipDevelopmentLine(lab)"),
    ]),
    ("ThisWeekChipFinesSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "ThisWeekLedgerChipSalesLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekChipFinesSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("chips", "LastWeekLedgerChipSalesLine(lab)"),
        ("fines", "LastWeekLedgerFinesLine(lab)"),
    ]),
    ("LifetimeChipFinesSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("chips", "LifetimeLedgerChipSalesLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
    ]),
    ("ThisWeekShadyGreySpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "ThisWeekLedgerGreyRevenueLine(lab)"),
        ("shady", "ThisWeekLedgerGreyRevenueLine(lab)"),
    ]),
    ("LastWeekShadyGreySpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("grey", "LastWeekLedgerGreyRevenueLine(lab)"),
        ("laundered", "LastWeekLedgerLaunderedLine(lab)"),
    ]),
    ("LifetimeShadyGreySpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("shady", "LifetimeShadyLedgerSummaryLine(lab)"),
        ("grey", "LifetimeLedgerGreyRevenueLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
    ]),
    ("ThisWeekHiringPatentSpendIncomeSummaryLine", [
        ("spend", "ThisWeekLedgerSpendSummaryLine(lab)"),
        ("income", "ThisWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("fines", "ThisWeekLedgerFinesLine(lab)"),
    ]),
    ("LastWeekHiringPatentSpendIncomeSummaryLine", [
        ("spend", "LastWeekLedgerSpendSummaryLine(lab)"),
        ("income", "LastWeekLedgerIncomeSummaryLine(lab)"),
        ("hiring", "LastWeekLedgerHiringLine(lab)"),
        ("patent", "LastWeekLedgerPatentLicenseLine(lab)"),
    ]),
    ("LifetimeHiringPatentSpendIncomeSummaryLine", [
        ("spend", "LifetimeLedgerSpendSummaryLine(lab)"),
        ("revenue", "LifetimeLedgerRevenueLine(lab)"),
        ("fines", "LifetimeLedgerFinesLine(lab)"),
        ("grants", "LifetimeLedgerGrantsLine(lab)"),
    ]),
]

SPRINT137 = [
    ("ThisWeekNetTrainingSpendIncomeSummaryLine", "thisWeekNetTrainingSpendIncome", "this week net training spend income", "this_week_net_training_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Net = 3_490;\n            lab.Ledger.ThisWeek.Training = -910;", "3,490", "910"),
    ("LastWeekNetTrainingSpendIncomeSummaryLine", "lastWeekNetTrainingSpendIncome", "last week net training spend income", "last_week_net_training_spend_income_summary_line",
     "lab.Ledger.LastWeek.Net = 3_500;\n            lab.Ledger.LastWeek.Training = -890;", "3,500", "890"),
    ("LifetimeNetTrainingSpendIncomeSummaryLine", "lifetimeNetTrainingSpendIncome", "lifetime net training spend income", "lifetime_net_training_spend_income_summary_line",
     "lab.Ledger.Lifetime.Net = 122_000;\n            lab.Ledger.Lifetime.Training = -53_000;", "122,000", "53,000"),
    ("ThisWeekGpuPowerSpendIncomeSummaryLine", "thisWeekGpuPowerSpendIncome", "this week gpu power spend income", "this_week_gpu_power_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GpuRent = -1_220;\n            lab.Ledger.ThisWeek.Power = -3_030;", "1,220", "3,030"),
    ("LastWeekGpuPowerSpendIncomeSummaryLine", "lastWeekGpuPowerSpendIncome", "last week gpu power spend income", "last_week_gpu_power_spend_income_summary_line",
     "lab.Ledger.LastWeek.GpuRent = -1_200;\n            lab.Ledger.LastWeek.Power = -2_480;", "1,200", "2,480"),
    ("LifetimeGpuPowerSpendIncomeSummaryLine", "lifetimeGpuPowerSpendIncome", "lifetime gpu power spend income", "lifetime_gpu_power_spend_income_summary_line",
     "lab.Ledger.Lifetime.Power = -172_000;\n            lab.Ledger.Lifetime.Training = -53_500;", "172,000", "53,500"),
]

SPRINT138 = [
    ("ThisWeekRevenueGrantsSpendIncomeSummaryLine", "thisWeekRevenueGrantsSpendIncome", "this week revenue grants spend income", "this_week_revenue_grants_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Revenue = 18_860;\n            lab.Ledger.ThisWeek.Grants = 5_340;", "18,860", "5,340"),
    ("LastWeekRevenueGrantsSpendIncomeSummaryLine", "lastWeekRevenueGrantsSpendIncome", "last week revenue grants spend income", "last_week_revenue_grants_spend_income_summary_line",
     "lab.Ledger.LastWeek.Revenue = 18_420;\n            lab.Ledger.LastWeek.Grants = 6_240;", "18,420", "6,240"),
    ("LifetimeRevenueGrantsSpendIncomeSummaryLine", "lifetimeRevenueGrantsSpendIncome", "lifetime revenue grants spend income", "lifetime_revenue_grants_spend_income_summary_line",
     "lab.Ledger.Lifetime.Revenue = 453_000;\n            lab.Ledger.Lifetime.Grants = 192_000;", "453,000", "192,000"),
    ("ThisWeekConstructionSiteSpendIncomeSummaryLine", "thisWeekConstructionSiteSpendIncome", "this week construction site spend income", "this_week_construction_site_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Construction = -2_230;\n            lab.Ledger.ThisWeek.Net = 3_510;", "2,230", "3,510"),
    ("LastWeekConstructionSiteSpendIncomeSummaryLine", "lastWeekConstructionSiteSpendIncome", "last week construction site spend income", "last_week_construction_site_spend_income_summary_line",
     "lab.Ledger.LastWeek.Construction = -2_210;\n            lab.Ledger.LastWeek.Revenue = 18_430;", "2,210", "18,430"),
    ("LifetimeConstructionSiteSpendIncomeSummaryLine", "lifetimeConstructionSiteSpendIncome", "lifetime construction site spend income", "lifetime_construction_site_spend_income_summary_line",
     "lab.Ledger.Lifetime.Construction = -87_000;\n            lab.Ledger.Lifetime.Net = 122_500;", "87,000", "122,500"),
]

SPRINT139 = [
    ("ThisWeekGrantDevSpendIncomeSummaryLine", "thisWeekGrantDevSpendIncome", "this week grant dev spend income", "this_week_grant_dev_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Grants = 5_350;\n            lab.Ledger.ThisWeek.ChipDevelopment = -4_130;", "5,350", "4,130"),
    ("LastWeekGrantDevSpendIncomeSummaryLine", "lastWeekGrantDevSpendIncome", "last week grant dev spend income", "last_week_grant_dev_spend_income_summary_line",
     "lab.Ledger.LastWeek.Grants = 6_250;\n            lab.Ledger.LastWeek.ChipDevelopment = -4_010;", "6,250", "4,010"),
    ("LifetimeGrantDevSpendIncomeSummaryLine", "lifetimeGrantDevSpendIncome", "lifetime grant dev spend income", "lifetime_grant_dev_spend_income_summary_line",
     "lab.Ledger.Lifetime.Grants = 193_000;\n            lab.Ledger.Lifetime.ChipDevelopment = -93_500;", "193,000", "93,500"),
    ("ThisWeekChipFinesSpendIncomeSummaryLine", "thisWeekChipFinesSpendIncome", "this week chip fines spend income", "this_week_chip_fines_spend_income_summary_line",
     "lab.Ledger.ThisWeek.ChipSales = 8_240;\n            lab.Ledger.ThisWeek.Fines = -1_020;", "8,240", "1,020"),
    ("LastWeekChipFinesSpendIncomeSummaryLine", "lastWeekChipFinesSpendIncome", "last week chip fines spend income", "last_week_chip_fines_spend_income_summary_line",
     "lab.Ledger.LastWeek.ChipSales = 7_930;\n            lab.Ledger.LastWeek.Fines = -1_030;", "7,930", "1,030"),
    ("LifetimeChipFinesSpendIncomeSummaryLine", "lifetimeChipFinesSpendIncome", "lifetime chip fines spend income", "lifetime_chip_fines_spend_income_summary_line",
     "lab.Ledger.Lifetime.ChipSales = 116_000;\n            lab.Ledger.Lifetime.Fines = -28_800;", "116,000", "28,800"),
]

SPRINT140 = [
    ("ThisWeekShadyGreySpendIncomeSummaryLine", "thisWeekShadyGreySpendIncome", "this week shady grey spend income", "this_week_shady_grey_spend_income_summary_line",
     "lab.Ledger.ThisWeek.GreyRevenue = 2_190;\n            lab.Ledger.ThisWeek.Laundered = 3_520;", "2,190", "3,520"),
    ("LastWeekShadyGreySpendIncomeSummaryLine", "lastWeekShadyGreySpendIncome", "last week shady grey spend income", "last_week_shady_grey_spend_income_summary_line",
     "lab.Ledger.LastWeek.GreyRevenue = 2_710;\n            lab.Ledger.LastWeek.Laundered = 3_140;", "2,710", "3,140"),
    ("LifetimeShadyGreySpendIncomeSummaryLine", "lifetimeShadyGreySpendIncome", "lifetime shady grey spend income", "lifetime_shady_grey_spend_income_summary_line",
     "lab.Ledger.Lifetime.GreyRevenue = 53_700;\n            lab.Ledger.Lifetime.Laundered = 33_300;", "53,700", "33,300"),
    ("ThisWeekHiringPatentSpendIncomeSummaryLine", "thisWeekHiringPatentSpendIncome", "this week hiring patent spend income", "this_week_hiring_patent_spend_income_summary_line",
     "lab.Ledger.ThisWeek.Hiring = -610;\n            lab.Ledger.ThisWeek.Fines = -1_040;", "610", "1,040"),
    ("LastWeekHiringPatentSpendIncomeSummaryLine", "lastWeekHiringPatentSpendIncome", "last week hiring patent spend income", "last_week_hiring_patent_spend_income_summary_line",
     "lab.Ledger.LastWeek.Hiring = -600;\n            lab.Ledger.LastWeek.PatentLicense = 4_220;", "600", "4,220"),
    ("LifetimeHiringPatentSpendIncomeSummaryLine", "lifetimeHiringPatentSpendIncome", "lifetime hiring patent spend income", "lifetime_hiring_patent_spend_income_summary_line",
     "lab.Ledger.Lifetime.Hiring = -18_500;\n            lab.Ledger.Lifetime.Grants = 194_000;", "18,500", "194,000"),
]

PA137 = '            string lifetimeGreyFinesSpendIncomePart = string.IsNullOrEmpty(lifetimeGreyFinesSpendIncome) ? "" : " · " + lifetimeGreyFinesSpendIncome;'
PA138 = '            string lifetimeGpuPowerSpendIncomePart = string.IsNullOrEmpty(lifetimeGpuPowerSpendIncome) ? "" : " · " + lifetimeGpuPowerSpendIncome;'
PA139 = '            string lifetimeConstructionSiteSpendIncomePart = string.IsNullOrEmpty(lifetimeConstructionSiteSpendIncome) ? "" : " · " + lifetimeConstructionSiteSpendIncome;'
PA140 = '            string lifetimeChipFinesSpendIncomePart = string.IsNullOrEmpty(lifetimeChipFinesSpendIncome) ? "" : " · " + lifetimeChipFinesSpendIncome;'