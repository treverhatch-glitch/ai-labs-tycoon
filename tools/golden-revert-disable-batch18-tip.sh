# shellcheck shell=bash
# Disable batch-eighteen at tip when golden-reverting an earlier batch (sourced after disable_cfg exists).
disable_batch18_at_tip() {
  for _b18 in specialistEraHooks sectorPricingBreakdown; do
    disable_cfg "$_b18"
  done
  if [ -f "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch19-tip.sh" ]; then
    # shellcheck source=golden-revert-disable-batch19-tip.sh
    source "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch19-tip.sh"
    disable_batch19_at_tip
  fi
}