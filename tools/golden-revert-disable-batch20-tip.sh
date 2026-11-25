# shellcheck shell=bash
disable_batch20_at_tip() {
  for _b20 in openWeightsEraThrottle evalHarnessEraBoost campaignDeadlineEraGrace; do
    disable_cfg "$_b20"
  done
}