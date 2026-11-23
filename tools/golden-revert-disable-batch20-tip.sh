# shellcheck shell=bash
disable_batch20_at_tip() {
  for _b20 in openWeightsEraThrottle evalHarnessEraBoost campaignDeadlineEraGrace; do
    disable_cfg "$_b20"
  done
  if [ -f "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch21-tip.sh" ]; then
    # shellcheck source=golden-revert-disable-batch21-tip.sh
    source "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch21-tip.sh"
    disable_batch21_at_tip
  fi
}