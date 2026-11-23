# shellcheck shell=bash
disable_batch19_at_tip() {
  for _b19 in dataSourceSpecialistPairing publicMetricsEraLoad; do
    disable_cfg "$_b19"
  done
}