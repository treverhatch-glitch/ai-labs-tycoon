# Margin call (schema 144)

Weekly after `Loans.RunWeek`, if collateral times `covenantPermille` is below outstanding debt, the player gets `MarginCall` and `SatisfyMarginCall` (pay or sell GPUs) or fast default (Q468). Closes the bible funding-ladder tail in [loans.md](loans.md).