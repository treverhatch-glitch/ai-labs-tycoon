# shellcheck shell=bash
# Disable batch-eighteen at tip when golden-reverting an earlier batch (sourced after disable_cfg exists).
disable_batch18_at_tip() {
  for _b18 in specialistEraHooks sectorPricingBreakdown; do
    disable_cfg "$_b18"
  done
}