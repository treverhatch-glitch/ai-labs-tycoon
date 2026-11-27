# shellcheck shell=bash
disable_batch24_at_tip() {
  for _b24 in marginSatisfyEraRelief scenarioGoalsEraSlack takeoffCrisisEraGrace; do
    disable_cfg "$_b24"
  done
}