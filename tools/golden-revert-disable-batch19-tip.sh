# shellcheck shell=bash
disable_batch19_at_tip() {
  for _b19 in dataSourceSpecialistPairing publicMetricsEraLoad; do
    disable_cfg "$_b19"
  done
  if [ -f "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch20-tip.sh" ]; then
    # shellcheck source=golden-revert-disable-batch20-tip.sh
    source "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch20-tip.sh"
    disable_batch20_at_tip
  fi
}