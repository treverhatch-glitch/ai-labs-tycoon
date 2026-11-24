# shellcheck shell=bash
disable_batch23_at_tip() {
  for _b23 in hearingSharePriceEraBoost rungSixEraCooldown dataSourceAcquireEraDiscount; do
    disable_cfg "$_b23"
  done
}