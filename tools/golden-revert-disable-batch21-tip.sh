# shellcheck shell=bash
disable_batch21_at_tip() {
  for _b21 in lawShapingEraCooldown dataSourceRenewEraBonus sectorPricingEraRelief; do
    disable_cfg "$_b21"
  done
}