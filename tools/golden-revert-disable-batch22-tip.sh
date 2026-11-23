# shellcheck shell=bash
disable_batch22_at_tip() {
  for _b22 in lawTargetEraAmplify specialistBranchEraDiscount marginCallEraGrace; do
    disable_cfg "$_b22"
  done
}