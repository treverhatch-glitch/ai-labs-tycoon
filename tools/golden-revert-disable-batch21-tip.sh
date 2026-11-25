# shellcheck shell=bash
disable_batch21_at_tip() {
  for _b21 in lawShapingEraCooldown dataSourceRenewEraBonus sectorPricingEraRelief; do
    disable_cfg "$_b21"
  done
  if [ -f "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch22-tip.sh" ]; then
    # shellcheck source=golden-revert-disable-batch22-tip.sh
    source "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch22-tip.sh"
    disable_batch22_at_tip
  fi
}